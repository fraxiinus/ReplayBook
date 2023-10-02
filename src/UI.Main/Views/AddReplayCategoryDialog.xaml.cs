namespace Fraxiinus.ReplayBook.UI.Main.Views;

using Fraxiinus.ReplayBook.UI.Main.Utilities;
using ModernWpf.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

/// <summary>
/// Interaction logic for AddReplayCategoryDialog.xaml
/// </summary>
public partial class AddReplayCategoryDialog : ContentDialog
{
    public AddReplayCategoryDialog()
    {
        InitializeComponent();
    }

    private bool RequiredFieldsFilled()
    {
        if (!string.IsNullOrEmpty(NameInputBox.Text) && !string.IsNullOrEmpty(DescInputBox.Text) && !string.IsNullOrEmpty(SearchInputBox.Text))
        {
            return true;
        }
        return false;
    }

    private void NameInputBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (RequiredFieldsFilled())
        {
            IsPrimaryButtonEnabled = true;
        }
        else
        {
            IsPrimaryButtonEnabled = false;
        }
    }

    private void DescInputBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (RequiredFieldsFilled())
        {
            IsPrimaryButtonEnabled = true;
        }
        else
        {
            IsPrimaryButtonEnabled = false;
        }
    }

    private void SearchInputBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (RequiredFieldsFilled())
        {
            IsPrimaryButtonEnabled = true;
        }
        else
        {
            IsPrimaryButtonEnabled = false;
        }
    }
}
