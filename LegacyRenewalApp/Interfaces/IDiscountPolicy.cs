using LegacyRenewalApp.Znizki;

namespace LegacyRenewalApp.Interfaces
{
    public interface IDiscountPolicy
    {
        FinalnaZnizka LiczenieZnizki(KontekstZnizki context);
    }
}
