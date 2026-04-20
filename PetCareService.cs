using System.Text.Json;

namespace PetCareMobile
{
    public enum TaskType { Feeding, Medication, Walk, Grooming }

    public class PetActivity
    {
        public DateTime TimeStamp { get; set; }
        public string PetName { get; set; }
        public TaskType Task { get; set; }
        public string LoggedBy { get; set; }
    }

    public class PetCareService
    {
        private List<PetActivity> _tasks = new List<PetActivity>();
        private readonly string _filePath = Path.Combine(FileSystem.AppDataDirectory, "pet_history.json");

        public void AddTask(string pet, TaskType task, string user)
        {
            _tasks.Insert(0, new PetActivity
            {
                TimeStamp = DateTime.Now,
                PetName = pet,
                Task = task,
                LoggedBy = string.IsNullOrWhiteSpace(user) ? "Someone" : user
            });
        }

        public List<PetActivity> GetAllTasks() => _tasks;

        public void SaveToFile()
        {
            try
            {
                string json = JsonSerializer.Serialize(_tasks);
                File.WriteAllText(_filePath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving data: {ex.Message}");
            }
        }

        public void LoadFromFile()
        {
            try
            {
                if (File.Exists(_filePath))
                {
                    string json = File.ReadAllText(_filePath);
                    _tasks = JsonSerializer.Deserialize<List<PetActivity>>(json) ?? new List<PetActivity>();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading data: {ex.Message}");
                _tasks = new List<PetActivity>(); // Reset to empty list if file is broken
            }
        }
    }
}
