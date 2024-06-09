namespace CrmMahata.Models
{
    public class ServicePlan
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Duration { get; set; }
        public double Price { get; set; }
        public ICollection<Account> Accounts { get; set; }
    }
}
