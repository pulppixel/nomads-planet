using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Security.Permissions;
using System.Threading;
using Photon.Realtime;
using UnityEngine;

public class DnsChecks : MonoBehaviour
{
    private string ServerAddress = "ns.exitgames.com";
    private IPHostEntry hostEntryHostEntry;
    private IPAddress[] addressesHostEntry;


    private IPAddress[] addressesGetAddress;

    private IPHostEntry hostEntryResolve;
    private IPAddress[] addressesResolve;

    private int process;


    void OnEnable()
    {
        List<string> buildProperties = new List<string>(10);
        #if SUPPORTED_UNITY
        buildProperties.Add(Application.unityVersion);
        buildProperties.Add(Application.platform.ToString());
        #endif
        #if ENABLE_IL2CPP
        buildProperties.Add("ENABLE_IL2CPP");
        #endif
        #if ENABLE_MONO
        buildProperties.Add("ENABLE_MONO");
        #endif
        #if DEBUG
        buildProperties.Add("DEBUG");
        #endif
        #if MASTER
        buildProperties.Add("MASTER");
        #endif
        #if NET_4_6
        buildProperties.Add("NET_4_6");
        #endif
        #if NET_STANDARD_2_0
        buildProperties.Add("NET_STANDARD_2_0");
        #endif
        #if NETFX_CORE
        buildProperties.Add("NETFX_CORE");
        #endif
        #if NET_LEGACY
        buildProperties.Add("NET_LEGACY");
        #endif
        #if UNITY_64
        buildProperties.Add("UNITY_64");
        #endif

        Debug.Log(buildProperties.ToStringFull());


        this.process = 0;
        this.DnsBasic();
        //#if NET_4_6 || NET_STANDARD_2_0
        //this.DnsAttributed();
        //#endif
    }

    void OnGUI()
    {
        GUILayout.Label("Process: " + this.process);
    }


    public string IpAddressArrayToString(IPAddress[] addresses)
    {
        if (addresses == null)
        {
            return "";
        }

        string result = "";
        for (int i=0; i<addresses.Length; i++)
        {
            result = result + addresses[i].ToString() + " ";
        }
        return result;
    }

    private void DnsBasic()
    {
        try
        {
            this.hostEntryHostEntry = Dns.GetHostEntry(this.ServerAddress);
            this.process++;
            this.addressesHostEntry = this.hostEntryHostEntry.AddressList;
            this.process++;
            Debug.Log("Dns.GetHostEntry results in " + this.addressesHostEntry.Length + " entries: " + IpAddressArrayToString(this.addressesHostEntry));
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }

        try
        {
            this.addressesGetAddress = Dns.GetHostAddresses(this.ServerAddress);
            this.process++;
            Debug.Log("Dns.GetHostAddresses results in " + this.addressesGetAddress.Length + " entries: " + IpAddressArrayToString(this.addressesGetAddress));
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }

        try
        {
            this.hostEntryResolve = Dns.Resolve(this.ServerAddress);
            this.process++;
            this.addressesResolve = this.hostEntryResolve.AddressList;
            this.process++;
            Debug.Log("Dns.Resolve results in " + this.addressesResolve.Length + " entries: " + IpAddressArrayToString(this.addressesResolve));
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }
    }

    //#if NET_4_6 || NET_STANDARD_2_0
    //[DnsPermission(SecurityAction.Assert, Unrestricted = true)]
    //private void DnsAttributed()
    //{
    //    var x = new DnsPermission(PermissionState.Unrestricted);

    //    try
    //    {
    //        this.hostEntryHostEntry = Dns.GetHostEntry(this.ServerAddress);
    //        this.process++;
    //        this.addressesHostEntry = this.hostEntryHostEntry.AddressList;
    //        this.process++;
    //        Debug.Log("DnsPermission Dns.GetHostEntry results in " + this.addressesHostEntry.Length + " entries.");
    //    }
    //    catch (Exception ex)
    //    {
    //        Debug.Log(ex);
    //    }

    //    try
    //    {
    //        this.addressesGetAddress = Dns.GetHostAddresses(this.ServerAddress);
    //        this.process++;
    //        Debug.Log("DnsPermission Dns.GetHostAddresses results in " + this.addressesGetAddress.Length + " entries.");
    //    }
    //    catch (Exception ex)
    //    {
    //        Debug.Log(ex);
    //    }

    //    try
    //    {
    //        this.hostEntryResolve = Dns.Resolve(this.ServerAddress);
    //        this.process++;
    //        this.addressesResolve = this.hostEntryResolve.AddressList;
    //        this.process++;
    //        Debug.Log("DnsPermission Dns.Resolve results in " + this.addressesResolve.Length + " entries.");
    //    }
    //    catch (Exception ex)
    //    {
    //        Debug.Log(ex);
    //    }

    //    this.process = 100;
    //}
    //#endif
}