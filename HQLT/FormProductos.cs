using MySql.Data.MySqlClient;
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

namespace HQLT
{
    public partial class FormProductos: Form
    {
        string connectionString = "server=127.0.0.1;port=3306;user id=root;password=Imbecil2.v;database=hqltdb;";
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hwnd, int wmsg, int wparam, int lparam);
        const int WM_NCLBUTTONDOWN = 0xA1;
        const int HTCAPTION = 0x2;
        private string tipoUsuario;
        public FormProductos(string tipoUsuario)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.tipoUsuario = tipoUsuario;
        }
        private void FormProductos_Load(object sender, EventArgs e)
        {
            CargarProductos();
            TablaProductos.ReadOnly = true;
            TablaProductos.AllowUserToAddRows = false;
            TablaProductos.AllowUserToDeleteRows = false;
            TablaProductos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            if (tipoUsuario == "Invitado")
            {
                richTxtDesc.ReadOnly = true;
                txtNombre.ReadOnly = true;
                txtCantDisp.ReadOnly = true;
                txtCat.ReadOnly = true;
                txtPrecio.ReadOnly = true;
                txtStockMin.ReadOnly = true;
            }
        }
        private void CargarProductos()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT * FROM productos";
                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    TablaProductos.DataSource = dt;
                    TablaProductos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                    TablaProductos.Columns["Precio_Unitario"].DefaultCellStyle.Format = "C2"; 
                    TablaProductos.Columns["Fecha_Creacion"].DefaultCellStyle.Format = "g";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar productos: " + ex.Message);
            }
        }
        private void LimpiarCampos()
        {
            txtNombre.Clear();
            richTxtDesc.Clear();
            txtPrecio.Clear();
            txtCat.Clear();
            txtCantDisp.Clear();
            txtStockMin.Clear();
        }
        private void btnAgregar_Click(object sender, EventArgs e)
        {
            string nombre = txtNombre.Text.Trim();
            string descripcion = richTxtDesc.Text.Trim();
            string categoria = txtCat.Text.Trim();
            if (tipoUsuario == "Invitado")
            {
                MessageBox.Show("Solo los usuarios 'Normales' y 'Administradores' tienen acceso a esta opción.",
                                "Acceso denegado",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }
            if (nombre == "" || descripcion == "" || txtPrecio.Text.Trim() == "" || categoria == "" ||
                txtCantDisp.Text.Trim() == "" || txtStockMin.Text.Trim() == "")
            {
                MessageBox.Show("Todos los campos son obligatorios.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!float.TryParse(txtPrecio.Text.Trim(), out float precio) || precio <= 0)
            {
                MessageBox.Show("El precio debe ser un número positivo.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!int.TryParse(txtCantDisp.Text.Trim(), out int cantidad) || cantidad < 0)
            {
                MessageBox.Show("La cantidad disponible debe ser un número entero positivo.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!int.TryParse(txtStockMin.Text.Trim(), out int stockMin) || stockMin < 0)
            {
                MessageBox.Show("El stock mínimo debe ser un número entero positivo.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DialogResult confirm = MessageBox.Show("¿Estás seguro de agregar este producto?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes)
                return;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string checkQuery = "SELECT COUNT(*) FROM productos WHERE Nombre = @nombre";
                    MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@nombre", nombre);
                    int existe = Convert.ToInt32(checkCmd.ExecuteScalar());
                    if (existe > 0)
                    {
                        MessageBox.Show("Ya existe un producto con ese nombre.", "Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    string query = @"INSERT INTO productos (Nombre, Descripcion, Precio_Unitario, Categoria, Cantidad_Disponible, Stock_Minimo, Fecha_Creacion)
                             VALUES (@nombre, @desc, @precio, @cat, @cant, @stock, NOW())";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@nombre", nombre);
                    cmd.Parameters.AddWithValue("@desc", descripcion);
                    cmd.Parameters.AddWithValue("@precio", precio);
                    cmd.Parameters.AddWithValue("@cat", categoria);
                    cmd.Parameters.AddWithValue("@cant", cantidad);
                    cmd.Parameters.AddWithValue("@stock", stockMin);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Producto agregado correctamente.");
                    LimpiarCampos();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al agregar producto: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            string nombre = txtNombre.Text.Trim();
            if (tipoUsuario == "Invitado")
            {
                MessageBox.Show("Solo los usuarios 'Normales' y 'Administradores' tienen acceso a esta opción.",
                                "Acceso denegado",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(nombre))
            {
                MessageBox.Show("Ingresa el nombre del producto a eliminar.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DialogResult confirm = MessageBox.Show("¿Estás seguro de eliminar este producto?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm != DialogResult.Yes)
                return;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string checkUso = "SELECT COUNT(*) FROM detalles WHERE id_producto = (SELECT id_producto FROM productos WHERE Nombre = @nombre)";
                    MySqlCommand checkCmd = new MySqlCommand(checkUso, conn);
                    checkCmd.Parameters.AddWithValue("@nombre", nombre);
                    int enUso = Convert.ToInt32(checkCmd.ExecuteScalar());
                    if (enUso > 0)
                    {
                        MessageBox.Show("No se puede eliminar el producto porque está asociado a una factura.", "Operación no permitida", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    string deleteQuery = "DELETE FROM productos WHERE Nombre = @nombre";
                    MySqlCommand deleteCmd = new MySqlCommand(deleteQuery, conn);
                    deleteCmd.Parameters.AddWithValue("@nombre", nombre);
                    int filasAfectadas = deleteCmd.ExecuteNonQuery();
                    if (filasAfectadas > 0)
                        MessageBox.Show("Producto eliminado correctamente.");
                    else
                        MessageBox.Show("Producto no encontrado.");

                    LimpiarCampos();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar producto: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnActualizar_Click(object sender, EventArgs e)
        {
            string nombre = txtNombre.Text.Trim();
            string descripcion = richTxtDesc.Text.Trim();
            string categoria = txtCat.Text.Trim();
            if (tipoUsuario == "Invitado")
            {
                MessageBox.Show("Solo los usuarios 'Normales' y 'Administradores' tienen acceso a esta opción.",
                                "Acceso denegado",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }
            if (nombre == "" || descripcion == "" || txtPrecio.Text.Trim() == "" || categoria == "" ||
                txtCantDisp.Text.Trim() == "" || txtStockMin.Text.Trim() == "")
            {
                MessageBox.Show("Todos los campos son obligatorios.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!float.TryParse(txtPrecio.Text.Trim(), out float precio) || precio <= 0)
            {
                MessageBox.Show("El precio debe ser un número positivo.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!int.TryParse(txtCantDisp.Text.Trim(), out int cantidad) || cantidad < 0)
            {
                MessageBox.Show("La cantidad disponible debe ser un número entero positivo.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!int.TryParse(txtStockMin.Text.Trim(), out int stockMin) || stockMin < 0)
            {
                MessageBox.Show("El stock mínimo debe ser un número entero positivo.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DialogResult confirm = MessageBox.Show("¿Estás seguro de modificar este producto?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes)
                return;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"UPDATE productos SET Descripcion = @desc, Precio_Unitario = @precio, 
                             Categoria = @cat, Cantidad_Disponible = @cant, Stock_Minimo = @stock 
                             WHERE Nombre = @nombre";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@nombre", nombre);
                    cmd.Parameters.AddWithValue("@desc", descripcion);
                    cmd.Parameters.AddWithValue("@precio", precio);
                    cmd.Parameters.AddWithValue("@cat", categoria);
                    cmd.Parameters.AddWithValue("@cant", cantidad);
                    cmd.Parameters.AddWithValue("@stock", stockMin);

                    int rows = cmd.ExecuteNonQuery();
                    if (rows > 0)
                        MessageBox.Show("Producto modificado correctamente.");
                    else
                        MessageBox.Show("Producto no encontrado.");
                    LimpiarCampos();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al modificar producto: " + ex.Message);
            }
        }
        private void btnActTabla_Click(object sender, EventArgs e)
        {
            CargarProductos();
        }
        private void TablaProductos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // Verifica que no sea el encabezado
            {
                DataGridViewRow fila = TablaProductos.Rows[e.RowIndex];
                txtNombre.Text = fila.Cells["Nombre"].Value.ToString();
                richTxtDesc.Text = fila.Cells["Descripcion"].Value.ToString();
                txtPrecio.Text = fila.Cells["Precio_Unitario"].Value.ToString();
                txtCat.Text = fila.Cells["Categoria"].Value.ToString();
                txtCantDisp.Text = fila.Cells["Cantidad_Disponible"].Value.ToString();
                txtStockMin.Text = fila.Cells["Stock_Minimo"].Value.ToString();
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
        private void btnBuscar_Click(object sender, EventArgs e)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox("Ingrese el nombre del producto a buscar:", "Buscar Producto", "");

            if (string.IsNullOrWhiteSpace(input))
            {
                MessageBox.Show("Debes ingresar un nombre válido.", "Campo vacío", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT * FROM productos WHERE Nombre = @nombre LIMIT 1";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@nombre", input);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtNombre.Text = reader["Nombre"].ToString();
                            richTxtDesc.Text = reader["Descripcion"].ToString();
                            txtPrecio.Text = reader["Precio_Unitario"].ToString();
                            txtCat.Text = reader["Categoria"].ToString();
                            txtCantDisp.Text = reader["Cantidad_Disponible"].ToString();
                            txtStockMin.Text = reader["Stock_Minimo"].ToString();

                            foreach (DataGridViewRow row in TablaProductos.Rows)
                            {
                                if (row.Cells["Nombre"].Value?.ToString() == txtNombre.Text)
                                {
                                    row.Selected = true;
                                    TablaProductos.FirstDisplayedScrollingRowIndex = row.Index;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Producto no encontrado.", "Sin resultados", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar producto: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
            FormMain main = new FormMain(tipoUsuario);
            main.Show();
        }
        private void TablaProductos_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
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
