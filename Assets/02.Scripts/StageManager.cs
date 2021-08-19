using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public GameObject goodItem;
    public GameObject badItem;

    [Range(10, 50)] public int goodItemCnt = 30;
    [Range(10, 50)] public int badItemCnt = 20;

    public List<GameObject> goodList = new List<GameObject>();
    public List<GameObject> badList = new List<GameObject>();

    public void InitStage()
    {
        foreach(var obj in goodList)
        {
            Destroy(obj);
        }
        foreach (var obj in badList)
        {
            Destroy(obj);
        }

        goodList.Clear();
        badList.Clear();
        for (int i = 0; i < goodItemCnt; i++)
        {
            Vector3 pos = new Vector3(Random.Range(-22.0f, 22.0f), 0.05f, Random.Range(-22.0f, 22.0f));
            Quaternion rot = Quaternion.Euler(Vector3.up * Random.Range(0, 360));
            goodList.Add(Instantiate(goodItem, transform.position + pos, rot, transform));
        }

        for (int i = 0; i < badItemCnt; i++)
        {
            Vector3 pos = new Vector3(Random.Range(-22.0f, 22.0f), 0.05f, Random.Range(-22.0f, 22.0f));
            Quaternion rot = Quaternion.Euler(Vector3.up * Random.Range(0, 360));
            badList.Add(Instantiate(badItem, transform.position + pos, rot, transform));
        }
    }

    private void Start()
    {
        InitStage();
    }


}
