using LegacyRenewalApp.Fees;

namespace LegacyRenewalApp.Interfaces
{
    public interface ISupportFeeCalculator
    {
        FeeResult Calculate(string planCode, bool includePremiumSupport);
    }

    public interface IPaymentFeeCalculator
    {
        FeeResult LiczenieMachen(string paymentMethod, decimal taxableBase);
    }
}
