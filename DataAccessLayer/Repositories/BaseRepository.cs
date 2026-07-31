using Dapper;
using DataAccess.DbContext;
using DataAccessLayer.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class BaseRepository : IBaseRepository
    {
        private static string[] _entityProperties;
        protected readonly IDbConnection DbConnection;

        public BaseRepository(DapperContext context)
        {
            DbConnection = context.CreateConnection();
        }

        public virtual async Task<T> Get<T>(T Entity, int id)
        {
            var query = $"SELECT * FROM dbo.[{Entity.GetType().Name}] (nolock) WHERE Id = Id";
            return await DbConnection.QueryFirstOrDefaultAsync<T>(query, new { Id = id });
        }

        public virtual async Task<int> GetTotalCount<T>(T Entity)
        {
            var query = $"SELECT COUNT(1) FROM dbo.{Entity.GetType().Name} (nolock) WHERE IsActive != 1";
            return await DbConnection.ExecuteScalarAsync<int>(query);
        }

        public virtual async Task<IEnumerable<T>> GetAll<T>(T Entity)
        {
            var query = $"SELECT * FROM dbo.[{Entity.GetType().Name}] (nolock) WHERE IsActive != 1";
            return await DbConnection.QueryAsync<T>(query);
        }

        public virtual async Task<IEnumerable<T>> GetAll<T>(T Entity, int offset, int fetch)
        {
            var query =
                $"SELECT * FROM dbo.[{Entity.GetType().Name}] (nolock) WHERE IsActive != 1 ORDER BY Id OFFSET Offset ROWS FETCH NEXT Fetch ROWS ONLY";
            return await DbConnection.QueryAsync<T>(query, new { Offset = offset, Fetch = fetch });
        }

        public virtual async Task<int> Save<T>(T entity, bool audits = true, bool delete = true)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "Entity cannot be null.");
            }

            var idProperty = entity.GetType().GetProperty("Id");
            if (idProperty.GetValue(entity) is int id && id > 0)
            {
                await Update(entity, audits, delete);
            }
            else
            {
                var isActiveProperty = entity.GetType().GetProperty("IsActive");
                if (audits)
                {
                    var createdOnProperty = entity.GetType().GetProperty("CreatedOn");
                    var createdByProperty = entity.GetType().GetProperty("CreatedBy");
                    createdOnProperty?.SetValue(entity, DateTime.UtcNow);
                    isActiveProperty?.SetValue(entity, true);
                    createdByProperty?.SetValue(entity, 1);
                } 
                var query = GetInsertQuery(entity, audits, delete);
                idProperty.SetValue(entity, await DbConnection.ExecuteScalarAsync<int>(query, entity));
            }

            return (int)idProperty.GetValue(entity);
        }

        public virtual async Task Update<T>(T entity, bool audits = true, bool delete = true)
        {
            var isActiveProperty = entity.GetType().GetProperty("IsActive");
            if (audits == true)
            {
                var createdOnProperty = entity.GetType().GetProperty("ModifiedOn");
                var createdByProperty = entity.GetType().GetProperty("ModifiedBy");
                createdOnProperty?.SetValue(entity, DateTime.UtcNow);
                isActiveProperty?.SetValue(entity, true); 
                createdByProperty?.SetValue(entity, 1);
            } 
            var query = GetUpdateQuery(entity, audits, delete);
            await DbConnection.ExecuteAsync(query, entity);
        }


        public virtual async Task Delete<T>(T Entity, int id)
        {
            var query = $"UPDATE dbo.[{Entity.GetType().Name}] SET IsActive = 0 WHERE Id = Id";
            await DbConnection.ExecuteAsync(query, new { Id = id });
        }
        //public virtual async Task<bool> DeleteAsync<T>(int id) where T : class
        //{
        //    if (id >= 0)
        //    {
        //        var entityType = typeof(T).Name;
        //        var query = $"UPDATE dbo.[{entityType}] SET IsActive = 0 WHERE Id = Id";
        //        try
        //        {
        //            var rowsAffected = await DbConnection.ExecuteAsync(query, new { Id = id });
        //            return rowsAffected > 0;
        //        }
        //        catch (Exception ex)
        //        {
        //            //Log the exception
        //        }
        //    }
        //    return false;
        //}


        private string GetInsertQuery<T>(T Entity, bool audits = true, bool delete = true)
        {
            var query = new StringBuilder($"INSERT dbo.[{Entity.GetType().Name}] (");
            var values = new StringBuilder("VALUES(");
          
            GetFilledEntityProperties(Entity);

            if (audits == true)
            {
                foreach (var property in _entityProperties)
                {
                    if (property == $"Id") continue;
                    if (delete == false && property == $"IsActive") continue;
                    query.Append($"[{property}],");
                    values.Append($"{property},");
                }
            }
            else
            {
                foreach (var property in _entityProperties)
                {
                    if (property != $"CreatedBy" && property != $"CreatedOn" && property != $"ModifiedBy" && property != $"ModifiedOn")
                    {
                        if (property == $"Id") continue;
                        if (delete == false && property == $"IsActive") continue;
                        query.Append($"[{property}],");
                        values.Append($"{property},");
                    }
                }
            }

            query.Remove(query.Length - 1, 1);
            values.Remove(values.Length - 1, 1);

            query.Append(")");
            values.Append(") SELECT SCOPE_IdENTITY()");
            query.Append(values);

            return query.ToString();
        }

        private string GetUpdateQuery<T>(T Entity, bool audits = true, bool delete = true)
        {
            var query = new StringBuilder($"UPDATE dbo.[{Entity.GetType().Name}] SET ");
            GetFilledEntityProperties(Entity);
            var omittedProperties = new[] { $"Id", "CreatedOn" };
            var omittedProperties2 = new[] { $"Id", "CreatedOn", "CreatedBy", "ModifiedBy", "ModifiedOn", "IsActive", };



            foreach (var property in _entityProperties)
            {
                if (omittedProperties.Contains(property)) continue;
                if (audits == false)
                    if (omittedProperties2.Contains(property)) continue;
                if (delete == false && property == $"IsActive") continue;
                query.Append($"[{property}]={property},");
            }

            query.Remove(query.Length - 1, 1);

            query.Append($" WHERE Id = Id");

            return query.ToString();
        }

        private void GetFilledEntityProperties<T>(T Entity)
        {
            _entityProperties = null;
            if (_entityProperties != null) return;
            _entityProperties = Entity.GetType().GetProperties().Where(x => !x.GetMethod.IsVirtual).Select(x => x.Name)
                .ToArray();
        }

        public async Task DeletePerminent<T>(T Entity, int id)
        {
            var query = $"delete dbo.[{Entity.GetType().Name}] WHERE Id = Id";
            await DbConnection.ExecuteAsync(query, new { Id = id });
        }

        public async Task DeletesByColumn<T>(T Entity, int id, string column)
        {
            var query = $"delete dbo.[{Entity.GetType().Name}] WHERE {column} = Id";
            await DbConnection.ExecuteAsync(query, new { Id = id });
        }
    }
}
