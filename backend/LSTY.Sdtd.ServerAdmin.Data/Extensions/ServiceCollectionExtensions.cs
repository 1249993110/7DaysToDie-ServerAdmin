using LSTY.Sdtd.ServerAdmin.Data.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Raven.Client.Documents;
using Raven.Client.ServerWide.Operations;
using Raven.Client.ServerWide;
using Raven.Client.Exceptions;
using System.Xml.Linq;

namespace LSTY.Sdtd.ServerAdmin.Data.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds a Raven <see cref="IDocumentStore"/> singleton to the dependency injection services.
        /// The document store is configured based on the <see cref="RavenOptions"/> action configuration.
        /// </summary>
        /// <param name="services">The dependency injection services.</param>
        /// <param name="configure">The configuration for the <see cref="RavenOptions"/></param>
        /// <returns>The dependency injection services.</returns>
        public static IServiceCollection AddRavenDb(this IServiceCollection services, Action<RavenOptions> configure)
        {
            services.AddOptions<RavenOptions>().Configure(configure);
            services.InternalAdd();
            return services;
        }

        /// <summary>
        /// Adds a Raven <see cref="IDocumentStore"/> singleton to the dependency injection services.
        /// The document store is configured based on the <see cref="RavenOptions"/> action configuration.
        /// </summary>
        /// <param name="services">The dependency injection services</param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddRavenDb(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<RavenOptions>().Bind(configuration);
            services.InternalAdd();
            return services;
        }

        /// <summary>
        /// Adds a Raven <see cref="IDocumentStore"/> singleton to the dependency injection services.
        /// The document store is configured based on the <see cref="RavenOptions"/> action configuration.
        /// </summary>
        /// <param name="services">The dependency injection services</param>
        /// <param name="configurationSectionPath"></param>
        /// <returns></returns>
        public static IServiceCollection AddRavenDb(this IServiceCollection services, string configurationSectionPath)
        {
            services.AddOptions<RavenOptions>().BindConfiguration(configurationSectionPath);
            services.InternalAdd();
            return services;
        }

        private static void InternalAdd(this IServiceCollection services)
        {
            services.AddSingleton<IDocumentStore>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<RavenOptions>>().Value;
                var documentStore = new DocumentStore()
                {
                    Urls = options.Urls,
                    Database = options.DatabaseName
                };

                //if (options.Certificate != null)
                //{
                //    documentStore.Certificate = options.Certificate;
                //}

                documentStore.Initialize();

                var dbServer = documentStore.Maintenance.Server;
                var result = dbServer.Send(new GetDatabaseRecordOperation(options.DatabaseName));
                if (result == null)
                {
                    try
                    {
                        dbServer.Send(new CreateDatabaseOperation(new DatabaseRecord(options.DatabaseName)));
                    }
                    catch (ConcurrencyException)
                    {
                        // The database was already created before calling CreateDatabaseOperation
                    }
                }
                return documentStore;
            });

            services.AddScoped(sp => sp.GetRequiredService<IDocumentStore>().OpenSession());
            services.AddScoped(sp => sp.GetRequiredService<IDocumentStore>().OpenAsyncSession());
        }
    }
}
