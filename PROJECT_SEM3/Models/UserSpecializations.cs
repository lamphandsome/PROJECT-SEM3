namespace PROJECT_SEM3.Models
{
    public class UserSpecialization
    {
        public int Id { get; set; }
        public string UserId { get; set; } // Foreign Key to Users
        public int SpecializationId { get; set; } // Foreign Key to Specializations

        public Users User { get; set; }
        public Specialization Specialization { get; set; }
    }
}
