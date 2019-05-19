using System;
using System.Collections.Generic;
using System.Text;
using MoneyBroker.Core;
using MoneyBroker.Core.Quote.Providers;
using NUnit.Framework;

namespace MoneyBroker.Tests {
   public class EngineTests {
      const string ExpectedOutputForFullTest = @"
Requested amount: £1000
Rate: 7.0%
Monthly repayment: £30.78
Total repayment: £1108.10
";


      [Test]
      public void Engine_FullTest_OK() {
         var input = new[] { Helpers.GetCsvFileFullPath(), "1000" };

         var service = new QuoteEngine();
         var output = service.GetQuote(input);

         Assert.AreEqual(ExpectedOutputForFullTest, output);
      }

      [Test]
      public void Engine_Validate_WrongFileName_Throws_Error() {
         var input = new[] { "somefile", "1000" };

         Assert.That(() => {
            var service = new QuoteEngine();
            var output = service.GetQuote(input);

         }, Throws.TypeOf<Core.Validation.ValidationException>());
      }

      [Test]
      public void Engine_Validate_InputWrongParams_Throws_Error() {
         var input = new[] { "1000" };

         Assert.That(() => {
            var service = new QuoteEngine();
            var output = service.GetQuote(input);

         }, Throws.TypeOf<Core.Validation.ValidationException>(), QuoteEngine.ErrorWrongParams);
      }

      [Test]
      [TestCase("0")]
      [TestCase("10")]
      [TestCase("1001")]
      [TestCase("1501")]
      [TestCase("15100")]
      [TestCase("not-really-a-number")]
      public void Engine_Validate_InvalidAmmount_Throws_Error(string amount) {
         var input = new[] { Helpers.GetCsvFileFullPath(), amount };

         Assert.That(() => {
            var service = new QuoteEngine();
            var output = service.GetQuote(input);

         }, Throws.TypeOf<Core.Validation.ValidationException>());
      }

      [Test]
      [TestCase("4500")]
      public void Engine_Validate_NotEnoughtMarketMoney_Throws_Error(string amount) {
         var input = new[] { Helpers.GetCsvFileFullPath(), amount };

         Assert.That(() => {
            var service = new QuoteEngine();
            var output = service.GetQuote(input);

         }, Throws.TypeOf<Core.Validation.ValidationException>(), LendQuoteProvider.ErrorUnavailableMarketMoney);
      }
   }
}
