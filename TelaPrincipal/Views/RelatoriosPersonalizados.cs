using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using FirebirdSql.Data.FirebirdClient;
using TelaPrincipal.DAO;
using TelaPrincipal.Utils;
using System.Drawing;

namespace TelaPrincipal.Views
{
    public partial class RelatoriosPersonalizados : Form
    {
        private List<Relacionamento> relacionamentos = new List<Relacionamento>();
        private string colunaDataSelecionada = null;
        private DateTime? dataInicio = null;
        private DateTime? dataFim = null;
        private bool carregandoColunasData = false;
        public RelatoriosPersonalizados()
        {
            InitializeComponent();
            try
            {
                IniciarSessao();

                relacionamentos = CarregarRelacionamentos(); // 🔥 FK

                CarregarTabelas();
                ConfigurarGridTabelas();
                ConfigurarGridColunas();

                dgvTabelas.CurrentCellDirtyStateChanged += dgvTabelas_CurrentCellDirtyStateChanged;
                dgvTabelas.CellValueChanged += dgvTabelas_CellValueChanged;
                dgvColunas.CurrentCellDirtyStateChanged += dgvColunas_CurrentCellDirtyStateChanged;
                dgvColunas.CellValueChanged += dgvColunas_CellValueChanged;

                this.Resize += RelatoriosPersonalizados_Resize;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao iniciar: " + ex.Message);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            AjustarLayoutResponsivo();
        }

        private void RelatoriosPersonalizados_Resize(object sender, EventArgs e)
        {
            AjustarLayoutResponsivo();
        }

        private void AjustarLayoutResponsivo()
        {
            int w = this.ClientSize.Width;
            int h = this.ClientSize.Height;
            int margem = 12;
            int yTopo = 50;
            int alturaMetade = (h - yTopo - margem * 3) / 2;
            int larguraMetade = (w - margem * 3) / 2;

            // Linha 1 — Tabelas (esq) e Preview (dir)
            panel1.Location = new Point(margem, yTopo);
            panel1.Size = new Size(larguraMetade, alturaMetade);
            dgvTabelas.Size = new Size(larguraMetade, alturaMetade - 27);

            int xDir = margem * 2 + larguraMetade;
            panel3.Location = new Point(xDir, yTopo);
            panel3.Size = new Size(w - xDir - margem, alturaMetade);
            dgvResultado.Size = new Size(panel3.Width, alturaMetade - 32);

            // Linha 2 — Colunas (esq) e Filtros/Gerar (dir)
            int yLinha2 = yTopo + alturaMetade + margem;
            panel2.Location = new Point(margem, yLinha2);
            panel2.Size = new Size(larguraMetade, alturaMetade);
            dgvColunas.Size = new Size(larguraMetade, alturaMetade - 27);

            panel4.Location = new Point(xDir, yLinha2);
            panel4.Size = new Size(w - xDir - margem, alturaMetade);
        }

        #region 🔌 CONEXÃO

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

        #endregion

        #region 🔗 FK

        public class Relacionamento
        {
            public string TabelaFilha { get; set; }
            public string CampoFilha { get; set; }
            public string TabelaPai { get; set; }
            public string CampoPai { get; set; }
        }

        private List<Relacionamento> CarregarRelacionamentos()
        {
            var lista = new List<Relacionamento>();

            using (var conn = ConexaoFactory.CriarConexao())
            {
                conn.Open();

                string sql = @"
                SELECT
                    TRIM(rc.RDB$RELATION_NAME) AS TABELA_FILHA,
                    TRIM(seg.RDB$FIELD_NAME) AS CAMPO_FILHA,
                    TRIM(rc2.RDB$RELATION_NAME) AS TABELA_PAI,
                    TRIM(seg2.RDB$FIELD_NAME) AS CAMPO_PAI
                FROM RDB$RELATION_CONSTRAINTS rc
                JOIN RDB$REF_CONSTRAINTS ref 
                    ON rc.RDB$CONSTRAINT_NAME = ref.RDB$CONSTRAINT_NAME
                JOIN RDB$RELATION_CONSTRAINTS rc2 
                    ON ref.RDB$CONST_NAME_UQ = rc2.RDB$CONSTRAINT_NAME
                JOIN RDB$INDEX_SEGMENTS seg 
                    ON seg.RDB$INDEX_NAME = rc.RDB$INDEX_NAME
                JOIN RDB$INDEX_SEGMENTS seg2 
                    ON seg2.RDB$INDEX_NAME = rc2.RDB$INDEX_NAME
                WHERE rc.RDB$CONSTRAINT_TYPE = 'FOREIGN KEY'";

                using (var cmd = new FbCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new Relacionamento
                        {
                            TabelaFilha = reader["TABELA_FILHA"].ToString(),
                            CampoFilha = reader["CAMPO_FILHA"].ToString(),
                            TabelaPai = reader["TABELA_PAI"].ToString(),
                            CampoPai = reader["CAMPO_PAI"].ToString()
                        });
                    }
                }
            }

            return lista;
        }

