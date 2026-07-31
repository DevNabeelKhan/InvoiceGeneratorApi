
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace DataAccess.DbContext
{
    public class DapperContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private readonly string _connectionStringUsptoFile;

        public DapperContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("InvoiceProjectStr");
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
        public IDbConnection CreateConnectionUsptoFile()
        {
            return new SqlConnection(_connectionStringUsptoFile);
        }
    }
}
