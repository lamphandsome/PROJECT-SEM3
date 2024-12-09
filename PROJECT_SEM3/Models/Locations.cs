namespace PROJECT_SEM3.Models
{
    public class Location
    {
        public int Id { get; set; }
        public string Country { get; set; }
        public string City { get; set; }

        public ICollection<Users> Users { get; set; }
    }
}
