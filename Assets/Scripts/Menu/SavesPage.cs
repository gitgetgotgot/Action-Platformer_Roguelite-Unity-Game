using UnityEngine;
using UnityEngine.UI;

public class SavesPage : MonoBehaviour
{
    public static SavesPage Instance;
    public GameObject deleteWindow;
    public Button approveButton;
    public Button DeclineButton;

    private int saveIndexToDelete = 0;

    private void Awake()
    {
        Instance = this;
        deleteWindow.SetActive(false);
        approveButton.onClick.AddListener(DeleteSave);
        DeclineButton.onClick.AddListener(CloseDeleteWindow);
    }
    public void OpenDeleteWindow(int saveIndex)
    {
        saveIndexToDelete = saveIndex;
        deleteWindow.SetActive(true);
    }
    private void DeleteSave()
    {
        GameContext.Delete_chosen_save(saveIndexToDelete);
        deleteWindow.SetActive(false);
    }
    private void CloseDeleteWindow()
    {
        deleteWindow.SetActive(false);
    }
}
