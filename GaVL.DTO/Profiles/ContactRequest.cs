namespace GaVL.DTO.Profiles
{
    public class ContactRequest
    {
        public string Provider { get; set; }

        public string Value { get; set; }

        public string? DisplayLabel { get; set; }

        public bool IsPublic { get; set; } = true;

        public int Position { get; set; } = 0;
    }
}
