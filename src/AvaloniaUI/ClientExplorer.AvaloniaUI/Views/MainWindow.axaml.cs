using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace ClientExplorer.AvaloniaUI;

public partial class MainWindow : Window
{
  #region Private Members

  private readonly Control _mCityListPopup;
  private readonly Control _mCityName;

  private readonly Control _mStreetListPopup;
  private readonly Control _mStreetName;

  private readonly Control _mHouseNumberPopup;
  private readonly Control _mHouseNumber;

  private readonly Control _mMainGrid;

  #endregion

  public MainWindow()
  {
    InitializeComponent();
#if DEBUG
    this.AttachDevTools();
#endif

    _mCityListPopup = this.FindControl<Control>(nameof(CitiesListPopup));
    _mCityName = this.FindControl<Control>(nameof(CityName));

    _mStreetListPopup = this.FindControl<Control>(nameof(StreetsListPopup));
    _mStreetName = this.FindControl<Control>(nameof(StreetName));

    _mHouseNumberPopup = this.FindControl<Control>(nameof(HouseNumbersListPopup));
    _mHouseNumber = this.FindControl<Control>(nameof(HouseNumber));

    _mMainGrid = this.FindControl<Control>(nameof(MainGrid));
  }

  public override void Render(DrawingContext context)
  {
    base.Render(context);

    var position = _mCityName.TranslatePoint(new Point(), _mMainGrid) ??
                   throw new Exception("Cannot get TranslatePoint from City Name TextBox");

    _mCityListPopup.Margin = new Thickness(
      position.X,
      position.Y + _mCityName.Bounds.Height,
      0,
      0
    );

    position = _mStreetName.TranslatePoint(new Point(), _mMainGrid) ??
               throw new Exception("Cannot get TranslatePoint from Street Name TextBox");

    _mStreetListPopup.Margin = new Thickness(
      position.X,
      position.Y + _mStreetName.Bounds.Height,
      0,
      0
    );

    position = _mHouseNumber.TranslatePoint(new Point(), _mMainGrid) ??
               throw new Exception("Cannot get TranslatePoint from House Number TextBox");

    _mHouseNumberPopup.Margin = new Thickness(
      position.X,
      position.Y + _mHouseNumber.Bounds.Height,
      0,
      0
    );
  }
}