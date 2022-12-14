using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NeuralNetwork;
using UnityEngine.UI;
using System.Threading;

public class NetLayer : MonoBehaviour {
	private const double MinimumError = 0.1;
	private static RNN rnn;
	public static List<DataSet> dataSets;
	public static bool trained;

	private int 		collectedDatasets 	= 0;
	private const int 	maxNumberOfDatasets = 60;

	public Player player;

	void Start()
	{
		rnn = new RNN(2, 3, 1);
		dataSets = new List<DataSet>();
	}
	
	void Update()
	{
		if (trained) {
			double result = Compute(
				new double[] { player.distanceInPercent, player.canJump });

			if (result > 0.5)
				player.Jump();
		}
	}

	public void Train(double canJump, double jumped)
	{
		double[] C = { player.distanceInPercent, canJump };
		double[] v = { jumped };

		dataSets.Add(new DataSet(C, v));

		collectedDatasets++;
		if (!trained && collectedDatasets == maxNumberOfDatasets)
			TrainNetwork();
	}

	double Compute(double[] vals)
	{
		double[] result = rnn.Compute(vals);

		return result[0];
	}

	public static void TrainNetwork()
	{
		rnn.Train(dataSets, MinimumError);
		trained = true;
	}
}