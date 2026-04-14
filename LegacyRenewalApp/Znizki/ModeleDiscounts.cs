namespace LegacyRenewalApp.Znizki
{
    public class KontekstZnizki
    {
        public Customer Customer { get; }
        public SubscriptionPlan Plan { get; }
        public int SeatCount { get; }
        public decimal BaseAmount { get; }
        public bool UseLoyaltyPoints { get; }

        public KontekstZnizki(Customer customer, SubscriptionPlan plan, int seatCount, decimal baseAmount, bool useLoyaltyPoints)
        {
            Customer = customer;
            Plan = plan;
            SeatCount = seatCount;
            BaseAmount = baseAmount;
            UseLoyaltyPoints = useLoyaltyPoints;
        }
    }

   
    public class FinalnaZnizka
    {
        public decimal Amount { get; }
        public string Note { get; }
        
        
        public FinalnaZnizka(decimal amount, string note)
        {
            Amount = amount;
            Note = note;
        }

        public static FinalnaZnizka None => new FinalnaZnizka(0m, string.Empty);
    }
}
