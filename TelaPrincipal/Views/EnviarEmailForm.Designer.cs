namespace TelaPrincipal.Views
{
    partial class EnviarEmailForm
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
            this.txtDestinatario = new System.Windows.Forms.TextBox();
            this.btnEnviar = new FontAwesome.Sharp.IconButton();
            this.SuspendLayout();
            // 
            // txtDestinatario
            // 
            this.txtDestinatario.Location = new System.Drawing.Point(200, 88);
            this.txtDestinatario.Name = "txtDestinatario";
            this.txtDestinatario.Size = new System.Drawing.Size(100, 20);
            this.txtDestinatario.TabIndex = 0;
            // 
            // btnEnviar
            // 
            this.btnEnviar.IconChar = FontAwesome.Sharp.IconChar.None;
            this.btnEnviar.IconColor = System.Drawing.Color.Black;
            this.btnEnviar.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnEnviar.Location = new System.Drawing.Point(214, 114);
            this.btnEnviar.Name = "btnEnviar";
            this.btnEnviar.Size = new System.Drawing.Size(75, 23);
            this.btnEnviar.TabIndex = 1;
            this.btnEnviar.Text = "iconButton1";
            this.btnEnviar.UseVisualStyleBackColor = true;
           
            // 
            // EnviarEmailForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnEnviar);
            this.Controls.Add(this.txtDestinatario);
            this.Name = "EnviarEmailForm";
            this.Text = "EnviarEmailForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtDestinatario;
        private FontAwesome.Sharp.IconButton btnEnviar;
    }
}