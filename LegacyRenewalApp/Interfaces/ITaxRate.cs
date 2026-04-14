namespace LegacyRenewalApp.Interfaces
{
    public interface ITaxRate
    {
        decimal GetRate(string country);
    }
}
