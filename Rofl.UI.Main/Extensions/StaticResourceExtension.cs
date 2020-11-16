using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rofl.UI.Main.Extensions
{
    /// <summary>
    /// ModernWPF by https://github.com/Kinnara/
    /// https://github.com/Kinnara/ModernWpf/blob/master/ModernWpf/Markup/StaticResourceExtension.cs
    /// </summary>
    public class StaticResourceExtension : System.Windows.StaticResourceExtension
    {
        public StaticResourceExtension()
        {
        }

        public StaticResourceExtension(object resourceKey) : base(resourceKey)
        {
        }
    }
}
