namespace Fraxiinus.ReplayBook.UI.Main.Views;

using Fraxiinus.ReplayBook.Configuration.Models;
using Fraxiinus.ReplayBook.UI.Main.Utilities;
using ModernWpf.Controls;
using System.Windows.Controls;

/// <summary>
/// Interaction logic for AddReplayCategoryDialog.xaml
/// </summary>
public partial class AddReplayCategoryDialog : ContentDialog
{
    public AddReplayCategoryDialog()
    {
        InitializeComponent();
    }

    public AddReplayCategoryDialog(CategoryItem editSource)
    {
        InitializeComponent();

        Title = ResourceTools.GetObjectFromResource<string>("EditCategory__DialogTitle");
        PrimaryButtonText = ResourceTools.GetObjectFromResource<string>("SaveButtonText");

        NameInputBox.Text = editSource.Name;
        DescInputBox.Text = editSource.Description;
        SearchInputBox.Text = editSource.SearchTerm;
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