        public static string NomeAmigavel(string nomeTabela)
        {
            if (string.IsNullOrWhiteSpace(nomeTabela))
                return nomeTabela;

            // 1. tenta pegar do mapa manual
            if (TabelaNomes.Map.TryGetValue(nomeTabela, out var nomeManual))
                return nomeManual;

            // 2. fallback automático
            return string.Join(" ",
                nomeTabela
                    .ToLower()
                    .Split('_')
                    .Select(p => char.ToUpper(p[0]) + p.Substring(1))
            );
        }

        #endregion

        #region 📊 TABELAS
        public static class TabelaNomes
        {
            public static readonly Dictionary<string, string> Map = new Dictionary<string, string>()
        {
        { "ABASTECIMENTO_AUTOMACAO", "Abastecimento Automação" },
        { "ALIQUOTAS_EFETIVAS", "Alíquotas Efetivas" },
        { "ALIQUOTAS_FCP_DESTINO", "Alíquotas FCP Destino" },
        { "ALIQUOTAS_IBPT", "Alíquotas IBPT" },
        { "ALIQUOTAS_ICMS_DESONERADO", "Alíquotas ICMS Desonerado" },
        { "ALIQUOTAS_ICMS_DIFERIDO", "Alíquotas ICMS Diferido" },
        { "ALIQUOTA_BASE_RETENCAO", "Alíquota Base Retenção" },
        { "ALIQUOTA_CBS_IBS_UF", "Alíquota CBS/IBS UF" },
        { "ALIQUOTA_CBS_IBS_UF_DIF", "Alíquota CBS/IBS UF Dif" },
        { "ALIQUOTA_PRODUTO_UF", "Alíquota Produto UF" },
        { "BANCO", "Bancos" },
        { "BANDEIRA_CARTAO", "Bandeiras de Cartão" },
        { "BICO", "Bicos" },
        { "BICO_APRAZO", "Bicos a Prazo" },
        { "BOLETO", "Boletos" },
        { "BOMBA", "Bombas" },
        { "CARTAO", "Cartões" },
        { "CIDADE", "Cidades" },
        { "CLIENTE", "Clientes" },
        { "COMPRA", "Compras" },
        { "CONTA_CORRENTE", "Contas Correntes" },
        { "CONTATO", "Contatos" },
        { "CONTROLE", "Controles" },
        { "EMPRESA", "Empresas" },
        { "ENDERECO", "Endereços" },
        { "ESTADOS", "Estados" },
        { "FUNCIONARIO", "Funcionários" },
        { "PRODUTO", "Produtos" },
        { "SERVICO", "Serviços" },
        { "VENDA", "Vendas" }

    };
            public static string NomeAmigavel(string nomeTabela)
            {
                if (string.IsNullOrWhiteSpace(nomeTabela))
                    return nomeTabela;

                // 1. tenta no dicionário primeiro
                if (TabelaNomes.Map.TryGetValue(nomeTabela, out var nomeManual))
                    return nomeManual;

                // 2. fallback automático
                return string.Join(" ",
                    nomeTabela
                        .ToLower()
                        .Split('_')
                        .Select(p => char.ToUpper(p[0]) + p.Substring(1))
                );
            }
        }
        public DataTable BuscarTabelas()
        {
            var dt = new DataTable();

            using (var conn = ConexaoFactory.CriarConexao())
            {
                conn.Open();

                string sql = @"
            SELECT TRIM(RDB$RELATION_NAME) AS NOME
            FROM RDB$RELATIONS
            WHERE RDB$SYSTEM_FLAG = 0
              AND RDB$VIEW_SOURCE IS NULL
              AND TRIM(RDB$RELATION_NAME) IN (
                'ABASTECIMENTO_AUTOMACAO',
                'ALIQUOTAS_EFETIVAS',
                'ALIQUOTAS_FCP_DESTINO',
                'ALIQUOTAS_IBPT',
                'ALIQUOTAS_ICMS_DESONERADO',
                'ALIQUOTAS_ICMS_DIFERIDO',
                'ALIQUOTA_BASE_RETENCAO',
                'ALIQUOTA_CBS_IBS_UF',
                'ALIQUOTA_CBS_IBS_UF_DIF',
                'ALIQUOTA_PRODUTO_UF',
                'BANCO',
                'BANDEIRA_CARTAO',
                'BICO',
                'BICO_APRAZO',
                'BOLETO',
                'BOMBA',
                'CARTAO',
                'CARTAO_EMPRESA',
                'CARTAO_MAGNETICO',
                'CARTA_CORRECAO_ELETRONICA',
                'CARTA_FRETE',
                'CENTRO_CUSTO',
                'CENTRO_CUSTO_EMPRESA',
                'CFOP',
                'CHEQUE_EMITIDO',
                'CHEQUE_MODELO_IMPRESSAO',
                'CHEQUE_PRE_AVISTA',
                'CHEQUE_SEMFUNDO',
                'CIDADE',
                'CLASS_TRIB',
                'COBERTURA',
                'COBERTURA_CENTRO_CUSTO',
                'CODIGO_ANP',
                'CODIGO_BASE_PIS_COFINS',
                'COD_BARRA',
                'COD_BARRA_PRODUTO',
                'COMBUSTIVEL',
                'COMBUSTIVEL_BENEFICIO',
                'COMBUSTIVEL_CENTRO_CUSTO',
                'COMBUSTIVEL_EMPRESA',
                'COMBUSTIVEL_FORNECEDOR_NFE',
                'COMBUSTIVEL_IMPORTADOR_PRODUTOR',
                'COMB_SUJ_TRIB_MONOFASICA',
                'COMPONENTE',
                'COMPRA',
                'CONSUMO_ENERGIA',
                'CONTAGEM_ESTOQUE_PRODUTO',
                'CONTAS_APAGAR',
                'CONTATO',
                'CONTA_CORRENTE',
                'CONTROLE',
                'CONVENIADO',
                'CST',
                'DADOS_EMISSAO',
                'DADOS_NFE_DEVOLUCAO',
                'DADOS_NFE_DEVOLUCAO_EXCLUIDAS',
                'DELIVERY',
                'DELIVERY_EMPRESA',
                'DELIVERY_IFOODDIRETA_CATEGORIA',
                'DELIVERY_IFOODDIRETA_FILA',
                'DESCONTO_ECF_CONSUMIDOR_99',
                'DESCONTO_ITEM_ECF_CONSUMIDOR_99',
                'DESPESA_CAIXA',
                'DESPESA_NIVEL1',
                'DESPESA_NIVEL2',
                'DESPESA_NIVEL3',
                'DEVOLUCAO_PRODUTO',
                'DIFAL',
                'DISTRIBUICAO_TANQUE',
                'DIV_DESPESA',
                'ECF_CONSUMIDOR',
                'ECF_CONSUMIDOR_CANC',
                'ECF_CONSUMIDOR_PROMOCAO',
                'ECF_MODELO',
                'ECF_OBS_FISC',
                'ECF_OBS_FISC_CANC',
                'ECF_OBS_FISC_HISTORICO',
                'EMAIL',
                'EMPRESA',
                'ENDERECO',
                'EQUIVALENCIA_CARTAO',
                'EQUIVALENCIA_REDE',
                'EQUIVALENCIA_TIPO_OPERACAO',
                'ESTADOS',
                'ESTOQUE_INICIAL_DIA_CFWIN',
                'EST_PRATELEIRA',
                'EXTRATO',
                'FABRICANTE',
                'FATURAMENTO_NFCOM',
                'FECHAMENTO_CAIXA',
                'FILA_EMAIL',
                'FILA_PRODUCAO',
                'FILA_PRODUCAO_PRODUTO_NIVEL2',
                'FILTROS_COMBUSTIVEIS',
                'FINALIDADE_NFCOM',
                'FORMA_PAGAMENTO',
                'FORMA_PAGAMENTO_NFE',
                'FORMA_PAGAMENTO_VENDA',
                'FUNCIONARIO',
                'GRUPO_PERFIL',
                'GRUPO_PRECO',
                'GRUPO_PRECO_PESSOA',
                'HAVER_ADIANTAMENTO',
                'INVENTARIO_PRODUTO',
                'ITEM_COMPRA',
                'ITEM_ECF_CONSUMIDOR',
                'ITEM_ECF_CONSUMIDOR_CANC',
                'ITEM_NF',
                'ITEM_NF_COMB_SUJ_TRIB_MONO',
                'ITEM_NF_EXCLUIDAS',
                'ITEM_NF_IRRF',
                'ITEM_OUTRAS_NF',
                'ITEM_REQUISICAO',
                'ITEM_RESP_PEDIDO_LEITURA_TQ',
                'ITEM_VENDA',
                'ITEM_VENDA_CFWIN',
                'LACRE_BOMBA',
                'LANC_BANCARIO',
                'LEITURA_TANQUE',
                'LEITURA_TIPO_TANQUE',
                'LGPD_HISTORICO',
                'LGPD_PESSOAS',
                'LGPD_SOLICITACOES',
                'LISTA_CFOP',
                'LISTA_CFOP_NATUREZA_OPERACAO',
                'LISTA_SERVICOS',
                'LOCAL_ESTOQUE',
                'LOCAL_ESTOQUE_PRODUTO',
                'MANIFESTACAO_DADOS',
                'MANIFESTACAO_NFE',
                'MANUT_BICO',
                'MAQUINA_CARTAO',
                'MAQUINA_SMART_POS',
                'MEDICAO_FISICA',
                'META_FUNCIONARIO',
                'META_FUNCIONARIO_ITENS',
                'MODALIDADE_CAIXA',
                'MODALIDADE_FRETE',
                'MODELO_DOC_FISCAL',
                'MODELO_DOC_FISCAL_ACOBERTAMENTO',
                'MODELO_OUTRAS_NF',
                'MOTIVOS_DESONERACAO_ICMS',
                'MOVIMENTO_CARTAO',
                'MOVIMENTO_ENC',
                'MOVIMENTO_PRODUTO',
                'MOVIMENTO_SERVICO',
                'NATUREZAS_RECEITA',
                'NATUREZAS_RECEITA_CST',
                'NATUREZA_OPERACAO',
                'NATUREZA_OPERACAO_ISSQN',
                'NCM_UF_FCP',
                'NFE_INUTILIZACAO',
                'NFE_LOTE',
                'NFE_PAGAMENTO',
                'NF_CANCELADA',
                'NF_DADOS_ADICIONAIS',
                'NF_DADOS_ADICIONAIS_EXCLUIDAS',
                'NF_DADOS_FISCO',
                'NF_ELETRONICA',
                'NF_ELETRONICA_CAIXA',
                'NF_ELETRONICA_EXCLUIDAS',
                'NF_ELETRONICA_OBS_FISC',
                'NF_ELETRONICA_PENDENTE_ENVIO',
                'NF_ENDERECO_ENTREGA',
                'NF_MODELO',
                'NF_MODELO1',
                'NF_MODELO1_EXCLUIDAS',
                'NF_MODELO_CAMPOS',
                'NF_MODELO_LAYOUT',
                'OBS_LMC',
                'OCORRENCIA_CONC_CARTAO',
                'OPCAO_BOLETO',
                'OPCAO_BOLETO_COBRANCA',
                'OPCAO_CNAB_REMESSA',
                'OPCAO_NOTA_FISCAL',
                'ORIGEM_PROCESSO',
                'OUTRAS_NF',
                'PAGAMENTO_ECF_CONSUMIDOR',
                'PAGAMENTO_ECF_CONSUMIDOR_CANC',
                'PAIS',
                'PARCELAS_CRED_ICMSST_ESTOQUE_RS',
                'PARCELAS_EST_CRED_ICMS_CESTA_PR',
                'PARCELA_APAGAR',
                'PARCELA_CREDIARIO',
                'PDA',
                'PDV',
                'PEDIDO_LEITURA_TANQUES',
                'PENDENCIA_CONC_CARTAO',
                'PERFIL_AJUSTE_APURACAO_ICMS',
                'PERFIL_ASPECTO',
                'PERFIL_CONTABIL',
                'PER_ICMS_INTERESTADUAL',
                'PER_ICMS_UF',
                'PESSOA',
                'PESSOA_ATIVIDADE',
                'PESSOA_FECHAMENTO_CAIXA',
                'PESSOA_IRRF',
                'PESSOA_PF',
                'PESSOA_PJ',
                'PLANOS_CONTABEIS',
                'PLANO_CONTAS',
                'PRATELEIRA',
                'PRAZO',
                'PRECO_COMB',
                'PRECO_COMB_BICO',
                'PRECO_ESPECIAL',
                'PRODUTO',
                'PRODUTOS_DIFAL',
                'PRODUTO_BENEFICIO',
                'PRODUTO_DELIVERY',
                'PRODUTO_EMPRESA',
                'PRODUTO_ESCALA_REL',
                'PRODUTO_FORNECEDOR_NFE',
                'PRODUTO_IMPORTADOR_PRODUTOR',
                'PRODUTO_ITWORKS',
                'PRODUTO_N1_CENTRO_CUSTO',
                'PRODUTO_NIVEL1',
                'PRODUTO_NIVEL1_COBERTURA',
                'PRODUTO_NIVEL2',
                'RATEIO_CC',
                'RECEITA',
                'RECEITA_DIVERSA',
                'RECIBO_PAGAMENTO',
                'RECIBO_PAG_CHEQUE',
                'RECIBO_PAG_DESPESA',
                'REDE_ADMINISTRADORA_CARTAO',
                'REGIME_TRIBUTARIO',
                'REGIME_TRIBUTARIO_ALIQUOTA',
                'REGIME_TRIBUTARIO_TRACE',
                'REQUISICAO',
                'RESPONSAVEL_TECNICO',
                'RESP_PEDIDO_LEITURA_TQ',
                'RESTRICAO_USUARIO',
                'RESTR_CLIENTE_ABAST',
                'RETIRADA',
                'RETIRADA_TIPO',
                'ROT_ICMS_ST_RS',
                'SALDO_CAT17',
                'SALDO_COMBUSTIVEL',
                'SALDO_PRODUTO',
                'SAT',
                'SELCON_CODBAR_PROD',
                'SELCON_DEPTO',
                'SELCON_GRUPO1',
                'SELCON_GRUPO2',
                'SELCON_GRUPO3',
                'SELCON_GRUPO4',
                'SELCON_MIX',
                'SELCON_MIX_GRUPOS',
                'SELCON_MIX_ITENS',
                'SELCON_PRODUTOS',
                'SELCON_SECAO',
                'SERVICO',
                'SERVICO_EMPRESA',
                'SERVICO_N1_CENTRO_CUSTO',
                'SERVICO_NIVEL1',
                'SUB_TIPO_PRAZO',
                'SUB_TIPO_RECEITA',
                'TANQUE',
                'TB_COMBO_GRUPO',
                'TB_COMBO_GRUPO_EMPRESA',
                'TB_COMBO_GRUPO_ITEM',
                'TB_COMBO_MIX',
                'TB_COMBO_MIX_EMPRESA',
                'TELEFONE',
                'TENSAO_ENERGIA',
                'TIPO_COMB',
                'TIPO_FILA_PRODUCAO',
                'TIPO_JORNADA',
                'TIPO_LIGACAO_ENERGIA',
                'TIPO_MODALIDADE_CAIXA',
                'TIPO_OPERACAO_CARTAO',
                'TIPO_PLANO_CONTA',
                'TIPO_TANQUE',
                'TIPO_VENDA',
                'TRANSFERENCIA_ESTOQUE',
                'TRANSFERENCIA_PRODUTO',
                'TRANSF_CAIXA',
                'TRANSF_PRODUTO',
                'TRILHA_AUDITORIA',
                'TURNO',
                'TURNO_CENTRO_CUSTO',
                'UF_FCP_PADRAO',
                'USUARIO',
                'USUARIO_EMPRESA',
                'VALE_GERADO',
                'VEICULO',
                'VEICULO_TIPO_COM',
                'VENDA',
                'VENDAS_ARQUIVO_VAN',
                'VENDA_CFWIN',
                'VERSAO_LAYOUT_SPED',
                'VINCULO_CUPOM_NF',
                'VINCULO_CUPOM_NF_EXCLUIDAS'
              )
            ORDER BY RDB$RELATION_NAME";

                using (var da = new FbDataAdapter(sql, conn))
                {
                    da.Fill(dt);
                }
            }
            dt.Columns.Add("NOME_AMIGAVEL", typeof(string));

            foreach (DataRow row in dt.Rows)
            {
                var nomeOriginal = row["NOME"].ToString();

                row["NOME_AMIGAVEL"] = NomeAmigavel(nomeOriginal);
            }
            return dt;
        }

