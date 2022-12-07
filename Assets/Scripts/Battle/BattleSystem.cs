using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum BattleState
{
	Start,
	ActionSelection,
	MoveSelection,
	PerformMove,
	Busy,
	PartyScreen,
	BattleOver
}
public class BattleSystem : MonoBehaviour
{
	[SerializeField] private BattleUnit playerUnit;

	[SerializeField] private BattleUnit enemyUnit;

	[SerializeField] private BattleDialogBox dialogBox;

	[SerializeField] private PartyScreen partyScreen;

	public event Action<bool> OnBattleOver;
	private BattleState _state;
	private int _currentAction; //0-Fight,1-Run
	private int _currentMove;
	private int _currentMember;

	private PokemonParty _playerParty;
	private Pokemon _wildPokemon;
	
	public void StartBattle(PokemonParty playerParty,Pokemon wildPokemon)
	{
		_playerParty = playerParty;
		_wildPokemon = wildPokemon;
		StartCoroutine(SetupBattle());
	}

	private IEnumerator SetupBattle()
	{
		playerUnit.Setup(_playerParty.GetHealthyPokemon());

		enemyUnit.Setup(_wildPokemon);

		partyScreen.Initialization();
		
		dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);
		
		yield return dialogBox.TypeDialog($"A wild {enemyUnit.Pokemon.Base.Name} appeared.");

