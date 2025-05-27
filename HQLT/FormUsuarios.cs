using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HQLT
{
    public partial class FormUsuarios : Form
    {
        string connectionString = "server=127.0.0.1;port=3306;user id=root;password=Imbecil2.v;database=hqltdb;";
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();

        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hwnd, int wmsg, int wparam, int lparam);

        const int WM_NCLBUTTONDOWN = 0xA1;
        const int HTCAPTION = 0x2;
        private string tipoUsuario;
        public FormUsuarios(string tipoUsuario)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.tipoUsuario = tipoUsuario;
        }
        private void FormUsuarios_Load(object sender, EventArgs e)
        {
            CargarUsuarios();
            TablaUsuarios.ReadOnly = true;
            TablaUsuarios.AllowUserToAddRows = false;
            TablaUsuarios.AllowUserToDeleteRows = false;
            TablaUsuarios.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }
        private void CargarUsuarios()
        {
            try
            {
                using (MySqlConnection conexion = new MySqlConnection(connectionString))
                {
                    conexion.Open();
                    string query = "SELECT * FROM usuarios";
                    MySqlDataAdapter adaptador = new MySqlDataAdapter(query, conexion);
                    DataTable tabla = new DataTable();
                    adaptador.Fill(tabla);
                    TablaUsuarios.DataSource = tabla;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar usuarios: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnAgregar_Click(object sender, EventArgs e)
        {
            string usuario = txtUsuario.Text.Trim();
            string nombre = txtNombre.Text.Trim();
            string apellP = txtApellP.Text.Trim();
            string apellM = txtApellM.Text.Trim();
            string correo = txtCorreo.Text.Trim();
            string pass = txtPass.Text.Trim();
            string rol = "";

            if (usuario == "" || nombre == "" || apellP == "" || apellM == "" || correo == "" || pass == "")
            {
                MessageBox.Show("Todos los campos son obligatorios.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!EsCorreoValido(correo))
            {
                MessageBox.Show("El correo electrónico no tiene un formato válido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int rolesSeleccionados = (checkAdmin.Checked ? 1 : 0) + (checkInvitado.Checked ? 1 : 0) + (checkNormal.Checked ? 1 : 0);
            if (rolesSeleccionados != 1)
            {
                MessageBox.Show("Selecciona solo un tipo de usuario.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            rol = checkAdmin.Checked ? "Administrador" : checkInvitado.Checked ? "Invitado" : "Normal";

            DialogResult confirm = MessageBox.Show("¿Estás seguro de agregar este usuario?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes)
                return;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string checkQuery1 = "SELECT COUNT(*) FROM usuarios WHERE Usuario = @usuario OR Correo = @correo";
                    MySqlCommand checkCmd1 = new MySqlCommand(checkQuery1, conn);
                    checkCmd1.Parameters.AddWithValue("@usuario", usuario);
                    checkCmd1.Parameters.AddWithValue("@correo", correo);
                    int existe1 = Convert.ToInt32(checkCmd1.ExecuteScalar());

                    if (existe1 > 0)
                    {
                        MessageBox.Show("Ya existe un usuario con este nombre de usuario o correo.", "Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    string checkQuery2 = "SELECT COUNT(*) FROM usuarios WHERE Nombre = @nombre AND Apellido_Paterno = @apellP AND Apellido_Materno = @apellM";
                    MySqlCommand checkCmd2 = new MySqlCommand(checkQuery2, conn);
                    checkCmd2.Parameters.AddWithValue("@nombre", nombre);
                    checkCmd2.Parameters.AddWithValue("@apellP", apellP);
                    checkCmd2.Parameters.AddWithValue("@apellM", apellM);
                    int existe2 = Convert.ToInt32(checkCmd2.ExecuteScalar());

                    if (existe2 > 0)
                    {
                        MessageBox.Show("Ya existe un usuario con el mismo nombre y apellidos.", "Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    string query = "INSERT INTO usuarios (Usuario, Nombre, Apellido_Paterno, Apellido_Materno, Correo, Contraseña, Rol) " +
                                   "VALUES (@usuario, @nombre, @apellP, @apellM, @correo, @pass, @rol)";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@usuario", usuario);
                    cmd.Parameters.AddWithValue("@nombre", nombre);
                    cmd.Parameters.AddWithValue("@apellP", apellP);
                    cmd.Parameters.AddWithValue("@apellM", apellM);
                    cmd.Parameters.AddWithValue("@correo", correo);
                    cmd.Parameters.AddWithValue("@pass", pass);
                    cmd.Parameters.AddWithValue("@rol", rol);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Usuario agregado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimpiarCampos();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al agregar usuario: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void btnEliminar_Click(object sender, EventArgs e)
        {
            string usuario = txtUsuario.Text.Trim();
            if (usuario == "")
            {
                MessageBox.Show("Ingresa un usuario para eliminar.");
                return;
            }

            DialogResult res = MessageBox.Show("¿Seguro que deseas eliminar este usuario?", "Confirmar", MessageBoxButtons.YesNo);
            if (res != DialogResult.Yes) return;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    MySqlCommand checkCmd = new MySqlCommand("SELECT COUNT(*) FROM facturas WHERE id_usuario = (SELECT id_usuario FROM usuarios WHERE Usuario = @u)", conn);
                    checkCmd.Parameters.AddWithValue("@u", usuario);
                    int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (count > 0)
                    {
                        MessageBox.Show("No se puede eliminar, el usuario tiene facturas asociadas.");
                        return;
                    }

                    MySqlCommand cmd = new MySqlCommand("DELETE FROM usuarios WHERE Usuario = @u", conn);
                    cmd.Parameters.AddWithValue("@u", usuario);
                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        MessageBox.Show("Usuario eliminado.");
                        LimpiarCampos();
                        CargarUsuarios();
                    }
                    else
                    {
                        MessageBox.Show("Usuario no encontrado.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar: " + ex.Message);
            }
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            string usuario = txtUsuario.Text.Trim();
            string nombre = txtNombre.Text.Trim();
            string apellP = txtApellP.Text.Trim();
            string apellM = txtApellM.Text.Trim();
            string correo = txtCorreo.Text.Trim();
            string pass = txtPass.Text.Trim();
            string rol = "";

            if (usuario == "" || nombre == "" || apellP == "" || apellM == "" || correo == "" || pass == "")
            {
                MessageBox.Show("Todos los campos son obligatorios.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!EsCorreoValido(correo))
            {
                MessageBox.Show("El correo electrónico no tiene un formato válido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int rolesSeleccionados = (checkAdmin.Checked ? 1 : 0) + (checkInvitado.Checked ? 1 : 0) + (checkNormal.Checked ? 1 : 0);
            if (rolesSeleccionados != 1)
            {
                MessageBox.Show("Selecciona solo un tipo de usuario.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            rol = checkAdmin.Checked ? "Administrador" : checkInvitado.Checked ? "Invitado" : "Normal";

            DialogResult confirm = MessageBox.Show("¿Estás seguro de modificar este usuario?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes)
                return;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "UPDATE usuarios SET Nombre = @nombre, Apellido_Paterno = @apellP, Apellido_Materno = @apellM, Correo = @correo, Contraseña = @pass, Rol = @rol WHERE Usuario = @usuario";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@usuario", usuario);
                    cmd.Parameters.AddWithValue("@nombre", nombre);
                    cmd.Parameters.AddWithValue("@apellP", apellP);
                    cmd.Parameters.AddWithValue("@apellM", apellM);
                    cmd.Parameters.AddWithValue("@correo", correo);
                    cmd.Parameters.AddWithValue("@pass", pass);
                    cmd.Parameters.AddWithValue("@rol", rol);

                    int rows = cmd.ExecuteNonQuery();
                    if (rows > 0)
                        MessageBox.Show("Usuario modificado exitosamente.");
                    else
                        MessageBox.Show("Usuario no encontrado.");

                    LimpiarCampos();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al modificar usuario: " + ex.Message);
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
        private bool EsCorreoValido(string correo)
        {
            return Regex.IsMatch(correo, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }
        private void LimpiarCampos()
        {
            txtUsuario.Clear();
            txtNombre.Clear();
            txtApellP.Clear();
            txtApellM.Clear();
            txtCorreo.Clear();
            txtPass.Clear();
            checkAdmin.Checked = false;
            checkInvitado.Checked = false;
            checkNormal.Checked = false;
        }
        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }

        private void btnActTabla_Click(object sender, EventArgs e)
        {
            CargarUsuarios();
        }

        private void TablaUsuarios_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow fila = TablaUsuarios.Rows[e.RowIndex];

                txtUsuario.Text = fila.Cells["Usuario"].Value.ToString();
                txtNombre.Text = fila.Cells["Nombre"].Value.ToString();
                txtApellP.Text = fila.Cells["Apellido_Paterno"].Value.ToString();
                txtApellM.Text = fila.Cells["Apellido_Materno"].Value.ToString();
                txtCorreo.Text = fila.Cells["Correo"].Value.ToString();
                txtPass.Text = fila.Cells["Contraseña"].Value.ToString();

                string rol = fila.Cells["Rol"].Value.ToString();

                checkAdmin.Checked = rol == "Administrador";
                checkInvitado.Checked = rol == "Invitado";
                checkNormal.Checked = rol == "Normal";
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox("Ingrese el nombre, usuario o correo del usuario a buscar:", "Buscar Usuario", "");

            if (string.IsNullOrWhiteSpace(input))
            {
                MessageBox.Show("Debes ingresar un dato válido para buscar.", "Entrada vacía", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"SELECT * FROM usuarios 
                             WHERE Usuario = @input OR Correo = @input OR Nombre = @input 
                             LIMIT 1";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@input", input);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtUsuario.Text = reader["Usuario"].ToString();
                            txtNombre.Text = reader["Nombre"].ToString();
                            txtApellP.Text = reader["Apellido_Paterno"].ToString();
                            txtApellM.Text = reader["Apellido_Materno"].ToString();
                            txtCorreo.Text = reader["Correo"].ToString();
                            txtPass.Text = reader["Contraseña"].ToString();

                            string rol = reader["Rol"].ToString();
                            checkAdmin.Checked = rol == "Administrador";
                            checkNormal.Checked = rol == "Normal";
                            checkInvitado.Checked = rol == "Invitado";

                            foreach (DataGridViewRow row in TablaUsuarios.Rows)
                            {
                                if (row.Cells["Usuario"].Value?.ToString() == txtUsuario.Text)
                                {
                                    row.Selected = true;
                                    TablaUsuarios.FirstDisplayedScrollingRowIndex = row.Index;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("No se encontró un usuario con esos datos.", "Sin resultados", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar usuario: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
