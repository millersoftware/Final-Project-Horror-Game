using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Start2But : MonoBehaviour
{

    public void StartGameButton (string newGame)
    {
        SceneManager.LoadScene(newGame);
    }
	
}
