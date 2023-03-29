namespace CommonAsyncService.DtoAPI
{
    public class EmailMessageDtoAPI
    {
        public string ToEmail { get; set; }
        public string? Subject { get; set; }
        public string? From { get; set; }
        public string Message { get; set; }
    }
}
