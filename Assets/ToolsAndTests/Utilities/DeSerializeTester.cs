using System;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.Assertions;


public class DeSerializeTester : MonoBehaviour
{
    public bool UseProtocol18;
    public bool RegisterAllCustomTypes;


    private IProtocol protocol;
    public string hexString;

    // Use this for initialization
    public void Start()
    {
        this.protocol = (this.UseProtocol18) ? (IProtocol)new Protocol18() : (IProtocol)new Protocol16();
        if (this.RegisterAllCustomTypes) this.RegisterAnyCustomType();
        this.TestArbitraryHexString();
    }

    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.D))
        {
            this.TestArbitraryHexString();
        }

        if (Input.GetKeyUp(KeyCode.C))
        {
            this.RegisterAnyCustomType();
        }
    }

    public void TestArbitraryHexString()
    {
        string[] hexValues = this.hexString.Trim().Replace("0x","").Split('-', ' ', ',');
        byte[] bytes = new byte[hexValues.Length - 1];

        byte magicNumber = byte.Parse(hexValues[0], NumberStyles.HexNumber);
        byte msgType = byte.Parse(hexValues[1], NumberStyles.HexNumber);

        // Protocol.Deserialize() does not read the magic number, so we skip it in the data byte[]
        for (int i = 1; i < hexValues.Length; i++)
        {
            bytes[i - 1] = byte.Parse(hexValues[i], NumberStyles.HexNumber);
        }

        char[] chars = new char[bytes.Length];
        for (int i = 0; i < bytes.Length; i++)
        {
            byte b = bytes[i];
            chars[i] = (char)b;
        }

        string asString = new string(chars);
        Debug.Log("As string: " + asString);

        // check it's a magic number
        Assert.AreEqual((byte)magicNumber, (byte)0xF3);

        // Protocol.Deserialize() does not read the msg-type but a data-type. this is the conversion:
        switch (msgType)
        {
            case 4:
                // Event msg
                bytes[0] = (this.UseProtocol18) ? (byte)26 : (byte)'e';
                break;
            case 3:
                // OperationResponse msg
                bytes[0] = (this.UseProtocol18) ? (byte)25 : (byte)'p';
                break;
            case 2:
                // OperationRequest msg
                bytes[0] = (this.UseProtocol18) ? (byte)24 : (byte)'q';
                break;
        }

        // deserialize
        object result = this.protocol.Deserialize(bytes);
        EventData ev = result as EventData;
        if (ev != null)
        {
            Debug.Log(ev.ToStringFull());
        }
        else
        {
            Debug.Log("Result: " + result + " Type: " + result.GetType());
        }
    }


    #region Custom Type Support

    private void RegisterAnyCustomType()
    {
        for (byte t = 0; t < byte.MaxValue; t++)
        {
            Type temp = this.CreateCustomType(t);
            if (!Protocol.TryRegisterType(temp, t, this.fakeWriteb, this.fakeReadb))
            {
                Debug.LogWarning("Can't register: " + t);
            }
        }
    }

    private byte[] fakeWriteb(object customobject)
    {
        return new byte[0];
    }

    private object fakeReadb(byte[] serializedcustomobject)
    {
        Debug.Log("Custom type data: " + SupportClass.ByteArrayToString(serializedcustomobject));
        return 0;
    }

    private Type CreateCustomType(byte i)
    {
        throw new NotImplementedException("CreateCustomType needs update");
        //AssemblyName asemblyName;
        //asemblyName = new AssemblyName("CustomType" + i);
        //AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(asemblyName, AssemblyBuilderAccess.Run);
        //ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");

        //TypeBuilder typeBuilder = moduleBuilder.DefineType(asemblyName.FullName, TypeAttributes.Public |
        //                                                                         TypeAttributes.Class |
        //                                                                         TypeAttributes.AutoClass |
        //                                                                         TypeAttributes.AnsiClass |
        //                                                                         TypeAttributes.BeforeFieldInit |
        //                                                                         TypeAttributes.AutoLayout
        //                                                   , null);
        //return typeBuilder.CreateType();
    }

    #endregion
}