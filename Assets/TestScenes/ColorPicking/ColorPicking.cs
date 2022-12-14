using UnityEngine;
using System.Collections;
using NeuralNetwork;
using UnityEngine.UI;
using System.Collections.Generic;

public class ColorPicking : MonoBehaviour {
	private const double MinimumError = 0.1;
	private static RNN rnn;
	private static List<DataSet> dataSets;

	public Image I1;
	public Image I2;

	public GameObject pointer1;
	public GameObject pointer2;

	bool isTrained;

	int i = 0;

	void Start()
	{
		rnn = new RNN(3, 4, 1);
		dataSets = new List<DataSet>();

		Next();
	}

	void Next()
	{
		Color c = new Color(
			Random.Range(0, 1f),
			Random.Range(0, 1f),
			Random.Range(0, 1f));

		I1.color = c;
		I2.color = c;

		double[] C = {
			(double) I1.color.r,
			(double) I1.color.g,
			(double) I1.color.b
		};

		if(isTrained)
		{
			double d = tryValues(C);

			if(d > 0.5)
			{
				pointer1.SetActive(false);
				pointer2.SetActive(true);
			}
			else
			{
				pointer1.SetActive(true);
				pointer2.SetActive(false);
			}
		}
	}
	
	public void Train(float val)
	{
		double[] C = {
			(double) I1.color.r,
			(double) I1.color.g,
			(double) I1.color.b
		};

		double[] v = { (double) val };

		dataSets.Add(new DataSet(C, v));

		i++;
		if(!isTrained && (i % 10) == 9)
			Train();

		Next();
	}

	private void Train()
	{
		rnn.Train(dataSets, MinimumError);
		isTrained = true;
	}

	double tryValues(double[] vals)
	{
	 	double[] result = rnn.Compute(vals);

	 	return result[0];
	}
}