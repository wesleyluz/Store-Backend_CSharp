using EvolveDb;
using lojinha_backend.Model.Context;
using lojinha_backend.Repository.Implementation;
using lojinha_backend.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;
using System;
using System.Collections.Generic;
using lojinha_backend.Business;
using lojinha_backend.Business.Implementation;
using lojinha_backend.Services;
using lojinha_backend.Services.Implementations;
using lojinha_backend.Config;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace lojinha_backend
{
    public class Startup
    { 
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }
       
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

       

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
                                //Validação//
            // Cria a instancia das configurações do token 
            var tokenConfigurations = new TokenConfiguration();

            //Seta os valores da instancia a partir das informações no appsettings
            new ConfigureFromConfigurationOptions<TokenConfiguration>(
                    Configuration.GetSection("TokenConfiguration")
                )
                .Configure(tokenConfigurations);

            // Injeta a instancia unica da configuração.
            services.AddSingleton(tokenConfigurations);

            // Adiciona as autenticações
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {   
                // seta os valores de validação do token de acordo com as configurações 
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = tokenConfigurations.Issuer,
                    ValidAudience = tokenConfigurations.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenConfigurations.Secret))
                };
            });
                                //Autorização
            // Politica de autorização para acessar a aplicação
            services.AddAuthorization(auth => 
            {
                auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser().Build());
            });

                                //Cross Orign
            //Permite que qualquer origem, metodo ou header possa acessar essa api
            services.AddCors(options => options.AddDefaultPolicy(builder =>
            {
                builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();

            }));

            services.AddControllers();
            //String de conexão do banco no appsettings
            var connection = Configuration["MSSQLConnection:MSSQLServerConnectionString"];
            services.AddDbContext<MSSQLContext>(options => options.UseSqlServer(connection));

                                //Migrations
            // Esse trecho de código invoca as migrations utilizando o Evolve, para utilizar localmente altere a string de conexão do banco 
            // crie uma tabela com o nome Loja e descomente o if.
            
            //if (Environment.IsDevelopment())
            //{
            //    MigrateDataBase(connection);
            //}

                                //Swagger
            services.AddSwaggerGen(c =>
            {   
                //Informações da api que irão ser mostradas no Swagger UI
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Lojinha",
                    Version = "v1",
                    Contact = new OpenApiContact
                    {
                        Name = "Wesley Luz",
                        Url = new Uri("https://github.com/wesleyluz")
                    }
                });
                //Adicionando opções para autenticar e consumir a api diretamente pelo swagger
                //Define as variaveis de segurança que serão mostradas na UI
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Coloque o access Token aqui no seguinte formar Bearer[espaço]Token",

                });
                //Solicita e armazena o token
                c.AddSecurityRequirement(new OpenApiSecurityRequirement 
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[]{}
                    }
                });
            });
            //Injeções de dependeica do repositorios e business 
            services.AddScoped<IRepository, RepositoryImp>()
                    .AddScoped<IProdutoBusiness, ProdutoBusinessImp>()
                    .AddScoped<ILoginBusiness, LoginBusiness>()
                    .AddScoped<IUserRepository,UserRepositoryImp>();
            
            //Injeção de dependecia do TokenService
            services.AddTransient<ITokenService, TokenService>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
           

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors();


            //Swagger 
            app.UseDeveloperExceptionPage();
            //Ativando swagger
            app.UseSwagger();
            
            if (env.IsDevelopment())
            {   //Endpoint para uso da UI
                app.UseSwaggerUI(c =>
                {   
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "lojinha_backend v1");
                    c.RoutePrefix = "swagger";
                });
            }

            

            var option = new RewriteOptions();
            app.UseRewriter(option);
            //Redirecionando para pagina do swagger
            option.AddRedirect("^$", "swagger");

            app.UseAuthorization(); 
            //End-points da API
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute("DefaultApi","{controller=values}");
            });
        }

        
        /// <summary>
        /// Função para a midrations utilizando o  Evolve,
        /// recebe a string de conexão, abre uma nova conexão sql e roda os scripts nas subpastas do arquivo "db"
        /// </summary>
        /// <param name="connection"></param>
        private void MigrateDataBase(string connection)
        {
            try
            {
                var evolveConnection = new SqlConnection(connection);
                var evolve = new Evolve(evolveConnection, msg => Log.Information(msg))
                {
                    Locations = new List<string> { "db/migrations", "db/dataset" },
                    IsEraseDisabled = true,
                };
                evolve.Migrate();
            }
            catch (Exception ex)
            {
                Log.Error("DataBase Migration Falied", ex);
                throw;
            }
        }
    }
}
