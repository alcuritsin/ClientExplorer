using System.Collections;

namespace ClientExplorer.Application;

public class DirectoriesInClient
{
  public List<DirectoryEntity> Folders { get; private set; }

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
    Folders.Add(new DirectoryEntity(ClientExplorerApp.FolderObjectsName));
  }
}