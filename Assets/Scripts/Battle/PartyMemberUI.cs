using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberUI : MonoBehaviour
{
	[SerializeField] private Text nameText;
	[SerializeField] private Text levelText;
	[SerializeField] private HPBar hpBar;

	[SerializeField] private Color hightlightedColor;

	private Pokemon _pokemon;
	public void SetData(Pokemon pokemon)
	{
		_pokemon = pokemon;
		nameText.text = _pokemon.Base.Name;
		levelText.text = "Lvl " + _pokemon.Level;
		hpBar.SetHP((float)_pokemon.HP/_pokemon.MaxHp);
	}

	public void SetSelected(bool selected)
	{
		if (selected)
			nameText.color = hightlightedColor;
		else
			nameText.color = Color.black;
			
		
	}
}
