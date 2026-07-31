using BusinessLogicLayer.Interfaces;
using BusinessObjectsLayer.Entities;
using DataAccessLayer.Interface;

namespace BusinessLogicLayer.Service
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly IConfigurationRepository _configurationRepository;

        public ConfigurationService(IConfigurationRepository configurationRepository)
        {
            _configurationRepository = configurationRepository;
        }

        public async Task<dynamic> GetConfiguration(string TableName, int? Id, string? SearchText, bool? IsActive, int? PageNumber, int? PageSize)
        {
            var res = await _configurationRepository.GetConfiguration(TableName, Id, SearchText, IsActive, PageNumber, PageSize);
            return res;
        }

        public async Task<dynamic> InsertUpdateConfiguration(ConfigurationModel model)
        {
            var res = await _configurationRepository.InsertUpdateConfiguration(model);
            return res;
        }

        public async Task<dynamic> DeleteConfiguration(string TableName, int? Id)
        {
            var res = await _configurationRepository.DeleteConfiguration(TableName, Id);
            return res;
        }
    }
}
