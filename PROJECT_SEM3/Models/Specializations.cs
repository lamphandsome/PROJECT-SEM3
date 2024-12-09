namespace PROJECT_SEM3.Models
{
    public class Specialization
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<UserSpecialization> UserSpecializations { get; set; }
    }
}
