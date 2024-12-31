namespace PROJECT_SEM3.ViewModels
{
    public class PostViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Thumbnail { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Content { get; internal set; }
        public string Title { get; set; }
    }
}
