using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartInstruction : MonoBehaviour
{
    private void Start()
    {
        Persistent[] persistents = FindObjectsOfType<Persistent>();
        foreach (Persistent persistent in persistents)
		{
            if (!persistent.destroying && persistent.seenRestart)
			{
                Destroy(gameObject);
                break;
			}
		}
    }
}
