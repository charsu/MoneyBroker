using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Autofac.Extras.Moq;
using MoneyBroker.Core;
using MoneyBroker.Core.Models;
using MoneyBroker.Core.Quote;
using NUnit.Framework;

namespace MoneyBroker.Tests {
   public static class Helpers {
      public static string GetCsvFileFullPath(string filename = "Market Data for Exercise-csv.csv") {
         var dir = TestContext.CurrentContext.TestDirectory;
         return Path.Combine(dir, @"..\..\..\..\" + filename);
      }

      public static AutoMock SetupLenderProvider(this AutoMock mock, List<LenderModel> lenderModels = null) {
         var source = (lenderModels ?? new List<LenderModel>())
            .OrderBy(x => x.Rate)
            .ToList();

         var m = mock.Mock<ILenderProvider>();
         m.SetupGet(x => x.Lenders)
            .Returns(source);

         return mock;
      }
   }
}
