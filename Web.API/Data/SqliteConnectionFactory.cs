// using System.Data;
// using Microsoft.Data.Sqlite;
//
// namespace Web.API.Data;
//
// public class SqliteConnectionFactory : IDbConnectionFactory
// {
//     private readonly string _connectionString;
//
//     public SqliteConnectionFactory(string connectionString)
//     {
//         _connectionString = connectionString;
//     }
//
//     public async Task<IDbConnection> GetConnectionAsync()
//     {
//         var connection = new SqliteConnection(_connectionString);
//         await connection.OpenAsync();
//         return connection;
//     }
// }