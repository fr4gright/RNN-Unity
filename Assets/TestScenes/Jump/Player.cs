using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using NeuralNetwork;

public class Player : MonoBehaviour {
	public GameObject currentGroundTile;
	public GameObject restartText;

	public double distanceInPercent;
	public double canJump;

	private const double timeBetweenDatasets = 0.3;
	float countedTime = 0;

	public NetLayer net;

	public void Start()
	{
		canJump = 1;
	}

	void Update()
	{
		countedTime += Time.deltaTime;

		this.transform.parent.position += Vector3.right * 3F * Time.deltaTime;

		if (Input.GetKeyDown(KeyCode.Space))
		{
			Jump();
		}
		else
		{
			if (countedTime > timeBetweenDatasets && !NetLayer.trained)
			{
				countedTime = 0;
				net.Train(canJump, 0);
			}
		}

		Vector3 startPointTile 	= currentGroundTile.transform.position -
				(Vector3.right * currentGroundTile.transform.localScale.x) / 2;
		Vector3 endPointTile 	= currentGroundTile.transform.position +
				(Vector3.right * currentGroundTile.transform.localScale.x) / 2;
		Vector3 platformLength 	= endPointTile - startPointTile; 
		Vector3 distanceToEndOfPlatform = endPointTile - transform.position;

		distanceInPercent = distanceToEndOfPlatform.x / platformLength.x;

		this.gameObject.GetComponent<LineRenderer>().SetPositions(
			new Vector3[] { transform.position, endPointTile });

		if (distanceToEndOfPlatform.x < 0)
			distanceToEndOfPlatform = Vector3.zero;

		IsGameOver();
	}

	public void OnTriggerEnter(Collider other)
	{
		canJump = 1;
		currentGroundTile = other.gameObject;
	}

	private void IsGameOver()
	{
		if (transform.position.y < -24)
		{
			restartText.SetActive(true);
			Time.timeScale = 0;
		}
	}

	public void Jump()
	{
		if (canJump == 1)
		{
			GameObject.Find("Network").GetComponent<NetLayer>().Train(1, 1);
			this.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * 450F);
			canJump = 0;
		}
	}
}
