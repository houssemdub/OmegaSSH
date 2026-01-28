using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OmegaSSH.Models;
using System;
using System.Windows;

namespace OmegaSSH.ViewModels;

public partial class SnippetEditViewModel : ObservableObject
{
    [ObservableProperty] private string _name;
    [ObservableProperty] private string _command;
    [ObservableProperty] private string _description;

    public SnippetModel Result { get; private set; }
    public bool IsSaved { get; private set; }

    public SnippetEditViewModel(SnippetModel? existing = null)
    {
        if (existing != null)
        {
            Result = existing;
            _name = existing.Name;
            _command = existing.Command;
            _description = existing.Description ?? string.Empty;
        }
        else
        {
            Result = new SnippetModel();
            _name = "New Snippet";
            _command = "";
            _description = "";
        }
    }

    [RelayCommand]
    private void Save(Window window)
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            MessageBox.Show("Please enter a name for the snippet.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        Result.Name = Name;
        Result.Command = Command;
        Result.Description = Description;
        
        IsSaved = true;
        window.DialogResult = true;
        window.Close();
    }

    [RelayCommand]
    private void Cancel(Window window)
    {
        window.DialogResult = false;
        window.Close();
    }
}
