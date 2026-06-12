using System;
using System.IO;
using System.Net.Mail;
using System.Net;
using System.Windows.Forms;
using TelaPrincipal.Models;
using TelaPrincipal.Utils;

namespace TelaPrincipal.Views
{
    public partial class EnviarEmailForm : Form
    {
        public static class EmailUtils
        {
            public static void EnviarEmail(string assunto, string corpo, string caminhoAnexo, ConfigEmail config, string destinatario)
            {
                try
                {
                    using (var mail = new MailMessage())
                    {
                        mail.From = new MailAddress(config.Remetente);
                        mail.To.Add(destinatario); // destinatário dinâmico
                        mail.Subject = assunto;
                        mail.Body = corpo;
                        mail.IsBodyHtml = true;

                        if (!string.IsNullOrEmpty(caminhoAnexo) && File.Exists(caminhoAnexo))
                            mail.Attachments.Add(new Attachment(caminhoAnexo));

                        using (var smtp = new SmtpClient(config.Smtp, config.Porta))
                        {
                            smtp.Credentials = new NetworkCredential(config.Usuario, config.Senha);
                            smtp.EnableSsl = config.UsaSSL;
                            smtp.Send(mail);
                        }
                    }

                    MessageBox.Show("E-mail enviado com sucesso!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao enviar e-mail: " + ex.Message);
                }
            }
        }
    }
}