using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
	[SerializeField] private float moveSpeed;
	[SerializeField] private LayerMask solidObjectsLayer;
	[SerializeField] private LayerMask grassLayer;

	[SerializeField] private int grassBattleChance;

	public event Action OnEncountered;
	
	private bool _isMoving;
	private Vector2 _input;

	private Animator _animator;

	private const float PlayerRadiusCheck = 0.2f;
	
	

	private void Start()
	{
		_isMoving = false;
		_animator = GetComponent<Animator>();
	}

	public void HandleUpdate()
	{
		if (!_isMoving)
		{
			_input.x = Input.GetAxisRaw("Horizontal");
			_input.y = Input.GetAxisRaw("Vertical");

			if (_input.x != 0)
				_input.y = 0;
			if (_input != Vector2.zero)
			{
				_animator.SetFloat("moveX",_input.x);
				_animator.SetFloat("moveY",_input.y);
				
				var targetPos = transform.position;
				
				targetPos.x += _input.x;
				targetPos.y += _input.y;
				
				if(IsWalkable(targetPos))
					StartCoroutine(Move(targetPos));
			}
		}
		
		
		_animator.SetBool("isMoving",_isMoving);
	}

	IEnumerator Move(Vector3 targetPos)
	{
		_isMoving = true;
		while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
		{
			transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
			yield return null;
		}

		transform.position = targetPos;
		_isMoving = false;

		CheckForEncounters();
	}

	private bool IsWalkable(Vector3 targetPos)
	{
		return !Physics2D.OverlapCircle(targetPos, PlayerRadiusCheck, solidObjectsLayer);
	}

	private void CheckForEncounters()
	{
		if (Physics2D.OverlapCircle(transform.position, PlayerRadiusCheck, grassLayer)!=null)
		{
			if (Random.Range(0, 100) <= grassBattleChance)
			{
				Debug.Log("Encountered a wild pokemon!");
				_animator.SetBool("isMoving",false);
				if (OnEncountered != null) OnEncountered();
			}
		}
	}
}
