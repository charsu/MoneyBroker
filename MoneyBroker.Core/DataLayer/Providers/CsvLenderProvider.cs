using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using MoneyBroker.Core.Models;

namespace MoneyBroker.Core.DataLayer.Providers {
   public class CsvLenderProvider : ILenderProvider {
      public CsvLenderProvider() {
      }

      public CsvLenderProvider(string filename) : this() {
         Init(filename);
      }

      public List<LenderModel> Lenders { get; private set; } = new List<LenderModel>();

      private Configuration GetConfiguration()
         => new Configuration() {
            Delimiter = ",",
            HasHeaderRecord = true
         };

      public void Init(string file) {
         if (!File.Exists(file)) {
            throw new Validation.ValidationException($"could not locate file specified : {Path.GetFullPath(file)}");
         }

         using (var reader = new StreamReader(file))
         using (var csv = new CsvReader(reader, GetConfiguration())) {
            Lenders = csv.GetRecords<LenderModel>()?.ToList()?.OrderBy(x => x.Rate).ToList() ?? new List<LenderModel>();
         }
      }
   }
}
