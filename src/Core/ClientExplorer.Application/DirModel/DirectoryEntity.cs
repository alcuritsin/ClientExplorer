namespace ClientExplorer.Application;

public class DirectoryEntity
{
  public DirectoryEntity(string name, List<DirectoryEntity>? childDirs = null)
  {
    Name = name;
    ChildDirs = childDirs;
  }

  public string Name { get; set; }
  public List<DirectoryEntity>? ChildDirs { get; set; }
}