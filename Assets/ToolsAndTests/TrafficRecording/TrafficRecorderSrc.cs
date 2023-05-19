#if (NET_4_6 || NET_STANDARD_2_0) && (UNITY_EDITOR || UNITY_STANDALONE)
#define PCAP_SUPPORT
#endif


using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using ExitGames.Client.Photon;

#if PCAP_SUPPORT
using PacketDotNet;
using SharpPcap.LibPcap;
#endif

public class TrafficRecorderSrc : ITrafficRecorder
{
    /// <inheritdoc />
    public bool Enabled { get; set; }

    public bool Debug { get; set; }
    public int ChunkSize { get; set; }
    public int ChunkCountMax { get; set; }

    /// <summary>Gives you the option to handle completed chunks.</summary>
    public Action<StreamBuffer, TrafficRecorderSrc> OnChunkComplete;

    /// <summary>Recorded traffic in chunks from oldest to newest. Each with a number of messages.</summary>
    public readonly Queue<StreamBuffer> CaptureChunkQueue = new Queue<StreamBuffer>();

    // the peerId is initially 0xFFFF or 0xFFFE. either needs to be replaced by the peerId assigned by the server.
    public const short DefaultPeerId = unchecked((short)0xFFFE);

    private StreamBuffer currentCaptureChunk;

    private IPhotonSocket lastRecordedConnection;

    private readonly Protocol18 protocol = new Protocol18();


    public override string ToString()
    {
        return string.Format("{0} Chunks: {1}", this.Enabled ? "enabled" : "disabled", this.ChunkCount);
    }

    public int ChunkCount
    {
        get { return this.CaptureChunkQueue.Count; }
    }

    private bool updatePeerId;

    private int currentServerPort;
    private int previousServerPort;

    /// <summary>Writes messages into chunks of memory. Once setup, called by PhotonPeer for each message sent and received.</summary>
    /// <remarks>
    /// This implementation uses a chunk per conversation.
    ///
    /// Each chunk has a "chunk header":<br/>
    ///   remote address as 6 bytes<br/>
    ///   peerId of Enet conversation as 2 bytes<br/>
    ///
    /// This is followed by messages. Each message has a header:<br/>
    ///   1 byte "incoming"<br/>
    ///   8 bytes for DateTime.UtcNow.ToBinary()<br/>
    ///   byte[] data (prefixed with length (up to 4 bytes))<br/>
    /// </remarks>
    /// <param name="inBuffer">Buffer for sending/receiving. May exceed length.</param>
    /// <param name="length">Actual length of the network data.</param>
    /// <param name="incoming">Indicates incoming (true) or outgoing (false) traffic.</param>
    /// <param name="peerId">The local peerId for the connection. This is a default value until assigned by the Server.</param>
    /// <param name="connection">The currently used IPhotonSocket of this Peer. Enables you to track the connection endpoints.</param>
    public void Record(byte[] inBuffer, int length, bool incoming, short peerId, IPhotonSocket connection)
    {
        if (!this.Enabled)
        {
            return;
        }


        bool chunkStart = false;

        // if the message is for a new conversation, we write to a new, fresh chunk
        // we will record the messages from (exactly) the previous connection too but not use the (previous) peerId

        bool fromCurrentPort = connection.ServerPort == currentServerPort;
        bool fromPreviousPort = connection.ServerPort == this.previousServerPort;

        bool useNextChunk = !fromCurrentPort && !fromPreviousPort;

        lock (this.CaptureChunkQueue)
        {
            // check if we got a current capture buffer, create one if null, enqueue right away
            if (this.currentCaptureChunk == null)
            {
                this.currentCaptureChunk = new StreamBuffer(new byte[this.ChunkSize]);
                this.CaptureChunkQueue.Enqueue(this.currentCaptureChunk);
                chunkStart = true;
                useNextChunk = false;
            }

            // check if next message fits in current chunk
            bool willFit = this.currentCaptureChunk.Position + length + 9 < this.currentCaptureChunk.Length;

            //if (this.Debug)
            //{
            //    UnityEngine.Debug.Log("Record length: " + length + " chunk.Pos: " + this.currentCaptureChunk.Position + " chunk.Length " + this.currentCaptureChunk.Length + " willFit: " + willFit);
            //}

            if (!willFit || useNextChunk)
            {
                this.currentCaptureChunk.Position = 0;
                this.OnChunkComplete(this.currentCaptureChunk, this);

                chunkStart = true;
                useNextChunk = false;

                // if new message doesn't fit, check config if we create another chunk or get the oldest from queue
                if (this.CaptureChunkQueue.Count >= this.ChunkCountMax)
                {
                    this.currentCaptureChunk = this.CaptureChunkQueue.Dequeue();
                    this.currentCaptureChunk.SetCapacityMinimum(this.ChunkSize);
                    this.currentCaptureChunk.Position = 0;

                    this.CaptureChunkQueue.Enqueue(this.currentCaptureChunk); // re-enqueue as last chunk in queue
                }
                else
                {
                    this.currentCaptureChunk = new StreamBuffer(new byte[this.ChunkSize]);
                    this.CaptureChunkQueue.Enqueue(this.currentCaptureChunk);
                }
            }
        }

        lock (this.currentCaptureChunk)
        {
            if (chunkStart)
            {
                if (!fromCurrentPort && !fromPreviousPort)
                {
                    this.previousServerPort = this.currentServerPort;
                    this.currentServerPort = connection.ServerPort;
                }

                this.protocol.Serialize(this.currentCaptureChunk, Ipv4StringToBytes(IPhotonSocket.ServerIpAddress, connection.ServerPort), false); // 6 bytes ip:port
                this.protocol.Serialize(this.currentCaptureChunk, short.MaxValue, false);
                this.updatePeerId = true;
            }

            // as the peerId is assigned from the server, it's not known immediately. it needs to be updated for the chunk when known (non-default value).
            if (this.updatePeerId && fromCurrentPort && (peerId & DefaultPeerId) != DefaultPeerId)
            {
                this.updatePeerId = false;
                byte[] buff = this.currentCaptureChunk.GetBuffer();
                buff[7] = (byte)(peerId >> 0);
                buff[8] = (byte)(peerId >> 8);
            }

            //write: in/out, remote address (byte[]), timestamp, data (byte[])
            ArraySegment<byte> segment = new ArraySegment<byte>(inBuffer, 0, length);

            this.protocol.Serialize(this.currentCaptureChunk, incoming ? (byte)1 : (byte)0, false); // 1 byte
            this.protocol.Serialize(this.currentCaptureChunk, DateTime.UtcNow.ToBinary(), false);   // 8 bytes time
            this.protocol.Serialize(this.currentCaptureChunk, segment, false);                      // byte[] (includes length info with up to 4 bytes)

            if (this.Debug)
            {
                UnityEngine.Debug.Log("Wrote: in: " + incoming + " remote-port: " + connection.ServerPort + " peerId: " + peerId.ToString("X") + " len: " + length);
            }

            // if it fits: write a "end of stream" sign
            if (this.currentCaptureChunk.Position < this.currentCaptureChunk.Length)
            {
                this.currentCaptureChunk.WriteByte(0xff);
                this.currentCaptureChunk.Position--;
            }
        }
    }

