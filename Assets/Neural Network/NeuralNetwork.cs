using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NeuralNetwork
{
	public class RNN
	{
		public double LearnRate { get; set; }
		public double Momentum 	{ get; set; }

		public List<Neuron> 		InputLayer		{ get; set; }
		public List<List<Neuron>> 	HiddenLayers	{ get; set; }
		public List<Neuron> 		OutputLayer		{ get; set; }

		private static readonly System.Random Random = new System.Random();

		public RNN(int inputSize, int hiddenSize, int outputSize)
		{
			int numHiddenLayers = 1;

			LearnRate 	= .4;
			Momentum 	= .9;

			InputLayer 		= new List<Neuron>();
			HiddenLayers 	= new List<List<Neuron>>();
			OutputLayer 	= new List<Neuron>();

			for (var i = 0; i < inputSize; i++)
				InputLayer.Add(new Neuron());

		for (int i = 0; i < numHiddenLayers; i++)
		{
			HiddenLayers.Add(new List<Neuron>());

			for (var j = 0; j < hiddenSize; j++)
				if (i == 0)
					HiddenLayers[i].Add(new Neuron(InputLayer));
				else
					HiddenLayers[i].Add(new Neuron(HiddenLayers[i - 1]));
		}

			for (var i = 0; i < outputSize; i++)
				OutputLayer.Add(new Neuron(HiddenLayers[numHiddenLayers - 1]));
		}

		public void Train(List<DataSet> dataSets, double minimumError)
		{
			var error 		= 1.0;
			var numEpochs 	= 0;

			while (error > minimumError && numEpochs < int.MaxValue)
			{
				var errors = new List<double>();

				foreach (var dataSet in dataSets)
				{
					ForwardPropagate(dataSet.Values);
					BackPropagate(dataSet.Targets);

					errors.Add(CalculateError(dataSet.Targets));
				}

				error = errors.Average();
				numEpochs++;
			}
		}

		private void ForwardPropagate(params double[] inputs)
		{
			var i = 0;

			InputLayer.ForEach(a => a.Value = inputs[i++]);
			foreach (var layer in HiddenLayers)
				layer.ForEach(a => a.CalculateValue());
			OutputLayer.ForEach(a => a.CalculateValue());
		}

		private void BackPropagate(params double[] targets)
		{
			var i = 0;

			OutputLayer.ForEach(a => a.CalculateGradient(targets[i++]));
			foreach(var layer in HiddenLayers.AsEnumerable<List<Neuron>>().Reverse())
			{
				layer.ForEach(a => a.CalculateGradient());
				layer.ForEach(a => a.UpdateWeights(LearnRate, Momentum));
			}
			OutputLayer.ForEach(a => a.UpdateWeights(LearnRate, Momentum));
		}

		public double[] Compute(params double[] inputs)
		{
			ForwardPropagate(inputs);

			return OutputLayer.Select(a => a.Value).ToArray();
		}

		private double CalculateError(params double[] targets)
		{
			var i = 0;

			return OutputLayer.Sum(
				a => Mathf.Abs((float) a.CalculateError(targets[i++])));
		}

		public static double GetRandom()
		{
			return (2 * Random.NextDouble()) - 1;
		}
	}
}