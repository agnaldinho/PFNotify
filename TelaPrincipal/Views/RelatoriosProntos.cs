using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using FirebirdSql.Data.FirebirdClient;
using Newtonsoft.Json;
using TelaPrincipal.Models;
using TelaPrincipal.DAO;
using TelaPrincipal.Utils;
using TelaPrincipal.Controllers;
using FontAwesome.Sharp;
using System.Drawing;


namespace TelaPrincipal.Views
{
    public partial class RelatoriosProntos : Form
    {
        private List<RelatorioPronto> relatorios;

        public RelatoriosProntos()
        {
            InitializeComponent();

            CbFormato.DrawMode = DrawMode.OwnerDrawFixed;
            CbFormato.DropDownStyle = ComboBoxStyle.DropDownList;
            CbFormato.ItemHeight = 22;

            CbFormato.DrawItem += CbFormato_DrawItem;

            InicializarCb();
            CarregarComboBoxEmpresa();
            IniciarSessao();

            this.Resize += RelatoriosProntos_Resize;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            AjustarLayoutResponsivo();
        }

        private void RelatoriosProntos_Resize(object sender, EventArgs e)
        {
            AjustarLayoutResponsivo();
        }

        private void AjustarLayoutResponsivo()
        {
            int w = this.ClientSize.Width;
            int h = this.ClientSize.Height;
            int margem = 14;
            int larguraColunas = 270;    // panel2 (campos disponíveis) — largura fixa esquerda
            int alturaTopo = 56;      // ícone + título no topo
            int alturaFiltros = 155;     // panel1 (filtros)
            int alturaBotoes = 40;      // BtnGerar / BtnEnviar na base
            int gap = 6;

            // ── Cabeçalho (ícone + título) ────────────────────────────────
            iconPictureBox4.Location = new Point(margem, 12);
            label2.Location = new Point(margem + 45, 15);
            label3.Location = new Point(margem + 45, 28);

            // ── panel1 — barra de filtros (largura total) ─────────────────
            panel1.Location = new Point(margem, alturaTopo);
            panel1.Size = new Size(w - margem * 2, alturaFiltros);

            // BtnVisualizar alinhado à direita dentro do panel1
            BtnVisualizar.Location = new Point(panel1.Width - BtnVisualizar.Width - 10, 102);

            // ── Área de conteúdo (abaixo dos filtros) ────────────────────
            int yConteudo = alturaTopo + alturaFiltros + gap;
            int alturaConteudo = h - yConteudo - alturaBotoes - gap * 2;

            // ── panel2 — lista de campos (esquerda) ───────────────────────
            panel2.Location = new Point(margem, yConteudo);
            panel2.Size = new Size(larguraColunas, alturaConteudo);
            ClbColunas.Size = new Size(larguraColunas - 4, alturaConteudo - 37);

            // ── Área direita: label de preview + dgvResultado ─────────────
            int xDir = margem + larguraColunas + gap;
            int larguraDir = w - xDir - margem;

            // panel3 só tem o título "Pré-visualização" — altura pequena
            panel3.Location = new Point(xDir, yConteudo);
            panel3.Size = new Size(larguraDir, 32);

            // dgvResultado diretamente no form, logo abaixo do panel3
            int yGrid = yConteudo + 32 + 2;
            dgvResultado.Location = new Point(xDir, yGrid);
            dgvResultado.Size = new Size(larguraDir, h - yGrid - alturaBotoes - gap);

            // ── Botões na base, alinhados à direita ───────────────────────
            int yBotoes = h - alturaBotoes - 2;
            BtnGerar.Location = new Point(xDir, yBotoes);
            BtnEnviar.Location = new Point(xDir + BtnGerar.Width + 8, yBotoes);
        }
        private void IniciarSessao()
        {
            var config = ConfigHelper.Carregar();

            if (config == null)
            {
                MessageBox.Show("Configure o banco primeiro.");
                return;
            }

            string connStr = $"Database={config.CaminhoBanco};" +
                             $"DataSource={config.Servidor};" +
                             $"User={config.Usuario};" +
                             $"Password={config.Senha};" +
                             $"Port={config.Porta};" +
                             $"Dialect=3;";

            ConexaoFactory.Inicializar(connStr);
            using (var conn = new FbConnection(connStr))
            {
                conn.Open();
            }
        }



