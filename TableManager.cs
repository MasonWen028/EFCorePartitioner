using EFCorePartitioner.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EFCorePartitioner
{
    public class TableManager
    {
        private readonly DbContext _context;
        private readonly IPartitionStrategy _strategy;

        public TableManager(DbContext context, IPartitionStrategy strategy)
        {
            _context = context;
            _strategy = strategy;
        }

        public void EnsureTableExists<T>(T entity)
        {
            var tableName = _strategy.GetTargetTableName(entity);
            // 逻辑来检查表是否存在，如果不存在则创建
        }
    }

}
