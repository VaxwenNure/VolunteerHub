using System.Text.Json;
using VolunteerHub.Mvc.Models;

namespace VolunteerHub.Mvc.Services
{
    public class JsonStore : IJsonStore
    {
        private readonly string _basePath;

        public JsonStore(IWebHostEnvironment env)
        {
            _basePath = Path.Combine(env.ContentRootPath, "App_Data");
            Directory.CreateDirectory(_basePath);
        }

        private string PathFor(string file) => Path.Combine(_basePath, file);

        private static readonly JsonSerializerOptions Opt = new()
        {
            WriteIndented = true
        };

        private List<T> Read<T>(string file) where T : class
        {
            var path = PathFor(file);
            if (!File.Exists(path)) return new List<T>();
            var json = File.ReadAllText(path);
            return string.IsNullOrWhiteSpace(json) ? new List<T>() : (JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>());
        }

        private void Write<T>(string file, List<T> data)
        {
            var path = PathFor(file);
            File.WriteAllText(path, JsonSerializer.Serialize(data, Opt));
        }

        public List<Volunteer> GetVolunteers() => Read<Volunteer>("volunteers.json");
        public void SaveVolunteers(List<Volunteer> items) => Write("volunteers.json", items);

        public List<HelpType> GetHelpTypes() => Read<HelpType>("helpTypes.json");
        public void SaveHelpTypes(List<HelpType> items) => Write("helpTypes.json", items);

        public List<HelpRecord> GetHelpRecords() => Read<HelpRecord>("helpRecords.json");
        public void SaveHelpRecords(List<HelpRecord> items) => Write("helpRecords.json", items);
    }
}
