namespace ClientExplorer.Application;
using static ClientExplorerApp;

/// <summary>
/// Директории в каталоге клиента 
/// </summary>
public class DirectoriesInClient
{
  /// <summary>
  /// Список директорий в каталоге клиентка
  /// </summary>
  public List<DirectoryEntity> Folders { get; private set; }

  /// <summary>
  /// Генератор директорий в каталоге клиента, по умолчанию
  /// </summary>
  public DirectoriesInClient()
  {
    
    Folders = new List<DirectoryEntity>();
    
    /*
      ./Клиент/Документы общие
      ./Клиент/Каталог стандартных решений
      ./Клиент/Объекты
    */
    
    InitInClient();
    
    Folders.Sort((x, y) => String.Compare(x.Name, y.Name, StringComparison.Ordinal));
  }

  private void InitInClient()
  {
    
    var templatePath = GetTemplateDirectoryPath() + Path.DirectorySeparatorChar + "InClient";
    
    var directoryInfo = new DirectoryInfo(templatePath);
    
    if (!Directory.Exists(templatePath))
    {
      var msg = string.Format("Err: In methods - {0}. {1} - not available...", nameof(DirectoriesInClient),
        nameof(templatePath));
      //TODO: Вывести сообщение об ошибке. Не доступена папка ".ClientExplorer/Assets/InClient".
      return;
    }
    
    foreach (var directory in directoryInfo.GetDirectories())
    {
      //  Точка вначале - символ скрытности. Такие директории пропускать. 
      if (directory.Name.First() != '.')
      {
        
        var directoryEntity = new DirectoryEntity(directory.Name);
        
        directoryEntity.AddSubDirs(directory,directoryEntity);

        Folders.Add(directoryEntity);
      }
    }
    
    Folders.Add(new DirectoryEntity(Settings.FolderObjectsEntityName));
  }
  
}