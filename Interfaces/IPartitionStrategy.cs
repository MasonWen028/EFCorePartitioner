using EFCorePartitioner.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCorePartitioner.Interfaces
{
    /// <summary>
    /// Defines the interface for a partition strategy.
    /// </summary>
    public interface IPartitionStrategy
    {
        /// <summary>
        /// Gets the partition key for the specified entity.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="entity">The entity for which to get the partition key.</param>
        /// <returns>The partition key value.</returns>
        string GetPartitionKey<T>(T entity, string id="Id");

        /// <summary>
        /// Gets the table name for the specified entity based on the partition key and partition options.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="entity">The entity for which to get the table name.</param>
        /// <param name="options">The partition options.</param>
        /// <returns>The table name.</returns>
        string GetTableName<T>(T entity, PartitionOptions options);
    }

}
