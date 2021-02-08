using System;
using Core;
using Core.Interfaces;
using Core.Interfaces.Mails;
using Core.Settings;
using Domain;
using Infrastructure.Mails;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Application
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File("logs/scheduleAppLog.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            try
            {
                Log.Information("Starting up");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    var conf = hostContext.Configuration;
                    var emailSettings = conf.GetSection(EmailSettings.Name).Get<EmailSettings>();
                    
                    services
                        .Configure<CsvFileSettings>(conf.GetSection(CsvFileSettings.Name))
                        .Configure<EmailSettings>(conf.GetSection(EmailSettings.Name))
                        .AddScoped<ICsvParser, CsvParser>()
                        .AddScoped<IMailBuilder, MailBuilder>()
                        .AddHostedService<AkkaBuilderService>()
                        .AddFluentEmail(emailSettings.From)
                        .AddSmtpSender(emailSettings.Host, emailSettings.Port, emailSettings.Username, emailSettings.Password)
                        .AddRazorRenderer();
                });
    }
}