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
using MySql.Data.MySqlClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace HQLT
{
    public partial class FormLogin: Form
    {
        string connectionString = "server=127.0.0.1;port=3306;user id=root;password=Imbecil2.v;database=hqltdb;";
        private bool mostrarContrasena = false;

        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();

        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hwnd, int wmsg, int wparam, int lparam);

        const int WM_NCLBUTTONDOWN = 0xA1;
        const int HTCAPTION = 0x2;

        public FormLogin()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;

        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string entrada = txtUser.Text.Trim();
            string contrasena = txtPass.Text.Trim();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string query = @"
                        SELECT Rol 
                        FROM usuarios 
                        WHERE (Usuario = @entrada OR Correo = @entrada) 
                        AND Contraseña = @contrasena";



                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@entrada", entrada);
                    cmd.Parameters.AddWithValue("@contrasena", contrasena);

                    var tipo = cmd.ExecuteScalar()?.ToString();

                    if (!string.IsNullOrEmpty(tipo))
                    {
                        this.Hide();

                        FormMain main = new FormMain(tipo);
                        main.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show("Usuario/correo o contraseña incorrectos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtUser.Clear();
                        txtPass.Clear();
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al conectar: " + ex.Message);
                }
            }
        }


        private void btnNew_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormRegister registro = new FormRegister();
            registro.ShowDialog();
            this.Show();
        }

        private void pictureView_Click(object sender, EventArgs e)
        {
            mostrarContrasena = !mostrarContrasena;

            txtPass.UseSystemPasswordChar = !mostrarContrasena;  
        }

        private void FormLogin_Load(object sender, EventArgs e)
        {
            txtPass.UseSystemPasswordChar = true;
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

        private void label3_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormRegister registro = new FormRegister();
            registro.ShowDialog();
            this.Show();
        }
    }
}
