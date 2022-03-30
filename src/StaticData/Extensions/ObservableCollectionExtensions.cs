using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fraxiinus.ReplayBook.StaticData.Extensions
{
    public static class ObservableCollectionExtensions
    {
        public static bool TryAdd(this ObservableCollection<(string key, string value)> collection, string key, string value)
        {
            var existingValue = collection.Where(x => x.key == key)
                .Cast<(string key, string value)?>()
                .FirstOrDefault();
            
            if (existingValue != null)
            {
                return false;
            }
            else
            {
                collection.Add((key, value));
                return true;
            }
        }
    }
}
