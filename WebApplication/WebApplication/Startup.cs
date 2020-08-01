using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebApplication.AuthorizationRequirements;
using WebApplication.Controllers;
using WebApplication.CustomPolicyProvider;
using WebApplication.Transformer;

namespace WebApplication
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication("CookieAuth").AddCookie("CookieAuth", config => {
                   config.Cookie.Name = "Grandmas.Cookie";
                   config.LoginPath = "/Home/Authenticate";
            });

            services.AddAuthorization(config =>
            {
                //var defaultAuthBuilder = new AuthorizationPolicyBuilder();
                //var defaultAuthPolicy = defaultAuthBuilder
                //    .RequireAuthenticatedUser()
                //    .RequireClaim(ClaimTypes.DateOfBirth)
                //    .Build();
                //config.DefaultPolicy = defaultAuthPolicy;


                // config.AddPolicy("Claim.DoB", policyBuilder =>
                // {
                //    policyBuilder.RequireClaim(ClaimTypes.DateOfBirth);
                // });


               // config.AddPolicy("Claim.DoB", policyBuilder => {
                //    policyBuilder.AddRequirements(new CustomRequireClaim(ClaimTypes.DateOfBirth));
               // });

                config.AddPolicy("Claim.DoB", policyBuilder => { policyBuilder.RequireCustomClaim(ClaimTypes.DateOfBirth);});
                config.AddPolicy("Admin", policyBuilder => policyBuilder.RequireClaim(ClaimTypes.Role, "Admin"));
            });

            services.AddScoped<IAuthorizationHandler, CustomRequireClaimHandler>();
            services.AddSingleton<IAuthorizationPolicyProvider, CustomAuthorizationPolicyProvider>();
            services.AddScoped<IAuthorizationHandler, SecurityLevelHandler>();
            services.AddScoped<IAuthorizationHandler, CookieJarAuthorizationHandler>();
            services.AddScoped<IClaimsTransformation, ClaimsTransformation>(); 

            services.AddControllersWithViews(config =>
              {
                  var defaultAuthBuilder = new AuthorizationPolicyBuilder();
                  var defaultAuthPolicy = defaultAuthBuilder
                      .RequireAuthenticatedUser()
                      .Build();

                  // global authorization filter
                  //config.Filters.Add(new AuthorizeFilter(defaultAuthPolicy));
              }); 

           services.AddRazorPages().AddRazorPagesOptions(config => {
                   config.Conventions.AuthorizePage("/Razor/Secured");
                   config.Conventions.AuthorizePage("/Razor/Policy", "Admin");
                   config.Conventions.AuthorizeFolder("/RazorSecured");
                   config.Conventions.AllowAnonymousToPage("/RazorSecured/Anon");});
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapRazorPages();
            });
        }
    }
}
