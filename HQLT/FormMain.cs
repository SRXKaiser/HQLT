using iTextSharp.text.pdf;
using iTextSharp.text;
using iTextSharp.tool.xml;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace HQLT
{

    public partial class FormMain: Form
    {
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();

        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hwnd, int wmsg, int wparam, int lparam);

        const int WM_NCLBUTTONDOWN = 0xA1;
        const int HTCAPTION = 0x2;
        string connectionString = "server=127.0.0.1;port=3306;user id=root;password=Imbecil2.v;database=hqltdb;";
        private string tipoUsuario;
        ToolTip toolTip = new ToolTip();

        public FormMain(string tipoUsuario)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.tipoUsuario = tipoUsuario;

        }

        private void pbClientes_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormClientes clientes = new FormClientes(tipoUsuario);
            clientes.ShowDialog();
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

        private void pbMin_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void pbUsuarios_Click(object sender, EventArgs e)
        {
            if (tipoUsuario == "Invitado" || tipoUsuario == "Normal")
            {
                MessageBox.Show("Solo los administradores pueden acceder a esta opción.", "Acceso restringido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            this.Hide();
            FormUsuarios usuarios = new FormUsuarios(tipoUsuario);
            usuarios.ShowDialog();
        }

        private void pbProductos_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormProductos productos= new FormProductos(tipoUsuario);
            productos.ShowDialog();
        }

        private void pbFacturas_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormFacturas facturas = new FormFacturas(tipoUsuario);
            facturas.ShowDialog();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            CargarHistorialFacturas();
            MostrarHoraActual();

        }
        private void MostrarHoraActual()
        {
            lblHora.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        }

        private void pbReportes_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormReportes reportes = new FormReportes(tipoUsuario);
            reportes.ShowDialog();
        }

        private void pbEdit_Click(object sender, EventArgs e)
        {
            if (tipoUsuario == "Invitado" || tipoUsuario == "Normal")
            {
                MessageBox.Show("Solo los administradores pueden acceder a esta opción.", "Acceso restringido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            this.Hide();
            FormEditar editar = new FormEditar(tipoUsuario);
            editar.ShowDialog();
        }
        private void CargarHistorialFacturas()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                SELECT f.id_factura AS ID, 
                       c.Nombre AS Cliente, 
                       u.Usuario AS Emisor, 
                       f.Fecha_emision AS Fecha, 
                       f.Total, 
                       f.Metodo_Pago
                FROM facturas f
                JOIN clientes c ON f.id_cliente = c.id_cliente
                JOIN usuarios u ON f.id_usuario = u.id_usuario
                ORDER BY f.Fecha_emision DESC";

                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    DataTable dt = new DataTable(tipoUsuario);
                    adapter.Fill(dt);
                    TablaHistorial.DataSource = dt;
                    TablaHistorial.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                    TablaHistorial.ReadOnly = true;
                    TablaHistorial.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    TablaHistorial.MultiSelect = false;
                    TablaHistorial.AllowUserToAddRows = false;
                    TablaHistorial.AllowUserToDeleteRows = false;
                    TablaHistorial.AllowUserToOrderColumns = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar historial: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            if (TablaHistorial.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecciona una factura del historial.");
                return;
            }

            int idFactura = Convert.ToInt32(TablaHistorial.SelectedRows[0].Cells["ID"].Value);

            FormFacturas form = new FormFacturas(tipoUsuario);
            form.GenerarFacturaPDF(idFactura); 
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
            FormLogin login = new FormLogin();
            login.Show();
        }

        private void timerHoras_Tick(object sender, EventArgs e)
        {
            MostrarHoraActual();
        }

        private void pbUsuarios_MouseHover(object sender, EventArgs e)
        {
            
        }

        private void btnImprimirHistorial_Click(object sender, EventArgs e)
        {
            if (TablaHistorial.Rows.Count == 0)
            {
                MessageBox.Show("No hay facturas para imprimir.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "Archivo PDF|*.pdf";
            saveFile.Title = "Guardar historial de facturas";
            saveFile.FileName = $"HistorialFacturas_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string htmlFilas = "";
                    foreach (DataGridViewRow fila in TablaHistorial.Rows)
                    {
                        htmlFilas += "<tr>";
                        foreach (DataGridViewCell celda in fila.Cells)
                        {
                            htmlFilas += $"<td>{celda.Value?.ToString()}</td>";
                        }
                        htmlFilas += "</tr>";
                    }

                    string htmlEncabezados = "<tr>";
                    foreach (DataGridViewColumn col in TablaHistorial.Columns)
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
    <h1>HISTORIAL DE FACTURAS</h1>
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

                    MessageBox.Show("Historial exportado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Abrir automáticamente
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                    {
                        FileName = saveFile.FileName,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al generar el historial: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

    }
}
