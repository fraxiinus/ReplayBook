using ModernWpf.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Rofl.UI.Main.Utilities
{
    public class FlyoutHelper
    {
        public Flyout Flyout { get; private set; }
        public Button Button { get; private set; }
        public TextBlock TextBlock { get; private set; }

        public FlyoutHelper(bool includeButton = true)
        {
            // Create button that does the actual delete
            Button = new Button();

            // Create textblock
            TextBlock = new TextBlock();

            // Add items to a panel
            var contentPanel = new SimpleStackPanel
            {
                Spacing = 12
            };
            contentPanel.Children.Add(TextBlock);

            if (includeButton) contentPanel.Children.Add(Button);

            // Add the grid into a flyout
            Flyout = new Flyout
            {
                Content = contentPanel
            };
        }
    }
}
