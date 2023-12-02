using SharpDX;

namespace TankDuel
{
    public class GameStartScreen
    {
        private RectangleF _screenRect;
        public RectangleF ScreenRect { get => _screenRect; }
        private RectangleF _menuButtonRect;
        public RectangleF MenuButtonRect { get => _menuButtonRect; }

        public GameStartScreen(Size2F ScreenSize, Size2F buttonSize)
        {
            _screenRect = new RectangleF(0, 0, ScreenSize.Width, ScreenSize.Height);
            float Xoffset = ScreenSize.Width / 2 - buttonSize.Width / 2;
            float Yoffset = ScreenSize.Height / 2 - buttonSize.Height / 2;
            _menuButtonRect = new RectangleF(Xoffset, Yoffset, 
                buttonSize.Width, buttonSize.Height);
        }
    }
}
