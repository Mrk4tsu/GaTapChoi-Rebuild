namespace GaVL.DTO.Notification
{
    public record MemberJoinedDto(
        string UserId,
        string Username,
        string Discriminator,
        string AvatarUrl,
        DateTime JoinedAt,
        string GuildId,
        string GuildName
    );
}
