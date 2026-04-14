using System;
using System.Collections.Generic;
using LegacyRenewalApp.Interfaces;

namespace LegacyRenewalApp.Fees
{
    
    public class FeeResult
    {
        public decimal Fee { get; }
        public string Note { get; }

        public FeeResult(decimal fee, string note)
        {
            Fee = fee;
            Note = note;
        }
        
        
        public static FeeResult None => new FeeResult(0m, string.Empty);
    }

    //Wsparcie (techniczne? troche tego nie łapie lmao)
    public class SupportFee : ISupportFeeCalculator
    {
        private static readonly Dictionary<string, decimal> PlanFees = new()
        {
            { "START", 250m },
            { "PRO",  400m },
            { "ENTERPRISE", 700m }
        };

        public FeeResult Calculate(string planCode, bool includePremiumSupport)
        {
            if (!includePremiumSupport)
                return FeeResult.None;

            if (PlanFees.TryGetValue(planCode, out decimal fee))
                return new FeeResult(fee, "premium support included; ");

            return FeeResult.None;
        }
    }

    //metoda płatności
    public class PaymentFee : IPaymentFeeCalculator
    {
        private static readonly Dictionary<string, (decimal Rate, string Note)> PaymentRates = new()
        {
            { "CARD", (0.02m,  "card payment fee; ") },
            { "BANK_TRANSFER", (0.01m,  "bank transfer fee; ") },
            { "PAYPAL",  (0.035m, "paypal fee; ") },
            { "INVOICE", (0m,     "invoice payment; ") }
        };

        public FeeResult LiczenieMachen(string paymentMethod, decimal taxableBase)
        {
            if (!PaymentRates.TryGetValue(paymentMethod, out var entry))
                throw new ArgumentException("Unsupported payment method");
            return new FeeResult(taxableBase * entry.Rate, entry.Note);
        }
    }
}
