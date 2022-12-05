using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pokemon
{
	public PokemonBase Base { get; set; }
	public int Level { get; set; }

	public int HP { get; set; }
	public List<Move> Moves { get; set; }

	public Pokemon(PokemonBase pBase, int pLevel)
	{
		Base = pBase;
		Level = pLevel;
		HP = MaxHp;

		// Generate Moves
		Moves = new List<Move>();
		foreach (var move in Base.LearnableMoves)
		{
			if(move.Level<=Level)
				Moves.Add(new Move(move.Base));
			
			if(Moves.Count>=4)
				break;
		}
	}

	public int Attack => Mathf.FloorToInt((Base.Attack * Level) / 100f) + 5; // Формула из оригинальной игры 
	public int Defense => Mathf.FloorToInt((Base.Defense * Level) / 100f) + 5;
	public int SpAttack => Mathf.FloorToInt((Base.SpAttack * Level) / 100f) + 5;
	public int SpDefense => Mathf.FloorToInt((Base.SpDefense * Level) / 100f) + 5;
	public int Speed => Mathf.FloorToInt((Base.Speed * Level) / 100f) + 5;
	public int MaxHp => Mathf.FloorToInt((Base.MaxHp * Level) / 100f) + 10;
}
