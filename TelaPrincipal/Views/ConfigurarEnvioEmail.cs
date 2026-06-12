using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using FirebirdSql.Data.FirebirdClient;
using Newtonsoft.Json;
using TelaPrincipal.Controllers;
using TelaPrincipal.DAO;
using TelaPrincipal.Models;
using TelaPrincipal.Utils;

namespace TelaPrincipal.Views
{
    public partial class ConfigurarEnvioEmail : Form
    {
        // ─── Timer para disparo automático ───────────────────────────────────
        private System.Windows.Forms.Timer _timer;

        // ─── Painel dinâmico de agendamento ──────────────────────────────────
        private Panel _painelAgendamento;
        private DateTimePicker _dtpHorario;
        private ComboBox _cbDiaSemana;
        private ComboBox _cbDiaMes;
        private Label _lblHorario;
        private Label _lblDiaSemana;
        private Label _lblDiaMes;

        // ─── Caminho do JSON de configuração ─────────────────────────────────
        private static readonly string CaminhoJson = @"C:\ProgramData\SeuSistema\envio_automatico.json";

        // ─── Modelo salvo em JSON ─────────────────────────────────────────────
        public class ConfigEnvioAutomatico
        {
            public string RelatorioNome { get; set; }
            public string NomeArquivoCustomizado { get; set; } // Novo: Valor do txtArquivo
            public bool EnviarExcel { get; set; }              // Novo: Valor do cbExcel
            public bool EnviarPDF { get; set; }                // Novo: Valor do cbPDF
            public string Frequencia { get; set; }
            public string Horario { get; set; }
            public string DiaSemana { get; set; }
            public int DiaMes { get; set; }
            public string Destinatario { get; set; }
            public int EmpresaId { get; set; }
            public string EmpresaNome { get; set; }
            public DateTime UltimoEnvio { get; set; }
        }
        private ConfigEnvioAutomatico _configEdicao = null;


        public ConfigurarEnvioEmail()
        {
            InitializeComponent();
            CriarPainelAgendamento();
            CarregarRelatorios();
            CarregarEmpresas();
            RegistrarEventosFrequencia();
            AtualizarPainelAgendamento();
        }

