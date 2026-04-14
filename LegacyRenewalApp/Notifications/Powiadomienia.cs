using LegacyRenewalApp.Interfaces;

namespace LegacyRenewalApp.Notifications
{
   //Maile z powiadomieniami
    public class Powiadomienia : INotificationService
    {
        private readonly IBillingGateway _billingGateway;

        public Powiadomienia(IBillingGateway billingGateway)
        {
            _billingGateway = billingGateway;
        }

        public void SendRenewalConfirmation(Customer customer, RenewalInvoice invoice)
        {
            if (string.IsNullOrWhiteSpace(customer.Email))
                return;

            string subject = "Subscription renewal invoice";
            string body =
                $"Hello {customer.FullName}, your renewal for plan {invoice.PlanCode} " +
                $"has been prepared. Final amount: {invoice.FinalAmount:F2}.";

            _billingGateway.SendEmail(customer.Email, subject, body);
        }
    }
}
