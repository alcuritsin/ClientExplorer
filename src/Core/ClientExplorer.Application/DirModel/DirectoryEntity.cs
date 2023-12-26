using System.Data;

namespace ClientExplorer.Application;

/// <summary>
/// Сущность директории
/// </summary>
public class DirectoryEntity
{
  /// <summary>
  /// Создание сущности директории
  /// </summary>
  /// <param name="name">Наименование</param>
  /// <param name="childDirs">Список поддиректорий</param>
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

  /// <summary>
  /// Возвращает путь до директории
  /// </summary>
  /// <returns>Путь</returns>
  public string GetDirectoryPath()
  {
    return GetDirectoryFrom(this);
  }

  private void AddChildDir(DirectoryEntity childDir)
  {
    childDir.ParentDirectory = this;
    
    var directoryEntities = this.ChildDirs;
    
    if (directoryEntities != null)
    {
      directoryEntities.Add(childDir);
    }
    else
    {
      this.ChildDirs = new List<DirectoryEntity>();
      this.ChildDirs.Add(childDir);
    }
    
  }

  /// <summary>
  /// Добавляет поддиректории из шаблона, если они есть. 
  /// </summary>
  /// <param name="root">Путь до каталога</param>
  /// <param name="parent">Сущность каталога в программе</param>
  public void AddSubDirs(DirectoryInfo root, DirectoryEntity parent)
  {
    var subDirs = root.GetDirectories();
    
    foreach (var directory in subDirs)
    {
      var directoryEntity = new DirectoryEntity(directory.Name);
      
      parent.AddChildDir(directoryEntity);
      
      AddSubDirs(directory, directoryEntity);            
    }
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

  /// <summary>
  /// Наименование сущности
  /// </summary>
  public string Name { get; set; }
  /// <summary>
  /// Родительская сущность
  /// Может быть пустой. Тогда это папка высшего уровня вложенности
  /// </summary>
  public DirectoryEntity? ParentDirectory { get; set; }
  /// <summary>
  /// Список поддиректорий
  /// Может быть пустой. Тогда текущая сущность не имеет вложенных подкаталогов
  /// </summary>
  public List<DirectoryEntity>? ChildDirs { get; set; }
}