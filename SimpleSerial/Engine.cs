using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace SimpleSerial
{
    class ArtNetEngine
    {
        #region "Public"

        public ArtNetEngine(string name, string broadcastAddress)
        {
            artNetHeader = new byte[] { 0x41, 0x72, 0x74, 0x2d, 0x4e, 0x65, 0x74, 0 };
            artNetAddress = new byte[] { 0x7f, 0, 0, 1, Convert.ToByte(this.LoByte(0x1936)), Convert.ToByte(this.HiByte(0x1936)) };
            artNetShortName = "Art-Net Node";

            if (name == "")
            {
                artNetLongName = "LiteCore";
                artNetNodeReport = "LiteCore";
            }
            else
            {
                artNetLongName = name;
                artNetNodeReport = name;
            }


            if (broadcastAddress == "")
            {
                BroadcastAddress = "255.255.255.255";
            }
            else
            {
                BroadcastAddress = broadcastAddress;
            }

            udpServer = new UdpClient(0x1936);
            udpServer.EnableBroadcast = true;
            udpServer.Client.SendTimeout = 100;
            udpServer.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            updateTimer = new System.Timers.Timer();
            updateTimer.Interval = 400;
            updateTimer.Elapsed += new ElapsedEventHandler(timer_Elapsed);

            status = runStatus.Stopped;
        }

        public void Start()
        {
            updateTimer.Enabled = true;
            updateTimer.Start();
            status = runStatus.Running;
        }

        public void Pause()
        {
            updateTimer.Enabled = false;
            updateTimer.Stop();
            status = runStatus.Paused;
        }

        public void SendDMX(short universe, byte[] data, int DataLength)
        {
            byte[] packet = new byte[(0x11 + DataLength) + 1];
            Buffer.BlockCopy(this.artNetHeader, 0, packet, 0, this.artNetHeader.Length);
            packet[8] = Convert.ToByte(this.LoByte(0x5000));
            packet[9] = Convert.ToByte(this.HiByte(0x5000));
            packet[10] = 0;          //ProtVerHi
            packet[11] = 14;         //ProtVerLo
            packet[12] = 0;          //Sequence
            packet[13] = 0;          //Physical
            packet[14] = Convert.ToByte(this.LoByte(universe));
            packet[15] = Convert.ToByte(this.HiByte(universe));
            packet[0x10] = Convert.ToByte(this.HiByte(DataLength));
            packet[0x11] = Convert.ToByte(this.LoByte(DataLength));

            try
            {
                Buffer.BlockCopy(data, 0, packet, 0x12, DataLength);
            }
            catch (Exception exception1)
            {
                Error(exception1, new EventArgs());
            }

            broadcast(packet);
        }

        #endregion

        //---------------------------------------------------------------------------------------------------------------------------------------------

        #region "Private"

        private void sendArtPoll()
        {
            byte[] packet = new byte[14];

            Buffer.BlockCopy(this.artNetHeader, 0, packet, 0, this.artNetHeader.Length);

            packet[8] = Convert.ToByte(this.LoByte(0x2000));
            packet[9] = Convert.ToByte(this.HiByte(0x2000));
            packet[10] = 0;      //ProtVerHi
            packet[11] = 14;     //ProtVerLo
            packet[12] = 0;      //TalkToMe
            packet[13] = 0;      //Priority
            broadcast(packet);
        }

        private void sendArtPollReply()
        {
            byte[] header = new byte[14];

            byte num;
            byte[] packet = new byte[240];
            Buffer.BlockCopy(this.artNetHeader, 0, packet, 0, this.artNetHeader.Length);
            packet[8] = Convert.ToByte(this.LoByte(0x2100));
            packet[9] = Convert.ToByte(this.HiByte(0x2100));
            Buffer.BlockCopy(this.artNetAddress, 0, packet, 10, this.artNetAddress.Length);
            packet[0x10] = 1;
            packet[0x11] = 1;
            packet[0x12] = 0;
            packet[0x13] = 0;
            packet[20] = Convert.ToByte(this.HiByte(0xff));
            packet[0x15] = Convert.ToByte(this.LoByte(0xff));
            packet[0x16] = 0;
            packet[0x17] = 0;
            packet[0x18] = 0;
            packet[0x19] = 0;
            byte num2 = (byte)((0x1a + this.artNetShortName.Length) - 1);
            for (num = 0x1a; num <= num2; num = (byte)(num + 1))
            {
                packet[num] = (byte)Asc(new string(this.artNetShortName.ToCharArray(num - 0x1a, 1)));
            }
            byte num3 = (byte)((0x2c + this.artNetLongName.Length) - 1);
            for (num = 0x2c; num <= num3; num = (byte)(num + 1))
            {
                packet[num] = (byte)Asc(new string(this.artNetLongName.ToCharArray(num - 0x2c, 1)));
            }
            byte num4 = (byte)((0x6c + this.artNetNodeReport.Length) - 1);
            for (num = 0x6c; num <= num4; num = (byte)(num + 1))
            {

                packet[num] = (byte)Asc(new string(this.artNetNodeReport.ToCharArray(num - 0x6c, 1)));
            }
            packet[0xac] = 0;
            packet[0xad] = 4;
            packet[0xae] = 0;
            packet[0xaf] = 0;
            packet[0xb0] = 0;
            packet[0xb1] = 0;
            packet[0xb2] = 0;
            packet[0xb3] = 0;
            packet[180] = 0;
            packet[0xb5] = 0;
            packet[0xb6] = 0;
            packet[0xb7] = 0;
            packet[0xb8] = 0;
            packet[0xb9] = 0;
            packet[0xba] = 0;
            packet[0xbb] = 0;
            packet[0xbc] = 0;
            packet[0xbd] = 0;
            packet[190] = 0;
            packet[0xbf] = 0;
            packet[0xc0] = 0;
            packet[0xc1] = 0;
            packet[0xc2] = 0;
            packet[0xc3] = 0;
            packet[0xc4] = 0;
            num = 0xc5;
            do
            {
                packet[num] = 0;
                num = (byte)(num + 1);
            }
            while (num <= 0xc7);
            packet[200] = 0;
            packet[0xc9] = 0;
            packet[0xca] = 0;
            packet[0xcb] = 0;
            packet[0xcc] = 0;
            packet[0xcd] = 0;
            packet[0xce] = 0;
            num = 0xcf;
            do
            {
                packet[num] = num;
                num = (byte)(num + 1);
            }
            while (num <= 0xef);
            packet[0xef] = 0xff;

            broadcast(packet);

        }

        private void recieve()
        {

            IPEndPoint RemoteServer = new IPEndPoint(IPAddress.Any, 0);
            for (; ; )
            {
                try
                {
                    byte[] RecPacket = udpServer.Receive(ref RemoteServer);
                    string test = Encoding.ASCII.GetString(RecPacket);

                    switch (((OpCodes)(RecPacket[this.artNetHeader.Length + 1] << (8 + RecPacket[this.artNetHeader.Length]))))
                    {
                        case OpCodes.OpPoll:
                            this.onPoll(RecPacket);
                            break;

                        case OpCodes.OpPollReply:
                            this.onPollReply(RecPacket, RemoteServer);
                            break;

                        case OpCodes.OpDmx:
                            this.onDmxRecieved(RecPacket, RemoteServer);
                            break;
                    }

                }
                catch (Exception ex)
                {
                    //Error(ex, new EventArgs()); 

                    //Nothing has happened so we shall do nout.
                }
            }
        }

        private void onDmxRecieved(byte[] Data, IPEndPoint RemoteServer)
        {
            DmxInPacket Dmx = new DmxInPacket();
            Dmx.Protocol = SupportedTypes.ArtNet;

            //Set Universe number.
            Dmx.Universe = Convert.ToInt16(Data[14]);
            Buffer.BlockCopy(Data, 18, Dmx.DmxData, 0, Dmx.DmxData.Length);

            ArtNetDetected = true;

            lastPacketRecieved = Dmx;


            RecievingDMX(Dmx, new EventArgs());

        }

        private void onPollReply(byte[] RecPacket, IPEndPoint RemoteServer)
        {
            throw new NotImplementedException();
        }

        private void onPoll(byte[] RecPacket)
        {
            sendArtPollReply();
        }

        private void broadcast(byte[] data)
        {
            if (status == runStatus.Paused)
            {
                Error("ArtNet is Paused", new EventArgs());
            }
            else if (status == runStatus.Stopped)
            {
                Error("ArtNet has not been started", new EventArgs());
            }
            else if (status == runStatus.Running)
            {
                udpServer.Send(data, data.Length, BroadcastAddress, 0x1936);
            }

        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            sendArtPoll();
            recieve();

        }

        #endregion

        //---------------------------------------------------------------------------------------------------------------------------------------------

        #region "Work Around"

        //Work around for lack of Asc in C# vs VB.
        static short Asc(string String)
        {
            return Encoding.Default.GetBytes(String)[0];
        }

        static string Chr(int CharCode)
        {
            if (CharCode > 255)
                throw new ArgumentOutOfRangeException("CharCode", CharCode, "CharCode must be between 0 and 255.");
            return Encoding.Default.GetString(new[] { (byte)CharCode });
        }

        private object LoByte(int wParam)
        {
            return (wParam & 0xffL);
        }

        private object HiByte(int wParam)
        {
            return ((wParam / 0x100) & 0xffL);
        }

        private short MakeInt16(byte lsb, byte msb)
        {
            short num2 = msb;
            num2 = (short)(num2 << 8);
            return (short)(num2 + lsb);
        }

        #endregion

        //---------------------------------------------------------------------------------------------------------------------------------------------

        //Events
        public event EventHandler RecievingDMX;
        private event EventHandler Error;

        //---------------------------------------------------------------------------------------------------------------------------------------------

        //Fields
        public string BroadcastAddress { get; set; }
        private UdpClient udpServer;
        private System.Timers.Timer updateTimer;
        private byte[] artNetAddress;
        private string artNetLongName;
        private string artNetShortName;
        private byte[] artNetHeader;
        private string artNetNodeReport;
        private runStatus status;
        private bool ArtNetDetected = false;
        private DmxInPacket lastPacketRecieved;
        private enum runStatus
        {
            Running,
            Paused,
            Stopped
        }
    }



    public enum OpCodes
    {
        OpPoll = 0x2000,
        OpPollReply = 0x2100,
        OpDiagData = 0x2300,
        OpCommand = 0x2400,
        OpDmx = 0x5000,
        OpNzs = 5100,
        OpAddress = 0x6000,
        OpInput = 0x7000,
        OpTodRequest = 0x8000,
        OpTodData = 0x8100,
        OpTodControl = 0x8200,
        OpRdm = 0x8300,
        OpRdmSub = 0x8400,
        OpVideoSetup = 0xa010,
        OpVidoPalette = 0xa020,
        OpVideoData = 0xa040,
        OpVideoMaster = 0xf000,
        OpMacMaster = 0xf000,
        OpMacSlave = 0xf100,
        OpFirmwaveMaster = 0xf200,
        OpFirmwareReply = 0xf300,
        OpFileTnMaster = 0xf400,
        OpFileFnMaster = 0xf500,
        OpFileFnReply = 0xf600,
        OplpProg = 0xf800,
        OplpOProgReply = 0xf900,
        OpMedia = 0x9000,
        OpMediaPatch = 0x9100,
        OpMediaControl = 0x9200,
        OpMediaContrlReply = 0x9300,
        OpTimeCode = 0x9700,
        OpTimeSync = 0x9800,
        OpTrigger = 0x9900,
        OpDirectory = 0x9a00,
        OpDirectoryReply = 0x9b00
    }
}
