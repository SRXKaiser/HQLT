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
    public partial class FormClientes : Form
    {
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();

        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hwnd, int wmsg, int wparam, int lparam);

        const int WM_NCLBUTTONDOWN = 0xA1;
        const int HTCAPTION = 0x2;
        string connectionString = "server=127.0.0.1;port=3306;user id=root;password=Imbecil2.v;database=hqltdb;";
        private string tipoUsuario;
        public FormClientes(string tipoUsuario)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.tipoUsuario = tipoUsuario;
        }
        private void FormClientes_Load(object sender, EventArgs e)
        {
            CargarClientes();
            TablaClientes.ReadOnly = true;
            TablaClientes.AllowUserToAddRows = false;
            TablaClientes.AllowUserToDeleteRows = false;
            TablaClientes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            if (tipoUsuario == "Invitado")
            {
                txtRFC.ReadOnly = true;
                txtNombre.ReadOnly = true;
                txtApellP.ReadOnly = true;
                txtApellM.ReadOnly = true;
                txtCorreo.ReadOnly = true;
                txtDir.ReadOnly = true;
                txtRazon.ReadOnly = true;

            }
        }
        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(txtRFC.Text) &&
                string.IsNullOrWhiteSpace(txtNombre.Text) &&
                string.IsNullOrWhiteSpace(txtApellP.Text) &&
                string.IsNullOrWhiteSpace(txtApellM.Text) &&
                string.IsNullOrWhiteSpace(txtCorreo.Text) &&
                string.IsNullOrWhiteSpace(txtDir.Text) &&
                string.IsNullOrWhiteSpace(txtRazon.Text))
            {
                MessageBox.Show("Inserta la informacion que desas agregar.", "Campos vacíos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtRFC.Text) ||
                string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtApellP.Text) ||
                string.IsNullOrWhiteSpace(txtApellM.Text) ||
                string.IsNullOrWhiteSpace(txtCorreo.Text) ||
                string.IsNullOrWhiteSpace(txtDir.Text) ||
                string.IsNullOrWhiteSpace(txtRazon.Text))
            {
                MessageBox.Show("Todos los campos son obligatorios.", "Campos vacíos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // formato de correo electronico
            if (!Regex.IsMatch(txtCorreo.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Ingrese un correo electrónico válido.", "Correo inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox("Ingrese nombre, RFC o correo del cliente a buscar:", "Buscar Cliente", "");

            if (string.IsNullOrWhiteSpace(input))
            {
                MessageBox.Show("Debes ingresar un dato válido para buscar.", "Entrada vacía", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (MySqlConnection conexion = new MySqlConnection(connectionString))
                {
                    conexion.Open();
                    string query = @"SELECT * FROM clientes 
                                      WHERE id_cliente = @input OR 
                                            Nombre LIKE CONCAT('%', @input, '%') OR 
                                            RFC LIKE CONCAT('%', @input, '%') OR
                                            Correo LIKE CONCAT('%', @input, '%')";

                    MySqlCommand comando = new MySqlCommand(query, conexion);
                    comando.Parameters.AddWithValue("@input", input);

                    using (MySqlDataReader reader = comando.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtNombre.Text = reader["Nombre"].ToString();
                            txtApellP.Text = reader["Apellido_Paterno"].ToString();
                            txtApellM.Text = reader["Apellido_Materno"].ToString();
                            txtCorreo.Text = reader["Correo"].ToString();
                            txtRFC.Text = reader["RFC"].ToString();
                            txtRazon.Text = reader["Razon_Social"].ToString();
                            txtDir.Text = reader["Direccion"].ToString();
                        }
                        else
                        {
                            MessageBox.Show("Cliente no encontrado.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar cliente: " + ex.Message);
            }
        }

        private void CargarClientes()
        {
            try
            {
                using (MySqlConnection conexion = new MySqlConnection(connectionString))
                {
                    conexion.Open();
                    string query = "SELECT * FROM clientes";
                    MySqlDataAdapter adaptador = new MySqlDataAdapter(query, conexion);
                    DataTable tabla = new DataTable();
                    adaptador.Fill(tabla);

                    TablaClientes.DataSource = tabla;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar clientes: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LimpiarCampos()
        {
            txtRFC.Clear();
            txtNombre.Clear();
            txtApellP.Clear();
            txtApellM.Clear();
            txtCorreo.Clear();
            txtDir.Clear();
            txtRazon.Clear();
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (tipoUsuario == "Invitado")
            {
                MessageBox.Show("Solo los usuarios 'Normales' y 'Administradores' tienen acceso a esta opción.",
                                "Acceso denegado",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }

            if (!ValidarCampos()) return;

            DialogResult resultado = MessageBox.Show(
                "¿Está seguro que desea agregar este cliente?",
                "Confirmación",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Question);

            if (resultado != DialogResult.OK) return;

            try
            {
                using (MySqlConnection conexion = new MySqlConnection(connectionString))
                {
                    conexion.Open();

                    string check1 = "SELECT COUNT(*) FROM clientes WHERE RFC = @rfc OR Correo = @correo";
                    MySqlCommand cmd1 = new MySqlCommand(check1, conexion);
                    cmd1.Parameters.AddWithValue("@rfc", txtRFC.Text.Trim());
                    cmd1.Parameters.AddWithValue("@correo", txtCorreo.Text.Trim());
                    int duplicado1 = Convert.ToInt32(cmd1.ExecuteScalar());

                    if (duplicado1 > 0)
                    {
                        MessageBox.Show("Ya existe un cliente con este RFC o correo.", "Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    string check2 = "SELECT COUNT(*) FROM clientes WHERE Nombre = @nombre AND Apellido_Paterno = @apellidop AND Apellido_Materno = @apellidom";
                    MySqlCommand cmd2 = new MySqlCommand(check2, conexion);
                    cmd2.Parameters.AddWithValue("@nombre", txtNombre.Text.Trim());
                    cmd2.Parameters.AddWithValue("@apellidop", txtApellP.Text.Trim());
                    cmd2.Parameters.AddWithValue("@apellidom", txtApellM.Text.Trim());
                    int duplicado2 = Convert.ToInt32(cmd2.ExecuteScalar());

                    if (duplicado2 > 0)
                    {
                        MessageBox.Show("Ya existe un cliente con el mismo nombre y apellidos.", "Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    string query = @"INSERT INTO clientes (RFC, Nombre, Apellido_Paterno, Apellido_Materno, Correo, Direccion, Razon_Social, Fecha_Registro)
                             VALUES (@rfc, @nombre, @apellidop, @apellidom, @correo, @direccion, @razonsocial, NOW())";

                    MySqlCommand comando = new MySqlCommand(query, conexion);
                    comando.Parameters.AddWithValue("@rfc", txtRFC.Text.Trim());
                    comando.Parameters.AddWithValue("@nombre", txtNombre.Text.Trim());
                    comando.Parameters.AddWithValue("@apellidop", txtApellP.Text.Trim());
                    comando.Parameters.AddWithValue("@apellidom", txtApellM.Text.Trim());
                    comando.Parameters.AddWithValue("@correo", txtCorreo.Text.Trim());
                    comando.Parameters.AddWithValue("@direccion", txtDir.Text.Trim());
                    comando.Parameters.AddWithValue("@razonsocial", txtRazon.Text.Trim());

                    comando.ExecuteNonQuery();
                    MessageBox.Show("Cliente agregado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    CargarClientes();
                    LimpiarCampos();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al agregar cliente: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            if (tipoUsuario == "Invitado")
            {
                MessageBox.Show("Solo los usuarios 'Normales' y 'Administradores' tienen acceso a esta opción.",
                                "Acceso denegado",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }

            if (!ValidarCampos()) return;

            string rfc = txtRFC.Text.Trim();
            if (rfc == "")
            {
                MessageBox.Show("Primero seleccione o escriba el RFC del cliente a modificar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult resultado = MessageBox.Show(
                "¿Está seguro que desea modificar este cliente?",
                "Confirmación",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Question
            );

            if (resultado == DialogResult.OK)
            {
                try
                {
                    using (MySqlConnection conexion = new MySqlConnection(connectionString))
                    {
                        conexion.Open();

                        string checkRFCyCorreo = "SELECT COUNT(*) FROM clientes WHERE (Correo = @correo OR RFC = @nuevoRFC) AND RFC <> @actualRFC";
                        MySqlCommand check1 = new MySqlCommand(checkRFCyCorreo, conexion);
                        check1.Parameters.AddWithValue("@correo", txtCorreo.Text.Trim());
                        check1.Parameters.AddWithValue("@nuevoRFC", rfc);
                        check1.Parameters.AddWithValue("@actualRFC", rfc);

                        if (Convert.ToInt32(check1.ExecuteScalar()) > 0)
                        {
                            MessageBox.Show("Ya existe otro cliente con el mismo RFC o correo.", "Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        //duplicado de nombre completo (distinto RFC)
                        string checkNombre = "SELECT COUNT(*) FROM clientes WHERE Nombre = @nombre AND Apellido_Paterno = @apellP AND Apellido_Materno = @apellM AND RFC <> @actualRFC";
                        MySqlCommand check2 = new MySqlCommand(checkNombre, conexion);
                        check2.Parameters.AddWithValue("@nombre", txtNombre.Text.Trim());
                        check2.Parameters.AddWithValue("@apellP", txtApellP.Text.Trim());
                        check2.Parameters.AddWithValue("@apellM", txtApellM.Text.Trim());
                        check2.Parameters.AddWithValue("@actualRFC", rfc);

                        if (Convert.ToInt32(check2.ExecuteScalar()) > 0)
                        {
                            MessageBox.Show("Ya existe otro cliente con el mismo nombre y apellidos.", "Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        
                        string query = @"UPDATE clientes 
                                 SET Nombre = @nombre, Apellido_Paterno = @apellidop, Apellido_Materno = @apellidom, 
                                     Correo = @correo, Direccion = @direccion, Razon_Social = @razonsocial 
                                 WHERE RFC = @rfc";

                        MySqlCommand comando = new MySqlCommand(query, conexion);
                        comando.Parameters.AddWithValue("@nombre", txtNombre.Text.Trim());
                        comando.Parameters.AddWithValue("@apellidop", txtApellP.Text.Trim());
                        comando.Parameters.AddWithValue("@apellidom", txtApellM.Text.Trim());
                        comando.Parameters.AddWithValue("@correo", txtCorreo.Text.Trim());
                        comando.Parameters.AddWithValue("@direccion", txtDir.Text.Trim());
                        comando.Parameters.AddWithValue("@razonsocial", txtRazon.Text.Trim());
                        comando.Parameters.AddWithValue("@rfc", rfc);

                        int rows = comando.ExecuteNonQuery();

                        if (rows > 0)
                            MessageBox.Show("Cliente actualizado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        else
                            MessageBox.Show("No se encontró el cliente para actualizar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        CargarClientes();
                        LimpiarCampos();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al modificar cliente: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (tipoUsuario == "Invitado")
            {
                MessageBox.Show("Solo los usuarios 'Normales' y 'Administradores' tienen acceso a esta opción.",
                                "Acceso denegado",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }

            if (txtRFC.Text == "")
            {
                MessageBox.Show("Primero seleccione o escriba el RFC del cliente a eliminar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult resultado = MessageBox.Show(
                "¿Está seguro que desea eliminar este cliente?",
                "Confirmación",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Warning
            );

            if (resultado == DialogResult.OK)
            {
                try
                {
                    using (MySqlConnection conexion = new MySqlConnection(connectionString))
                    {
                        conexion.Open();

                        string checkUso = "SELECT COUNT(*) FROM facturas WHERE id_cliente = (SELECT id_cliente FROM clientes WHERE RFC = @rfc)";
                        MySqlCommand checkCmd = new MySqlCommand(checkUso, conexion);
                        checkCmd.Parameters.AddWithValue("@rfc", txtRFC.Text);
                        int enUso = Convert.ToInt32(checkCmd.ExecuteScalar());

                        if (enUso > 0)
                        {
                            MessageBox.Show("No se puede eliminar el cliente porque está asociado a una factura.", "Operación no permitida", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        string query = "DELETE FROM clientes WHERE RFC = @rfc";
                        MySqlCommand comando = new MySqlCommand(query, conexion);
                        comando.Parameters.AddWithValue("@rfc", txtRFC.Text);

                        int filasAfectadas = comando.ExecuteNonQuery();

                        if (filasAfectadas > 0)
                        {
                            MessageBox.Show("Cliente eliminado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            CargarClientes();
                            LimpiarCampos();
                        }
                        else
                        {
                            MessageBox.Show("No se encontró el cliente.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al eliminar cliente: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void TablaClientes_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow fila = TablaClientes.Rows[e.RowIndex];

                txtRFC.Text = fila.Cells["RFC"].Value?.ToString();
                txtNombre.Text = fila.Cells["Nombre"].Value?.ToString();
                txtApellP.Text = fila.Cells["Apellido_Paterno"].Value?.ToString();
                txtApellM.Text = fila.Cells["Apellido_Materno"].Value?.ToString();
                txtCorreo.Text = fila.Cells["Correo"].Value?.ToString();
                txtDir.Text = fila.Cells["Direccion"].Value?.ToString();
                txtRazon.Text = fila.Cells["Razon_Social"].Value?.ToString();
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }

        private void btnActTabla_Click(object sender, EventArgs e)
        {
            CargarClientes();
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
            FormMain main = new FormMain(tipoUsuario);
            main.Show();
        }

        private void TablaClientes_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (tipoUsuario == "Invitado")
            {
                e.Cancel = true;
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
            FormMain main = new FormMain(tipoUsuario);
            main.Show();
        }
    }
}
