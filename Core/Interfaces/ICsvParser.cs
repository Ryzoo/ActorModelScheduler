using System.Collections.Generic;

namespace Core.Interfaces
{
    public interface ICsvParser
    {
        public IReadOnlyCollection<TModel> ReadCsvFile<TModel>(string filePath, int startLine, int toTakeCount);
    }
}