namespace DwitterSocial.Dtos
{
    public class CreateMessageDto
    {
        public string ReceiverUsername { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}
