//using APICatalago.Context;
//using Microsoft.EntityFrameworkCore;

//namespace ApiCatalogo.Services
//{
//    public class SqlQueryExecutor
//    {
//        private readonly AppDbContext _context;

//        public SqlQueryExecutor(AppDbContext context)
//        {
//            _context = context;
//        }

//        public List<T> ExecuteQuery<T>(string sql, params object[] parameters) where T : class
//        {
//            return _context.Set<T>().FromSqlRaw(sql).ToList();
//        }
//    }
//}
using APICatalago.Context;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using System.Dynamic;

namespace ApiCatalogo.Services
{
    public class SqlQueryExecutor
    {
        private readonly AppDbContext _context;

        public SqlQueryExecutor(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<ExpandoObject>> ExecuteQueryAsync(string sql)
        {
            var result = new List<ExpandoObject>();

            using var conn = _context.Database.GetDbConnection();
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                IDictionary<string, object> row = new ExpandoObject();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                }
                result.Add((ExpandoObject)row);
            }

            return result;
        }
       
    }
}
