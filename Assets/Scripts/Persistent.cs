using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Persistent : MonoBehaviour
{
    public bool seenRestart = false;
    public bool destroying = false;

    public AudioSource insideMusic;
    public AudioSource outsideMusic;

    private float insideVolume;
    private float outsideVolume;

    private void Awake()
    {
        Persistent[] persistents = FindObjectsOfType<Persistent>();

        if (persistents.Length > 1)
        {
            destroying = true;
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        insideVolume = insideMusic.volume;
        outsideVolume = outsideMusic.volume;
        outsideMusic.volume = 0;
    }

    public void SetInside(bool inside)
	{
        StartCoroutine(FadeVolume(inside));
    }

    private IEnumerator FadeVolume(bool toInside)
	{
        for (float t = 0; t < GameManager.fadeTime; t += Time.deltaTime)
		{
            float tp = t / GameManager.fadeTime;
            insideMusic.volume = toInside ? Mathf.Lerp(0, insideVolume, tp) : Mathf.Lerp(insideVolume, 0, tp);
            outsideMusic.volume = toInside ? Mathf.Lerp(outsideVolume, 0, tp) : Mathf.Lerp(0, outsideVolume, tp);
            yield return new WaitForEndOfFrame();
		}
        insideMusic.volume = toInside ? insideVolume : 0;
        outsideMusic.volume = toInside ? 0 : outsideVolume;
    }
}
