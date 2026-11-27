// Reconstructed, not recovered source. WEKA 3.8.x API.
import java.io.*;
import java.util.*;
import weka.core.*;
import weka.core.converters.ConverterUtils.DataSource;
import weka.classifiers.*;
import weka.classifiers.bayes.*;
import weka.classifiers.functions.SMO;
import weka.classifiers.lazy.IBk;
import weka.classifiers.meta.*;
import weka.filters.Filter;
import weka.attributeSelection.BestFirst;
import weka.attributeSelection.CfsSubsetEval;

public class DialectEngine {
    private static Instances read(String path) throws Exception {
        Instances d=DataSource.read(path); if(d==null || d.numAttributes()<2 || d.numInstances()==0) throw new IllegalArgumentException("Empty ARFF");
        d.setClassIndex(d.numAttributes()-1);
        if(!d.classAttribute().isNominal() || !d.classAttribute().name().equals("dialect") || d.numClasses()!=5) throw new IllegalArgumentException("Expected dialect {A,D,H,L,S}");
        String[] classes={"A","D","H","L","S"}; for(int i=0;i<5;i++) if(!classes[i].equals(d.classAttribute().value(i))) throw new IllegalArgumentException("Wrong label order");
        for(int i=0;i<d.classIndex();i++) if(!d.attribute(i).isNumeric()) throw new IllegalArgumentException("Expected numeric input features");
        return d;
    }
    private static void validateTraining(Instances d) {
        int[] counts=new int[5]; for(Instance x:d) { if(x.classIsMissing()) throw new IllegalArgumentException("Missing training label"); counts[(int)x.classValue()]++; }
        for(int c:counts) if(c<2) throw new IllegalArgumentException("Need at least two recordings per dialect");
    }
    private static Classifier model(String name,String prep,String ensemble) throws Exception {
        Classifier m;
        switch(name.toUpperCase(Locale.ROOT)) {
            case "NB": m=new NaiveBayes(); break;
            case "BN": m=new BayesNet(); break;
            case "SVM": m=new SMO(); break;
            case "KNN1": m=new IBk(1); break;
            case "KNN5": m=new IBk(5); break;
            default: throw new IllegalArgumentException("Unknown classifier");
        }
        // FilteredClassifier learns preprocessing only on training data.
        Filter f=null;
        switch(prep.toLowerCase(Locale.ROOT)) {
            case "none": break;
            case "discretize": f=new weka.filters.supervised.attribute.Discretize(); break;
            case "unsupervised": f=new weka.filters.unsupervised.attribute.Discretize(); break;
            case "select":
                weka.filters.supervised.attribute.AttributeSelection s=new weka.filters.supervised.attribute.AttributeSelection();
                s.setEvaluator(new CfsSubsetEval()); s.setSearch(new BestFirst()); f=s; break;
            default: throw new IllegalArgumentException("Unknown preprocessing");
        }
        if(f!=null) { FilteredClassifier fc=new FilteredClassifier(); fc.setFilter(f); fc.setClassifier(m); m=fc; }
        switch(ensemble.toLowerCase(Locale.ROOT)) {
            case "none": break;
            case "bagging": Bagging b=new Bagging(); b.setSeed(1); b.setNumIterations(10); b.setClassifier(m); m=b; break;
            case "boost": AdaBoostM1 a=new AdaBoostM1(); a.setSeed(1); a.setNumIterations(10); a.setClassifier(m); m=a; break;
            default: throw new IllegalArgumentException("Unknown ensemble");
        }
        return m;
    }
    private static void report(Evaluation e,Instances d) throws Exception {
        System.out.println(e.toSummaryString()); System.out.println(e.toClassDetailsString()); System.out.println(e.toMatrixString());
        System.out.printf(Locale.ROOT,"accuracy=%.6f macroF1=%.6f weightedAUC=%.6f%n",e.pctCorrect(),macroF1(e,d),e.weightedAreaUnderROC());
    }
    private static double macroF1(Evaluation e,Instances d){double n=0;for(int i=0;i<d.numClasses();i++){double f=e.fMeasure(i);n+=Double.isNaN(f)?0:f;}return n/d.numClasses();}
    public static void main(String[] args) throws Exception {
        if(args.length==0) throw new IllegalArgumentException("train|predict|cv|holdout");
        switch(args[0]) {
            case "train": { // train ARFF MODEL classifier prep ensemble
                if(args.length!=6) throw new IllegalArgumentException("train ARFF MODEL classifier prep ensemble");
                Instances d=read(args[1]); validateTraining(d); Classifier m=model(args[3],args[4],args[5]); m.buildClassifier(d);
                SerializationHelper.writeAll(args[2],new Object[]{m,new Instances(d,0)}); System.out.println("Model saved"); break;
            }
            case "predict": { // predict MODEL ARFF
                if(args.length!=3) throw new IllegalArgumentException("predict MODEL ARFF");
                Object[] saved=SerializationHelper.readAll(args[1]); Instances header=(Instances)saved[1]; Instances t=read(args[2]);
                if(!header.equalHeaders(t) || !header.relationName().equals(t.relationName())) throw new IllegalArgumentException("Model/feature schema mismatch");
                if(t.numInstances()!=1 || !t.instance(0).classIsMissing()) throw new IllegalArgumentException("Expected one unlabeled instance");
                Classifier m=(Classifier)saved[0]; double[] p=m.distributionForInstance(t.instance(0)); int i=Utils.maxIndex(p);
                if(i<0 || !Double.isFinite(p[i]) || p[i]<=0) throw new IllegalStateException("Invalid model distribution");
                System.out.printf(Locale.ROOT,"DID_RESULT\t%s\t%.8f%n",t.classAttribute().value(i),p[i]); break;
            }
            case "cv": {
                if(args.length!=6) throw new IllegalArgumentException("cv ARFF classifier prep ensemble folds");
                Instances d=read(args[1]); validateTraining(d); int k=Integer.parseInt(args[5]); if(k<2 || k>d.numInstances()) throw new IllegalArgumentException("Invalid folds");
                Evaluation e=new Evaluation(d); e.crossValidateModel(model(args[2],args[3],args[4]),d,k,new Random(1));
                System.out.println("Recording-level CV only. NOT speaker-independent unless exactly one sample per speaker."); report(e,d); break;
            }
            case "holdout": {
                if(args.length!=6) throw new IllegalArgumentException("holdout TRAIN TEST classifier prep ensemble");
                Instances d=read(args[1]), t=read(args[2]); validateTraining(d);
                if(!d.equalHeaders(t) || !d.relationName().equals(t.relationName())) throw new IllegalArgumentException("Schema mismatch");
                for(Instance x:t) if(x.classIsMissing()) throw new IllegalArgumentException("Missing test label");
                Classifier m=model(args[3],args[4],args[5]); m.buildClassifier(d); Evaluation e=new Evaluation(d); e.evaluateModel(m,t); report(e,d); break;
            }
            default: throw new IllegalArgumentException("Unknown command");
        }
    }
}
