using UnityEngine;

public class CastleBossArena : MonoBehaviour
{
    //borders of arena for different boss attacks
    [Header("ArenaBorders")]
    public Transform arenaLeftSide;
    public Transform arenaRightSide;
    [Header("ArenaFences")]
    public GameObject arenaLeftFence;
    public GameObject arenaRightFence;

    private void Awake()
    {
        GameContext.bossArena = this;
        arenaLeftFence.SetActive(false);
        arenaRightFence.SetActive(false);
    }
    public void CloseArena()
    {
        arenaLeftFence.SetActive(true);
        arenaRightFence.SetActive(true);
    }
    public void OpenArena()
    {
        arenaLeftFence.SetActive(false);
        arenaRightFence.SetActive(false);
    }
}
