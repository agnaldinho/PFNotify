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
                   $"Port=3050;" +
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
                MessageBox.Show("Erro: " + ex.Message);
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
                txtIp.Text = config.Servidor;
                txtDiretorio.Text = config.CaminhoBanco;
                txtBanco.Text = Path.GetFileNameWithoutExtension(config.CaminhoBanco);
                txtUsuario.Text = config.Usuario;
                txtSenha.Text = config.Senha;
                txtPorta.Text = config.Porta;

                var connStr = MontarConnectionString(config);
                ConexaoFactory.Inicializar(connStr);
            }
        }

        private void btnTestar_Click(object sender, EventArgs e)
        {
            var config = new BancoConfig
            {
                Servidor = txtIp.Text,
                CaminhoBanco = txtDiretorio.Text,
                Usuario = txtUsuario.Text,
                Senha = txtSenha.Text,
                Porta = txtPorta.Text
            };

            var connStr = MontarConnectionString(config);

            if (TestarConexao(connStr))
                MessageBox.Show("Conexão realizada com sucesso!");
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            var config = new BancoConfig
            {
                Servidor = txtIp.Text,
                CaminhoBanco = txtDiretorio.Text,
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

        private void btnSelecionarArquivo_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Selecionar Banco de Dados";
                openFileDialog.Filter = "Banco Firebird (*.fdb)|*.fdb|Todos (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string caminho = openFileDialog.FileName;

                    txtDiretorio.Text = caminho;
                    txtBanco.Text = Path.GetFileNameWithoutExtension(caminho);

                    if (caminho.StartsWith(@"\\"))
                        txtIp.Text = caminho.Split('\\')[2];
                    else
                        txtIp.Text = "localhost";

                    txtUsuario.Text = "SYSDBA";
                    txtPorta.Text = "3050";
                    txtSenha.Text = "masterkey";

                    var config = new BancoConfig
                    {
                        Servidor = txtIp.Text,
                        CaminhoBanco = caminho,
                        Usuario = txtUsuario.Text,
                        Senha = txtSenha.Text
                    };

                    SalvarConfig(config);

                    var connStr = MontarConnectionString(config);
                    ConexaoFactory.Inicializar(connStr);

                    MessageBox.Show("Banco selecionado e configurado com sucesso!");
                }
            }
        }
    }
}