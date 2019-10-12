using Rofl.Reader.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rofl.UI.Main.Models
{
    public class PlayerDetailModel : INotifyPropertyChanged
    {
        public PlayerDetailModel(Player player, PlayerPreviewModel previewModel)
        {
            if (player == null) { throw new ArgumentNullException(nameof(player)); }

            PreviewModel = previewModel ?? throw new ArgumentNullException(nameof(previewModel));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public PlayerPreviewModel PreviewModel { get; set; }
    }
}
