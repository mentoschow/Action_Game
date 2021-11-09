using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene(1);  //Load the main scene of the game
    }

    public void QuitGame()
    {
        Application.Quit();  //Quit the game
    }
}
