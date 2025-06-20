using System;
using DatabaseConnectionTutorial.Services;
using DatabaseConnectionTutorial.Utility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DatabaseConnectionTutorial
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // 1) create the builder (has DI, logging, config, etc.)
            var builder = Host.CreateApplicationBuilder(args);

            // 2) register services
            builder.Services
                // set up the connection‐factory
                .AddSingleton<IConnectionFactory>(_ => new DatabaseConfig("tutorial"))
                // open‐generic CRUD impl
                .AddTransient(typeof(IService<>), typeof(GenericCrudService<>))
                // generic console menu
                .AddTransient(typeof(GenericMenu<>));

            // 3) build the host
            var app = builder.Build();

            // 4) resolve & run the User menu
            var userMenu = app.Services.GetRequiredService<GenericMenu<User>>();
            userMenu.Show();
        }
    }
}

