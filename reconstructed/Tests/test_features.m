function test_features
% Synthetic signals ONLY to exercise the software. Not Yemeni speech.
root=fileparts(fileparts(mfilename('fullpath'))); addpath(fullfile(root,'MATLAB'));
d=tempname; mkdir(d); cleanup=onCleanup(@()rmdir(d,'s'));
t=(0:15999)'/16000; f=fullfile(d,'tone.wav'); audiowrite(f,.2*sin(2*pi*320*t),16000);
v=did_features(f,'MFCC',1); assert(numel(v)==13*104 && all(isfinite(v)));
for method={'DB4','HAAR'}, x=did_features(f,method{1},1); y=did_features(f,method{1},.5); assert(numel(x)==numel(y) && all(isfinite(x))); end
p=fullfile(d,'test.arff');did_extract_one(f,p,'MFCC',1);s=fileread(p);assert(contains(s,'@attribute dialect {A,D,H,L,S}') && endsWith(strtrim(s),'?'));
audiowrite(f,zeros(16000,1),16000);failed=false;try,did_features(f,'MFCC');catch,failed=true;end;assert(failed,'Silent input must fail');
fprintf('Synthetic feature smoke tests passed (no accuracy claim).\n');
end
