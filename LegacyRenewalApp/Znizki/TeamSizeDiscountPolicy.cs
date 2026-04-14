using LegacyRenewalApp.Interfaces;

namespace LegacyRenewalApp.Znizki
{
    /// Zniżka za liczbę stanowisk
    public class TeamSizeDiscountPolicy : IDiscountPolicy
    {
        public FinalnaZnizka LiczenieZnizki(KontekstZnizki context)
        {
            int seatCount = context.SeatCount;
            decimal baseAmount = context.BaseAmount;

            if (seatCount >= 50)
                return new FinalnaZnizka(baseAmount * 0.12m, "large team discount; ");

            if (seatCount >= 20)
                return new FinalnaZnizka(baseAmount * 0.08m, "medium team discount; ");

            if (seatCount >= 10)
                return new FinalnaZnizka(baseAmount * 0.04m, "small team discount; ");

            return FinalnaZnizka.None;
        }
    }
}
