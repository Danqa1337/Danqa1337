using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuButtons : MonoBehaviour
{
    public Button continueButton;
    public void Start()
    {
        continueButton.interactable = SaveLoader.SavesExist();  
    }
    public void NewGame()
    {
        SaveLoader.DeleteSaves();
        LoadGame();  
    }

    public void LoadGame()
    {
        LoadingScreen.i.StartLoading(1);
    }

    public void Arena()
    {
        LoadGame();
    }
    public void Exit()
    {
        Application.Quit();
    }
    
}
