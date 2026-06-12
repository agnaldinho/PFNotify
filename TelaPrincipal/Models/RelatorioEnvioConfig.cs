using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelaPrincipal.Models
{
    public class RelatorioEnvioConfig
    {
        public List<string> RelatoriosSelecionados { get; set; }
        public string Frequencia { get; set; } 
        public string Email { get; set; }
        public int EmpresaId { get; set; }
        public TimeSpan HoraEnvio { get; set; }
    }
}
