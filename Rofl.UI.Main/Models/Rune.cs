using Rofl.UI.Main.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rofl.UI.Main.Models
{
    public class Rune : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Rune(int order, string id, string firstValue, string secondValue, string thirdValue)
        {
            GridPosition = order;
            RuneId = id;
            RuneName = RuneHelper.GetRune(id).Name;
            Value0 = int.TryParse(firstValue, out int parsedValue0) ? parsedValue0 : 0;
            Value1 = int.TryParse(secondValue, out int parsedValue1) ? parsedValue1 : 0;
            Value2 = int.TryParse(thirdValue, out int parsedValue2) ? parsedValue2 : 0;
        }

        public int GridPosition { get; set; }

        public string RuneId { get; set; }

        public string RuneName { get; set; }

        public int Value0 { get; set; }

        public int Value1 { get; set; }

        public int Value2 { get; set; }
    }
}
