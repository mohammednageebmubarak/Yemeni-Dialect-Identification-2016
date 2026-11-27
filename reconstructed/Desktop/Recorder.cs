using System;
using System.Threading.Tasks;
using NAudio.Wave;
namespace Dilacts_Identification {
 // Reconstructed NAudio recorder; waits for RecordingStopped before reading WAV.
 internal sealed class Recorder : IDisposable {
  private WaveInEvent input; private WaveFileWriter writer; private TaskCompletionSource<bool> stopped;
  public bool Active { get { return input != null; } }
  public void Start(string path,int device) {
   if(Active) throw new InvalidOperationException("Already recording");
   var w=new WaveInEvent { DeviceNumber=device, WaveFormat=new WaveFormat(16000,16,1) };
   try {
    writer=new WaveFileWriter(path,w.WaveFormat); stopped=new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
    input=w;
    w.DataAvailable += (s,e)=>writer.Write(e.Buffer,0,e.BytesRecorded);
    w.RecordingStopped += (s,e)=> { try { writer.Dispose(); if(e.Exception!=null) stopped.TrySetException(e.Exception); else stopped.TrySetResult(true); } catch(Exception ex){ stopped.TrySetException(ex); } };
    w.StartRecording();
   } catch { writer?.Dispose(); w.Dispose(); input=null; throw; }
  }
  public async Task StopAsync() { var w=input; if(w==null) return; try { w.StopRecording(); await stopped.Task; } finally { w.Dispose(); input=null; writer=null; } }
  public void Dispose(){ if(input!=null) throw new InvalidOperationException("Stop recording before disposal"); }
 }
}
