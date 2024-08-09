namespace applicationcrm.Models
{
    public class ServicePlan
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Duration { get; set; } // Duration in months
        public double Price { get; set; }
        public DateTime CreatedAt { get; set; } // New field to track creation date
        public ICollection<Account> Accounts { get; set; }
    }
}
