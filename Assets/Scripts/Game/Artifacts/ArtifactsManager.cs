using System.Collections.Generic;
using UnityEngine;

public class ArtifactsManager : MonoBehaviour
{
    public static ArtifactsManager Instance;
    public BuffSprites artifactSprites;

    private List<Artifact> artifacts;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitArtifacts();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitArtifacts()
    {
        artifacts = new List<Artifact>()
        {
            new Artifact(), //id 0 is empty
            new Artifact("Wizard Flashlight", "Increases Basic Max Mana by 20 points",
            artifactSprites.sprites[0], new List<int>{1}, 5, 3),
            new Artifact("Soul in a Bottle", "Increases Mana Regen by 20%",
            artifactSprites.sprites[1], new List<int>{0}, 5, 3),
            new Artifact("Wizard Soul Flashlight", "Increases Basic Max Mana by 20 points and Mana Regen by 20%",
            artifactSprites.sprites[2], new List<int>{0, 1}, 10, 0, 1, 2),

            new Artifact("Gauntlet MK1", "Increases Sword Dmg by 10%",
            artifactSprites.sprites[3], new List<int>{2}, 5, 6),
            new Artifact("Rage Gauntlet", "Increases Crit Rate by 10%",
            artifactSprites.sprites[4], new List<int>{3}, 5, 6),
            new Artifact("Gauntlet MK2", "Increases Sword Dmg by 10% and Crit Rate by 10%",
            artifactSprites.sprites[5], new List<int>{2, 3}, 10, 0, 4, 5),

            new Artifact("Shield MK1", "Increases Dmg Reduction by 10%",
            artifactSprites.sprites[6], new List<int>{4}, 5, 9),
            new Artifact("Life Shield", "Increases Max HP by 10%",
            artifactSprites.sprites[7], new List<int>{5}, 5, 9),
            new Artifact("Shield MK2", "Increases Dmg Reduction by 10% and Max HP by 10%",
            artifactSprites.sprites[8], new List<int>{4, 5}, 10, 0, 7, 8),

            new Artifact("Wizard Amulet", "Increases Magic Dmg by 10%",
            artifactSprites.sprites[9], new List<int>{6}, 5, 12),
            new Artifact("Magic Necklace", "Increases Max Mana by 10%",
            artifactSprites.sprites[10], new List<int>{7}, 5, 12),
            new Artifact("Wizard Necklace", "Increases Magic Dmg by 10% and Max Mana by 10%",
            artifactSprites.sprites[11], new List<int>{6, 7}, 10, 0, 10, 11)
        };
    }

    public Artifact GetArtifact(int artifactId)
    {
        return artifacts[artifactId];
    }
}
