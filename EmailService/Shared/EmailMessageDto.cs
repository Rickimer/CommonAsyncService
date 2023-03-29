namespace EmailService.Shared
{
    public class EmailMessageDto
    {
        public string ToEmail { get; set; } = string.Empty;
        public string? Subject { get; set; }
        public string? From { get; set; }
        public string Message { get; set; } = string.Empty;
        public EmailTypeDto emailType { get; set; }
    }
}
