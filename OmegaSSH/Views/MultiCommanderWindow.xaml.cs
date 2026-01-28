using System.Windows;

namespace OmegaSSH.Views;

public partial class MultiCommanderWindow : Window
{
    public MultiCommanderWindow()
    {
        InitializeComponent();
    }

    private void Close_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
}
