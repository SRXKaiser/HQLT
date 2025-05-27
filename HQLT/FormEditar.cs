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
using System.IO;
using System.Diagnostics;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;


namespace HQLT
{
    public partial class FormEditar: Form
    {
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();

        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hwnd, int wmsg, int wparam, int lparam);

        const int WM_NCLBUTTONDOWN = 0xA1;
        const int HTCAPTION = 0x2;

        string connectionString = "server=localhost;user id=root;password=Imbecil2.v;database=hqltdb;";
        private string rutaLogo = "";
        private string tipoUsuario;
        public FormEditar(string tipoUsuario)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.tipoUsuario = tipoUsuario;
        }


        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtRFC.Text) ||
                string.IsNullOrWhiteSpace(txtDir.Text) ||
                string.IsNullOrWhiteSpace(txtCorreo.Text) ||
                string.IsNullOrWhiteSpace(txtTelefono.Text))
            {
                MessageBox.Show("Por favor completa todos los campos y selecciona un logotipo.", "Campos incompletos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO configuracion (Nombre_Empresa, RFC, Direccion, Correo, Telefono) " +
                                   "VALUES (@Nombre, @RFC, @Direccion, @Correo, @Telefono)";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Nombre", txtNombre.Text.Trim());
                        cmd.Parameters.AddWithValue("@RFC", txtRFC.Text.Trim());
                        cmd.Parameters.AddWithValue("@Direccion", txtDir.Text.Trim());
                        cmd.Parameters.AddWithValue("@Correo", txtCorreo.Text.Trim());
                        cmd.Parameters.AddWithValue("@Telefono", txtTelefono.Text.Trim());

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Información guardada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        txtNombre.Clear();
                        txtRFC.Clear();
                        txtDir.Clear();
                        txtCorreo.Clear();
                        txtTelefono.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar los datos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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


        private void btnVer_Click(object sender, EventArgs e)
        {
            try
            {
                using (MySqlConnection conexion = new MySqlConnection(connectionString))
                {
                    conexion.Open();
                    MySqlCommand cmd = new MySqlCommand("SELECT * FROM configuracion LIMIT 1", conexion);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        string nombre = reader["Nombre_Empresa"].ToString();
                        string rfc = reader["RFC"].ToString();
                        string direccion = reader["Direccion"].ToString();
                        string telefono = reader["Telefono"].ToString();
                        string correo = reader["Correo"].ToString();

                        string html = $@"
<html>
<body>
    <h1 style='text-align:center;'>Factura</h1>
    <p><b>Fecha:</b> {DateTime.Now.ToString("dd/MM/yyyy")}<br/>
    <b>Número:</b> #0001</p>

    <hr/>

    <p><b>Empresa:</b> {nombre}<br/>
    <b>RFC:</b> {rfc}<br/>
    <b>Dirección:</b> {direccion}<br/>
    <b>Teléfono:</b> {telefono}<br/>
    <b>Correo:</b> {correo}</p>

    <p><b>Facturado a:</b><br/>
    Juan Pérez<br/>
    Av. Siempre Viva 742<br/>
    Ciudad, País</p>

    <table border='1' cellpadding='5' cellspacing='0' width='100%'>
        <tr>
            <th>Descripción</th>
            <th>Cantidad</th>
            <th>Precio Unitario</th>
            <th>Total</th>
        </tr>
        <tr>
            <td>Producto A</td>
            <td>2</td>
            <td>$50.00</td>
            <td>$100.00</td>
        </tr>
        <tr>
            <td>Servicio B</td>
            <td>1</td>
            <td>$120.00</td>
            <td>$120.00</td>
        </tr>
    </table>

    <p style='text-align:right;'><b>Subtotal:</b> $220.00<br/>
    <b>IVA (16%):</b> $35.20<br/>
    <b>Total:</b> $255.20</p>

    <p><b>Notas:</b> Gracias por su compra. Esta factura es válida sin firma ni sello.</p>
</body>
</html>";



                        SaveFileDialog sfd = new SaveFileDialog
                        {
                            Filter = "Archivo PDF|*.pdf",
                            Title = "Guardar Vista Previa de Factura",
                            FileName = $"factura_{DateTime.Now:yyyy-MM-dd}.pdf"
                        };

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
                            {
                                Process.Start(new ProcessStartInfo(sfd.FileName) { UseShellExecute = true });
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("No se encontró información de empresa registrada.", "Sin datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al generar la vista previa: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
