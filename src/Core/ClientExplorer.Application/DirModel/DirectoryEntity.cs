namespace ClientExplorer.Application;

public class DirectoryEntity
{
  public DirectoryEntity(string dirName, DirectoryEntity? parentDir = null)
  {
    DirName = dirName;
    ParentDir = parentDir;
  }

  public string DirName { get; set; }
  public DirectoryEntity? ParentDir { get; set; }

}