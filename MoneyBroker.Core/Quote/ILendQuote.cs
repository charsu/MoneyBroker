using System;
using System.Collections.Generic;
using System.Text;
using MoneyBroker.Core.Models;

namespace MoneyBroker.Core.Quote {
   public interface ILendQuote {
      QuoteOutputModel GetQuote(decimal loanAmmount, int period = 36);

      (decimal pmt, decimal total) GetRepayment(decimal amount, decimal rate, int period = 36);
   }
}
