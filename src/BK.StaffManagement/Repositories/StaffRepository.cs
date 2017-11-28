using AutoMapper;
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
        public StaffViewModel Get(string id)
        {
           
            var mapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<ApplicationUser, StaffViewModel>();
                cfg.CreateMap<Customer, StaffViewModel>();
            });
            var mapperConf = mapper.CreateMapper();
            var customer = Connection.Query<Customer, ApplicationUser, StaffViewModel>($@"
SELECT c.*, u.* FROM Staff c
INNER JOIN AspNetUsers u ON c.Id = u.Id
WHERE c.Id='{id}'", (c, u) =>
            {
                var result = mapperConf.Map<StaffViewModel>(u);
                result = mapperConf.Map(c, result);
                return result;
            }, transaction: Transaction,
                    splitOn: "Id").FirstOrDefault();
            return customer;
        }


        public IEnumerable<StaffViewModel> All()
        {
            var mapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<ApplicationUser, StaffViewModel>();
                cfg.CreateMap<Staff, StaffViewModel>();
            });
            var mapperConf = mapper.CreateMapper();
            var customers = Connection.Query<Staff, ApplicationUser, StaffViewModel>(@"
SELECT c.*, u.* FROM Staff c
INNER JOIN AspNetUsers u ON c.Id = u.Id", (c, u) =>
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
