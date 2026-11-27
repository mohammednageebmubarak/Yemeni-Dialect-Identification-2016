// ARCHIVAL TRANSCRIPTION — thesis pp. 82–88.
// Incomplete as printed; not a verified buildable source file.

using NAudio.Wave;
using NAudio;
using System.IO;
using System.Data;
using System.Xml;
namespace Dilacts_Identification
{

    public partial class Processing : Page
    {
        System.Windows.Forms.Timer PTimer;
        Recording.Recorder Recorder = new Recording.Recorder();
        Recording.Identifier Identifier = new Recording.Identifier();
        System.Windows.Threading.DispatcherTimer           timer         =          new System.Windows.Threading.DispatcherTimer();
        public MediaPlayer player = new MediaPlayer();
        string Wavepath = null;
        private string PredClass { get; set; }
        private string folderpath { get; set; }
        MessageBoxpage MsgPage;
        private enum PlayerState { play,paus }
        private enum VolumeState { Normal,Mute}
        int Counter =0;
        public Processing(string source)
        {
            InitializeComponent();
            lblStatus.Content =PlayerState.play;
            player.MediaEnded += media_ended;
            Wavepath =source;
            player_Timer();

       }
      void media_ended(object sender,EventArgs e)
       {
           try
           {
               player.Stop();
               lblStatus.Content = PlayerState.play;
               btnplaypuse.ToolTip = PlayerState.play;
               btnStop.IsEnabled = false;
               btnplaypuse.Background = SetBackgroundImg(@"images\icon_152.png");
           }
           catch(Exception ex)
           {
               MsgPage = new MessageBoxpage(ex.Message);
               MsgPage.ShowDialog();
           }
       }
       public void player_Timer()
       {
           try
           {
               timer.Interval = TimeSpan.FromMilliseconds(1);
               timer.Tick += player_Tick;
               player.Open(new Uri(Wavepath));
               timer.Start();
           }
           catch(Exception ex)
           {
               MsgPage = new MessageBoxpage(ex.Message);
               MsgPage.ShowDialog();
           }
       }
       void player_Tick(object sender, EventArgs e)
       {
           try
           {
               if (player.Source != null)
                {
                    if (player.NaturalDuration.HasTimeSpan)
                    {
                        lblStart.Content                                                 = String.Format(player.Position.ToString(@"hh\:mm\:ss"));
                        lblEnd.Content                                                   = String.Format(player.NaturalDuration.TimeSpan.ToString(@"hh\:mm\:ss"));
                        playerSliderbar.Minimum = 0;
                        playerSliderbar.Maximum                                          = player.NaturalDuration.TimeSpan.TotalSeconds;
                        playerSliderbar.Value = player.Position.TotalMilliseconds / 1000;
                    }
                }
                else
                {
                    return;
                }
            }
            catch(Exception ex)
            {
                MsgPage = new MessageBoxpage(ex.Message);
                MsgPage.ShowDialog();
            }

        }
        private void processing_Timer()
        {
            PTimer = new System.Windows.Forms.Timer();
            PTimer.Interval =100;
            PTimer.Tick += processing_Tick;
            PTimer.Start();

        }
        private void Stop_Timers()
        {
            try
            {
                timer.Stop();
                timer = null;
                player.Close();
            }
            catch(Exception ex)
            {
                MsgPage = new MessageBoxpage(ex.Message);
                MsgPage.ShowDialog();
            }

        }
      private void processing_Tick(object sender, EventArgs e)
        {
            try
            {
                ringprogress.Visibility = Visibility.Visible;
                lblPercentage.Visibility = Visibility.Visible;
                lblprocessing.Visibility = Visibility.Visible;
                Counter = Counter + 1;
                lblPercentage.Content = Counter + " % Complete";
                btnback.IsEnabled = false;
               btnplaypuse.IsEnabled = false;
               player.Stop();
               if (Counter == 20)
               {
                   lblprocessing.Content = "Extracting Features....";

               }
               else if (Counter == 40)
               {
                   lblprocessing.Content = "Generating ARFF File..";

               }
               else if (Counter == 60)
               {
                   lblprocessing.Content = "Loading Classification Model..";

               }
               else if (Counter == 80)
               {
                   lblprocessing.Content = "Classifying Your Voice..";
               }
               else if (Counter== 100)
               {
                    Stop_Timers();
                    PTimer.Enabled = false;
                    PredClass = GetPredicatedClass();
                    IdentifyingResult res = new IdentifyingResult(PredClass);
                    this.NavigationService.Navigate(res);
                    btnprocessing.IsEnabled = true;
                    btnprocessing.Content = "Processing"; ;
                    btnback.IsEnabled = false;
                    btnplaypuse.IsEnabled = false;
                    ringprogress.IsActive = false;
                    Store_File(PredClass);

               }
           }
          catch(Exception ex)
           {
               MsgPage = new MessageBoxpage(ex.Message);
               MsgPage.ShowDialog();
           }
        }
        private void Store_File(string Dilect)
        {
folderpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)     +@"\Wave Files";
            int fnum = Properties.Settings.Default.FileNumber;
            Directory.CreateDirectory(folderpath);
            File.Move(Wavepath, folderpath + @"\" +Dilect+"("+fnum+")"+".wav");
            XmlDocument EventsData = new XmlDocument();
            EventsData.Load(@"\Resources\EventsData.xml");
            XmlElement Row = EventsData.CreateElement("Event");
            XmlElement FileNum = EventsData.CreateElement("FileNum");
            FileNum.InnerText = (fnum).ToString();
            Properties.Settings.Default.FileNumber = fnum + 1;
            Properties.Settings.Default.Save();
            XmlElement Target = EventsData.CreateElement("Target");
           Target.InnerText = Dilect;
           XmlElement Date = EventsData.CreateElement("Date");
           Date.InnerText = DateTime.Now.ToShortDateString();
           XmlElement Time = EventsData.CreateElement("Time");
           Time.InnerText = DateTime.Now.ToShortTimeString();
           Row.AppendChild(FileNum);
           Row.AppendChild(Target);
           Row.AppendChild(Date);
           Row.AppendChild(Time);
           EventsData.DocumentElement.AppendChild(Row);
           EventsData.Save(@"\Resources\EventsData.xml");

        }
        private string GetPredicatedClass()
        {
            try
            {
                string                             arffsave                             = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)+@"\Test";
                return Identifier.IdentifierEngine(arffsave,Wavepath);
            }
           catch(Exception ex)
            {
                MsgPage = new MessageBoxpage(ex.Message);
                MsgPage.ShowDialog();
                return null;
            }
        }
        private ImageBrush SetBackgroundImg(string path)
        {
            var img = new ImageBrush();
            img.ImageSource = new BitmapImage(new Uri(path, UriKind.Relative));
            return img;
        }
        private void btnplaypuse_Click(object sender, RoutedEventArgs e)
        {
            try {
                string content = lblStatus.Content.ToString();

               if (content == PlayerState.play.ToString()) //Plying state
               {
                   player.Play();
                   btnplaypuse.Background = SetBackgroundImg(@"images\icon_154.png");
                   lblStatus.Content = PlayerState.paus;//paus state
                   btnplaypuse.ToolTip =PlayerState.play;
                   btnStop.IsEnabled = true;
               }
               else
               {
                   player.Pause();
                   lblStatus.Content =PlayerState.play;
                   btnplaypuse.Background = SetBackgroundImg(@"images\icon_152.png");
                   btnplaypuse.ToolTip =PlayerState.play;
               }
           }
           catch (Exception ex)
           {
               MsgPage = new MessageBoxpage(ex.Message);
         MsgPage.ShowDialog();
    }
}

private void btnStop_Click(object sender, RoutedEventArgs e)
{
    player.Stop();
    lblStatus.Content =PlayerState.play;
    btnplaypuse.ToolTip = PlayerState.play;
    btnplaypuse.Background = SetBackgroundImg(@"images\icon_152.png");
    btnStop.IsEnabled = false;
}

private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double>e)
{
    player.Volume =(double)slider.Value/100;
}

private void btnvolume_Click(object sender, RoutedEventArgs e)
{
    string content = btnvolume.Content.ToString();
    if (content==VolumeState.Normal.ToString()) // Normal State
    {
        btnvolume.Background = SetBackgroundImg(@"images\volume_4.png");
        btnvolume.Content =VolumeState.Mute;

        slider.Value = 0;
    }
    else// Mute State
    {
        btnvolume.Background = SetBackgroundImg(@"images\volume_1.png");
        btnvolume.Content =VolumeState.Normal;
        slider.Value =100;
    }
}

private void btnprocessing_Click(object sender, RoutedEventArgs e)
{
    try
    {
        if (btnprocessing.IsEnabled == true)
        {
            btnprocessing.IsEnabled = false;
            processing_Timer();//Start processing
            btnprocessing.Content = "Waiting ...";
        }
        else { return; }
    }
    catch(Exception ex)
    {
        MsgPage = new MessageBoxpage(ex.Message);
        MsgPage.Show();
    }

}

private void btnback_Click(object sender, RoutedEventArgs e)
{
                  Stop_Timers();
                  pages.Home pag = new pages.Home();
                  this.NavigationService.GoBack();
              }
         }
}
