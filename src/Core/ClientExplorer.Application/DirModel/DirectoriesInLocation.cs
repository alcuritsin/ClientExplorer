namespace ClientExplorer.Application;
using static ClientExplorerApp;

/// <summary>
/// Директории в каталоге локации 
/// </summary>
public class DirectoriesInLocation
{
  /// <summary>
  /// Список директорий в каталоге локации
  /// </summary>
  public List<DirectoryEntity> Folders { get; private set; }

  /// <summary>
  /// Генератор директорий в каталоге локации, по умолчанию
  /// </summary>
  public DirectoriesInLocation()
  {
    
    Folders = new List<DirectoryEntity>();
    
    InitLocationFolders();
    
    Folders.Sort((x, y) => String.Compare(x.Name, y.Name, StringComparison.Ordinal));
    
  }

  private void InitLocationFolders()
  {
    
      #region ./Клиент/Объекты/Город/Адрес, Дом, ТЦ/

        var templatePath = GetTemplateDirectoryPath() + Path.DirectorySeparatorChar + "InLocation";
        
        var directoryInfo = new DirectoryInfo(templatePath);
        
        if (!Directory.Exists(templatePath))
        {
          var msg = string.Format("Err: In methods - {0}. {1} - not available...",
            nameof(DirectoriesInLocation),
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
          
      #endregion
      
  }
}