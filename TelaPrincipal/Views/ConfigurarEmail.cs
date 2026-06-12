using System;
using System.Windows.Forms;
using TelaPrincipal.Controllers;
using TelaPrincipal.Models;
using TelaPrincipal.Utils;

namespace TelaPrincipal.Views
{
    public partial class ConfigurarEmail : Form
    {
        public ConfigurarEmail()
        {
            InitializeComponent();
            CarregarConfig();
        }

        private void CarregarConfig()
        {
            try
            {
                var config = ConfigEmailHelper.Carregar();
                txtSenha.UseSystemPasswordChar = true;

                // Preenche os campos do formulário
                txtSmtp.Text = config.Smtp;
                txtPorta.Text = config.Porta.ToString();
                txtUsuario.Text = config.Usuario;
                txtSenha.Text = config.Senha;
                txtRemetente.Text = config.Remetente;

                if (ChkSSL != null)
                    ChkSSL.Checked = config.UsaSSL;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar configurações: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGravar_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtSmtp.Text) || string.IsNullOrWhiteSpace(txtPorta.Text))
                {
                    MessageBox.Show("Por favor, preencha os campos obrigatórios (SMTP e Porta).", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                bool usaSSL = ChkSSL != null && ChkSSL.Checked;

                var config = new ConfigEmail
                {
                    Smtp = txtSmtp.Text,
                    Porta = int.Parse(txtPorta.Text),
                    Usuario = txtUsuario.Text,
                    Senha = txtSenha.Text,
                    Remetente = txtRemetente.Text,
                    UsaSSL = usaSSL
                };

                ConfigEmailHelper.Salvar(config);
                MessageBox.Show("Configuração gravada com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao gravar: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnTestar_Click(object sender, EventArgs e)
        {
            try
            {
                var config = new ConfigEmail
                {
                    Smtp = txtSmtp.Text,
                    Porta = int.Parse(txtPorta.Text),
                    Usuario = txtUsuario.Text,
                    Senha = txtSenha.Text,
                    Remetente = txtRemetente.Text,
                    UsaSSL = ChkSSL.Checked
                };

                EmailUtils.EnviarEmail(
                    assunto: "Teste de E-mail",
                    corpo: "Este é um e-mail de teste para verificar a configuração do SMTP.",
                    caminhoAnexo: null,
                    config: config,
                    destinatario: config.Remetente
                );

                MessageBox.Show("E-mail de teste enviado com sucesso! Verifique sua caixa de entrada.", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao testar e-mail: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}