using iTextSharp.text.pdf;
using iTextSharp.text;
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
    public partial class FormConsultas: Form
    {
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();

        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hwnd, int wmsg, int wparam, int lparam);

        const int WM_NCLBUTTONDOWN = 0xA1;
        const int HTCAPTION = 0x2;
        string connectionString = "server=127.0.0.1;port=3306;user id=root;password=Imbecil2.v;database=hqltdb;";
        private string tipoUsuario;
        public FormConsultas(string tipoUsuario)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.tipoUsuario = tipoUsuario;
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

        private void btnEjecutar_Click(object sender, EventArgs e)
        {
            string query = txtQuery.Text.Trim();

            if (string.IsNullOrWhiteSpace(query))
            {
                MessageBox.Show("La consulta no puede estar vacía.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string[] comandosProhibidos = {
        "drop", "truncate", "alter", "shutdown",
        "grant", "revoke", "delete", "update",
        "exec", "call", "--", "/*", "*/",
        "create table", "create" // ?????????????????????????????????????????????
    };

            string lowerQuery = query.ToLower();
            string queryNormalizada = System.Text.RegularExpressions.Regex.Replace(lowerQuery, @"\s+", " ");

            foreach (string comando in comandosProhibidos)
            {
                if (queryNormalizada.Contains(comando))
                {
                    MessageBox.Show($"Comando no permitido: '{comando.ToUpper()}' está restringido por seguridad.",
                                    "Comando Bloqueado", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    if (lowerQuery.StartsWith("select"))
                    {
                        MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        TablaConsulta.DataSource = dt;
                        TablaConsulta.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    }
                    else
                    {
                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        MessageBox.Show($"Consulta ejecutada correctamente. Filas afectadas: {rowsAffected}", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        TablaConsulta.DataSource = null;
                        TablaConsulta.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al ejecutar la consulta:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            string logPath = "historial_consultas.txt";
            string logEntry = $"{DateTime.Now:G} → {query}\n";
            File.AppendAllText(logPath, logEntry);
        }


        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            DialogResult confirm = MessageBox.Show(
                "¿Deseas limpiar la consulta y los resultados?",
                "Confirmar limpieza",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirm == DialogResult.Yes)
            {
                txtQuery.Clear();
                TablaConsulta.DataSource = null;
            }
        }


        private void btnGuardarConsulta_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "Archivo SQL (*.sql)|*.sql|Archivo de texto (*.txt)|*.txt";
            saveFile.Title = "Guardar Consulta";
            saveFile.FileName = "consulta.sql";

            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    File.WriteAllText(saveFile.FileName, txtQuery.Text);
                    MessageBox.Show("Consulta guardada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al guardar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnCargarConsulta_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Archivos SQL (*.sql;*.txt)|*.sql;*.txt";
            openFile.Title = "Cargar Consulta SQL";

            if (openFile.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string contenido = File.ReadAllText(openFile.FileName);
                    txtQuery.Text = contenido;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar el archivo: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnListaNegra_Click(object sender, EventArgs e)
        {
            string comandosBloqueados = "Comandos bloqueados por seguridad:\n" +
                                         "- CREATE / CREATE TABLE\n" +
                                         "- DROP\n" +
                                         "- TRUNCATE\n" +
                                         "- ALTER\n" +
                                         "- SHUTDOWN\n" +
                                         "- GRANT / REVOKE\n" +
                                         "- DELETE / UPDATE\n" +
                                         "- EXEC / CALL\n" +
                                         "- Comentarios (--, /* */)\n" +
                                         "- Separadores ;";

            MessageBox.Show(comandosBloqueados, "Lista Negra de Comandos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }


        private void btnHistorial_Click(object sender, EventArgs e)
        {
            string logPath = "historial_consultas.txt";

            if (!File.Exists(logPath))
            {
                MessageBox.Show("Aún no hay historial de consultas.", "Historial vacío", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string historial = File.ReadAllText(logPath);

            DialogResult generarPDF = MessageBox.Show("¿Deseas generar un PDF con el historial de consultas?",
                                                      "Exportar historial",
                                                      MessageBoxButtons.YesNo,
                                                      MessageBoxIcon.Question);

            if (generarPDF == DialogResult.Yes)
            {
                SaveFileDialog saveFile = new SaveFileDialog
                {
                    Filter = "Archivo PDF|*.pdf",
                    Title = "Guardar historial como PDF",
                    FileName = "Historial_Consultas.pdf"
                };

                if (saveFile.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (FileStream fs = new FileStream(saveFile.FileName, FileMode.Create))
                        using (Document doc = new Document(PageSize.A4, 40f, 40f, 40f, 40f))
                        using (PdfWriter writer = PdfWriter.GetInstance(doc, fs))
                        {
                            doc.Open();

                            Paragraph titulo = new Paragraph("Historial de Consultas", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16))
                            {
                                Alignment = Element.ALIGN_CENTER,
                                SpacingAfter = 20f
                            };
                            doc.Add(titulo);

                            Paragraph contenido = new Paragraph(historial, FontFactory.GetFont(FontFactory.COURIER, 10))
                            {
                                Alignment = Element.ALIGN_LEFT
                            };
                            doc.Add(contenido);

                            doc.Close();
                        }

                        MessageBox.Show("PDF generado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                        {
                            FileName = saveFile.FileName,
                            UseShellExecute = true
                        });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al generar el PDF: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show(historial, "Historial de Consultas", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
            FormReportes reportes = new FormReportes(tipoUsuario);
            reportes.Show();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
            FormMain main = new FormMain(tipoUsuario);
            main.Show();
        }
    }
}
