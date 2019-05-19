using System;
using System.Collections.Generic;
using System.Text;
using Autofac.Extras.Moq;
using MoneyBroker.Core.Models;
using MoneyBroker.Core.Quote.Providers;
using NUnit.Framework;

namespace MoneyBroker.Tests {
   public class LendQuoteTests {

      private AutoMock GetMock()
        => AutoMock.GetLoose()
            .SetupLenderProvider();

      [Test]
      public void LendQuote_GetRepayment_OK() {
         var service = GetMock().Create<LendQuoteProvider>();

         var (m, t) = service.GetRepayment(1000, 0.07003999999999999M);

         Assert.AreEqual(30.7805943855424m, m);
         Assert.AreEqual(1108.10139787953m, t);
      }

      [Test]
      public void LendQuote_GetBestRate_OK() {
         var lenders = new List<LenderModel>() {
            new LenderModel(){ Available = 600, Rate =4 },
            new LenderModel(){ Available = 300, Rate =5 },
            new LenderModel(){ Available = 10000, Rate =6 },
         };

         var service = GetMock()
            .SetupLenderProvider(lenders)
            .Create<LendQuoteProvider>();

         var rate = service.GetBestRate(1000);

         Assert.AreEqual(4.5M, rate);
      }
   }
}
