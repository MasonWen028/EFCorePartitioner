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
        string GetTableName<T>(T entity);
    }

}
