using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Fraxiinus.ReplayBook.UI.Main.Controls.akr
{

    public class CirclePanel : Panel
    {
        static CirclePanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CirclePanel), new FrameworkPropertyMetadata(typeof(Panel)));
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (InternalChildren.Count == 0)
                return availableSize;

            var children = InternalChildren.Cast<UIElement>().ToList();
            foreach (var child in children)
            {
                child.Measure(availableSize);
            }

            var maxChildWidth = children.Select(child => child.DesiredSize.Width).Max();
            var maxChildHeight = children.Select(child => child.DesiredSize.Height).Max();

            //内円の円周 
            var circumference = maxChildWidth * children.Count;

            //外円の半径 l = 2 * PI * r --> r = l / (PI * 2)
            double r = (circumference / Math.PI * 2) + Math.Max(maxChildWidth, maxChildHeight);

            var result = new Size(r * 2, r * 2);
            if (double.IsInfinity(availableSize.Width) == false && availableSize.Width < result.Height)
                result.Width = availableSize.Width;

            if (double.IsInfinity(availableSize.Height) == false && availableSize.Height < result.Height)
                result.Height = availableSize.Height;
            return result;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (InternalChildren.Count == 0)
                return finalSize;
            
            var firstRect = finalSize.Width > finalSize.Height ?
                new Rect((finalSize.Width - finalSize.Height) / 2, 0, finalSize.Height, finalSize.Height) :
                new Rect(0, (finalSize.Height - finalSize.Width) / 2, finalSize.Width, finalSize.Width);

            var angleRange = 360.0 / InternalChildren.Count;
            var angle = 0.0;

            foreach (UIElement child in InternalChildren)
            {
                var childLocation = new Point(firstRect.Left + (firstRect.Width - child.DesiredSize.Width) / 2, firstRect.Top);
                child.RenderTransform = new RotateTransform(angle, child.DesiredSize.Width / 2, finalSize.Height / 2 - firstRect.Top);
                child.Arrange(new Rect(childLocation, child.DesiredSize));
                angle += angleRange;
            }
            return finalSize;
        }
    }
}
