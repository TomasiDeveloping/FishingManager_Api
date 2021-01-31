using Api.Data;
using Api.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Api.Helper
{
    public class DatabaseLogger
    {
        private readonly IConfiguration _configuration;

        public DatabaseLogger(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async void InsertDatabaseLog(DataBaseLog log)
        {
            var options = new DbContextOptionsBuilder<FishingManagerContext>()
                .UseSqlServer(new SqlConnection(_configuration.GetConnectionString("FishingManager")))
                .Options;
            var context = new FishingManagerContext(options);

            await context.DataBaseLogs.AddAsync(log);
            await context.SaveChangesAsync();
        }
    }
}