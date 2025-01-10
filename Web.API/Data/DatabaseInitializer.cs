// using Dapper;
//
// namespace Web.API.Data;
//
// public class DatabaseInitializer
// {
//     private readonly IDbConnectionFactory _connectionFactory;
//
//     public DatabaseInitializer(IDbConnectionFactory connectionFactory)
//     {
//         _connectionFactory = connectionFactory;
//     }
//
//     public async Task InitializeAsync()
//     {
//         using var connection = await _connectionFactory.GetConnectionAsync();
//         await connection.ExecuteAsync(
//             @"CREATE TABLE IF NOT EXISTS Books(
//         
//                 Isbn TEXT PRIMARY KEY,
//                 Title TEXT NOT NULL,
//                 Author TEXT NOT NULL,
//                 ShortDescription TEXT NOT NULL,
//                 PageCount INTERGER,
//                 ReleaseDate TEXT NOT NULL);
//
// ");
//     }
//
// }