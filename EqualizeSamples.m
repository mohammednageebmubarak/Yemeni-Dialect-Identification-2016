% Archival transcription of Figure 5.2, thesis p. 35.
% Original code fragment; wav_file must already be defined.
% Read speech samples, sampling rate and precision from file
[ speech, fs ] = audioread( wav_file );
% remove trialing and leading zeros
speechNew = speech(find(speech,1,'first'):find(speech,1,'last'));
% analysing 1 sec
if length(speechNew)>=17000
    speechNew = speechNew(1:17000)';
else
    speechNew = [speechNew' zeros(1,(17000-length(speechNew)))];
end
