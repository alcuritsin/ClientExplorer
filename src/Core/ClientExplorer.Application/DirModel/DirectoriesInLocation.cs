namespace ClientExplorer.Application;

public class DirectoriesInLocation
{
  public List<DirectoryEntity> Folders { get; private set; }

  public DirectoriesInLocation()
  {
    Folders = new List<DirectoryEntity>();
    InitLocationFolders();
  }

  private void InitLocationFolders()
  {
    //TODO Вынести список папок в какой-то файл настроек, для возможности редактирования.
    //Пока список папок хардкодим. Известная проблема - из-за использования в качестве дочерней директории,
    //объект этого же класса, образуется рекурсия с бесконечностью. Это не позволяет сериализовать такую коллекцию.
    
    //TODO Откорректировать структуру папок
    // из папки 02 Фото папку 01 Замеры убираем
    // 04 Проект\01 Входящие документы - Данные от клиента и Данные от собственника - убираем

    #region ./Клиент/Объекты/Город/Адрес, Дом, ТЦ/

    /*
     ./01 Документы
     ./02 Фото
     ./03 Дизайн
     ./04 Проект
     ./05 Согласование в администрации
     */

    #region ./01 Документы

    Folders.Add(new DirectoryEntity("01 Документы"));

    #endregion

    #region ./02 Фото

    /*
      ./02 Фото/01 Замеры
      */

    List<DirectoryEntity> photo = new List<DirectoryEntity>();
    photo.Add(new DirectoryEntity("01 Замеры"));

    Folders.Add(new DirectoryEntity("02 Фото", photo));

    #endregion

    #region ./03 Дизайн

    Folders.Add(new DirectoryEntity("03 Дизайн"));

    #endregion

    #region ./04 Проект

    /*
     ./04 Проект/01 Входящие документы
     ./04 Проект/02 Проект на согласование
     ./04 Проект/03 Проект рабочий
     */

    #region ./04 Проект/01 Входящие документы

    /*
     ./04 Проект/01 Входящие документы/Данные от клиента
     ./04 Проект/01 Входящие документы/Данные от собственника
     */
    List<DirectoryEntity> inDoc = new List<DirectoryEntity>();
    inDoc.Add(new DirectoryEntity("Данные от клиента"));
    inDoc.Add(new DirectoryEntity("Данные от собственника"));

    #endregion

    #region ./04 Проект/03 Проект рабочий

    /*
     ./04 Проект/03 Проект рабочий/01 Файлы для печати
     ./04 Проект/03 Проект рабочий/02 Файлы для плоттера
     ./04 Проект/03 Проект рабочий/03 Файлы для фрезера
     ./04 Проект/03 Проект рабочий/04 Файлы для лазера
     */
    List<DirectoryEntity> workDoc = new List<DirectoryEntity>();
    workDoc.Add(new DirectoryEntity("01 Файлы для печати"));
    workDoc.Add(new DirectoryEntity("02 Файлы для плоттера"));
    workDoc.Add(new DirectoryEntity("03 Файлы для фрезера"));
    workDoc.Add(new DirectoryEntity("04 Файлы для лазера"));

    #endregion

    List<DirectoryEntity> proj = new List<DirectoryEntity>();
    proj.Add(new DirectoryEntity("01 Входящие документы", inDoc));
    proj.Add(new DirectoryEntity("02 Проект на согласование"));
    proj.Add(new DirectoryEntity("03 Проект рабочий", workDoc));

    Folders.Add(new DirectoryEntity("04 Проект", proj));

    #endregion

    #region ./05 Согласование в администрации

    Folders.Add(new DirectoryEntity("05 Согласование в администрации"));

    #endregion

    #endregion
  }
}