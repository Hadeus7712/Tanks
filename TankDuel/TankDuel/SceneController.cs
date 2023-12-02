using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DirectInput;
using SharpDX.Mathematics.Interop;
using DX;
using Types;
using DataStructuresManipulation;
using System.Diagnostics;
using SharpDX.Windows;

namespace TankDuel
{
    class SceneController : IDisposable
    {
        static private readonly List<string> _texturePaths = new List<string>() 
        {
            "Insurmountable.png", "Destructible.png", "IceGround.png", "River.png", "Swamp.png", 
            "Tank.png","ProjectileV2.png", "SpeedDown.png",
            "Armor.png", "ProjectileSpeed.png", "SpeedUp.png", 
            "Ground.png"
        };

        private Timer _globalTime;
        private int _fps;
        private float _time;
        private float _dT;

        private RenderForm _renderForm;
        private DX2D _dx2d;
        private InputController _inputController;
        private WindowRenderTarget _renderTarget;
        private List<Bitmap> _bitmaps;
        private Brush _boundsColor;
        private Brush _textColor;
        private Brush _textUIColor;

        private Client _client;

        private GameStartScreen _startScreen;
        private RectangleF _mousePosition = new RectangleF(0, 0, 0, 0);

        private ServerConnection _connectionFlag;
        private bool _initSceneFlag = true;

        private List<Block> _gameobjects;
        private List<Tank> _players;
        private List<PlayerStatsUI> _stats;

        private bool _boundsDisplayEnable;
        private bool _statisticDisplayEnable;

        private float _size, _semiSize, _projectileSize, _playerSize, _offset, _quarterOffset;
        private int _unitX, _unitY;

        //private float[] _settings;
        private SettingsData _settings;

        private StringBuilder _stringBuilder;

        private SceneObjectGenerator _SOGenerator;

        private int _maxPlayers;
        public DataPackageInput PlayerDataInput;
        public List<DataPackageOutput> PlayersDataFromServer;


        private string _lastScreenText;
        public SceneController(/*DX2D dxd2, InputController inputController*/)
        {
            _renderForm = new RenderForm("TankDuel");
            _dx2d = new DX2D(_renderForm);
            _inputController = new InputController(_renderForm);

            _client = new Client("127.0.0.1", 5555);

            //RenderForm_Resize(this, null);
            /*_dx2d = dxd2;
            _inputController = inputController;*/

            _renderTarget = _dx2d.RenderTarget;
            _globalTime = new Timer();
            _bitmaps = new List<Bitmap>();
            LoadBitmaps();
            _boundsColor = _dx2d.GreenBrush;
            _textColor = _dx2d.WhiteBrush;
            _textUIColor = _dx2d.BlackBrush;
            _boundsDisplayEnable = false;
            _statisticDisplayEnable = false;
            _stringBuilder = new StringBuilder();
            _startScreen = new GameStartScreen(_renderTarget.Size, new Size2F(200, 50));

            _renderForm.MouseMove += (sender, args) => {
                _mousePosition.X = args.X;
                _mousePosition.Y = args.Y;
            };
            _connectionFlag = ServerConnection.Start;
        }
        private void BoundsDisplayToggle()
        {
            _boundsDisplayEnable = !_boundsDisplayEnable;
        }

        private void StatisticDisplayToggle()
        {
            _statisticDisplayEnable = !_statisticDisplayEnable;
        }
        private void LoadBitmaps()
        {
            int index;
            for (int i = 0; i < _texturePaths.Count; i++)
            {
                index = _dx2d.LoadBitmap(_texturePaths[i]);
                _bitmaps.Add(_dx2d.Bitmaps[index]);
            }
        }

        public void SceneClientInitialization()
        {
            _maxPlayers = _client.MaxOnline;

            List<DataPackageOutput> playersData = _client.GetPlayersData();
            PlayerDataInput = new DataPackageInput(0, 3, false);
            PlayersDataFromServer = playersData;

            _SOGenerator = new SceneObjectGenerator(_bitmaps);
            byte[,] map = _client.InitializeObjects();
            _SOGenerator.SetMap(map);
            _unitX = map.GetLength(1);
            _unitY = map.GetLength(0);

            _settings = _client.InitializeSettings();
            _size = _settings.BlockSize;
            _semiSize = _settings.BonusSize;
            _projectileSize = _settings.ProjectileSize;
            _playerSize = _settings.PlayerSize;
            _offset = _settings.BlockOffset;
            _quarterOffset = _settings.QuarterOffset;

            _gameobjects = _SOGenerator.GenerateField(_offset, _size, _semiSize, _quarterOffset);
            _stats = new List<PlayerStatsUI>();
            _players = new List<Tank>();
            GeneratePlayers(playersData);
            GenerateUIStatsPanels();
        }

