using AutoMapper;
using BK.StaffManagement.Extensions;
using BK.StaffManagement.Models;
using BK.StaffManagement.ViewModels;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace BK.StaffManagement.Repositories
{

    public class StaffRepository : BaseRepository<Staff>
    {
        public StaffRepository(IDbConnection conn, IDbTransaction trans) : base(conn, trans)
        {

        }
        public int Count(string search)
        {
            var searchCondition = !string.IsNullOrWhiteSpace(search)
                ? $"WHERE u.FirstName LIKE '%{search}%' " +
                $"OR u.LastName LIKE '%{search}%' " +
                $"OR u.PhoneNumber LIKE '%{search}%'" +
                $"OR u.Email LIKE '%{search}%'" +
                $"OR c.Title LIKE '%{search}%' " +
                $"OR c.StaffCode LIKE '%{search}%' " 
                : string.Empty;
            var count = Connection.Query<int>($@"
SELECT COUNT(c.Id) FROM Staff c
INNER JOIN AspNetUsers u ON c.Id = u.Id
{searchCondition}
", transaction: Transaction).FirstOrDefault();
            return count;
        }
        public StaffViewModel Get(string id)
        {

            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ApplicationUser, StaffViewModel>();
                cfg.CreateMap<Staff, StaffViewModel>();
            });
            var mapperConf = mapper.CreateMapper();
            var staff = Connection.Query<Staff, ApplicationUser, StaffViewModel>($@"
SELECT c.*, u.* FROM Staff c
INNER JOIN AspNetUsers u ON c.Id = u.Id
WHERE c.Id='{id}'", (c, u) =>
            {
                var result = mapperConf.Map<StaffViewModel>(u);
                result = mapperConf.Map(c, result);
                return result;
            }, transaction: Transaction,
                    splitOn: "Id").FirstOrDefault();
            return staff;
        }


        //        public IEnumerable<StaffViewModel> All()
        //        {
        //            var mapper = new MapperConfiguration(cfg => {
        //                cfg.CreateMap<ApplicationUser, StaffViewModel>();
        //                cfg.CreateMap<Staff, StaffViewModel>();
        //            });
        //            var mapperConf = mapper.CreateMapper();
        //            var customers = Connection.Query<Staff, ApplicationUser, StaffViewModel>(@"
        //SELECT c.*, u.* FROM Staff c
        //INNER JOIN AspNetUsers u ON c.Id = u.Id", (c, u) =>
        //            {
        //                var result = mapperConf.Map<StaffViewModel>(u);
        //                result = mapperConf.Map(c, result);
        //                return result;
        //            }, transaction: Transaction,
        //                    splitOn: "Id");
        //            return customers;
        //        }
        public IEnumerable<StaffViewModel> All(string search, int? limit = null, int? offset = null)
        {
            var tableName = typeof(Staff).GetTableName();
            limit = limit ?? 100;
            offset = offset ?? 0;

            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ApplicationUser, StaffViewModel>();
                cfg.CreateMap<Staff, StaffViewModel>();
            });
            var mapperConf = mapper.CreateMapper();
            var searchCondition = !string.IsNullOrWhiteSpace(search)
                ? $"WHERE u.FirstName LIKE '%{search}%' " +
                $"OR u.LastName LIKE '%{search}%'" +
                $"OR u.PhoneNumber LIKE '%{search}%'" +
                $"OR u.Email LIKE '%{search}%'" +
                $"OR c.Title LIKE '%{search}%' " +
                $"OR c.StaffCode LIKE '%{search}%' "
                : string.Empty;
            var customers = Connection.Query<Staff, ApplicationUser, StaffViewModel>($@"
SELECT c.*, u.* FROM [{tableName}] c
INNER JOIN AspNetUsers u ON c.Id = u.Id
{searchCondition}
ORDER BY c.StaffCode
OFFSET {offset} ROWS
FETCH NEXT {limit} ROWS ONLY;", (c, u) =>
            {
                var result = mapperConf.Map<StaffViewModel>(u);
                result = mapperConf.Map(c, result);
                return result;
            }, transaction: Transaction,
                    splitOn: "Id");
            return customers;
        }
        public IEnumerable<StaffViewModel> AllUser(int? limit = null, int? offset = null)
        {
            var tableName = typeof(Staff).GetTableName();
            limit = limit ?? 100;
            offset = offset ?? 0;

            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ApplicationUser, StaffViewModel>();
                cfg.CreateMap<Staff, StaffViewModel>();
            });
            var mapperConf = mapper.CreateMapper();

            var customers = Connection.Query<Staff, ApplicationUser, StaffViewModel>($@"
SELECT c.*, u.* FROM [{tableName}] c
INNER JOIN AspNetUsers u ON c.Id = u.Id
ORDER BY c.StaffCode
OFFSET {offset} ROWS
FETCH NEXT {limit} ROWS ONLY;", (c, u) =>
            {
                var result = mapperConf.Map<StaffViewModel>(u);
                result = mapperConf.Map(c, result);
                return result;
            }, transaction: Transaction,
                    splitOn: "Id");
            return customers;
        }
    }
}
