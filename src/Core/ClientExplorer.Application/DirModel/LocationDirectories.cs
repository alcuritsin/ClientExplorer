namespace ClientExplorer.Application;

public class LocationDirectories
{
  public List<DirectoryEntity> LocationFolders { get; private set; }

  public LocationDirectories()
  {
    LocationFolders = new List<DirectoryEntity>();
    InitLocationFolders();
  }

  private void InitLocationFolders()
  {
    //TODO Вынести список папок в какой-то файл настроек. Для возможности редактирования. Пока список папок хардкодим. 

    #region ./Клиент/Объекты/Город/Адрес, Дом, ТЦ/

    /*  
      ./01 Документы
      ./02 Фото
      ./03 Дизайн
      ./04 Проект
      ./05 Согласование в администрации
      */

    LocationFolders.Add(new DirectoryEntity(dirName: ClientEr.FolderDocumentName));
    LocationFolders.Add(new DirectoryEntity(dirName: ClientEr.FolderPhotoName));
    LocationFolders.Add(new DirectoryEntity(dirName: ClientEr.FolderDesignName));
    LocationFolders.Add(new DirectoryEntity(dirName: ClientEr.FolderProjectName));
    LocationFolders.Add(new DirectoryEntity(dirName: ClientEr.FolderApprovalInAdministrationName));

    #region ./02 Фото

    /*
      ./02 Фото/01 Замеры
      */
    var directoryWithParent = new DirectoryEntity(
      "01 Замеры",
      LocationFolders.Find(item => item.DirName == ClientEr.FolderPhotoName)
    );
    LocationFolders.Add(directoryWithParent);

    #endregion

    #region ./04 Проект

    /*
      ./04 Проект/01 Входящие документы
      ./04 Проект/02 Проект на согласование
      ./04 Проект/03 Проект рабочий
      */

    directoryWithParent = new DirectoryEntity(
      "01 Входящие документы",
      LocationFolders.Find(item => item.DirName == ClientEr.FolderProjectName));
    LocationFolders.Add(directoryWithParent);

    directoryWithParent = new DirectoryEntity(
      "02 Проект на согласование",
      LocationFolders.Find(item => item.DirName == ClientEr.FolderProjectName));
    LocationFolders.Add(directoryWithParent);

    directoryWithParent = new DirectoryEntity(
      "03 Проект рабочий",
      LocationFolders.Find(item => item.DirName == ClientEr.FolderProjectName));
    LocationFolders.Add(directoryWithParent);

    #region ./04 Проект/01 Входящие документы

    /*
      ./04 Проект/01 Входящие документы/Данные от клиента
      ./04 Проект/01 Входящие документы/Данные от собственника
      */

    directoryWithParent = new DirectoryEntity(
      "Данные от клиента",
      LocationFolders.Find(item => item.DirName == "01 Входящие документы"));
    LocationFolders.Add(directoryWithParent);

    directoryWithParent = new DirectoryEntity(
      "Данные от собственника",
      LocationFolders.Find(item => item.DirName == "01 Входящие документы"));
    LocationFolders.Add(directoryWithParent);

    #endregion

    #region ./04 Проект/03 Проект рабочий

    /*
       ./04 Проект/03 Проект рабочий/01 Файлы для печати
       ./04 Проект/03 Проект рабочий/02 Файлы для плоттера
       ./04 Проект/03 Проект рабочий/03 Файлы для фрезера
       ./04 Проект/03 Проект рабочий/04 Файлы для лазера
       */

    directoryWithParent = new DirectoryEntity(
      "01 Файлы для печати",
      LocationFolders.Find(item => item.DirName == "03 Проект рабочий"));
    LocationFolders.Add(directoryWithParent);

    directoryWithParent = new DirectoryEntity(
      "02 Файлы для плоттера",
      LocationFolders.Find(item => item.DirName == "03 Проект рабочий"));
    LocationFolders.Add(directoryWithParent);

    directoryWithParent = new DirectoryEntity(
      "03 Файлы для фрезера",
      LocationFolders.Find(item => item.DirName == "03 Проект рабочий"));
    LocationFolders.Add(directoryWithParent);

    directoryWithParent = new DirectoryEntity(
      "04 Файлы для лазера",
      LocationFolders.Find(item => item.DirName == "03 Проект рабочий"));
    LocationFolders.Add(directoryWithParent);

    #endregion

    #endregion

    #endregion
    
  }
}