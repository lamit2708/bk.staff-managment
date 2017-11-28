using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BK.StaffManagement.Data;
using BK.StaffManagement.Models;
using BK.StaffManagement.Services;
using System.Data;
using System.Data.SqlClient;
using BK.StaffManagement.Repositories;

namespace BK.StaffManagement
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();
            var repoInterfaceType = typeof(IRepository<>);
            var repoTypes = repoInterfaceType
                .Assembly
                .DefinedTypes
                .Where(t =>
                    !t.IsInterface
                    && !t.IsAbstract
                    && (
                        t.ImplementedInterfaces.Contains(repoInterfaceType)
                        || t.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == repoInterfaceType)
                    ));
            foreach (var repoTypeInfo in repoTypes)
            {
                var repoType = repoTypeInfo.AsType();
                services.AddScoped(repoType, repoType);
            }
            services.AddScoped<IDbConnection>(p =>
            {
                var configuration = p.GetService<IConfiguration>();
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                var conn = new SqlConnection(connectionString);
                conn.Open();
                return conn;
            });
            services.AddScoped(p =>
            {
                var conn = p.GetService<IDbConnection>();
                return conn.BeginTransaction();
            });
            services.AddMvc();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
