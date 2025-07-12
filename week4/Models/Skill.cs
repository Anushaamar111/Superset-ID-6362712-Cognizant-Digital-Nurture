namespace MyFirstApi.Models
{
    public class Skill
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Level { get; set; } = string.Empty; // Beginner, Intermediate, Advanced, Expert
        public int YearsOfExperience { get; set; }
    }
}
