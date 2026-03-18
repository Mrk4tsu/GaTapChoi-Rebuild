namespace GaVL.DTO.Profiles
{
    public class ContactViewModel
    {
        public Guid Id { get; set; }
        public string Provider { get; set; }
        public string Value { get; set; }
        public string? DisplayLabel { get; set; }
        public bool IsPublic { get; set; }
        public int Position { get; set; }
    }
}
