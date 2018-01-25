using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TourOfHeroesBackend.Models;

namespace TourOfHeroesBackend
{
    public class Startup
    {
        public Startup( IHostingEnvironment env )
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath( env.ContentRootPath )
                .AddJsonFile( "appsettings.json", optional: false, reloadOnChange: true )
                .AddJsonFile( $"appsettings.{env.EnvironmentName}.json", optional: true )
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }


        //den exw idea thn diafora alla auto kanei xrisi tou Configuration
        //public Startup( IConfiguration configuration )
        //{
        //    Configuration = configuration;
        //}

        //public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices( IServiceCollection services )
        {

            services.Configure<Jwt>( options => Configuration.GetSection( "Jwt" ).Bind( options ) );


            //Make the whole app private and in need of authorization
            //services.AddMvc( config =>
            //{
            //    var policy = new AuthorizationPolicyBuilder()
            //                     .RequireAuthenticatedUser()
            //                     .Build();
            //    config.Filters.Add( new AuthorizeFilter( policy ) );
            //} );

            //add JWT authorization
            services.AddAuthentication( JwtBearerDefaults.AuthenticationScheme )
             .AddJwtBearer( options =>
              {
                  options.TokenValidationParameters = new TokenValidationParameters
                  {
                      ValidateIssuer = true,
                      ValidateAudience = false,
                      ValidateLifetime = true,
                      ValidateIssuerSigningKey = true,
                      ValidIssuer = Configuration.GetValue<string>("Jwt:Issuer"),
                      ValidAudience = Configuration.GetValue<string>("Jwt:Audience"),
                      IssuerSigningKey = new SymmetricSecurityKey( Encoding.UTF8.GetBytes( Configuration.GetValue<string>("Jwt:Key") ) )
                  };
              } );

            services.AddMvc();

            //connect backend to frontend with Angular
            services.AddCors( options =>
             {
                 options.AddPolicy( "AllowAll",
                         builder =>
                         {
                             builder.WithOrigins( "http://localhost:4200" )
                                    .AllowAnyMethod()
                                    .AllowAnyHeader()
                                    .AllowCredentials();
                         } );
             } );

            //adding policies to make entities like "admin", "guest" etc
            //services.AddAuthorization( options =>
            //{
            //    options.AddPolicy( "AuthorizedUser",
            //                      policy => policy.RequireClaim( "AuthorizedLogger", "IAmAuthorized" ) );
            //} );

            services
                .AddMvcCore()
                .AddJsonFormatters()
                .AddAuthorization();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure( IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory )
        {
            loggerFactory.AddConsole( Configuration.GetSection( "Logging" ) );
            loggerFactory.AddDebug();
            app.UseCors( "AllowAll" );

            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
