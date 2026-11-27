# Source mapping and reconstruction decisions

| Component | Source | Reconstruction |
|---|---|---|
| Audio collection | pp. 32–35, 70–71 | 16 kHz/16-bit/mono recording; resample imported WAV; exact edge-zero trimming, silence guard, 17,000 samples |
| MFCC | pp. 15–16, 36–37 | 25 ms/10 ms; 26 mel filters, 13 DCT coefficients including C0 (report says both 13 and 12); pre-emphasis .97, 512 FFT, Hamming; flatten column order, no delta features. These choices are not recovered settings. |
| DWT | pp. 37–38, 52–58 | MATLAB wavedec, Haar/DB4, level 5, symmetric extension; all cA and cD coefficients; optional largest-magnitude cA retention at fraction <1 is an interpretation, not original code. Default retains all. |
| ARFF | pp. 27–30, 39, 71–73 | One file/row per recording, five labels, numeric attributes; test class is missing. Header checks enforce method/fraction compatibility. |
| Classifiers | pp. 20–24, 48–58 | WEKA NB, BN, SMO, IBk; CfsSubsetEval/BestFirst (exact original evaluator/settings unknown), discretization, 10-iteration bagging/boosting. WEKA defaults unless stated; no reproduction of exact attribute selection count. |
| Evaluation | pp. 40–47, 52–58 | Fold-local filter fitting to reduce leakage; new speaker split/holdout; historical results remain separately archived. |
| Desktop | pp. 59–94 | Recreated dark/teal WPF layout and record/player/result/events workflow. Uses standard window chrome, new controls, no recovered logos or original XAML; new splash screen. |
| Integration | pp. 60–69, 71–73 | Missing DLLs/IKVM assemblies prevent exact integration. Default adapter invokes licensed MATLAB and Java/WEKA via processes, preserving languages and core logic; saves trained model instead of retraining every prediction. Full original in-process binary bridge not implemented/tested. |

The original report appears to use MFCC with unsupervised discretization in the application, whereas the 95% experiments use DB4 and a discretization filter discussed as supervised. Both paths are represented separately. This reconstruction uses modern compatible build tooling only, not a Python/deep-learning rewrite. It is not a bit-for-bit restoration. Source text and images are preserved in the archive. See [research rights and attribution](../original-2016/README.md#research-rights-and-attribution).
