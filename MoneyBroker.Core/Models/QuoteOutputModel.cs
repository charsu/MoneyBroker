using System;
using System.Collections.Generic;
using System.Text;

namespace MoneyBroker.Core.Models {
   public class QuoteOutputModel {
      public decimal RequestAmount { get; set; }
      public decimal RateAsPercent { get; set; }
      public decimal MounthlyRepayment { get; set; }
      public decimal TotalRepayment { get; set; }
   }
}
