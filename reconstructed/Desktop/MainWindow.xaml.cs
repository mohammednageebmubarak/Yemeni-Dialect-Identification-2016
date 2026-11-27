using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml.Linq;
using Microsoft.Win32;
using NAudio.Wave;
namespace Dilacts_Identification {
 public partial class MainWindow : Window {
  private readonly Recorder recorder=new Recorder(); private readonly MediaPlayer player=new MediaPlayer(); private readonly Stopwatch clock=new Stopwatch(); private readonly DispatcherTimer timer=new DispatcherTimer();
  private string wav,model; private bool busy,playing,updating,recorded;
  private readonly string dataDir=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),"Identified Dialects");
  private string EventsPath=>Path.Combine(dataDir,"EventsData.xml");
  public MainWindow(){InitializeComponent(); for(int i=0;i<WaveIn.DeviceCount;i++)Devices.Items.Add(WaveIn.GetCapabilities(i).ProductName); if(Devices.Items.Count>0)Devices.SelectedIndex=0; else {RecordButton.IsEnabled=false;HomeStatus.Text="No microphone detected. Open a WAV instead.";} timer.Interval=TimeSpan.FromMilliseconds(200); timer.Tick+=(s,e)=>Tick();timer.Start();player.MediaOpened+=(s,e)=>{if(player.NaturalDuration.HasTimeSpan){Position.Maximum=player.NaturalDuration.TimeSpan.TotalSeconds;Position.IsEnabled=true;}};player.MediaEnded+=(s,e)=>{player.Stop();playing=false;};player.MediaFailed+=(s,e)=>PlayerStatus.Text=e.ErrorException.Message;}
  private void Show(UIElement target){foreach(UIElement p in new UIElement[]{HomePanel,PlayerPanel,ResultPanel,HistoryPanel})p.Visibility=p==target?Visibility.Visible:Visibility.Collapsed;}
  private bool Locked()=>busy||recorder.Active;
  private void CleanRecording(){if(recorded&&wav!=null){try{File.Delete(wav);}catch(IOException){}catch(UnauthorizedAccessException){}}recorded=false;wav=null;}
  private void HomeClick(object s,RoutedEventArgs e){if(Locked())return; player.Close();playing=false;CleanRecording();Show(HomePanel);}
  private void RecordClick(object s,RoutedEventArgs e){if(Locked()||Devices.SelectedIndex<0)return;try{CleanRecording();wav=Path.Combine(Path.GetTempPath(),"DID-"+Guid.NewGuid().ToString("N")+".wav");recorder.Start(wav,Devices.SelectedIndex);recorded=true;clock.Restart();RecordButton.IsEnabled=false;StopButton.IsEnabled=true;OpenButton.IsEnabled=false;HomeStatus.Text="Recording…";}catch(Exception ex){HomeStatus.Text=ex.Message;}}
  private async void StopClick(object s,RoutedEventArgs e){StopButton.IsEnabled=false;busy=true;try{await recorder.StopAsync();clock.Stop();OpenPlayer();}catch(Exception ex){HomeStatus.Text=ex.Message;}finally{busy=false;RecordButton.IsEnabled=Devices.Items.Count>0;OpenButton.IsEnabled=true;}}
  private void OpenClick(object s,RoutedEventArgs e){if(Locked())return;var d=new OpenFileDialog{Filter="WAV audio (*.wav)|*.wav"};if(d.ShowDialog()==true){player.Close();CleanRecording();wav=d.FileName;recorded=false;OpenPlayer();}}
  private void OpenPlayer(){player.Close();playing=false;Position.IsEnabled=false;Position.Value=0;FileLabel.Text=Path.GetFileName(wav);PlayerStatus.Text="";player.Open(new Uri(wav));player.Volume=Volume.Value;Show(PlayerPanel);}
  private void PlayClick(object s,RoutedEventArgs e){if(playing){player.Pause();playing=false;}else{player.Play();playing=true;}}
  private void PlaybackStop(object s,RoutedEventArgs e){player.Stop();playing=false;}
  private void VolumeChanged(object s,RoutedPropertyChangedEventArgs<double> e){player.Volume=e.NewValue;}
  private void SeekChanged(object s,RoutedPropertyChangedEventArgs<double> e){if(!updating&&Position!=null&&Position.IsEnabled)player.Position=TimeSpan.FromSeconds(e.NewValue);}
  private void Tick(){if(recorder.Active)Elapsed.Text=clock.Elapsed.ToString(@"hh\:mm\:ss");if(player.NaturalDuration.HasTimeSpan){updating=true;Position.Value=player.Position.TotalSeconds;PlaybackTime.Text=player.Position.ToString(@"mm\:ss")+" / "+player.NaturalDuration.TimeSpan.ToString(@"mm\:ss");updating=false;}}
  private void ModelClick(object s,RoutedEventArgs e){if(busy)return;var d=new OpenFileDialog{Filter="Trusted WEKA model (*.model)|*.model"};if(d.ShowDialog()==true){model=d.FileName;ModelLabel.Text=Path.GetFileName(model);}}
  private async void IdentifyClick(object s,RoutedEventArgs e){if(Locked())return;if(string.IsNullOrEmpty(model)){PlayerStatus.Text="Train a model with your own consented recordings, then select it.";return;}busy=true;IdentifyButton.IsEnabled=false;ModelButton.IsEnabled=false;Progress.IsIndeterminate=true;PlayerStatus.Text="Extracting features and classifying…";player.Stop();playing=false;string method=((ComboBoxItem)Method.SelectedItem).Content.ToString();try{var p=await Task.Run(()=>Identifier.Predict(wav,model,method));ResultText.Text=p.Dialect;ScoreText.Text="Model score: "+p.Score.ToString("P1",CultureInfo.InvariantCulture);if(MessageBox.Show("Save this WAV and prediction locally?", "Save event",MessageBoxButton.YesNo,MessageBoxImage.Question)==MessageBoxResult.Yes) {try{SaveEvent(p,method);}catch(Exception ex){MessageBox.Show("Prediction succeeded, event save failed: "+ex.Message);}}Show(ResultPanel);}catch(Exception ex){PlayerStatus.Text=ex.Message;}finally{busy=false;IdentifyButton.IsEnabled=true;ModelButton.IsEnabled=true;Progress.IsIndeterminate=false;}}
  private void SaveEvent(Prediction p,string method){Directory.CreateDirectory(dataDir);string name=Guid.NewGuid().ToString("N")+".wav";File.Copy(wav,Path.Combine(dataDir,name));var doc=File.Exists(EventsPath)?XDocument.Load(EventsPath):new XDocument(new XElement("Events"));doc.Root.Add(new XElement("Event",new XElement("Date",DateTime.UtcNow.ToString("o")),new XElement("Dialect",p.Dialect),new XElement("Method",method),new XElement("Score",p.Score.ToString(CultureInfo.InvariantCulture)),new XElement("File",name)));doc.Save(EventsPath);}
  private void HistoryClick(object s,RoutedEventArgs e){if(Locked())return;try{player.Close();playing=false;HistoryGrid.ItemsSource=File.Exists(EventsPath)?XDocument.Load(EventsPath).Root.Elements("Event").Select(x=>new {Date=(string)x.Element("Date"),Dialect=(string)x.Element("Dialect"),Method=(string)x.Element("Method"),Score=(string)x.Element("Score"),File=(string)x.Element("File")}).ToList():null;Show(HistoryPanel);}catch(Exception ex){MessageBox.Show(ex.Message);}}
  private void OnClosing(object s,CancelEventArgs e){if(Locked()){e.Cancel=true;MessageBox.Show("Stop recording or finish identification before closing.");return;}timer.Stop();player.Close();recorder.Dispose();CleanRecording();}
 }
}