    /// <summary>
    /// Finishes the current chunk of recording and calls OnChunkComplete.
    /// </summary>
    /// <remarks>
    /// Use this to finish the final chunk and write it into a file, as you see fit.
    /// </remarks>
    public void CompleteCurrentChunk()
    {
        if (this.currentCaptureChunk != null)
        {
            lock (this.currentCaptureChunk)
            {
                this.currentCaptureChunk.Position = 0;
                this.OnChunkComplete(this.currentCaptureChunk, this);
                this.currentCaptureChunk = null;
            }
        }
    }


    // 6 bytes
    public byte[] Ipv4StringToBytes(string ip, int port)
    {
        string[] parts = ip.Split(new char[] {'.', ':'});

        return new byte[]
        {
            Convert.ToByte(parts[0]), Convert.ToByte(parts[1]),
            Convert.ToByte(parts[2]), Convert.ToByte(parts[3]),
            (byte)(port >> 8), (byte)port
        };
    }

    public string Ipv4BytesToString(byte[] bytes)
    {
        int port = (bytes[4] << 8) + bytes[5];
        return string.Format("{0}.{1}.{2}.{3}:{4}", bytes[0], bytes[1], bytes[2], bytes[3], port);
    }


    public CapturedMessage ChunkToMessageInfo(StreamBuffer chunkBuffer)
    {
        // each chunk starts with 6 bytes ip:port info
        byte[] ipBytes = (byte[])this.protocol.Deserialize(chunkBuffer, (byte)Protocol18.GpType.ByteArray); // should be used in filename
        short peerId = (short)this.protocol.Deserialize(chunkBuffer, (byte)Protocol18.GpType.Short);        // should be used in filename

        CapturedMessage message = new CapturedMessage();
        message.RemoteIpPort = ipBytes;
        message.PeerId = peerId;
        byte firstByte = (byte)this.protocol.Deserialize(chunkBuffer, (byte)Protocol18.GpType.Byte);

        if (firstByte > 1)
        {
            if (this.Debug)
            {
                UnityEngine.Debug.Log("End of stream! Invalid first byte for message!");
            }

            return message;
        }

        message.Incoming = firstByte == 1;
        message.Time = (long)this.protocol.Deserialize(chunkBuffer, (byte)Protocol18.GpType.CompressedLong);

        return message;
    }

