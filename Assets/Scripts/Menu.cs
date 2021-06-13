using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
	public void Start()
	{
		Persistent[] persistents = FindObjectsOfType<Persistent>();
		foreach (Persistent persistent in persistents)
		{
			Destroy(persistent.gameObject);
		}
	}

	public void LoadStart()
	{
		SceneManager.LoadScene(0);
	}

	public void LoadGame()
	{
		SceneManager.LoadScene(1);
	}

	public void Quit()
	{
		Application.Quit();
	}
}
