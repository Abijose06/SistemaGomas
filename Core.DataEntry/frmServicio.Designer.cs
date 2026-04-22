namespace Core.DataEntry
{
    partial class frmServicio
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
            this.label9 = new System.Windows.Forms.Label();
            this.btnInsertarServicio = new System.Windows.Forms.Button();
            this.txtDescripcion = new System.Windows.Forms.TextBox();
            this.txtNombreServicio = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.nudPrecio = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.nudPrecio)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(145, 124);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 13);
            this.label1.TabIndex = 31;
            this.label1.Text = "Nombre Servicio";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(419, 124);
            this.label9.Name = "label9";
            this.label9.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label9.Size = new System.Drawing.Size(63, 13);
            this.label9.TabIndex = 30;
            this.label9.Text = "Descripcion";
            // 
            // btnInsertarServicio
            // 
            this.btnInsertarServicio.Location = new System.Drawing.Point(148, 313);
            this.btnInsertarServicio.Name = "btnInsertarServicio";
            this.btnInsertarServicio.Size = new System.Drawing.Size(508, 51);
            this.btnInsertarServicio.TabIndex = 29;
            this.btnInsertarServicio.Text = "Insertar Servicio";
            this.btnInsertarServicio.UseVisualStyleBackColor = true;
            this.btnInsertarServicio.Click += new System.EventHandler(this.btnInsertarServicio_Click);
            // 
            // txtDescripcion
            // 
            this.txtDescripcion.Location = new System.Drawing.Point(422, 140);
            this.txtDescripcion.Name = "txtDescripcion";
            this.txtDescripcion.PasswordChar = '*';
            this.txtDescripcion.Size = new System.Drawing.Size(234, 20);
            this.txtDescripcion.TabIndex = 28;
            // 
            // txtNombreServicio
            // 
            this.txtNombreServicio.Location = new System.Drawing.Point(148, 140);
            this.txtNombreServicio.Name = "txtNombreServicio";
            this.txtNombreServicio.Size = new System.Drawing.Size(245, 20);
            this.txtNombreServicio.TabIndex = 27;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(173, 45);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(448, 25);
            this.label3.TabIndex = 32;
            this.label3.Text = "INSERTAR SERVICIO A LA BASE DE DATOS";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // nudPrecio
            // 
            this.nudPrecio.Location = new System.Drawing.Point(148, 224);
            this.nudPrecio.Name = "nudPrecio";
            this.nudPrecio.Size = new System.Drawing.Size(245, 20);
            this.nudPrecio.TabIndex = 33;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(145, 208);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 13);
            this.label2.TabIndex = 34;
            this.label2.Text = "Precio";
            // 
            // frmServicio
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.nudPrecio);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.btnInsertarServicio);
            this.Controls.Add(this.txtDescripcion);
            this.Controls.Add(this.txtNombreServicio);
            this.Name = "frmServicio";
            this.Text = "frmServicio";
            ((System.ComponentModel.ISupportInitialize)(this.nudPrecio)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnInsertarServicio;
        private System.Windows.Forms.TextBox txtDescripcion;
        private System.Windows.Forms.TextBox txtNombreServicio;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown nudPrecio;
        private System.Windows.Forms.Label label2;
    }
}