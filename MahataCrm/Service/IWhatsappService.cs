namespace MahataCrm.Service
{
    public interface IWhatsappService
    {
        void EnvoyerNotificationWhatsApp(long toWhatsAppNumber, string message);
    }
}
