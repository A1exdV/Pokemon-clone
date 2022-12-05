using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pokemon
{
	private PokemonBase _base;
	private int level;

	public int HP { get; set; }
	public List<Move> Moves { get; set; }

	public Pokemon(PokemonBase pBase, int pLevel)
	{
		_base = pBase;
		level = pLevel;
		HP = _base.MaxHp;

		// Generate Moves
		Moves = new List<Move>();
		foreach (var move in _base.LearnableMoves)
		{
			if(move.Level<=level)
				Moves.Add(new Move(move.Base));
			
			if(Moves.Count>=4)
				break;
		}
	}

	public int Attack => Mathf.FloorToInt((_base.Attack * level) / 100f) + 5; // Формула из оригинальной игры 
	public int Defense => Mathf.FloorToInt((_base.Defense * level) / 100f) + 5;
	public int SpAttack => Mathf.FloorToInt((_base.SpAttack * level) / 100f) + 5;
	public int SpDefense => Mathf.FloorToInt((_base.SpDefense * level) / 100f) + 5;
	public int Speed => Mathf.FloorToInt((_base.Speed * level) / 100f) + 5;
	public int MaxHp => Mathf.FloorToInt((_base.MaxHp * level) / 100f) + 10;
}
