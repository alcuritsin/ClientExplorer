using ClientExplorer.Application;

namespace ClientExplorer.Shared.ViewModels;

public class FolderLocationEntityViewModel : BaseViewModel
{
  public string FolderName { get; set; }
  public bool IsCheck { get; set; }
  public bool IsEnable { get; set; } = true;
  public DirectoryEntity FolderDirectory { get; }

  public FolderLocationEntityViewModel(string folderName, DirectoryEntity folderDirectory)
  {
    FolderName = folderName;
    FolderDirectory = folderDirectory;
  }
}