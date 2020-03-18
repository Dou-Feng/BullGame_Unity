using UnityEngine;
using System.Collections;

public class GameButton : MonoBehaviour
{
	public string Message;

	void OnMouseDown()
	{
		if (transform.parent.gameObject.GetComponent<BullGame>() != null)
			transform.parent.gameObject.GetComponent<BullGame>().OnButton(Message);
		else
			transform.parent.gameObject.GetComponent<Player>().OnButton(Message);
	}
}