        private void CarregarTabelas()
        {
            var dt = BuscarTabelas();

            dgvTabelas.DataSource = dt;
            dgvTabelas.Columns["NOME"].HeaderText = "Tabelas";
            dgvTabelas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void ConfigurarGridTabelas()
        {
            if (dgvTabelas.Columns["Selecionar"] == null)
            {
                var chk = new DataGridViewCheckBoxColumn
                {
                    Name = "Selecionar",
                    Width = 30
                };

                dgvTabelas.Columns.Insert(0, chk);
            }
        }

        public List<string> ObterTabelasSelecionadas()
        {
            var lista = new List<string>();

            foreach (DataGridViewRow row in dgvTabelas.Rows)
            {
                if (row.IsNewRow) continue; // 🔥 importante

                bool selecionado = row.Cells["Selecionar"].Value != null &&
                                   Convert.ToBoolean(row.Cells["Selecionar"].Value);

                if (selecionado)
                {
                    string nome = row.Cells["NOME"].Value?.ToString();

                    if (!string.IsNullOrWhiteSpace(nome))
                        lista.Add(nome);
                }
            }

            return lista;
        }

        #endregion

        #region 📋 COLUNAS

        private void ConfigurarGridColunas()
        {
            dgvColunas.Columns.Clear();

            dgvColunas.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = "Selecionar",
                Width = 30
            });

