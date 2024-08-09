using applicationcrm.Data;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace applicationcrm.Models
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
            DateTime oneMonthLater = now.AddMonths(1);
            DateTime oneWeekLater = now.AddDays(7);

            // Fetch services that are expiring soon
            var expiringServices = await _context.ServicePlans
                .Where(sp => sp.CreatedAt.AddMonths(sp.Duration) <= oneMonthLater)
                .Include(sp => sp.Accounts)
                .ToListAsync();
                foreach (var service in expiringServices)
                {

                    var expiryDate = service.CreatedAt.AddMonths(service.Duration);

                    // Determine if emails need to be sent
                    if (expiryDate <= oneMonthLater)
                    {

                        bool sendWeeklyReminder = (now.DayOfWeek == DayOfWeek.Friday);
                        bool sendDailyReminder = (expiryDate <= now.AddDays(7));


                        if (sendWeeklyReminder)
                        {
                            foreach (var account in service.Accounts)
                            {
                                if (!string.IsNullOrEmpty(account.Email))
                                {
                                    await _emailSender.SendEmailAsync(account.Email, "Service Expiry Reminder",
                                        $"Your service plan '{service.Name}' is expiring on {expiryDate.ToShortDateString()}. Please take action if needed.");
                                }
                            }
                        }
                        bool isInLastWeek = now >= expiryDate.AddDays(-7) && now <= expiryDate && now.DayOfWeek >= DayOfWeek.Monday && now.DayOfWeek <= DayOfWeek.Friday;

                        if (sendDailyReminder && isInLastWeek)
                        {
                            foreach (var account in service.Accounts)
                            {
                                if (!string.IsNullOrEmpty(account.Email))
                                {
                                    await _emailSender.SendEmailAsync(account.Email, "Service Expiry Reminder",
                                        $" Hello ,'{account.BusinessName}' ,  Your service plan '{service.Name}' is expiring on {expiryDate.ToShortDateString()}. Please take action if needed.");
                                }
                            }
                        }
                    }
                }
            }

        }
    }

    

