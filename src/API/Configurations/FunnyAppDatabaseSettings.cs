using Microsoft.Extensions.Configuration;

namespace API.Configurations
{
    public class FunnyAppDatabaseSettings
    {
        private readonly IConfiguration _config;

        public FunnyAppDatabaseSettings()
        {
            ConnectionString = _config.GetSection("mongodb://localhost:27017").ToString();
            DatabaseName = _config.GetSection("DatabaseName").ToString();
        }

        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string UsersCollectionName { get; set; } = "Users";
        public string ApointmentsCollectionName { get; set; } = "Apointments";
        public string CustomersCollectionName { get; set; } = "Customers";
    }
}
