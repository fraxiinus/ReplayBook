namespace Fraxiinus.ReplayBook.UI.Main.Pages;

using System;

public interface IWelcomePage
{
    string GetTitle();

    Type GetNextPage();

    Type GetPreviousPage();
}
