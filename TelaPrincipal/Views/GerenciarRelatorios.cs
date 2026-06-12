using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using FirebirdSql.Data.FirebirdClient;
using Newtonsoft.Json;
using TelaPrincipal.Utils;

namespace TelaPrincipal.Views
{
    public partial class GerenciarRelatorios : Form
    {
        // ─── VARIÁVEIS GLOBAIS ────────────────────────────────────────────────

        // Guarda o nome do relatório que está sendo editado. Null = modo NOVO.
        private string _relatorioEmEdicao = null;

        // Caminho do arquivo JSON de agendamentos
        private static readonly string CaminhoJson = @"C:\ProgramData\SeuSistema\envio_automatico.json";

        // ─── PAINEL DINÂMICO DE AGENDAMENTO ──────────────────────────────────
        private Panel _painelAgendamento;
        private Label _lblHorario;
        private Label _lblDiaSemana;
        private Label _lblDiaMes;

        // ─── MODELO ──────────────────────────────────────────────────────────
        public class ConfigEnvioAutomatico
        {
            public string RelatorioNome { get; set; }
            public string NomeArquivoCustomizado { get; set; }
            public bool EnviarExcel { get; set; }
            public bool EnviarPDF { get; set; }
            public string Frequencia { get; set; }
            public string Horario { get; set; }
            public string DiaSemana { get; set; }
            public int DiaMes { get; set; }
            public string Destinatario { get; set; }
            public int EmpresaId { get; set; }
            public string EmpresaNome { get; set; }
            public DateTime UltimoEnvio { get; set; }
        }

        private class RelatorioJsonSimples
        {
            public string Nome { get; set; }
            public string Query { get; set; }
        }

        public GerenciarRelatorios()
        {
            InitializeComponent();

            // 1. Configura o grid de agendamentos (colunas)
            ConfigurarGridAgendamentos();

            // 2. Cria o painel dinâmico de horário/dia
            CriarPainelAgendamento();

            // 3. Carrega dados estáticos (relatórios e empresas)
            CarregarRelatoriosDisponiveis();
            CarregarEmpresas();

            // 4. Registra eventos ANTES de atualizar o grid
            dgvConfigurados.CellClick += DgvConfigurados_CellClick;
            RegistrarEventosFrequencia();

            // 5. Preenche o grid de agendamentos com todos os itens do JSON
            AtualizarListaConfiguracoes();

            // 6. Limpa os campos do formulário (modo NOVO)
            LimparCampos();

            // 7. Painel de frequência começa no estado correto
            AtualizarPaineisFrequencia();
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

            // ── Label Horário ──────────────────────────────────────────────
            _lblHorario = new Label
            {
                Text = "Horário de envio:",
                Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Bold),
                Location = new System.Drawing.Point(0, 6),
                AutoSize = true
            };

            _dtpHorario = new DateTimePicker
            {
                Format = DateTimePickerFormat.Time,
                ShowUpDown = true,
                Font = new System.Drawing.Font("Arial", 9f),
                Location = new System.Drawing.Point(140, 2),
                Width = 90,
                Value = DateTime.Today.AddHours(8)
            };

            // ── Label Dia da Semana ────────────────────────────────────────
            _lblDiaSemana = new Label
            {
                Text = "Dia da semana:",
                Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Bold),
                Location = new System.Drawing.Point(0, 42),
                AutoSize = true
            };

