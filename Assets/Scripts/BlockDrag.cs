using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockDrag : MonoBehaviour
{
    public GameObject face;
    public Color color;

    private void Start()
    {
        SpriteRenderer[] images = face.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer image in images)
		{
            image.color = color;
		}
    }
}
