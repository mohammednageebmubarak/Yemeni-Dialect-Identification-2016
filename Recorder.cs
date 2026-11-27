// ARCHIVAL TRANSCRIPTION — thesis pp. 70–71.
// Incomplete as printed; not a verified buildable source file.

using NAudio.Wave;
using System.IO;

namespace Dilacts_Identification.Recording
{
    class Recorder
    {
        WaveIn waveInStream; //Recorder
        WaveFileWriter writer;// Save The recorded Data
        private List<string> info; //obtain Avaliable Recorder Device
        //Performe Recording process
        public void Start_Recording(string filename)
        {
            waveInStream = new WaveIn();
            waveInStream.WaveFormat = new WaveFormat(16000,16,1);//Recording in custome Formate in wiche (16000=SampleRate,16=bitsPerSample,1=Channel)

            writer = new WaveFileWriter(filename,waveInStream.WaveFormat);//intilizing The writter with our custom format

waveInStream.DataAvailable                             +=                                  new EventHandler<WaveInEventArgs>(waveInStream_DataAvailable);//call Recording Event
            waveInStream.StartRecording();
        }
        void waveInStream_DataAvailable(object sender, WaveInEventArgs e)
        {
            writer.Write(e.Buffer, 0, e.BytesRecorded);//The writter will save(write) the recorded Bytes and buffer content While Data is avaliable
        }
        //Stop Recording and Realase The Recorder and writter resources
        public void Stop_Recording()
        {
            waveInStream.StopRecording();
            waveInStream.Dispose();
            waveInStream = null;
            writer.Close();
            writer = null;
        }
        //get avaliable Devices for recording
        public List<string> Device_info()
        {
            int waveInDevices = WaveIn.DeviceCount;
            for (int waveInDevice = 0; waveInDevice < waveInDevices; waveInDevice++)
            {
                WaveInCapabilities deviceInfo = WaveIn.GetCapabilities(waveInDevice);
                info=                           new                           List<string> {waveInDevice.ToString(),deviceInfo.ProductName,deviceInfo.Channels.ToString()};
            }
            return info;    }
