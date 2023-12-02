using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Server.DataStructuresManipulation;

namespace Server
{
    class Server
    {
        private IPEndPoint _endPoint;
        private Socket _server;
        private IPAddress _address;
        private int _port;

        private int _currentOnline;
        private int _maxOnline;
        private List<EndPoint> _clientAddresses;
        public List<Player> Players { get; private set; }

        byte[] _buffer = new byte[2048];

        private string _serverState;
        public string ServerState { get => _serverState; private set => _serverState = value; }

        public Server(string address, int port)
        {
            _maxOnline = 1;
            _clientAddresses = new List<EndPoint>();
            Players = new List<Player>();
            _address = IPAddress.Parse(address);
            //_address = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0];
            _port = port;
            CreateSocket();
            Bind();
        }

        private void ClearBuffer()
        {
            Array.Clear(_buffer, 0, _buffer.Length);
        }

        private void CopyToBuffer(byte[] destination)
        {
            Array.Copy(destination, _buffer, destination.Length);
        }

        private void CreateSocket()
        {
            _endPoint = new IPEndPoint(_address, _port);
            _server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        }

        private void Bind()
        {
            try
            {
                _server.Bind(_endPoint);
            }
            catch(Exception ex)
            {
                _server.Close();
                Console.WriteLine(ex.Message);
                throw ex;
            }
        }

        public void InitializeClient()
        {
            ProjectileData projectileEmpty1 = new ProjectileData(0, 0, 0, 0);
            ProjectileData projectileEmpty2 = new ProjectileData(1, 0, 0, 0);
            ProjectileData projectileEmpty3 = new ProjectileData(2, 0, 0, 0);
            DataPackageOutput data = new DataPackageOutput(150f, 400f, 3, 0, false, 5, 3, 1, 
                projectileEmpty1, projectileEmpty2, projectileEmpty3);
            while (_currentOnline < _maxOnline)
            {
                ClearBuffer();
                EndPoint remoteIp = new IPEndPoint(IPAddress.Any, 0);
                _server.ReceiveFrom(_buffer, ref remoteIp);
                _clientAddresses.Add(remoteIp);
                Players.Add(new Player(remoteIp, data));
                _currentOnline++;
                CopyToBuffer(BitConverter.GetBytes(_maxOnline));
                _server.SendTo(_buffer, remoteIp);
                data.x = 750f;
                data.CurrentDirection = 2;
            }
        }
        public void InitializeObjects(byte[,] array)
        {
            foreach(EndPoint ep in _clientAddresses)
            {
                BinaryFormatter bf = new BinaryFormatter();
                MemoryStream ms = new MemoryStream();
                bf.Serialize(ms, array);
                _server.SendTo(ms.GetBuffer(), ep);
            }
        }

        public void InitializeSettings(SettingsData settings)
        {
            foreach (EndPoint ep in _clientAddresses)
            {
                //BinaryFormatter bf = new BinaryFormatter();
                //MemoryStream ms = new MemoryStream();
                //bf.Serialize(ms, settings);
                ClearBuffer();
                CopyToBuffer(DataPackageReader.GetBytes(settings));
                _server.SendTo(_buffer, ep);
            }
        }

        public void UpdateMap(byte[] indexes)
        {
            try
            {
                foreach (EndPoint ep in _clientAddresses)
                {
                    int size = indexes.Length;
                    ClearBuffer();
                    CopyToBuffer(BitConverter.GetBytes(size));
                    _server.SendTo(_buffer, ep);
                    ClearBuffer();
                    CopyToBuffer(indexes);
                    _server.SendTo(_buffer, ep);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
            }
        }

        public void ReceiveFromClients()
        {
            try
            {
                for (int i = 0; i < _maxOnline; ++i)
                {
                    EndPoint remoteIp = new IPEndPoint(IPAddress.Any, 0);
                    ClearBuffer();
                    _server.ReceiveFrom(_buffer, ref remoteIp);
                    foreach (Player player in Players)
                    {
                        if (player.Address.Equals(remoteIp))
                        {
                            player.dataInput = DataPackageReader.FromBytes(_buffer);
                            _serverState = $"buffer: {player.dataInput} from address {remoteIp} for {player.Address}";
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
            }
        }

        public void SendToClients()
        {
            try
            {
                foreach (EndPoint address in _clientAddresses)
                {
                    foreach (Player player in Players)
                    {
                        ClearBuffer();
                        if (player.Address.Equals(address))
                        {
                            player.dataOutput.playerType = 1;
                        }
                        else
                        {
                            player.dataOutput.playerType = 0;
                        }
                        CopyToBuffer(DataPackageReader.GetBytes(player.dataOutput));
                        _server.SendTo(_buffer, address);
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
            }
        }

        ~Server()
        {
            _server.Close();
            _server.Dispose();
        }
        
    }
}
