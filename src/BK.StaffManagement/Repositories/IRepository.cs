using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BK.StaffManagement.Repositories
{
    public interface IRepository<out T> : IDisposable
       where T : class
    {
        // Factories
        bool Is<TRepository>(TRepository repo) where TRepository : class;

        T Get(string id);
        IEnumerable<T> GetMany(params string[] ids);
        IEnumerable<T> All(int? limit = null, int? offset = null);
        string Create(DynamicParameters @params);
        IEnumerable<string> CreateMany(IEnumerable<DynamicParameters> @params);
        void Delete(string id);
        void DeleteMany(params string[] ids);
        void Update(string id, DynamicParameters @params);
        void UpdateMany(IDictionary<string, DynamicParameters> @params);
        void Commit();
        void RollBack();
    }
}
