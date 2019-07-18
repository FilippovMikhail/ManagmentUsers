using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using WebAPI.Entities;
using WebAPI.Models;

namespace WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("Connection");
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

            services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            //Настройка валидации пароля и email
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireNonAlphanumeric = false; //Требуются ли не алфавитно-цифровые символы
                options.Password.RequireLowercase = false; //Требуются ли символы в нижнем регистре
                options.Password.RequireUppercase = false; //Требуются ли символы в верхнем регистре
                options.Password.RequireDigit = false; //Требуются ли цифры
                options.Password.RequiredLength = 6; //Минимальная длина пароль
                options.User.RequireUniqueEmail = true; //Требуется ли чтобы email был уникальным
            });

            /*Включаем CORS, который позволяет использовать наш веб-API с других сайтов, чем тот, на котором размещен веб-API.*/
            services.AddCors();

            //Jwt Authentication

            //Ключ для проверки подписи
            var key = Configuration.GetValue<string>("Tokens:Key");
            var siginingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = siginingKey,

                    ValidateIssuer = true,
                    ValidIssuer = Configuration["Tokens:Issuer"],

                    ValidateAudience = true,
                    ValidAudience = Configuration["Tokens:Audience"],
                };
            });


            //Изменение JSON объектов. В JSON имена переменных начинаются с маленькой буквы, тогда как в модели они большие.
            services.AddMvc().AddJsonOptions(options=> {
                var castedResolver = options.SerializerSettings.ContractResolver as DefaultContractResolver;
                if (castedResolver!=null)
                {
                    castedResolver.NamingStrategy = null;
                }
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, AppDbContext context, UserManager<User> userManager, RoleManager<Role> roleManager, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            loggerFactory.AddFile(Configuration.GetSection("Logging"));

            app.UseAuthentication();

            DbInitializer.Initialze(context, userManager, roleManager);

            app.UseCors(c => c
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());

            app.UseMvc();
        }
    }
}
