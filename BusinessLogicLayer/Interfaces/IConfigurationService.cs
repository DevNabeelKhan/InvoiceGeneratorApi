using BusinessObjectsLayer.Entities;

namespace BusinessLogicLayer.Interfaces
{
    public interface IConfigurationService
    {
        Task<dynamic> GetConfiguration(string TableName, int? Id, string? SearchText, bool? IsActive, int? PageNumber, int? PageSize);
        Task<dynamic> InsertUpdateConfiguration(ConfigurationModel model);
        Task<dynamic> DeleteConfiguration(string TableName, int? Id);
    }
}
