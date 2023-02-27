namespace ClientExplorer.Shared.ViewModels;

public class FolderEntityViewModel
{
  public string FolderName { get; set; }
  public string ParentFolder { get; set; }
}

/*
  ./Клиент/Документы общие
  ./Клиент/Каталог стандартных решений
  ./Клиент/Объекты

  ./Клиент/Объекты/Акъяр/Адрес, Дом, ТЦ/01 Документы
  
  ./Клиент/Объекты/Акъяр/Адрес, Дом, ТЦ/02 Фото
  ./Клиент/Объекты/Акъяр/Адрес, Дом, ТЦ/02 Фото/01 Замеры
  
  ./Клиент/Объекты/Акъяр/Адрес, Дом, ТЦ/03 Дизайн
  
  ./Клиент/Объекты/Акъяр/Адрес, Дом, ТЦ/04 Проект
  
  ./Клиент/Объекты/Акъяр/Адрес, Дом, ТЦ/04 Проект/01 Входящие документы/Данные от клиента
  ./Клиент/Объекты/Акъяр/Адрес, Дом, ТЦ/04 Проект/01 Входящие документы/Данные от собственника
  
  ./Клиент/Объекты/Акъяр/Адрес, Дом, ТЦ/04 Проект/02 Проект на согласование
  
  ./Клиент/Объекты/Акъяр/Адрес, Дом, ТЦ/04 Проект/03 Проект рабочий/01 Файлы для печати
  ./Клиент/Объекты/Акъяр/Адрес, Дом, ТЦ/04 Проект/03 Проект рабочий/02 Файлы для плоттера
  ./Клиент/Объекты/Акъяр/Адрес, Дом, ТЦ/04 Проект/03 Проект рабочий/03 Файлы для фрезера
  ./Клиент/Объекты/Акъяр/Адрес, Дом, ТЦ/04 Проект/03 Проект рабочий/04 Файлы для лазера

  ./Клиент/Объекты/Акъяр/Адрес, Дом, ТЦ/05 Согласование в администрации
  
*/