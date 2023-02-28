using System.Collections;

namespace ClientExplorer.Application;

public class ClientDirectories
{
  public List<DirectoryEntity> ClientFolders { get; private set; }

  public ClientDirectories()
  {
    ClientFolders = new List<DirectoryEntity>();
    /*
      ./Клиент/Документы общие
      ./Клиент/Каталог стандартных решений
      ./Клиент/Объекты
    */

    //TODO Вынести список папок в какой-то файл настроек. Для возможности редактирования. Пока список папок хардкодим. 
    ClientFolders.Add(new DirectoryEntity("Документы общие"));
    ClientFolders.Add(new DirectoryEntity("Каталог стандартных решений"));
    ClientFolders.Add(new DirectoryEntity("Объекты"));
  }
  
}