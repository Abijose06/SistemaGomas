namespace Core.DataEntry
{
    partial class frmVehiculo
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtPlaca = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnInsertarVehiculo = new System.Windows.Forms.Button();
            this.PrecioVenta = new System.Windows.Forms.Label();
            this.nudAño = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.lblMarca = new System.Windows.Forms.Label();
            this.txtModelo = new System.Windows.Forms.TextBox();
            this.txtMarca = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtChassis = new System.Windows.Forms.TextBox();
            this.cmbCliente = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.nudAño)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(117, 211);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 13);
            this.label1.TabIndex = 69;
            this.label1.Text = "Placa";
            // 
            // txtPlaca
            // 
            this.txtPlaca.Location = new System.Drawing.Point(120, 227);
            this.txtPlaca.Name = "txtPlaca";
            this.txtPlaca.Size = new System.Drawing.Size(234, 20);
            this.txtPlaca.TabIndex = 68;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(150, 64);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(456, 25);
            this.label3.TabIndex = 67;
            this.label3.Text = "INSERTAR VEHICULO A LA BASE DE DATOS";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // btnInsertarVehiculo
            // 
            this.btnInsertarVehiculo.Location = new System.Drawing.Point(120, 385);
            this.btnInsertarVehiculo.Name = "btnInsertarVehiculo";
            this.btnInsertarVehiculo.Size = new System.Drawing.Size(508, 51);
            this.btnInsertarVehiculo.TabIndex = 66;
            this.btnInsertarVehiculo.Text = "Insertar Vehiculo";
            this.btnInsertarVehiculo.UseVisualStyleBackColor = true;
            this.btnInsertarVehiculo.Click += new System.EventHandler(this.btnInsertarVehiculo_Click);
            // 
            // PrecioVenta
            // 
            this.PrecioVenta.AutoSize = true;
            this.PrecioVenta.Location = new System.Drawing.Point(117, 291);
            this.PrecioVenta.Name = "PrecioVenta";
            this.PrecioVenta.Size = new System.Drawing.Size(26, 13);
            this.PrecioVenta.TabIndex = 65;
            this.PrecioVenta.Text = "Año";
            // 
            // nudAño
            // 
            this.nudAño.Location = new System.Drawing.Point(120, 307);
            this.nudAño.Maximum = new decimal(new int[] {
            2200,
            0,
            0,
            0});
            this.nudAño.Minimum = new decimal(new int[] {
            1900,
            0,
            0,
            0});
            this.nudAño.Name = "nudAño";
            this.nudAño.Size = new System.Drawing.Size(245, 20);
            this.nudAño.TabIndex = 64;
            this.nudAño.Value = new decimal(new int[] {
            1900,
            0,
            0,
            0});
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(391, 130);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(42, 13);
            this.label9.TabIndex = 63;
            this.label9.Text = "Modelo";
            // 
            // lblMarca
            // 
            this.lblMarca.AutoSize = true;
            this.lblMarca.Location = new System.Drawing.Point(117, 130);
            this.lblMarca.Name = "lblMarca";
            this.lblMarca.Size = new System.Drawing.Size(37, 13);
            this.lblMarca.TabIndex = 62;
            this.lblMarca.Text = "Marca";
            // 
            // txtModelo
            // 
            this.txtModelo.Location = new System.Drawing.Point(394, 146);
            this.txtModelo.Name = "txtModelo";
            this.txtModelo.Size = new System.Drawing.Size(234, 20);
            this.txtModelo.TabIndex = 61;
            // 
            // txtMarca
            // 
            this.txtMarca.Location = new System.Drawing.Point(120, 146);
            this.txtMarca.Name = "txtMarca";
            this.txtMarca.Size = new System.Drawing.Size(245, 20);
            this.txtMarca.TabIndex = 60;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(391, 211);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 13);
            this.label2.TabIndex = 71;
            this.label2.Text = "Chassis";
            // 
            // txtChassis
            // 
            this.txtChassis.Location = new System.Drawing.Point(394, 227);
            this.txtChassis.Name = "txtChassis";
            this.txtChassis.Size = new System.Drawing.Size(234, 20);
            this.txtChassis.TabIndex = 70;
            // 
            // cmbCliente
            // 
            this.cmbCliente.FormattingEnabled = true;
            this.cmbCliente.Location = new System.Drawing.Point(394, 307);
            this.cmbCliente.Name = "cmbCliente";
            this.cmbCliente.Size = new System.Drawing.Size(234, 21);
            this.cmbCliente.TabIndex = 72;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(391, 291);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(39, 13);
            this.label4.TabIndex = 73;
            this.label4.Text = "Cliente";
            // 
            // frmVehiculo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(744, 500);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cmbCliente);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtChassis);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtPlaca);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnInsertarVehiculo);
            this.Controls.Add(this.PrecioVenta);
            this.Controls.Add(this.nudAño);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.lblMarca);
            this.Controls.Add(this.txtModelo);
            this.Controls.Add(this.txtMarca);
            this.Name = "frmVehiculo";
            this.Text = "frmVehiculo";
            ((System.ComponentModel.ISupportInitialize)(this.nudAño)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtPlaca;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnInsertarVehiculo;
        private System.Windows.Forms.Label PrecioVenta;
        private System.Windows.Forms.NumericUpDown nudAño;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label lblMarca;
        private System.Windows.Forms.TextBox txtModelo;
        private System.Windows.Forms.TextBox txtMarca;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtChassis;
        private System.Windows.Forms.ComboBox cmbCliente;
        private System.Windows.Forms.Label label4;
    }
}