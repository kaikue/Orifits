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
    public GameObject blockParticles;
    public bool dragging = false;
    private GameManager gameManager;
    private bool wasSnapped = true;

    public SpriteRenderer faceRenderer;
    private Orifice[] allOrifices;
    private Orifice[] myOrifices;
    private Orifice lastConnectedMine;
    private Orifice lastConnectedOther;

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

        bool connected = false;
        foreach (Orifice orifice in allOrifices)
        {
            if (orifice.transform.parent == transform) continue;
            foreach (Orifice myOrifice in myOrifices)
            {
                float distance = Vector3.Distance(orifice.transform.position, myOrifice.transform.position);
                if (distance < GameManager.snapDistance)
                {
                    lastConnectedMine = myOrifice;
                    lastConnectedOther = orifice;
                    connected = true;
                    break;
                }
            }
            if (connected) break;
        }
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
        SetVisualPosition(Vector3.zero);
        bool connected = false;
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
                            SpawnParticles(orifice.transform.position, color);
                            SpawnParticles(orifice.transform.position, orifice.transform.parent.GetComponent<BlockDrag>().color);
                            lastConnectedMine = myOrifice;
                            lastConnectedOther = orifice;
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
                if (lastConnectedMine != null)
                    SpawnParticles(lastConnectedMine.transform.position, color);
                if (lastConnectedOther != null)
                    SpawnParticles(lastConnectedOther.transform.position, lastConnectedOther.transform.parent.GetComponent<BlockDrag>().color);
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

    private void SpawnParticles(Vector3 position, Color color)
	{
        GameObject particles = Instantiate(blockParticles, position, Quaternion.identity);
        ParticleSystem.MainModule main = particles.GetComponent<ParticleSystem>().main;
        main.startColor = color;
    }
}
