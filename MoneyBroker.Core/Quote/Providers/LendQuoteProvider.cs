using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MoneyBroker.Core.Models;

namespace MoneyBroker.Core.Quote.Providers {
   public class LendQuoteProvider : ILendQuote {
      public const int MinAmount = 1000;
      public const int MaxAmount = 15000;
      public const int AmountStep = 100;

      public const string ErrorAmountOutsideBounds = "the amount needs to be within the interval [1000, 15000].";
      public const string ErrorAmountBadStep = "the amount needs to be an increment of 100.";
      public const string ErrorUnavailableMarketMoney = "the market does not have sufficient offers from lenders to satisfy the requested loan.";

      private readonly ILenderProvider _lenderProvider;

      public LendQuoteProvider(ILenderProvider lenderProvider) {
         _lenderProvider = lenderProvider;
      }

      public QuoteOutputModel GetQuote(decimal amount, int period = 36) {

         #region validation 
         // check if ammount is between 1000 - 15000
         if (amount < MinAmount || amount > MaxAmount) {
            throw new Validation.ValidationException(ErrorAmountOutsideBounds);
         }
         // check ammount is % 100 
         if (amount % AmountStep != 0) {
            throw new Validation.ValidationException(ErrorAmountBadStep);
         }
         // checked amount is not bigger then the total.
         if (_lenderProvider.Lenders.Sum(x => x.Available) < amount) {
            throw new Validation.ValidationException(ErrorUnavailableMarketMoney);
         }
         #endregion // validation

         var rate = GetBestRate(amount);
         var (montlyRepaymentRate, totalRepayment) = GetRepayment(amount, rate, period);

         return new QuoteOutputModel() {
            RequestAmount = amount,
            RateAsPercent = rate * 100,
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
            var lamount = Math.Min(loanAmmount - amount, lender.Available);
            amount += lamount;
            pot.Add((lender.Rate, lamount));
         }

         // get the weighted avg 
         decimal weightedValueSum = pot.Sum(x => x.rate * (x.amount * 100 / loanAmmount));
         decimal weightSum = pot.Sum(x => (x.amount * 100 / loanAmmount));

         return weightedValueSum / weightSum;
      }

      /// <summary>
      /// calculates the monthly payment and total amount to be paid.
      /// see : http://www.financeformulas.net/Loan_Payment_Formula.html
      /// </summary>
      /// <param name="amount"></param>
      /// <param name="rate"></param>
      /// <param name="period"></param>
      /// <returns></returns>
      public (decimal pmt, decimal total) GetRepayment(decimal amount, decimal rate, int period = 36) {
         var r = GetMonthlyInterestRate(rate);
         var p = (double)amount * r / (1 - Math.Pow(1 + r, -period));
         var t = p * period;
         return ((decimal)p, (decimal)t);
      }

      /// <summary>
      ///  - https://www.experiglot.com/2006/06/07/how-to-convert-from-an-annual-rate-to-an-effective-periodic-rate-javascript-calculator/
      ///  - https://www.engineeringtoolbox.com/effective-nominal-interest-rates-d_1468.html
      /// </summary>
      /// <param name="annualInterestRate"></param>
      /// <returns></returns>
      public double GetMonthlyInterestRate(decimal annualInterestRate)
      => Math.Pow((double)(1 + annualInterestRate), 1.0D / 12D) - 1;

   }
}
