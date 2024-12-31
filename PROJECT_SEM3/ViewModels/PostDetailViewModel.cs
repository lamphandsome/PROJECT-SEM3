namespace PROJECT_SEM3.ViewModels
{
    public class PostDetailViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string ImagePath { get; set; }
        public DateTime CreatedAt { get; set; }
        public string AuthorName { get; set; }
        public List<CommentViewModel> Comments { get; set; }
    }
}
