using System.Collections.Generic;
using UnityEngine;

public class Artifact
{
    public Artifact() { }
    public Artifact(string name, string description, Sprite sprite, int craftableArtifactId, List<int> buff_id_list)
    {
        this.name = name;
        this.description = description;
        this.sprite = sprite;
        this.buff_id_list = buff_id_list;
        this.craftableArtifactId = craftableArtifactId;
    }

    public string name;
    public string description;
    public Sprite sprite;
    public List<int> buff_id_list;
    public int craftableArtifactId = 0; //0 means that this artifact is not used in crafting
}
