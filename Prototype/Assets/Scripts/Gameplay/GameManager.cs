using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public string NextSceneName;

    public GameObject Spawner;

    public Vector3 EndPosition;

    public Controller Player;

    public bool isPlaying = false;

    public IEnumerator Game;

    public float startTime = 0;

    public void Start()
    {
        Game = StartGame();
        StartCoroutine(Game);
    }

    IEnumerator StartGame()
    {
        startTime = Time.time + 65;

        yield return new WaitForSeconds(5);

        Player.tag = "Target";
        Spawner.transform.position = EndPosition;
        
        isPlaying = true;

        yield return new WaitForSeconds(60);

        Debug.Log("60 seconds passed, calling Next function");

        isPlaying = false;

        Next();
    }

    public void Next()
    {
        StopAllCoroutines();

        isPlaying = false;

        SceneManager.LoadScene(NextSceneName);
    }

    public void Died()
    {
        isPlaying = false;

        Next();
    }
}
