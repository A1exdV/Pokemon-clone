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

	public DamageDetails TakeDamage(Move move, Pokemon attacker)
	{
		float critical = 1f;
		if (Random.value * 100f < 6.25f)
			critical = 2f;
		float type = TypeChart.GetEffectiveness(move.Base.Type, this.Base.Type1) *
		             TypeChart.GetEffectiveness(move.Base.Type, this.Base.Type2);
		var damageDetails = new DamageDetails()
		{
			TypeEffectiveness = type,
			Critical = critical,
			Fainted = false
		};

		var attack = (move.Base.IsSpecial) ? attacker.SpAttack : attacker.Attack;
		var defence = (move.Base.IsSpecial) ? attacker.SpDefense : attacker.Defense;
		
		var modifiers = Random.Range(0.85f, 1f)* type*critical;
		var a = (2 * attacker.Level + 10) / 250f;
		var d = a * move.Base.Power * ((float)attack / defence) + 2;
		var damage = Mathf.FloorToInt(d * modifiers);
		HP -= damage;
		if (HP <= 0)
		{
			HP = 0;
			damageDetails.Fainted = true;
		}

		return damageDetails;
	}

	public Move GetRandomMove()
	{
		return Moves[Random.Range(0, Moves.Count)];
	}
}

public class  DamageDetails
{
	public bool Fainted { get; set; }
	public float Critical { get; set; }
	public float TypeEffectiveness { get; set; }
}