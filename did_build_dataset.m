function did_build_dataset(manifest,outPath,method,fraction)
% CSV columns: path,dialect,speaker_id. Relative paths are manifest-relative.
% The ARFF does not contain speaker IDs (avoid model learning identities).
% A row-aligned groups CSV is emitted for speaker-independent evaluation.
if nargin<4, fraction=1; end
opts=detectImportOptions(manifest);
assert(all(ismember({'path','dialect','speaker_id'},opts.VariableNames)),'Missing manifest columns');
opts=setvartype(opts,{'path','dialect','speaker_id'},'string');
T=readtable(manifest,opts);
assert(all(ismember({'path','dialect','speaker_id'},T.Properties.VariableNames)),'Missing manifest columns');
assert(height(T)>0 && ~any(ismissing(T.path)) && ~any(ismissing(T.speaker_id)) && ~any(ismissing(T.dialect)),'Invalid manifest');
assert(all(ismember(T.dialect,["A","D","H","L","S"])),'Invalid dialect');
assert(numel(unique(T.path))==height(T),'Duplicate file paths');
for g=unique(T.speaker_id)'
    assert(numel(unique(T.dialect(T.speaker_id==g)))==1,'Conflicting speaker labels');
end
base=fileparts(manifest); X=[];
for i=1:height(T)
    p=char(T.path(i));
    if ~(startsWith(p,'/') || startsWith(p,'\') || (numel(p)>1 && p(2)==':')), p=fullfile(base,p); end
    v=did_features(p,method,fraction);
    if i==1, X=zeros(height(T),numel(v)); end
    assert(size(X,2)==numel(v),'Feature size mismatch'); X(i,:)=v;
end
did_write_arff(outPath,X,cellstr(T.dialect),method,fraction);
writetable(T(:,{'speaker_id','dialect'}),[outPath '.groups.csv']);
end
