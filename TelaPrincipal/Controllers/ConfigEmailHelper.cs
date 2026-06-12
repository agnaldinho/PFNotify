using System;
using System.IO;
using Newtonsoft.Json;
using TelaPrincipal.Models;
using System.Windows.Forms;

namespace TelaPrincipal.Controllers
{
    public static class ConfigEmailHelper
    {
        private static readonly string Caminho = Path.Combine(Application.StartupPath, "Json", "ConfigEmail.json");

        public static void Salvar(ConfigEmail config)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(Caminho));
            File.WriteAllText(Caminho, JsonConvert.SerializeObject(config, Formatting.Indented));
        }

        public static ConfigEmail Carregar()
        {
            if (!File.Exists(Caminho))
                return new ConfigEmail(); // Retorna padrão

            try
            {
                var json = File.ReadAllText(Caminho);
                return JsonConvert.DeserializeObject<ConfigEmail>(json) ?? new ConfigEmail();
            }
            catch
            {
                return new ConfigEmail();
            }
        }
    }
}