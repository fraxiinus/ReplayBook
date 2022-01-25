using ModernWpf.Controls;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Rofl.UI.Main.Utilities
{
    public static class FlyoutHelper
    {
        /// <summary>
        /// Creates a Flyout with parameters.
        /// </summary>
        /// <param name="includeButton"></param>
        /// <param name="includeCustom"></param>
        /// <returns></returns>
        public static Flyout CreateFlyout(bool includeButton = true,
                                          bool includeCustom = false)
        {
            #region Grid Definitions
            var contentPanel = new Grid();

            var columnOne = new ColumnDefinition
            {
                Width = new GridLength(1, GridUnitType.Star)
            };
            var columnTwo = new ColumnDefinition
            {
                Width = new GridLength(1, GridUnitType.Auto)
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
            contentPanel.ColumnDefinitions.Add(columnTwo);
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

            // if there is a custom item at (0, 1) span label across two columns
            if (includeCustom)
            {
                label.Margin = new Thickness(0, 0, 0, 12);
                Grid.SetColumnSpan(label, 2);
            }

            var button = new Button
            {
                Name = "PrimaryButton",
                Margin = new Thickness(12, 0, 0, 0),
                IsDefault = true // default makes the button accented + enter key
            };
            Grid.SetColumn(button, 1);
            if (includeCustom) { Grid.SetRow(button, 1); }
            else { Grid.SetRow(button, 0); }

            // Include the controls to the flyout grid
            _ = contentPanel.Children.Add(label);
            if (includeButton) { _ = contentPanel.Children.Add(button); }

            // Apply the template so that the visual tree is built
            _ = contentPanel.ApplyTemplate();

            return new Flyout
            {
                Content = contentPanel
            };
        }

        /// <summary>
        /// Only works for flyouts created by <see cref="FlyoutHelper"/>
        /// </summary>
        /// <param name="flyout"></param>
        /// <returns></returns>
        public static TextBlock GetFlyoutLabel(this Flyout flyout)
        {
            return flyout == null
                ? throw new ArgumentNullException(nameof(flyout))
                : flyout.Content is not Grid content
                    ? null
                    : LogicalTreeHelper.FindLogicalNode(content, "LabelTextBlock") as TextBlock;
        }

        /// <summary>
        /// Only works for flyouts created by <see cref="FlyoutHelper"/>
        /// </summary>
        /// <param name="flyout"></param>
        /// <param name="text"></param>
        public static void SetFlyoutLabelText(this Flyout flyout, string text)
        {
            if (flyout == null) { throw new ArgumentNullException(nameof(flyout)); }

            if (flyout.Content is not Grid content) { throw new ArgumentException("Flyout content is not as expected"); }

            (LogicalTreeHelper.FindLogicalNode(content, "LabelTextBlock") as TextBlock).Text = text;
        }

        /// <summary>
        /// Only works for flyouts created by <see cref="FlyoutHelper"/>
        /// </summary>
        /// <param name="flyout"></param>
        /// <returns></returns>
        public static Button GetFlyoutButton(this Flyout flyout)
        {
            return flyout == null
                ? throw new ArgumentNullException(nameof(flyout))
                : flyout.Content is not Grid content
                    ? null
                    : LogicalTreeHelper.FindLogicalNode(content, "PrimaryButton") as Button;
        }

        /// <summary>
        /// Only works for flyouts created by <see cref="FlyoutHelper"/>
        /// </summary>
        /// <param name="flyout"></param>
        /// <param name="text"></param>
        public static void SetFlyoutButtonText(this Flyout flyout, string text)
        {
            if (flyout == null) { throw new ArgumentNullException(nameof(flyout)); }
            if (flyout.Content is not Grid content) { throw new ArgumentException("Flyout content is not as expected"); }

            (LogicalTreeHelper.FindLogicalNode(content, "PrimaryButton") as Button).Content = text;
        }
    }
}
