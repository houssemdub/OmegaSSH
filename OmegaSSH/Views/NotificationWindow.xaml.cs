using System.Windows;

namespace OmegaSSH.Views;

public partial class NotificationWindow : Window
{
    public NotificationWindow(string message, string title = "NOTIFICATION", bool isError = false)
    {
        InitializeComponent();
        MessageLabel.Text = message;
        TitleLabel.Text = title.ToUpper();
        if (isError)
        {
            IconImage.Icon = FontAwesome.WPF.FontAwesomeIcon.ExclamationTriangle;
            IconImage.Foreground = System.Windows.Media.Brushes.Red;
            TitleLabel.Foreground = System.Windows.Media.Brushes.Red;
            this.BorderBrush = System.Windows.Media.Brushes.Red;
        }
    }

    private void Dismiss_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    public static void Show(string message, string title = "NOTIFICATION", bool isError = false)
    {
        var win = new NotificationWindow(message, title, isError);
        win.ShowDialog();
    }
}
