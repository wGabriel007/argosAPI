using Npgsql;

namespace argos.Infra.Persistence;

public class Conexao
{
    public static string StringConexao => new NpgsqlConnectionStringBuilder
    {
        Host     = VariavelObrigatoria("DB_HOST"),
        Port     = int.Parse(VariavelObrigatoria("DB_PORT")),
        Database = VariavelObrigatoria("DB_NAME"),
        Username = VariavelObrigatoria("DB_USER"),
        Password = VariavelObrigatoria("DB_PASSWORD")
    }.ConnectionString;

    private static string VariavelObrigatoria(string nome)
    {
        return Environment.GetEnvironmentVariable(nome)
            ?? throw new InvalidOperationException(
                $"A variável de ambiente '{nome}' não foi configurada.");
    }
}