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
	Busy,
	PartyScreen
}
public class BattleSystem : MonoBehaviour
{
	[SerializeField] private BattleUnit playerUnit;
	[SerializeField] private BattleHud playerHud;
	
	[SerializeField] private BattleUnit enemyUnit;
	[SerializeField] private BattleHud enemyHud;

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
		playerHud.SetData(playerUnit.Pokemon);
		
		enemyUnit.Setup(_wildPokemon);
		enemyHud.SetData(enemyUnit.Pokemon);
		
		partyScreen.Initialization();
		
		dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);
		
		yield return dialogBox.TypeDialog($"A wild {enemyUnit.Pokemon.Base.Name} appeared.");

		StartCoroutine(PlayerAction());

	}

	private IEnumerator PlayerAction()
	{
		StartCoroutine(dialogBox.TypeDialog("Choose an action"));
		yield return new WaitForSeconds(0.5f);
		_state = BattleState.PlayerAction;
		dialogBox.EnableActionSelector(true);
	}
	
	private void OpenPartyScreen()
	{
		_state = BattleState.PartyScreen;
		partyScreen.SetPartyData(_playerParty.Pokemons);
		partyScreen.gameObject.SetActive(true);
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
		move.PP--;
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
		move.PP--;
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

			var nextPokemon = _playerParty.GetHealthyPokemon();
			if (nextPokemon != null)
			{
				OpenPartyScreen();
			}
			else if (OnBattleOver != null) OnBattleOver(false);
			
		}
		else
		{
			StartCoroutine(PlayerAction());
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
		
		dialogBox.UpdateActionSelection(_currentAction);

		if (Input.GetKeyDown(KeyCode.Z))
		{
			switch (_currentAction)
			{
				case 0: //Fight
					PlayerMove();
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
	        
	        dialogBox.UpdateMoveSelection(_currentMove, playerUnit.Pokemon.Moves[_currentMove]);

	        if (Input.GetKeyDown(KeyCode.Z))
	        {
		        dialogBox.EnableMoveSelector(false);
		        dialogBox.EnableDialogText(true);
		        StartCoroutine(PerformPlayerMove());
	        }
	        else if (Input.GetKeyDown(KeyCode.X))
	        {
		        dialogBox.EnableMoveSelector(false);
		        dialogBox.EnableDialogText(true);
		        StartCoroutine(PlayerAction());
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
			StartCoroutine(PlayerAction());
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
		playerHud.SetData(newPokemon);

		dialogBox.SetMoveNames(newPokemon.Moves);
		
		yield return dialogBox.TypeDialog($"Go {newPokemon.Base.Name}!");

		StartCoroutine(EnemyMove());
	}
}
