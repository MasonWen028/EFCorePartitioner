using EFCorePartitioner.Interfaces;
using EFCorePartitioner.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCorePartitioner.Strategies
{
    /// <summary>
    /// Implements a partition strategy that partitions entities based on their date.
    /// </summary>
    public class ByDatePartitionStrategy : IPartitionStrategy
    {
        /// <summary>
        /// Gets the partition key for the specified entity.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="entity">The entity for which to get the partition key.</param>
        /// <param name="id">The name of the property that contains the partition key value (default: "Id").</param>
        /// <returns>The formatted partition key value (e.g., "202308" for a date partition).</returns>
        public string? GetPartitionKey<T>(T entity, string id = "Id")
        {
            var property = typeof(T).GetProperty(id);
            if (property != null && entity != null)
            {
                var partitionKey = property.GetValue(entity);
                if (partitionKey != null) 
                {
                    return partitionKey.ToString();
                }
            }

            return null;
        }


        /// <summary>
        /// Gets the table name for the specified entity based on the partition key and partition options.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="entity">The entity for which to get the table name.</param>
        /// <param name="options">The partition options.</param>
        /// <returns>The table name (Orders_{0:yyyyMM} format).</returns>
        public string? GetTableName<T>(T entity, PartitionOptions options)
        {
            var partitionKey = GetPartitionKey(entity);
            if (partitionKey != null)
            {
                return string.Format(options.TableNameFormat, partitionKey);
            }

            return null;
        }
    }

}
