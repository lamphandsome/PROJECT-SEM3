namespace PROJECT_SEM3.ViewModels
{
    public class DoctorViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Thumbnail { get; set; }
        public string Locations { get; internal set; }
    }
}
