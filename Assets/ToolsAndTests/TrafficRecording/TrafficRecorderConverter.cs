using System.Collections;
using System.Collections.Generic;
using System.IO;
using ExitGames.Client.Photon;
using UnityEngine;

public class TrafficRecorderConverter : MonoBehaviour
{

    public string fileName = "";
    
    private TrafficRecorderSrc trafficRecorder = new TrafficRecorderSrc();
    
    
    // Start is called before the first frame update
    void Start()
    {
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
        TextAsset textAsset = Resources.Load(fileNameWithoutExtension) as TextAsset;

        StreamBuffer buff = new StreamBuffer(textAsset.bytes);
        
        List<CapturedMessage> messages = this.trafficRecorder.ChunkToMessageList(buff);
        
        Debug.Log("Messages: " + messages.Count + " textAsset.bytes: " + textAsset.bytes.Length);

        string filename = "server-" + "TOBEFOUND" + "-peerId-" + messages[0].PeerId.ToString("X4") + ".pcap";
        this.trafficRecorder.WriteMessagesToPcap(messages, filename);
        this.trafficRecorder.CloseFile();
    }
}
