using System;
using FirebirdSql.Data.FirebirdClient;
using TelaPrincipal.Utils;

namespace TelaPrincipal.DAO
{
    public static class ConexaoFactory
    {
        private static string _connectionString;

        public static void Inicializar(string connectionString)
        {
            _connectionString = connectionString;
        }

        private static void CarregarConfiguracao()
        {
            if (!string.IsNullOrWhiteSpace(_connectionString))
                return;

            var config = ConfigHelper.Carregar();

            if (config == null)
                throw new InvalidOperationException(
                    "Configuração do banco não encontrada."
                );

            _connectionString =
                $"Database={config.CaminhoBanco};" +
                $"DataSource={config.Servidor};" +
                $"User={config.Usuario};" +
                $"Password={config.Senha};" +
                $"Port={config.Porta};" +
                $"Dialect=3;";
        }

        public static FbConnection CriarConexao()
        {
            CarregarConfiguracao();

            return new FbConnection(_connectionString);
        }
    }
}