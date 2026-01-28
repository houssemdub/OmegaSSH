using OmegaSSH.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace OmegaSSH.Views;

public partial class SftpView : UserControl
{
    public SftpView()
    {
        InitializeComponent();
    }

    private void FilesList_DragOver(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            e.Effects = DragDropEffects.Copy;
        }
        else
        {
            e.Effects = DragDropEffects.None;
        }
        e.Handled = true;
    }

    private async void FilesList_Drop(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (DataContext is SftpViewModel vm)
            {
                await vm.UploadFilesCommand.ExecuteAsync(files);
            }
        }
    }
}
