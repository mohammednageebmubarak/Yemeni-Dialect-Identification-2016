# Reconstructed desktop application

This is a **new, separately labelled reconstruction** of the February 2016 undergraduate project, not a recovered working copy. `../original-2016/` contains the PDFs and archival transcriptions. The reconstruction implements C# / WPF, NAudio, MATLAB feature extraction, Java and WEKA. It does not supply real training audio, old models, the original WPF markup, compiled MATLAB libraries, or the original IKVM assembly. Do not interpret the historical 95% as the performance of this code.

## Setup (Windows)

1. Install Visual Studio 2022 with .NET desktop development and the .NET Framework 4.8 targeting pack. Install MATLAB R2019a or later (for `-batch`) with Signal Processing Toolbox (resampling) and Wavelet Toolbox (DWT). A licensed full MATLAB installation is needed for the default adapter. Install a JDK 8 or later and WEKA 3.8.x with its dependency JARs. These are proposed compatibility targets, not a tested combination; consult the original report for the 2016 environment. No third-party binaries are redistributed.
2. Open PowerShell in this directory. Compile the Java bridge (use the directory containing dependent JARs if WEKA needs them):
   ```powershell
   .\scripts\Setup-Weka.ps1 -WekaJar 'C:\Program Files\Weka-3-8-6\weka.jar'
   # If needed, add -DependencyDirectory 'C:\path\to\jars'
   # If MATLAB is not on PATH:
   $env:DID_MATLAB = 'C:\Program Files\MATLAB\R2024b\bin\matlab.exe'
   [Environment]::SetEnvironmentVariable('DID_MATLAB',$env:DID_MATLAB,'User')
   ```
3. Prepare consented WAVs with `DatasetTools/manifest.example.csv` as a schema (the example paths are placeholders). At least two recordings per dialect are required for training; substantially more distinct speakers are needed for useful evaluation. Files and IDs must not be published unless explicitly authorized. Use pseudonymous speaker IDs. The desktop workflow uses fraction=1 and 16 kHz features. Train the default (MFCC + unsupervised discretization + NB) inspired by the printed app code:
   ```powershell
   .\scripts\Train.ps1 -Manifest C:\private\manifest.csv -OutputDirectory C:\private\did-models
   ```
   For the reported DB4 method, use `-Method DB4 -Classifier NB -Prep discretize` (or `-Classifier SVM`). Other combinations: `-Method HAAR -Classifier BN -Prep select`, `-Ensemble bagging` or `boost`. Large DWT data and feature selection can be memory-intensive. `-Folds 10` is optional recording-level CV only.
4. Restart Visual Studio (to see user environment variables); open `YemeniDID.sln`, restore NuGet, build and launch. Record or open a WAV, play it, select the model and matching MFCC/DB4/HAAR method, then Identify. Only select trusted `.model` files: WEKA uses Java deserialization. Do not use predictions to infer ethnicity, identity, residence or eligibility. A result is a forced choice among five labels, not a validated determination. The app does not save a WAV/event unless the user confirms; saved files stay under Documents/Identified Dialects. Recordings are temporary until saved; interrupted sessions may leave temporary files.

## Evaluation without speaker overlap

Run `DatasetTools/Split-Speakers.ps1 -Manifest C:\private\manifest.csv -OutputDirectory C:\private\split` to create train/test manifests. Run `Train.ps1` on the train manifest only. Build the held-out ARFF with the identical method and fraction:

```powershell
matlab -batch "addpath('MATLAB'); did_build_dataset('C:/private/split/test.csv','C:/private/split/test.arff','MFCC',1)"
java -cp $env:DID_WEKA_CP DialectEngine holdout C:\private\did-models\MFCC.arff C:\private\split\test.arff NB unsupervised none
```

Run from this directory. The Java holdout command trusts the supplied split; the splitter enforces speaker ID separation, not real-world identity. Independently check repeated/duplicate audio, recording devices and speaker labels. Do not tune hyperparameters on the final holdout. `FilteredClassifier` fits supervised/unsupervised transformations on training folds only. Macro F1, weighted AUC, accuracy and confusion matrix are reported. No historical results are reproduced by these commands.

## Tests

Run the MATLAB feature smoke tests with `matlab -batch "addpath('Tests'); test_features"`; run `Tests/Smoke-Weka.ps1` after setting up the JDK/WEKA. Both use synthetic non-speech data only to exercise functionality. Then test the desktop on Windows (record, stop, play, select compatible model, predict, consent to save, view history, close; verify invalid model and missing model errors). See `VALIDATION.md` for actual checks performed here. See `ASSUMPTIONS.md` for methodological and integration differences.
