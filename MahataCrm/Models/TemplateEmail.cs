namespace MahataCrm.Models
{
    public class TemplateEmail
    {
        public int Id { get; set; }
        public string TypeEmail { get; set; }
        public string Header { get; set; }
        public string Object { get; set; }
        public string? Footer { get; set; }
    }
}
