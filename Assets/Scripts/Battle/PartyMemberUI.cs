using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberUI : MonoBehaviour
{
	[SerializeField] private Text nameText;
	[SerializeField] private Text levelText;
	[SerializeField] private HPBarParty hpBarParty;

	[SerializeField] private Sprite selected;
	[SerializeField] private Sprite notSelected;

	[SerializeField] private Image background;

	private Pokemon _pokemon;
	public void SetData(Pokemon pokemon)
	{
		_pokemon = pokemon;
		nameText.text = _pokemon.Base.Name;
		levelText.text = _pokemon.Level.ToString();
		hpBarParty.SetHP((float)_pokemon.HP/_pokemon.MaxHp);
	}

	public void SetSelected(bool isSelected)
	{
		if (isSelected)
			background.sprite = this.selected;
		else
			background.sprite = this.notSelected;


	}
}
