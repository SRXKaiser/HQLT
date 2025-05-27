using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using MySql.Data.MySqlClient;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;


namespace HQLT
{
    public partial class FormReportes: Form
    {
        string connectionString = "server=127.0.0.1;port=3306;user id=root;password=Imbecil2.v;database=hqltdb;";

        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();

        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hwnd, int wmsg, int wparam, int lparam);

        const int WM_NCLBUTTONDOWN = 0xA1;
        const int HTCAPTION = 0x2;
        private string tipoUsuario;

        public FormReportes(string tipoUsuario)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.tipoUsuario = tipoUsuario;
        }

        private void pbMin_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void pbCerrar_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void panelBarra_MouseMove(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
        }

        private void btnBuscarxFecha_Click(object sender, EventArgs e)
        {
            if (dtpDesde.Value > dtpHasta.Value)
            {
                MessageBox.Show("La fecha 'Desde' no puede ser mayor que 'Hasta'.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string desde = dtpDesde.Value.ToString("yyyy-MM-dd");
            string hasta = dtpHasta.Value.ToString("yyyy-MM-dd");

            try
            {
                using (MySqlConnection conexion = new MySqlConnection(connectionString))
                {
                    string query = @"
                SELECT 
                    f.id_factura,
                    f.Fecha_emision,
                    c.Nombre AS Cliente,
                    u.Nombre AS Usuario,
                    f.Metodo_Pago,
                    f.Total
                FROM facturas f
                JOIN clientes c ON f.id_cliente = c.id_cliente
                JOIN usuarios u ON f.id_usuario = u.id_usuario
                WHERE DATE(f.Fecha_emision) BETWEEN @desde AND @hasta";

                    MySqlDataAdapter adaptador = new MySqlDataAdapter(query, conexion);
                    adaptador.SelectCommand.Parameters.AddWithValue("@desde", desde);
                    adaptador.SelectCommand.Parameters.AddWithValue("@hasta", hasta);
                    DataTable tabla = new DataTable();
                    adaptador.Fill(tabla);

                    if (tabla.Rows.Count == 0)
                    {
                        MessageBox.Show("No se encontraron resultados para la búsqueda.", "Sin datos", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    TablaReporte.DataSource = tabla;
                    TablaReporte.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar por fecha: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            LimpiarFiltros();
        }



        private void btnBuscarxCliente_Click(object sender, EventArgs e)
        {
            if (cbClientes.SelectedIndex == -1)
            {
                MessageBox.Show("Por favor selecciona un cliente.", "Campo requerido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string cliente = cbClientes.SelectedItem.ToString();

            try
            {
                using (MySqlConnection conexion = new MySqlConnection(connectionString))
                {
                    string query = @"
        SELECT f.id_factura, f.Fecha_emision, f.Total, f.Metodo_Pago,
        CONCAT(c.Nombre, ' ', c.Apellido_Paterno, ' ', c.Apellido_Materno) AS Cliente 
        FROM facturas f
        JOIN clientes c ON f.id_cliente = c.id_cliente
        WHERE CONCAT(c.Nombre, ' ', c.Apellido_Paterno, ' ', c.Apellido_Materno) = @cliente";


                    MySqlDataAdapter adaptador = new MySqlDataAdapter(query, conexion);
                    adaptador.SelectCommand.Parameters.AddWithValue("@cliente", cliente);
                    DataTable tabla = new DataTable();
                    adaptador.Fill(tabla);

                    if (tabla.Rows.Count == 0)
                    {
                        MessageBox.Show("No se encontraron facturas para este cliente.", "Sin datos", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    TablaReporte.DataSource = tabla;
                    TablaReporte.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar por cliente: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            LimpiarFiltros();
        }


        private void btnBuscarxProducto_Click(object sender, EventArgs e)
        {
            if (cbProductos.SelectedIndex == -1)
            {
                MessageBox.Show("Por favor selecciona un producto.", "Campo requerido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string producto = cbProductos.SelectedItem.ToString();

            try
            {
                using (MySqlConnection conexion = new MySqlConnection(connectionString))
                {
                    string query = @"
                SELECT p.Nombre AS Producto, d.cantidad, d.Subtotal, f.Fecha_emision 
                FROM detalles d
                JOIN productos p ON d.id_producto = p.id_producto
                JOIN facturas f ON d.id_factura = f.id_factura
                WHERE p.Nombre = @producto";

                    MySqlDataAdapter adaptador = new MySqlDataAdapter(query, conexion);
                    adaptador.SelectCommand.Parameters.AddWithValue("@producto", producto);
                    DataTable tabla = new DataTable();

                    adaptador.Fill(tabla);

                    if (tabla.Rows.Count == 0)
                    {
                        MessageBox.Show("No se encontraron ventas para este producto.", "Sin datos", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    TablaReporte.DataSource = tabla;
                    TablaReporte.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar por producto: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            LimpiarFiltros();
        }


        private void btnBuscarxIngreso_Click(object sender, EventArgs e)
        {
            if (cbIngresosx.SelectedIndex == -1)
            {
                MessageBox.Show("Selecciona un cliente para el reporte de ingresos.", "Campo requerido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string cliente = cbIngresosx.SelectedItem.ToString();
            string fecha = dtpIngresos.Value.ToString("yyyy-MM-dd");

            try
            {
                using (MySqlConnection conexion = new MySqlConnection(connectionString))
                {
                    string query = @"
    SELECT CONCAT(c.Nombre, ' ', c.Apellido_Paterno, ' ', c.Apellido_Materno) AS Cliente, 
           DATE(f.Fecha_emision) AS Fecha, 
           ROUND(SUM(f.Total), 4) AS Ingresos
    FROM facturas f
    JOIN clientes c ON f.id_cliente = c.id_cliente
    WHERE CONCAT(c.Nombre, ' ', c.Apellido_Paterno, ' ', c.Apellido_Materno) = @cliente 
          AND DATE(f.Fecha_emision) = @fecha
    GROUP BY Cliente, Fecha";



                    MySqlDataAdapter adaptador = new MySqlDataAdapter(query, conexion);
                    adaptador.SelectCommand.Parameters.AddWithValue("@cliente", cliente);
                    adaptador.SelectCommand.Parameters.AddWithValue("@fecha", fecha);
                    DataTable tabla = new DataTable();
                    adaptador.Fill(tabla);

                    if (tabla.Rows.Count == 0)
                    {
                        MessageBox.Show("No se encontraron ingresos para este cliente en esa fecha.", "Sin datos", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    TablaReporte.DataSource = tabla;
                    TablaReporte.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar ingresos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            LimpiarFiltros();
        }


        private void btnProdVen_Click(object sender, EventArgs e)
        {
            try
            {
                using (MySqlConnection conexion = new MySqlConnection(connectionString))
                {
                    string query = @"
                SELECT p.Nombre AS Producto, SUM(d.cantidad) AS Total_Vendido 
                FROM detalles d
                JOIN productos p ON d.id_producto = p.id_producto
                GROUP BY p.Nombre 
                ORDER BY Total_Vendido DESC 
                LIMIT 10";

                    MySqlDataAdapter adaptador = new MySqlDataAdapter(query, conexion);
                    DataTable tabla = new DataTable();
                    adaptador.Fill(tabla);

                    if (tabla.Rows.Count == 0)
                    {
                        MessageBox.Show("No se encontraron productos vendidos.", "Sin datos", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    TablaReporte.DataSource = tabla;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar productos vendidos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void btnQuery_Click(object sender, EventArgs e)
        {
            if (tipoUsuario == "Administrador")
            {
                this.Hide();
                FormConsultas consulta = new FormConsultas(tipoUsuario);
                consulta.ShowDialog();
            }
            else
            {
                MessageBox.Show("Solo los usuarios con rol de 'Administrador' tienen acceso a esta opción.",
                                "Acceso denegado",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }
            
        }
        

        private void FormReportes_Load_1(object sender, EventArgs e)
        {
            try
            {
                using (MySqlConnection conexion = new MySqlConnection(connectionString))
                {
                    conexion.Open();

                    MySqlCommand comando = new MySqlCommand("SELECT CONCAT(Nombre, ' ', Apellido_Paterno, ' ', Apellido_Materno) AS NombreCompleto FROM clientes", conexion);
                    MySqlDataReader lector = comando.ExecuteReader();
                    while (lector.Read())
                    {
                        string nombreCompleto = lector.GetString("NombreCompleto");
                        cbClientes.Items.Add(nombreCompleto);
                        cbIngresosx.Items.Add(nombreCompleto);
                    }

                    lector.Close();

                    comando.CommandText = "SELECT Nombre FROM productos";
                    lector = comando.ExecuteReader();
                    while (lector.Read())
                    {
                        cbProductos.Items.Add(lector.GetString(0));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar datos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGenerar_Click(object sender, EventArgs e)
        {
            if (TablaReporte.Rows.Count == 0)
            {
                MessageBox.Show("No hay datos en la tabla para generar el reporte.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string timestamp = DateTime.Now.ToString("ddMMyyyy_HHmmss");
            string nombreArchivo = $"Reporte_{timestamp}.pdf";

            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "Archivo PDF|*.pdf";
            saveFile.Title = "Guardar Reporte";
            saveFile.FileName = nombreArchivo;

            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // construccion tabla HTML
                    string htmlFilas = "";
                    foreach (DataGridViewRow fila in TablaReporte.Rows)
                    {
                        htmlFilas += "<tr>";
                        foreach (DataGridViewCell celda in fila.Cells)
                        {
                            htmlFilas += $"<td>{celda.Value?.ToString()}</td>";
                        }
                        htmlFilas += "</tr>";
                    }

                    string htmlEncabezados = "<tr>";
                    foreach (DataGridViewColumn col in TablaReporte.Columns)
                    {
                        htmlEncabezados += $"<th>{col.HeaderText}</th>";
                    }
                    htmlEncabezados += "</tr>";

                    string html = $@"
<html>
<head>
<style>
    body {{ font-family: Arial; }}
    h1 {{ text-align: center; }}
    table {{ width: 100%; border-collapse: collapse; margin-top: 20px; }}
    th, td {{ border: 1px solid black; padding: 5px; text-align: center; }}
    th {{ background-color: #e0e0e0; }}
</style>
</head>
<body>
    <h1>REPORTE DEL SISTEMA</h1>
    <p><b>Generado:</b> {DateTime.Now:dd/MM/yyyy HH:mm:ss}</p>
    <table>
        {htmlEncabezados}
        {htmlFilas}
    </table>
</body>
</html>";

                    using (FileStream fs = new FileStream(saveFile.FileName, FileMode.Create))
                    using (Document doc = new Document(PageSize.A4.Rotate(), 20, 20, 20, 20))
                    using (PdfWriter writer = PdfWriter.GetInstance(doc, fs))
                    {
                        doc.Open();
                        using (StringReader sr = new StringReader(html))
                        {
                            XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, sr);
                        }
                        doc.Close();
                    }

                    MessageBox.Show("Reporte PDF generado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    if (File.Exists(saveFile.FileName))
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                        {
                            FileName = saveFile.FileName,
                            UseShellExecute = true
                        });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al generar el reporte: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void LimpiarFiltros()
        {
            cbClientes.SelectedIndex = -1;
            cbProductos.SelectedIndex = -1;
            cbIngresosx.SelectedIndex = -1;
            dtpDesde.Value = DateTime.Now;
            dtpHasta.Value = DateTime.Now;
            dtpIngresos.Value = DateTime.Now;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
            FormMain main = new FormMain(tipoUsuario);
            main.Show();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
            FormMain main = new FormMain(tipoUsuario);
            main.Show();
        }
    }
}
