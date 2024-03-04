using Microsoft.EntityFrameworkCore;
using TaskManagerApi.Models;

namespace TaskManagerApi.Data
{
    public class TaskDbContext : DbContext
    {
        public TaskDbContext(DbContextOptions<TaskDbContext> options)
            : base(options)
        {
        }
        // DbSet for the TaskItem entity with table name 'tasks'
        public DbSet<TaskItem> Tasks { get; set; }
    }
}