        private void Render()
        {
            _renderTarget.BeginDraw();
            _renderTarget.Clear(Color.DarkGray);
            SystemUpdate();
            switch(_connectionFlag)
            {
                case ServerConnection.Start:
                    DrawMenuUserInterface();
                    CheckButtonClick();
                    break;
                case ServerConnection.Pending:
                    DrawLoadingScreen();
                    int initFlag = _client.InitializeOnServer();
                    if (initFlag == 1)
                    {
                        _connectionFlag = ServerConnection.Processing;
                    }
                    else if(initFlag == -1)
                    {
                        _connectionFlag = ServerConnection.Start;
                    }
                    break;
                case ServerConnection.Processing:
                    if(_initSceneFlag)
                    {
                        SceneClientInitialization();
                        _initSceneFlag = false;
                    }
                    Update();
                    PlayersDataFromServer = _client.ClientProcessing(PlayerDataInput);
                    UpdateGameMap(_client.UpdateMap());
                    break;
                case ServerConnection.End:
                    DrawWLScreen();
                    break;
                default:
                    break;
            }
            _renderTarget.EndDraw();
        }
        private void SystemUpdate()
        {
            GlobalTimeUpdate();
            UpdateKeyboardEvents();
            UpdateMouseEvents();
        }
        public void Update()
        {
            UpdateOutputPlayersInstance();
            DrawBackGround();
            DrawField();
            DrawTanks();
            DrawTanksProjectile();
            DrawPlayerUI();
            DrawStatistic();
            CheckGameEndCondition();
        }

        public void DrawLoadingScreen()
        {
            _renderTarget.Transform = Matrix3x2.Rotation(0);
            _renderTarget.FillRectangle(_startScreen.ScreenRect, _dx2d.BlackBrush);
            _renderTarget.DrawText("connection", _dx2d.TextFormatMenuButton,
                _startScreen.ScreenRect, _dx2d.WhiteBrush);

        }
        private void DrawMenuUserInterface()
        {
            _renderTarget.Transform = Matrix3x2.Rotation(0);
            _renderTarget.FillRectangle(_startScreen.ScreenRect, _dx2d.BlackBrush);
            _renderTarget.FillRectangle(_startScreen.MenuButtonRect, _dx2d.WhiteBrush);
            _renderTarget.DrawText("Join to server", 
                _dx2d.TextFormatMenuButton, _startScreen.MenuButtonRect, _dx2d.BlackBrush);
            BoundsDisplayDraw(_startScreen.MenuButtonRect);
        }


