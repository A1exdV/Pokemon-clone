using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHud : MonoBehaviour
{
	[SerializeField] private Text nameText;
	[SerializeField] private Text levelText;
	[SerializeField] private HPBar hpBar;

	private Pokemon _pokemon;
	public void SetData(Pokemon pokemon)
	{
		_pokemon = pokemon;
		nameText.text = _pokemon.Base.Name;
		levelText.text = "Lvl " + _pokemon.Level;
		hpBar.SetHP((float)_pokemon.HP/_pokemon.MaxHp);
	}

	public IEnumerator UpdateHP()
	{
		yield return hpBar.SetHpSmooth((float)_pokemon.HP/_pokemon.MaxHp);
	}
}
