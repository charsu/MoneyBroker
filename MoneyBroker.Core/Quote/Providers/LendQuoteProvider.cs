﻿using System;
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
         return ((decimal)Math.Round(p, 2), (decimal)Math.Round(t, 2));
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
