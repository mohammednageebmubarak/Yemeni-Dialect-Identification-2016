// ARCHIVAL TRANSCRIPTION — thesis pp. 71–73.
// Incomplete as printed; not a verified buildable source file.

using MathWorks.MATLAB.NET.Arrays;
using MathWorks.MATLAB.NET.Utility;
using FeatureExtraction;
using weka.classifiers;
using weka.core;
using weka.filters;
using java.io;

namespace Dilacts_Identification.Recording
{
    class Identifier
    {

        /*.............. Classification With Discrtiaze ........... */

        //load the Train and Test Datasets
        private Instances load(String filename)
        {
            Instances inputData;
            weka.core.converters.ArffLoader            reader            =             new weka.core.converters.ArffLoader();
            reader.setFile(new File(filename));
            inputData = reader.getDataSet();
            inputData.setClassIndex(inputData.numAttributes() - 1);
            return inputData;
        }
        private void save(Instances data, String filename)
        {
            weka.core.converters.ArffSaver writer;
            writer = new weka.core.converters.ArffSaver();
            writer.setInstances(data);
            writer.setFile(new File(filename));
            writer.writeBatch();
        }
        //performe Discritize Filter on both Train and Test DataSets
        private Instances Discretize(string type)
        {
            Instances inputTrain;
            Instances inputTest;
            Instances outputTrain;
            Instances outputTest;
            weka.filters.unsupervised.attribute.Discretize filter;
            // load data
            inputTrain                                                                   = load(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments)      + @"\Train-MelFCC.arff");
            inputTest                                                                    = load(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments)+@"\Test-MelFCC.arff");

            // setup filter
            filter = new weka.filters.unsupervised.attribute.Discretize();
            filter.setInputFormat(inputTrain);

            // apply filter
            outputTrain = weka.filters.Filter.useFilter(inputTrain, filter);
            outputTest = weka.filters.Filter.useFilter(inputTest, filter);

            if (type == "Train")
            {
                 return outputTrain;
            }
            else
            {
                 return outputTest;
            }

        }
        // Obtain Dialect from a given classlable
       private string GetDialect(string classlable)
        {
            if (classlable == "A")
            {
                Dialect = "Aden";
            }
            else if (classlable == "S")
            {
                 Dialect = "Sana'a";
            }
            else if (classlable == "D")
            {
                 Dialect = "Dalea";
            }
            else if (classlable == "L")
            {
                 Dialect = "Lahj";
            }
            else if (classlable == "H")
            {
                 Dialect = "Hadramout";
            }
            else
            {
                 Dialect = "Unkown";
            }
            return Dialect;
        }
        //Classification and Feature Extraction (Using Discrtize Filter)
        public string IdentifierEngine(string arffpath,string wavepath)
        {

            //mathlab (Feature Extraction Using LPCC)
            FExClass Ex = new FExClass();
            Ex.Coefmfplp((MWCharArray)"MelFCC",                        (MWNumericArray)1000,
(MWCharArray)arffpath,          (MWCharArray)wavepath,             (MWCharArray)"A,D,H,L,S",
(MWCharArray)"Dilacts", (MWCharArray)"H");

            // Weka (Classify Test data using NB)
            weka.classifiers.bayes.NaiveBayes             NB             =                new weka.classifiers.bayes.NaiveBayes();
            Instances Train = Discretize("Train");
            Instances Test = Discretize("Test");
            Train.setClassIndex(Train.numAttributes() - 1);
            Test.setClassIndex(Test.numAttributes() - 1);
            NB.buildClassifier(Train);
            double ClassIndex = NB.classifyInstance(Test.instance(0));
            string classlable = Test.classAttribute().value((int)ClassIndex);
            //identifying The Target
            return GetDialect(classlable);
        }

    }
}
