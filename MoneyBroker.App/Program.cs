using System;
using MoneyBroker.Core;
using MoneyBroker.Core.Validation;

namespace MoneyBrokerQuote.App {
   class Program {
      static void Main(string[] args) {
         try {
            var service = new QuoteEngine();
            var output = service.GetQuote(args);

            Console.WriteLine(output);
         }
         catch (ValidationException vex) {
            Console.WriteLine(vex.Message);
         }
         catch (Exception ex) {
            Console.WriteLine($"We encounter an error while trying to get your quote: {ex.Message}");
         }
      }
   }
}
