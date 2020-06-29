﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNN
{

    class LConnection
    {
        #region Delegates
        private delegate double Functions_Derivative(double neural);
        Functions_Derivative Function_Derivative;
        #endregion

        #region Global Variables

        private Layer Input_Layer;
        private Layer Output_Layer;

        private int ILLength;//Input Layer Length
        private int OLLength;//Output Layer Length

        private double[] Weight;//weight array is one dimention for easy fast saving
        private double[] Bias;

        public int[] WeightBackMap;//store map indexing for backprobagation

        private int WLength;//Weigth Length
        private int BLength;//Bias Length

        public double LearningRate { get; set; }

        #endregion

        #region Constructors
        public LConnection(Layer input_layer,Layer output_layer,Random rand)
        {
            Input_Layer = input_layer;//get input layer
            Output_Layer = output_layer;//get output layer

            ILLength = Input_Layer.Neurons_Length;//store the length of input layer
            OLLength = Output_Layer.Neurons_Length;//store the length of output layer

            WLength = OLLength * ILLength;//get weight array length
            BLength = Output_Layer.Neurons_Length;//get bias array length

            LearningRate = 0.1;//default learnig rate 

            Weight = new double[WLength];//create weight array 
            Bias = new double[BLength];//create bias array

            WeightBackMap = new int[WLength];//create weight back map array 

            CreateWeightBackMap();//create weight back map for backprobagation 

           //Random rand = new Random();//Initialize random weights and biases
            for (int i = 0; i < WLength; i++)
                //Weight[i] = 0.1;
           Weight[i] = (double)rand.Next(-200, 200) / 1000;//get random from -0.2 to 0.2

            for (int i = 0; i < BLength; i++)
               // Bias[i] = 0.1;
            Bias[i] = (double)rand.Next(-200, 200) / 1000;//end Initializing     

            switch (Input_Layer.GetActivatonFunction())//get input layer activation function and set the function derivative delegate
            {
                case Layer.ActivationFunction.Sigmoid:
                    Function_Derivative = SigmoidDerivative;
                    break;
                case Layer.ActivationFunction.TanH:
                    Function_Derivative = TanHDerivative;
                    break;
                case Layer.ActivationFunction.ReLU:
                    Function_Derivative = ReLUDerivative;
                    break;
                case Layer.ActivationFunction.LeakyReLU:
                    Function_Derivative = LeakyReLUDerivative;
                    break;
                case Layer.ActivationFunction.BinaryStep:
                    Function_Derivative = BinaryStepDerivative;
                    break;
                case Layer.ActivationFunction.Softmax:
                    Function_Derivative = SoftmaxDerivative;
                    break;
                default:
                    throw new ArgumentException("Activation Function don't exist");
                    
            }

        }
        #endregion

        #region Methods

        private void CreateWeightBackMap()
        {
            /* get transpose matrix n*m full with whole numbers  ex 
               [0,1,2] transpose [0,3]
               [3,4,5]           [1,4] and then store it in 1 dimensional array ex [0,3,1,4,2,5]
                                 [2,5]
             */
            int Shift = 0;//shift with the length of input layer
            int WBMIndex = 0;//WeightBackMap indexer

            for (int i = 0; i < ILLength; i++)
            {
                Shift = 0;
                for (int j = 0; j < OLLength; j++)
                {
                    WeightBackMap[WBMIndex] = Shift + i;
                    Shift += ILLength;
                    WBMIndex++;
                }
            }
        }

        public void FeedForward()
        {
            int WeightIndex = 0;//weight indexer
            double Z = 0;
            for (int i = 0; i < OLLength; i++)
            {
                Z = 0;//reset Z
                for (int j = 0; j < ILLength; j++)
                {
                    Z += Input_Layer[j] * Weight[WeightIndex];//first index of output neurons = sum of input layer * weigts
                    WeightIndex++;
                }
                Output_Layer[i] = Z + Bias[i];//set output layer neuron and it will activate automatically               
            }
        }
        public void BackPropagateDelta()
        {     
            int WBMIndexer = 0;//WeightBackMap Indexer
            double DeltaSum = 0;
            for (int i = 0; i < ILLength; i++)
            {
                DeltaSum = 0;//reset DeltaSum
                for (int j = 0; j < OLLength; j++)
                {
                    DeltaSum += Output_Layer.Delta[j] * Weight[WeightBackMap[WBMIndexer]];//sum delta of output layer
                    WBMIndexer++;
                }
                Input_Layer.Delta[i] = DeltaSum * Function_Derivative(Input_Layer[i]);//update delta of input layer          
            }

        }
        public void UpdateWeights()
        {
            int WIndexer = 0;//Weight Indexer
            for (int i = 0; i < OLLength; i++)
            {
                for (int j = 0; j < ILLength; j++)
                {
                    Weight[WIndexer] -= LearningRate * Input_Layer[j] * Output_Layer.Delta[i];
                    WIndexer++;
                }
                Bias[i] -= LearningRate * Output_Layer.Delta[i];
            }
        }
      
        public void FeedBackward()
        {
            int WeightIndex = 0;//weight indexer
            double Z = 0;
            for (int i = 0; i < ILLength; i++)
            {
                Z = 0;
                for (int j = 0; j < OLLength; j++)
                {
                    Z += Output_Layer[j] * Weight[WeightBackMap[WeightIndex]];//first index of output neurons = sum of input layer * weigts
                    WeightIndex++;
                }
                Input_Layer[i] = Z;//set output layer neuron and it will activate automatically

            }
        }
        public void ForwardPropagateDelta()
        {
            int WBMIndexer = 0;//WeightBackMap Indexer
            double DeltaSum = 0;
            for (int i = 0; i < OLLength; i++)
            {
                DeltaSum = 0;
                for (int j = 0; j < ILLength; j++)
                {
                    DeltaSum += Input_Layer.Delta[j] * Weight[WBMIndexer];//sum delta of output layer
                    WBMIndexer++;
                }
                Output_Layer.Delta[i] = DeltaSum * Function_Derivative(Output_Layer[i]);//update delta of input layer          
            }

        }
        public void UpdateWeightsBackward()
        {
            int WIndexer = 0;//Weight Indexer
            for (int i = 0; i < ILLength; i++)
            {
                for (int j = 0; j < OLLength; j++)
                {
                    Weight[WeightBackMap[WIndexer]] -= LearningRate * Output_Layer[j] * Input_Layer.Delta[i];
                    WIndexer++;
                }              
            }
        }

        #endregion

        #region Derivative Functions 

        private double SigmoidDerivative(double Neural)
        {
            return (Neural * (1 - Neural));
        }
        private double TanHDerivative(double Neural)
        {
            return (1 - Neural * Neural);
        }
        private double ReLUDerivative(double Neural)
        {
            if (Neural > 0)
                return 1;
            else
                return 0;
        }
        private double LeakyReLUDerivative(double Neural)
        {
            if (Neural > 0)
                return 1;
            else
                return 0.01;
        }
        private double SoftmaxDerivative(double Neural)
        {
            return Neural * (1 - Neural);
        }
        private double BinaryStepDerivative(double Neural)
        {
            return 0;
        }
        #endregion
    }
}