        private void CbFormato_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            var combo = sender as ComboBox;
            var item = (ComboItemIcon)combo.Items[e.Index];

            e.DrawBackground();

            bool selecionado = (e.State & DrawItemState.Selected) == DrawItemState.Selected;

            Color backColor = selecionado ? Color.FromArgb(52, 120, 246) : Color.White;
            Color textColor = selecionado ? Color.White : Color.Black;

            using (var bg = new SolidBrush(backColor))
                e.Graphics.FillRectangle(bg, e.Bounds);

            // Ícone
            if (item.IconeBitmap != null)
            {
                e.Graphics.DrawImage(item.IconeBitmap, e.Bounds.Left + 5, e.Bounds.Top + 3);
            }

            // Texto
            using (var br = new SolidBrush(textColor))
            {
                e.Graphics.DrawString(item.Texto, e.Font, br,
                    e.Bounds.Left + 28, e.Bounds.Top + 3);
            }

            e.DrawFocusRectangle();
        }

        public void InicializarCb()
        {
            relatorios = CarregarRelatorios();

            CbRelario.DataSource = null;
            CbRelario.Items.Clear();

            CbRelario.DataSource = relatorios.ToList();
            CbRelario.DisplayMember = "Nome";
            CbRelario.ValueMember = "Nome";

            // 🔥 garante seleção
            if (CbRelario.Items.Count > 0)
                CbRelario.SelectedIndex = 0;

            CbFormato.DrawMode = DrawMode.OwnerDrawFixed;
            CbFormato.DropDownStyle = ComboBoxStyle.DropDownList;
            CbFormato.ItemHeight = 22;

            CbFormato.Items.Clear();

            CbFormato.Items.Add(new ComboItemIcon
            {
                Texto = "Excel",
                Icone = IconChar.FileExcel,
                IconeBitmap = GerarIcone(IconChar.FileExcel, Color.Green)
            });

            CbFormato.Items.Add(new ComboItemIcon
            {
                Texto = "PDF",
                Icone = IconChar.FilePdf,
                IconeBitmap = GerarIcone(IconChar.FilePdf, Color.Red)
            });

            CbFormato.SelectedIndex = 0;
        }

        private void RelatoriosProntos_Load(object sender, EventArgs e)
        {
            try
            {
                IniciarSessao();
                CarregarComboBoxEmpresa();
                InicializarCb();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao iniciar sessão: " + ex.Message);
            }
        }

        public class ComboItemIcon
        {
            public string Texto { get; set; }
            public IconChar Icone { get; set; }
            public Bitmap IconeBitmap { get; set; }

            public override string ToString()
            {
                return Texto;
            }
        }
        private Bitmap GerarIcone(IconChar iconChar, Color cor)
        {
            var icon = new IconPictureBox
            {
                IconChar = iconChar,
                IconColor = cor,
                IconSize = 16,
                BackColor = Color.Transparent
            };

            Bitmap bmp = new Bitmap(16, 16);
            icon.DrawToBitmap(bmp, new Rectangle(0, 0, 16, 16));

            return bmp;
        }
        private List<RelatorioPronto> CarregarRelatorios()
        {
            var lista = new List<RelatorioPronto>();

            string pasta = Path.Combine(Application.StartupPath, "Json");

            if (!Directory.Exists(pasta))
            {
                MessageBox.Show("Pasta não encontrada:\n" + pasta);
                return lista;
            }

            var arquivos = Directory.GetFiles(pasta, "*.json");

            foreach (var arquivo in arquivos)
            {
                try
                {
                    var json = File.ReadAllText(arquivo);
                    var rel = JsonConvert.DeserializeObject<RelatorioPronto>(json);

                    if (rel == null)
                        continue;

                    if (rel.Parametros == null)
                        rel.Parametros = new List<Parametro>();

                    if (rel.Campos == null)
                        rel.Campos = new List<Campo>();

                    if (string.IsNullOrWhiteSpace(rel.Nome))
                        continue;

                    lista.Add(rel);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro em {arquivo}:\n{ex.Message}");
                }
            }

            return lista;
        }

