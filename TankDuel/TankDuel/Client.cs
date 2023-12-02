using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;
using DataStructuresManipulation;

namespace TankDuel
{
    public class Client
    {
        private Socket _client;
        private EndPoint _remotePoint;
        private IPAddress _address;
        private Timer _initPendingTimer;
        private bool _initFlagStart = true;
        private int _port;
        byte[] _buffer = new byte[2048];

        private int _maxOnline;
        public int MaxOnline { get => _maxOnline; }

        public string Error;
        public Client(string address, int port)
        {
            _address = IPAddress.Parse(address);
            _port = port;
            _client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _remotePoint = new IPEndPoint(_address, _port);
            _initPendingTimer = new Timer();
        }

        private void ClearBuffer()
        {
            Array.Clear(_buffer, 0, _buffer.Length);
        }

        private void CopyToBuffer(byte[] destination)
        {
            Array.Copy(destination, _buffer, destination.Length);
        }

        public int InitializeOnServer()
        {
            StartInitTimer();
            try
            {
                ClearBuffer();
                _client.SendTo(_buffer, _remotePoint);
                _client.ReceiveFrom(_buffer, ref _remotePoint);
                _maxOnline = BitConverter.ToInt32(_buffer, 0);
                return 1;
            }
            catch(Exception ex)
            {
                Debug.Print(ex.Message);
                return UpdateInitTimer();
            }
        }

        private void StartInitTimer()
        {
            if (_initFlagStart)
            {
                _initPendingTimer.Reset();
                _initFlagStart = false;
            }
        }

        private int UpdateInitTimer()
        {
            _initPendingTimer.Update();
            if(_initPendingTimer.Time >= 5f)
            {
                _initPendingTimer.Stop();
                _initFlagStart = true;
                return -1;
            }
            return 0;
        }

        public List<DataPackageOutput> GetPlayersData()
        {
            List<DataPackageOutput> playersData = new List<DataPackageOutput>();
            for (int i = 0; i < _maxOnline; ++i)
            {
                ClearBuffer();
                _client.ReceiveFrom(_buffer, ref _remotePoint);
                DataPackageOutput data = DataPackageReader.FromBytes(_buffer);
                playersData.Add(data);
            }
            return playersData;
        }
        public byte[,] InitializeObjects()
        {
            ClearBuffer();
            _client.ReceiveFrom(_buffer, ref _remotePoint);
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream(_buffer);
            byte[,] array = (byte[,])bf.Deserialize(ms);
            return array;
        }
        public SettingsData InitializeSettings()
        {
            ClearBuffer();
            _client.ReceiveFrom(_buffer, ref _remotePoint);
            return DataPackageReader.SettingsFromBytes(_buffer);
        }

        public byte[] UpdateMap()
        {
            try
            {
                ClearBuffer();
                _client.ReceiveFrom(_buffer, ref _remotePoint);
                int size = BitConverter.ToInt32(_buffer, 0);
                ClearBuffer();
                _client.ReceiveFrom(_buffer, ref _remotePoint);
                byte[] indexes = new byte[size];
                Buffer.BlockCopy(_buffer, 0, indexes, 0, indexes.Length);
                return indexes;
            }
            catch(Exception ex)
            {
                Error = ex.Message + "\n" + ex.StackTrace;
                Debug.Print(ex.Message + "\n" + ex.StackTrace);
            }
            return null;
        }

        public List<DataPackageOutput> ClientProcessing(DataPackageInput dataInput)
        {
            try
            {
                ClearBuffer();
                CopyToBuffer(DataPackageReader.GetBytes(dataInput));
                _client.SendTo(_buffer, _remotePoint);
                List<DataPackageOutput> dataFromServer = new List<DataPackageOutput>();
                for (int i = 0; i < _maxOnline; ++i)
                {
                    ClearBuffer();
                    _client.ReceiveFrom(_buffer, ref _remotePoint);
                    dataFromServer.Add(DataPackageReader.FromBytes(_buffer));
                }
                return dataFromServer;
            }
            catch(SocketException ex)
            {
                Error = ex.Message + "\n" + ex.StackTrace;
                Debug.Print(ex.Message + "\n" + ex.StackTrace);
                //_client.Close();
                //throw ex;
            }
            return null;
        }

        ~Client()
        {
            _client.Close();
            _client.Dispose();
        }
    }
}
