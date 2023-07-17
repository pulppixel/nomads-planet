using System;
using System.Collections;
using System.Collections.Generic;
using NomadsPlanet;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public CarHandler carPrefab;
    private List<Transform> spawnPoint = new(10);
    
    // todo: 네트워크 완성되면, 그걸로 플레이어 생산할 수 있도록 해.

    private void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            spawnPoint.Add(transform.GetChild(i));
            GameObject.Instantiate(carPrefab, spawnPoint[i]);
        }
    }
}