        public DataTable ExecutarRelatorio(RelatorioPronto relatorio, Dictionary<string, object> valores)
        {
            if (relatorio == null)
                throw new Exception("Relatório não foi selecionado corretamente.");

            var parametros = new List<FbParameter>();

            foreach (var param in relatorio.Parametros)
            {
                if (valores.ContainsKey(param.Nome))
                {
                    parametros.Add(new FbParameter(param.Nome, valores[param.Nome]));
                }
            }

            var dao = new BaseDAO();
            return dao.ExecutarQuery(relatorio.Query, parametros);
        }
        private void ExportarExcel(DataTable tabela)
        {
            using (var wb = new ClosedXML.Excel.XLWorkbook())
            {
                wb.Worksheets.Add(tabela, "Relatório");
                var saveFile = new SaveFileDialog
                {
                    Filter = "Excel Workbook|*.xlsx",
                    FileName = "Relatorio.xlsx"
                };

                if (saveFile.ShowDialog() == DialogResult.OK)
                {
                    wb.SaveAs(saveFile.FileName);
                    MessageBox.Show("Exportado para Excel com sucesso!");
                }
            }
        }

        private void ExportarPdf(DataTable tabela)
        {
            var saveFile = new SaveFileDialog
            {
                Filter = "PDF File|*.pdf",
                FileName = "Relatorio.pdf"
            };

            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                using (var fs = new FileStream(saveFile.FileName, FileMode.Create))
                using (var doc = new iTextSharp.text.Document())
                using (var writer = iTextSharp.text.pdf.PdfWriter.GetInstance(doc, fs))
                {
                    doc.Open();

                    var table = new iTextSharp.text.pdf.PdfPTable(tabela.Columns.Count);

                    // Cabeçalhos
                    foreach (DataColumn col in tabela.Columns)
                        table.AddCell(new iTextSharp.text.Phrase(col.ColumnName));

                    // Linhas
                    foreach (DataRow row in tabela.Rows)
                    {
                        foreach (var item in row.ItemArray)
                            table.AddCell(item?.ToString() ?? "");
                    }

                    doc.Add(table);
                    doc.Close();
                }

                MessageBox.Show("Exportado para PDF com sucesso!");
            }
        }

        private void BtnGerar_Click_1(object sender, EventArgs e)
        {
            if (dgvResultado.DataSource == null)
            {
                MessageBox.Show("Nenhum resultado para exportar.");
                return;
            }

            var item = (ComboItemIcon)CbFormato.SelectedItem;
            string formato = item.Texto;

            if (formato == "Excel")
                ExportarExcel((DataTable)dgvResultado.DataSource);
            else if (formato == "PDF")
                ExportarPdf((DataTable)dgvResultado.DataSource);

        }

        private void CarregarComboBoxEmpresa()
        {
            try
            {
                var config = ConfigHelper.Carregar();
                if (config == null)
                {
                    MessageBox.Show("Banco não configurado.", "Erro");
                    return;
                }

                using (var conn = new FbConnection(
                           $"Database={config.CaminhoBanco};" +
                           $"DataSource={config.Servidor};" +
                           $"User={config.Usuario};" +
                           $"Password={config.Senha};" +
                           $"Port={config.Porta};" +
                           $"Dialect=3;"))
                {
                    conn.Open();

                    string query = "SELECT empresa_id, nom_empresa FROM empresa ORDER BY nom_empresa";
                    using (var cmd = new FbCommand(query, conn))
                    using (var da = new FbDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        da.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
                            CbEmpresa.DataSource = dt;
                            CbEmpresa.DisplayMember = "nom_empresa";
                            CbEmpresa.ValueMember = "empresa_id";
                            CbEmpresa.SelectedIndex = 0;
                        }
                        else
                        {
                            MessageBox.Show("Nenhuma empresa encontrada no banco.", "Informação");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar empresas: " + ex.Message, "Erro");
            }
        }

        private void AplicarTitulos(DataGridView dgv, RelatorioPronto rel)
        {
            foreach (DataGridViewColumn col in dgv.Columns)
            {
                var campo = rel.Campos.FirstOrDefault(c => c.Nome == col.Name);

                if (campo != null)
                    col.HeaderText = campo.Titulo;
            }
        }

        private void PopularColunas(RelatorioPronto rel)
        {
            // Guardar colunas já selecionadas
            var selecionadas = new HashSet<string>();
            foreach (var item in ClbColunas.CheckedItems)
            {
                selecionadas.Add(item.ToString());
            }

            // Evitar limpar se já existem itens (apenas atualizar)
            ClbColunas.BeginUpdate();
            ClbColunas.Items.Clear();

            foreach (var campo in rel.Campos)
            {
                // Marca apenas se estava marcado antes
                bool marcado = selecionadas.Contains(campo.Nome);
                ClbColunas.Items.Add(campo.Nome, marcado);

                // Ajusta visibilidade do DataGridView
                var coluna = dgvResultado.Columns
                    .Cast<DataGridViewColumn>()
                    .FirstOrDefault(c => string.Equals(c.Name, campo.Nome, StringComparison.OrdinalIgnoreCase));

                if (coluna != null)
                    coluna.Visible = marcado;
            }

            ClbColunas.EndUpdate();
        }

        private void FormatarGrid(DataGridView dgv, RelatorioPronto rel)
        {
            foreach (DataGridViewColumn col in dgv.Columns)
            {
                var campo = rel.Campos.FirstOrDefault(c => c.Nome == col.Name);

                if (campo == null) continue;

                if (campo.Tipo == "Decimal")
                    col.DefaultCellStyle.Format = "N2";

                if (campo.Tipo == "DateTime")
                    col.DefaultCellStyle.Format = "dd/MM/yyyy";
            }
        }

        private void clbColunas_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            this.BeginInvoke((MethodInvoker)delegate
            {
                string item = ClbColunas.Items[e.Index].ToString();

                // Procura a coluna do DataGridView com o mesmo nome (ignora case)
                var coluna = dgvResultado.Columns
                    .Cast<DataGridViewColumn>()
                    .FirstOrDefault(c => string.Equals(c.Name, item, StringComparison.OrdinalIgnoreCase));

                if (coluna != null)
                {
                    coluna.Visible = e.NewValue == CheckState.Checked;
                }
            });
        }

        private void BtnVisualizar_Click(object sender, EventArgs e)
        {


            if (CbRelario.SelectedIndex == -1)
            {
                MessageBox.Show("Selecione um relatório.");
                return;
            }

            var relatorio = CbRelario.SelectedItem as RelatorioPronto;

            if (relatorio == null)
            {
                MessageBox.Show("Erro ao obter relatório.");
                return;
            }

            var valores = new Dictionary<string, object>
            {
                { "empresa_id", CbEmpresa.SelectedValue }, // pega o ID selecionado
                { "dataInicial", dtInicio.Value.Date },
                { "dataFinal", dtFim.Value.Date.AddDays(1).AddSeconds(-1) }
            };

            var resultado = ExecutarRelatorio(relatorio, valores);

            MessageBox.Show("Linhas: " + resultado.Rows.Count);

            dgvResultado.DataSource = null;
            dgvResultado.DataSource = resultado;

            AplicarTitulos(dgvResultado, relatorio);
            FormatarGrid(dgvResultado, relatorio);
            PopularColunas(relatorio);
        }

        private void BtnEnviar_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvResultado.DataSource == null)
                {
                    MessageBox.Show("Nenhum relatório gerado para enviar.");
                    return;
                }

                // Carrega a configuração de e-mail salva
                var config = ConfigEmailHelper.Carregar();
                if (config == null)
                {
                    MessageBox.Show("Configure o e-mail antes de enviar.");
                    return;
                }

                // Pergunta o destinatário usando prompt em C#
                string destinatario = PerguntarDestinatario(
                    "Digite o e-mail do destinatário:", "Enviar Relatório", config.Remetente);

                if (string.IsNullOrWhiteSpace(destinatario))
                {
                    MessageBox.Show("Destinatário inválido.");
                    return;
                }

                // Cria um arquivo temporário
                string caminhoArquivo = Path.Combine(Path.GetTempPath(),
                    "Relatorio_" + DateTime.Now.ToString("yyyyMMdd_HHmmss"));

                string formato = CbFormato.SelectedItem.ToString();
                if (formato == "Excel")
                {
                    caminhoArquivo += ".xlsx";
                    using (var wb = new ClosedXML.Excel.XLWorkbook())
                    {
                        wb.Worksheets.Add((DataTable)dgvResultado.DataSource, "Relatório");
                        wb.SaveAs(caminhoArquivo);
                    }
                }
                else if (formato == "PDF")
                {
                    caminhoArquivo += ".pdf";
                    using (var fs = new FileStream(caminhoArquivo, FileMode.Create))
                    using (var doc = new iTextSharp.text.Document())
                    using (var writer = iTextSharp.text.pdf.PdfWriter.GetInstance(doc, fs))
                    {
                        doc.Open();
                        var tabela = new iTextSharp.text.pdf.PdfPTable(((DataTable)dgvResultado.DataSource).Columns.Count);

                        // Cabeçalhos
                        foreach (DataColumn col in ((DataTable)dgvResultado.DataSource).Columns)
                            tabela.AddCell(new iTextSharp.text.Phrase(col.ColumnName));

                        // Linhas
                        foreach (DataRow row in ((DataTable)dgvResultado.DataSource).Rows)
                        {
                            foreach (var item in row.ItemArray)
                                tabela.AddCell(item?.ToString() ?? "");
                        }

                        doc.Add(tabela);
                        doc.Close();
                    }
                }

                // Envia o e-mail
                EmailUtils.EnviarEmail(
                    assunto: "Relatório " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                    corpo: "Segue em anexo o relatório solicitado.",
                    caminhoAnexo: caminhoArquivo,
                    config: config,
                    destinatario: destinatario
                );

                // Remove o arquivo temporário
                if (File.Exists(caminhoArquivo))
                    File.Delete(caminhoArquivo);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao enviar e-mail: " + ex.Message);
            }
        }

        // Função auxiliar para perguntar destinatário
        private string PerguntarDestinatario(string texto, string titulo, string valorPadrao = "")
        {
            Form prompt = new Form()
            {
                Width = 400,
                Height = 150,
                Text = titulo,
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            Label lbl = new Label() { Left = 20, Top = 20, Text = texto, Width = 340 };
            TextBox txt = new TextBox() { Left = 20, Top = 50, Width = 340, Text = valorPadrao };
            Button btnOk = new Button() { Text = "OK", Left = 280, Width = 80, Top = 80, DialogResult = DialogResult.OK };

            prompt.Controls.Add(lbl);
            prompt.Controls.Add(txt);
            prompt.Controls.Add(btnOk);
            prompt.AcceptButton = btnOk;

            return prompt.ShowDialog() == DialogResult.OK ? txt.Text : "";
        }
    }
}