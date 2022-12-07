using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPBarParty : MonoBehaviour
{
    [SerializeField] private Image health;

    public void SetHP(float hpNormalize)
    {
        health.fillAmount = hpNormalize;
    }
}