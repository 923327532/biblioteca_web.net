using System.Data;
using Biblioteca.Web.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Biblioteca.Web.Repositorios;

public interface ILibroRepositorio
{
    Task<IEnumerable<Libro>> ListarAsync(string? titulo);
    Task<Libro?> ObtenerPorIdAsync(int id);
    Task<IEnumerable<Autor>> ListarAutoresAsync();
    Task CrearAsync(Libro libro);
    Task ActualizarAsync(Libro libro);
    Task EliminarAsync(int id);
}

public class LibroRepositorio : ILibroRepositorio
{
    private readonly string _connectionString;

    public LibroRepositorio(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Biblioteca")
            ?? throw new InvalidOperationException("No se encontro la cadena de conexion 'Biblioteca'.");
    }

    public async Task<IEnumerable<Libro>> ListarAsync(string? titulo)
    {
        await using var connection = new SqlConnection(_connectionString);
        var procedimiento = string.IsNullOrWhiteSpace(titulo)
            ? "sp_Libros_Listar"
            : "sp_Libros_BuscarPorTitulo";
        var parametros = string.IsNullOrWhiteSpace(titulo)
            ? null
            : new { Titulo = titulo.Trim() };

        return await connection.QueryAsync<Libro>(
            procedimiento,
            parametros,
            commandType: CommandType.StoredProcedure);
    }

    public async Task<Libro?> ObtenerPorIdAsync(int id)
    {
        await using var connection = new SqlConnection(_connectionString);
        var libros = await connection.QueryAsync<Libro>(
            "sp_Libros_ObtenerPorId",
            new { LibroId = id },
            commandType: CommandType.StoredProcedure);

        return libros.FirstOrDefault();
    }

    public async Task<IEnumerable<Autor>> ListarAutoresAsync()
    {
        await using var connection = new SqlConnection(_connectionString);
        return await connection.QueryAsync<Autor>(
            "sp_Autores_ListarActivos",
            commandType: CommandType.StoredProcedure);
    }

    public async Task CrearAsync(Libro libro)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync(
            "sp_Libros_Insertar",
            new { libro.Titulo, libro.ISBN, libro.AutorId, libro.Ejemplares },
            commandType: CommandType.StoredProcedure);
    }

    public async Task ActualizarAsync(Libro libro)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync(
            "sp_Libros_Actualizar",
            new { libro.LibroId, libro.Titulo, libro.ISBN, libro.AutorId, libro.Ejemplares },
            commandType: CommandType.StoredProcedure);
    }

    public async Task EliminarAsync(int id)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync(
            "sp_Libros_Eliminar",
            new { LibroId = id },
            commandType: CommandType.StoredProcedure);
    }
}
