using System;

namespace TaskManager.Core
{
    public enum TaskStatus
    {
        New,
        InProgress,
        Done
    }

    public enum TaskPriority
    {
        Low,
        Medium,
        High
    }

    public class TaskItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;
        public DateTime DueDate { get; set; } = DateTime.Today;
        public TaskStatus Status { get; set; } = TaskStatus.New;
        public bool IsImportant { get; set; } = false;
    }
}