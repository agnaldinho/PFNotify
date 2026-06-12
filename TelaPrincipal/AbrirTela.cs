using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FontAwesome.Sharp;
using TelaPrincipal.Views;

namespace TelaPrincipal
{
    public partial class AbrirTela : Form
    {
        Button botaoSelecionado = null;

        // Controle dinâmico do estado do menu lateral
        private bool menuVisivel = true;

        public AbrirTela()
        {
            InitializeComponent();
            pnMenu.Paint += PanelMenu_Paint;

            // 1. DESATIVAR ANCORAGENS EM CONFLITO: Como posicionamos estes painéis por código,
            // removemos a ancoragem automática do Designer para que os botões não fiquem esmagados.
            PnMexer.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            panel3.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            PnMostrar.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            panel1.Anchor = AnchorStyles.Top | AnchorStyles.Left;

            // 2. GARANTIR QUE OS BOTÕES SEGUEM O CANTO DIREITO CORRECTAMENTE
            BtnMinimizar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnMaximizar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            BtnSair.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnRev.Anchor = AnchorStyles.Top | AnchorStyles.Left; // Hambúrguer fixo à esquerda

            // 3. VINCULAR O EVENTO DE CLIQUE DO BOTÃO HAMBÚRGUER
            this.btnRev.Click += button1_Click;

            // Tela cheia ao iniciar
            this.WindowState = FormWindowState.Maximized;
            this.MinimumSize = new Size(900, 600);

            // Recalcula layout quando a janela muda de tamanho
            this.Resize += AbrirTela_Resize;
            AjustarLayout();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Inverte o estado do menu (Se visível -> esconde / Se escondido -> mostra)
            menuVisivel = !menuVisivel;
            AjustarLayout();
        }

        private void AbrirTela_Resize(object sender, EventArgs e)
        {
            AjustarLayout();
        }

        private void AjustarLayout()
        {
            int larguraTotal = this.ClientSize.Width;
            int alturaTotal = this.ClientSize.Height;

            // Define a largura base do menu dinamicamente
            int larguraMenu = menuVisivel ? 220 : 0;
            int alturaTopBar = 33;
            int alturaBreadcrumb = 37;

            // Controla a visibilidade física do painel contentor do menu
            panel1.Visible = menuVisivel;

            // Barra de título (topo) - Afasta-se para a direita se o menu existir, ou vai para o canto 0
            PnMexer.Location = new Point(larguraMenu, 0);
            PnMexer.Size = new Size(larguraTotal - larguraMenu, alturaTopBar);

            // Breadcrumb
            panel3.Location = new Point(larguraMenu, alturaTopBar);
            panel3.Size = new Size(larguraTotal - larguraMenu, alturaBreadcrumb);

            // Área de conteúdo principal
            PnMostrar.Location = new Point(larguraMenu, alturaTopBar + alturaBreadcrumb);
            PnMostrar.Size = new Size(larguraTotal - larguraMenu, alturaTotal - alturaTopBar - alturaBreadcrumb);

            // Menu lateral ocupa toda a altura apenas se estiver visível
            if (menuVisivel)
            {
                panel1.Size = new Size(larguraMenu, alturaTotal);
                pnMenu.Size = new Size(larguraMenu, alturaTotal - 98);
            }
        }


        private void Hub(Form formularioParaMostrar)
        {
            PnMostrar.Controls.Clear();
            formularioParaMostrar.TopLevel = false;
            formularioParaMostrar.FormBorderStyle = FormBorderStyle.None;
            formularioParaMostrar.Dock = DockStyle.Fill;
            PnMostrar.Controls.Add(formularioParaMostrar);
            formularioParaMostrar.Show();
        }

        private void BtnMinimizar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void BtnSair_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        private void PnMexer_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void BtnRelatorio_Click(object sender, EventArgs e)
        {
            Button botao = (Button)sender;
            lbMostrar.Text = botao.Text;
            PnMostrar.Controls.Clear();
            RelatoriosProntos relatoriosprontos = new RelatoriosProntos();
            Hub(relatoriosprontos);
        }

        private void BtnPersonalizar_Click(object sender, EventArgs e)
        {
            Button botao = (Button)sender;
            lbMostrar.Text = botao.Text;
            PnMostrar.Controls.Clear();
            RelatoriosPersonalizados relatoriosP = new RelatoriosPersonalizados();
            Hub(relatoriosP);
        }

        private void BtnBanco_Click(object sender, EventArgs e)
        {
            Button botao = (Button)sender;
            lbMostrar.Text = botao.Text;
            PnMostrar.Controls.Clear();
            CarregarBanco bancoDeDados = new CarregarBanco();
            Hub(bancoDeDados);
        }

        private void btnEnvio_Click(object sender, EventArgs e)
        {
            Button botao = (Button)sender;
            lbMostrar.Text = botao.Text;
            PnMostrar.Controls.Clear();
            ConfigurarEnvioEmail EnvioEmail = new ConfigurarEnvioEmail();
            Hub(EnvioEmail);
        }

        private void btnGeren_Click(object sender, EventArgs e)
        {
            Button botao = (Button)sender;
            lbMostrar.Text = botao.Text;
            PnMostrar.Controls.Clear();
            GerenciarRelatorios gerenciar = new GerenciarRelatorios();
            Hub(gerenciar);
        }

        private void BtnEmail_Click(object sender, EventArgs e)
        {
            Button botao = (Button)sender;
            lbMostrar.Text = botao.Text;
            PnMostrar.Controls.Clear();
            ConfigurarEmail email = new ConfigurarEmail();
            Hub(email);
        }

        bool menuAberto = false;

        private void btnGerenciar_Click(object sender, EventArgs e)
        {
            if (pnSubmenuGer.Visible == false)
                pnSubmenuGer.Visible = true;
            else
                pnSubmenuGer.Visible = false;

            menuAberto = !menuAberto;
            pnSubmenuGer.Visible = menuAberto;
        }

        private void BtnRelatoriosMenu_Click(object sender, EventArgs e)
        {
            if (PnRelatoriosSubmenu.Visible == false)
                PnRelatoriosSubmenu.Visible = true;
            else
                PnRelatoriosSubmenu.Visible = false;
        }

        private void btnConfig_Click_1(object sender, EventArgs e)
        {
            if (pnConfigSub.Visible == false)
                pnConfigSub.Visible = true;
            else
                pnConfigSub.Visible = false;

            menuAberto = !menuAberto;
            pnConfigSub.Visible = menuAberto;
        }

        private void PanelMenu_Paint(object sender, PaintEventArgs e)
        {
            using (LinearGradientBrush brush = new LinearGradientBrush(
                pnMenu.ClientRectangle,
                Color.FromArgb(28, 36, 70),      // topo
                Color.FromArgb(18, 24, 100),      // baixo
                LinearGradientMode.Vertical))
            {
                e.Graphics.FillRectangle(brush, pnMenu.ClientRectangle);
            }
        }

        private void btnMaximizar_Click(object sender, EventArgs e)
        {
            // Alterna entre Maximizado e Normal de forma limpa
            if (this.WindowState == FormWindowState.Maximized)
                this.WindowState = FormWindowState.Normal;
            else
                this.WindowState = FormWindowState.Maximized;
        }

        private void btnRev_Click(object sender, EventArgs e)
        {

        }
    }
}