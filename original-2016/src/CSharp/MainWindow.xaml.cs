// ARCHIVAL TRANSCRIPTION — thesis pp. 76–78.
// Incomplete as printed; not a verified buildable source file.

using NAudio.Wave;
using NAudio;
namespace Dilacts_Identification
{
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
public MainWindow()
{
InitializeComponent();
}
private void btnMaxMin_Click(object sender, RoutedEventArgs e)
{
if( this.WindowState != WindowState.Minimized)
{
SystemCommands.MinimizeWindow(MainWindow1);
}
}
private void btnclose_Click(object sender, RoutedEventArgs e)
{
MainWindow1.Close();
}
private void Titlebar_MouseDown(object sender, MouseButtonEventArgs e)
{
if(e.ClickCount==1 && e.LeftButton== MouseButtonState.Pressed)
{
this.DragMove();
}
else { return; }
}

private void MainWindow1_Closing(object sender,
System.ComponentModel.CancelEventArgs e)
{
try
{
Recording.Recorder rec = new Recording.Recorder();
Application.Current.Shutdown();
Application.Current.ShutdownMode =
ShutdownMode.OnMainWindowClose;
rec.Stop_Recording();
}
catch(Exception ex)
{
MessageBoxpage msgpag = new MessageBoxpage(ex.Message);
msgpag.ShowDialog();
}
}
private void AboutItem_click(object sender,RoutedEventArgs e)
{
Aboutpage win = new Aboutpage();
win.ShowDialog();
}
private void EventsItem_click(object sender, RoutedEventArgs e)
{
EventsPage pag = new EventsPage();
Dashboard.Navigate(pag);
}
private void btnDropDown_Click(object sender, RoutedEventArgs e)
{
try
{
if (!(sender as Button).ContextMenu.IsOpen)
{
(sender as Button).ContextMenu.IsEnabled = true;
(sender as Button).ContextMenu.PlacementTarget = (sender as Button);
(sender as Button).ContextMenu.Placement  = System.Windows.Controls.Primitives.PlacementMode.Bottom;
(sender as Button).ContextMenu.IsOpen = true;
}
else
{
(sender as Button).ContextMenu.IsOpen = false;
}
}
catch (Exception ex)
{
MessageBoxpage msgpag = new MessageBoxpage(ex.Message);
msgpag.ShowDialog();
}