    public List<CapturedMessage> ChunkToMessageList(StreamBuffer chunkBuffer)
    {
        List<CapturedMessage> resultList = new List<CapturedMessage>();

        // each chunk starts with 6 bytes ip:port info
        byte[] ipBytes = (byte[])this.protocol.Deserialize(chunkBuffer, (byte)Protocol18.GpType.ByteArray); // should be used in filename
        short peerId = (short)this.protocol.Deserialize(chunkBuffer, (byte)Protocol18.GpType.Short);        // should be used in filename


        // now, we can read each message header and data until the next message won't fit anymore (or until we read a byte 0xff)
        while (chunkBuffer.Position < chunkBuffer.Length - 9)
        {
            CapturedMessage message = new CapturedMessage();
            message.RemoteIpPort = ipBytes;
            message.PeerId = peerId;
            byte firstByte = (byte)this.protocol.Deserialize(chunkBuffer, (byte)Protocol18.GpType.Byte);

            if (firstByte > 1)
            {
                if (this.Debug)
                {
                    UnityEngine.Debug.Log("End of stream!");
                }

                break;
            }

            message.Incoming = firstByte == 1;
            message.Time = (long)this.protocol.Deserialize(chunkBuffer, (byte)Protocol18.GpType.CompressedLong);
            message.Data = (byte[])this.protocol.Deserialize(chunkBuffer, (byte)Protocol18.GpType.ByteArray);


            if (this.Debug)
            {
                string remote = Ipv4BytesToString(ipBytes);
                UnityEngine.Debug.Log("Read. in: " + message.Incoming + " remote: " + remote + " peerId: " + peerId.ToString("X") + " time: " + message.Time + " len: " + message.Data.Length);
            }

            resultList.Add(message);
        }

        return resultList;
    }


    #if PCAP_SUPPORT

    private static CaptureFileWriterDevice captureFileWriter;
    public string capFile = "CapFileTest2.pcap";

    public void WriteMessagesToPcap(List<CapturedMessage> messages, string fileName = null)
    {
        string file = string.IsNullOrEmpty(fileName) ? this.capFile : fileName;

        foreach (CapturedMessage message in messages)
        {
            Packet packet = this.MessageToPacket(message);
            this.WritePacketToFile(packet, message.Time, file);
        }
    }

    private void WritePacketToFile(Packet packet, long time, string file)
    {
        if (captureFileWriter != null && !captureFileWriter.Name.Equals(file))
        {
            captureFileWriter.Close();
            captureFileWriter = null;
        }
        if (captureFileWriter == null)
        {
            captureFileWriter = new CaptureFileWriterDevice(file, FileMode.Create);
        }

        // open the output file
        var dt = DateTime.FromBinary(time);
        var deltaTime = dt.Subtract(new DateTime(1970, 1, 1));
        uint unixTimestamp = (uint)deltaTime.TotalSeconds;
        uint unixMicroseconds = (uint)(deltaTime.Milliseconds * 1000d);
        captureFileWriter.Write(packet.Bytes, new PcapHeader((uint)unixTimestamp, unixMicroseconds, (uint)packet.Bytes.Length, (uint)packet.Bytes.Length));
    }

    private Packet MessageToPacket(CapturedMessage message)
    {
        ushort srcPort = (ushort)(message.Incoming ? message.RemotePort : 0);
        ushort dstPort = (ushort)(message.Incoming ? 0 : message.RemotePort);
        IPAddress srcIp = message.Incoming ? message.RemoteIpAddress : new IPAddress((long)0);
        IPAddress dstIp = message.Incoming ? new IPAddress((long)0) : message.RemoteIpAddress;

        Packet udpPack = new UdpPacket(srcPort, dstPort);
        Packet ipPacket = new IPv4Packet(srcIp, dstIp);


        const string sourceHwAddress = "90-90-90-90-90-90";
        var ethernetSourceHwAddress = System.Net.NetworkInformation.PhysicalAddress.Parse(sourceHwAddress);
        const string destinationHwAddress = "80-80-80-80-80-80";
        var ethernetDestinationHwAddress = System.Net.NetworkInformation.PhysicalAddress.Parse(destinationHwAddress);
        var ethernetPacket = new EthernetPacket(ethernetSourceHwAddress, ethernetDestinationHwAddress, EthernetPacketType.IpV4);

        // Now stitch all of the packets together
        udpPack.PayloadData = message.Data;
        ipPacket.PayloadPacket = udpPack;
        ethernetPacket.PayloadPacket = ipPacket;

        //UnityEngine.Debug.Log(ethernetPacket.ToString());

        return ethernetPacket;
    }

    public void CloseFile()
    {
        if (captureFileWriter != null)
        {
            if (this.Debug)
            {
                UnityEngine.Debug.Log("Closing file." + captureFileWriter.Name);
            }

            captureFileWriter.Close();
        }
    }

    #endif
}

public class CapturedMessage
{
    public bool Incoming;
    public byte[] RemoteIpPort;
    public short PeerId;
    public long Time;
    public byte[] Data;

    private IPAddress remoteIpAddress;  // cached result

    public ushort RemotePort
    {
        get { return (ushort)((this.RemoteIpPort[4] << 8) + this.RemoteIpPort[5]); }
    }

    public IPAddress RemoteIpAddress
    {
        get
        {
            if (this.remoteIpAddress != null)
            {
                return this.remoteIpAddress;
            }

            byte[] address = new byte[] {this.RemoteIpPort[0], this.RemoteIpPort[1], this.RemoteIpPort[2], this.RemoteIpPort[3]};
            remoteIpAddress = new IPAddress(address);
            
            return remoteIpAddress;
        }
    }
}