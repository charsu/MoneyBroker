using System;
using System.Collections.Generic;
using System.Text;
using MoneyBroker.Core.Models;

namespace MoneyBroker.Core {
   public interface ILenderProvider {
      void Init(string file);
      List<LenderModel> Lenders { get; }
   }
}
