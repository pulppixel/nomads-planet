using UnityEngine;


public class GraphDemoScript : MonoBehaviour
{
    private Canvas canvas;
    private MiniGraphControl graph;

    // Use this for initialization
    void Start()
    {
        this.canvas = GameObject.FindObjectOfType<Canvas>();
        this.graph = MiniGraphControl.Create("time", this.canvas.gameObject);
        //this.graph.queueLength = 60;
    }

    // Update is called once per frame
    void Update()
    {
        this.graph.AddValue(Time.time);
    }
}
