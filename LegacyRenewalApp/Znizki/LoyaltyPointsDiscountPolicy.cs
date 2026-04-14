using System;
using LegacyRenewalApp.Interfaces;

namespace LegacyRenewalApp.Znizki
{
    //Zniżka; punkty lojalnościowe
    public class LoyaltyPointsDiscountPolicy : IDiscountPolicy
    {
        private const int MaxPointsUsable = 200;

        public FinalnaZnizka LiczenieZnizki(KontekstZnizki context)
        {
            if (!context.UseLoyaltyPoints || context.Customer.LoyaltyPoints <= 0)
                return FinalnaZnizka.None;

            int pointsToUse = Math.Min(context.Customer.LoyaltyPoints, MaxPointsUsable);
            return new FinalnaZnizka(pointsToUse, $"loyalty points used: {pointsToUse}; ");
        }
    }
}
