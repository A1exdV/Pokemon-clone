using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BattleUnit : MonoBehaviour
{
	private PokemonBase _base;
	private int level;

	[SerializeField] private bool isPlayerUnit;

	[SerializeField] private int outOfSceneX;
	[SerializeField] private int faintY;
	[SerializeField] private int attackDeltaX;
	[SerializeField] private float enterAnimationDuration;
	[SerializeField] private float attackAnimationDuration;
	[SerializeField] private float hitAnimationDuration;
	[SerializeField] private float faintAnimationDuration;
	[SerializeField] private Color hitColor;
	public Pokemon Pokemon { get; set; }

	private Image _image;
	private Vector3 _originalPos;
	private Color _originalColor;

	private void Awake()
	{
		_image = GetComponent<Image>();
		_originalPos = _image.transform.localPosition;
		_originalColor = _image.color;
	}

	public void Setup(Pokemon pokemon)
	{
		Pokemon = pokemon;
		if (isPlayerUnit)
			_image.sprite = Pokemon.Base.BackSprite;
		else
		{
			_image.sprite = Pokemon.Base.FrontSprite;
		}

		_image.color = _originalColor;
		PlayEnterAnimation();
	}

	public void PlayEnterAnimation()
	{
		if (isPlayerUnit)
			_image.transform.localPosition = new Vector3(-outOfSceneX,_originalPos.y,-_originalPos.z);
		else
		{
			_image.transform.localPosition = new Vector3(outOfSceneX,_originalPos.y,-_originalPos.z);
		}

		_image.transform.DOLocalMoveX(_originalPos.x,enterAnimationDuration);
	}

	public void PlayAttackAnimation()
	{
		var sequence = DOTween.Sequence();
		if (isPlayerUnit)
		{
			sequence.Append(_image.transform.DOLocalMoveX(_originalPos.x + attackDeltaX, attackAnimationDuration));
		}
		else
		{
			sequence.Append(_image.transform.DOLocalMoveX(_originalPos.x - attackDeltaX, attackAnimationDuration));
		}
		sequence.Append(_image.transform.DOLocalMoveX(_originalPos.x, attackAnimationDuration));
	}

	public void PlayHitAnimation()
	{
		var sequence = DOTween.Sequence();
		sequence.Append(_image.DOColor(hitColor,hitAnimationDuration));
		sequence.Append(_image.DOColor(_originalColor,hitAnimationDuration));

	}

	public void PlayFaintAnimation()
	{
		var sequence = DOTween.Sequence();
		sequence.Append(_image.transform.DOLocalMoveY(_originalPos.y-faintY,faintAnimationDuration));
		sequence.Join(_image.DOFade(0f, faintAnimationDuration));
	}
}
