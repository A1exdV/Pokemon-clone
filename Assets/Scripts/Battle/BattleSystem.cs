using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
	[SerializeField] private BattleUnit playerUnit;
	[SerializeField] private BattleHud playerHud;
	
	[SerializeField] private BattleUnit enemyUnit;
	[SerializeField] private BattleHud enemyHud;

	private void Start()
	{
		SetupBattle();
	}

	private void SetupBattle()
	{
		playerUnit.Setup();
		playerHud.SetData(playerUnit.Pokemon);
		
		enemyUnit.Setup();
		enemyHud.SetData(playerUnit.Pokemon);
	}
}
