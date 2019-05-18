using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MoneyBroker.Core.Models;

namespace MoneyBroker.Core.Quote.Providers {
   public class LendQuoteProvider : ILendQuote {
      private readonly ILenderProvider _lenderProvider;

      public LendQuoteProvider(ILenderProvider lenderProvider) {
         _lenderProvider = lenderProvider;
      }

      public QuoteOutputModel GetQuote(decimal amount, int period = 36) {
         //validation 
         // check if ammount is between 1000 - 15000
         // check ammount is % 100 
         // checked amount is not bigger then the total.

         var rate = GetBestRate(amount);
         var (montlyRepaymentRate, totalRepayment) = GetRepayment(amount, rate, period);

         return new QuoteOutputModel() {
            RequestAmount = amount,
            Rate = rate,
            MounthlyRepayment = montlyRepaymentRate,
            TotalRepayment = totalRepayment
         };
      }

      public decimal GetBestRate(decimal loanAmmount) {
         //determine the best rate and if we can load that much money
         // note : they are already ordered by the rates and we make a copy so that we dont mutated the original
         var lenders = new Queue<LenderModel>(_lenderProvider.Lenders);
         decimal amount = 0;

         var pot = new List<(decimal rate, decimal amount)>();

         while (lenders.Count > 0 && amount < loanAmmount) {
            var lender = lenders.Dequeue();
            //make sure we only take as much as we need 
            var lamount = amount + lender.Available > loanAmmount ? loanAmmount - amount : lender.Available;
            amount += lamount;
            pot.Add((lender.Rate, lamount));
         }

         // get the weighted avg 
         decimal weightedValueSum = pot.Sum(x => x.rate * (x.amount * 100 / loanAmmount));
         decimal weightSum = pot.Sum(x => (x.amount * 100 / loanAmmount));

         return weightedValueSum / weightSum;
      }

      public (decimal pmt, decimal total) GetRepayment(decimal amount, decimal rate, int period = 36) {
         // PMT = PV(r / n)(1 + r / n) ^{ nt} / [(1 + r / n) ^{ nt} -1]
         decimal pv = amount;
         decimal r = rate;
         decimal n = 12; // times compounded (eg once every year)

         var rn = r / n;
         var pow = (decimal)Math.Pow((double)(1 + rn), (double)(period));
         var pmt = Math.Round(pv * rn * pow / (pow - 1), 2);
         var total = Math.Round(pmt * period, 2);

         return (pmt, total);
      }
   }
}
