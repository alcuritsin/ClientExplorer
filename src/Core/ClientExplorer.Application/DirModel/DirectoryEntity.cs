namespace ClientExplorer.Application;

public class DirectoryEntity
{
  public DirectoryEntity(string name, List<DirectoryEntity>? childDirs = null)
  {
    Name = name;
    ChildDirs = childDirs;

    if (childDirs != null)
    {
      foreach (var childDir in childDirs)
      {
        childDir.ParentDirectory = this;
      }
    }
  }

  public string GetDirectoryPath()
  {
    return GetDirectoryFrom(this);
  }

  private string GetDirectoryFrom(DirectoryEntity directoryEntity)
  {
    var result = Path.DirectorySeparatorChar + directoryEntity.Name;
    
    if (directoryEntity.ParentDirectory == null)
    {
      return result;
    }

    return  GetDirectoryFrom(directoryEntity.ParentDirectory) + result;
  }

  public string Name { get; set; }
  public DirectoryEntity? ParentDirectory { get; set; }
  public List<DirectoryEntity>? ChildDirs { get; set; }
}