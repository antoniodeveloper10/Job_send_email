using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Job_send_emails
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            return base.StartAsync(cancellationToken);
        }
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }

        public Worker(ILogger<Worker> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                // We ultimately resolve the actual services we use from the scope we create below.
                // This ensures that all services that were registered with services.AddScoped<T>()
                // will be disposed at the end of the service scope (the current iteration).
                using var scope = _serviceScopeFactory.CreateScope();

                var sendGridClient = scope.ServiceProvider.GetRequiredService<ISendGridClient>();
                var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

                var message = new SendGridMessage
                {
                    Subject = "Aviso",
                    PlainTextContent = "bom dia, seu certificado esta vencido!",
                    From = new EmailAddress(configuration["Email:From"]),

                };

                message.AddTo(configuration["Email:Recipient"]);

                _logger.LogInformation($"Sending message to {configuration["Email:Recipient"]}: {message.PlainTextContent}");

                await sendGridClient.SendEmailAsync(message, cancellationToken);

                await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
            }
        }
    }
}
