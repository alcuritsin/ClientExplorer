using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;

namespace ClientExplorer.AvaloniaUI;

public partial class MainWindow : Window
{
  #region Private Members

  private Control _mCityListPopup;
  private Control _mCityName;
  
  private Control _mStreetListPopup;
  private Control _mStreetName;
  
  private Control _mMainGrid;

  #endregion
  public MainWindow()
  {
    InitializeComponent();
#if DEBUG
    this.AttachDevTools();
#endif

    _mCityListPopup = this.FindControl<Control>(nameof(CityListPopup)) ?? throw new Exception("Cannot find City List Popup by name");
    _mCityName = this.FindControl<Control>(nameof(CityName)) ?? throw new Exception("Cannot find City Name TextBox by name");
    
    _mStreetListPopup = this.FindControl<Control>(nameof(StreetListPopup)) ?? throw new Exception("Cannot find Street List Popup by name");
    _mStreetName = this.FindControl<Control>(nameof(StreetName)) ?? throw new Exception("Cannot find Street Name TextBox by name");
    
    _mMainGrid = this.FindControl<Control>(nameof(MainGrid)) ?? throw new Exception("Cannot find Main Grid by name");
  }

  public override void Render(DrawingContext context)
  {
    base.Render(context);

    // var position = _mCityName.TranslatePoint(_mCityName.Bounds.BottomLeft, _mMainGrid) ?? throw new Exception("Cannot get TranslatePoint from City Name TextBox");
    var position = _mCityName.TranslatePoint(new Point(), _mMainGrid) ?? throw new Exception("Cannot get TranslatePoint from City Name TextBox");
    
    _mCityListPopup.Margin = new Thickness(
      position.X,
      position.Y + _mCityName.Bounds.Height,
      0,
      0
    );
    
    position = _mStreetName.TranslatePoint(new Point(), _mMainGrid) ?? throw new Exception("Cannot get TranslatePoint from Street Name TextBox");
    
    _mStreetListPopup.Margin = new Thickness(
      position.X,
      position.Y + _mStreetName.Bounds.Height,
      0,
      0
    );
    
  }
}