            _cbDiaSemana = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new System.Drawing.Font("Arial", 9f),
                Location = new System.Drawing.Point(140, 38),
                Width = 150
            };
            foreach (var dia in new[]
            {
                "Segunda-feira","Terça-feira","Quarta-feira",
                "Quinta-feira","Sexta-feira","Sábado","Domingo"
            })
                _cbDiaSemana.Items.Add(dia);
            _cbDiaSemana.SelectedIndex = 0;

            // ── Label Semana do Mês ────────────────────────────────────────
            _lblDiaMes = new Label
            {
                Text = "Semana do mês:",
                Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Bold),
                Location = new System.Drawing.Point(0, 42),
                AutoSize = true
            };

            _cbDiaMes = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new System.Drawing.Font("Arial", 9f),
                Location = new System.Drawing.Point(140, 38),
                Width = 170
            };
            _cbDiaMes.Items.Add("1ª semana (dias 1-7)");
            _cbDiaMes.Items.Add("2ª semana (dias 8-14)");
            _cbDiaMes.Items.Add("3ª semana (dias 15-21)");
            _cbDiaMes.Items.Add("4ª semana (dias 22-28)");
            _cbDiaMes.SelectedIndex = 0;

            _painelAgendamento.Controls.AddRange(new Control[]
            {
                _lblHorario, _dtpHorario,
                _lblDiaSemana, _cbDiaSemana,
                _lblDiaMes,   _cbDiaMes
            });

            panel3.Controls.Add(_painelAgendamento);
            _painelAgendamento.BringToFront();
        }

        // =====================================================================
        //  GRADE DE AGENDAMENTOS
        // =====================================================================
        private void ConfigurarGridAgendamentos()
        {
            dgvConfigurados.Columns.Clear();
            dgvConfigurados.Columns.Add("colRelatorio", "Relatório");
            dgvConfigurados.Columns.Add("colFrequencia", "Frequência");
            dgvConfigurados.Columns.Add("colHorario", "Horário");
            dgvConfigurados.Columns.Add("colDestinatario", "Destinatário");

            dgvConfigurados.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvConfigurados.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvConfigurados.MultiSelect = false;
            dgvConfigurados.AllowUserToAddRows = false;
            dgvConfigurados.ReadOnly = true;
        }

        // Lê o JSON e popula o grid — chamado sempre que há mudança no JSON
        private void AtualizarListaConfiguracoes()
        {
            dgvConfigurados.Rows.Clear();

            var lista = CarregarListaConfigsJson();

            foreach (var cfg in lista)
            {
                dgvConfigurados.Rows.Add(
                    cfg.RelatorioNome,
                    cfg.Frequencia,
                    cfg.Horario,
                    cfg.Destinatario
                );
            }
        }

        // =====================================================================
        //  EVENTO DE CLIQUE NA GRADE — preenche os campos para edição
        // =====================================================================
        private void DgvConfigurados_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Ignora clique no cabeçalho
            if (e.RowIndex < 0) return;

            string nomeSelecionado = dgvConfigurados.Rows[e.RowIndex].Cells[0].Value?.ToString();
            if (string.IsNullOrEmpty(nomeSelecionado)) return;

            var lista = CarregarListaConfigsJson();
            var cfg = lista.FirstOrDefault(c =>
                c.RelatorioNome.Equals(nomeSelecionado, StringComparison.OrdinalIgnoreCase));

            if (cfg == null) return;

            // Marca que estamos EDITANDO este relatório
            _relatorioEmEdicao = cfg.RelatorioNome;

            // ── Desliga eventos de frequência para não disparar AtualizarPaineisFrequencia
            //    enquanto os campos ainda estão sendo populados
            Diariamente.CheckedChanged -= OnFrequenciaChanged;
            Semanalmente.CheckedChanged -= OnFrequenciaChanged;
            Quinzenalmente.CheckedChanged -= OnFrequenciaChanged;
            Mensalmente.CheckedChanged -= OnFrequenciaChanged;

            // ─── 1. Relatório no ComboBox ──────────────────────────────────
            if (!cbRelatorios.Items.Contains(cfg.RelatorioNome))
                cbRelatorios.Items.Add(cfg.RelatorioNome);
            cbRelatorios.SelectedItem = cfg.RelatorioNome;
            // Fallback se SelectedItem não funcionar (texto livre)
            if (cbRelatorios.SelectedItem?.ToString() != cfg.RelatorioNome)
                cbRelatorios.Text = cfg.RelatorioNome;

            // ─── 2. Campos de texto ───────────────────────────────────────
            txtNomeRelatorio.Text = cfg.NomeArquivoCustomizado ?? string.Empty;
            txtEmail.Text = cfg.Destinatario ?? string.Empty;

            // ─── 3. Checkboxes Excel / PDF ────────────────────────────────
            cbExcel.Checked = cfg.EnviarExcel;
            cbPDF.Checked = cfg.EnviarPDF;

            // ─── 4. Frequência ────────────────────────────────────────────
            Diariamente.Checked = false;
            Semanalmente.Checked = false;
            Quinzenalmente.Checked = false;
            Mensalmente.Checked = false;

            switch (cfg.Frequencia)
            {
                case "Diariamente": Diariamente.Checked = true; break;
                case "Semanalmente": Semanalmente.Checked = true; break;
                case "Quinzenalmente": Quinzenalmente.Checked = true; break;
                default: Mensalmente.Checked = true; break;
            }

            // ─── 5. Horário ───────────────────────────────────────────────
            if (TimeSpan.TryParse(cfg.Horario, out TimeSpan ts))
                _dtpHorario.Value = DateTime.Today.Add(ts);

            // ─── 6. Dia da semana / Semana do mês ────────────────────────
            if (!string.IsNullOrEmpty(cfg.DiaSemana) && _cbDiaSemana.Items.Contains(cfg.DiaSemana))
                _cbDiaSemana.SelectedItem = cfg.DiaSemana;

            if (cfg.DiaMes >= 0 && cfg.DiaMes < _cbDiaMes.Items.Count)
                _cbDiaMes.SelectedIndex = cfg.DiaMes;

            // ─── 7. Empresa no grid ───────────────────────────────────────
            dataGridView1.ClearSelection();
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells["empresa_id"].Value?.ToString() == cfg.EmpresaId.ToString())
                {
                    row.Selected = true;
                    dataGridView1.FirstDisplayedScrollingRowIndex = row.Index;
                    break;
                }
            }

            // Religa os eventos e atualiza visibilidade dos painéis
            Diariamente.CheckedChanged += OnFrequenciaChanged;
            Semanalmente.CheckedChanged += OnFrequenciaChanged;
            Quinzenalmente.CheckedChanged += OnFrequenciaChanged;
            Mensalmente.CheckedChanged += OnFrequenciaChanged;

            AtualizarPaineisFrequencia();
        }

        // =====================================================================
        //  FREQUÊNCIA — visibilidade dos controles dinâmicos
        // =====================================================================
        private void RegistrarEventosFrequencia()
        {
            Diariamente.CheckedChanged += OnFrequenciaChanged;
            Semanalmente.CheckedChanged += OnFrequenciaChanged;
            Quinzenalmente.CheckedChanged += OnFrequenciaChanged;
            Mensalmente.CheckedChanged += OnFrequenciaChanged;
        }

        private void OnFrequenciaChanged(object sender, EventArgs e)
        {
            AtualizarPaineisFrequencia();
        }

        private void AtualizarPaineisFrequencia()
        {
            bool semanal = Semanalmente.Checked;
            bool mensal = Mensalmente.Checked;

            _lblDiaSemana.Visible = semanal;
            _cbDiaSemana.Visible = semanal;
            _lblDiaMes.Visible = mensal;
            _cbDiaMes.Visible = mensal;

            _painelAgendamento.Height = (semanal || mensal) ? 75 : 30;
        }

        // =====================================================================
        //  LIMPEZA DOS CAMPOS (modo NOVO)
        // =====================================================================
        private void LimparCampos()
        {
            _relatorioEmEdicao = null;

            if (cbRelatorios.Items.Count > 0)
                cbRelatorios.SelectedIndex = 0;
            else
                cbRelatorios.Text = string.Empty;

            txtArquivo.Text = string.Empty;
            txtEmail.Text = string.Empty;
            cbExcel.Checked = false;
            cbPDF.Checked = false;

            // Desliga eventos para não re-renderizar o painel repetidamente
            Diariamente.CheckedChanged -= OnFrequenciaChanged;
            Semanalmente.CheckedChanged -= OnFrequenciaChanged;
            Quinzenalmente.CheckedChanged -= OnFrequenciaChanged;
            Mensalmente.CheckedChanged -= OnFrequenciaChanged;

            Diariamente.Checked = true;
            Semanalmente.Checked = false;
            Quinzenalmente.Checked = false;
            Mensalmente.Checked = false;

            Diariamente.CheckedChanged += OnFrequenciaChanged;
            Semanalmente.CheckedChanged += OnFrequenciaChanged;
            Quinzenalmente.CheckedChanged += OnFrequenciaChanged;
            Mensalmente.CheckedChanged += OnFrequenciaChanged;

            _dtpHorario.Value = DateTime.Today.AddHours(8);

            if (_cbDiaSemana.Items.Count > 0) _cbDiaSemana.SelectedIndex = 0;
            if (_cbDiaMes.Items.Count > 0) _cbDiaMes.SelectedIndex = 0;

            dataGridView1.ClearSelection();

            AtualizarPaineisFrequencia();
        }

        // =====================================================================
        //  CARREGAR DADOS
        // =====================================================================
        private void CarregarRelatoriosDisponiveis()
        {
            cbRelatorios.Items.Clear();
            string pasta = Path.Combine(Application.StartupPath, "Json");
            if (!Directory.Exists(pasta)) return;

            foreach (var arquivo in Directory.GetFiles(pasta, "*.json"))
            {
                try
                {
                    var relJson = JsonConvert.DeserializeObject<RelatorioJsonSimples>(
                        File.ReadAllText(arquivo));

                    if (relJson == null || string.IsNullOrWhiteSpace(relJson.Nome)) continue;

                    string q = relJson.Query ?? string.Empty;
                    if (q.Contains(":dataInicial") || q.Contains("@dataInicial") ||
                        q.Contains(":dataFinal") || q.Contains("@dataFinal"))
                    {
                        cbRelatorios.Items.Add(relJson.Nome);
                    }
                }
                catch { /* arquivo inválido — ignora */ }
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

                string connStr =
                    $"Database={config.CaminhoBanco};" +
                    $"DataSource={config.Servidor};" +
                    $"User={config.Usuario};" +
                    $"Password={config.Senha};" +
                    $"Port={config.Porta};Dialect=3;";

                using (var conn = new FbConnection(connStr))
                {
                    conn.Open();
                    using (var cmd = new FbCommand(
                        "SELECT empresa_id, nom_empresa FROM empresa ORDER BY nom_empresa", conn))
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

        // =====================================================================
        //  JSON — leitura e escrita
        // =====================================================================
        private List<ConfigEnvioAutomatico> CarregarListaConfigsJson()
        {
            if (!File.Exists(CaminhoJson))
                return new List<ConfigEnvioAutomatico>();
            try
            {
                string json = File.ReadAllText(CaminhoJson).Trim();

                // Suporte a arquivo legado com objeto único (não lista)
                if (json.StartsWith("{"))
                {
                    var unico = JsonConvert.DeserializeObject<ConfigEnvioAutomatico>(json);
                    return unico != null
                        ? new List<ConfigEnvioAutomatico> { unico }
                        : new List<ConfigEnvioAutomatico>();
                }

                return JsonConvert.DeserializeObject<List<ConfigEnvioAutomatico>>(json)
                       ?? new List<ConfigEnvioAutomatico>();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao ler o arquivo JSON:\n{ex.Message}",
                    "Erro de Leitura", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<ConfigEnvioAutomatico>();
            }
        }

        private void SalvarListaConfigsJson(List<ConfigEnvioAutomatico> lista)
        {
            string pasta = Path.GetDirectoryName(CaminhoJson);
            if (!Directory.Exists(pasta)) Directory.CreateDirectory(pasta);
            File.WriteAllText(CaminhoJson, JsonConvert.SerializeObject(lista, Formatting.Indented));
        }

        // =====================================================================
        //  MONTAR CONFIG A PARTIR DA TELA
        // =====================================================================
        private ConfigEnvioAutomatico MontarConfig()
        {
            if (string.IsNullOrWhiteSpace(cbRelatorios.Text))
            {
                MessageBox.Show("Selecione ou digite o nome do relatório.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            if (string.IsNullOrWhiteSpace(txtArquivo.Text))
            {
                MessageBox.Show("Digite o nome que o arquivo anexo terá.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            if (!cbExcel.Checked && !cbPDF.Checked)
            {
                MessageBox.Show("Selecione pelo menos um formato (Excel ou PDF).", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Informe o e-mail do destinatário.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            string frequencia = "Mensalmente";
            if (Diariamente.Checked) frequencia = "Diariamente";
            else if (Semanalmente.Checked) frequencia = "Semanalmente";
            else if (Quinzenalmente.Checked) frequencia = "Quinzenalmente";

            int empresaId = 0;
            string empresaNome = string.Empty;
            if (dataGridView1.SelectedRows.Count > 0)
            {
                var row = dataGridView1.SelectedRows[0];
                int.TryParse(row.Cells["empresa_id"].Value?.ToString(), out empresaId);
                empresaNome = row.Cells["nom_empresa"].Value?.ToString() ?? string.Empty;
            }

            return new ConfigEnvioAutomatico
            {
                RelatorioNome = cbRelatorios.Text.Trim(),
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
                UltimoEnvio = DateTime.MinValue   // substituído no salvar se for edição
            };
        }

        // =====================================================================
        //  BOTÃO SALVAR / ALTERAR
        // =====================================================================
        private void btnAlterar_Click(object sender, EventArgs e)
        {
            var novaCfg = MontarConfig();
            if (novaCfg == null) return;

            var lista = CarregarListaConfigsJson();

            if (!string.IsNullOrEmpty(_relatorioEmEdicao))
            {
                // ── MODO EDIÇÃO: substitui o item existente ────────────────
                var antigo = lista.FirstOrDefault(c =>
                    c.RelatorioNome.Equals(_relatorioEmEdicao, StringComparison.OrdinalIgnoreCase));

                if (antigo != null)
                {
                    novaCfg.UltimoEnvio = antigo.UltimoEnvio; // preserva histórico
                    lista[lista.IndexOf(antigo)] = novaCfg;
                }
                else
                {
                    lista.Add(novaCfg); // segurança: não encontrou, adiciona
                }
            }
            else
            {
                // ── MODO NOVO: verifica duplicata ──────────────────────────
                bool jaExiste = lista.Any(c =>
                    c.RelatorioNome.Equals(novaCfg.RelatorioNome, StringComparison.OrdinalIgnoreCase));

                if (jaExiste)
                {
                    MessageBox.Show(
                        "Já existe um agendamento para este relatório.\nClique nele na lista para editar.",
                        "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                lista.Add(novaCfg);
            }

            SalvarListaConfigsJson(lista);

            MessageBox.Show("Configuração salva com sucesso!", "Sucesso",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            LimparCampos();
            AtualizarListaConfiguracoes();
        }

        // =====================================================================
        //  BOTÃO EXCLUIR / LIMPAR
        // =====================================================================
        private void btnExcluir_Click(object sender, EventArgs e)
        {
            // Se há um item em edição, pergunta se quer excluir do JSON
            if (!string.IsNullOrEmpty(_relatorioEmEdicao))
            {
                var confirm = MessageBox.Show(
                    $"Deseja excluir o agendamento \"{_relatorioEmEdicao}\"?",
                    "Confirmar exclusão",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (confirm == DialogResult.Yes)
                {
                    var lista = CarregarListaConfigsJson();
                    lista.RemoveAll(c =>
                        c.RelatorioNome.Equals(_relatorioEmEdicao, StringComparison.OrdinalIgnoreCase));
                    SalvarListaConfigsJson(lista);
                    AtualizarListaConfiguracoes();
                }
            }

            LimparCampos();
        }
    }
}