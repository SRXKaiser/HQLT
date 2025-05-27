using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using System.IO;
using System.Diagnostics;
using iTextSharp.tool.xml;


namespace HQLT
{
    public partial class FormFacturas : Form
    {
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();

        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hwnd, int wmsg, int wparam, int lparam);

        decimal subtotal = 0, iva = 0, total = 0;

        const int WM_NCLBUTTONDOWN = 0xA1;
        const int HTCAPTION = 0x2;

        MySqlConnection connectionString = new MySqlConnection("server=127.0.0.1;port=3306;user id=root;password=Imbecil2.v;database=hqltdb;");
        DataTable detallesTabla = new DataTable();
        private string tipoUsuario;
        public FormFacturas(string tipoUsuario)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            CargarCombos();
            ConfigurarTablaDetalles();
            this.tipoUsuario = tipoUsuario;
        }

        private void CargarCombos()
        {
            CargarCombo("SELECT id_usuario, Usuario FROM usuarios", cbUsuarios, "id_usuario", "Usuario");
            CargarCombo("SELECT id_cliente, CONCAT(Nombre, ' ', Apellido_Paterno) AS NombreCompleto FROM clientes", cbClientes, "id_cliente", "NombreCompleto");
            CargarCombo("SELECT id_producto, Nombre FROM productos", cbProductos, "id_producto", "Nombre");
            cbMetodoPago.Items.Clear();
            cbMetodoPago.Items.AddRange(new string[] { "Efectivo", "Tarjeta", "Transferencia", "Otro" });
            cbMetodoPago.SelectedIndex = 0;
        }

        private void CargarCombo(string query, ComboBox combo, string valueMember, string displayMember)
        {
            try
            {
                MySqlDataAdapter da = new MySqlDataAdapter(query, connectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);
                combo.DataSource = dt;
                combo.ValueMember = valueMember;
                combo.DisplayMember = displayMember;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los datos: " + ex.Message);
            }
        }

        private void ConfigurarTablaDetalles()
        {
            detallesTabla.Columns.Add("id_producto", typeof(int));
            detallesTabla.Columns.Add("Producto", typeof(string));
            detallesTabla.Columns.Add("Cantidad", typeof(int));
            detallesTabla.Columns.Add("Precio_Unitario", typeof(float));
            detallesTabla.Columns.Add("Subtotal", typeof(float));
            detallesTabla.Columns.Add("Metodo Pago", typeof(string));
            TablaDetalles.DataSource = detallesTabla;
        }

        private void FormFacturas_Load(object sender, EventArgs e)
        {
            TablaDetalles.ReadOnly = true;
            TablaDetalles.AllowUserToAddRows = false;
            TablaDetalles.AllowUserToDeleteRows = false;
            TablaDetalles.AllowUserToOrderColumns = true;

            TablaFactura.ReadOnly = true;
            TablaFactura.AllowUserToAddRows = false;
            TablaFactura.AllowUserToDeleteRows = false;
            TablaFactura.AllowUserToOrderColumns = true;

            TablaDetalles.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            TablaFactura.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            if (tipoUsuario == "Invitado")
            {
                cbUsuarios.Enabled = false;
                cbProductos.Enabled = false;
                cbClientes.Enabled = false;
                cbMetodoPago.Enabled = false;
                fechaEmPicker.Enabled = false;
                nudIVA.Enabled = false;
                nudCantidad.Enabled = false;
            }
        }


        private void pbCerrar_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void pbMin_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void panelBarra_MouseMove(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
        }

        private void btnAgregarProducto_Click(object sender, EventArgs e)
        {
            if (tipoUsuario == "Invitado")
            {
                MessageBox.Show("Solo los usuarios 'Normales' y 'Administradores' tienen acceso a esta opción.",
                                "Acceso denegado",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }

            if (cbProductos.SelectedIndex == -1 || nudCantidad.Value <= 0)
            {
                MessageBox.Show("Selecciona un producto y una cantidad válida.");
                return;
            }

            int idProducto = Convert.ToInt32(cbProductos.SelectedValue);
            string nombreProducto = cbProductos.Text;
            int cantidadNueva = Convert.ToInt32(nudCantidad.Value);

            int stockDisponible = ObtenerStockDisponible(idProducto);

            // Sumar cantidades si ya existe
            DataRow filaExistente = detallesTabla.Rows.Cast<DataRow>()
                .FirstOrDefault(row => Convert.ToInt32(row["id_producto"]) == idProducto);

            if (filaExistente != null)
            {
                int cantidadActual = Convert.ToInt32(filaExistente["Cantidad"]);
                int cantidadTotal = cantidadActual + cantidadNueva;

                if (cantidadTotal > stockDisponible)
                {
                    MessageBox.Show($"No hay suficiente stock disponible. Stock actual: {stockDisponible}");
                    return;
                }

                filaExistente["Cantidad"] = cantidadTotal;
                filaExistente["Subtotal"] = cantidadTotal * ObtenerPrecioProducto(idProducto);
            }
            else
            {
                if (cantidadNueva > stockDisponible)
                {
                    MessageBox.Show($"No hay suficiente stock disponible. Stock actual: {stockDisponible}");
                    return;
                }

                float precioUnitario = ObtenerPrecioProducto(idProducto);
                float subtotal = cantidadNueva * precioUnitario;

                detallesTabla.Rows.Add(idProducto, nombreProducto, cantidadNueva, precioUnitario, subtotal);
            }

            ActualizarTotales();
        }

        private int ObtenerStockDisponible(int idProducto)
        {
            int stock = 0;
            try
            {
                MySqlCommand cmd = new MySqlCommand("SELECT Cantidad_Disponible FROM productos WHERE id_producto = @id", connectionString);
                cmd.Parameters.AddWithValue("@id", idProducto);
                connectionString.Open();
                stock = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener stock del producto: " + ex.Message);
            }
            finally
            {
                connectionString.Close();
            }
            return stock;
        }

        private float ObtenerPrecioProducto(int idProducto)
        {
            float precio = 0;
            try
            {
                MySqlCommand cmd = new MySqlCommand("SELECT Precio_Unitario FROM productos WHERE id_producto = @id", connectionString);
                cmd.Parameters.AddWithValue("@id", idProducto);
                connectionString.Open();
                precio = Convert.ToSingle(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener precio del producto: " + ex.Message);
            }
            finally
            {
                connectionString.Close();
            }
            return precio;
        }

        private void ActualizarTotales()
        {
            float subtotal = 0;
            foreach (DataRow row in detallesTabla.Rows)
            {
                subtotal += Convert.ToSingle(row["Subtotal"]);
            }

            float iva = subtotal * (Convert.ToSingle(nudIVA.Value) / 100);
            float total = subtotal + iva;

            lblSubTotal.Text = "SUBTOTAL: " + subtotal.ToString("F2");
            lblIVA.Text = "IVA: " + iva.ToString("F2");
            lblTotal.Text = "TOTAL: " + total.ToString("F2");
        }

        private void btnGuardarFactura_Click(object sender, EventArgs e)
        {
            if (tipoUsuario == "Invitado")
            {
                MessageBox.Show("Solo los usuarios 'Normales' y 'Administradores' tienen acceso a esta opción.",
                                "Acceso denegado",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }
            if (cbUsuarios.SelectedIndex == -1 || cbClientes.SelectedIndex == -1 || detallesTabla.Rows.Count == 0)
            {
                MessageBox.Show("Completa todos los datos antes de guardar la factura.");
                return;
            }

            try
            {
                connectionString.Open();
                MySqlTransaction transaccion = connectionString.BeginTransaction();

                MySqlCommand cmd = new MySqlCommand("INSERT INTO facturas (id_usuario, id_cliente, Fecha_emision, IVA, Total, Metodo_Pago) VALUES (@usuario, @cliente, @fecha, @iva, @total, @metodo); SELECT LAST_INSERT_ID();", connectionString, transaccion);
                cmd.Parameters.AddWithValue("@usuario", cbUsuarios.SelectedValue);
                cmd.Parameters.AddWithValue("@cliente", cbClientes.SelectedValue);
                cmd.Parameters.AddWithValue("@fecha", fechaEmPicker.Value);
                cmd.Parameters.AddWithValue("@iva", nudIVA.Value);
                float total = float.Parse(lblTotal.Text.Replace("TOTAL: ", ""));
                cmd.Parameters.AddWithValue("@total", total);
                cmd.Parameters.AddWithValue("@metodo", cbMetodoPago.Text);
                int idFactura = Convert.ToInt32(cmd.ExecuteScalar());

                foreach (DataRow row in detallesTabla.Rows)
                {
                    int idProducto = Convert.ToInt32(row["id_producto"]);
                    int cantidad = Convert.ToInt32(row["Cantidad"]);

                    // Insertar detalle
                    MySqlCommand cmdDetalle = new MySqlCommand("INSERT INTO detalles (id_factura, id_producto, cantidad, Subtotal) VALUES (@id_factura, @id_producto, @cantidad, @subtotal)", connectionString, transaccion);
                    cmdDetalle.Parameters.AddWithValue("@id_factura", idFactura);
                    cmdDetalle.Parameters.AddWithValue("@id_producto", idProducto);
                    cmdDetalle.Parameters.AddWithValue("@cantidad", cantidad);
                    cmdDetalle.Parameters.AddWithValue("@subtotal", row["Subtotal"]);
                    cmdDetalle.ExecuteNonQuery();

                    // Actualizar stock
                    MySqlCommand cmdUpdateStock = new MySqlCommand("UPDATE productos SET Cantidad_Disponible = Cantidad_Disponible - @cantidad WHERE id_producto = @id_producto", connectionString, transaccion);
                    cmdUpdateStock.Parameters.AddWithValue("@cantidad", cantidad);
                    cmdUpdateStock.Parameters.AddWithValue("@id_producto", idProducto);
                    cmdUpdateStock.ExecuteNonQuery();
                }

                transaccion.Commit();

                MessageBox.Show("Factura guardada exitosamente.");
                detallesTabla.Clear();
                ActualizarTotales();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar: " + ex.Message);
            }
            finally
            {
                if (connectionString.State == ConnectionState.Open)
                    connectionString.Close();
            }
        }


        private void btnBuscarFactura_Click(object sender, EventArgs e)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox("Ingrese el ID de la factura a buscar:", "Buscar Factura", "");
            if (!int.TryParse(input, out int idFactura))
            {
                MessageBox.Show("ID inválido.");
                return;
            }

            try
            {
                connectionString.Open();

                DataTable dtFactura = new DataTable();
                using (MySqlCommand cmdFactura = new MySqlCommand(@"
            SELECT id_factura, id_usuario, id_cliente, Fecha_emision, IVA, Total, Metodo_Pago
            FROM facturas
            WHERE id_factura = @id", connectionString))
                {
                    cmdFactura.Parameters.AddWithValue("@id", idFactura);
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmdFactura))
                    {
                        adapter.Fill(dtFactura);
                    }
                }

                if (dtFactura.Rows.Count == 0)
                {
                    MessageBox.Show("Factura no encontrada.");
                    return;
                }

                TablaFactura.DataSource = dtFactura;
                TablaFactura.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                DataTable dtDetalles = new DataTable();
                using (MySqlCommand cmdDetalles = new MySqlCommand(@"
            SELECT d.id_producto, p.Nombre AS Producto, d.cantidad AS Cantidad, 
                   p.Precio_Unitario, d.Subtotal
            FROM detalles d
            JOIN productos p ON d.id_producto = p.id_producto
            WHERE d.id_factura = @id", connectionString))
                {
                    cmdDetalles.Parameters.AddWithValue("@id", idFactura);
                    using (MySqlDataAdapter adapterDetalles = new MySqlDataAdapter(cmdDetalles))
                    {
                        adapterDetalles.Fill(dtDetalles);
                    }
                }

                TablaDetalles.DataSource = dtDetalles;
                TablaDetalles.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                // Actualizar totales
                float subtotal = 0;
                foreach (DataRow row in dtDetalles.Rows)
                {
                    subtotal += Convert.ToSingle(row["Subtotal"]);
                }
                float iva = subtotal * (Convert.ToSingle(dtFactura.Rows[0]["IVA"]) / 100);
                float total = subtotal + iva;

                lblSubTotal.Text = "SUBTOTAL: " + subtotal.ToString("F2");
                lblIVA.Text = "IVA: " + iva.ToString("F2");
                lblTotal.Text = "TOTAL: " + total.ToString("F2");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar factura: " + ex.Message);
            }
            finally
            {
                if (connectionString.State == ConnectionState.Open)
                    connectionString.Close();
            }
        }




        private void btnEliminarProducto_Click(object sender, EventArgs e)
        {
            if (tipoUsuario == "Invitado")
            {
                MessageBox.Show("Solo los usuarios 'Normales' y 'Administradores' tienen acceso a esta opción.",
                                "Acceso denegado",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }
            if (TablaDetalles.CurrentRow != null)
            {
                TablaDetalles.Rows.RemoveAt(TablaDetalles.CurrentRow.Index);
                ActualizarTotales();
            }
            else
            {
                MessageBox.Show("Selecciona un producto para eliminar.");
            }
        }

        private void btnNuevaFactura_Click(object sender, EventArgs e)
        {
            if (tipoUsuario == "Invitado")
            {
                MessageBox.Show("Solo los usuarios 'Normales' y 'Administradores' tienen acceso a esta opción.",
                                "Acceso denegado",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }
            Limpiar();
        }

        private void Limpiar()
        {
            detallesTabla.Clear();
            cbUsuarios.SelectedIndex = -1;
            cbClientes.SelectedIndex = -1;
            cbProductos.SelectedIndex = -1;
            cbMetodoPago.SelectedIndex = 0;
            nudCantidad.Value = 1;
            nudIVA.Value = 16;
            lblSubTotal.Text = "SUBTOTAL: 0.00";
            lblIVA.Text = "IVA: 0.00";
            lblTotal.Text = "TOTAL: 0.00";
            fechaEmPicker.Value = DateTime.Now;
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("¿Deseas cancelar esta factura?", "Cancelar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                Limpiar();
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
            FormMain main = new FormMain(tipoUsuario);
            main.Show();
        }
        private void btnImprimir_Click(object sender, EventArgs e)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox("Ingrese el ID de la factura a imprimir:", "Imprimir Factura", "");
            if (int.TryParse(input, out int idFactura))
            {
                GenerarFacturaPDF(idFactura);
            }
            else
            {
                MessageBox.Show("ID de factura inválido.");
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
            FormMain main = new FormMain(tipoUsuario);
            main.Show();
        }

        public void GenerarFacturaPDF(int idFactura)
        {

            try
            {
                if (connectionString.State != ConnectionState.Open)
                    connectionString.Open();
                string empresa = "", rfc = "", direccion = "", correo = "", telefono = "";

                MySqlCommand cmdConf = new MySqlCommand("SELECT * FROM configuracion LIMIT 1", connectionString);
                MySqlDataReader readerConf = cmdConf.ExecuteReader();
                if (readerConf.Read())
                {
                    empresa = readerConf["Nombre_Empresa"].ToString();
                    rfc = readerConf["RFC"].ToString();
                    direccion = readerConf["Direccion"].ToString();
                    correo = readerConf["Correo"].ToString();
                    telefono = readerConf["Telefono"].ToString();
                }
                readerConf.Close();

                string fecha = "", usuario = "", ivaStr = "", totalStr = "", metodoPago = "", clienteHTML = "";

                MySqlCommand cmdFactura = new MySqlCommand(@"
            SELECT f.Fecha_emision, f.IVA, f.Total, f.Metodo_Pago,
                   c.Nombre, c.Apellido_Paterno, c.Apellido_Materno, c.Correo, c.Direccion, c.RFC, c.Razon_Social,
                   u.Usuario
            FROM facturas f
            JOIN clientes c ON f.id_cliente = c.id_cliente
            JOIN usuarios u ON f.id_usuario = u.id_usuario
            WHERE f.id_factura = @id", connectionString);
                cmdFactura.Parameters.AddWithValue("@id", idFactura);
                MySqlDataReader readerFactura = cmdFactura.ExecuteReader();
                if (readerFactura.Read())
                {
                    string nombreCliente = readerFactura["Nombre"].ToString();
                    string apePat = readerFactura["Apellido_Paterno"].ToString();
                    string apeMat = readerFactura["Apellido_Materno"].ToString();
                    string correoCliente = readerFactura["Correo"].ToString();
                    string direccionCliente = readerFactura["Direccion"].ToString();
                    string rfcCliente = readerFactura["RFC"].ToString();
                    string razonSocial = readerFactura["Razon_Social"].ToString();

                    clienteHTML = $@"{nombreCliente} {apePat} {apeMat}<br/>
            RFC: {rfcCliente}<br/>
            Razón Social: {razonSocial}<br/>
            Correo: {correoCliente}<br/>
            Dirección: {direccionCliente}";

                    usuario = readerFactura["Usuario"].ToString();
                    fecha = Convert.ToDateTime(readerFactura["Fecha_emision"]).ToString("dd/MM/yyyy");
                    ivaStr = readerFactura["IVA"].ToString();
                    totalStr = readerFactura["Total"].ToString();
                    metodoPago = readerFactura["Metodo_Pago"].ToString();
                }
                readerFactura.Close();

                string htmlFilas = "";
                float subtotalAcumulado = 0;

                MySqlCommand cmdDetalles = new MySqlCommand(@"
            SELECT p.Nombre, d.cantidad, p.Precio_Unitario, d.Subtotal
            FROM detalles d
            JOIN productos p ON d.id_producto = p.id_producto
            WHERE d.id_factura = @id", connectionString);
                cmdDetalles.Parameters.AddWithValue("@id", idFactura);
                MySqlDataReader readerDetalles = cmdDetalles.ExecuteReader();
                while (readerDetalles.Read())
                {
                    string nombreProd = readerDetalles["Nombre"].ToString();
                    int cantidad = Convert.ToInt32(readerDetalles["cantidad"]);
                    float precio = Convert.ToSingle(readerDetalles["Precio_Unitario"]);
                    float sub = Convert.ToSingle(readerDetalles["Subtotal"]);
                    subtotalAcumulado += sub;

                    htmlFilas += $@"
            <tr>
                <td>{nombreProd}</td>
                <td>{cantidad}</td>
                <td>${precio:F2}</td>
                <td>${sub:F2}</td>
            </tr>";
                }
                readerDetalles.Close();

                float ivaCalc = subtotalAcumulado * float.Parse(ivaStr) / 100;

                string html = $@"
<html>
<head>
<style>
    body {{ font-family: Arial; }}
    table {{ width: 100%; border-collapse: collapse; }}
    th, td {{ border: 1px solid black; padding: 5px; text-align: center; }}
</style>
</head>
<body>
    <h1 style='text-align:center;'>Factura</h1>
    <p><b>Fecha:</b> {fecha}<br/>
    <b>Número:</b> #{idFactura}</p>

    <hr/>

    <p><b>Empresa:</b> {empresa}<br/>
    <b>RFC:</b> {rfc}<br/>
    <b>Dirección:</b> {direccion}<br/>
    <b>Teléfono:</b> {telefono}<br/>
    <b>Correo:</b> {correo}</p>

    <p><b>Facturado a:</b><br/>{clienteHTML}</p>

    <table>
        <tr>
            <th>Descripción</th>
            <th>Cantidad</th>
            <th>Precio Unitario</th>
            <th>Total</th>
        </tr>
        {htmlFilas}
    </table>

    <p style='text-align:right;'><b>Subtotal:</b> ${subtotalAcumulado:F2}<br/>
    <b>IVA ({ivaStr}%):</b> ${ivaCalc:F2}<br/>
    <b>Total:</b> ${totalStr}<br/>
    <b>Método de Pago:</b> {metodoPago}</p>

    <p><b>Notas:</b> Gracias por su compra. Esta factura es válida sin firma ni sello.<br/>
    <b>Usuario que generó la factura:</b> {usuario}</p>
</body>
</html>";

                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.Filter = "Archivo PDF|*.pdf";
                    sfd.Title = "Guardar Factura";
                    sfd.FileName = $"factura_{idFactura}_{DateTime.Now:yyyyMMddHHmmss}.pdf";

                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        using (FileStream fs = new FileStream(sfd.FileName, FileMode.Create))
                        using (Document doc = new Document(PageSize.A4, 25, 25, 30, 30))
                        using (PdfWriter writer = PdfWriter.GetInstance(doc, fs))
                        {
                            doc.Open();
                            using (StringReader sr = new StringReader(html))
                            {
                                XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, sr);
                            }
                            doc.Close();
                        }

                        if (File.Exists(sfd.FileName))
                            Process.Start(new ProcessStartInfo(sfd.FileName) { UseShellExecute = true });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al generar PDF: " + ex.Message);
            }
            finally
            {
                if (connectionString.State == ConnectionState.Open)
                    connectionString.Close();
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

    }
}
