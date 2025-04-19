#pragma warning disable 

using DAL2;
using Microsoft.EntityFrameworkCore;

namespace DAL2.Migrations
{
    public static class MigrationInitializer
    {
        public static void ApplyMigrations()
        {
            using var context = new AppDbContext();
            context.Database.Migrate(); // Автоматично застосовує всі доступні міграції
        }
    }
}

