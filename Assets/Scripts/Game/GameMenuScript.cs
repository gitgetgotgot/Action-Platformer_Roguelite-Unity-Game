using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMenuScript : MonoBehaviour
{
    static public GameMenuScript Instance;

    public GameObject pausePage;
    public GameObject settingsPage;
    public GameObject deathPage;
    public GameObject buffsPage;
    public GameObject constBuffsTreePage;
    public GameObject inventoryPage;

    private Inventory inventory;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        inventory = inventoryPage.GetComponent<Inventory>();
        //pause page
        Button button = pausePage.transform.Find("Canvas/ContinueButton").GetComponent<Button>();
        button.onClick.AddListener(UnpauseGame);
        button = pausePage.transform.Find("Canvas/SettingsButton").GetComponent<Button>();
        button.onClick.AddListener(OpenSettings);
        button = pausePage.transform.Find("Canvas/ExitButton").GetComponent<Button>();
        button.onClick.AddListener(SaveAndExit);
        //settings page
        button = settingsPage.transform.Find("Canvas/ExitButton").GetComponent<Button>();
        button.onClick.AddListener(CloseSettings);
        //death page
        button = deathPage.transform.Find("Canvas/ExitButton").GetComponent<Button>();
        button.onClick.AddListener(SaveAndExit);
        button = deathPage.transform.Find("Canvas/ContinueButton").GetComponent<Button>();
        button.onClick.AddListener(StartNewRun);
    }

    private void Start()
    {
        pausePage.SetActive(false);
        settingsPage.SetActive(false);
        deathPage.SetActive(false);
        buffsPage.SetActive(false);
        constBuffsTreePage.SetActive(false);
        inventory.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameState state = GameContext.gameState;
            if (state == GameState.inGame) PauseGame();
            else if (state == GameState.inPauseMenu) UnpauseGame();
            else if (state == GameState.inSettings) CloseSettings();
            else if (state == GameState.inConstBuffs) CloseConstBuffsPage();
            else if (state == GameState.inInventory) CloseInventory();
        }
        if(Input.GetKeyDown(KeyCode.E))
        {
            GameState state = GameContext.gameState;
            if (state == GameState.inInventory)
            {
                CloseInventory();
            }
            else if(state == GameState.inGame)
            {
                OpenInventory();
            }
        }
    }

    private void PauseGame()
    {
        GameContext.gameState = GameState.inPauseMenu;
        pausePage.SetActive(true);
        Time.timeScale = 0.0f;
    }
    private void OpenSettings()
    {
        GameContext.gameState = GameState.inSettings;
        pausePage.SetActive(false);
        settingsPage.SetActive(true);
        AudioMixerManager.Instance.PlaySound(13);
    }
    private void CloseSettings()
    {
        GameContext.gameState = GameState.inPauseMenu;
        settingsPage.SetActive(false);
        pausePage.SetActive(true);
        AudioMixerManager.Instance.PlaySound(13);
    }
    private void UnpauseGame()
    {
        GameContext.gameState = GameState.inGame;
        pausePage.SetActive(false);
        Time.timeScale = 1.0f;
        AudioMixerManager.Instance.PlaySound(13);
    }
    private void StartNewRun()
    {
        SceneManager.LoadScene("Game_Scene");
        AudioMixerManager.Instance.PlaySound(13);
    }
    private void SaveAndExit()
    {
        AudioMixerManager.Instance.StopMusic();
        SceneManager.LoadScene("Menu_Scene");
        Time.timeScale = 1.0f;
        AudioMixerManager.Instance.PlaySound(13);
    }
    public void OpenDeathPage()
    {
        deathPage.SetActive(true);
    }
    public void OpenBuffsPage()
    {
        GameContext.gameState = GameState.inRunBasedBuffs;
        buffsPage.SetActive(true);
    }
    public void OpenConstBuffsPage()
    {
        GameContext.gameState = GameState.inConstBuffs;
        inventoryPage.SetActive(false);
        constBuffsTreePage.SetActive(true);
    }
    public void CloseConstBuffsPage()
    {
        GameContext.gameState = GameState.inInventory;
        inventoryPage.SetActive(true);
        constBuffsTreePage.SetActive(false);
        AudioMixerManager.Instance.PlaySound(13);
    }
    public void OpenInventory()
    {
        GameContext.gameState = GameState.inInventory;
        inventoryPage.SetActive(true);
        inventory.UpdateInventoryText();
        Time.timeScale = 0.0f;
    }
    public void CloseInventory()
    {
        GameContext.gameState = GameState.inGame;
        inventoryPage.SetActive(false);
        Time.timeScale = 1.0f;
    }
}