            dgvColunas.Columns.Add("COLUNA", "Coluna");
            dgvColunas.Columns.Add("TABELA", "Tabela");

            dgvColunas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        public DataTable BuscarColunas(string tabela)
        {
            var dt = new DataTable();

            using (var conn = ConexaoFactory.CriarConexao())
            {
                conn.Open();

                string sql = @"
                SELECT TRIM(rf.RDB$FIELD_NAME) AS COLUNA
                FROM RDB$RELATION_FIELDS rf
                WHERE rf.RDB$RELATION_NAME = @TABELA
                ORDER BY rf.RDB$FIELD_POSITION";

                using (var cmd = new FbCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@TABELA", tabela);

                    using (var da = new FbDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }

            return dt;
        }
        private void AtualizarColunasData()
        {
            cmbColunasData.Items.Clear();

            var colunasData = new HashSet<string>();

            foreach (DataGridViewRow row in dgvColunas.Rows)
            {
                if (row.IsNewRow) continue;

                string coluna = row.Cells["COLUNA"].Value?.ToString()?.ToUpper();

                if (string.IsNullOrWhiteSpace(coluna))
                    continue;

                if (coluna.Contains("DATA") || coluna.Contains("DAT") || coluna.Contains("DT"))
                {
                    string tabela = row.Cells["TABELA"].Value?.ToString();
                    colunasData.Add($"{tabela}.{coluna}");
                }
            }

            foreach (var col in colunasData)
                cmbColunasData.Items.Add(col);

            if (cmbColunasData.Items.Count > 0)
                cmbColunasData.SelectedIndex = 0;
        }
        private void AtualizarColunas()
        {
            dgvColunas.Rows.Clear();

            var adicionadas = new HashSet<string>();

            foreach (var tabela in ObterTabelasSelecionadas())
            {
                var dt = BuscarColunas(tabela);

                foreach (DataRow row in dt.Rows)
                {
                    string coluna = row["COLUNA"]?.ToString();

                    if (string.IsNullOrWhiteSpace(coluna))
                        continue;

                    string chave = $"{tabela}.{coluna}";

                    if (adicionadas.Contains(chave))
                        continue;

                    adicionadas.Add(chave);

                    dgvColunas.Rows.Add(false, coluna, tabela);
                }
            }


            AtualizarColunasData();
            AtualizarPreview();



        }
        #endregion

        #region 🔥 FILTRO FK

        private void AtualizarFiltroTabelas()
        {
            var selecionadas = ObterTabelasSelecionadas();

            if (selecionadas.Count == 0)
            {
                foreach (DataGridViewRow row in dgvTabelas.Rows)
                {
                    row.Visible = true;
                    row.Cells["Selecionar"].ReadOnly = false;
                }
                return;
            }

            var permitidas = FiltrarTabelasRelacionadasDiretas(selecionadas);

            foreach (DataGridViewRow row in dgvTabelas.Rows)
            {
                if (row.IsNewRow) continue; // 🔥 importante

                string nome = row.Cells["NOME"].Value?.ToString();

                if (string.IsNullOrWhiteSpace(nome))
                    continue;

                bool permitido = permitidas.Contains(nome);

                row.Visible = permitido;
                row.Cells["Selecionar"].ReadOnly = !permitido;
            }
        }
        private List<string> FiltrarTabelasRelacionadasDiretas(List<string> selecionadas)
        {
            var permitidas = new HashSet<string>();

            foreach (var tabela in selecionadas)
            {
                permitidas.Add(tabela); // mantém a própria

                var relacionadasDiretas = relacionamentos
                    .Where(r => r.TabelaFilha == tabela || r.TabelaPai == tabela);

                foreach (var rel in relacionadasDiretas)
                {
                    permitidas.Add(rel.TabelaFilha);
                    permitidas.Add(rel.TabelaPai);
                }
            }

            return permitidas.ToList();
        }

        #endregion

        #region ⚡ EVENTOS

        private void dgvTabelas_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvTabelas.IsCurrentCellDirty)
                dgvTabelas.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void dgvTabelas_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dgvTabelas.Columns["Selecionar"].Index)
            {
                AtualizarFiltroTabelas();
                AtualizarColunas();
                AtualizarPreview();
            }
        }

        #endregion

        public List<(string tabela, string coluna)> ObterColunasSelecionadas()
        {
            var lista = new List<(string, string)>();

            foreach (DataGridViewRow row in dgvColunas.Rows)
            {
                if (row.IsNewRow) continue; // 🔥 evita erro

                bool selecionado = row.Cells["Selecionar"].Value != null &&
                                   Convert.ToBoolean(row.Cells["Selecionar"].Value);

                if (!selecionado) continue;

                string coluna = row.Cells["COLUNA"].Value?.ToString();
                string tabela = row.Cells["TABELA"].Value?.ToString();

                if (string.IsNullOrWhiteSpace(coluna) || string.IsNullOrWhiteSpace(tabela))
                    continue;

                lista.Add((tabela, coluna));
            }

            return lista;
        }

        public string GerarSQL(bool preview = true)
        {
            var colunas = ObterColunasSelecionadas();

            if (colunas.Count == 0)
                return "";

            var select = new List<string>();
            var tabelas = new HashSet<string>();

            var tabelaAlias = new Dictionary<string, string>();
            int aliasIndex = 0;

            // =========================
            // SELECT
            // =========================
            foreach (var (tabela, coluna) in colunas)
            {
                if (!tabelaAlias.ContainsKey(tabela))
                    tabelaAlias[tabela] = $"t{aliasIndex++}";

                string alias = tabelaAlias[tabela];

                string nomeAlias = $"{tabela}_{coluna}";
                if (nomeAlias.Length > 30)
                    nomeAlias = nomeAlias.Substring(0, 30);

                select.Add($"{alias}.{coluna} AS {nomeAlias}");
                tabelas.Add(tabela);
            }

            // =========================
            // FROM
            // =========================
            var primeiraTabela = colunas.First().tabela;

            string sql = $"SELECT {string.Join(", ", select)} FROM {primeiraTabela} {tabelaAlias[primeiraTabela]}";

            var tabelasProcessadas = new HashSet<string> { primeiraTabela };
            var joinsJaFeitos = new HashSet<string>();

            // =========================
            // JOINS
            // =========================
            foreach (var tabelaDestino in tabelas.Where(t => t != primeiraTabela))
            {
                var caminho = EncontrarCaminho(primeiraTabela, tabelaDestino);

                if (caminho == null || caminho.Count == 0)
                    continue;

                foreach (var rel in caminho)
                {
                    string tabelaA = rel.TabelaFilha;
                    string tabelaB = rel.TabelaPai;

                    if (!tabelaAlias.ContainsKey(tabelaA))
                        tabelaAlias[tabelaA] = $"t{aliasIndex++}";

                    if (!tabelaAlias.ContainsKey(tabelaB))
                        tabelaAlias[tabelaB] = $"t{aliasIndex++}";

                    string chaveJoin = $"{tabelaA}-{tabelaB}";
                    string chaveJoinInvertida = $"{tabelaB}-{tabelaA}";

                    if (joinsJaFeitos.Contains(chaveJoin) || joinsJaFeitos.Contains(chaveJoinInvertida))
                        continue;

                    string join = "";

                    if (tabelasProcessadas.Contains(tabelaA) && !tabelasProcessadas.Contains(tabelaB))
                    {
                        join = $" LEFT JOIN {tabelaB} {tabelaAlias[tabelaB]} ON {tabelaAlias[tabelaB]}.{rel.CampoPai} = {tabelaAlias[tabelaA]}.{rel.CampoFilha}";
                        tabelasProcessadas.Add(tabelaB);
                    }
                    else if (tabelasProcessadas.Contains(tabelaB) && !tabelasProcessadas.Contains(tabelaA))
                    {
                        join = $" LEFT JOIN {tabelaA} {tabelaAlias[tabelaA]} ON {tabelaAlias[tabelaA]}.{rel.CampoFilha} = {tabelaAlias[tabelaB]}.{rel.CampoPai}";
                        tabelasProcessadas.Add(tabelaA);
                    }

                    if (!string.IsNullOrEmpty(join))
                    {
                        sql += join;
                        joinsJaFeitos.Add(chaveJoin);
                    }
                }
            }

            // =========================
            // WHERE
            // =========================
            string where = "";

            // =========================
            // PERÍODO
            // =========================
            if (chkPeriodo.Checked && !string.IsNullOrWhiteSpace(colunaDataSelecionada))
            {
                string colunaWhere = colunaDataSelecionada;

                if (colunaWhere.Contains("."))
                {
                    var parts = colunaWhere.Split('.');
                    string tabela = parts[0];
                    string coluna = parts[1];

                    if (tabelaAlias.TryGetValue(tabela, out string alias))
                    {
                        colunaWhere = $"{alias}.{coluna}";
                    }
                }

                where = $" WHERE {colunaWhere} BETWEEN @dataInicial AND @dataFinal";
            }

            // =========================
            // EMPRESA (TABELA DINÂMICA)
            // =========================
            if (chkEmpresa.Checked)
            {
                var tabelasSelecionadas = ObterTabelasSelecionadas();

                string tabelaEmpresa = null;

                foreach (var t in tabelasSelecionadas)
                {
                    var dt = BuscarColunas(t);

                    foreach (DataRow r in dt.Rows)
                    {
                        if (r["COLUNA"].ToString().ToUpper() == "EMPRESA_ID")
                        {
                            tabelaEmpresa = t;
                            break;
                        }
                    }

                    if (tabelaEmpresa != null)
                        break;
                }

                if (!string.IsNullOrEmpty(tabelaEmpresa))
                {
                    if (!string.IsNullOrWhiteSpace(where))
                        where += " AND ";
                    else
                        where = " WHERE ";

                    string colunaEmpresa = "EMPRESA_ID";

                    if (tabelaAlias.TryGetValue(tabelaEmpresa, out string alias))
                    {
                        colunaEmpresa = $"{alias}.EMPRESA_ID";
                    }

                    where += $"{colunaEmpresa} = @empresa_id";
                }
            }

            sql += where;

            return sql;
        }

        private void btnGerarSQL_Click(object sender, EventArgs e)
        {

        }

        public List<Relacionamento> EncontrarCaminho(string origem, string destino)
        {
            var visitados = new HashSet<string>();
            var fila = new Queue<(string tabela, List<Relacionamento> caminho)>();

            fila.Enqueue((origem, new List<Relacionamento>()));

            while (fila.Count > 0)
            {
                var (tabelaAtual, caminhoAtual) = fila.Dequeue();

                if (tabelaAtual == destino)
                    return caminhoAtual;

                if (visitados.Contains(tabelaAtual))
                    continue;

                visitados.Add(tabelaAtual);

                var relacionados = relacionamentos
                    .Where(r => r.TabelaFilha == tabelaAtual || r.TabelaPai == tabelaAtual);

                foreach (var rel in relacionados)
                {
                    string proxima = rel.TabelaFilha == tabelaAtual ? rel.TabelaPai : rel.TabelaFilha;

                    var novoCaminho = new List<Relacionamento>(caminhoAtual) { rel };

                    fila.Enqueue((proxima, novoCaminho));
                }
            }

            return null;
        }
        private string EncontrarTabelaEmpresa(List<string> tabelasSelecionadas)
        {
            foreach (var tabela in tabelasSelecionadas)
            {
                var possuiCampo = BuscarColunas(tabela)
                    .AsEnumerable()
                    .Any(r => r["COLUNA"].ToString().ToUpper() == "EMPRESA_ID");

                if (possuiCampo)
                    return tabela;
            }

            return null;
        }
        private void AtualizarPreview()
        {
            try
            {
                string sql = GerarSQL();

                sql = sql.Replace("BETWEEN @dataInicial AND @dataFinal", "IS NOT NULL");


                sql = sql.Replace("EMPRESA_ID = @empresa_id", "EMPRESA_ID IS NOT NULL");
                sql = sql.Replace("= @empresa_id", "IS NOT NULL");

                // MessageBox.Show(sql, "SQL GERADO (DEBUG)");

                if (string.IsNullOrWhiteSpace(sql) || ObterColunasSelecionadas().Count == 0)
                    return;

                sql += " ROWS 50";

                var dt = ExecutarSQL(sql);

                dgvResultado.DataSource = dt;
                dgvResultado.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro Preview:\n" + ex.Message);
            }
        }
        private void btnExecutar_Click(object sender, EventArgs e)
        {
            try
            {
                string sql = GerarSQL();

                if (string.IsNullOrWhiteSpace(sql))
                {
                    MessageBox.Show("Selecione colunas.");
                    return;
                }

                sql += " ROWS 50"; // preview apenas

                var dt = ExecutarSQL(sql);

                dgvResultado.DataSource = dt;
                dgvResultado.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao executar SQL:\n" + ex.Message);
            }
        }

        public DataTable ExecutarSQL(string sql, DateTime? ini = null, DateTime? fim = null)
        {
            var dt = new DataTable();

            using (var conn = ConexaoFactory.CriarConexao())
            {
                conn.Open();

                using (var cmd = new FbCommand(sql, conn))
                {
                    if (ini.HasValue && fim.HasValue)
                    {
                        cmd.Parameters.Add("@dataInicial", ini.Value);
                        cmd.Parameters.Add("@dataFinal", fim.Value);
                    }

                    using (var da = new FbDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }

            return dt;
        }
        private void chkPeriodo_CheckedChanged(object sender, EventArgs e)
        {
            cmbColunasData.Enabled = chkPeriodo.Checked;
            cmbColunasData.Visible = chkPeriodo.Checked;

            if (chkPeriodo.Checked)
            {
                carregandoColunasData = true;

                AtualizarColunasData();

                carregandoColunasData = false;
            }

            AtualizarPreview();
        }

        private void cmbColunasData_SelectedIndexChanged(object sender, EventArgs e)
        {
            colunaDataSelecionada = cmbColunasData.SelectedItem?.ToString();

            //MessageBox.Show("Coluna selecionada: " + colunaDataSelecionada);
        }

        public class RelatorioJson
        {
            public string Nome { get; set; }
            public string Descricao { get; set; }
            public string Query { get; set; }
            public List<ParametroJson> Parametros { get; set; } = new();
            public List<CampoJson> Campos { get; set; } = new();
            public bool PermiteExcel { get; set; }
            public bool Agrupado { get; set; }
        }

        public class ParametroJson
        {
            public string Nome { get; set; }
            public string Tipo { get; set; }
            public string Label { get; set; }
        }

        public class CampoJson
        {
            public string Nome { get; set; }
            public string Titulo { get; set; }
            public string Tipo { get; set; }
        }


        private void dgvColunas_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvColunas.IsCurrentCellDirty)
                dgvColunas.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void dgvColunas_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dgvColunas.Columns["Selecionar"].Index)
            {
                AtualizarPreview();
            }
        }

        private void btnGerar_Click(object sender, EventArgs e)
        {
            try
            {
                string sql = GerarSQL();

                if (string.IsNullOrWhiteSpace(sql))
                {
                    MessageBox.Show("Gere uma query primeiro.");
                    return;
                }

                var relatorio = new RelatorioJson
                {
                    Nome = txtNomeRelatorio.Text,
                    Descricao = "Gerado pelo sistema",
                    Query = sql,
                    PermiteExcel = true,
                    Agrupado = false
                };

                // 🔥 PARÂMETROS FIXOS (você pode depois deixar dinâmico pela UI)
                relatorio.Parametros.Add(new ParametroJson
                {
                    Nome = "empresa_id",
                    Tipo = "Int",
                    Label = "Empresa"
                });

                if (chkPeriodo.Checked)
                {
                    relatorio.Parametros.Add(new ParametroJson
                    {
                        Nome = "dataInicial",
                        Tipo = "DateTime",
                        Label = "Data Inicial"
                    });

                    relatorio.Parametros.Add(new ParametroJson
                    {
                        Nome = "dataFinal",
                        Tipo = "DateTime",
                        Label = "Data Final"
                    });
                }

                // 🔥 CAMPOS AUTOMÁTICOS
                foreach (DataGridViewRow row in dgvColunas.Rows)
                {
                    if (row.IsNewRow) continue;

                    bool selecionado = Convert.ToBoolean(row.Cells["Selecionar"].Value ?? false);
                    if (!selecionado) continue;

                    string coluna = row.Cells["COLUNA"].Value?.ToString();
                    string tabela = row.Cells["TABELA"].Value?.ToString();

                    relatorio.Campos.Add(new CampoJson
                    {
                        Nome = coluna,
                        Titulo = coluna,
                        Tipo = "String"
                    });
                }

                // 🔥 SERIALIZAR JSON
                var json = System.Text.Json.JsonSerializer.Serialize(relatorio,
                    new System.Text.Json.JsonSerializerOptions
                    {
                        WriteIndented = true
                    });

                // 🔥 SALVAR EM PASTA
                string pasta = @"D:\Projeto_relatorio\TelaPrincipal\TelaPrincipal\bin\Debug\Json";

                if (!System.IO.Directory.Exists(pasta))
                    System.IO.Directory.CreateDirectory(pasta);

                string nomeArquivo = relatorio.Nome;

                if (string.IsNullOrWhiteSpace(nomeArquivo))
                    nomeArquivo = "Relatorio_Sem_Nome";

                nomeArquivo = string.Join("_", nomeArquivo.Split(Path.GetInvalidFileNameChars()));

                string arquivo = Path.Combine(pasta, nomeArquivo + ".json");

                System.IO.File.WriteAllText(arquivo, json);

                MessageBox.Show("JSON criado com sucesso:\n" + arquivo);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao criar JSON:\n" + ex.Message);
            }
        }

        private void dgvResultado_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            MessageBox.Show("Evento disparou");

            if (e.ColumnIndex == dgvColunas.Columns["Selecionar"].Index)
            {
                AtualizarPreview();
            }
        }

        private void dgvResultado_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvColunas.IsCurrentCellDirty)
                dgvColunas.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }
    }
}