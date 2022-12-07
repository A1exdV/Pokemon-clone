using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleDialogBox : MonoBehaviour
{
	[SerializeField] private int lettersPerSecond;

	[SerializeField] private Text dialogText;
	[SerializeField] private GameObject actionSelector;
	[SerializeField] private GameObject actionSelectorArrows;
	[SerializeField] private GameObject moveSelector;
	[SerializeField] private GameObject moveSelectorArrows;
	[SerializeField] private GameObject moveDetails;
	
	[SerializeField] private List<Image> actionArrows;
	[SerializeField] private List<Image> moveArrows;
	[SerializeField] private List<Text> moveTexts;
	
	[SerializeField] private Text ppText;
	[SerializeField] private Text typeText;

	[SerializeField] private Color transparent;

		public void SetDialog(string dialog)
	{
		dialogText.text = dialog;
	}

	public IEnumerator TypeDialog(string dialog)
	{
		dialogText.text = "";
		foreach (var letter in dialog.ToCharArray())
		{
			dialogText.text += letter;
			yield return new WaitForSeconds(1f/lettersPerSecond);
		}

		yield return new WaitForSeconds(1f);
	}

	public void EnableDialogText(bool Enabled)
	{
		dialogText.enabled = Enabled;
	}
	public void EnableActionSelector(bool Enabled)
	{
		actionSelector.SetActive(Enabled);
		actionSelectorArrows.SetActive(Enabled);
	}
	public void EnableMoveSelector(bool Enabled)
	{
		moveSelector.SetActive(Enabled);
		moveSelectorArrows.SetActive(Enabled);
		moveDetails.SetActive(Enabled);
	}

	public void UpdateActionArrowSelection(int selectedAction)
	{
		for (var i = 0; i < actionArrows.Count; i++)
		{
			if (i == selectedAction)
				actionArrows[i].color = Color.white;
			else
			{
				actionArrows[i].color = transparent;
			}

		}
	}
	public void UpdateMoveArrowSelection(int selectedMove, Move move)
		{
			for (var i = 0; i < moveArrows.Count; i++)
			{
				if (i == selectedMove)
					moveArrows[i].color = Color.white;
				else
				{
					moveArrows[i].color = transparent;
				}
	
			}
	
			ppText.text = $"PP {move.PP}/ {move.Base.PP}";
			typeText.text = move.Base.Type.ToString();
		}
	public void SetMoveNames(List<Move> moves)
	{
		for (int i = 0; i < moveArrows.Count; i++)
		{
			if (i < moves.Count)
				moveTexts[i].text = moves[i].Base.Name;
			else
				moveTexts[i].text = "-";
		}
	}
	
	
}
