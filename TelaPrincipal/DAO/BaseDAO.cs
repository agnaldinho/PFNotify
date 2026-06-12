using FirebirdSql.Data.FirebirdClient;
using System.Collections.Generic;
using System.Data;
using TelaPrincipal.DAO;

public class BaseDAO
{
    public DataTable ExecutarQuery(string sql, List<FbParameter> parametros = null)
    {
        using (var conn = ConexaoFactory.CriarConexao())
        {
            conn.Open();

            using (var cmd = new FbCommand(sql, conn))
            {
                if (parametros != null)
                    cmd.Parameters.AddRange(parametros.ToArray());

                var da = new FbDataAdapter(cmd);
                var dt = new DataTable();
                da.Fill(dt);

                return dt;
            }
        }
    }

    protected int ExecutarNonQuery(string sql, List<FbParameter> parametros = null)
    {
        using (var conn = ConexaoFactory.CriarConexao())
        {
            conn.Open();

            using (var cmd = new FbCommand(sql, conn))
            {
                if (parametros != null)
                    cmd.Parameters.AddRange(parametros.ToArray());

                return cmd.ExecuteNonQuery();
            }
        }
    }

    protected object ExecutarScalar(string sql, List<FbParameter> parametros = null)
    {
        using (var conn = ConexaoFactory.CriarConexao())
        {
            conn.Open();

            using (var cmd = new FbCommand(sql, conn))
            {
                if (parametros != null)
                    cmd.Parameters.AddRange(parametros.ToArray());

                return cmd.ExecuteScalar();
            }
        }
    }
}