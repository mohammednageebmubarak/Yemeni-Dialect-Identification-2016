function did_extract_one(wavPath,outPath,method,fraction)
if nargin<4, fraction=1; end
v=did_features(wavPath,method,fraction);
did_write_arff(outPath,v,{'?'},method,fraction);
end
