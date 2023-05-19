using UnityEngine;

namespace MameshibaGames.Kekos.DemoScene
{
    public class RotateObject : MonoBehaviour
    {
        [SerializeField]
        private float speed;

        private void Update()
        {
            transform.Rotate(0, speed * Time.deltaTime, 0);
        }
    }
}
