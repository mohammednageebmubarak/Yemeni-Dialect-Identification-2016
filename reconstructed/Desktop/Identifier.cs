using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
namespace Dilacts_Identification {
 internal sealed class Prediction { public string Dialect; public double Score; }
 internal static class Identifier {
  // Explicit adapter replacing the missing 2016 compiled MATLAB/IKVM binaries.
  // Use MATLAB for features and Java/WEKA for prediction; no fabricated model.
  private static string MatlabQuote(string s){return "'"+s.Replace("'","''")+"'";}
  private static string Quote(string s) { // Windows command-line escaping
   var b=new StringBuilder("\""); int n=0;
   foreach(char c in s) {if(c=='\\'){n++;continue;} if(c=='\"') {b.Append('\\',2*n+1);b.Append(c);} else {b.Append('\\',n);b.Append(c);} n=0;} b.Append('\\',2*n);b.Append('"');return b.ToString();
  }
  private static string Run(string exe, params string[] args){
   var psi=new ProcessStartInfo(exe,string.Join(" ",args.Select(Quote))){UseShellExecute=false,CreateNoWindow=true,RedirectStandardOutput=true,RedirectStandardError=true};
   using(var p=Process.Start(psi)){var o=p.StandardOutput.ReadToEndAsync();var e=p.StandardError.ReadToEndAsync(); if(!p.WaitForExit(300000)){p.Kill();throw new TimeoutException(exe+" timed out");} string output=o.GetAwaiter().GetResult();string error=e.GetAwaiter().GetResult(); if(p.ExitCode!=0)throw new InvalidOperationException(exe+" failed: "+error+"\n"+output); return output;}
  }
  public static Prediction Predict(string wav,string model,string method){
   string cp=Environment.GetEnvironmentVariable("DID_WEKA_CP"); if(string.IsNullOrWhiteSpace(cp))throw new InvalidOperationException("Set DID_WEKA_CP using scripts/Setup-Weka.ps1 first.");
   string matlab=Environment.GetEnvironmentVariable("DID_MATLAB")??"matlab"; string java=Environment.GetEnvironmentVariable("DID_JAVA")??"java";
   string dir=Path.Combine(Path.GetTempPath(),"YemeniDID-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(dir);
   try {string arff=Path.Combine(dir,"test.arff"); string lib=Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"MATLAB");
    string cmd="addpath("+MatlabQuote(lib)+"); did_extract_one("+MatlabQuote(wav)+","+MatlabQuote(arff)+","+MatlabQuote(method)+",1);";
    Run(matlab,"-batch",cmd);
    string output=Run(java,"-cp",cp,"DialectEngine","predict",model,arff);
    string line=output.Split(new[]{'\r','\n'},StringSplitOptions.RemoveEmptyEntries).LastOrDefault(s=>s.StartsWith("DID_RESULT\t",StringComparison.Ordinal));
    if(line==null)throw new InvalidOperationException("No prediction returned"); var a=line.Split('\t'); double score=double.Parse(a[2],CultureInfo.InvariantCulture);
    if(double.IsNaN(score)||double.IsInfinity(score)||score<0||score>1)throw new InvalidOperationException("Invalid score");
    string dialect; switch(a[1]){case "A":dialect="Aden";break;case "D":dialect="Dalea";break;case "H":dialect="Hadramout";break;case "L":dialect="Lahj";break;case "S":dialect="Sana'a";break;default:throw new InvalidOperationException("Unexpected class");}
    return new Prediction{Dialect=dialect,Score=score};
   }finally{try{Directory.Delete(dir,true);}catch(IOException){}catch(UnauthorizedAccessException){}}
  }
 }
}
