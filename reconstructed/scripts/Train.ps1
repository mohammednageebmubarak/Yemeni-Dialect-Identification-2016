param([Parameter(Mandatory=$true)][string]$Manifest,[Parameter(Mandatory=$true)][string]$OutputDirectory,[ValidateSet('MFCC','DB4','HAAR')][string]$Method='MFCC',[ValidateSet('NB','BN','SVM','KNN1','KNN5')][string]$Classifier='NB',[ValidateSet('none','discretize','unsupervised','select')][string]$Prep='unsupervised',[ValidateSet('none','bagging','boost')][string]$Ensemble='none',[int]$Folds=0)
$ErrorActionPreference='Stop'; if(!$env:DID_WEKA_CP){throw 'Run Setup-Weka.ps1 first'}
$root=Split-Path $PSScriptRoot -Parent; $matlab=if($env:DID_MATLAB){$env:DID_MATLAB}else{'matlab'}; $java=if($env:DID_JAVA){$env:DID_JAVA}else{'java'}
function mq([string]$v){"'"+$v.Replace("'","''")+"'"}
$manifestPath=(Resolve-Path $Manifest).Path; New-Item -ItemType Directory -Force $OutputDirectory|Out-Null; $out=(Resolve-Path $OutputDirectory).Path
$arff=Join-Path $out "$Method.arff"; $model=Join-Path $out "$Method-$Classifier-$Prep-$Ensemble.model"
$expr="addpath($(mq (Join-Path $root 'MATLAB'))); did_build_dataset($(mq $manifestPath),$(mq $arff),$(mq $Method),1);"
& $matlab -batch $expr
if($LASTEXITCODE -ne 0){throw 'MATLAB extraction failed'}
if($Folds -gt 0){ & $java -cp $env:DID_WEKA_CP DialectEngine cv $arff $Classifier $Prep $Ensemble $Folds | Tee-Object -FilePath (Join-Path $out 'recording-cv.txt'); if($LASTEXITCODE -ne 0){throw 'Evaluation failed'} }
& $java -cp $env:DID_WEKA_CP DialectEngine train $arff $model $Classifier $Prep $Ensemble
if($LASTEXITCODE -ne 0){throw 'Training failed'}
Write-Host "Saved $model"
