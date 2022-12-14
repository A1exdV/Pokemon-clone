using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pokemon", menuName = "Pokemon/Create new pokemon")]
public class PokemonBase : ScriptableObject
{
	[SerializeField]private new string name;
	
	[TextArea]
	[SerializeField]private string description;

	[SerializeField] private Sprite frontSprite;
	[SerializeField] private Sprite backSprite;
	
	[SerializeField] private PokemonType type1;
	[SerializeField] private PokemonType type2;
	
	[SerializeField] private int maxHp;
	[SerializeField] private int attack;
	[SerializeField] private int defense;
	[SerializeField] private int spAttack;
	[SerializeField] private int spDefense;
	[SerializeField] private int speed;

	[SerializeField] private List<LearnableMove> learnableMoves;

	public string Name
	{
		get { return name; }
	}
	public string Description
	{
		get { return description; }
	}
	
	public Sprite FrontSprite
	{
		get { return frontSprite; }
	}
	public Sprite BackSprite
	{
		get { return backSprite; }
	}
	public PokemonType Type1
	{
		get { return type1; }
	}
	public PokemonType Type2
	{
		get { return type2; }
	}
	public int MaxHp
	{
		get { return maxHp; }
	}
	public int Attack
	{
		get { return attack; }
	}
	public int Defense
	{
		get { return defense; }
	}
	public int SpAttack
	{
		get { return spAttack; }
	}
	public int SpDefense
	{
		get { return spDefense; }
	}
	public int Speed
	{
		get { return speed; }
	}
	public List<LearnableMove> LearnableMoves
	{
		get { return learnableMoves; }
	}
}
[System.Serializable]
public class LearnableMove
{
	[SerializeField] private MoveBase moveBase;
	[SerializeField] private int level;
	
	public MoveBase Base
	{
		get { return moveBase; }
	}
	public int Level
	{
		get { return level; }
	}
}
public enum PokemonType
{
	None,
	Normal,
	Fire,
	Water,
	Electric,
	Grass,
	Ice,
	Fighting,
	Poison,
	Ground,
	Flying,
	Psychic,
	Bug,
	Rock,
	Ghost,
	Dragon,
}

public static class TypeChart
{
	static float[][] chart =
	{
		//                       Nor   Fir   Wat   Ele   Gra   Ice   Fig   Poi   Gro   Fly   Psy   Bug   Roc   Gho   Dra
        /*Normal*/  new float[] {1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   0.5f, 0,    1f},
        /*Fire*/    new float[] {1f,   0.5f, 0.5f, 1f,   2f,   2f,   1f,   1f,   1f,   1f,   1f,   2f,   0.5f, 1f,   0.5f},
        /*Water*/   new float[] {1f,   2f,   0.5f, 1f,   0.5f, 1f,   1f,   1f,   2f,   1f,   1f,   1f,   2f,   1f,   0.5f},
        /*Electric*/new float[] {1f,   1f,   2f,   0.5f, 0.5f, 1f,   1f,   1f,   0f,   2f,   1f,   1f,   1f,   1f,   0.5f},
        /*Grass*/   new float[] {1f,   0.5f, 2f,   1f,   0.5f, 1f,   1f,   0.5f, 2f,   0.5f, 1f,   0.5f, 2f,   1f,   0.5f},
        /*Ice*/     new float[] {1f,   0.5f, 0.5f, 1f,   2f,   0.5f, 1f,   1f,   2f,   2f,   1f,   1f,   1f,   1f,   2f},
        /*Fighting*/new float[] {2f,   1f,   1f,   1f,   1f,   2f,   1f,   0.5f, 1f,   0.5f, 0.5f, 0.5f, 2f,   0f,   1f},
        /*Poison*/  new float[] {1f,   1f,   1f,   1f,   2f,   1f,   1f,   0.5f, 0.5f, 1f,   1f,   1f,   0.5f, 0.5f, 1f},
        /*Ground*/  new float[] {1f,   2f,   1f,   2f,   0.5f, 1f,   1f,   2f,   1f,   0f,   1f,   0.5f, 2f,   1f,   1f},
        /*Flying*/  new float[] {1f,   1f,   1f,   0.5f, 2f,   1f,   2f,   1f,   1f,   1f,   1f,   2f,   0.5f, 1f,   1f},
        /*Psychic*/ new float[] {1f,   1f,   1f,   1f,   1f,   1f,   2f,   2f,   1f,   1f,   0.5f, 1f,   1f,   1f,   1f},
        /*Bug*/     new float[] {1f,   0.5f, 1f,   1f,   2f,   1f,   0.5f, 0.5f, 1f,   0.5f, 2f,   1f,   1f,   0.5f, 1f},
        /*Rock*/    new float[] {1f,   2f,   1f,   1f,   1f,   2f,   0.5f, 1f,   0.5f, 2f,   1f,   2f,   1f,   1f,   1f},
        /*Ghost*/   new float[] {0f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   0.5f, 1f,   1f,   2f,   1f},
        /*Dragon*/  new float[] {1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   2f},
	};

	public static float GetEffectiveness(PokemonType attackType, PokemonType defenseType)
	{
		if (attackType == PokemonType.None || defenseType == PokemonType.None)
			return 1;
		
		int row = (int)attackType - 1;
		int col = (int)defenseType - 1;

		return chart[row][col];
	}
}