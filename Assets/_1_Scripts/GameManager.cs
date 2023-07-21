using System;
using System.Collections.Generic;
using UnityEngine;
using NomadsPlanet;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public CarHandler carPrefab;
    private List<Transform> spawnPoint = new(10);
    
    // todo: 네트워크 완성되면, 그걸로 플레이어 생산할 수 있도록 해.

    private void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            spawnPoint.Add(transform.GetChild(i));
            
            if (Random.Range(0f, 1f) < .8f)
            {
                Instantiate(carPrefab, spawnPoint[i]);
            }
        }
    }

    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Time.timeScale += 1f;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Time.timeScale -= 1f;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Time.timeScale = 1f;
        }
    }
}
