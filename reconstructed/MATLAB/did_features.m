function v = did_features(wavPath, method, fraction)
% RECONSTRUCTED: thesis pp. 34-39, not the missing original source.
% Fixed input 16 kHz, mono, trim exact zero edges, pad/truncate to 17000.
% fraction=1 retains all coefficients; optional DWT threshold interpretation
% is documented as a reconstruction, not the original Truncate by Fraction.
if nargin < 3, fraction = 1; end
assert(isscalar(fraction) && isfinite(fraction) && fraction > 0 && fraction <= 1, 'Invalid fraction');
[x,fs] = audioread(wavPath);
assert(~isempty(x) && all(isfinite(x(:))), 'Invalid/empty audio');
x = mean(x,2);
if fs ~= 16000, x = resample(x,16000,fs); end
nz = find(x ~= 0);
assert(~isempty(nz) && max(abs(x)) > 1e-5, 'Silent recording');
x = x(nz(1):nz(end));
x = x(1:min(numel(x),17000));
x(end+1:17000,1) = 0;
switch upper(char(method))
    case 'MFCC'
        % Explicit MFCC implementation. 26 triangular mel filters, 13 DCT
        % coefficients including C0, Hamming window, 512 FFT. See ASSUMPTIONS.
        x = filter([1 -0.97],1,x);
        frame = 400; hop = 160; nfft = 512; nf = 26;
        n = 1+floor((numel(x)-frame)/hop);
        hz = (0:nfft/2)'*16000/nfft;
        mel = @(f) 2595*log10(1+f/700);
        edges = 700*(10.^(linspace(0,mel(8000),nf+2)/2595)-1);
        bank = zeros(nf,numel(hz));
        for k=1:nf
            bank(k,:) = max(0,min((hz-edges(k))/(edges(k+1)-edges(k)),(edges(k+2)-hz)/(edges(k+2)-edges(k+1))))';
        end
        w = 0.54-0.46*cos(2*pi*(0:frame-1)'/(frame-1));
        D = sqrt(2/nf)*cos(pi/nf*((0:12)'*((0:nf-1)+0.5))); D(1,:) = D(1,:)/sqrt(2);
        F = zeros(13,n);
        for k=1:n
            s=x((k-1)*hop+(1:frame)).*w;
            p=abs(fft(s,nfft)).^2/nfft;
            F(:,k)=D*log(max(bank*p(1:nfft/2+1),1e-12));
        end
        v=F(:)';
    case {'DB4','HAAR'}
        % MATLAB Wavelet Toolbox. Explicit boundary mode for reproducibility.
        old=dwtmode('status','nodisp'); cleanup=onCleanup(@()dwtmode(old,'nodisp'));
        dwtmode('sym','nodisp');
        [c,l]=wavedec(x',5,lower(char(method)));
        v=c(:)';
        if fraction < 1
            % Alternative interpretation: keep largest magnitude cA5 terms;
            % preserve cD terms and vector length. Not a recovered algorithm.
            a=l(1); [~,order]=sort(abs(v(1:a)),'descend');
            v(order(ceil(a*fraction)+1:end))=0;
        end
    otherwise, error('Unknown method: %s',char(method));
end
assert(all(isfinite(v)) && ~isempty(v),'Invalid features');
end
