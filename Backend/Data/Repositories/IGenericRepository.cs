using System.Linq.Expressions;

namespace KingOfKings.Backend.Data.Repositories
{
    /// <summary>
    /// Generic Repository Interface defining standard data access operations.
    /// 泛型存儲庫介面，定義標準的資料存取操作。
    /// </summary>
    /// <typeparam name="T">The entity type (實體類型).</typeparam>
    public interface IGenericRepository<T> where T : class
    {
        /// <summary>
        /// Gets all entities.
        /// 取得所有實體。
        /// </summary>
        /// <returns>A collection of all entities (所有實體的集合).</returns>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Gets an entity by its identifier.
        /// 根據識別碼取得實體。
        /// </summary>
        /// <param name="id">The entity identifier (實體識別碼).</param>
        /// <returns>The entity if found, otherwise null (如果找到則返回實體，否則返回 null).</returns>
        Task<T?> GetByIdAsync(object id);

        /// <summary>
        /// Finds entities based on a predicate.
        /// 根據條件尋找實體。
        /// </summary>
        /// <param name="predicate">The filter condition (過濾條件).</param>
        /// <returns>A collection of matching entities (符合條件的實體集合).</returns>
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Adds a new entity.
        /// 新增一個實體。
        /// </summary>
        /// <param name="entity">The entity to add (要新增的實體).</param>
        Task AddAsync(T entity);

        /// <summary>
        /// Updates an existing entity.
        /// 更新現有的實體。
        /// </summary>
        /// <param name="entity">The entity to update (要更新的實體).</param>
        void Update(T entity);

        /// <summary>
        /// Removes an entity.
        /// 移除一個實體。
        /// </summary>
        /// <param name="entity">The entity to remove (要移除的實體).</param>
        void Remove(T entity);

        /// <summary>
        /// Saves changes to the database.
        /// 儲存變更到資料庫。
        /// </summary>
        Task SaveAsync();
    }
}
