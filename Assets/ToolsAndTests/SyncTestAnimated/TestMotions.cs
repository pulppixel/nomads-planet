using Photon.Pun;
using UnityEngine;


public class TestMotions : MonoBehaviourPun
{
    public enum MovementType
    {
        Circular,
        Quad,
        ScalePulse
    }


    [SerializeField]
    public MovementType MoveType;



    private Vector3 circleCenter;

    private Vector3[] quadCorners;
    private int quadEdge;
    private float quadEdgeFraction;

    /* speed of orbit (in degrees/second) */
    public float speed = 10;

    public float scaleMax = 3;
    private float scaleCurrent = 1;
    private bool scaleUp = true;

    // Use this for initialization
    private void Start()
    {
                this.circleCenter = this.transform.position - Vector3.up;
                this.quadCorners = new Vector3[] { this.transform.position, this.transform.position + Vector3.right * 2, this.transform.position + (Vector3.right + Vector3.up)*2, this.transform.position + Vector3.up*2 };
        
    }

    public void Update()
    {
        switch (this.MoveType)
        {
            case MovementType.Circular:
                this.transform.RotateAround(this.circleCenter, Vector3.back, this.speed * Time.deltaTime);
                break;
            case MovementType.Quad:
                Vector3 from = this.quadCorners[this.quadEdge];
                Vector3 to = this.quadCorners[(this.quadEdge+1)%4];
                quadEdgeFraction = this.quadEdgeFraction + this.speed * Time.deltaTime;
                this.transform.position = Vector3.Lerp(from, to, quadEdgeFraction);
                if (Vector3.Distance(this.transform.position, to) < 0.000001f)
                {
                    quadEdgeFraction = 0;
                    this.quadEdge = (this.quadEdge + 1) % 4;
                }
                break;
            case MovementType.ScalePulse:
                if (this.scaleUp)
                {
                    this.scaleCurrent = this.scaleCurrent + this.speed * Time.deltaTime;
                    if (this.scaleCurrent > this.scaleMax)
                    {
                        this.scaleCurrent = this.scaleMax;
                        this.scaleUp = false;
                    }
                }
                else
                {
                    this.scaleCurrent = this.scaleCurrent - this.speed * Time.deltaTime;
                    if (this.scaleCurrent < 0.1f)
                    {
                        this.scaleCurrent = 0.1f;
                        this.scaleUp = true;
                    }
                }

                this.transform.localScale = new Vector3(this.scaleCurrent, this.scaleCurrent, this.scaleCurrent);
                break;
        }
    }
}