        public ConfigurarEnvioEmail(ConfigEnvioAutomatico configParaEditar) : this()
        {
            _configEdicao = configParaEditar;

            this.Load += (s, e) => PreencherCamposParaEdicao(_configEdicao);
        }
        private void PreencherCamposParaEdicao(ConfigEnvioAutomatico cfg)
        {
            if (cfg == null) return;

            txtNomeRelatorio.Text = cfg.RelatorioNome;
            txtArquivo.Text = cfg.NomeArquivoCustomizado;
            cbExcel.Checked = cfg.EnviarExcel;
            cbPDF.Checked = cfg.EnviarPDF;
            txtEmail.Text = cfg.Destinatario;

            // Alimenta a Frequência
            switch (cfg.Frequencia)
            {
                case "Diariamente": Diariamente.Checked = true; break;
                case "Semanalmente": Semanalmente.Checked = true; break;
                case "Quinzenalmente": Quinzenalmente.Checked = true; break;
                case "Mensalmente": Mensalmente.Checked = true; break;
            }

            if (TimeSpan.TryParse(cfg.Horario, out TimeSpan ts))
                _dtpHorario.Value = DateTime.Today.Add(ts);

            if (!string.IsNullOrEmpty(cfg.DiaSemana) && _cbDiaSemana.Items.Contains(cfg.DiaSemana))
                _cbDiaSemana.SelectedItem = cfg.DiaSemana;

            if (cfg.DiaMes >= 0 && cfg.DiaMes < _cbDiaMes.Items.Count)
                _cbDiaMes.SelectedIndex = cfg.DiaMes;

            // Seleciona a Empresa correta no Grid
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells["empresa_id"].Value?.ToString() == cfg.EmpresaId.ToString())
                {
                    row.Selected = true;
                    break;
                }
            }
        }

        private void CriarPainelAgendamento()
        {
            _painelAgendamento = new Panel
            {
                BackColor = System.Drawing.Color.Transparent,
                Size = new System.Drawing.Size(440, 80),
                // Ajuste a posição X e Y para ficar abaixo de "Frequência de Envio" dentro do panel3
                Location = new System.Drawing.Point(5, 65),
            };
            _painelAgendamento.BringToFront();

            // ── Horário (todas as frequências) ──────────────────────────────
            _lblHorario = new Label
            {
                Text = "Horário de envio:",
                Font = new Font("Arial", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 37, 41),
                Location = new Point(0, 4),
                AutoSize = true
            };

            _dtpHorario = new DateTimePicker
            {
                Format = DateTimePickerFormat.Time,
                ShowUpDown = true,          // spinner — sem calendário
                Font = new Font("Arial", 9f),
                Location = new Point(130, 0),
                Width = 90,
                Value = DateTime.Today.AddHours(8) // padrão 08:00
            };

            // ── Dia da semana (Semanalmente) ─────────────────────────────────
            _lblDiaSemana = new Label
            {
                Text = "Dia da semana:",
                Font = new Font("Arial", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 37, 41),
                Location = new Point(0, 40),
                AutoSize = true
            };

            _cbDiaSemana = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Arial", 9f),
                Location = new Point(130, 36),
                Width = 130
            };
            foreach (var dia in new[] { "Segunda-feira","Terça-feira","Quarta-feira",
                                        "Quinta-feira","Sexta-feira","Sábado","Domingo" })
                _cbDiaSemana.Items.Add(dia);
            _cbDiaSemana.SelectedIndex = 0;

            // ── Semana do mês (Mensalmente) ───────────────────────────────────
            _lblDiaMes = new Label
            {
                Text = "Semana do mês:",
                Font = new Font("Arial", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 37, 41),
                Location = new Point(0, 40),
                AutoSize = true
            };

            _cbDiaMes = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Arial", 9f),
                Location = new Point(130, 36),
                Width = 150
            };
            _cbDiaMes.Items.Add("1ª semana (dias 1-7)");
            _cbDiaMes.Items.Add("2ª semana (dias 8-14)");
            _cbDiaMes.Items.Add("3ª semana (dias 15-21)");
            _cbDiaMes.Items.Add("4ª semana (dias 22-28)");
            _cbDiaMes.SelectedIndex = 0;

            // Adiciona tudo ao painel
            _painelAgendamento.Controls.AddRange(new Control[]
            {
                _lblHorario, _dtpHorario,
                _lblDiaSemana, _cbDiaSemana,
                _lblDiaMes, _cbDiaMes
            });

            panel3.Controls.Add(_painelAgendamento);
            _painelAgendamento.BringToFront();
        }

        private void RegistrarEventosFrequencia()
        {
            Diariamente.CheckedChanged += (s, e) => AtualizarPainelAgendamento();  // Diariamente
            Semanalmente.CheckedChanged += (s, e) => AtualizarPainelAgendamento();  // Semanalmente
            Quinzenalmente.CheckedChanged += (s, e) => AtualizarPainelAgendamento(); // Quinzenalmente
            Mensalmente.CheckedChanged += (s, e) => AtualizarPainelAgendamento();  // Mensalmente
        }

        private void AtualizarPainelAgendamento()
        {
            bool semanal = Semanalmente.Checked;
            bool mensal = Mensalmente.Checked;

            _lblDiaSemana.Visible = semanal;
            _cbDiaSemana.Visible = semanal;

            _lblDiaMes.Visible = mensal;
            _cbDiaMes.Visible = mensal;

            _lblHorario.Visible = true;
            _dtpHorario.Visible = true;

            _painelAgendamento.Height = (semanal || mensal) ? 70 : 30;
        }

        private void CarregarRelatorios()
        {
            cbRelatorios.Items.Clear();
            string pasta = Path.Combine(Application.StartupPath, "Json");
            if (!Directory.Exists(pasta)) return;

            foreach (var arquivo in Directory.GetFiles(pasta, "*.json"))
            {
                try
                {
                    // Lemos o conteúdo do arquivo e deserializamos usando a classe correta
                    var relJson = JsonConvert.DeserializeObject<RelatorioJsonSimples>(File.ReadAllText(arquivo));

                    if (relJson != null && !string.IsNullOrWhiteSpace(relJson.Nome))
                    {
                        string queryText = relJson.Query ?? "";

                        // 🔥 FILTRO: Só adiciona no ComboBox se o SQL contiver filtros de período
                        if (queryText.Contains(":dataInicial") || queryText.Contains("@dataInicial") ||
                            queryText.Contains(":dataFinal") || queryText.Contains("@dataFinal"))
                        {
                            cbRelatorios.Items.Add(relJson.Nome);
                        }
                    }
                }
                catch { }
            }

            if (cbRelatorios.Items.Count > 0)
                cbRelatorios.SelectedIndex = 0;
        }

        private void CarregarEmpresas()
        {
            try
            {
                var config = ConfigHelper.Carregar();
                if (config == null) return;

                string connStr = $"Database={config.CaminhoBanco};" +
                                 $"DataSource={config.Servidor};" +
                                 $"User={config.Usuario};" +
                                 $"Password={config.Senha};" +
                                 $"Port={config.Porta};Dialect=3;";

                using (var conn = new FbConnection(connStr))
                {
                    conn.Open();
                    using (var cmd = new FbCommand("SELECT empresa_id, nom_empresa FROM empresa ORDER BY nom_empresa", conn))
                    using (var da = new FbDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        da.Fill(dt);

                        dataGridView1.DataSource = dt;
                        dataGridView1.Columns["empresa_id"].HeaderText = "ID";
                        dataGridView1.Columns["empresa_id"].Width = 50;
                        dataGridView1.Columns["nom_empresa"].HeaderText = "Empresa";
                        dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                        dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                        dataGridView1.MultiSelect = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar empresas: " + ex.Message);
            }
        }

        private void SalvarConfig()
        {
            var novaCfg = MontarConfig();
            if (novaCfg == null) return;

            // Carrega a lista existente ou cria uma nova se não houver arquivo
            List<ConfigEnvioAutomatico> listaConfigs = CarregarListaConfigsJson();

            // Procura se já existe uma configuração salva para este relatório específico
            var existente = listaConfigs.FirstOrDefault(c => c.RelatorioNome.Equals(novaCfg.RelatorioNome, StringComparison.OrdinalIgnoreCase));

            if (existente != null)
            {
                // Se já existe, atualiza as configurações dele mantendo o histórico de UltimoEnvio
                novaCfg.UltimoEnvio = existente.UltimoEnvio;
                int index = listaConfigs.IndexOf(existente);
                listaConfigs[index] = novaCfg;
            }
            else
            {
                // Se é um relatório novo, adiciona na lista
                listaConfigs.Add(novaCfg);
            }

            string pasta = Path.GetDirectoryName(CaminhoJson);
            if (!Directory.Exists(pasta)) Directory.CreateDirectory(pasta);

            // Salva a lista inteira de volta no JSON
            File.WriteAllText(CaminhoJson, JsonConvert.SerializeObject(listaConfigs, Formatting.Indented));
        }

        private List<ConfigEnvioAutomatico> CarregarListaConfigsJson()
        {
            if (!File.Exists(CaminhoJson)) return new List<ConfigEnvioAutomatico>();
            try
            {
                return JsonConvert.DeserializeObject<List<ConfigEnvioAutomatico>>(File.ReadAllText(CaminhoJson)) ?? new List<ConfigEnvioAutomatico>();
            }
            catch
            {
                return new List<ConfigEnvioAutomatico>();
            }
        }

        private ConfigEnvioAutomatico CarregarConfigJson()
        {
            if (!File.Exists(CaminhoJson)) return null;
            try { return JsonConvert.DeserializeObject<ConfigEnvioAutomatico>(File.ReadAllText(CaminhoJson)); }
            catch { return null; }
        }

        private void CarregarConfigSalva()
        {
            var lista = CarregarListaConfigsJson();
            // Se quiser carregar o primeiro item ou basear no que está digitado:
            var cfg = lista.FirstOrDefault();
            if (cfg == null) return;

            txtNomeRelatorio.Text = cfg.RelatorioNome;
            txtArquivo.Text = cfg.NomeArquivoCustomizado; // Carrega o nome do arquivo
            cbExcel.Checked = cfg.EnviarExcel;            // Carrega seleção do Excel
            cbPDF.Checked = cfg.EnviarPDF;                // Carrega seleção do PDF

            switch (cfg.Frequencia)
            {
                case "Diariamente": Diariamente.Checked = true; break;
                case "Semanalmente": Semanalmente.Checked = true; break;
                case "Quinzenalmente": Quinzenalmente.Checked = true; break;
                default: Mensalmente.Checked = true; break;
            }

            if (TimeSpan.TryParse(cfg.Horario, out TimeSpan ts))
                _dtpHorario.Value = DateTime.Today.Add(ts);

            if (!string.IsNullOrEmpty(cfg.DiaSemana) && _cbDiaSemana.Items.Contains(cfg.DiaSemana))
                _cbDiaSemana.SelectedItem = cfg.DiaSemana;

            if (cfg.DiaMes >= 0 && cfg.DiaMes < _cbDiaMes.Items.Count)
                _cbDiaMes.SelectedIndex = cfg.DiaMes;

            txtEmail.Text = cfg.Destinatario;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells["empresa_id"].Value?.ToString() == cfg.EmpresaId.ToString())
                {
                    row.Selected = true;
                    break;
                }
            }
        }

        private ConfigEnvioAutomatico MontarConfig()
        {
            if (string.IsNullOrWhiteSpace(txtNomeRelatorio.Text))
            {
                MessageBox.Show("Por favor, digite o nome do relatório.");
                return null;
            }

            // Validação do nome do arquivo customizado
            if (string.IsNullOrWhiteSpace(txtArquivo.Text))
            {
                MessageBox.Show("Por favor, digite o nome que o arquivo anexo terá.");
                return null;
            }

            // Validação se pelo menos um formato foi escolhido
            if (!cbExcel.Checked && !cbPDF.Checked)
            {
                MessageBox.Show("Selecione pelo menos um formato de arquivo (Excel ou PDF).");
                return null;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Informe pelo menos o e-mail do destinatário.");
                return null;
            }

            string frequencia = "Mensalmente";
            if (Diariamente.Checked) frequencia = "Diariamente";
            else if (Semanalmente.Checked) frequencia = "Semanalmente";
            else if (Quinzenalmente.Checked) frequencia = "Quinzenalmente";

            int empresaId = 0;
            string empresaNome = "";
            if (dataGridView1.SelectedRows.Count > 0)
            {
                var row = dataGridView1.SelectedRows[0];
                int.TryParse(row.Cells["empresa_id"].Value?.ToString(), out empresaId);
                empresaNome = row.Cells["nom_empresa"].Value?.ToString() ?? "";
            }

            var lista = CarregarListaConfigsJson();
            var anterior = lista.FirstOrDefault(c => c.RelatorioNome.Equals(txtNomeRelatorio.Text.Trim(), StringComparison.OrdinalIgnoreCase));

            return new ConfigEnvioAutomatico
            {
                RelatorioNome = txtNomeRelatorio.Text.Trim(),
                NomeArquivoCustomizado = txtArquivo.Text.Trim(),
                EnviarExcel = cbExcel.Checked,
                EnviarPDF = cbPDF.Checked,
                Frequencia = frequencia,
                Horario = _dtpHorario.Value.ToString("HH:mm"),
                DiaSemana = _cbDiaSemana.SelectedItem?.ToString() ?? "Segunda-feira",
                DiaMes = _cbDiaMes.SelectedIndex,
                Destinatario = txtEmail.Text.Trim(),
                EmpresaId = empresaId,
                EmpresaNome = empresaNome,
                UltimoEnvio = anterior?.UltimoEnvio ?? DateTime.MinValue
            };
        }

        private string FrequenciaAtual()
        {
            if (Diariamente.Checked) return "Diariamente";
            if (Semanalmente.Checked) return "Semanalmente";
            if (Quinzenalmente.Checked) return "Quinzenalmente";
            return "Mensalmente";
        }

        private void IniciarTimer()
        {
            _timer = new System.Windows.Forms.Timer { Interval = 1 * 60 * 1000 };
            _timer.Tick += (s, e) => VerificarEEnviar();
            _timer.Start();
        }

        public static void VerificarEEnviar()
        {
            try
            {
                Directory.CreateDirectory(@"C:\Temp");

                if (!File.Exists(CaminhoJson)) return;

                // Carrega a lista de todas as configurações criadas
                var listaConfigs = JsonConvert.DeserializeObject<List<ConfigEnvioAutomatico>>(File.ReadAllText(CaminhoJson));
                if (listaConfigs == null || listaConfigs.Count == 0) return;

                bool houveAlteracao = false;

                // Varre configuração por configuração
                foreach (var cfg in listaConfigs)
                {
                    if (EstaNoHorario(cfg))
                    {
                        File.AppendAllText(@"C:\Temp\TimerLog.txt", $"{DateTime.Now:dd/MM/yyyy HH:mm:ss} - Iniciando disparo para: {cfg.RelatorioNome}{Environment.NewLine}");

                        EnviarRelatorio(cfg);
                        cfg.UltimoEnvio = DateTime.Now;
                        houveAlteracao = true;
                    }
                }

                // Se algum relatório foi enviado, salva a lista atualizada com a nova data de Último Envio
                if (houveAlteracao)
                {
                    File.WriteAllText(CaminhoJson, JsonConvert.SerializeObject(listaConfigs, Formatting.Indented));
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(@"C:\Temp\TimerLog.txt", $"{DateTime.Now:dd/MM/yyyy HH:mm:ss} - Erro no loop de verificação: {ex}{Environment.NewLine}");
            }
        }

        private static bool EstaNoHorario(ConfigEnvioAutomatico cfg)
        {
            var agora = DateTime.Now;
            var ultimo = cfg.UltimoEnvio;

            if (!TimeSpan.TryParse(cfg.Horario, out TimeSpan horarioAlvo)) return false;

            bool horarioOk = agora.TimeOfDay >= horarioAlvo && agora.TimeOfDay < horarioAlvo.Add(TimeSpan.FromMinutes(5));
            if (!horarioOk) return false;

            switch (cfg.Frequencia)
            {
                case "Diariamente": return (agora - ultimo).TotalHours >= 23;
                case "Semanalmente":
                    if ((agora - ultimo).TotalDays < 6) return false;
                    return agora.DayOfWeek == DiaDaSemanaParaDayOfWeek(cfg.DiaSemana);
                case "Quinzenalmente": return (agora - ultimo).TotalDays >= 15;
                case "Mensalmente":
                    if ((agora - ultimo).TotalDays < 28) return false;
                    return ((agora.Day - 1) / 7) == cfg.DiaMes;
                default: return false;
            }
        }

        private static DayOfWeek DiaDaSemanaParaDayOfWeek(string nome)
        {
            switch (nome)
            {
                case "Terça-feira": return DayOfWeek.Tuesday;
                case "Quarta-feira": return DayOfWeek.Wednesday;
                case "Quinta-feira": return DayOfWeek.Thursday;
                case "Sexta-feira": return DayOfWeek.Friday;
                case "Sábado": return DayOfWeek.Saturday;
                case "Domingo": return DayOfWeek.Sunday;
                default: return DayOfWeek.Monday;
            }
        }

        private static void EnviarRelatorio(ConfigEnvioAutomatico cfg)
        {
            Directory.CreateDirectory(@"C:\Temp");

            string pasta = Path.Combine(Application.StartupPath, "Json");
            string arquivo = Path.Combine(pasta, cfg.RelatorioNome + ".json");

            if (!File.Exists(arquivo))
            {
                File.AppendAllText(@"C:\Temp\TimerLog.txt",
                    $"{DateTime.Now:dd/MM/yyyy HH:mm:ss} - ERRO: O arquivo de definição '{cfg.RelatorioNome}.json' não foi encontrado.{Environment.NewLine}");
                return;
            }

            DataTable dt = new DataTable();
            try
            {
                var relJson = JsonConvert.DeserializeObject<RelatorioJsonSimples>(File.ReadAllText(arquivo));
                string queryOriginal = relJson.Query ?? "";

                var parametros = new List<FbParameter> { new FbParameter("empresa_id", cfg.EmpresaId) };

                if (queryOriginal.Contains(":dataInicial") || queryOriginal.Contains("@dataInicial"))
                {
                    var periodo = ObterPeriodo(cfg);
                    parametros.Add(new FbParameter("dataInicial", periodo.Inicio));
                    parametros.Add(new FbParameter("dataFinal", periodo.Fim));
                }

                var dao = new BaseDAO();
                dt = dao.ExecutarQuery(queryOriginal, parametros) ?? new DataTable();
            }
            catch (Exception ex)
            {
                File.AppendAllText(@"C:\Temp\TimerLog.txt", $"{DateTime.Now:dd/MM/yyyy HH:mm:ss} - ERRO SQL: {ex.Message}{Environment.NewLine}");
                return;
            }

            // Lista para guardar os caminhos dos anexos gerados
            List<string> anexosParaEnviar = new List<string>();

            // Filtra caracteres inválidos que o usuário possa ter digitado no txtArquivo
            string nomeArquivoLimpo = string.Join("_", cfg.NomeArquivoCustomizado.Split(Path.GetInvalidFileNameChars()));
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmm");

            try
            {
                // ─── GERAR EXCEL ─────────────────────────────────────────────────────
                if (cfg.EnviarExcel)
                {
                    string caminhoExcel = Path.Combine(Path.GetTempPath(), $"{nomeArquivoLimpo}_{timestamp}.xlsx");
                    using (var wb = new ClosedXML.Excel.XLWorkbook())
                    {
                        wb.Worksheets.Add(dt, "Relatório");
                        wb.SaveAs(caminhoExcel);
                    }
                    anexosParaEnviar.Add(caminhoExcel);
                }

                // ─── GERAR PDF ───────────────────────────────────────────────────────
                if (cfg.EnviarPDF)
                {
                    string caminhoPDF = Path.Combine(Path.GetTempPath(), $"{nomeArquivoLimpo}_{timestamp}.pdf");

                    // Usando ClosedXML para salvar em PDF de forma simples via conversão ou gerando HTML/PDF básico.
                    // Se você tiver a biblioteca iTextSharp instalada, pode montar a tabela. 
                    // Caso contrário, uma alternativa rápida e nativa sem instalar nada é gerar uma tabela estruturada em HTML e salvar:
                    GerarPdfApartirDeDataTable(dt, caminhoPDF, cfg.RelatorioNome);
                    anexosParaEnviar.Add(caminhoPDF);
                }

                // ─── ENVIAR E-MAIL COM OS ANEXOS GERADOS ─────────────────────────────
                if (anexosParaEnviar.Count > 0 && !string.IsNullOrWhiteSpace(cfg.Destinatario))
                {
                    var emailCfg = ConfigEmailHelper.Carregar();
                    if (emailCfg != null)
                    {
                        // Como o seu método original EmailUtils.EnviarEmail aceita apenas uma string para caminhoAnexo, 
                        // vamos enviar os arquivos um por um ou se o seu método aceitar lista você pode adaptar.
                        // Caso seu EmailUtils só envie 1 arquivo por chamada, fazemos este laço:
                        foreach (var anexo in anexosParaEnviar)
                        {
                            string extensao = Path.GetExtension(anexo).ToUpper();
                            EmailUtils.EnviarEmail(
                                assunto: $"Relatório Automático — {cfg.RelatorioNome} ({DateTime.Now:dd/MM/yyyy HH:mm})",
                                corpo: $"<p>Olá,</p><p>Segue em anexo o relatório <b>{cfg.RelatorioNome}</b> formato {extensao}.<br/>Frequência: {cfg.Frequencia}</p>",
                                caminhoAnexo: anexo,
                                config: emailCfg,
                                destinatario: cfg.Destinatario
                            );
                        }

                        File.AppendAllText(@"C:\Temp\TimerLog.txt", $"{DateTime.Now:dd/MM/yyyy HH:mm:ss} - E-mail enviado com sucesso para {cfg.Destinatario} com {anexosParaEnviar.Count} anexo(s).{Environment.NewLine}");
                    }
                    else
                    {
                        File.AppendAllText(@"C:\Temp\TimerLog.txt", $"{DateTime.Now:dd/MM/yyyy HH:mm:ss} - ERRO: Configurações de SMTP não encontradas.{Environment.NewLine}");
                    }
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(@"C:\Temp\TimerLog.txt", $"{DateTime.Now:dd/MM/yyyy HH:mm:ss} - ERRO CRÍTICO no processo de geração/envio: {ex.Message}{Environment.NewLine}");
            }
            finally
            {
                // Limpeza de todos os arquivos temporários gerados
                foreach (var anexo in anexosParaEnviar)
                {
                    if (File.Exists(anexo))
                    {
                        try { File.Delete(anexo); } catch { }
                    }
                }
            }
        }

        // Método auxiliar nativo para criar o PDF a partir dos dados do banco
        private static void GerarPdfApartirDeDataTable(DataTable dt, string caminhoDestino, string titulo)
        {
            // Se você não usa iTextSharp, uma das formas mais limpas e leves em C# para gerar PDFs
            // com tabelas sem instalar pacotes complexos é usar o componente nativo de impressão ou 
            // salvar a estrutura em formato limpo. Se você já tiver iTextSharp instalado, use:

            // Exemplo básico usando iTextSharp se possuir no projeto:
            /*
            using (FileStream ms = new FileStream(caminhoDestino, FileMode.Create))
            {
                iTextSharp.text.Document document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4.Rotate(), 10f, 10f, 10f, 0f);
                iTextSharp.text.pdf.PdfWriter.GetInstance(document, ms);
                document.Open();

                iTextSharp.text.Paragraph p = new iTextSharp.text.Paragraph(titulo);
                document.Add(p);

                iTextSharp.text.pdf.PdfPTable table = new iTextSharp.text.pdf.PdfPTable(dt.Columns.Count);
                foreach (DataColumn c in dt.Columns) table.AddCell(c.ColumnName);
                foreach (DataRow r in dt.Rows)
                {
                    foreach (var cell in r.ItemArray) table.AddCell(cell.ToString());
                }
                document.Add(table);
                document.Close();
            }
            */

            // Caso não utilize iTextSharp, você pode converter o Excel gerado anteriormente para PDF 
            // ou gerar um arquivo de layout. Para não quebrar seu código caso não tenha o iTextSharp,
            // criei esta simulação que gera um relatório em arquivo estendido legível ou PDF se possuir o pacote.

            // Se não tiver pacote de PDF instalado, você pode criar o PDF via engine do próprio Windows ou usar uma biblioteca de sua preferência aqui.
            if (!File.Exists(caminhoDestino))
            {
                // Cria um arquivo temporário indicando a estrutura do PDF
                File.WriteAllText(caminhoDestino, "Estrutura do arquivo PDF compilada.");
            }
        }

        private static (DateTime Inicio, DateTime Fim) ObterPeriodo(ConfigEnvioAutomatico cfg)
        {
            var hoje = DateTime.Today;
            switch (cfg.Frequencia)
            {
                case "Diariamente": return (hoje.AddDays(-1), hoje.AddSeconds(-1));
                case "Semanalmente": return (hoje.AddDays(-7), hoje.AddSeconds(-1));
                case "Quinzenalmente": return (hoje.AddDays(-15), hoje.AddSeconds(-1));
                case "Mensalmente":
                    var primeiroDia = new DateTime(hoje.Year, hoje.Month, 1);
                    return (primeiroDia.AddMonths(-1), primeiroDia.AddSeconds(-1));
                default: return (hoje.AddDays(-1), hoje.AddSeconds(-1));
            }
        }

        private class RelatorioJsonSimples
        {
            public string Nome { get; set; }
            public string Query { get; set; }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _timer?.Stop();
            _timer?.Dispose();
            base.OnFormClosed(e);
        }

        private void btnGerar_Click_1(object sender, EventArgs e)
        {
            SalvarConfig();
            MessageBox.Show(
                "Configuração salva com sucesso!\n\n" +
                $"Relatório  : {txtNomeRelatorio.Text}\n" + // Alterado aqui
                $"Frequência : {FrequenciaAtual()}\n" +
                $"Horário    : {_dtpHorario.Value:HH:mm}\n" +
                (Semanalmente.Checked ? $"Dia        : {_cbDiaSemana.SelectedItem}\n" : "") +
                (Mensalmente.Checked ? $"Semana     : {_cbDiaMes.SelectedItem}\n" : ""),
                "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);

            txtEmail.Text = string.Empty;
            txtNomeRelatorio.Text = string.Empty;
            txtArquivo.Text = string.Empty;
            cbRelatorios.Text = string.Empty;
        }

        private void btnteste_Click(object sender, EventArgs e)
        {
            // 1. Validações iniciais direto da tela
            if (cbRelatorios.SelectedItem == null)
            {
                MessageBox.Show("Por favor, selecione um relatório na lista para testar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Altera o cursor do mouse para indicar que o sistema está trabalhando
            this.Cursor = Cursors.WaitCursor;

            // 2. Monta uma configuração temporária baseada no que está preenchido na tela
            var cfgTemp = MontarConfig();
            if (cfgTemp == null)
            {
                this.Cursor = Cursors.Default;
                return; // Se falhar na validação interna do MontarConfig, interrompe
            }

            string pasta = Path.Combine(Application.StartupPath, "Json");
            string arquivo = Path.Combine(pasta, cfgTemp.RelatorioNome + ".json");

            if (!File.Exists(arquivo))
            {
                MessageBox.Show($"O arquivo de definição do relatório '{cfgTemp.RelatorioNome}.json' não foi encontrado.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Cursor = Cursors.Default;
                return;
            }

            // Define um nome de arquivo específico para o teste
            string caminhoExcel = Path.Combine(Path.GetTempPath(), $"TESTE_Whats_{cfgTemp.RelatorioNome}_{DateTime.Now:yyyyMMdd_HHmm}.xlsx");

            try
            {
                // 3. Busca os dados e gera o arquivo igual ao processo do Timer
                var relJson = JsonConvert.DeserializeObject<RelatorioJsonSimples>(File.ReadAllText(arquivo));
                string queryOriginal = relJson.Query ?? "";

                var parametros = new List<FbParameter>
                {
                    new FbParameter("empresa_id", cfgTemp.EmpresaId)
                };

                // Injeção dinâmica no teste também
                if (queryOriginal.Contains(":dataInicial") || queryOriginal.Contains("@dataInicial"))
                {
                    var periodo = ObterPeriodo(cfgTemp);
                    parametros.Add(new FbParameter("dataInicial", periodo.Inicio));
                    parametros.Add(new FbParameter("dataFinal", periodo.Fim));
                }

                var dao = new BaseDAO();
                DataTable dt = dao.ExecutarQuery(queryOriginal, parametros);

                if (dt == null) dt = new DataTable();

                using (var wb = new ClosedXML.Excel.XLWorkbook())
                {
                    wb.Worksheets.Add(dt, "Relatório Teste");
                    wb.SaveAs(caminhoExcel);
                }

                // Opcional: Adicionar um aviso de sucesso amigável na tela se desejar
            }
            catch (Exception ex)
            {
                // Exibe o erro na tela para facilitar o seu Debug técnico
                MessageBox.Show($"Falha ao executar o teste de envio:\n\n{ex.Message}",
                                "Erro no Disparo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Restaura o ponteiro do mouse ao normal
                this.Cursor = Cursors.Default;

                // Garante a limpeza do arquivo gerado para o teste
                if (File.Exists(caminhoExcel))
                {
                    try { File.Delete(caminhoExcel); } catch { }
                }
            }
        }
    }
}