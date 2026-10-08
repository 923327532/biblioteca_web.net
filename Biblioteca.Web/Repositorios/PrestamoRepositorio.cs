using System.Data;
using Biblioteca.Web.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Biblioteca.Web.Repositorios;

public interface IPrestamoRepositorio
{
    Task<IEnumerable<PrestamoReporte>> ReporteAsync(DateTime desde, DateTime hasta);
}

public class PrestamoRepositorio : IPrestamoRepositorio
{
    private readonly string _connectionString;

    public PrestamoRepositorio(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Biblioteca")
            ?? throw new InvalidOperationException("No se encontro la cadena de conexion 'Biblioteca'.");
    }

    public async Task<IEnumerable<PrestamoReporte>> ReporteAsync(DateTime desde, DateTime hasta)
    {
        await using var connection = new SqlConnection(_connectionString);
        return await connection.QueryAsync<PrestamoReporte>(
            "sp_Prestamos_ReportePorFechas",
            new { FechaInicio = desde.Date, FechaFin = hasta.Date },
            commandType: CommandType.StoredProcedure);
    }
}
