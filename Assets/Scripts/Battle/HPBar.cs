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

	public IEnumerator SetHPSmooth(float newHp)
	{
		float currentHp = health.transform.localScale.x;
		float changeAmt = currentHp - newHp;

		while (currentHp - newHp > Mathf.Epsilon)
		{
			currentHp -= changeAmt * Time.deltaTime;
			health.transform.localScale = new Vector3(currentHp, 1f);
			yield return null;
		}
		health.transform.localScale = new Vector3(newHp, 1f);
	}
}