        private void DrawWLScreen()
        {
            _renderTarget.Transform = Matrix3x2.Rotation(0);
            _renderTarget.FillRectangle(_startScreen.ScreenRect, _dx2d.TransparentBrush);
            _renderTarget.DrawText(_lastScreenText, _dx2d.TextFormatLastScreen, 
                _startScreen.ScreenRect, _dx2d.WhiteBrush);
        }
        private void CheckGameEndCondition()
        {
            foreach (Tank tank in _players)
            {
                if (tank.Armor <= 0)
                {
                    _connectionFlag = ServerConnection.End;
                    switch (tank.PlayerType)
                    {
                        case PlayerType.player:
                            _lastScreenText = "defeat";
                            break;
                        case PlayerType.enemy:
                            _lastScreenText = "victory";
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        private void CheckButtonClick()
        {
            if (_startScreen.MenuButtonRect.Intersects(_mousePosition) && _inputController.LMB)
            {
                _connectionFlag = ServerConnection.Pending;
            }
        }

        private void GeneratePlayers(List<DataPackageOutput> playersData)
        {
            for (int i = 0; i < _maxPlayers; i++)
            {
                _players.Add(_SOGenerator.GenerateTank(playersData[i], _playerSize));
                _players[i].UpdateStatsValues(playersData[i].Armor, playersData[i].ProjectileSpeed, playersData[i].Speed);
                GenerateProjectiles(_players[i]);
            }
        }

        private void GenerateProjectiles(Tank tank)
        {
            for(int i = 0; i < 3; i++)
            {
                tank.Projectiles[i] = _SOGenerator.GenerateProjectile(new Vector2(0,0), _projectileSize);
            }
        }

        private void GenerateUIStatsPanels()
        {
            Vector2 pos = new Vector2(_size, 0);
            Vector2 size = new Vector2(_size * 5, _size);
            for (int i = 0; i < _maxPlayers; i++)
            {
                _stats.Add(new PlayerStatsUI(pos, size.X, size.Y, _dx2d.TextFormatUserInterface.FontSize, _settings));
                pos.X = _renderTarget.Size.Width - _size - size.X;
                _stats[i].CreateIconsPosition();
            }

        }

        public void UpdateGameMap(byte[] deleterIndexes)
        {
            for(int i = 0; i < deleterIndexes.Length; i++)
            {
                _gameobjects.RemoveAt(deleterIndexes[i]);
            }
        }

        private void GlobalTimeUpdate()
        {
            _globalTime.Update();
            _fps = _globalTime.FPS;
            _time = _globalTime.Time;
            _dT = _globalTime.dT;
        }
        private void UpdateKeyboardEvents()
        {
            _inputController.UpdateKeyboardState();
            if (_inputController.KeyboardUpdated)
            {
                UpdateDisplayKeys();
                UpdateTankDirection();
            }
        }
        private void UpdateMouseEvents()
        {
            _inputController.UpdateMouseState();
            if (_inputController.MouseUpdated)
            {
                UpdateMouseInputs();
            }
        }
        private void UpdateDisplayKeys()
        {
            if (_inputController.KeyQ)
            {
                BoundsDisplayToggle();
            }
            if (_inputController.KeyZ)
            {
                StatisticDisplayToggle();
            }
        }

        private void UpdateOutputPlayersInstance()
        {
            for (int i = 0; i < _maxPlayers; ++i)
            {
                _players[i].UpdateStatsValues(
                    PlayersDataFromServer[i].Armor, 
                    PlayersDataFromServer[i].ProjectileSpeed, 
                    PlayersDataFromServer[i].Speed
                    );
                _players[i].ChangePosition(PlayersDataFromServer[i].x, PlayersDataFromServer[i].y);
                _players[i].ChangeDirection((Directions)PlayersDataFromServer[i].CurrentDirection);

                if (PlayersDataFromServer[i].IsShoot != 0)
                {
                    _players[i].InitProjectilesData(
                        PlayersDataFromServer[i].ProjectileData1,
                        PlayersDataFromServer[i].ProjectileData2,
                        PlayersDataFromServer[i].ProjectileData3
                        );
                }
                _players[i].UpdateProjectilesData(
                        PlayersDataFromServer[i].ProjectileData1,
                        PlayersDataFromServer[i].ProjectileData2,
                        PlayersDataFromServer[i].ProjectileData3
                        );

            }
        }

        private void UpdateTankDirection()
        {
            if (_inputController.KeyboardState.IsPressed(Key.W))
            {
                PlayerDataInput.CurrentInput = (byte)Key.W;
                PlayerDataInput.CurrentDirection = (byte)Directions.Up;
            }
            else if (_inputController.KeyboardState.IsPressed(Key.A))
            {
                PlayerDataInput.CurrentInput = (byte)Key.A;
                PlayerDataInput.CurrentDirection = (byte)Directions.Left;
            }
            else if (_inputController.KeyboardState.IsPressed(Key.S))
            {
                PlayerDataInput.CurrentInput = (byte)Key.S;
                PlayerDataInput.CurrentDirection = (byte)Directions.Down;
            }
            else if (_inputController.KeyboardState.IsPressed(Key.D))
            {
                PlayerDataInput.CurrentInput = (byte)Key.D;
                PlayerDataInput.CurrentDirection = (byte)Directions.Right;
            }
            else
            {
                PlayerDataInput.CurrentInput = 0;
            }

        }
        private void UpdateMouseInputs()
        {
            PlayerDataInput.IsShooting = (byte)(_inputController.LMB ? 1 : 0);
        }
        private void DrawPropertyRow(Tank tank, PlayerStatsUI statsUI)
        {
            for (int i = 0; i < tank.Armor; ++i)
            {
                _renderTarget.DrawBitmap(_bitmaps[(int)ObjectType.Armor], 
                    statsUI.IconsRect[i], 1.0f, BitmapInterpolationMode.Linear);
                BoundsDisplayDraw(statsUI.IconsRect[i]);
            }
        }
        private void DrawPlayerUI()
        {
            for(int i = 0; i < _players.Count; i++)
            {
                _renderTarget.Transform = Matrix3x2.Rotation(0);
                _renderTarget.DrawText(_players[i].PlayerType.ToString(), _dx2d.TextFormatUserInterface, _stats[i].Rect, _textUIColor);
                DrawPropertyRow(_players[i], _stats[i]);
                BoundsDisplayDraw(_stats[i].Rect);
            }
        }
        private void DrawTank(Tank tank)
        {
            int index = tank.Block.IntObjectType();
            _renderTarget.Transform = Matrix3x2.Rotation(tank.Angle, tank.GetCenter());
            _renderTarget.DrawBitmap(_bitmaps[index], tank.BitmapRect, 1.0f, BitmapInterpolationMode.Linear);
            BoundsDisplayDraw(tank.Block.Rect);
        }

        private void DrawTanks()
        {
            for(int i = 0; i <_players.Count; i++)
            {
                DrawTank(_players[i]);
            }
        }
        private void DrawProjectile(Projectile projectile)
        {
            int index = projectile.Block.IntObjectType();
            _renderTarget.Transform = Matrix3x2.Rotation(projectile.Angle, projectile.GetCenter());
            _renderTarget.DrawBitmap(_bitmaps[index], projectile.BitmapRect, 1.0f, BitmapInterpolationMode.Linear);
            BoundsDisplayDraw(projectile.Block.Rect);
        }
        private void DrawTanksProjectile()
        {
            for (int i = 0; i < _players.Count; i++)
            {
                foreach(Projectile projectile in _players[i].Projectiles)
                {
                    if (projectile.Display)
                    {
                        DrawProjectile(projectile);
                    }
                }
            }
        }
        private void DrawField()
        {
            _renderTarget.Transform = Matrix3x2.Rotation(0);
            foreach (Block block in _gameobjects)
            {
                _renderTarget.DrawBitmap(_bitmaps[block.IntObjectType()], block.Rect, 1.0f, BitmapInterpolationMode.Linear);
                BoundsDisplayDraw(block.Rect);
            }
        }

        private void DrawBackGround()
        {
            _renderTarget.Transform = Matrix3x2.Rotation(0);
            RectangleF rect = new RectangleF(_size, _size, _size * _unitX, _size * _unitY);
            _renderTarget.DrawBitmap(_bitmaps[(int)ObjectType.Ground], rect, 1.0f, BitmapInterpolationMode.Linear);
        }

        private void DrawStatistic()
        {
            if (_statisticDisplayEnable)
            {
                _renderTarget.Transform = Matrix3x2.Rotation(0);
                RectangleF rect = new RectangleF(0, 0, _renderTarget.Size.Width, _size);
                _renderTarget.DrawText(Statistic(), _dx2d.TextFormatStatistic, rect, _textColor);
                //BoundsDisplayDraw(rect);
            }
        }

        private string Statistic()
        {
            _stringBuilder.Clear();
            _stringBuilder.AppendLine($"[Time: {_globalTime.Time}] [FPS: {_fps}]");
            _stringBuilder.AppendLine($"==========output data==========");
            _stringBuilder.AppendLine(PlayerDataInput.ToString());
            _stringBuilder.AppendLine($"===============================");
            int i = 0;
            foreach (Tank tank in _players)
            {
                _stringBuilder.AppendLine($"[pType: {tank.PlayerType}] [rect: {tank.GetCenter()}] [dir: {tank.CurrentDirection}]");
                _stringBuilder.AppendLine($"==========output data==========");
                _stringBuilder.AppendLine(PlayersDataFromServer[i].ToString());
                _stringBuilder.AppendLine($"===============================");
                i++;
            }
            return _stringBuilder.ToString();
        }

        private void BoundsDisplayDraw(RectangleF rect)
        {
            if (_boundsDisplayEnable)
            {
                _renderTarget.Transform = Matrix3x2.Rotation(0);
                _renderTarget.DrawRectangle(rect, _boundsColor);
                _renderTarget.DrawLine(rect.TopLeft, rect.TopRight + new Vector2(0, rect.Height), _boundsColor);
                _renderTarget.DrawLine(rect.TopLeft + new Vector2(0, rect.Height), rect.TopRight, _boundsColor);
            }
        }
        public void Run()
        {
            //_renderForm.Resize += RenderForm_Resize;
            RenderLoop.Run(_renderForm, Render);
        }

        public void Dispose()
        {
            _inputController.Dispose();
            _dx2d.Dispose();
            _renderForm.Dispose();
        }
    }
}