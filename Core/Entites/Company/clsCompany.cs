namespace Core.Entites.Company
{
    public class clsCompany
    {
        public long COM_ID { get; set; }
        public long COM_CODE { get; set; }
        public string? COM_NAME { get; set; } = string.Empty;
        public string? COM_ADDRESS { get; set; } = string.Empty;
        public string? COM_MOBILE { get; set; } = string.Empty;
        public bool COM_STATE { get; set; } = true;
        public long CLI_ID { get; set; }
    }
}
