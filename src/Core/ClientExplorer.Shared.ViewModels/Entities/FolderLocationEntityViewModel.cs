using ClientExplorer.Application;

namespace ClientExplorer.Shared.ViewModels;

public class FolderLocationEntityViewModel : BaseViewModel
{
  public string FolderName { get; set; } = string.Empty;
  public bool IsCheck { get; set; } = false;
  public bool IsEnable { get; set; } = true;
  public DirectoryEntity FolderDirectory { get; set; }

  public FolderLocationEntityViewModel(string folderName, DirectoryEntity folderDirectory)
  {
    FolderName = folderName;
    FolderDirectory = folderDirectory;
  }
}