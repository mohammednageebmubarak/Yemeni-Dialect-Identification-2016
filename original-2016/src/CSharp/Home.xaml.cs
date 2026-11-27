// ARCHIVAL TRANSCRIPTION — thesis pp. 79–81.
// Incomplete as printed; not a verified buildable source file.

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NAudio.Wave;
namespace Dilacts_Identification.pages
{
/// <summary>
/// Interaction logic for Home.xaml
/// </summary>
public partial class Home : Page
{
Recording.Recorder Recorder = new Recording.Recorder();
private TimeSpan SpanTime;
System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
private string wavepath { get; set; }
MessageBoxpage msgpage;
public Home()
{
InitializeComponent();
GetUserInfo();
}
private void Start_Timer()
{
try
{
timer.Interval = new TimeSpan(0, 0, 0, 1);
timer.Tick += timer_Tick;
timer.Start();
Recorder.Start_Recording(wavepath);
}
catch(Exception ex)
{
msgpage = new MessageBoxpage(ex.Message);
msgpage.ShowDialog();
}
}
public void Stop_Timer()
{
try
{
Recorder.Stop_Recording();
timer.Stop();
timer = null;
}
catch (Exception ex)
{
msgpage = new MessageBoxpage(ex.Message);
msgpage.ShowDialog();
}
}
void timer_Tick(object sender, EventArgs e)
{
try
{
SpanTime = SpanTime.Add(timer.Interval);
lblTimer.Content = SpanTime;
if (lblTimer.Content.ToString() == ("00:00:02").ToString())
{
Stop_Timer();
System.Threading.Thread.Sleep(1000);
lblTimer.Content = ("00:00:00").ToString();
Processing pro = new Processing(wavepath);
this.NavigationService.Navigate(pro);
}
}
catch(Exception ex)
{
msgpage = new MessageBoxpage(ex.Message);
msgpage.ShowDialog();
}
}
private void GetUserInfo()
{
try
{
txtInfo.Text = "Welcome " + Environment.UserName +Environment.NewLine + "please Start Recording Your Voice";
List<string> Info = Recorder.Device_info();
lblInfo.Content = "Avaliable Recorder Device : " + Info [1].ToString() + " Channel " + Info[2].ToString() + ")";
wavepath       =    Environment.GetFolderPath        (Environment.SpecialFolder.MyMusic).ToString()           + @"\temp.wav";
}
catch (Exception ex)
{
msgpage = new MessageBoxpage(ex.Message);
msgpage.ShowDialog();
}
}
private void btnstart_Click(object sender, RoutedEventArgs e)


{
Start_Timer();
}
