using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using NomadsPlanet;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public CarHandler carPrefab;
    public List<TrafficFlow> trafficFlows;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        
        Instance = this;
    }

    private void Start()
    {
        foreach (var obj in trafficFlows)
        {
            // 각 2개씩 생성
            var enu = Enumerable.Range(0, 7).ToList();
            var targets = obj.GetTargetValues();

            for (int i = 0; i < Random.Range(0, 3); i++)
            {
                var car = Instantiate(carPrefab, transform);
                var pos = targets[enu[i]].position;
                car.transform.position = new Vector3(pos.x, -1, pos.z);
            }
        }
    }
}