		StartCoroutine(ActionSelection());

	}

	private void BattleOver(bool won)
	{
		_state = BattleState.BattleOver;
		if (OnBattleOver != null) OnBattleOver(won);
	}
	private IEnumerator ActionSelection()
	{
		StartCoroutine(dialogBox.TypeDialog("Choose an action"));
		yield return new WaitForSeconds(0.5f);
		_state = BattleState.ActionSelection;
		dialogBox.EnableActionSelector(true);
	}
	
	private void OpenPartyScreen()
	{
		_state = BattleState.PartyScreen;
		partyScreen.SetPartyData(_playerParty.Pokemons);
		partyScreen.gameObject.SetActive(true);
	}

	private void MoveSelection()
	{
		_state = BattleState.MoveSelection;
		dialogBox.EnableActionSelector(false);
		dialogBox.EnableDialogText(false);
		dialogBox.EnableMoveSelector(true);
	}
	private IEnumerator PlayerMove()
	{
		_state = BattleState.PerformMove;
		
		var move = playerUnit.Pokemon.Moves[_currentMove];
		yield return RunMove(playerUnit,enemyUnit,move);
		
		//if the battle state was not changed by RunMove, then go to next step
		if(_state ==BattleState.PerformMove)
			StartCoroutine(EnemyMove());
	}

	private IEnumerator EnemyMove()
	{
		_state = BattleState.PerformMove;
		
		var move = enemyUnit.Pokemon.GetRandomMove();
		yield return RunMove(enemyUnit,playerUnit,move);
		
		//if the battle state was not changed by RunMove, then go to next step
		if(_state ==BattleState.PerformMove)
			StartCoroutine(ActionSelection());
	}
	
	private IEnumerator RunMove(BattleUnit sourceUnit,BattleUnit targetUnit, Move move)
	{
		move.PP--;
		yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.Name} used {move.Base.Name}");
		
		sourceUnit.PlayAttackAnimation();
		yield return new WaitForSeconds(1f);

		targetUnit.PlayHitAnimation();
		
		var damageDetails = targetUnit.Pokemon.TakeDamage(move, targetUnit.Pokemon);
		
		yield return targetUnit.Hud.UpdateHP();
		yield return ShowDamageDetails(damageDetails);
		
		if (damageDetails.Fainted)
		{
			yield return dialogBox.TypeDialog($"{targetUnit.Pokemon.Base.Name} Fainted");
			targetUnit.PlayFaintAnimation();

			yield return new WaitForSeconds(2f);
			
			CheckForBattleOver(targetUnit);
		}
	}

	void CheckForBattleOver(BattleUnit faintedUnit)
	{
		if (faintedUnit.IsPlayerUnit)
		{
			var nextPokemon = _playerParty.GetHealthyPokemon();
			if (nextPokemon != null)
			{
				OpenPartyScreen();
			}
			else 
				BattleOver(false);
		}
		else 
			BattleOver(true);
		
	}

	private IEnumerator ShowDamageDetails(DamageDetails damageDetails)
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
			case BattleState.ActionSelection:
				HandleActionSelection();
				break;
			case BattleState.MoveSelection:
				HandleMoveSelection();
				break;
			case BattleState.PartyScreen:
				HandlePartySelection();
				break;
		}

	}

	

	private void HandleActionSelection()
	{
		if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			_currentAction++;
		}
		else if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			_currentAction--;
		}
		else if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			_currentAction += 2;
		}
		else if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			_currentAction-=2;
		}

		_currentAction = Mathf.Clamp(_currentAction, 0, 3);
		
		dialogBox.UpdateActionArrowSelection(_currentAction);

		if (Input.GetKeyDown(KeyCode.Z))
		{
			switch (_currentAction)
			{
				case 0: //Fight
					MoveSelection();
					break;
				case 1: //Bag
					
					break;
				case 2: //Pokemon
					OpenPartyScreen();
					break;
				case 3: //Run
					
					break;
			}
		}
	}
	private void HandleMoveSelection()
     	{
	        if (Input.GetKeyDown(KeyCode.DownArrow))
	        {
		        _currentMove += 2;
	        }
	        else if(Input.GetKeyDown(KeyCode.UpArrow))
	        {
		        _currentMove -= 2;
	        }
	        if (Input.GetKeyDown(KeyCode.RightArrow))
	        {
		        _currentMove++;
	        }
	        else if(Input.GetKeyDown(KeyCode.LeftArrow))
	        {
		        _currentMove--;
	        }
	        
	        _currentMove = Mathf.Clamp(_currentMove, 0, playerUnit.Pokemon.Moves.Count-1);
	        
	        dialogBox.UpdateMoveArrowSelection(_currentMove, playerUnit.Pokemon.Moves[_currentMove]);

	        if (Input.GetKeyDown(KeyCode.Z))
	        {
		        dialogBox.EnableMoveSelector(false);
		        dialogBox.EnableDialogText(true);
		        StartCoroutine(PlayerMove());
	        }
	        else if (Input.GetKeyDown(KeyCode.X))
	        {
		        dialogBox.EnableMoveSelector(false);
		        dialogBox.EnableDialogText(true);
		        StartCoroutine(ActionSelection());
	        }
     	}

	private void HandlePartySelection()
	{
		if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			_currentMember += 2;
		}
		else if(Input.GetKeyDown(KeyCode.UpArrow))
		{
			_currentMember -= 2;
		}
		if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			_currentMember++;
		}
		else if(Input.GetKeyDown(KeyCode.LeftArrow))
		{
			_currentMember--;
		}
		_currentMember = Mathf.Clamp(_currentMember, 0, _playerParty.Pokemons.Count-1);
		
		partyScreen.UpdateMemberSelection(_currentMember);

		if (Input.GetKeyDown(KeyCode.Z))
		{
			var selectedMember = _playerParty.Pokemons[_currentMember];
			if (selectedMember.HP <= 0)
			{
				partyScreen.SetMessageText("You can't send a fainted pokemon");
				return;
			}

			if (selectedMember == playerUnit.Pokemon)
			{
				partyScreen.SetMessageText("This pokemon is already in battle");
				return;
			}
			partyScreen.gameObject.SetActive(false);
			_state = BattleState.Busy;
			StartCoroutine(SwitchPokemon(selectedMember));
		}
		else if (Input.GetKeyDown(KeyCode.X))
		{
			partyScreen.gameObject.SetActive(false);
			StartCoroutine(ActionSelection());
		}
	}

	IEnumerator SwitchPokemon(Pokemon newPokemon)
	{
		dialogBox.EnableActionSelector(false);
		if (playerUnit.Pokemon.HP > 0)
		{
			yield return dialogBox.TypeDialog($"Come back {playerUnit.Pokemon.Base.Name}");
			playerUnit.PlayFaintAnimation();
			yield return new WaitForSeconds(2f);
		}
		playerUnit.Setup(newPokemon);

		dialogBox.SetMoveNames(newPokemon.Moves);
		
		yield return dialogBox.TypeDialog($"Go {newPokemon.Base.Name}!");

		StartCoroutine(EnemyMove());
	}
}
