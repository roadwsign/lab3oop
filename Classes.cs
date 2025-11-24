using System.Text.Json.Serialization;

namespace lab3
{
    public class Student
    {
        public string Name { get; set; }
        public string Faculty { get; set; }
        public string Department { get; set; }
        public int Year { get; set; }
        public List<Discipline> Disciplines { get; set; }

        public Student()
        {
            Disciplines = new List<Discipline>();
        }
        [JsonIgnore]
        public string DisciplinesString => Disciplines.Count > 0
            ? string.Join("; ", Disciplines.Select(d => $"{d.Title} ({d.ControlType}: {d.Grade}, {d.Date})"))
            : "";
    }
    public class Discipline
    {
        public string Title { get; set; }
        public string ControlType { get; set; }
        public string Date { get; set; }
        public int Grade { get; set; }
    }

    public class SearchParams
    {
        public string Name { get; set; }
        public string Faculty { get; set; }
        public string Department { get; set; }
        public int? Year { get; set; }
        public string DisciplineName { get; set; }
    }
}