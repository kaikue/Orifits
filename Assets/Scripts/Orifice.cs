using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orifice : MonoBehaviour
{
    public GameObject blocker;
    private Orifice pairedOrifice;
    private bool open = false;

    private void Start()
    {
        if (!open)
        {
            Orifice[] allOrifices = FindObjectsOfType<Orifice>();
            foreach (Orifice otherOrifice in allOrifices)
            {
                if (otherOrifice == this) continue;

                float d = Vector3.Distance(otherOrifice.transform.position, transform.position);
                if (d < GameManager.connectionDistance)
                {
                    OpenWith(otherOrifice);
                    break;
                }
            }
        }
    }

    public void OpenWith(Orifice otherOrifice)
    {
        pairedOrifice = otherOrifice;
        otherOrifice.pairedOrifice = this;
        Open();
        otherOrifice.Open();
    }
    
    private void Open()
    {
        open = true;
        blocker.SetActive(false);
	}

    public void Close()
    {
        if (open)
        {
            pairedOrifice.open = false;
            pairedOrifice.blocker.SetActive(true);
            pairedOrifice.pairedOrifice = null;

            open = false;
            blocker.SetActive(true);
            pairedOrifice = null;
        }
    }
}
