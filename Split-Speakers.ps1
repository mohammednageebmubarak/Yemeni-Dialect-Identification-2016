param([Parameter(Mandatory=$true)][string]$Manifest,[Parameter(Mandatory=$true)][string]$OutputDirectory,[double]$TestFraction=.25,[int]$Seed=1)
$ErrorActionPreference='Stop'; if($TestFraction -le 0 -or $TestFraction -ge 1){throw 'Invalid fraction'}
$src=(Resolve-Path $Manifest).Path; $base=Split-Path $src -Parent; $rows=@(Import-Csv $src)
if($rows.Count -eq 0){throw 'Empty manifest'}
$classes=@('A','D','H','L','S'); $rnd=[System.Random]::new($Seed); $testIds=@{}; $train=@(); $test=@()
foreach($r in $rows){if([string]::IsNullOrWhiteSpace($r.speaker_id) -or $classes -cnotcontains $r.dialect -or [string]::IsNullOrWhiteSpace($r.path)){throw 'Invalid row'}; if(![IO.Path]::IsPathRooted($r.path)){$r.path=Join-Path $base $r.path}; $r.path=(Resolve-Path $r.path).Path}
foreach($g in ($rows|Group-Object speaker_id)){if(@($g.Group.dialect|Sort-Object -Unique).Count -ne 1){throw 'Conflicting speaker labels'}}
foreach($c in $classes){$ids=@($rows|Where-Object dialect -ceq $c|Select-Object -ExpandProperty speaker_id -Unique|Sort-Object); if($ids.Count -lt 3){throw "Need at least 3 speakers per dialect: $c"}; for($i=$ids.Count-1;$i -gt 0;$i--){$j=$rnd.Next($i+1);$tmp=$ids[$i];$ids[$i]=$ids[$j];$ids[$j]=$tmp};$n=[Math]::Min($ids.Count-2,[Math]::Max(1,[Math]::Round($ids.Count*$TestFraction)));for($i=0;$i -lt $n;$i++){$testIds[$ids[$i]]=$true}}
foreach($r in $rows){if($testIds.ContainsKey($r.speaker_id)){$test+=$r}else{$train+=$r}}
New-Item -ItemType Directory -Force $OutputDirectory|Out-Null
$train|Export-Csv (Join-Path $OutputDirectory 'train.csv') -NoTypeInformation -Encoding UTF8
$test|Export-Csv (Join-Path $OutputDirectory 'test.csv') -NoTypeInformation -Encoding UTF8
Write-Host "Train $($train.Count) recordings; test $($test.Count). No shared speaker IDs by design."
