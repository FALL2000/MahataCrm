using MahataCrm.Data;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace MahataCrm.Service
{
    public class ServiceExpiryJob : IJob
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender; // Utiliser l'interface
        private readonly ILogger<ServiceExpiryJob> _logger;

        public ServiceExpiryJob(ApplicationDbContext context, IEmailSender emailSender, ILogger<ServiceExpiryJob> logger)
        {
            _context = context;
            _emailSender = emailSender; // Injecter l'interface
            _logger = logger;
        }


        public async Task Execute(IJobExecutionContext context)
        {
            bool value = false;
            DateTime now = DateTime.Now;
            //DateTime oneMonthLater = now.AddMonths(1);
            DateTime twoWeekLater = now.AddDays(14);

            // Fetch services that are expiring soon
            var subcribes = await _context.Souscriptions
                .Where(s => s.DateSouscription.AddDays(s.ServicePlan.Duration) <= twoWeekLater).Include(s => s.Account).Include(s => s.ServicePlan).ToListAsync();
                
            foreach (var service in subcribes)
            {

                var expiryDate = service.DateSouscription.AddDays(service.ServicePlan.Duration);

                // Determine if emails need to be sent
                if (expiryDate <= twoWeekLater)
                {
                    
                    if (!string.IsNullOrEmpty(service.Account.Email))
                    {
                        await _emailSender.SendEmailAsync("axeltsotie@gmail.com", "Service Expiry Reminder",
                            $"Hello ,'{service.Account.BusinessName}' , Your service plan  is expiring on {expiryDate.ToShortDateString()}. Please take action if needed.");
                    }


                }
                    
                }
            }
        }

}

