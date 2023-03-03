namespace ClientExplorer.Shared.ViewModels;

public class FolderLocationEntityViewModel : BaseViewModel
{
  public string FolderName { get; set; } = string.Empty;
  public bool IsCheck { get; set; } = false;
  public bool IsEnable { get; set; } = true;

  public FolderLocationEntityViewModel(string folderName)
  {
    FolderName = folderName;
  }
}