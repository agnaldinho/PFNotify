using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelaPrincipal.Models
{
    public class BancoConfig
    {
        public string Servidor { get; set; }
        public string CaminhoBanco { get; set; }
        public string Usuario { get; set; }
        public string Senha { get; set; }

        public string Porta { get; set; }
    }
}
