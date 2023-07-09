using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    static Canvas gameover;
    static Canvas winscreen;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void TriggerGameOver()
    {
        Canvas gameover = GameObject.Find("GameOver").GetComponent<Canvas>();

        if (winscreen.enabled == false)
        {
            gameover.enabled = true;
            Time.timeScale = 0;
        }
        

    }
    public static void TriggerWinScreen()
    {
        Canvas winscreen = GameObject.Find("WinScreen").GetComponent<Canvas>();
        winscreen.enabled = true;
        Time.timeScale = 0;
        Debug.Log("YOU WINNN!");
    }
}
