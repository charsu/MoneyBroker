using System.Linq;
using Autofac;
using Autofac.Core;
using Autofac.Extras.Moq;
using MoneyBroker.Core.DataLayer.Providers;
using NUnit.Framework;

namespace MoneyBroker.Tests {
   public class Tests {
      private AutoMock GetMock()
         => AutoMock.GetLoose();

      [Test]
      public void LenderProvider_LoadCsv_OK() {
         var csv = Helpers.GetCsvFileFullPath();

         var service = GetMock().Create<CsvLenderProvider>(new NamedParameter("filename", csv));
         var lenders = service.Lenders;

         //asserts
         Assert.AreEqual(7, lenders.Count);
         Assert.IsTrue(lenders.All(x => !string.IsNullOrEmpty(x.Lender)));
         Assert.IsTrue(lenders.All(x => x.Available > 0));
         Assert.IsTrue(lenders.All(x => x.Rate > 0));
      }
   }
}