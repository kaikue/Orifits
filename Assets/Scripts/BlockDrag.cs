using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BlockDrag : MonoBehaviour
{
    public GameObject face;
    public Color color;
    public Sprite idleFace;
    public Sprite draggingFace;
    public Sprite connectedFace;
    public bool dragging = false;

    public SpriteRenderer faceRenderer;
    private Orifice[] allOrifices;
    private Orifice[] myOrifices;

    private void Start()
    {
        SpriteRenderer[] images = GetComponentsInChildren<SpriteRenderer>(true);
        foreach (SpriteRenderer image in images)
		{
            image.color = color;
		}
        Tilemap tilemap = GetComponentInChildren<Tilemap>();
        tilemap.color = color;

        allOrifices = FindObjectsOfType<Orifice>();
        myOrifices = gameObject.GetComponentsInChildren<Orifice>();
    }

    private void Update()
	{
        bool connected = false;
        foreach (Orifice orifice in allOrifices)
        {
            if (orifice.transform.parent == transform) continue;
            foreach (Orifice myOrifice in myOrifices)
            {
                float distance = Vector3.Distance(orifice.transform.position, myOrifice.transform.position);
                if (distance < GameManager.snapDistance)
                {
                    connected = true;
                    break;
                }
            }
            if (connected) break;
        }
        if (connected)
		{
            faceRenderer.sprite = connectedFace;
		}
        else if (dragging)
		{
            faceRenderer.sprite = draggingFace;
        }
        else
		{
            faceRenderer.sprite = idleFace;
            //TODO look at other
        }
    }
}
