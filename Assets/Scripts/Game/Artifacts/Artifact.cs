using System.Collections.Generic;
using UnityEngine;

public class Artifact
{
    public Artifact() { }
    public Artifact(string name, string description, Sprite sprite,
        List<int> buff_id_list, int cost, int craftableArtifactId = 0,
        int child1_artifact_id = 0, int child2_artifact_id = 0)
    {
        this.name = name;
        this.description = description;
        this.sprite = sprite;
        this.buff_id_list = buff_id_list;
        this.cost = cost;
        this.craftableArtifactId = craftableArtifactId;
        this.child1_artifact_id = child1_artifact_id;
        this.child2_artifact_id = child2_artifact_id;
    }

    public string name;
    public string description;
    public Sprite sprite;
    public List<int> buff_id_list;
    public int cost = 5; //how many shadow shards can be obtained by recycling this artifact
    public int craftableArtifactId = 0; //0 means that this artifact is not used in crafting
    //if artifact can be crafted, then it has 2 children artifacts
    public int child1_artifact_id = 0;
    public int child2_artifact_id = 0;
}
