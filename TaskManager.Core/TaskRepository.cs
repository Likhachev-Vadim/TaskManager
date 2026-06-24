using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace TaskManager.Core
{
    public class TaskRepository
    {
        private List<TaskItem> _tasks = new List<TaskItem>();

        public void Add(TaskItem task)
        {
            _tasks.Add(task);
        }

        public void Remove(Guid id)
        {
            _tasks.RemoveAll(t => t.Id == id);
        }

        public void Update(TaskItem updated)
        {
            var idx = _tasks.FindIndex(t => t.Id == updated.Id);
            if (idx >= 0) _tasks[idx] = updated;
        }

        public List<TaskItem> GetAll()
        {
            return _tasks.ToList();
        }

        public List<TaskItem> FilterByStatus(TaskStatus status)
        {
            return _tasks.Where(t => t.Status == status).ToList();
        }

        public List<TaskItem> Search(string query)
        {
            return _tasks
                .Where(t => t.Title.Contains(query, StringComparison.OrdinalIgnoreCase)
                         || t.Description.Contains(query, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public List<TaskItem> GetSortedByPriority()
        {
            return _tasks.OrderByDescending(t => t.Priority).ToList();
        }

        public List<TaskItem> GetSortedByDueDate()
        {
            return _tasks.OrderBy(t => t.DueDate).ToList();
        }

        public int CountDone() => _tasks.Count(t => t.Status == TaskStatus.Done);
        public int CountOverdue() => _tasks.Count(t => t.DueDate < DateTime.Today && t.Status != TaskStatus.Done);

        public void SaveToFile(string path)
        {
            var json = JsonConvert.SerializeObject(_tasks, Formatting.Indented);
            File.WriteAllText(path, json);
        }

        public void LoadFromFile(string path)
        {
            if (!File.Exists(path)) return;
            var json = File.ReadAllText(path);
            _tasks = JsonConvert.DeserializeObject<List<TaskItem>>(json) ?? new List<TaskItem>();
        }
    }
}