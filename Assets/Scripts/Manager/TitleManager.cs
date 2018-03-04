using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour {

    public void OnePersonButtonOnClick()
    {
        Globe.gameMode = GameMode.OnePerson;
        LoadGameScene();
    }
    public void TwoPersonButtonOnClick()
    {
        Globe.gameMode = GameMode.TwoPerson;
        LoadGameScene();
    }
    public void QuitButtonOnClick()
    {
        Application.Quit();
    }

    [SerializeField]
    private GameObject helpHUD;
    [SerializeField]
    private GameObject gameMakersHUD;
    public void HelpButtonOnClick()
    {
        if (helpHUD.activeInHierarchy)
            helpHUD.SetActive(false);
        else
            helpHUD.SetActive(true);
        gameMakersHUD.SetActive(false);
    }
    public void GameMakersButtonOnClick()
    {
        if (gameMakersHUD.activeInHierarchy)
            gameMakersHUD.SetActive(false);
        else
            gameMakersHUD.SetActive(true);
        helpHUD.SetActive(false);
    }

    private void LoadGameScene()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void LoadTitleScene()
    {
        SceneManager.LoadScene("TitleScene");
    }
}
