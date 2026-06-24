using System;
using System.Windows;
using TaskManager.Core;
using TaskStatus = TaskManager.Core.TaskStatus;

namespace TaskManager.UI
{
    public partial class TaskDialog : Window
    {
        public TaskItem? Result { get; private set; }
        private readonly Guid _existingId;

        public TaskDialog(TaskItem? existing = null)
        {
            InitializeComponent();
            DuePicker.SelectedDate = DateTime.Today;

            if (existing != null)
            {
                _existingId = existing.Id;
                TitleBox.Text = existing.Title;
                DescBox.Text = existing.Description;
                PriorityBox.SelectedIndex = (int)existing.Priority;
                StatusBox.SelectedIndex = (int)existing.Status;
                DuePicker.SelectedDate = existing.DueDate;
                ImportantBox.IsChecked = existing.IsImportant;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TitleBox.Text))
            {
                MessageBox.Show("Введите название задачи.");
                return;
            }

            Result = new TaskItem
            {
                Id = _existingId == Guid.Empty ? Guid.NewGuid() : _existingId,
                Title = TitleBox.Text.Trim(),
                Description = DescBox.Text.Trim(),
                Priority = (TaskPriority)PriorityBox.SelectedIndex,
                Status = (TaskStatus)StatusBox.SelectedIndex,
                DueDate = DuePicker.SelectedDate ?? DateTime.Today,
                IsImportant = ImportantBox.IsChecked == true
            };

            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}