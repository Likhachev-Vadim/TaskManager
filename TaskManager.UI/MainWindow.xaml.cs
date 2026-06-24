using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TaskManager.Core;
using TaskStatus = TaskManager.Core.TaskStatus;

namespace TaskManager.UI
{
    public partial class MainWindow : Window
    {
        private readonly TaskRepository _repo = new TaskRepository();
        private readonly string _filePath = "tasks.json";
        private List<TaskItem> _current = new List<TaskItem>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _repo.LoadFromFile(_filePath);
            Refresh();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _repo.SaveToFile(_filePath);
        }

        private void Refresh()
        {
            if (SortCombo == null || FilterCombo == null || SearchBox == null || StatsText == null || TaskList == null)
                return;

            var all = _repo.GetAll();

            var filterItem = (FilterCombo.SelectedItem as ComboBoxItem)?.Content?.ToString();
            if (filterItem == "Новая")
                all = all.Where(t => t.Status == TaskStatus.New).ToList();
            else if (filterItem == "В процессе")
                all = all.Where(t => t.Status == TaskStatus.InProgress).ToList();
            else if (filterItem == "Завершена")
                all = all.Where(t => t.Status == TaskStatus.Done).ToList();

            var q = SearchBox.Text.Trim();
            if (!string.IsNullOrEmpty(q))
                all = all.Where(t =>
                    t.Title.Contains(q, System.StringComparison.OrdinalIgnoreCase) ||
                    t.Description.Contains(q, System.StringComparison.OrdinalIgnoreCase)).ToList();

            var sortItem = (SortCombo.SelectedItem as ComboBoxItem)?.Content?.ToString();
            if (sortItem == "По приоритету")
                all = all.OrderByDescending(t => t.Priority).ToList();
            else if (sortItem == "По дате")
                all = all.OrderBy(t => t.DueDate).ToList();

            _current = all;
            TaskList.ItemsSource = all.Select(t => new TaskViewModel(t)).ToList();

            StatsText.Text = $"Всего: {_repo.GetAll().Count}  |  Завершено: {_repo.CountDone()}  |  Просрочено: {_repo.CountOverdue()}";
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new TaskDialog();
            if (dlg.ShowDialog() == true)
            {
                _repo.Add(dlg.Result!);
                Refresh();
            }
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            if (TaskList.SelectedIndex < 0) return;
            var selected = _current[TaskList.SelectedIndex];
            var dlg = new TaskDialog(selected);
            if (dlg.ShowDialog() == true)
            {
                _repo.Update(dlg.Result!);
                Refresh();
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (TaskList.SelectedIndex < 0) return;
            var selected = _current[TaskList.SelectedIndex];
            _repo.Remove(selected.Id);
            Refresh();
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e) => Refresh();
        private void FilterCombo_SelectionChanged(object sender, SelectionChangedEventArgs e) => Refresh();
        private void SortCombo_SelectionChanged(object sender, SelectionChangedEventArgs e) => Refresh();
        private void TaskList_SelectionChanged(object sender, SelectionChangedEventArgs e) { }
    }

    public class TaskViewModel
    {
        private readonly TaskItem _t;
        public TaskViewModel(TaskItem t) { _t = t; }
        public string ImportantMark => _t.IsImportant ? "★" : "";
        public string Title => _t.Title;
        public string Description => _t.Description;
        public string PriorityDisplay => _t.Priority switch
        {
            TaskPriority.High => "Высокий",
            TaskPriority.Medium => "Средний",
            _ => "Низкий"
        };
        public string StatusDisplay => _t.Status switch
        {
            TaskStatus.InProgress => "В процессе",
            TaskStatus.Done => "Завершена",
            _ => "Новая"
        };
        public string DueDateDisplay => _t.DueDate.ToString("dd.MM.yyyy");
    }
}