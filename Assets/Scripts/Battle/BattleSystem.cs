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

	private BattleState _state;
	private int _currentAction; //0-Fight,1-Run
	private int _currentMove;
	
	private void Start()
	{
		StartCoroutine(SetupBattle());
	}

	private IEnumerator SetupBattle()
	{
		playerUnit.Setup();
		playerHud.SetData(playerUnit.Pokemon);
		
		enemyUnit.Setup();
		enemyHud.SetData(playerUnit.Pokemon);
		
		dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);
		
		yield return dialogBox.TypeDialog($"A wild {enemyUnit.Pokemon.Base.Name} appeared.");
		yield return new WaitForSeconds(1f);

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

	private void Update()
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
     	}
}
