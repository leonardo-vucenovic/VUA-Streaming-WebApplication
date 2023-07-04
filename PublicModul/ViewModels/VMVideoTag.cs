namespace PublicModul.ViewModels
{
    public class VMVideoTag
    {
        public int Id { get; set; }

        public int VideoId { get; set; }

        public int TagId { get; set; }

        public virtual VMTag Tag { get; set; } = null!;

        public virtual VMVideo Video { get; set; } = null!;
    }
}
