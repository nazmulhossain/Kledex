﻿using Kledex.Bus.ServiceBus.Extensions;
using Kledex.Examples.Domain.Commands;
using Kledex.Examples.Reporting.Queries;
using Kledex.Examples.Shared;
using Kledex.Extensions;
using Kledex.Store.Cosmos.Sql.Configuration;
using Kledex.Store.Cosmos.Sql.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Kledex.Examples.Web.Cosmos.Sql
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.AddHttpContextAccessor();

            services
                .AddKledex(typeof(CreateProduct), typeof(GetProduct))
                .AddCosmosDbSqlProvider(Configuration)
                .AddServiceBusProvider(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IDispatcher dispatcher, IOptions<DomainDbConfiguration> settings)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseKledex().EnsureCosmosDbSqlDbCreated(settings);

            // Create a sample product loading data from domain events.
            var product = GettingStarted.CreateProduct(dispatcher).GetAwaiter().GetResult();

            app.Run(async context =>
            {
                // Display product title.
                await context.Response.WriteAsync($"Product title: {product.Title}");
            });
        }
    }
}
