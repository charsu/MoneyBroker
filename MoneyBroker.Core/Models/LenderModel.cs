using System;
using System.Collections.Generic;
using System.Text;

namespace MoneyBroker.Core.Models {
   public class LenderModel {
      public string Lender { get; set; }
      public decimal Rate { get; set; }
      public decimal Available { get; set; }
   }
}
