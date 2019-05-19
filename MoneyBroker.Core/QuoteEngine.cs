using System;
using System.Collections.Generic;
using System.Text;
using MoneyBroker.Core.DataLayer.Providers;
using MoneyBroker.Core.Quote.Providers;
using MoneyBroker.Core.Validation;

namespace MoneyBroker.Core {
   public class QuoteEngine {
      public const string UsageInstructions = "[application] [market_file] [loan_amount]";
      public readonly static string ErrorWrongParams = $"invalid parameters: {UsageInstructions}";

      public const string OutputTemplate = @"
Requested amount: £{0:0}
Rate: {1:0.0}%
Monthly repayment: £{2:0.00}
Total repayment: £{3:0.00}
";

      public string GetQuote(string[] args) {
         // validation phase
         if ((args?.Length ?? 0) != 2) {
            throw new ValidationException(ErrorWrongParams);
         }

         var fileName = args[0];
         int.TryParse(args[1], out var amount);

         var service = new LendQuoteProvider(new CsvLenderProvider(fileName));
         var o = service.GetQuote(amount);

         // format the output as expected
         return string.Format(OutputTemplate,
            o.RequestAmount, o.RateAsPercent, o.MounthlyRepayment, o.TotalRepayment);
      }
   }
}
