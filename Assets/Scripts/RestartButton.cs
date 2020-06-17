using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartButton : MonoBehaviour
{
    public void OnClick()
    {
        GameObject.Find("Player").GetComponent<PlayerController>().ResetGame();
    }
    public void LevelUp()
    {
        GameObject.Find("Player").GetComponent<PlayerController>().NextLevel();
    }
}
