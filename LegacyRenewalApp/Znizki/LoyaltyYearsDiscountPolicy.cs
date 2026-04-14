using LegacyRenewalApp.Interfaces;

namespace LegacyRenewalApp.Znizki
{
    /// Zniżka za staż klienta
    public class LoyaltyYearsDiscountPolicy : IDiscountPolicy
    {
        public FinalnaZnizka LiczenieZnizki(KontekstZnizki context)
        {
            int years = context.Customer.YearsWithCompany;
            decimal baseAmount = context.BaseAmount;

            if (years >= 5)
                return new FinalnaZnizka(baseAmount * 0.07m, "long-term loyalty discount; ");

            if (years >= 2)
                return new FinalnaZnizka(baseAmount * 0.03m, "basic loyalty discount; ");

            return FinalnaZnizka.None;
        }
    }
}
