using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Header("--Castle segments--")]
    [Header("Base Segments")]
    public GameObject startSegment;
    public GameObject endSegment;
    public GameObject shopSegment;
    public GameObject arenaSegment;
    public GameObject bossArenaSegment;
    [Header("Platforming Segments")]
    public GameObject platformingSegment1;
    public GameObject platformingSegment2;
    public GameObject platformingSegment3;
    public GameObject platformingSegment4;
    public GameObject platformingSegment5;
    public GameObject platformingSegment6;

    private Queue<GameObject> segmentsList = new();
    private Queue<Vector3> segmentsPositions = new();
    private const string EXIT_POINT = "ExitPoint";
    private const string ENTRY_POINT = "EntryPoint";
    public Vector3 GenerateLevel(Transform segmentsHolder)
    {
        if (GameContext.activeSave.active_room == 1)
        {
            //add start segment and add its exit point
            segmentsList.Enqueue(startSegment);
            segmentsPositions.Enqueue(startSegment.transform.Find(EXIT_POINT).position);
            //add segment1
            segmentsList.Enqueue(platformingSegment1);
            segmentsPositions.Enqueue(platformingSegment1.transform.Find(EXIT_POINT).position);
            //add arena segment
            segmentsList.Enqueue(arenaSegment);
            segmentsPositions.Enqueue(arenaSegment.transform.Find(EXIT_POINT).position);

            //add shop segment
            segmentsList.Enqueue(shopSegment);
            segmentsPositions.Enqueue(shopSegment.transform.Find(EXIT_POINT).position);
            //add end segment
            segmentsList.Enqueue(endSegment);
            //Instantiate segments
            InstantiateLevelSegments(segmentsHolder);
            return startSegment.transform.Find("StartMark").transform.position;
        }
        else if (GameContext.activeSave.active_room == 2)
        {
            //add start segment and add its exit point
            segmentsList.Enqueue(startSegment);
            segmentsPositions.Enqueue(startSegment.transform.Find(EXIT_POINT).position);
            if(Random.Range(0, 2) == 1)
            {
                //add arena segment
                segmentsList.Enqueue(arenaSegment);
                segmentsPositions.Enqueue(arenaSegment.transform.Find(EXIT_POINT).position);
                //add shop segment
                segmentsList.Enqueue(shopSegment);
                segmentsPositions.Enqueue(shopSegment.transform.Find(EXIT_POINT).position);
                //add arena segment
                segmentsList.Enqueue(arenaSegment);
                segmentsPositions.Enqueue(arenaSegment.transform.Find(EXIT_POINT).position);
            }
            else
            {
                //add arena segment
                segmentsList.Enqueue(arenaSegment);
                segmentsPositions.Enqueue(arenaSegment.transform.Find(EXIT_POINT).position);
                //add shop segment
                segmentsList.Enqueue(shopSegment);
                segmentsPositions.Enqueue(shopSegment.transform.Find(EXIT_POINT).position);
                //add segment1
                segmentsList.Enqueue(platformingSegment1);
                segmentsPositions.Enqueue(platformingSegment1.transform.Find(EXIT_POINT).position);
            }
            
            //add end segment
            segmentsList.Enqueue(endSegment);
            //Instantiate segments
            InstantiateLevelSegments(segmentsHolder);
            return startSegment.transform.Find("StartMark").transform.position;
        }
        else
        {
            //add start segment and add its exit point
            segmentsList.Enqueue(startSegment);
            segmentsPositions.Enqueue(startSegment.transform.Find(EXIT_POINT).position);
            //add arena segment
            segmentsList.Enqueue(bossArenaSegment);
            segmentsPositions.Enqueue(bossArenaSegment.transform.Find(EXIT_POINT).position);
            //add shop segment
            segmentsList.Enqueue(shopSegment);
            segmentsPositions.Enqueue(shopSegment.transform.Find(EXIT_POINT).position);
            //add end segment
            segmentsList.Enqueue(endSegment);
            //Instantiate segments
            InstantiateLevelSegments(segmentsHolder);
            return startSegment.transform.Find("StartMark").transform.position;
        }
    }
    private void InstantiateLevelSegments(Transform segmentsHolder)
    {
        //start segment
        Vector3 entryPos;
        Vector3 prevPos = new Vector3 (0, 0, 0);
        GameObject segment = segmentsList.Dequeue();
        Instantiate(segment, segmentsHolder);
        //other segments
        while (segmentsList.Count > 0)
        {
            segment = segmentsList.Dequeue();
            entryPos = segmentsPositions.Dequeue();
            Vector3 pos = prevPos + entryPos - segment.transform.Find(ENTRY_POINT).position;
            GameObject newSegment = Instantiate(segment, segmentsHolder);
            newSegment.transform.position = pos;
            prevPos = pos;
        }
        //clear (probably not needed for queue)
        segmentsList.Clear();
        segmentsPositions.Clear();
    }

}
