using System;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Windows;
using SharpDX.DirectInput;
using SharpDX.Mathematics.Interop;
using DX;
using System.Diagnostics;

namespace TankDuel
{
    class Game : IDisposable
    {
        /*private RenderForm _renderForm;
        public RenderForm RenderForm { get => _renderForm; }

        private DX2D _dx2d;
        private SceneController _sceneController;
        public DX2D DX2D { get => _dx2d; }
        private InputController _inputController;*/

        //private RectangleF _clientRect;

        /*private Client _client;

        private Vector2 MousePosition = new Vector2();

        private bool _renderFlag = true;*/
        public Game()
        {
            /*_renderForm = new RenderForm("TankDuel");
            _dx2d = new DX2D(_renderForm);
            _inputController = new InputController(_renderForm);
            _sceneController = new SceneController(_dx2d, _inputController);

            _client = new Client("192.168.31.198", 5555);

            //RenderForm_Resize(this, null);

            _renderForm.MouseMove += (sender, args) => {
                MousePosition.X = args.X;
                MousePosition.Y = args.Y;
            };*/
        }
        /*private void Initialization()
        {
            _sceneController.SceneClientInitialization(
                _client.InitOnServer(),
                _client.GetClientObject(),
                _client.InitializeObjects(),
                _client.InitializeSettings()
                );
        }*/
        private void RenderCallback()
        {
            /*WindowRenderTarget target = _dx2d.RenderTarget;
            Size2F targetSize = target.Size;
            _clientRect.Width = targetSize.Width;
            _clientRect.Height = targetSize.Height;

            target.BeginDraw();
            target.Clear(SharpDX.Color.DarkGray);
            if(_renderFlag)
            {
                _sceneController.DrawMenuUserInterface(MousePosition, ref _renderFlag);
            }
            else
            {
                _sceneController.Update();
                _sceneController.PlayersDataFromServer = _client.ClientProcessing(_sceneController.PlayerDataInput);
                _sceneController.UpdateGameMap(_client.UpdateMap());
            }
            target.EndDraw();*/
        }

        private void RenderForm_Resize(object sender, EventArgs e)
        {
            /*int width = _renderForm.ClientSize.Width;
            int height = _renderForm.ClientSize.Height;
            _dx2d.RenderTarget.Resize(new Size2(width, height));
            //_clientRect.Width = _dx2d.RenderTarget.Size.Width;
            //_clientRect.Height = _dx2d.RenderTarget.Size.Height;
            _clientRect.Width = _dx2d.RenderTarget.Size.Width;
            _clientRect.Height = _dx2d.RenderTarget.Size.Height;
            //_scale = _clientRect.Height / _unitsPerHeight;*/
        }

        public void Run()
        {
            /*_renderForm.Resize += RenderForm_Resize;
            RenderLoop.Run(_renderForm, RenderCallback);*/
        }

        public void Dispose()
        {
            /*_inputController.Dispose();
            _dx2d.Dispose();
            _renderForm.Dispose();*/
        }
    }
}