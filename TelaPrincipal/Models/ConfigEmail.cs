using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelaPrincipal.Models
{
    public class ConfigEmail
    {
        public string Smtp { get; set; } = "";
        public int Porta { get; set; } = 587;
        public string Usuario { get; set; } = "";
        public string Senha { get; set; } = "";
        public string Remetente { get; set; } = "";
        public string DestinatarioPadrao { get; set; } = "";
        public bool UsaSSL { get; set; } = true;
    }
}
