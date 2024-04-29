using EFCorePartitioner.Attributes;
using EFCorePartitioner.Enums;
using EFCorePartitioner.Interfaces;
using EFCorePartitioner.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EFCorePartitioner.Strategies
{
    /// <summary>
    /// Implements a partition strategy that partitions entities based on their date.
    /// </summary>
    public class TablePartitioningStrategy : IPartitionStrategy
    {
        public string GetTableName<T>(T entity)
        {
            var type = typeof(T);
            var partitionAttr = type.GetCustomAttribute<TableSplittingAttribute>();

            if (partitionAttr != null && partitionAttr.EnablePartitioning)
            {
                switch (partitionAttr.Strategy)
                {
                    case PartitionStrategy.TimeBased:
                        return GetTimeBasedTableName(entity, partitionAttr);
                    case PartitionStrategy.QuantityBased:
                        return GetQuantityBasedTableName(entity, partitionAttr);
                    default:
                        throw new InvalidOperationException("Unsupported partition strategy.");
                }
            }

            return type.Name;  // Fallback to the default table name
        }

        private string GetTimeBasedTableName<T>(T entity, TableSplittingAttribute partitionAttr)
        {    
            string tableName = typeof(T).Name;
            if (partitionAttr == null || !partitionAttr.EnablePartitioning)
                return tableName;

            var idProperty = FindIdProperty(entity);
            if (idProperty == null || idProperty.PropertyType != typeof(long))
                throw new InvalidOperationException("No valid ID property found for partitioning.");

            long idValue = (long)idProperty.GetValue(entity);
            DateTime date = SnowflakeIdGenerator.GetTimestampFromId(idValue);

            switch (partitionAttr.PartitionUnit)
            {
                case TimePartitionUnit.Daily:
                    return $"{tableName}_{date:yyyyMMdd}";
                case TimePartitionUnit.Weekly:
                    int weekOfYear = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
                    return $"{tableName}_{date.Year}_W{weekOfYear}";
                case TimePartitionUnit.Monthly:
                    return $"{tableName}_{date:yyyyMM}";
                default:
                    //todo write log to indicate user there's no partitionStrategy applied successfully.
                    return tableName;
            }
        }

        private PropertyInfo FindIdProperty<TEntity>(TEntity entity)
        {
            var type = typeof(TEntity);
            // 首先尝试找到同时标记为 SnowflakeIdAttribute 且 IsPrimaryKey 为 true 的属性
            var idProperty = type.GetProperties()
                .FirstOrDefault(p => Attribute.IsDefined(p, typeof(SnowflakeIdAttribute)) &&
                                     p.GetCustomAttribute<SnowflakeIdAttribute>().IsPrimaryKey);

            // 如果没有符合条件的 SnowflakeIdAttribute 属性，尝试找到标记为 [Key] 的属性
            if (idProperty == null)
            {
                idProperty = type.GetProperties()
                    .FirstOrDefault(p => Attribute.IsDefined(p, typeof(KeyAttribute)));
            }

            return idProperty;
        }


        private string GetQuantityBasedTableName<TEntity>(TEntity entity, TableSplittingAttribute partitionAttr)
        {
            // Assume we have a mechanism to track quantity and decide on the table
            return $"{partitionAttr}_QuantityBased";
        }
    }

}
