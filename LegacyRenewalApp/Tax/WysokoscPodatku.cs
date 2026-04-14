using System.Collections.Generic;
using LegacyRenewalApp.Interfaces;

namespace LegacyRenewalApp.Tax
{
    public class WysokoscPodatku : ITaxRate
    {
        private const decimal DefaultRate = 0.20m;

        private static readonly Dictionary<string, decimal> CountryRates = new()
        {
            { "Poland", 0.23m },
            { "Germany", 0.19m },
            { "Czech Republic", 0.21m },
            { "Norway", 0.25m }
        };

        public decimal GetRate(string country)
        {
            return CountryRates.TryGetValue(country, out decimal rate) ? rate : DefaultRate;
        }
    }
}
