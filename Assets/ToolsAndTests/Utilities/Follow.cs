// a good looking follow script, as posted on reddit:
// https://www.reddit.com/r/Unity3D/comments/ayf8rq/a_simple_follow_script_using_damped_harmonic/
// see: https://imgur.com/a/hQwJXhQ

using UnityEngine;

public class Follow : MonoBehaviour
{
    [SerializeField]
    private float m_Height;

    [SerializeField]
    private float m_MaxSpeed;

    [SerializeField]
    private float m_Damping;

    private Vector3 velocity = new Vector3();

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (!Physics.Raycast(ray, out hit)) return;

        var target = new Vector3(hit.point.x, m_Height, hit.point.z);
        velocity = Vector3.ClampMagnitude(velocity, m_MaxSpeed);

        var n1 = velocity - (transform.position - target) * m_Damping * m_Damping * Time.deltaTime;
        var n2 = 1 + m_Damping * Time.deltaTime;
        velocity = n1 / (n2 * n2);

        transform.position += velocity * Time.deltaTime;
    }
}