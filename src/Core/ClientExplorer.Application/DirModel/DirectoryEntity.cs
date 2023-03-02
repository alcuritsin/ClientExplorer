namespace ClientExplorer.Application;

public class DirectoryEntity
{
  public DirectoryEntity(string dirName, DirectoryEntity? parentDir = null, List<DirectoryEntity>? childDirs = null)
  {
    DirName = dirName;
    ParentDir = parentDir;
    if (ChildDirs != null)
    {
      ChildDirs = childDirs;
    }
    
  }

  public string DirName { get; set; }
  public DirectoryEntity? ParentDir { get; set; }
  public List<DirectoryEntity>? ChildDirs { get; set; }

}