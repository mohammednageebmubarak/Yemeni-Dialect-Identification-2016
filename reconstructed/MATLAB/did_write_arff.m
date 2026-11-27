function did_write_arff(outPath,X,labels,method,fraction)
% Reconstructed ARFF serializer. Missing target = '?' (never a fake label).
assert(size(X,1)==numel(labels) && ~isempty(X) && all(isfinite(X(:))));
assert(all(ismember(labels,{'A','D','H','L','S','?'})));
fid=fopen(outPath,'w'); assert(fid>=0,'Cannot open ARFF'); closeFile=onCleanup(@()fclose(fid));
fprintf(fid,'@relation YemeniDID_%s_f%.12g\n\n',upper(char(method)),fraction);
for i=1:size(X,2), fprintf(fid,'@attribute f%d numeric\n',i); end
fprintf(fid,'@attribute dialect {A,D,H,L,S}\n\n@data\n');
for i=1:size(X,1)
    fprintf(fid,'%.17g,',X(i,:)); fprintf(fid,'%s\n',labels{i});
end
end
