using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace lab3
{
    public class JsonManager
    {
        private readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
            PropertyNameCaseInsensitive = true
        };

        public async Task SaveToJson(string filePath, List<Student> students)
        {
            string jsonString = JsonSerializer.Serialize(students, _options);
            await File.WriteAllTextAsync(filePath, jsonString);
        }
        public async Task<List<Student>> LoadFromJson(string filePath)
        {
            if (!File.Exists(filePath)) return new List<Student>();

            string json = await File.ReadAllTextAsync(filePath);

            try
            {
                var node = JsonNode.Parse(json);
                if (node is JsonArray)
                {
                    return node.Deserialize<List<Student>>(_options) ?? new List<Student>();
                }
                foreach (var item in node.AsObject())
                {
                    if (item.Value is JsonArray)
                    {
                        return item.Value.Deserialize<List<Student>>(_options) ?? new List<Student>();
                    }
                }
            }
            catch
            {
            }
            return new List<Student>();
        }
    }
    public class StudentManager
    {
        private List<Student> _allStudents = new();

        public void LoadStudents(List<Student> students) => _allStudents = students;
        public List<Student> GetAllStudents() => _allStudents;
        public void AddStudent(Student s) => _allStudents.Add(s);
        public void DeleteStudent(Student s) => _allStudents.Remove(s);

        public void UpdateStudent(Student oldStudent, Student newStudent)
        {
            var index = _allStudents.IndexOf(oldStudent);
            if (index != -1) _allStudents[index] = newStudent;
        }

        public List<string> GetUniqueFaculties() =>
            _allStudents.Select(s => s.Faculty).Distinct().Where(f => !string.IsNullOrEmpty(f)).ToList();

        public List<Student> Search(SearchParams p)
        {
            var query = _allStudents.AsQueryable();

            if (!string.IsNullOrWhiteSpace(p.Name))
                query = query.Where(s => s.Name != null && s.Name.Contains(p.Name, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(p.Faculty) && p.Faculty != "Всі факультети")
                query = query.Where(s => s.Faculty != null && s.Faculty.Equals(p.Faculty, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(p.Department))
                query = query.Where(s => s.Department != null && s.Department.Contains(p.Department, StringComparison.OrdinalIgnoreCase));

            if (p.Year.HasValue)
                query = query.Where(s => s.Year == p.Year.Value);

            if (!string.IsNullOrWhiteSpace(p.DisciplineName))
            {
                query = query.Where(s => s.Disciplines.Any(d =>
                    d.Title != null && d.Title.Contains(p.DisciplineName, StringComparison.OrdinalIgnoreCase)));
            }

            return query.ToList();
        }
    }
}