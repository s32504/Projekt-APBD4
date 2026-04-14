using LegacyRenewalApp.Interfaces;

namespace LegacyRenewalApp.Znizki
{
    /// Zniżka na podstawie segmentu klienta (Silver / Gold / Platinum / Education).
    public class SegmentDiscountPolicy : IDiscountPolicy
    {
        public FinalnaZnizka LiczenieZnizki(KontekstZnizki context)
        {
            decimal baseAmount = context.BaseAmount;

            return context.Customer.Segment switch
            {
                "Silver"    => new FinalnaZnizka(baseAmount * 0.05m, "silver discount; "),
                "Gold"      => new FinalnaZnizka(baseAmount * 0.10m, "gold discount; "),
                "Platinum"  => new FinalnaZnizka(baseAmount * 0.15m, "platinum discount; "),
                "Education" when context.Plan.IsEducationEligible
                            => new FinalnaZnizka(baseAmount * 0.20m, "education discount; "),
                _           => FinalnaZnizka.None
            };
        }
    }
}
