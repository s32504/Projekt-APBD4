using System;
using System.Collections.Generic;
using System.Linq;
using LegacyRenewalApp.Adapter;
using LegacyRenewalApp.Fees;
using LegacyRenewalApp.Interfaces;
using LegacyRenewalApp.Notifications;
using LegacyRenewalApp.Tax;
using LegacyRenewalApp.Validation;
using LegacyRenewalApp.Znizki;

namespace LegacyRenewalApp
{//Będę szczery, nie wiedziałem czy pisać odpowiedzi po polsku bo po polsku mamy zajęcia czy jechać angielskim :V
    public class SubscriptionRenewalService
    { //interfejsenmachen
        private readonly ICustomerRepository _customerRepository;
        private readonly ISubscriptionPlanRepository _planRepository;
        private readonly IEnumerable<IDiscountPolicy> _discountPolicies;
        private readonly ISupportFeeCalculator _supportFeeCalculator;
        private readonly IPaymentFeeCalculator _paymentFeeCalculator;
        private readonly ITaxRate _taxRate;
        private readonly IBillingGateway _billingGateway;
        private readonly INotificationService _notificationService;
        public SubscriptionRenewalService()
        { //Adapter Machen
            var billingGateway = new LegacyBillingGatewayAdapter();

            _customerRepository  = new CustomerRepository();
            _planRepository = new SubscriptionPlanRepository();
            _discountPolicies = new IDiscountPolicy[]
            {
                new SegmentDiscountPolicy(),
                new LoyaltyYearsDiscountPolicy(),
                new TeamSizeDiscountPolicy(),
                new LoyaltyPointsDiscountPolicy()
            };
            _supportFeeCalculator = new SupportFee();
            _paymentFeeCalculator = new PaymentFee();
            _taxRate      = new WysokoscPodatku();
            _billingGateway       = billingGateway;
            _notificationService  = new Powiadomienia(billingGateway);
        }
        
        public RenewalInvoice CreateRenewalInvoice(
            int customerId,
            string planCode,
            int seatCount,
            string paymentMethod,
            bool includePremiumSupport,
            bool useLoyaltyPoints)
        
        {
            RenewalValidator.Walidacja(customerId, planCode, seatCount, paymentMethod);

            string normalizedPlanCode = planCode.Trim().ToUpperInvariant();
            string normalizedPaymentMethod = paymentMethod.Trim().ToUpperInvariant();

            
            var customer = _customerRepository.GetById(customerId);
            var plan = _planRepository.GetByCode(normalizedPlanCode);

            if (!customer.IsActive)
                throw new InvalidOperationException("Inactive customers cannot renew subscriptions");

            
            decimal baseAmount = (plan.MonthlyPricePerSeat * seatCount * 12m) + plan.SetupFee;

            
            var kontekstZnizki = new KontekstZnizki(customer, plan, seatCount, baseAmount, useLoyaltyPoints);
            var discounts = _discountPolicies.Select(p => p.LiczenieZnizki(kontekstZnizki)).ToList();
            decimal discountsTotal = discounts.Sum(d => d.Amount);
            string notes = string.Concat(discounts.Select(d => d.Note));

            decimal subtotal = baseAmount - discountsTotal;
            if (subtotal < 300m)
            {
                subtotal = 300m;
                notes += "minimum discounted subtotal applied; ";
            }

            //Nie, serio, czy chodzi o opłaty za wsparcie techniczne? 
            var supportResult = _supportFeeCalculator.Calculate(normalizedPlanCode, includePremiumSupport);
            notes += supportResult.Note;

            
            var paymentResult = _paymentFeeCalculator.LiczenieMachen(normalizedPaymentMethod, subtotal + supportResult.Fee);
            notes += paymentResult.Note;

            
            decimal taxRate  = _taxRate.GetRate(customer.Country);
            decimal taxBase  = subtotal + supportResult.Fee + paymentResult.Fee;
            decimal taxAmount = taxBase * taxRate;

            decimal finalAmount = taxBase + taxAmount;
            if (finalAmount < 500m)
            {
                finalAmount = 500m;
                notes += "minimum invoice amount applied; ";
            }

            
            var invoice = new RenewalInvoice
            {
                InvoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMdd}-{customerId}-{normalizedPlanCode}",
                CustomerName = customer.FullName,
                PlanCode = normalizedPlanCode,
                PaymentMethod = normalizedPaymentMethod,
                SeatCount = seatCount,
                BaseAmount = Math.Round(baseAmount,     2, MidpointRounding.AwayFromZero),
                DiscountAmount = Math.Round(discountsTotal, 2, MidpointRounding.AwayFromZero),
                SupportFee = Math.Round(supportResult.Fee, 2, MidpointRounding.AwayFromZero),
                PaymentFee = Math.Round(paymentResult.Fee, 2, MidpointRounding.AwayFromZero),
                TaxAmount = Math.Round(taxAmount,      2, MidpointRounding.AwayFromZero),
                FinalAmount = Math.Round(finalAmount,    2, MidpointRounding.AwayFromZero),
                Notes  = notes.Trim(),
                GeneratedAt   = DateTime.UtcNow
            };
            
            _billingGateway.SaveInvoice(invoice);
            _notificationService.SendRenewalConfirmation(customer, invoice);

            return invoice;
        }
    }
}
