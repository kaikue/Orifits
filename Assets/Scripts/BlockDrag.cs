using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BlockDrag : MonoBehaviour
{
    public GameObject face;
    public GameObject inside;
    public Tilemap tilemap;
    public Color color;
    public Sprite idleFace;
    public Sprite draggingFace;
    public Sprite connectedFace;
    public bool dragging = false;
    private GameManager gameManager;
    private bool wasSnapped = true;

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
        tilemap.color = color;

        allOrifices = FindObjectsOfType<Orifice>();
        myOrifices = gameObject.GetComponentsInChildren<Orifice>();
        gameManager = FindObjectOfType<GameManager>();
    }

    private void SetVisualPosition(Vector3 position)
    {
        position = Quaternion.Inverse(transform.rotation) * position;
        face.transform.localPosition = position;
        foreach (Orifice orifice in myOrifices)
        {
            orifice.outsideRenderer.transform.localPosition = position;
        }
    }

    private void Update()
	{
        bool connected = false;
        SetVisualPosition(Vector3.zero);
        foreach (Orifice orifice in allOrifices)
        {
            if (orifice.transform.parent == transform) continue;
            foreach (Orifice myOrifice in myOrifices)
            {
                float distance = Vector3.Distance(orifice.transform.position, myOrifice.transform.position);
                if (distance < GameManager.snapDistance)
                {
                    if (dragging)
                    {
                        Vector3 offset = orifice.transform.position - myOrifice.transform.position;
                        SetVisualPosition(offset);
                        if (!wasSnapped)
						{
                            gameManager.PlaySound(gameManager.attachSound);
                            wasSnapped = true;
						}
                    }

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
            if (wasSnapped)
            {
                gameManager.PlaySound(gameManager.separateSound);
                wasSnapped = false;
            }
        }
        else
		{
            faceRenderer.sprite = idleFace;
            //TODO look at other
        }
    }

    public void SetDragging(bool dragging)
	{
        this.dragging = dragging;
        tilemap.gameObject.SetActive(!dragging);
        inside.SetActive(!dragging);
    }
}
