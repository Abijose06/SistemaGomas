namespace Core.DataEntry
{
    partial class frmSucursal
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
            this.label3 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.btnInsertarSucursal = new System.Windows.Forms.Button();
            this.txtTelefono = new System.Windows.Forms.TextBox();
            this.txtDireccion = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(200, 51);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(464, 25);
            this.label3.TabIndex = 25;
            this.label3.Text = "INSERTAR SUCURSAL A LA BASE DE DATOS";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(425, 144);
            this.label9.Name = "label9";
            this.label9.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label9.Size = new System.Drawing.Size(49, 13);
            this.label9.TabIndex = 24;
            this.label9.Text = "Telefono";
            // 
            // btnInsertarSucursal
            // 
            this.btnInsertarSucursal.Location = new System.Drawing.Point(154, 249);
            this.btnInsertarSucursal.Name = "btnInsertarSucursal";
            this.btnInsertarSucursal.Size = new System.Drawing.Size(508, 51);
            this.btnInsertarSucursal.TabIndex = 23;
            this.btnInsertarSucursal.Text = "Insertar Sucursal";
            this.btnInsertarSucursal.UseVisualStyleBackColor = true;
            // 
            // txtTelefono
            // 
            this.txtTelefono.Location = new System.Drawing.Point(428, 160);
            this.txtTelefono.Name = "txtTelefono";
            this.txtTelefono.PasswordChar = '*';
            this.txtTelefono.Size = new System.Drawing.Size(234, 20);
            this.txtTelefono.TabIndex = 22;
            // 
            // txtDireccion
            // 
            this.txtDireccion.Location = new System.Drawing.Point(154, 160);
            this.txtDireccion.Name = "txtDireccion";
            this.txtDireccion.Size = new System.Drawing.Size(245, 20);
            this.txtDireccion.TabIndex = 21;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(151, 144);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 13);
            this.label1.TabIndex = 26;
            this.label1.Text = "Direccion";
            // 
            // frmSucursal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.btnInsertarSucursal);
            this.Controls.Add(this.txtTelefono);
            this.Controls.Add(this.txtDireccion);
            this.Name = "frmSucursal";
            this.Text = "frmSucursal";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnInsertarSucursal;
        private System.Windows.Forms.TextBox txtTelefono;
        private System.Windows.Forms.TextBox txtDireccion;
        private System.Windows.Forms.Label label1;
    }
}