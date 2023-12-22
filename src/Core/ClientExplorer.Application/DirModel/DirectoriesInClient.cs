namespace ClientExplorer.Application;

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

    //TODO Вынести список папок в какой-то файл настроек. Для возможности редактирования. Пока список папок хардкодим. 
    Folders.Add(new DirectoryEntity("Документы общие"));
    Folders.Add(new DirectoryEntity("Каталог стандартных решений"));
    Folders.Add(new DirectoryEntity(ClientExplorerApp.Settings.FolderObjectsEntityName));
  }
}