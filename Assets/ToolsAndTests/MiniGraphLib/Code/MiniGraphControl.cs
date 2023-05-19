using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MiniGraphControl : MonoBehaviour {


    public enum GraphValueType
    {
        ValueSum,
        ValueMax,
    }

    private struct Packet
    {
        public float Timestamp;
        public float Frame;
        public float Value;

        public Packet(float timeStamp, float frame, float value)
        {
            Timestamp = timeStamp;
            Frame = frame;
            Value = value;
        }
    }

    private static List<MiniGraphControl> Graphs;
    private static int nextGraphId = 0;
    private int graphId = 0;

    private GraphValueType graphValueType;

    private float updateInterval = 0.2f;
    /// <summary>The count of values to show in the graph.</summary>
    public int queueLength = 100;

    // Graph layout options
    private int barHeight = 30;
    private int barWidth = 3;
    private int barSpace = 1;

    /// <summary>The actual queue of values to show in the bars.</summary>
    private LimitedQueue<float> lq;
    private List<float> valueQueue;
    private LimitedQueue<Packet> valueHistory;
    private int valueHistoryLength = 50000;

    public bool pauseUpdates;
    private float minimumBarScaleY = 0.025f;

    public Text titleText;
    public Text minMaxAverageText;
    public RectTransform barsPanel;
    public Image img;

    private Image[] BarSprites = null;


    private string graphName;
    //    private float lifeMaxValue = 0;
    private float maxValue = 0;
    private float minValue = 0;
    private float avgValue = 0;
    private float lastUpdate = 0;

    private GUIStyle guiStyle;

    private bool isVisible;

    public bool LowIsGreen = false;
    public bool IgnoreNullForAverage = false;
    public bool IgnoreNullForMin = false;


    public static MiniGraphControl Create(string name, GameObject parent = null)
    {
        GameObject prefab = Resources.Load("GraphPanel", typeof (GameObject)) as GameObject;
        GameObject go = (GameObject)Instantiate(prefab);
        go.name = name;

        if (parent != null)
        {
            go.transform.SetParent(parent.transform, false);
        }
        return go.GetComponent<MiniGraphControl>();
    }

    public void Reset()
    {
        this.lq.Clear();
        this.valueQueue.Clear();
        this.lastUpdate = 0;
        this.maxValue = 0;
        this.minValue = 0;
        this.avgValue = 0;
        for (int i = 0; i < BarSprites.Length; i++)
        {
            BarSprites[i].transform.localScale = new Vector3(1,this.minimumBarScaleY,1);
        }
    }

    public static void SetPauseAll(bool pause)
    {
        foreach (var graph in Graphs)
        {
            graph.pauseUpdates = pause;
        }
    }

    public void Awake()
    {
        if (Graphs == null)
        {
            Graphs = new List<MiniGraphControl>();
        }

        valueQueue = new List<float>();
        lq = new LimitedQueue<float>(queueLength);
        valueHistory = new LimitedQueue<Packet>(this.valueHistoryLength);
        graphValueType = GraphValueType.ValueSum;


        isVisible = true;
    }

    public void Start()
    {

        if (this.GetComponentInParent<Canvas>() == null)
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            this.transform.SetParent(canvas.transform, false);
        }

        if (string.IsNullOrEmpty(graphName))
        {
            SetName(this.name);
        }
        

        AddGraph();

        var rt = GetComponent<RectTransform>();
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ((barWidth + barSpace) * queueLength) + 2);
        rt.anchoredPosition = rt.anchoredPosition - Vector2.up * graphId * 70;
    }

    public void OnDestroy()
    {
        Graphs.Remove(this);
    }

    public void SetUpdateRate(int rateInSecs)
    {
        updateInterval = 1.0f / rateInSecs;
    }

    public void SetGraphType(GraphValueType valueType)
    {
        graphValueType = valueType;
    }

    public void AddValue(float value)
    {
        valueQueue.Add(value);

        valueHistory.Enqueue(new Packet(Time.time, Time.frameCount, value));
    }

    public void AddValue(Vector3 value, bool ignoreY)
    {
        value = new Vector3(value.x, (ignoreY ? 0 : value.y), value.z);

        valueQueue.Add(value.sqrMagnitude);

        valueHistory.Enqueue(new Packet(Time.time, Time.frameCount, value.sqrMagnitude));
    }

    public void SetName(string newName)
    {
        graphName = newName;
        titleText.text = graphName;
    }

    private void AddGraph()
    {
        this.graphId = nextGraphId;
        Graphs.Add(this);
        nextGraphId++;
    }


    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            isVisible = !isVisible;
        }

        if (this.pauseUpdates)
        {
            return;
        }

        // Reset the queue length if the limit changes
        if (lq.Limit != queueLength)
        {
            lq = new LimitedQueue<float>(queueLength);
            maxValue = 0;
            //            lifeMaxValue = 0;
            minValue = 0;
            avgValue = 0;
        }

        // Update in interval
        if (lastUpdate >= updateInterval)
        {
            float value = 0;

            // Grab all the values that have been added since the last update
            switch (graphValueType)
            {
                case GraphValueType.ValueMax:
                    {
                        // Get the highest value in the list
                        foreach (float f in valueQueue)
                        {
                            value = Mathf.Max(value, f);
                        }
                        break;
                    }
                case GraphValueType.ValueSum:
                default:
                    {
                        // Sum all the values in the list
                        foreach (float f in valueQueue)
                        {
                            value += f;
                        }
                        break;
                    }
            }

            valueQueue.Clear();

            Enqueue(value);

            lastUpdate = 0;
        }

        CalculateMaxAndAvg();
        UpdateUi();
        lastUpdate += Time.deltaTime;
    }

    private void CalculateMaxAndAvg()
    {
        float total = 0;
        float newMax = 0;
        int countedValues = 0;
        foreach (float f in lq)
        {
            if (this.IgnoreNullForAverage && f <= 0.000001f)
            {
                continue;
            }
            total += f;
            newMax = Mathf.Max(f, newMax);
            countedValues++;
        }

        avgValue = (countedValues > 0) ? (total / countedValues) : 0;

        // Only interpolate the maxvalue 
        if (newMax < maxValue)
        {
            maxValue = Mathf.Lerp(maxValue, newMax, Time.deltaTime);
        }
        else
        {
            maxValue = newMax;
        }
    }

    private void Enqueue(float value)
    {
        //        lifeMaxValue = Mathf.Max(maxValue, value);

        if (this.IgnoreNullForMin)
        {
            if (this.minValue <= 0.00001f)
            {
                this.minValue = value;
            }
            else if (value >= 0.00001f)
            {
                this.minValue = Mathf.Min(this.minValue, value);
            }
        }
        else
        {
            this.minValue = Mathf.Min(this.minValue, value);
        }

        lq.Enqueue(value);
    }

    public void UpdateUi()
    {
        if (!isVisible)
        {
            return;
        }
        
        if (BarSprites == null)
        {
            BarSprites = new Image[queueLength];
            for (int i = 0; i < queueLength; i++)
            {
                BarSprites[i] = (Instantiate(img) as Image);
                BarSprites[i].transform.SetParent(this.barsPanel.transform, false);
                BarSprites[i].transform.localPosition = img.rectTransform.anchoredPosition + new Vector2((barWidth + barSpace) * i, 0);
                BarSprites[i].transform.localScale = new Vector3(1,this.minimumBarScaleY,1);
            }
            img.gameObject.SetActive(false);
        }

        if (BarSprites.Length != queueLength)
        {
            Debug.LogWarning("Not Implemented Error: You can't change the queueLength at the moment.");
        }


        minMaxAverageText.text = string.Format("Min:{0} Max:{1} Avg:{2}", minValue.ToString("N2"), maxValue.ToString("N2"), avgValue.ToString("N2"));


        int j = (queueLength - lq.Count);
        foreach (float val in lq)
        {
            // Normalize the value
            float normVal = (maxValue > 0) ? val / maxValue : Mathf.Clamp01(val);
            normVal = Mathf.Max(normVal, this.minimumBarScaleY);

            Color color = new Color(1, 1, 1, 0.3f);
            
            if (normVal <= 0.01f)
            {
                // color is not adjusted if the bar is zero
            }
            else
            {
                // If it crosses a threshold, color it
                if (this.LowIsGreen)
                {
                    color = (normVal > 0.75f) ? ((normVal > 0.9f) ? Color.red : Color.yellow) : Color.green;
                }
                else
                {
                    color = (normVal > 0.75f) ? ((normVal > 0.9f) ? Color.green : Color.yellow) : Color.red;
                }
            }

            // adjust the bar color and scale (height)
            if (j < BarSprites.Length)
            BarSprites[j].color = color;
            BarSprites[j].transform.localScale = new Vector3(1, normVal, 1);
            j++;
        }
    }


    public void DumpToCSV()
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("Time, Frame, Value");

        foreach (Packet p in valueHistory)
        {
            sb.AppendLine(string.Format("{0}, {1}, {2}", p.Timestamp, p.Frame, p.Value));
        }

        string path = Application.persistentDataPath + "/" + graphName + ".csv";

        Debug.Log("Saving log file " + path);

        StreamWriter sw = new StreamWriter(path);
        sw.Write(sb.ToString());
        sw.Close();
    }
}
