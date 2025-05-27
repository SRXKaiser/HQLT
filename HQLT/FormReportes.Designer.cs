namespace HQLT
{
    partial class FormReportes
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormReportes));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panelBarra = new System.Windows.Forms.Panel();
            this.pbMin = new System.Windows.Forms.PictureBox();
            this.pbCerrar = new System.Windows.Forms.PictureBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.TablaReporte = new System.Windows.Forms.DataGridView();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.dtpDesde = new System.Windows.Forms.DateTimePicker();
            this.dtpHasta = new System.Windows.Forms.DateTimePicker();
            this.cbClientes = new System.Windows.Forms.ComboBox();
            this.btnProdVen = new System.Windows.Forms.Button();
            this.btnBuscarxFecha = new System.Windows.Forms.Button();
            this.dtpIngresos = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.cbIngresosx = new System.Windows.Forms.ComboBox();
            this.btnGenerar = new System.Windows.Forms.Button();
            this.btnBuscarxCliente = new System.Windows.Forms.Button();
            this.btnBuscarxIngreso = new System.Windows.Forms.Button();
            this.btnBuscarxProducto = new System.Windows.Forms.Button();
            this.cbProductos = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnQuery = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.panelBarra.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbCerrar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TablaReporte)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // panelBarra
            // 
            this.panelBarra.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.panelBarra.Controls.Add(this.pbMin);
            this.panelBarra.Controls.Add(this.pbCerrar);
            this.panelBarra.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelBarra.Location = new System.Drawing.Point(0, 0);
            this.panelBarra.Name = "panelBarra";
            this.panelBarra.Size = new System.Drawing.Size(1340, 44);
            this.panelBarra.TabIndex = 76;
            this.panelBarra.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelBarra_MouseMove);
            // 
            // pbMin
            // 
            this.pbMin.Image = ((System.Drawing.Image)(resources.GetObject("pbMin.Image")));
            this.pbMin.Location = new System.Drawing.Point(1256, 8);
            this.pbMin.Name = "pbMin";
            this.pbMin.Size = new System.Drawing.Size(35, 29);
            this.pbMin.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbMin.TabIndex = 1;
            this.pbMin.TabStop = false;
            this.pbMin.Click += new System.EventHandler(this.pbMin_Click);
            // 
            // pbCerrar
            // 
            this.pbCerrar.Image = ((System.Drawing.Image)(resources.GetObject("pbCerrar.Image")));
            this.pbCerrar.Location = new System.Drawing.Point(1297, 8);
            this.pbCerrar.Name = "pbCerrar";
            this.pbCerrar.Size = new System.Drawing.Size(31, 29);
            this.pbCerrar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbCerrar.TabIndex = 0;
            this.pbCerrar.TabStop = false;
            this.pbCerrar.Click += new System.EventHandler(this.pbCerrar_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Century Gothic", 19.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(129, 75);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(157, 39);
            this.label9.TabIndex = 129;
            this.label9.Text = "Reportes";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(27, 330);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(205, 23);
            this.label2.TabIndex = 112;
            this.label2.Text = "Facturas por cliente";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(27, 139);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(287, 23);
            this.label1.TabIndex = 110;
            this.label1.Text = "Ventas por rango de fechas";
            // 
            // TablaReporte
            // 
            this.TablaReporte.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.TablaReporte.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.TablaReporte.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.TablaReporte.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            this.TablaReporte.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.TablaReporte.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.LightSeaGreen;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.TablaReporte.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.TablaReporte.ColumnHeadersHeight = 30;
            this.TablaReporte.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.TablaReporte.DefaultCellStyle = dataGridViewCellStyle2;
            this.TablaReporte.EnableHeadersVisualStyles = false;
            this.TablaReporte.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.TablaReporte.Location = new System.Drawing.Point(498, 174);
            this.TablaReporte.Margin = new System.Windows.Forms.Padding(4);
            this.TablaReporte.Name = "TablaReporte";
            this.TablaReporte.RowHeadersVisible = false;
            this.TablaReporte.RowHeadersWidth = 51;
            this.TablaReporte.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.TablaReporte.Size = new System.Drawing.Size(793, 514);
            this.TablaReporte.TabIndex = 108;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(57, 174);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(78, 23);
            this.label5.TabIndex = 130;
            this.label5.Text = "Desde:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(57, 243);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(72, 23);
            this.label6.TabIndex = 131;
            this.label6.Text = "Hasta:";
            // 
            // dtpDesde
            // 
            this.dtpDesde.Location = new System.Drawing.Point(77, 209);
            this.dtpDesde.Name = "dtpDesde";
            this.dtpDesde.Size = new System.Drawing.Size(267, 22);
            this.dtpDesde.TabIndex = 132;
            this.dtpDesde.Value = new System.DateTime(2025, 5, 17, 16, 4, 42, 0);
            // 
            // dtpHasta
            // 
            this.dtpHasta.CalendarFont = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpHasta.Location = new System.Drawing.Point(77, 280);
            this.dtpHasta.Name = "dtpHasta";
            this.dtpHasta.Size = new System.Drawing.Size(267, 22);
            this.dtpHasta.TabIndex = 133;
            this.dtpHasta.Value = new System.DateTime(2025, 5, 17, 16, 4, 38, 0);
            // 
            // cbClientes
            // 
            this.cbClientes.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbClientes.FormattingEnabled = true;
            this.cbClientes.Location = new System.Drawing.Point(61, 369);
            this.cbClientes.Name = "cbClientes";
            this.cbClientes.Size = new System.Drawing.Size(283, 31);
            this.cbClientes.TabIndex = 134;
            // 
            // btnProdVen
            // 
            this.btnProdVen.Font = new System.Drawing.Font("Century Gothic", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnProdVen.Location = new System.Drawing.Point(498, 711);
            this.btnProdVen.Name = "btnProdVen";
            this.btnProdVen.Size = new System.Drawing.Size(133, 56);
            this.btnProdVen.TabIndex = 135;
            this.btnProdVen.Text = "Productos mas Vendidos";
            this.btnProdVen.UseVisualStyleBackColor = true;
            this.btnProdVen.Click += new System.EventHandler(this.btnProdVen_Click);
            // 
            // btnBuscarxFecha
            // 
            this.btnBuscarxFecha.Font = new System.Drawing.Font("Century Gothic", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBuscarxFecha.Location = new System.Drawing.Point(381, 236);
            this.btnBuscarxFecha.Name = "btnBuscarxFecha";
            this.btnBuscarxFecha.Size = new System.Drawing.Size(87, 41);
            this.btnBuscarxFecha.TabIndex = 136;
            this.btnBuscarxFecha.Text = "Buscar";
            this.btnBuscarxFecha.UseVisualStyleBackColor = true;
            this.btnBuscarxFecha.Click += new System.EventHandler(this.btnBuscarxFecha_Click);
            // 
            // dtpIngresos
            // 
            this.dtpIngresos.Location = new System.Drawing.Point(61, 622);
            this.dtpIngresos.Name = "dtpIngresos";
            this.dtpIngresos.Size = new System.Drawing.Size(283, 22);
            this.dtpIngresos.TabIndex = 138;
            this.dtpIngresos.Value = new System.DateTime(2025, 5, 17, 16, 4, 33, 0);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(29, 495);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(203, 23);
            this.label3.TabIndex = 137;
            this.label3.Text = "Reporte de ingresos";
            // 
            // cbIngresosx
            // 
            this.cbIngresosx.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbIngresosx.FormattingEnabled = true;
            this.cbIngresosx.Location = new System.Drawing.Point(61, 566);
            this.cbIngresosx.Name = "cbIngresosx";
            this.cbIngresosx.Size = new System.Drawing.Size(283, 31);
            this.cbIngresosx.TabIndex = 139;
            // 
            // btnGenerar
            // 
            this.btnGenerar.Font = new System.Drawing.Font("Century Gothic", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGenerar.Location = new System.Drawing.Point(1171, 711);
            this.btnGenerar.Name = "btnGenerar";
            this.btnGenerar.Size = new System.Drawing.Size(120, 56);
            this.btnGenerar.TabIndex = 140;
            this.btnGenerar.Text = "Generar Reporte";
            this.btnGenerar.UseVisualStyleBackColor = true;
            this.btnGenerar.Click += new System.EventHandler(this.btnGenerar_Click);
            // 
            // btnBuscarxCliente
            // 
            this.btnBuscarxCliente.Font = new System.Drawing.Font("Century Gothic", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBuscarxCliente.Location = new System.Drawing.Point(381, 367);
            this.btnBuscarxCliente.Name = "btnBuscarxCliente";
            this.btnBuscarxCliente.Size = new System.Drawing.Size(87, 36);
            this.btnBuscarxCliente.TabIndex = 141;
            this.btnBuscarxCliente.Text = "Buscar";
            this.btnBuscarxCliente.UseVisualStyleBackColor = true;
            this.btnBuscarxCliente.Click += new System.EventHandler(this.btnBuscarxCliente_Click);
            // 
            // btnBuscarxIngreso
            // 
            this.btnBuscarxIngreso.Font = new System.Drawing.Font("Century Gothic", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBuscarxIngreso.Location = new System.Drawing.Point(381, 532);
            this.btnBuscarxIngreso.Name = "btnBuscarxIngreso";
            this.btnBuscarxIngreso.Size = new System.Drawing.Size(87, 41);
            this.btnBuscarxIngreso.TabIndex = 142;
            this.btnBuscarxIngreso.Text = "Buscar";
            this.btnBuscarxIngreso.UseVisualStyleBackColor = true;
            this.btnBuscarxIngreso.Click += new System.EventHandler(this.btnBuscarxIngreso_Click);
            // 
            // btnBuscarxProducto
            // 
            this.btnBuscarxProducto.Font = new System.Drawing.Font("Century Gothic", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBuscarxProducto.Location = new System.Drawing.Point(381, 448);
            this.btnBuscarxProducto.Name = "btnBuscarxProducto";
            this.btnBuscarxProducto.Size = new System.Drawing.Size(87, 36);
            this.btnBuscarxProducto.TabIndex = 145;
            this.btnBuscarxProducto.Text = "Buscar";
            this.btnBuscarxProducto.UseVisualStyleBackColor = true;
            this.btnBuscarxProducto.Click += new System.EventHandler(this.btnBuscarxProducto_Click);
            // 
            // cbProductos
            // 
            this.cbProductos.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbProductos.FormattingEnabled = true;
            this.cbProductos.Location = new System.Drawing.Point(61, 450);
            this.cbProductos.Name = "cbProductos";
            this.cbProductos.Size = new System.Drawing.Size(283, 31);
            this.cbProductos.TabIndex = 144;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(27, 411);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(231, 23);
            this.label4.TabIndex = 143;
            this.label4.Text = "Facturas por producto";
            // 
            // btnQuery
            // 
            this.btnQuery.Font = new System.Drawing.Font("Century Gothic", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnQuery.Location = new System.Drawing.Point(1022, 711);
            this.btnQuery.Name = "btnQuery";
            this.btnQuery.Size = new System.Drawing.Size(120, 56);
            this.btnQuery.TabIndex = 146;
            this.btnQuery.Text = "Consultas Avanzadas";
            this.btnQuery.UseVisualStyleBackColor = true;
            this.btnQuery.Click += new System.EventHandler(this.btnQuery_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(57, 532);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(193, 23);
            this.label7.TabIndex = 147;
            this.label7.Text = "Seleccione Cliente";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(45, 725);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(49, 42);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 148;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(1213, 59);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(100, 90);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 149;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Click += new System.EventHandler(this.pictureBox2_Click);
            // 
            // FormReportes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(1340, 814);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.btnQuery);
            this.Controls.Add(this.btnBuscarxProducto);
            this.Controls.Add(this.cbProductos);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnBuscarxIngreso);
            this.Controls.Add(this.btnBuscarxCliente);
            this.Controls.Add(this.btnGenerar);
            this.Controls.Add(this.cbIngresosx);
            this.Controls.Add(this.dtpIngresos);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnBuscarxFecha);
            this.Controls.Add(this.btnProdVen);
            this.Controls.Add(this.cbClientes);
            this.Controls.Add(this.dtpHasta);
            this.Controls.Add(this.dtpDesde);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TablaReporte);
            this.Controls.Add(this.panelBarra);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormReportes";
            this.Text = "FormReportes";
            this.Load += new System.EventHandler(this.FormReportes_Load_1);
            this.panelBarra.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbCerrar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TablaReporte)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pbMin;
        private System.Windows.Forms.PictureBox pbCerrar;
        public System.Windows.Forms.Panel panelBarra;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView TablaReporte;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.DateTimePicker dtpDesde;
        private System.Windows.Forms.DateTimePicker dtpHasta;
        private System.Windows.Forms.ComboBox cbClientes;
        private System.Windows.Forms.Button btnProdVen;
        private System.Windows.Forms.Button btnBuscarxFecha;
        private System.Windows.Forms.DateTimePicker dtpIngresos;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cbIngresosx;
        private System.Windows.Forms.Button btnGenerar;
        private System.Windows.Forms.Button btnBuscarxCliente;
        private System.Windows.Forms.Button btnBuscarxIngreso;
        private System.Windows.Forms.Button btnBuscarxProducto;
        private System.Windows.Forms.ComboBox cbProductos;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnQuery;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
    }
}