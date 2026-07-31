using BusinessObjectsLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interface
{
    public interface IBaseRepository
    {
        Task<T> Get<T>(T entity, int id);
        Task<int> GetTotalCount<T>(T entity);
        Task<IEnumerable<T>> GetAll<T>(T entity);
        Task<IEnumerable<T>> GetAll<T>(T entity, int offset, int fetch);
        Task<int> Save<T>(T entity, bool audits = true, bool delete = true);
        Task Update<T>(T entity, bool audits = true, bool delete = true);
        Task Delete<T>(T entity, int id);
        Task DeletesByColumn<T>(T entity, int id, string column);
        Task DeletePerminent<T>(T entity, int id);
    }
}
