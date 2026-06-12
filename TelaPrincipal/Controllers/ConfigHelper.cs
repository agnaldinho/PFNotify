using System;
using System.IO;
using Newtonsoft.Json;
using TelaPrincipal.Models;

namespace TelaPrincipal.Utils
{
    public static class ConfigHelper
    {
        private static string CaminhoConfig()
        {
            string pasta = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "SeuSistema"
            );

            if (!Directory.Exists(pasta))
                Directory.CreateDirectory(pasta);

            return Path.Combine(pasta, "config.json");
        }

        public static void Salvar(BancoConfig config)
        {
            var json = JsonConvert.SerializeObject(config, Formatting.Indented);
            File.WriteAllText(CaminhoConfig(), json);
        }

        public static BancoConfig Carregar()
        {
            var caminho = CaminhoConfig();

            if (!File.Exists(caminho))
                return null;

            var json = File.ReadAllText(caminho);
            return JsonConvert.DeserializeObject<BancoConfig>(json);
        }

        public static bool ExisteConfig()
        {
            return File.Exists(CaminhoConfig());
        }

        public static void ExcluirConfig()
        {
            var caminho = CaminhoConfig();

            if (File.Exists(caminho))
                File.Delete(caminho);
        }
    }
}