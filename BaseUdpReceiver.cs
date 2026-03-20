using System.Net;
using System.Net.Sockets;
using System.Text;
using Color = Microsoft.Maui.Graphics.Color;
public delegate void UpdatedHandler(BaseUdpReceiver udpReceiver, Boolean extra);

public class BaseUdpReceiver
{

    public IPAddress ip = IPAddress.Any;
    public int portReceive = 45000;
    public int portSend = 45001;

    public event UpdatedHandler Updated;

    private UdpClient udpClient;

    public struct structInfo
    {
        public UInt16 speed;
        public String gear;
        public Boolean gearAuto;
        public UInt16 rpm;
        public Color slipFL;
        public Color slipFR;
        public Color slipRL;
        public Color slipRR;
        public Color dirtFL;
        public Color dirtFR;
        public Color dirtRL;
        public Color dirtRR;
        public Single accel;
        public Single brake;
        public Single clutch;
        public UInt16 turbo;
    }
    public structInfo Info;

    public struct structInfoExtra
    {
        public UInt16 rpmMax;
        public Color wearFL;
        public Color wearFR;
        public Color wearRL;
        public Color wearRR;
        public byte MaxFuel;
        public byte Fuel; // Kg
        public Single FuelAvg; // Kg/100KM
        public byte NumCars;
        public byte Position;
        public byte NumberOfLaps;
        public byte CompletedLaps;
        public Single DistanceTraveled; // KMs
        public UInt16 turboMax;
    }
    public structInfoExtra InfoExtra;

    public BaseUdpReceiver()
    {
    }


    public void Start()
    {
        if (udpClient != null) { return; }

        udpClient = new UdpClient()
        {
            ExclusiveAddressUse = false,
            EnableBroadcast = true
        };
        udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        udpClient.Client.Bind(new IPEndPoint(ip, portReceive));

        udpClient.BeginReceive(new AsyncCallback(OnUdpDataReceived), this);
    }


    public void End()
    {
        if (udpClient == null) { return; }
        udpClient.Close();
        udpClient.Dispose();
        udpClient = null;
    }


    public void StartDebug()
    {
        (Application.Current as CVJoyMAUI.App).Dispatcher.StartTimer(TimeSpan.FromMilliseconds(100), () =>
        {
            byte len = 20; // Main info every 10Hz
            if (new Random().Next(1000) > 800) len = 36; // Extra info every 2Hz
            if (new Random().Next(1000) > 990) len = (byte)(new Random().Next(33) + 2); // simulate errors

            byte[] recvBuffer = new byte[len];
            for (Int16 x = 0; x < len - 1; x++)
            {
                recvBuffer[x] = (byte)new Random().Next(254);
            }
            if (new Random().Next(1000) < 990) recvBuffer[0] = 255; // simulate errors

            setData(recvBuffer);

            return true;
        });
    }


    private static void OnUdpDataReceived(IAsyncResult result)
    {
        BaseUdpReceiver thi = (BaseUdpReceiver)result.AsyncState;
        IPEndPoint remoteAddr = null;
        byte[] recvBuffer = null;
        try
        {
            recvBuffer = thi.udpClient.EndReceive(result, ref remoteAddr);
        }
        catch (Exception ex)
        {
            (Application.Current as CVJoyMAUI.App).DebugPrint("Error on EndReceive: " + ex.Message);
            return;
        }

        if (recvBuffer != null)
        {
            thi.setData(recvBuffer);
        }

        if (thi.udpClient != null)
        {
            thi.udpClient.BeginReceive(OnUdpDataReceived, thi);
        }
    }


