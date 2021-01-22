using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SendGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Job_send_emails
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {          
                    services.AddScoped<ISendGridClient>(s => new SendGridClient(new SendGridClientOptions
                    {
                        ApiKey = "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"
                    }));
                    services.AddHostedService<Worker>();
                });
    }
}
