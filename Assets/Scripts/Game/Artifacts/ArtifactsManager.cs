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
            new Artifact("Wizard Flashlight", "Increases basic max Mana by 20 points",
            artifactSprites.sprites[0], new List<int>{1}, 5, 3 ),
            new Artifact("Soul in a Bottle", "Increases Mana Regen by 20%",
            artifactSprites.sprites[1], new List<int>{0}, 5, 3 ),
            new Artifact("Wizard Soul Flashlight", "Increases basic max Mana by 20 points and Mana Regen by 20%",
            artifactSprites.sprites[2], new List<int>{0, 1}, 5, 0, 1, 2 )

        };
    }

    public Artifact GetArtifact(int artifactId)
    {
        return artifacts[artifactId];
    }
}
