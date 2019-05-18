using System;
using System.Collections.Generic;
using System.Text;
using MoneyBroker.Core.Models;

namespace MoneyBroker.Core {
   public interface ILenderProvider {
      void Init(string file);

      /// <summary>
      /// provides a list of lenders ordered by the rates (asc)
      /// </summary>
      List<LenderModel> Lenders { get; }
   }
}
