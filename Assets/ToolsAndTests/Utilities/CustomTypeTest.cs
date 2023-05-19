using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;



public class CustomTypeTest : MonoBehaviourPunCallbacks
{
    public static Dictionary<int, object> dictionaryObj = new Dictionary<int, object>(1) { { 1, new Vector3() } };

    public static Dictionary<int, Vector3> dictionary = new Dictionary<int, Vector3>(1) { { 1, new Vector3() } };
    public static List<object> list = new List<object>() { dictionary };

    public void Start()
    {
        IProtocol prot = new Protocol16();

        //Debug.Log("Obj value dict");
        //prot.Serialize(dictionaryObj);

        //Debug.Log("Vector3 list with dict");
        //prot.Serialize(list);

        //Debug.Log("Vector3 value dict");
        //prot.Serialize(dictionary);



        prot = new Protocol18();

        //Debug.Log("Obj value dict");
        //prot.Serialize(dictionaryObj);
        
        //Debug.Log("Vector3 list with dict");
        //prot.Serialize(list);
        
        //Debug.Log("Vector3 value dict");
        //prot.Serialize(dictionary);


        prot = new Protocol18();
        Protocol.TryRegisterType(typeof(IndexedMap<string>.Pair), 0x81, SerializeTagPair, DeserializeTagPair);

        var value = new IndexedMap<string>.Pair();
        var valueArray = new IndexedMap<string>.Pair[1];
        
        var result = prot.Serialize(value);
        var resultArray = prot.Serialize(valueArray);

        Debug.Log("value and array: "+ SupportClass.ByteArrayToString(result) + " "+SupportClass.ByteArrayToString(resultArray) );

    }


    
    private object DeserializeTagPair(StreamBuffer instream, short length)
    {
        return new IndexedMap<string>().pair;
    }

    private short SerializeTagPair(StreamBuffer outstream, object customobject)
    {
        outstream.WriteBytes(0x2A, 0, 0, 0);
        return 4;
    }
    
}


public class IndexedMap<T> 
{
    /// <summary>
    /// Public-facing representation of an IndexedMap entry
    /// </summary>
    public struct Pair
    {
        public T value;
        public int index;
    }


    public Pair pair = new Pair();


}