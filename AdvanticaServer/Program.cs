using AdvanticaServer.Services;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace AdvanticaServer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Additional configuration is required to successfully run gRPC on macOS.
            // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

            // Add services to the container.
            builder.Services.AddGrpc();

            builder.Services.AddDbContext<AdvanticaContext>(op =>
            {
                op.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
                op.LogTo(s =>
                {
                    Debug.WriteLine(s);
                    using TextWriter writer = File.AppendText("app_log.txt");
                    writer.WriteLine(s);
                });
            });

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AdvanticaContext>();

                await context.Database.EnsureCreatedAsync();
            }

            // Configure the HTTP request pipeline.
            app.MapGrpcService<WorkerStreamService>();
            app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

            app.Run();
        }
    }
}