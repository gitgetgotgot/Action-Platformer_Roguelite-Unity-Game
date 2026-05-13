using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    public GameObject menuPage;
    public GameObject savesPage;
    public GameObject newGamePage;
    public GameObject settingsPage;
    public GameObject saveElementPrefab;
    public Transform savesContent;

    public static MenuScript instance;

    private GameObject active_page;
    private List<GameObject> savesList;
    private TMP_InputField newGamePlayerNameField;

    void Start()
    {
        instance = this;
        savesList = new();
        menuPage.SetActive(true);
        savesPage.SetActive(false);
        settingsPage.SetActive(false);
        active_page = menuPage;

        InitPages();
        SavesManager.Instance.LoadSaves();
        LoadSavesUI();
    }
    public void DeleteSaveElement(int saveIndex)
    {
        Destroy(savesList[saveIndex]);
        savesList.RemoveAt(saveIndex);
        //change save indexes for next elements
        for(int i = saveIndex; i < savesList.Count; i++)
        {
            savesList[i].GetComponent<SaveElement>().saveDataIndex = i;
        }
    }
    private void InitPages()
    {
        //menu page
        Button playButton = menuPage.transform.Find("Canvas/PlayButton").GetComponent<Button>();
        Button settingsButton = menuPage.transform.Find("Canvas/SettingsButton").GetComponent<Button>();
        Button exitButton = menuPage.transform.Find("Canvas/ExitButton").GetComponent<Button>();
        playButton.onClick.AddListener(OpenSavesPage);
        settingsButton.onClick.AddListener(OpenSettings);
        exitButton.onClick.AddListener(ExitGame);
        //settings page
        exitButton = settingsPage.transform.Find("Canvas/ExitButton").GetComponent<Button>();
        exitButton.onClick.AddListener(ReturnToMenu);
        //saves page
        exitButton = savesPage.transform.Find("Canvas/ExitButton").GetComponent<Button>();
        exitButton.onClick.AddListener(ReturnToMenu);
        Button newGameButton = savesPage.transform.Find("Canvas/NewGameButton").GetComponent<Button>();
        newGameButton.onClick.AddListener(CreateNewGame);
        //new game page
        newGamePlayerNameField = newGamePage.transform.Find("Canvas/PlayerNameInput").GetComponent<TMP_InputField>();
        playButton = newGamePage.transform.Find("Canvas/StartButton").GetComponent<Button>();
        playButton.onClick.AddListener(StartNewGame);
        exitButton = newGamePage.transform.Find("Canvas/ExitButton").GetComponent<Button>();
        exitButton.onClick.AddListener(OpenSavesPage);
    }
    private void LoadSavesUI()
    {
        for (int i = 0; i < GameContext.savesDataList.Count; i++)
        {
            GameObject save = Instantiate(saveElementPrefab, savesContent);
            SaveElement saveElement = save.GetComponent<SaveElement>();
            saveElement.SetData(i, GameContext.savesDataList[i].playerName, GameContext.savesDataList[i].level);
            savesList.Add(save);
        }
    }
    private void ReturnToMenu()
    {
        active_page.SetActive(false);
        if(active_page == settingsPage)
        {
            SavesManager.Instance.SaveGameSettings();
        }
        menuPage.SetActive(true);
        active_page = menuPage;
        AudioMixerManager.Instance.PlaySound(13);
    }
    private void ExitGame()
    {
        Application.Quit();
    }
    private void OpenSettings()
    {
        active_page.SetActive(false);
        settingsPage.SetActive(true);
        active_page = settingsPage;
        AudioMixerManager.Instance.PlaySound(13);
    }
    private void OpenSavesPage()
    {
        active_page.SetActive(false);
        savesPage.SetActive(true);
        active_page = savesPage;
        AudioMixerManager.Instance.PlaySound(13);
    }

    private void CreateNewGame()
    {
        active_page.SetActive(false);
        newGamePage.SetActive(true);
        active_page = newGamePage;
        AudioMixerManager.Instance.PlaySound(13);
    }

    private void StartNewGame()
    {
        SavesManager.Instance.AddNewSave(newGamePlayerNameField.text);
        SceneManager.LoadScene("Game_Scene");
        AudioMixerManager.Instance.PlaySound(13);
    }
}
