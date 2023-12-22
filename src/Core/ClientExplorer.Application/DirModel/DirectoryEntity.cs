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