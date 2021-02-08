using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Core.Interfaces;
using CsvHelper;

namespace Core
{
    public class CsvParser : ICsvParser
    {
        public IReadOnlyCollection<TModel> ReadCsvFile<TModel>(string filePath, int startLine, int toTakeCount)
        {
            try
            {
                using var reader = new StreamReader(filePath, Encoding.Default);
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
                
                return csv.GetRecords<TModel>()
                    .Skip(startLine)
                    .Take(toTakeCount)
                    .ToList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}