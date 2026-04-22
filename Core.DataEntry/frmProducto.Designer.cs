namespace Core.DataEntry
{
    partial class frmProducto
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
            this.PrecioVenta = new System.Windows.Forms.Label();
            this.nudPrecioVenta = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.lblMarca = new System.Windows.Forms.Label();
            this.txtModelo = new System.Windows.Forms.TextBox();
            this.txtMarca = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnInsertarProducto = new System.Windows.Forms.Button();
            this.nudCosto = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.txtMedida = new System.Windows.Forms.TextBox();
            this.Costo = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.nudPrecioVenta)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCosto)).BeginInit();
            this.SuspendLayout();
            // 
            // PrecioVenta
            // 
            this.PrecioVenta.AutoSize = true;
            this.PrecioVenta.Location = new System.Drawing.Point(145, 247);
            this.PrecioVenta.Name = "PrecioVenta";
            this.PrecioVenta.Size = new System.Drawing.Size(40, 13);
            this.PrecioVenta.TabIndex = 54;
            this.PrecioVenta.Text = "Sueldo";
            // 
            // nudPrecioVenta
            // 
            this.nudPrecioVenta.DecimalPlaces = 2;
            this.nudPrecioVenta.Location = new System.Drawing.Point(148, 263);
            this.nudPrecioVenta.Name = "nudPrecioVenta";
            this.nudPrecioVenta.Size = new System.Drawing.Size(245, 20);
            this.nudPrecioVenta.TabIndex = 53;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(419, 86);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(42, 13);
            this.label9.TabIndex = 50;
            this.label9.Text = "Modelo";
            // 
            // lblMarca
            // 
            this.lblMarca.AutoSize = true;
            this.lblMarca.Location = new System.Drawing.Point(145, 86);
            this.lblMarca.Name = "lblMarca";
            this.lblMarca.Size = new System.Drawing.Size(37, 13);
            this.lblMarca.TabIndex = 49;
            this.lblMarca.Text = "Marca";
            // 
            // txtModelo
            // 
            this.txtModelo.Location = new System.Drawing.Point(422, 102);
            this.txtModelo.Name = "txtModelo";
            this.txtModelo.Size = new System.Drawing.Size(234, 20);
            this.txtModelo.TabIndex = 48;
            // 
            // txtMarca
            // 
            this.txtMarca.Location = new System.Drawing.Point(148, 102);
            this.txtMarca.Name = "txtMarca";
            this.txtMarca.Size = new System.Drawing.Size(245, 20);
            this.txtMarca.TabIndex = 47;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(178, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(469, 25);
            this.label3.TabIndex = 56;
            this.label3.Text = "INSERTAR PRODUCTO A LA BASE DE DATOS";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // btnInsertarProducto
            // 
            this.btnInsertarProducto.Location = new System.Drawing.Point(148, 341);
            this.btnInsertarProducto.Name = "btnInsertarProducto";
            this.btnInsertarProducto.Size = new System.Drawing.Size(508, 51);
            this.btnInsertarProducto.TabIndex = 55;
            this.btnInsertarProducto.Text = "Insertar Producto";
            this.btnInsertarProducto.UseVisualStyleBackColor = true;
            this.btnInsertarProducto.Click += new System.EventHandler(this.btnInsertarProducto_Click);
            // 
            // nudCosto
            // 
            this.nudCosto.DecimalPlaces = 2;
            this.nudCosto.Location = new System.Drawing.Point(422, 263);
            this.nudCosto.Name = "nudCosto";
            this.nudCosto.Size = new System.Drawing.Size(245, 20);
            this.nudCosto.TabIndex = 57;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(145, 167);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 13);
            this.label1.TabIndex = 59;
            this.label1.Text = "Medida";
            // 
            // txtMedida
            // 
            this.txtMedida.Location = new System.Drawing.Point(148, 183);
            this.txtMedida.Name = "txtMedida";
            this.txtMedida.Size = new System.Drawing.Size(234, 20);
            this.txtMedida.TabIndex = 58;
            // 
            // Costo
            // 
            this.Costo.AutoSize = true;
            this.Costo.Location = new System.Drawing.Point(419, 247);
            this.Costo.Name = "Costo";
            this.Costo.Size = new System.Drawing.Size(40, 13);
            this.Costo.TabIndex = 60;
            this.Costo.Text = "Sueldo";
            // 
            // frmProducto
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(808, 454);
            this.Controls.Add(this.Costo);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtMedida);
            this.Controls.Add(this.nudCosto);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnInsertarProducto);
            this.Controls.Add(this.PrecioVenta);
            this.Controls.Add(this.nudPrecioVenta);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.lblMarca);
            this.Controls.Add(this.txtModelo);
            this.Controls.Add(this.txtMarca);
            this.Name = "frmProducto";
            this.Text = "frmProducto";
            ((System.ComponentModel.ISupportInitialize)(this.nudPrecioVenta)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCosto)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label PrecioVenta;
        private System.Windows.Forms.NumericUpDown nudPrecioVenta;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label lblMarca;
        private System.Windows.Forms.TextBox txtModelo;
        private System.Windows.Forms.TextBox txtMarca;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnInsertarProducto;
        private System.Windows.Forms.NumericUpDown nudCosto;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtMedida;
        private System.Windows.Forms.Label Costo;
    }
}