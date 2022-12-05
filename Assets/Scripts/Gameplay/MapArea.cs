using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapArea : MonoBehaviour
{
	[SerializeField] private List<Pokemon> wildPokemons;

	public Pokemon GetRandomWildPokemon()
	{
		var wildPokemon =  wildPokemons[Random.Range(0, wildPokemons.Count)];
		wildPokemon.Initialization();
		return wildPokemon;
	}
}
