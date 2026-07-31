using BusinessObjectsLayer.Entities;

namespace DataAccessLayer.Interface
{
    public interface IConfigurationRepository
    {
        Task<dynamic> GetConfiguration(string TableName, int? Id, string? SearchText, bool? IsActive, int? PageNumber = 1, int? PageSize = 20);
        Task<dynamic> InsertUpdateConfiguration(ConfigurationModel model);
        Task<dynamic> DeleteConfiguration(string TableName, int? Id);
    }
}
