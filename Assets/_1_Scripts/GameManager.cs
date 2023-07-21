using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using NomadsPlanet;

public class GameManager : MonoBehaviour
{
    public CarHandler carPrefab;
    public List<TrafficFlow> TrafficFlows;

    private void Start()
    {
        foreach (var obj in TrafficFlows)
        {
            // 각 2개씩 생성
            var enu = Enumerable.Range(0, 7).ToList();
            var targets = obj.GetTargetValues();

            for (int i = 0; i < Random.Range(0, 3); i++)
            {
                var car = Instantiate(carPrefab, this.transform);
                var pos = targets[enu[i]].position;
                car.transform.position = new Vector3(pos.x, -1, pos.z);
                car.transform.rotation = Quaternion.Euler(new Vector3(0f, obj.rotateY, 0f));
            }
        }
    }
}