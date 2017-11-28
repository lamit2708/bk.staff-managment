using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using BK.StaffManagement.Extensions;

namespace BK.StaffManagement.Repositories
{
    public abstract class BaseRepository<T> : IRepository<T>
        where T : class
    {
        protected Type DataType => typeof(T);
        protected IDbConnection Connection;
        protected IDbTransaction Transaction;

        protected BaseRepository(IDbConnection conn, IDbTransaction trans)
        {
            Connection = conn;
            Transaction = trans;
        }

        public bool Is<TRepository>(TRepository repo) where TRepository : class
        {
            return typeof(TRepository).FullName == DataType.FullName;
        }

        public T Get(string id)
        {
            var tableName = typeof(T).GetTableName();
            return Connection
                .Query<T>(
                    $"SELECT * FROM [{tableName}] WHERE Id = @Id",
                    param: new { Id = id },
                    transaction: Transaction)
                .FirstOrDefault();
        }

        public IEnumerable<T> GetMany(params string[] ids)
        {
            var tableName = typeof(T).GetTableName();
            return Connection
                .Query<T>(
                    $"SELECT * FROM [{tableName}] WHERE Id IN @IdS",
                    param: new { Ids = ids },
                    transaction: Transaction);
        }

        public IEnumerable<T> All(int? limit = null, int? offset = null)
        {
            var tableName = typeof(T).GetTableName();
            limit = limit ?? 100;
            offset = offset ?? 0;
            return Connection
                .Query<T>(
                    $@"SELECT * FROM [{tableName}]
ORDER BY [Id]
OFFSET {offset} ROWS
FETCH NEXT {limit} ROWS ONLY;",
                    transaction: Transaction);
        }

        public string Create(DynamicParameters @params)
        {
            var tableName = typeof(T).GetTableName();
            var columns = string.Join(", ", @params.ParameterNames.Select(p => $"[{p}]"));
            var values = string.Join(", ", @params.ParameterNames.Select(p => $"@{p}"));

            var sql = $@"
DECLARE @Output TABLE(id nvarchar(450))
INSERT INTO [{tableName}] ({columns})
OUTPUT INSERTED.Id INTO @Output
VALUES ({values})
SELECT [id] FROM @Output
";

            return Connection
                .Query<string>(
                    sql,
                    param: @params,
                    transaction: Transaction)
                .FirstOrDefault();
        }

        public IEnumerable<string> CreateMany(IEnumerable<DynamicParameters> @params)
        {
            if (@params == null)
            {
                return new List<string>();
            }
            return @params.Select(Create).ToList();
        }

        public void Delete(string id)
        {
            var tableName = typeof(T).GetTableName();
            Connection
                .Execute(
                    $"DELETE FROM [{tableName}] WHERE [Id] = @Id",
                    param: new { Id = id },
                    transaction: Transaction);
        }

        public void DeleteMany(params string[] ids)
        {
            var tableName = typeof(T).GetTableName();
            Connection
                .Execute(
                    $"DELETE FROM [{tableName}] WHERE [Id] IN @Ids",
                    param: new { Ids = ids },
                    transaction: Transaction);
        }

        public void Update(string id, DynamicParameters @params)
        {
            var tableName = typeof(T).GetTableName();
            var setters = string.Join(", ", @params.ParameterNames.Select(p => $"[{p}] = @{p}"));
            var sql = $"UPDATE {tableName} SET {setters} WHERE [Id] = @Id";
            //// make sure that @Id is included
            @params.Add("@Id", id);
            Connection
                .Execute(
                    sql,
                    param: @params,
                    transaction: Transaction);
        }

        public void UpdateMany(IDictionary<string, DynamicParameters> @params)
        {
            foreach (var param in @params)
            {
                Update(param.Key, param.Value);
            }
        }

        public void Commit()
        {
            Transaction?.Commit();
        }

        public void RollBack()
        {
            Transaction?.Rollback();
        }

        public void Dispose()
        {
            Connection?.Close();
            Transaction?.Dispose();
            Connection?.Dispose();
        }

       
    }
}
