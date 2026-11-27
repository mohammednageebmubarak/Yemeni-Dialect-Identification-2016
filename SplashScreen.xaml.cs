// ARCHIVAL TRANSCRIPTION — thesis pp. 74–75.
// Incomplete as printed; not a verified buildable source file.

namespace Dilacts_Identification
{
    public partial class SplashScreen : Window
    {
        System.Windows.Forms.Timer PTimer;
        int counter = 0;
        public SplashScreen()
        {
            InitializeComponent();
            Start_Timer();

         }
         private void Start_Timer()
         {
             PTimer = new System.Windows.Forms.Timer();
             PTimer.Interval =100;
             PTimer.Tick += Timer_Tick;
             PTimer.Start();

         }
         private void Timer_Tick(object sender, EventArgs e)
         {
             try
             {
                 lblPercentage.Visibility = Visibility.Visible;
                 ringprogress.Visibility = Visibility.Visible;
                counter = counter + 2;
                 lblPercentage.Content = counter ;
                 this.Cursor = System.Windows.Input.Cursors.Wait;
                 if (counter== 100)
                 {
                     System.Threading.Thread.Sleep(500);
                     MainWindow pag = new MainWindow();
                     pag.Show();
                     PTimer.Stop();
                     PTimer.Enabled = false;
                     ringprogress.IsActive = false;
                     this.Hide();
                 }
             }
             catch (Exception ex)
             {
                MessageBoxpage MsgPage = new MessageBoxpage(ex.Message);
                 MsgPage.ShowDialog();
             }
         }
         private void Titlebar_MouseDown(object sender, MouseButtonEventArgs e)
         {
             this.DragMove();
         }
     }
}}
