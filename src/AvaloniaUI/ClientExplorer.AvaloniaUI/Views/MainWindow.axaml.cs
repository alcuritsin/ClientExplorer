using Avalonia;
using Avalonia.Controls;

namespace ClientExplorer.AvaloniaUI;

public partial class MainWindow : Window
{
  public MainWindow()
  {
    InitializeComponent();
#if DEBUG
    this.AttachDevTools();
#endif
  }
}