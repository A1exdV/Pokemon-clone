using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum BattleState
{
	Start,
	PlayerAction,
	PlayerMove,
	EnemyMove,
	Busy
}
public class BattleSystem : MonoBehaviour
{
	[SerializeField] private BattleUnit playerUnit;
	[SerializeField] private BattleHud playerHud;
	
	[SerializeField] private BattleUnit enemyUnit;
	[SerializeField] private BattleHud enemyHud;

	[SerializeField] private BattleDialogBox dialogBox;

	public event Action<bool> OnBattleOver;
	private BattleState _state;
	private int _currentAction; //0-Fight,1-Run
	private int _currentMove;
	
	public void StartBattle()
	{
		StartCoroutine(SetupBattle());
	}

	private IEnumerator SetupBattle()
	{
		playerUnit.Setup();
		playerHud.SetData(playerUnit.Pokemon);
		
		enemyUnit.Setup();
		enemyHud.SetData(enemyUnit.Pokemon);
		
		dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);
		
		yield return dialogBox.TypeDialog($"A wild {enemyUnit.Pokemon.Base.Name} appeared.");

		PlayerAction();

	}

	private void PlayerAction()
	{
		_state = BattleState.PlayerAction;
		StartCoroutine(dialogBox.TypeDialog("Choose an action"));
		dialogBox.EnableActionSelector(true);
	}

	private void PlayerMove()
	{
		_state = BattleState.PlayerMove;
		dialogBox.EnableActionSelector(false);
		dialogBox.EnableDialogText(false);
		dialogBox.EnableMoveSelector(true);
	}
	private IEnumerator PerformPlayerMove()
	{
		_state = BattleState.Busy;
		
		var move = playerUnit.Pokemon.Moves[_currentMove];
		yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} used {move.Base.Name}");
		
		playerUnit.PlayAttackAnimation();
		yield return new WaitForSeconds(1f);

		enemyUnit.PlayHitAnimation();
		
		var damageDetails = enemyUnit.Pokemon.TakeDamage(move, enemyUnit.Pokemon);
		
		yield return enemyHud.UpdateHP();
		yield return ShowDamageDetails(damageDetails);
		
		if (damageDetails.Fainted)
		{
			yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.Name} Fainted");
			enemyUnit.PlayFaintAnimation();

			yield return new WaitForSeconds(2f);
			if (OnBattleOver != null) OnBattleOver(true);
		}
		else
		{
			StartCoroutine(EnemyMove());
		}
		
	}

	private IEnumerator EnemyMove()
	{
		_state = BattleState.EnemyMove;
		var move = enemyUnit.Pokemon.GetRandomMove();
		
		yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.Name} used {move.Base.Name}");
		
		enemyUnit.PlayAttackAnimation();
		yield return new WaitForSeconds(1f);
		
		playerUnit.PlayHitAnimation();

		var damageDetails = playerUnit.Pokemon.TakeDamage(move, enemyUnit.Pokemon);
		
		yield return playerHud.UpdateHP();
		yield return ShowDamageDetails(damageDetails);
		
		if (damageDetails.Fainted)
		{
			yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} Fainted");
			playerUnit.PlayFaintAnimation();
			
			yield return new WaitForSeconds(2f);
			if (OnBattleOver != null) OnBattleOver(false);
			
		}
		else
		{
			PlayerAction();
		}
	}

	IEnumerator ShowDamageDetails(DamageDetails damageDetails)
	{
		if (damageDetails.Critical > 1f)
			yield return dialogBox.TypeDialog("A Critical Hit!");
		
		if(damageDetails.TypeEffectiveness>1f)
			yield return dialogBox.TypeDialog("It's super effective!");
		else if(damageDetails.TypeEffectiveness<1f)
			yield return dialogBox.TypeDialog("It's not very effective!");
			
	}
	public void HandleUpdate()
	{
		switch (_state)
		{
			case BattleState.PlayerAction:
				HandleActionSelection();
				break;
			case BattleState.PlayerMove:
				HandleMoveSelection();
				break;
		}

	}

	

	private void HandleActionSelection()
	{
		if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			if (_currentAction < 1)
				++_currentAction;
		}
		else if(Input.GetKeyDown(KeyCode.UpArrow))
		{
			if (_currentAction > 0)
				--_currentAction;
		}
		
		dialogBox.UpdateActionSelection(_currentAction);

		if (Input.GetKeyDown(KeyCode.Z))
		{
			switch (_currentAction)
			{
				case 0: //Fight
					PlayerMove();
					break;
				
				case 1: //Run
					
					break;
			}
		}
	}
	private void HandleMoveSelection()
     	{
	        if (Input.GetKeyDown(KeyCode.DownArrow))
	        {
		        if (_currentMove < playerUnit.Pokemon.Moves.Count - 2)
			        _currentMove += 2;
	        }
	        else if(Input.GetKeyDown(KeyCode.UpArrow))
	        {
		        if (_currentMove > 1)
			        _currentMove -= 2;
	        }
	        if (Input.GetKeyDown(KeyCode.RightArrow))
	        {
		        if (_currentMove < playerUnit.Pokemon.Moves.Count - 1)
			        ++_currentMove;
	        }
	        else if(Input.GetKeyDown(KeyCode.LeftArrow))
	        {
		        if (_currentMove > 0)
			        --_currentMove;
	        }
	        dialogBox.UpdateMoveSelection(_currentMove, playerUnit.Pokemon.Moves[_currentMove]);

	        if (Input.GetKeyDown(KeyCode.Z))
	        {
		        dialogBox.EnableMoveSelector(false);
		        dialogBox.EnableDialogText(true);
		        StartCoroutine(PerformPlayerMove());
	        }
     	}

	
}
