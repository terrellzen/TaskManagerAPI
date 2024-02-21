namespace TaskManagerApi.Models
{
    public class TaskModel
    {
        public int TaskId { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public int Status { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = "";

    }
}
