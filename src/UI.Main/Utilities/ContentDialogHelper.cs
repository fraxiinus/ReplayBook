using ModernWpf.Controls;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Fraxiinus.ReplayBook.UI.Main.Utilities
{
    public static class ContentDialogHelper
    {
        public static ContentDialog CreateContentDialog(string title,
                                                        string description,
                                                        string primaryButtonText,
                                                        string secondaryButtonText = null,
                                                        double labelWidth = 300) 
        {
            // include secondary button if secondary buton text is provided
            var dialog = CreateContentDialog(secondaryButtonText != null);

            // set properties
            dialog.Title = title;
            dialog.PrimaryButtonText = primaryButtonText;
            dialog.SecondaryButtonText = secondaryButtonText;
            dialog.SetLabelText(description);
            dialog.GetContentDialogLabel().TextWrapping = TextWrapping.Wrap;
            dialog.GetContentDialogLabel().Width = labelWidth;
            dialog.DefaultButton = ContentDialogButton.Primary;

            return dialog;
        }

        /// <summary>
        /// Creates a <see cref="ContentDialog"/> using the parameters
        /// </summary>
        /// <param name="includeCloseButton"></param>
        /// <returns></returns>
        public static ContentDialog CreateContentDialog(bool includeSecondaryButton = false)
        {
            #region Grid Definitions
            var contentPanel = new Grid();

            var columnOne = new ColumnDefinition
            {
                Width = new GridLength(1, GridUnitType.Star)
            };

            var rowOne = new RowDefinition
            {
                Height = new GridLength(1, GridUnitType.Auto)
            };
            var rowTwo = new RowDefinition
            {
                Height = new GridLength(1, GridUnitType.Star)
            };

            contentPanel.ColumnDefinitions.Add(columnOne);
            contentPanel.RowDefinitions.Add(rowOne);
            contentPanel.RowDefinitions.Add(rowTwo);
            #endregion

            var label = new TextBlock
            {
                Name = "LabelTextBlock",
                VerticalAlignment = VerticalAlignment.Center,
                Padding = new Thickness(0, 0, 0, 0)
            };
            Grid.SetRow(label, 0);
            Grid.SetColumn(label, 0);

            _ = contentPanel.Children.Add(label);

            var dialog = new ContentDialog
            {
                Content = contentPanel,
                IsPrimaryButtonEnabled = true,
                IsSecondaryButtonEnabled = includeSecondaryButton,
                Background = Application.Current.FindResource("TabBackground") as Brush
            };

            _ = dialog.ApplyTemplate();
            return dialog;
        }

        /// <summary>
        /// Only works for <see cref="ContentDialog"/>s made by <see cref="ContentDialogHelper"/>
        /// </summary>
        /// <param name="dialog"></param>
        /// <returns></returns>
        public static TextBlock GetContentDialogLabel(this ContentDialog dialog)
        {
            return dialog == null
                ? throw new ArgumentNullException(nameof(dialog))
                : dialog.Content is not Grid content
                    ? null
                    : LogicalTreeHelper.FindLogicalNode(content, "LabelTextBlock") as TextBlock;
        }

        public static void SetCustomObject(this ContentDialog dialog, UIElement customObject)
        {
            if (dialog.Content is not Grid contentPanel) { return; }

            Grid.SetRow(customObject, 1);
            Grid.SetColumn(customObject, 0);

            contentPanel.Children.Add(customObject);
        }

        /// <summary>
        /// Only works for <see cref="ContentDialog"/>s made by <see cref="ContentDialogHelper"/>
        /// </summary>
        /// <param name="dialog"></param>
        /// <param name="text"></param>
        public static void SetLabelText(this ContentDialog dialog, string text)
        {
            if (dialog == null) { throw new ArgumentNullException(nameof(dialog)); }
            if (dialog.Content is not Grid content) { throw new ArgumentException("ContentDialog content is not as expected"); }

            (LogicalTreeHelper.FindLogicalNode(content, "LabelTextBlock") as TextBlock).Text = text;
        }

        /// <summary>
        /// Set the overlay smoke color and appears behind the dialog.
        /// </summary>
        /// <param name="dialog"></param>
        /// <param name="color"></param>
        public static void SetBackgroundSmokeColor(this ContentDialog dialog, Brush color)
        {
            if (dialog == null) { throw new ArgumentNullException(nameof(dialog)); }

            Grid root = WindowHelper.FindVisualChildren<Grid>(dialog)
                                    .First(x => x.Name.Equals("LayoutRoot", StringComparison.OrdinalIgnoreCase));

            root.Background = color;
        }
    }
}
