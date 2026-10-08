using System.Data;
using Biblioteca.Web.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Biblioteca.Web.Repositorios;

public interface ISocioRepositorio
{
    Task<IEnumerable<Socio>> ListarAsync();
    Task<bool> ExisteDniAsync(string dni);
    Task CrearAsync(Socio socio);
}

public class SocioRepositorio : ISocioRepositorio
{
    private readonly string _connectionString;

    public SocioRepositorio(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Biblioteca")
            ?? throw new InvalidOperationException("No se encontro la cadena de conexion 'Biblioteca'.");
    }

    public async Task<IEnumerable<Socio>> ListarAsync()
    {
        await using var connection = new SqlConnection(_connectionString);
        return await connection.QueryAsync<Socio>(
            "sp_Socios_Listar",
            commandType: CommandType.StoredProcedure);
    }

    public async Task<bool> ExisteDniAsync(string dni)
    {
        var socios = await ListarAsync();
        return socios.Any(s => string.Equals(s.DNI, dni.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    public async Task CrearAsync(Socio socio)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync(
            "sp_Socios_Insertar",
            new { socio.DNI, socio.Nombre, socio.Email },
            commandType: CommandType.StoredProcedure);
    }
}
