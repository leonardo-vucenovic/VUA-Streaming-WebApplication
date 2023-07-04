namespace PublicModul.ViewModels
{
    public class VMVideo
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string Name { get; set; }
        public string Description { get; set; }
        public int GenreId { get; set; }
        public int TotalSeconds { get; set; }
        public string StreamingUrl { get; set; }
        public int ImageId { get; set; }
        public virtual VMGenre Genre { get; set; } = null!;
        public virtual VMImage Image { get; set; }
        public string GenreName { get; set; }
        public string ImageContent { get; set; }
    }
}
