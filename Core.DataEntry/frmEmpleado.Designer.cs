namespace Core.DataEntry
{
    partial class frmEmpleado
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
            this.lblCorreo = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lblTelefono = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnInsertarEmpleado = new System.Windows.Forms.Button();
            this.txtContraseña = new System.Windows.Forms.TextBox();
            this.txtCorreo = new System.Windows.Forms.TextBox();
            this.txtApellidos = new System.Windows.Forms.TextBox();
            this.txtDocumento = new System.Windows.Forms.TextBox();
            this.txtTelefono = new System.Windows.Forms.TextBox();
            this.txtNombres = new System.Windows.Forms.TextBox();
            this.txtTipoDocumento = new System.Windows.Forms.TextBox();
            this.cmbRol = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.cmbSucursal = new System.Windows.Forms.ComboBox();
            this.nudSueldo = new System.Windows.Forms.NumericUpDown();
            this.dtpFechaIngreso = new System.Windows.Forms.DateTimePicker();
            this.label8 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.nudSueldo)).BeginInit();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(198, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(467, 25);
            this.label3.TabIndex = 38;
            this.label3.Text = "INSERTAR EMPLEADO A LA BASE DE DATOS";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(419, 295);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(61, 13);
            this.label9.TabIndex = 37;
            this.label9.Text = "Contraseña";
            // 
            // lblCorreo
            // 
            this.lblCorreo.AutoSize = true;
            this.lblCorreo.Location = new System.Drawing.Point(419, 213);
            this.lblCorreo.Name = "lblCorreo";
            this.lblCorreo.Size = new System.Drawing.Size(38, 13);
            this.lblCorreo.TabIndex = 36;
            this.lblCorreo.Text = "Correo";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(419, 133);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(49, 13);
            this.label7.TabIndex = 35;
            this.label7.Text = "Apellidos";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(419, 66);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(62, 13);
            this.label6.TabIndex = 34;
            this.label6.Text = "Documento";
            // 
            // lblTelefono
            // 
            this.lblTelefono.AutoSize = true;
            this.lblTelefono.Location = new System.Drawing.Point(145, 213);
            this.lblTelefono.Name = "lblTelefono";
            this.lblTelefono.Size = new System.Drawing.Size(49, 13);
            this.lblTelefono.TabIndex = 32;
            this.lblTelefono.Text = "Telefono";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(145, 133);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 13);
            this.label2.TabIndex = 31;
            this.label2.Text = "Nombres";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(145, 66);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 13);
            this.label1.TabIndex = 30;
            this.label1.Text = "Tipo Documento";
            // 
            // btnInsertarEmpleado
            // 
            this.btnInsertarEmpleado.Location = new System.Drawing.Point(148, 536);
            this.btnInsertarEmpleado.Name = "btnInsertarEmpleado";
            this.btnInsertarEmpleado.Size = new System.Drawing.Size(508, 51);
            this.btnInsertarEmpleado.TabIndex = 29;
            this.btnInsertarEmpleado.Text = "Insertar Empleado";
            this.btnInsertarEmpleado.UseVisualStyleBackColor = true;
            this.btnInsertarEmpleado.Click += new System.EventHandler(this.btnInsertarEmpleado_Click);
            // 
            // txtContraseña
            // 
            this.txtContraseña.Location = new System.Drawing.Point(422, 311);
            this.txtContraseña.Name = "txtContraseña";
            this.txtContraseña.PasswordChar = '*';
            this.txtContraseña.Size = new System.Drawing.Size(234, 20);
            this.txtContraseña.TabIndex = 28;
            // 
            // txtCorreo
            // 
            this.txtCorreo.Location = new System.Drawing.Point(422, 229);
            this.txtCorreo.Name = "txtCorreo";
            this.txtCorreo.Size = new System.Drawing.Size(234, 20);
            this.txtCorreo.TabIndex = 27;
            // 
            // txtApellidos
            // 
            this.txtApellidos.Location = new System.Drawing.Point(422, 149);
            this.txtApellidos.Name = "txtApellidos";
            this.txtApellidos.Size = new System.Drawing.Size(234, 20);
            this.txtApellidos.TabIndex = 26;
            // 
            // txtDocumento
            // 
            this.txtDocumento.Location = new System.Drawing.Point(422, 82);
            this.txtDocumento.Name = "txtDocumento";
            this.txtDocumento.Size = new System.Drawing.Size(234, 20);
            this.txtDocumento.TabIndex = 25;
            // 
            // txtTelefono
            // 
            this.txtTelefono.Location = new System.Drawing.Point(148, 229);
            this.txtTelefono.Name = "txtTelefono";
            this.txtTelefono.Size = new System.Drawing.Size(245, 20);
            this.txtTelefono.TabIndex = 23;
            // 
            // txtNombres
            // 
            this.txtNombres.Location = new System.Drawing.Point(148, 149);
            this.txtNombres.Name = "txtNombres";
            this.txtNombres.Size = new System.Drawing.Size(245, 20);
            this.txtNombres.TabIndex = 22;
            // 
            // txtTipoDocumento
            // 
            this.txtTipoDocumento.Location = new System.Drawing.Point(148, 82);
            this.txtTipoDocumento.Name = "txtTipoDocumento";
            this.txtTipoDocumento.Size = new System.Drawing.Size(245, 20);
            this.txtTipoDocumento.TabIndex = 21;
            // 
            // cmbRol
            // 
            this.cmbRol.FormattingEnabled = true;
            this.cmbRol.Location = new System.Drawing.Point(148, 311);
            this.cmbRol.Name = "cmbRol";
            this.cmbRol.Size = new System.Drawing.Size(245, 21);
            this.cmbRol.TabIndex = 39;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(145, 295);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(23, 13);
            this.label4.TabIndex = 40;
            this.label4.Text = "Rol";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(145, 376);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(48, 13);
            this.label5.TabIndex = 42;
            this.label5.Text = "Sucursal";
            // 
            // cmbSucursal
            // 
            this.cmbSucursal.FormattingEnabled = true;
            this.cmbSucursal.Location = new System.Drawing.Point(148, 392);
            this.cmbSucursal.Name = "cmbSucursal";
            this.cmbSucursal.Size = new System.Drawing.Size(245, 21);
            this.cmbSucursal.TabIndex = 41;
            // 
            // nudSueldo
            // 
            this.nudSueldo.DecimalPlaces = 2;
            this.nudSueldo.Location = new System.Drawing.Point(148, 472);
            this.nudSueldo.Name = "nudSueldo";
            this.nudSueldo.Size = new System.Drawing.Size(245, 20);
            this.nudSueldo.TabIndex = 43;
            // 
            // dtpFechaIngreso
            // 
            this.dtpFechaIngreso.Location = new System.Drawing.Point(422, 389);
            this.dtpFechaIngreso.Name = "dtpFechaIngreso";
            this.dtpFechaIngreso.Size = new System.Drawing.Size(234, 20);
            this.dtpFechaIngreso.TabIndex = 44;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(420, 376);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(75, 13);
            this.label8.TabIndex = 45;
            this.label8.Text = "Fecha Ingreso";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(145, 456);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(40, 13);
            this.label10.TabIndex = 46;
            this.label10.Text = "Sueldo";
            // 
            // frmEmpleado
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(813, 626);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.dtpFechaIngreso);
            this.Controls.Add(this.nudSueldo);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cmbSucursal);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cmbRol);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.lblCorreo);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.lblTelefono);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnInsertarEmpleado);
            this.Controls.Add(this.txtContraseña);
            this.Controls.Add(this.txtCorreo);
            this.Controls.Add(this.txtApellidos);
            this.Controls.Add(this.txtDocumento);
            this.Controls.Add(this.txtTelefono);
            this.Controls.Add(this.txtNombres);
            this.Controls.Add(this.txtTipoDocumento);
            this.Name = "frmEmpleado";
            this.Text = "frmEmpleado";
            this.Load += new System.EventHandler(this.frmEmpleado_Load);
            ((System.ComponentModel.ISupportInitialize)(this.nudSueldo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label lblCorreo;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblTelefono;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnInsertarEmpleado;
        private System.Windows.Forms.TextBox txtContraseña;
        private System.Windows.Forms.TextBox txtCorreo;
        private System.Windows.Forms.TextBox txtApellidos;
        private System.Windows.Forms.TextBox txtDocumento;
        private System.Windows.Forms.TextBox txtTelefono;
        private System.Windows.Forms.TextBox txtNombres;
        private System.Windows.Forms.TextBox txtTipoDocumento;
        private System.Windows.Forms.ComboBox cmbRol;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cmbSucursal;
        private System.Windows.Forms.NumericUpDown nudSueldo;
        private System.Windows.Forms.DateTimePicker dtpFechaIngreso;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label10;
    }
}