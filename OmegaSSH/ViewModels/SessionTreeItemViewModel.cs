using CommunityToolkit.Mvvm.ComponentModel;
using OmegaSSH.Models;
using System.Collections.ObjectModel;

namespace OmegaSSH.ViewModels;

public partial class SessionTreeItemViewModel : ObservableObject
{
    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private bool _isFolder;

    [ObservableProperty]
    private SessionModel? _session;

    [ObservableProperty]
    private ObservableCollection<SessionTreeItemViewModel> _children = new();

    public SessionTreeItemViewModel(string name, bool isFolder, SessionModel? session = null)
    {
        _name = name;
        _isFolder = isFolder;
        _session = session;
    }
}
