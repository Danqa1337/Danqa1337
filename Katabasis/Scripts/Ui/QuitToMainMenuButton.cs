using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuitToMainMenuButton : MonoBehaviour
{

    
    public void Play()
    {

        Engine.i.Dispose();
        SceneManager.LoadScene(0);
        
    }
    
}
