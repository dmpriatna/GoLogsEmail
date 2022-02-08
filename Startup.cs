using AutoMapper;
using GoLogs.Api.Application.Internals;
using GoLogs.Api.BusinessLogic.Commands;
using GoLogs.Api.BusinessLogic.Handler;
using GoLogs.Api.BusinessLogic.Interfaces;
using GoLogs.Api.Constants;
using GoLogs.Api.Models;
using GoLogs.Api.ViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Text;

namespace GoLogs.Api
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
            // JWT Bearer
            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = false,
                        ValidateIssuerSigningKey = true,
                        //ValidIssuer = "http://localhost:50000",
                        //ValidAudience = "http://localhost:50000",
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Constant.GoLogsAuthKey))
                    };
                });

            // Add CORS policy
            //services.AddCors(c =>
            //{
            //    c.AddPolicy("AllowAll",
            //    options =>
            //    {
            //        // Not a permanent solution, but just trying to isolate the problem
            //        options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
            //    });

            //    //c.AddPolicy("AllowAll", options => options.WithOrigins("http://localhost:52884").AllowAnyMethod().AllowAnyHeader());
            //});
            services.AddCors();

            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "GoLogs.Api", Version = "v1" });

                // Include 'SecurityScheme' to use JWT Authentication
                var jwtSecurityScheme = new OpenApiSecurityScheme
                {
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    Name = "JWT Authentication",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",

                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };

                c.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { jwtSecurityScheme, Array.Empty<string>() }
                });
            });

            // Postgre EF
            services.AddEntityFrameworkNpgsql().AddDbContext<GoLogsContext>(opt =>
            opt.UseNpgsql(Configuration.GetConnectionString("GoLogsDb")));

            services
                .AddScoped<IDeliveryOrderLogic, DeliveryOrderLogic>()
                .AddScoped<IEmailLogic, EmailLogic>()
                .AddScoped<IEmailTemplateLogic, EmailTemplateLogic>()
                .AddScoped<INLELogic, NLELogic>()
                .AddScoped<INotifyLogic, NotifyLogic>()
                .AddScoped<INotifyTemplateLogic, NotifyTemplateLogic>()
                .AddScoped<IPersonLogic, PersonLogic>()
                .AddAutoMapper(typeof(Startup));

            // HttpContext to get Claim from JWT Token
            services.AddHttpContextAccessor();
            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();

            // Mapper Config
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<DeliveryOrderModel, DeliveryOrderViewModel>();
                cfg.CreateMap<EmailTemplateCommand, EmailTemplateModel>();
                cfg.CreateMap<EmailTemplateModel, EmailTemplateViewModel>();
                cfg.CreateMap<NLECustDataModel, NLECustDataViewModel>();
                cfg.CreateMap<NotifyTemplateModel, NotifyTemplateViewModel>();
                cfg.CreateMap<PersonModel, PersonViewModel>();
            });

            IMapper mapper = mapperConfig.CreateMapper();

            services.AddSingleton(mapper);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "GoLogs.Api v1"));
            }
            else
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("v1/swagger.json", "GoLogs.Api v1");
                });
            }

            app.UseHttpsRedirection();

            //var supportedCultures = new[] { "id-ID", "id" };
            //var localizationOptions = new RequestLocalizationOptions().SetDefaultCulture(supportedCultures[0])
            //    .AddSupportedCultures(supportedCultures)
            //    .AddSupportedUICultures(supportedCultures);

            //app.UseRequestLocalization(localizationOptions);

            app.UseRouting();

            //app.UseCors("AllowAll");
            app.UseCors(c => c.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
    }
}
