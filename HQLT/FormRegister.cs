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

namespace HQLT
{
    public partial class FormRegister : Form
    {
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();

        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hwnd, int wmsg, int wparam, int lparam);

        const int WM_NCLBUTTONDOWN = 0xA1;
        const int HTCAPTION = 0x2;

        string connectionString = "server=localhost;user id=root;password=Imbecil2.v;database=hqltdb;";
        private bool mostrarContrasena = true;

        public FormRegister()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;

        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            string nombre = txtNombre.Text.Trim();
            string apellP = txtApellP.Text.Trim();
            string apellM = txtApellM.Text.Trim();
            string usuario = txtUsuario.Text.Trim();
            string correo = txtCorreo.Text.Trim();
            string correoVerificado = txtVerificar.Text.Trim();
            string contrasena = txtContra.Text.Trim();
            string rol = "";

            // Validaciones
            if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(apellP) ||
                string.IsNullOrWhiteSpace(apellM) || string.IsNullOrWhiteSpace(usuario) ||
                string.IsNullOrWhiteSpace(correo) || string.IsNullOrWhiteSpace(correoVerificado) ||
                string.IsNullOrWhiteSpace(contrasena))
            {
                MessageBox.Show("Por favor llena todos los campos.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(correo, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Formato de correo inválido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (correo != correoVerificado)
            {
                MessageBox.Show("Los correos no coinciden.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int seleccionados = 0;
            if (checkAdmin.Checked) seleccionados++;
            if (checkInvitado.Checked) seleccionados++;
            if (checkNormal.Checked) seleccionados++;

            if (seleccionados != 1)
            {
                MessageBox.Show("Selecciona solo un tipo de usuario.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (checkAdmin.Checked) rol = "Administrador";
            else if (checkInvitado.Checked) rol = "Invitado";
            else if (checkNormal.Checked) rol = "Normal";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // Verificación de duplicados
                    string checkQuery = @"
    SELECT COUNT(*) FROM usuarios
    WHERE Usuario = @usuario 
       OR Correo = @correo 
       OR (Nombre = @nombre AND Apellido_Paterno = @apellP AND Apellido_Materno = @apellM)";

                    MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@usuario", usuario);
                    checkCmd.Parameters.AddWithValue("@correo", correo);
                    checkCmd.Parameters.AddWithValue("@nombre", nombre);
                    checkCmd.Parameters.AddWithValue("@apellP", apellP);
                    checkCmd.Parameters.AddWithValue("@apellM", apellM);
                    int existe = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (existe > 0)
                    {
                        MessageBox.Show("Este usuario ya está registrado con alguno de los datos ingresados.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    string insertQuery = "INSERT INTO usuarios (Nombre, Apellido_Paterno, Apellido_Materno, Usuario, Correo, Contraseña, Rol) " +
                                         "VALUES (@nombre, @apellP, @apellM, @usuario, @correo, @contrasena, @rol)";
                    MySqlCommand cmd = new MySqlCommand(insertQuery, conn);
                    cmd.Parameters.AddWithValue("@nombre", nombre);
                    cmd.Parameters.AddWithValue("@apellP", apellP);
                    cmd.Parameters.AddWithValue("@apellM", apellM);
                    cmd.Parameters.AddWithValue("@usuario", usuario);
                    cmd.Parameters.AddWithValue("@correo", correo);
                    cmd.Parameters.AddWithValue("@contrasena", contrasena);
                    cmd.Parameters.AddWithValue("@rol", rol);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Usuario registrado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Limpiar campos
                    txtNombre.Clear();
                    txtApellP.Clear();
                    txtApellM.Clear();
                    txtUsuario.Clear();
                    txtCorreo.Clear();
                    txtVerificar.Clear();
                    txtContra.Clear();
                    checkAdmin.Checked = false;
                    checkInvitado.Checked = false;
                    checkNormal.Checked = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();

        }

        private void pictureView_Click(object sender, EventArgs e)
        {
            mostrarContrasena = !mostrarContrasena;

            txtContra.UseSystemPasswordChar = !mostrarContrasena;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}