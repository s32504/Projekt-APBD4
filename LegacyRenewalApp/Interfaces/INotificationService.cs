namespace LegacyRenewalApp.Interfaces
{
    public interface INotificationService
    {
        void SendRenewalConfirmation(Customer customer, RenewalInvoice invoice);
    }
}
