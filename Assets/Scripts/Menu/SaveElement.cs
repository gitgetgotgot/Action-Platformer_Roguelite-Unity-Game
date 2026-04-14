using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveElement : MonoBehaviour
{
    public int saveDataIndex;
    public TMP_Text playerName;
    public TMP_Text lvl;
    private Button playButton;
    private Button deleteButton;

    private void Awake()
    {
        playButton = transform.Find("PlayButton").GetComponent<Button>();
        deleteButton = transform.Find("DeleteButton").GetComponent<Button>();
        playButton.onClick.AddListener(LoadSave);
    }
    public void SetData(int index, string playerName, int level)
    {
        saveDataIndex = index;
        lvl.text = "LVL " + level.ToString();
        this.playerName.text = playerName;
    }
    public int GetDataIndex()
    {
        return saveDataIndex;
    }
    private void LoadSave()
    {
        GameContext.Update_active_save(saveDataIndex);
        SceneManager.LoadScene("Game_Scene");
    }
}
