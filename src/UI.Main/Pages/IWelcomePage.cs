using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fraxiinus.ReplayBook.UI.Main.Pages
{
    public interface IWelcomePage
    {
        string GetTitle();

        Type GetNextPage();

        Type GetPreviousPage();
    }
}
