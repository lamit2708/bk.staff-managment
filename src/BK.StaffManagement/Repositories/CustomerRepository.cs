﻿using AutoMapper;
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
   
    public class CustomerRepository : BaseRepository<Customer>
    {
        public CustomerRepository(IDbConnection conn, IDbTransaction trans) : base(conn, trans)
        {
           
        }
        //public EditCustomerViewModel Get(string id)
        public CustomerViewModel Get(string id)
        {
            //var mapper = new MapperConfiguration(cfg => {
            //    cfg.CreateMap<ApplicationUser, EditCustomerViewModel>();
            //    cfg.CreateMap<Customer, EditCustomerViewModel>();
            //});
            var mapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<ApplicationUser, CustomerViewModel>();
                cfg.CreateMap<Customer, CustomerViewModel>();
            });
            var mapperConf = mapper.CreateMapper();
            //var customer = Connection.Query<Customer, ApplicationUser, EditCustomerViewModel>($@"
            var customer = Connection.Query<Customer, ApplicationUser, CustomerViewModel>($@"
SELECT c.*, u.* FROM Customer c
INNER JOIN AspNetUsers u ON c.Id = u.Id
WHERE c.Id='{id}'", (c, u) =>
            {
                //var result = mapperConf.Map<EditCustomerViewModel>(u);
                var result = mapperConf.Map<CustomerViewModel>(u);
                result = mapperConf.Map(c, result);
                return result;
            }, transaction: Transaction,
                    splitOn: "Id").FirstOrDefault();
            return customer;
        }
        public CustomerViewModel GetByUsername(string username)
        {
            //var mapper = new MapperConfiguration(cfg => {
            //    cfg.CreateMap<ApplicationUser, EditCustomerViewModel>();
            //    cfg.CreateMap<Customer, EditCustomerViewModel>();
            //});
            var mapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<ApplicationUser, CustomerViewModel>();
                cfg.CreateMap<Customer, CustomerViewModel>();
            });
            var mapperConf = mapper.CreateMapper();
            //var customer = Connection.Query<Customer, ApplicationUser, EditCustomerViewModel>($@"
            var customer = Connection.Query<Customer, ApplicationUser, CustomerViewModel>($@"
SELECT c.*, u.* FROM Customer c
INNER JOIN AspNetUsers u ON c.Id = u.Id
WHERE u.UserName='{username}'", (c, u) =>
            {
                //var result = mapperConf.Map<EditCustomerViewModel>(u);
                var result = mapperConf.Map<CustomerViewModel>(u);
                result = mapperConf.Map(c, result);
                return result;
            }, transaction: Transaction,
                    splitOn: "Id").FirstOrDefault();
            return customer;
        }

        public IEnumerable<CustomerViewModel> All()
        {
            var mapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<ApplicationUser, CustomerViewModel>();
                cfg.CreateMap<Customer, CustomerViewModel>();
            });
            var mapperConf = mapper.CreateMapper();
            var customers = Connection.Query<Customer, ApplicationUser, CustomerViewModel>(@"
SELECT c.*, u.* FROM Customer c
INNER JOIN AspNetUsers u ON c.Id = u.Id", (c, u) =>
            {
                var result = mapperConf.Map<CustomerViewModel>(u);
                result = mapperConf.Map(c, result);
                return result;
            }, transaction: Transaction,
                    splitOn: "Id");
            return customers;
        }

        public IEnumerable<CustomerViewModel> AllByStaffId(string staffId)
        {
            var mapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<ApplicationUser, CustomerViewModel>();
                cfg.CreateMap<Customer, CustomerViewModel>();
            });
            var mapperConf = mapper.CreateMapper();
            var customers = Connection.Query<Customer, ApplicationUser, CustomerViewModel>($@"
SELECT c.*, u.* FROM Customer c
INNER JOIN AspNetUsers u ON c.Id = u.Id
WHERE c.StaffId = '{staffId}'", (c, u) =>
            {
                var result = mapperConf.Map<CustomerViewModel>(u);
                result = mapperConf.Map(c, result);
                return result;
            }, transaction: Transaction,
                    splitOn: "Id");
            return customers;
        }

        public int Count(string search)
        {
            var searchCondition = !string.IsNullOrWhiteSpace(search)
                ? $"WHERE u.FirstName LIKE '%{search}%' OR u.LastName LIKE '%{search}%'"
                : string.Empty;
            var count = Connection.Query<int>($@"
SELECT COUNT(c.Id) FROM Customer c
INNER JOIN AspNetUsers u ON c.Id = u.Id
{searchCondition}
", transaction: Transaction).FirstOrDefault();
            return count;
        }
        public int GetSumDebit()
        {
           
            var count = Connection.Query<int>($@"
SELECT SUM(c.DebitBalance) FROM Customer c
", transaction: Transaction).FirstOrDefault();
            return count;
        }
    }
}
