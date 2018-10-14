using System;
using MathNet.Numerics.LinearAlgebra;

namespace SnakeGameUsingNeuralNetwork
{
    public class NeuralNetwork
    {
        Vector<double> m_InputVector;
        Vector<double> m_InputToHiddenBias;
        Vector<double> m_OutputVector;
        Vector<double> m_HiddenToOutputBias;
        Matrix<double> m_InputToHidden_WeightsMatrix;
        Matrix<double> m_HiddenToOutput_WeightsMatrix;
        Vector<double> m_HiddenVector;

        public NeuralNetwork(int i_NumberOfInputs, int i_NumberOfHiddenNodes, int i_NumberOfOutputNodes)
        {
            // init empty vectors, based on the number of nodes.
            m_InputVector = Vector<double>.Build.Dense(i_NumberOfInputs);
            m_OutputVector = Vector<double>.Build.Dense(i_NumberOfOutputNodes);
            m_HiddenVector = Vector<double>.Build.Dense(i_NumberOfHiddenNodes);

            // init the weights and bais as random for starting.
            m_InputToHiddenBias = Vector<double>.Build.Random(i_NumberOfHiddenNodes);
            m_HiddenToOutputBias = Vector<double>.Build.Random(i_NumberOfOutputNodes);
            m_InputToHidden_WeightsMatrix = Matrix<double>.Build.Random(i_NumberOfHiddenNodes, i_NumberOfInputs);
            m_HiddenToOutput_WeightsMatrix = Matrix<double>.Build.Random(i_NumberOfOutputNodes, i_NumberOfHiddenNodes);
        }

        public double[] FeedForward(double[] input)
        {
            //Set the input to the input vector.
            m_InputVector.SetValues(input);

            //Generate hidden vector values (first multypliy, then add bias. then use sigmoid for normiliztion)
            m_HiddenVector = m_InputToHidden_WeightsMatrix.Multiply(m_InputVector);
            m_HiddenVector = m_HiddenVector.Add(m_InputToHiddenBias);
            m_HiddenVector = m_HiddenVector.Map(Sigmoid);

            //Same thing as above for Output vector.
            m_OutputVector = m_HiddenToOutput_WeightsMatrix.Multiply(m_HiddenVector);
            m_OutputVector = m_OutputVector.Add(m_HiddenToOutputBias);
            m_OutputVector = m_OutputVector.Map(Sigmoid);

            //Returing the output vector as array.
            return m_OutputVector.ToArray();
        }

        private double Sigmoid(double x)
        {
            // The activation function.
            return 1 / (1 + System.Math.Exp(-x));
        }

        internal void PrintCurrentNetwork(int x, int y)
        {
            // printing the weights matrixs to location (x,y) on the console.
            Console.SetCursorPosition(x,y);
            Console.WriteLine("Input To Hidden Weights: " + m_InputToHidden_WeightsMatrix);
            Console.WriteLine("Hidden To Output Weights: " + m_HiddenToOutput_WeightsMatrix);
        }

        private double SigmoidDerivative(double x)
        {
            // Derivative is : Sigmoid(x) * (1 - Sigmoid(x))
            // Becuase we already calculate sigmoid(x) when we feed forward. then we use only x.
            return x * (1 - x);
        }

        public void TrainNetwork(double[] Inputs, double[] KnownAnswers, double LearningRate)
        {
            // Output_Error is vector that contain the errors from the hidden layer to output layer.
            // Hidden_Error is vector that contain the errors from the Input layer to Hidden layer.
            Vector<double> Output_Error;
            Vector<double> Hidden_Error;

            // Starting train process, with feeding forward
            FeedForward(Inputs);

            // Now start backpropgation:
            // Calculate the outputError Vector (KnownAnswer - guess)
            Output_Error = Vector<double>.Build.Dense(KnownAnswers.Length);
            Output_Error.SetValues(KnownAnswers);
            Output_Error = Output_Error.Subtract(m_OutputVector);

            // Calculate the gradient of the output
            Matrix<double> Output_Gradients = m_OutputVector.Clone().ToColumnMatrix();
            Output_Gradients = Output_Gradients.Map(SigmoidDerivative);
            Output_Gradients = Output_Gradients.PointwiseMultiply(Output_Error.ToColumnMatrix());
            Output_Gradients = Output_Gradients.Multiply(LearningRate);

            // Calculate the deltas (and adjust weights and bais)
            Matrix<double> HiddenToOutputWeightDeltas = Output_Gradients.Multiply(m_HiddenVector.ToRowMatrix());
            m_HiddenToOutput_WeightsMatrix = m_HiddenToOutput_WeightsMatrix.Add(HiddenToOutputWeightDeltas);
            m_HiddenToOutputBias = m_HiddenToOutputBias.Add(Output_Gradients.Column(0));

            // Calculate the error of the hidden layer. (based on output error)
            // Using the Transpose of the weightMatrix that's becuase we going backward now..
            Hidden_Error = m_HiddenToOutput_WeightsMatrix.Transpose().Multiply(Output_Error);

            // Calculate the gradient of the Hidden
            Matrix<double> Hidden_Gradients = m_HiddenVector.Clone().ToColumnMatrix();
            Hidden_Gradients = Hidden_Gradients.Map(SigmoidDerivative);
            Hidden_Gradients = Hidden_Gradients.PointwiseMultiply(Hidden_Error.ToColumnMatrix());
            Hidden_Gradients = Hidden_Gradients.Multiply(LearningRate);

            // Calculate the deltas (and adjust weights and bais)
            Matrix<double> InputToHiddenWeightDeltas = Hidden_Gradients.Multiply(m_InputVector.ToRowMatrix());
            m_InputToHidden_WeightsMatrix = m_InputToHidden_WeightsMatrix.Add(InputToHiddenWeightDeltas);
            m_InputToHiddenBias = m_InputToHiddenBias.Add(Hidden_Gradients.Column(0));
        }


    }
}
