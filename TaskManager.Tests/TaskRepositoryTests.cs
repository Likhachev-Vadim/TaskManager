using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaskManager.Core;
using System;

namespace TaskManager.Tests
{
    [TestClass]
    public class TaskRepositoryTests
    {
        [TestMethod]
        public void Add_AddsTask()
        {
            var repo = new TaskRepository();
            repo.Add(new TaskItem { Title = "Тест" });
            Assert.AreEqual(1, repo.GetAll().Count);
        }

        [TestMethod]
        public void Remove_RemovesTask()
        {
            var repo = new TaskRepository();
            var task = new TaskItem { Title = "Удалить" };
            repo.Add(task);
            repo.Remove(task.Id);
            Assert.AreEqual(0, repo.GetAll().Count);
        }

        [TestMethod]
        public void FilterByStatus_ReturnsCorrect()
        {
            var repo = new TaskRepository();
            repo.Add(new TaskItem { Status = Core.TaskStatus.Done });
            repo.Add(new TaskItem { Status = Core.TaskStatus.New });
            var result = repo.FilterByStatus(Core.TaskStatus.Done);
            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public void Search_FindsByTitle()
        {
            var repo = new TaskRepository();
            repo.Add(new TaskItem { Title = "Купить молоко" });
            repo.Add(new TaskItem { Title = "Сделать уроки" });
            var result = repo.Search("молоко");
            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public void SaveLoad_Roundtrip()
        {
            var repo = new TaskRepository();
            repo.Add(new TaskItem { Title = "Сохранить" });
            var path = "test_tasks.json";
            repo.SaveToFile(path);

            var repo2 = new TaskRepository();
            repo2.LoadFromFile(path);
            Assert.AreEqual(1, repo2.GetAll().Count);
        }
    }
}