    public void setData(byte[] recvBuffer)
    {
        if (recvBuffer == null) { return; }


        if (recvBuffer[0] == 254 && recvBuffer.Length >= 5)
        {
            (Application.Current as CVJoyMAUI.App).ButtonboxConfiguration = Encoding.ASCII.GetString(recvBuffer,1, recvBuffer.Length-1);
        }
        else if (recvBuffer[0] == 255 && recvBuffer.Length >= 20)
        {

            Info.speed = Convert.ToUInt16(recvBuffer[1] + recvBuffer[2] * 256);
            Info.rpm = Convert.ToUInt16(recvBuffer[3] + recvBuffer[4] * 256);
            switch (recvBuffer[5])
            {
                case 0:
                    Info.gear = " ";
                    break;
                case 1:
                    Info.gear = "R";
                    break;
                case 2:
                    Info.gear = "N";
                    break;
                default:
                    Info.gear = (Convert.ToUInt16(recvBuffer[5]) - 2).ToString();
                    break;
            }
            Info.slipFL = FromArgb(recvBuffer[6], 0, 0, 0);
            Info.slipFR = FromArgb(recvBuffer[7], 0, 0, 0);
            Info.slipRL = FromArgb(recvBuffer[8], 0, 0, 0);
            Info.slipRR = FromArgb(recvBuffer[9], 0, 0, 0);
            Info.gearAuto = (recvBuffer[10] & 1) > 0;
            Info.dirtFL = FromArgb(recvBuffer[11], 128, 128, 128);
            Info.dirtFR = FromArgb(recvBuffer[12], 128, 128, 128);
            Info.dirtRL = FromArgb(recvBuffer[13], 128, 128, 128);
            Info.dirtRR = FromArgb(recvBuffer[14], 128, 128, 128);
            Info.accel = (Single)recvBuffer[15] / 255;
            Info.brake = (Single)recvBuffer[16] / 255;
            Info.clutch = (Single)recvBuffer[17] / 255;
            Info.turbo = Convert.ToUInt16(recvBuffer[18] + recvBuffer[19] * 256);

            if (recvBuffer.Length == 36)
            {
                InfoExtra.wearFL = FromArgb(byte.Min((byte)(recvBuffer[20] * 4), 255), 255, 0, 0);
                InfoExtra.wearFR = FromArgb(byte.Min((byte)(recvBuffer[21] * 4), 255), 255, 0, 0);
                InfoExtra.wearRL = FromArgb(byte.Min((byte)(recvBuffer[22] * 4), 255), 255, 0, 0);
                InfoExtra.wearRR = FromArgb(byte.Min((byte)(recvBuffer[23] * 1), 255), 255, 0, 0);
                InfoExtra.rpmMax = Convert.ToUInt16(recvBuffer[24] + recvBuffer[25] * 256);
                InfoExtra.MaxFuel = recvBuffer[26];
                InfoExtra.Fuel = recvBuffer[27];
                InfoExtra.NumCars = recvBuffer[28];
                InfoExtra.Position = recvBuffer[29];
                InfoExtra.NumberOfLaps = recvBuffer[30];
                InfoExtra.CompletedLaps = recvBuffer[31];
                InfoExtra.DistanceTraveled = (Single)(recvBuffer[32] + recvBuffer[33] * 256) / 1000;
                InfoExtra.FuelAvg = (Single)(recvBuffer[34] + recvBuffer[35] * 256) / 100;
            }

            Updated?.Invoke(this, recvBuffer.Length >= 36);

        }
        else
        {
            (Application.Current as CVJoyMAUI.App).DebugPrint("[0]=" + recvBuffer[0].ToString() + "   len=" + recvBuffer.Length.ToString());
            return;
        }
    }


    public double RpmPercent()
    {
        if (InfoExtra.rpmMax < Info.rpm)
        {
            InfoExtra.rpmMax = Info.rpm;
        }
        if (InfoExtra.rpmMax == 0)
        {
            return 0;
        }
        return (double)Info.rpm / (double)InfoExtra.rpmMax;
    }

    public Color RpmColor()
    {
        int r = (int)(RpmPercent() * 255);
        return FromArgb((byte)(127 + Convert.ToInt16(r / 2)), (byte)r, (byte)(255 - r), 0);
    }

    public double TurboPercent()
    {
        if (InfoExtra.turboMax < Info.turbo)
        {
            InfoExtra.turboMax = Info.turbo;
        }
        if (InfoExtra.turboMax == 0)
        {
            return 0;
        }
        return (double)Info.turbo / (double)InfoExtra.turboMax;
    }

    public Color FromArgb(byte a, byte r, byte g, byte b)
    {
        return Color.FromRgba(r, g, b, a);
    }


    public void SendButtonBoxCommand(string pText, bool pPressed)
    {
        byte[] sendBuffer = new byte[1 + pText.Length];
        sendBuffer[0] = (byte)(pPressed ? 253: 254);
        for (int i = 0; i < pText.Length; i++)
        {
            sendBuffer[i + 1] = (byte)pText[i];
        }
        try
        {
            //UdpClient udpClient45001 = new UdpClient(new IPEndPoint(ip, portSend));
            udpClient.Send(sendBuffer, sendBuffer.Length, new IPEndPoint(IPAddress.Broadcast, portSend));
        }
        catch (Exception ex)
        {
            (Application.Current as CVJoyMAUI.App).DebugPrint("Error sending ButtonBox config: " + ex.Message);
        }
    }
}