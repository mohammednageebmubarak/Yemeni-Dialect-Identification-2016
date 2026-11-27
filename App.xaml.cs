using System;
using System.Windows;
using System.Windows.Threading;
namespace Dilacts_Identification {
 public partial class App : Application {
  protected override void OnStartup(StartupEventArgs e){base.OnStartup(e);ShutdownMode=ShutdownMode.OnExplicitShutdown;var splash=new SplashScreen();splash.Show();var timer=new DispatcherTimer{Interval=TimeSpan.FromMilliseconds(850)};timer.Tick+=(s,a)=>{timer.Stop();try{var main=new MainWindow();MainWindow=main;main.Show();ShutdownMode=ShutdownMode.OnMainWindowClose;splash.Close();}catch(Exception ex){MessageBox.Show(ex.Message,"Startup failed");Shutdown(1);}};timer.Start();}
 }
}
