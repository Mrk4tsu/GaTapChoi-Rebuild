namespace GaVL.DTO.Notification
{
    public class NotifyViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public string Thumbnail { get; set; }
        public string Url { get; set; }
    }
}
