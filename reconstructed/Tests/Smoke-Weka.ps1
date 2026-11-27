$ErrorActionPreference='Stop'; if(!$env:DID_WEKA_CP){throw 'Run Setup-Weka.ps1 first'}
$d=Join-Path ([IO.Path]::GetTempPath()) ('DID-test-'+[guid]::NewGuid());New-Item -ItemType Directory $d|Out-Null
try{
$head="@relation synthetic_smoke`n@attribute f1 numeric`n@attribute dialect {A,D,H,L,S}`n@data`n"; $classes=@('A','D','H','L','S');$lines=@();for($i=0;$i -lt 5;$i++){for($j=0;$j -lt 4;$j++){$lines+="$(10*$i+$j),$($classes[$i])"}}
$train=Join-Path $d 'train.arff';$test=Join-Path $d 'test.arff';$model=Join-Path $d 'smoke.model';[IO.File]::WriteAllText($train,$head+($lines -join "`n"));[IO.File]::WriteAllText($test,$head+"1,?`n")
& java -cp $env:DID_WEKA_CP DialectEngine train $train $model NB unsupervised none;if($LASTEXITCODE -ne 0){throw 'Training failed'}
$r=& java -cp $env:DID_WEKA_CP DialectEngine predict $model $test;if($LASTEXITCODE -ne 0 -or !($r -match '^DID_RESULT\s+[ADHLS]\s+')){throw 'Prediction failed'};Write-Host 'Synthetic WEKA smoke test passed (no dialect accuracy claim).'
}finally{Remove-Item -Recurse -Force $d}
