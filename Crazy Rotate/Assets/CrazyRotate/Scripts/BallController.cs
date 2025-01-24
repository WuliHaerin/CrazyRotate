﻿using UnityEngine;
using System.Collections;

public class BallController : MonoBehaviour 
{
	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.transform.tag != transform.tag)
		{
			if(!GameController.instance.isCancelAd)
            {
				GameController.instance.PreOver();
			}
		}
		if (other.transform.tag == transform.tag)
		{
			GameController.instance.Score();
		}
		gameObject.SetActive(false);
		Destroy(gameObject, 3.0f);

	}
}
