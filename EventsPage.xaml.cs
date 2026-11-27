// ARCHIVAL TRANSCRIPTION — thesis pp. 89–91.
// Incomplete as printed; not a verified buildable source file.

namespace Dilacts_Identification
{
/// <summary>
/// Interaction logic for EventsPage.xaml
/// </summary>
public partial class EventsPage : Page
{
DataSet ds = new DataSet();
MessageBoxpage pag;
public EventsPage()
{
InitializeComponent();
GetData();
}
private void GetData()
{
try
{
ds.ReadXml(@"\Resources\EventsData.xml");
if (ds.Tables[0].Rows.Count > 0)
{
dataGrid.ItemsSource = ds.Tables[0].DefaultView;
}
else
{
label.Content = "No Events To Display";
}
}
catch(Exception ex)
{
pag = new MessageBoxpage(ex.Message);
pag.ShowDialog();
}
}
private void DeleteData()
{
try
{
string folderpath = Environment.GetFolderPath
(Environment.SpecialFolder.MyDocuments) + @"\Wave Files";
foreach (string fname in Directory.GetFiles(folderpath))
{
File.Delete(fname);
}
ds.Tables[0].Clear();
ds.WriteXml(@"\Resources\EventsData.xml");
}
catch (Exception ex)
{
pag = new MessageBoxpage(ex.Message);
pag.ShowDialog();
}
}
private void btndelete_Click(object sender, RoutedEventArgs e)
{
DeleteData();
}
private void btnback_Click(object sender, RoutedEventArgs e)
{
this.NavigationService.GoBack();
}
}
}
