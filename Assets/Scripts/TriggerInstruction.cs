using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerInstruction : MonoBehaviour
{
    public GameObject instruction;
	public bool isRestart;

	private void Start()
	{
		instruction.SetActive(false);
	}

	public void TurnOn()
	{
		instruction.SetActive(true);
		if (isRestart)
		{
			FindObjectOfType<Persistent>().seenRestart = true;
		}
	}
}
