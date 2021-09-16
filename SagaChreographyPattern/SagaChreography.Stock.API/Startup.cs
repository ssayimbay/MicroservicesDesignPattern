using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using SagaChreography.Shared.Settings;
using SagaChreography.Stock.API.Consumers;
using SagaChreography.Stock.API.Models.Contexts;

namespace SagaChreography.Stock.API
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

            //RabbitMQ
            services.AddMassTransit(x =>
            {
                x.AddConsumer<OrderCreatedEventConsumer>();
                x.AddConsumer<PaymentFailEventConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(Configuration.GetConnectionString("RabbitMQ"));
                    cfg.ReceiveEndpoint(RabbitMQSettingsConst.StockOrderCreatedEventQueueName, e =>
                    {
                        e.ConfigureConsumer<OrderCreatedEventConsumer>(context);
                    });

                    cfg.ReceiveEndpoint(RabbitMQSettingsConst.StockPaymentFailedEventQueueName, e =>
                    {
                        e.ConfigureConsumer<PaymentFailEventConsumer>(context);
                    });
                });

            });
            services.AddMassTransitHostedService();

            //InMemoryDb
            services.AddDbContext<StockDbContext>(options =>
            {
                options.UseInMemoryDatabase("StockDb");
            });

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SagaChreography.Stock.API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SagaChreography.Stock.API v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}