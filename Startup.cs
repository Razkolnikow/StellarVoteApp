using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StellarVoteApp.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StellarVoteApp.Data.Models;
using StellarVoteApp.Data.Models.Contracts;
using Microsoft.Extensions.Options;
using AspNetCore.Identity.Mongo;
using AspNetCore.Identity.Mongo.Model;

namespace StellarVoteApp
{
    public class Startup
    {
        private string ConnectionString => Configuration
            .GetSection("StellarVoteDatabaseSettings")
            .GetSection("ConnectionString")
            .Value;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            //services.AddDbContext<ApplicationDbContext>(options =>
            //    options.UseSqlServer(
            //        Configuration.GetConnectionString("DefaultConnection")));
            //services.AddDefaultIdentity<IdentityUser>()
            //    .AddEntityFrameworkStores<ApplicationDbContext>();

            services.Configure<StellarVoteDatabaseSettings>(
                    Configuration.GetSection(nameof(StellarVoteDatabaseSettings)));

            services.AddSingleton<IStellarVoteDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<StellarVoteDatabaseSettings>>().Value);

            services.AddIdentityMongoDbProvider<Data.Models.StellarVoteUser, MongoRole>(identityOptions => {
                identityOptions.Password.RequiredLength = 6;
            }, mongo =>
            {
                mongo.ConnectionString = ConnectionString;
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

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
