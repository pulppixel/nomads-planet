using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NomadsPlanet
{
    public class CarController : MonoBehaviour
    {
        // 직진
        public void GoThrough()
        {
            Debug.Log("직전");
        }
        
        // 유턴
        public void UTurn()
        {
            Debug.Log("유턴");
        }
        
        // 차선 변경
        public void ChangeLane()
        {
            Debug.Log("차선 변경");
        }
        
        // 좌회전
        public void TurnLeft()
        {
            Debug.Log("좌회전");
        }
        
        // 우회전
        public void TurnRight()
        {
            Debug.Log("우회전");
        }
    }
}