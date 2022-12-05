using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBar : MonoBehaviour
{
	[SerializeField] private GameObject health;
	
	public void SetHP(float hpNormalize)
	{
		health.transform.localScale = new Vector3(hpNormalize, 1f);
	}
}
