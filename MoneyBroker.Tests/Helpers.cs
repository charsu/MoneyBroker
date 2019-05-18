using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace MoneyBroker.Tests {
   public static class Helpers {
      public static string GetCsvFileFullPath(string filename = "Market Data for Exercise - csv.csv") {
         var dir = TestContext.CurrentContext.TestDirectory;
         return Path.Combine(dir, @"..\..\..\..\" + filename);
      }
   }
}
