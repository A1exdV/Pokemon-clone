using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyScreen : MonoBehaviour
{
	[SerializeField] private Text messageText;
	
	private PartyMemberUI[] _memberSlots;
	private List<Pokemon> _pokemons;

	public void Initialization()
	{
		_memberSlots = GetComponentsInChildren<PartyMemberUI>();
	}

	public void SetPartyData(List<Pokemon> pokemons)
	{
		_pokemons = pokemons;
		
		for (var i = 0; i < _memberSlots.Length; i++)
		{
			if(i<_pokemons.Count)
				_memberSlots[i].SetData(_pokemons[i]);
			else
			{
				_memberSlots[i].gameObject.SetActive(false);
			}
			
		}

		messageText.text = "Choose a Pokemon";
	}

	public void UpdateMemberSelection(int selectMember)
	{
		for (int i = 0; i < _pokemons.Count; i++)
		{
			if (i == selectMember)
			{
				_memberSlots[i].SetSelected(true);
			}
			else
			{
				_memberSlots[i].SetSelected(false);
			}
		}
	}

	public void SetMessageText(string message)
	{
		messageText.text = message;
	}
}
