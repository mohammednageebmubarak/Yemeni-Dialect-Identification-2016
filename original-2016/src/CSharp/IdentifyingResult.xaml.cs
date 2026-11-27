// ARCHIVAL TRANSCRIPTION — thesis pp. 88–89.
// Incomplete as printed; not a verified buildable source file.

namespace Dilacts_Identification
{
public partial class IdentifyingResult : Page
{
public IdentifyingResult(string Dialect)
{
InitializeComponent();
lblResult.Content = "Target Found ("+Dialect+")";
}
private void btnback_Click(object sender, RoutedEventArgs e)
{
pages.Home page = new pages.Home();
this.NavigationService.Navigate(page);
}
}
}
