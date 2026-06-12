using System;
using System.IO;
using System.Windows.Forms;
using FirebirdSql.Data.FirebirdClient;
using Newtonsoft.Json;
using TelaPrincipal.DAO;
using TelaPrincipal.Models;

namespace TelaPrincipal.Views
{
    public partial class CarregarBanco : Form
    {
        public CarregarBanco()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;

            // Centraliza o painel de botões em relação ao panel2 (área escura)
            var panelBotoes = this.panel2.Controls["panelBotoes"];
            if (panelBotoes != null)
            {
                panelBotoes.Location = new System.Drawing.Point(
                    (this.panel2.Width - panelBotoes.Width) / 2,
                    this.panel2.Height - panelBotoes.Height - 40 // 40 pixels de distância do rodapé
                );
            }
        }

        private string CaminhoConfig()
        {
            string pasta = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "SeuSistema"
            );

            if (!Directory.Exists(pasta))
                Directory.CreateDirectory(pasta);

            return Path.Combine(pasta, "config.json");
        }

        private string MontarConnectionString(BancoConfig config)
        {
            return $"Database={config.CaminhoBanco};" +
                   $"DataSource={config.Servidor};" +
                   $"User={config.Usuario};" +
                   $"Password={config.Senha};" +
                   $"Port={config.Porta};" + // 
                   $"Dialect=3;";
        }

        private bool TestarConexao(string connectionString)
        {
            try
            {
                using (var conn = new FbConnection(connectionString))
                {
                    conn.Open();
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao conectar: " + ex.Message);
                return false;
            }
        }

        private void SalvarConfig(BancoConfig config)
        {
            var json = JsonConvert.SerializeObject(config, Formatting.Indented);
            File.WriteAllText(CaminhoConfig(), json);
        }

        private BancoConfig CarregarConfig()
        {
            var caminho = CaminhoConfig();

            if (!File.Exists(caminho))
                return null;

            var json = File.ReadAllText(caminho);
            return JsonConvert.DeserializeObject<BancoConfig>(json);
        }

        private void CarregarBanco_Load(object sender, EventArgs e)
        {
            var config = CarregarConfig();

            if (config != null)
            {
                txtSenha.UseSystemPasswordChar = true;

                txtIp.Text = config.Servidor;
                txtPorta.Text = config.Porta;
                txtDiretorio.Text = config.CaminhoBanco; 
                txtBanco.Text = Path.GetFileNameWithoutExtension(config.CaminhoBanco);
                txtUsuario.Text = config.Usuario;
                txtSenha.Text = config.Senha;

                var connStr = MontarConnectionString(config);
                ConexaoFactory.Inicializar(connStr);
            }
        }

        private void btnSelecionarArquivo_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Selecionar Banco de Dados";
                openFileDialog.Filter = "Banco Firebird (*.fdb)|*.fdb|Todos (*.*)|*.*";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string caminhoCompleto = openFileDialog.FileName;

                    txtDiretorio.Text = caminhoCompleto;
                    txtBanco.Text = Path.GetFileNameWithoutExtension(caminhoCompleto);

                    // 🔥 Detecta servidor
                    if (caminhoCompleto.StartsWith(@"\\"))
                        txtIp.Text = caminhoCompleto.Split('\\')[2];
                    else
                        txtIp.Text = "localhost";

                    // ✅ PADRÃO DEFINIDO AQUI
                    txtPorta.Text = "3050";

                    txtUsuario.Text = "SYSDBA";
                    txtSenha.Text = "masterkey";

                    var config = new BancoConfig
                    {
                        Servidor = txtIp.Text,
                        CaminhoBanco = caminhoCompleto,
                        Usuario = txtUsuario.Text,
                        Senha = txtSenha.Text,
                        Porta = "3050" // ✅ garante padrão na config
                    };

                    SalvarConfig(config);

                    var connStr = MontarConnectionString(config);
                    ConexaoFactory.Inicializar(connStr);

                    MessageBox.Show("Banco selecionado e configurado com sucesso!");
                }
            }
        }

        private void btnGravar_Click(object sender, EventArgs e)
        {
            // ✅ validação básica
            if (string.IsNullOrWhiteSpace(txtDiretorio.Text))
            {
                MessageBox.Show("Informe o caminho do banco.");
                return;
            }

            var config = new BancoConfig
            {
                Servidor = txtIp.Text,
                CaminhoBanco = txtDiretorio.Text, // ✅ caminho completo
                Usuario = txtUsuario.Text,
                Senha = txtSenha.Text,
                Porta = txtPorta.Text
            };

            var connStr = MontarConnectionString(config);

            if (!TestarConexao(connStr))
                return;

            SalvarConfig(config);
            ConexaoFactory.Inicializar(connStr);

            MessageBox.Show("Configuração salva com sucesso!");
        }

        private void btnTestarConexao_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtIp.Text))
            {
                MessageBox.Show("Informe o servidor.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtDiretorio.Text))
            {
                MessageBox.Show("Informe o caminho do banco.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtUsuario.Text))
            {
                MessageBox.Show("Informe o usuário.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPorta.Text))
            {
                txtPorta.Text = "3050";
            }

            var config = new BancoConfig
            {
                Servidor = txtIp.Text,
                CaminhoBanco = txtDiretorio.Text,
                Usuario = txtUsuario.Text,
                Senha = txtSenha.Text,
                Porta = txtPorta.Text
            };

            var connStr = MontarConnectionString(config);

            Cursor.Current = Cursors.WaitCursor;

            try
            {
                using (var conn = new FbConnection(connStr))
                {
                    conn.Open();
                }

                MessageBox.Show("Conexão realizada com sucesso!",
                                "Sucesso",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Falha na conexão:\n\n{ex.Message}",
                                "Erro",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }
    }
}