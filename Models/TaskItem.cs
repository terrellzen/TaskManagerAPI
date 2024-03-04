using System.ComponentModel.DataAnnotations;

namespace TaskManagerApi.Models
{
    public class TaskItem
    {
        [Key]
        public int TaskId { get; set; }
        public string TaskName { get; set; } = "";
        public string TaskDesc { get; set; } = "";
        public int Status { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public int ProjectId { get; set; }
        public string ProjectDesc { get; set; } = "";

    }
}
