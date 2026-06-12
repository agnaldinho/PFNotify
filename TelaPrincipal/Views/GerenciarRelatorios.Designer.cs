namespace TelaPrincipal.Views
{
    partial class GerenciarRelatorios
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.iconPictureBox1 = new FontAwesome.Sharp.IconPictureBox();
            this.label9 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnPesquisar = new FontAwesome.Sharp.IconButton();
            this.cbRelatorios = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.txtArquivo = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtNomeRelatorio = new System.Windows.Forms.TextBox();
            this.cbPDF = new System.Windows.Forms.CheckBox();
            this.cbExcel = new System.Windows.Forms.CheckBox();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.Diariamente = new System.Windows.Forms.RadioButton();
            this.iconPictureBox4 = new FontAwesome.Sharp.IconPictureBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.Quinzenalmente = new System.Windows.Forms.RadioButton();
            this.Semanalmente = new System.Windows.Forms.RadioButton();
            this.panel3 = new System.Windows.Forms.Panel();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.label2 = new System.Windows.Forms.Label();
            this.btnExcluir = new FontAwesome.Sharp.IconButton();
            this.btnAlterar = new FontAwesome.Sharp.IconButton();
            this.Mensalmente = new System.Windows.Forms.RadioButton();
            this.dgvConfigurados = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.iconPictureBox1)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.iconPictureBox4)).BeginInit();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvConfigurados)).BeginInit();
            this.SuspendLayout();
            // 
            // iconPictureBox1
            // 
            this.iconPictureBox1.BackColor = System.Drawing.Color.White;
            this.iconPictureBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(150)))), ((int)(((byte)(255)))));
            this.iconPictureBox1.IconChar = FontAwesome.Sharp.IconChar.Eye;
            this.iconPictureBox1.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(150)))), ((int)(((byte)(255)))));
            this.iconPictureBox1.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.iconPictureBox1.IconSize = 27;
            this.iconPictureBox1.Location = new System.Drawing.Point(0, 3);
            this.iconPictureBox1.Name = "iconPictureBox1";
            this.iconPictureBox1.Size = new System.Drawing.Size(27, 29);
            this.iconPictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.iconPictureBox1.TabIndex = 26;
            this.iconPictureBox1.TabStop = false;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(3, 1);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(0, 13);
            this.label9.TabIndex = 24;
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Controls.Add(this.btnPesquisar);
            this.panel2.Controls.Add(this.cbRelatorios);
            this.panel2.Controls.Add(this.iconPictureBox1);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.label9);
            this.panel2.Location = new System.Drawing.Point(12, 153);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1020, 69);
            this.panel2.TabIndex = 43;
            // 
            // btnPesquisar
            // 
            this.btnPesquisar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPesquisar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(120)))), ((int)(((byte)(246)))));
            this.btnPesquisar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPesquisar.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnPesquisar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(246)))), ((int)(((byte)(250)))));
            this.btnPesquisar.IconChar = FontAwesome.Sharp.IconChar.Eye;
            this.btnPesquisar.IconColor = System.Drawing.Color.White;
            this.btnPesquisar.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnPesquisar.IconSize = 26;
            this.btnPesquisar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPesquisar.Location = new System.Drawing.Point(878, 22);
            this.btnPesquisar.Name = "btnPesquisar";
            this.btnPesquisar.Size = new System.Drawing.Size(110, 33);
            this.btnPesquisar.TabIndex = 40;
            this.btnPesquisar.Text = "Pesquisar";
            this.btnPesquisar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnPesquisar.UseVisualStyleBackColor = false;
            // 
            // cbRelatorios
            // 
            this.cbRelatorios.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbRelatorios.FormattingEnabled = true;
            this.cbRelatorios.Location = new System.Drawing.Point(33, 22);
            this.cbRelatorios.Name = "cbRelatorios";
            this.cbRelatorios.Size = new System.Drawing.Size(817, 21);
            this.cbRelatorios.TabIndex = 27;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label6.Location = new System.Drawing.Point(32, 3);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(147, 16);
            this.label6.TabIndex = 24;
            this.label6.Text = "Rélatorios Disponiveis";
            // 
            // label11
            // 
            this.label11.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label11.Location = new System.Drawing.Point(647, 139);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(215, 16);
            this.label11.TabIndex = 47;
            this.label11.Text = "Nome do Relatorio no Excel/PDF";
            // 
            // txtArquivo
            // 
            this.txtArquivo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtArquivo.Location = new System.Drawing.Point(562, 171);
            this.txtArquivo.Name = "txtArquivo";
            this.txtArquivo.Size = new System.Drawing.Size(380, 20);
            this.txtArquivo.TabIndex = 46;
            // 
            // label10
            // 
            this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label10.Location = new System.Drawing.Point(687, 68);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(126, 16);
            this.label10.TabIndex = 45;
            this.label10.Text = "Nome do Relatorio";
            // 
            // txtNomeRelatorio
            // 
            this.txtNomeRelatorio.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNomeRelatorio.Location = new System.Drawing.Point(562, 101);
            this.txtNomeRelatorio.Name = "txtNomeRelatorio";
            this.txtNomeRelatorio.Size = new System.Drawing.Size(379, 20);
            this.txtNomeRelatorio.TabIndex = 44;
            // 
            // cbPDF
            // 
            this.cbPDF.AutoSize = true;
            this.cbPDF.Location = new System.Drawing.Point(7, 174);
            this.cbPDF.Name = "cbPDF";
            this.cbPDF.Size = new System.Drawing.Size(47, 17);
            this.cbPDF.TabIndex = 41;
            this.cbPDF.Text = "PDF";
            this.cbPDF.UseVisualStyleBackColor = true;
            // 
            // cbExcel
            // 
            this.cbExcel.AutoSize = true;
            this.cbExcel.Location = new System.Drawing.Point(60, 174);
            this.cbExcel.Name = "cbExcel";
            this.cbExcel.Size = new System.Drawing.Size(52, 17);
            this.cbExcel.TabIndex = 40;
            this.cbExcel.Text = "Excel";
            this.cbExcel.UseVisualStyleBackColor = true;
            // 
            // txtEmail
            // 
            this.txtEmail.Location = new System.Drawing.Point(562, 39);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(379, 20);
            this.txtEmail.TabIndex = 38;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label3.Location = new System.Drawing.Point(729, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 16);
            this.label3.TabIndex = 37;
            this.label3.Text = "E-mail";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(3, 1);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(0, 13);
            this.label12.TabIndex = 24;
            // 
            // Diariamente
            // 
            this.Diariamente.AutoSize = true;
            this.Diariamente.Location = new System.Drawing.Point(1, 40);
            this.Diariamente.Name = "Diariamente";
            this.Diariamente.Size = new System.Drawing.Size(81, 17);
            this.Diariamente.TabIndex = 0;
            this.Diariamente.TabStop = true;
            this.Diariamente.Text = "Diariamente";
            this.Diariamente.UseVisualStyleBackColor = true;
            // 
            // iconPictureBox4
            // 
            this.iconPictureBox4.BackColor = System.Drawing.Color.White;
            this.iconPictureBox4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(150)))), ((int)(((byte)(255)))));
            this.iconPictureBox4.IconChar = FontAwesome.Sharp.IconChar.ChartSimple;
            this.iconPictureBox4.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(150)))), ((int)(((byte)(255)))));
            this.iconPictureBox4.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.iconPictureBox4.IconSize = 30;
            this.iconPictureBox4.Location = new System.Drawing.Point(12, 12);
            this.iconPictureBox4.Name = "iconPictureBox4";
            this.iconPictureBox4.Size = new System.Drawing.Size(39, 30);
            this.iconPictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.iconPictureBox4.TabIndex = 41;
            this.iconPictureBox4.TabStop = false;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label7.Location = new System.Drawing.Point(57, 15);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(240, 16);
            this.label7.TabIndex = 40;
            this.label7.Text = "Gerenciamento de envio de relatoris";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(117)))), ((int)(((byte)(125)))));
            this.label8.Location = new System.Drawing.Point(57, 28);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(195, 14);
            this.label8.TabIndex = 39;
            this.label8.Text = "Gerencie seus relatorios de envio";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(3, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(138, 16);
            this.label1.TabIndex = 34;
            this.label1.Text = "Frequencia de Envio";
            // 
            // Quinzenalmente
            // 
            this.Quinzenalmente.AutoSize = true;
            this.Quinzenalmente.Location = new System.Drawing.Point(186, 40);
            this.Quinzenalmente.Name = "Quinzenalmente";
            this.Quinzenalmente.Size = new System.Drawing.Size(101, 17);
            this.Quinzenalmente.TabIndex = 35;
            this.Quinzenalmente.TabStop = true;
            this.Quinzenalmente.Text = "Quinzenalmente";
            this.Quinzenalmente.UseVisualStyleBackColor = true;
            // 
            // Semanalmente
            // 
            this.Semanalmente.AutoSize = true;
            this.Semanalmente.Location = new System.Drawing.Point(85, 40);
            this.Semanalmente.Name = "Semanalmente";
            this.Semanalmente.Size = new System.Drawing.Size(95, 17);
            this.Semanalmente.TabIndex = 33;
            this.Semanalmente.TabStop = true;
            this.Semanalmente.Text = "Semanalmente";
            this.Semanalmente.UseVisualStyleBackColor = true;
            // 
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel3.BackColor = System.Drawing.Color.White;
            this.panel3.Controls.Add(this.dataGridView1);
            this.panel3.Controls.Add(this.label2);
            this.panel3.Controls.Add(this.btnExcluir);
            this.panel3.Controls.Add(this.btnAlterar);
            this.panel3.Controls.Add(this.label11);
            this.panel3.Controls.Add(this.txtArquivo);
            this.panel3.Controls.Add(this.label12);
            this.panel3.Controls.Add(this.Mensalmente);
            this.panel3.Controls.Add(this.label10);
            this.panel3.Controls.Add(this.txtNomeRelatorio);
            this.panel3.Controls.Add(this.label1);
            this.panel3.Controls.Add(this.Quinzenalmente);
            this.panel3.Controls.Add(this.cbExcel);
            this.panel3.Controls.Add(this.cbPDF);
            this.panel3.Controls.Add(this.Diariamente);
            this.panel3.Controls.Add(this.Semanalmente);
            this.panel3.Controls.Add(this.label3);
            this.panel3.Controls.Add(this.txtEmail);
            this.panel3.Location = new System.Drawing.Point(12, 224);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1021, 425);
            this.panel3.TabIndex = 42;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToOrderColumns = true;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(246)))), ((int)(((byte)(250)))));
            this.dataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView1.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridView1.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(214)))), ((int)(((byte)(219)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(37)))), ((int)(((byte)(41)))));
            this.dataGridView1.Location = new System.Drawing.Point(7, 220);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(1001, 149);
            this.dataGridView1.TabIndex = 50;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label2.Location = new System.Drawing.Point(430, 201);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(150, 16);
            this.label2.TabIndex = 49;
            this.label2.Text = "Empresas Cadastradas";
            // 
            // btnExcluir
            // 
            this.btnExcluir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExcluir.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(120)))), ((int)(((byte)(246)))));
            this.btnExcluir.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExcluir.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnExcluir.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(246)))), ((int)(((byte)(250)))));
            this.btnExcluir.IconChar = FontAwesome.Sharp.IconChar.Eye;
            this.btnExcluir.IconColor = System.Drawing.Color.White;
            this.btnExcluir.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnExcluir.IconSize = 26;
            this.btnExcluir.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExcluir.Location = new System.Drawing.Point(782, 375);
            this.btnExcluir.Name = "btnExcluir";
            this.btnExcluir.Size = new System.Drawing.Size(110, 33);
            this.btnExcluir.TabIndex = 48;
            this.btnExcluir.Text = "Excluir";
            this.btnExcluir.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnExcluir.UseVisualStyleBackColor = false;
            this.btnExcluir.Click += new System.EventHandler(this.btnExcluir_Click);
            // 
            // btnAlterar
            // 
            this.btnAlterar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAlterar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(120)))), ((int)(((byte)(246)))));
            this.btnAlterar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAlterar.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnAlterar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(246)))), ((int)(((byte)(250)))));
            this.btnAlterar.IconChar = FontAwesome.Sharp.IconChar.Eye;
            this.btnAlterar.IconColor = System.Drawing.Color.White;
            this.btnAlterar.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnAlterar.IconSize = 26;
            this.btnAlterar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAlterar.Location = new System.Drawing.Point(898, 375);
            this.btnAlterar.Name = "btnAlterar";
            this.btnAlterar.Size = new System.Drawing.Size(110, 33);
            this.btnAlterar.TabIndex = 41;
            this.btnAlterar.Text = "Salvar";
            this.btnAlterar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnAlterar.UseVisualStyleBackColor = false;
            this.btnAlterar.Click += new System.EventHandler(this.btnAlterar_Click);
            // 
            // Mensalmente
            // 
            this.Mensalmente.AutoSize = true;
            this.Mensalmente.Location = new System.Drawing.Point(287, 40);
            this.Mensalmente.Name = "Mensalmente";
            this.Mensalmente.Size = new System.Drawing.Size(88, 17);
            this.Mensalmente.TabIndex = 36;
            this.Mensalmente.TabStop = true;
            this.Mensalmente.Text = "Mensalmente";
            this.Mensalmente.UseVisualStyleBackColor = true;
            // 
            // dgvConfigurados
            // 
            this.dgvConfigurados.AllowUserToAddRows = false;
            this.dgvConfigurados.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvConfigurados.Location = new System.Drawing.Point(12, 48);
            this.dgvConfigurados.Name = "dgvConfigurados";
            this.dgvConfigurados.Size = new System.Drawing.Size(1021, 99);
            this.dgvConfigurados.TabIndex = 41;
            // 
            // GerenciarRelatorios
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1044, 661);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.iconPictureBox4);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.dgvConfigurados);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.panel3);
            this.Name = "GerenciarRelatorios";
            this.Text = "GerenciarRelatorios";
            ((System.ComponentModel.ISupportInitialize)(this.iconPictureBox1)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.iconPictureBox4)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvConfigurados)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private FontAwesome.Sharp.IconPictureBox iconPictureBox1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtArquivo;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtNomeRelatorio;
        private System.Windows.Forms.CheckBox cbPDF;
        private System.Windows.Forms.CheckBox cbExcel;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.RadioButton Diariamente;
        private FontAwesome.Sharp.IconPictureBox iconPictureBox4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton Quinzenalmente;
        private System.Windows.Forms.RadioButton Semanalmente;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.RadioButton Mensalmente;
        private FontAwesome.Sharp.IconButton btnPesquisar;
        private FontAwesome.Sharp.IconButton btnExcluir;
        private FontAwesome.Sharp.IconButton btnAlterar;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridView dgvConfigurados;
        private System.Windows.Forms.ComboBox cbRelatorios;
        private System.Windows.Forms.ComboBox _cbDiaSemana;
        private System.Windows.Forms.ComboBox _cbDiaMes;
        private System.Windows.Forms.DateTimePicker _dtpHorario;
    }
}