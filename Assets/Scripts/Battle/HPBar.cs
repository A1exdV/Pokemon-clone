using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
	[SerializeField] private Image health;

	[SerializeField] private Sprite full;
	[SerializeField] private Sprite middle;
	[SerializeField] private Sprite low;

	public void SetHP(float hpNormalize)
	{
		health.fillAmount = hpNormalize;
		ChangeHpColor();
	}

	public IEnumerator SetHpSmooth(float newHp)
	{
		float currentHp = health.fillAmount;
		float changeAmt = currentHp - newHp;

		while (currentHp - newHp > Mathf.Epsilon)
		{
			currentHp -= changeAmt * Time.deltaTime;
			health.fillAmount = currentHp;
			ChangeHpColor();
			yield return null;
		}
		health.fillAmount = newHp;
		
	}

	private void ChangeHpColor()
	{
		if (health.fillAmount > 0.5f)
		{
			health.sprite = full;
		}
		else if (health.fillAmount > 0.2f)
		{
			health.sprite = middle;
		}
		else
		{
			health.sprite = low;
		}
	}
}
