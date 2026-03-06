

namespace ECommerceWeb.Extentions
{
    public static class WebApplicationRegistration
    {
        public static async Task<WebApplication> MigrateDatabaseAsync(this WebApplication app)
        {
            await using var scope =  app.Services.CreateAsyncScope();
            var dbcontextService = scope.ServiceProvider.GetRequiredService<StoreDbContext>();
            var PendingmMigration = await dbcontextService.Database.GetPendingMigrationsAsync();
            if (PendingmMigration.Any())
            {
                await dbcontextService.Database.MigrateAsync();
            }
           return app;
        }

        public static async Task<WebApplication> SeedDataAsync(this WebApplication app)
        {
            await using var scope =  app.Services.CreateAsyncScope();
            var dataInitializer = scope.ServiceProvider.GetRequiredKeyedService<IDataInitializer>("Default");
            await dataInitializer.InitializeAsync();
            return app;
        }
        public static async Task<WebApplication> MigrateIdentityDatabaseAsync(this WebApplication app)
        {
            await using var scope = app.Services.CreateAsyncScope();
            var dbcontextService = scope.ServiceProvider.GetRequiredService<StoreIdentityDbContext>();
            var PendingmMigration = await dbcontextService.Database.GetPendingMigrationsAsync();
            if (PendingmMigration.Any())
            {
                await dbcontextService.Database.MigrateAsync();
            }
            return app;
        }
        public static async Task<WebApplication> SeedIdentityDataAsync(this WebApplication app)
        {
            await using var scope = app.Services.CreateAsyncScope();
            var dataInitializer = scope.ServiceProvider.GetRequiredKeyedService<IDataInitializer>("Identity");
            await dataInitializer.InitializeAsync();
            return app;
        }


    }
}
