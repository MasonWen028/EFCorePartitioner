using EFCorePartitioner.Attributes;
using EFCorePartitioner.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EFCorePartitioner.Interceptors
{
    /// <summary>
    /// An EF Core SaveChangesInterceptor that automatically assigns Snowflake IDs to entities.
    /// </summary>
    public class AutoSnowflakeIdInterceptor : SaveChangesInterceptor
    {
        /// <summary>
        /// Overrides the SavedChanges method to assign Snowflake IDs after changes have been saved.
        /// </summary>
        /// <param name="eventData">Event data containing the DbContext used for the operation.</param>
        /// <param name="result">The result from the save operation.</param>
        /// <returns>Returns the original result after processing.</returns>
        public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
        {
            if (eventData.Context != null)
            {
                AutoAssignSnowflakeIds(eventData.Context);
            }
            return base.SavedChanges(eventData, result);
        }

        /// <summary>
        /// Asynchronously overrides the SavedChanges method to assign Snowflake IDs after changes have been saved.
        /// </summary>
        /// <param name="eventData">Event data containing the DbContext used for the operation.</param>
        /// <param name="result">The result from the save operation.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to request cancellation of the operation.</param>
        /// <returns>Returns the original result as a ValueTask after processing.</returns>
        public override ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
        {
            if (eventData.Context != null)
            {
                AutoAssignSnowflakeIds(eventData.Context);
            }
            return base.SavedChangesAsync(eventData, result, cancellationToken);
        }

        /// <summary>
        /// Assigns Snowflake IDs to eligible entity properties marked with the SnowflakeIdAttribute and designated as a primary key.
        /// </summary>
        /// <param name="context">The DbContext containing the entity instances being saved.</param>
        private void AutoAssignSnowflakeIds(DbContext context)
        {
            // Iterate over each entity added to the DbContext's ChangeTracker
            foreach (var entry in context.ChangeTracker.Entries().Where(e => e.State == EntityState.Added))
            {
                // Find all properties that are marked with SnowflakeIdAttribute
                var properties = entry.Entity.GetType().GetProperties()
                    .Where(prop => Attribute.IsDefined(prop, typeof(SnowflakeIdAttribute)));

                foreach (var property in properties)
                {
                    var attribute = property.GetCustomAttribute<SnowflakeIdAttribute>();
                    // Check if property is long, has the attribute, and is marked as a primary key
                    if (property.PropertyType == typeof(long) && attribute != null && attribute.IsPrimaryKey)
                    {
                        var currentVal = property.GetValue(entry.Entity);
                        if (currentVal != null)
                        {
                            long currentValue = (long)currentVal;
                            // Assign a Snowflake ID if the current value is uninitialized (0)
                            if (currentValue == 0)
                            {
                                property.SetValue(entry.Entity, SnowflakeIdGenerator.GenerateId());
                            }
                        }
                    }
                }
            }
        }
    }
}