using DataStructuresManipulation;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankDuel
{
    public class PlayerStatsUI
    {
        private RectangleF _rect;
        public RectangleF Rect { get => _rect; }

        private int _iconsCount;

        private List<RectangleF> _iconsRect;

        /// <summary>
        /// rect1 = icon, rect2 = text
        /// </summary>
        public List<RectangleF> IconsRect { get => _iconsRect; }

        private float _iconSize;
        Vector2 _iconPosition;
        private float _offset;
        public PlayerStatsUI(Vector2 pos, float width, float height, float fontSize, SettingsData settings)
        {
            _rect.Location = pos;
            _rect.Size = new Size2F(width, height);
            _iconSize = settings.BonusSize - settings.QuarterOffset / 2;
            _iconPosition = new Vector2(pos.X, pos.Y + settings.BlockSize - fontSize);
            _offset = settings.QuarterOffset * 2;

            Debug.Print($"{width}, {_iconSize}, {_offset}");
            _iconsCount = (int)(width / _offset);
            _iconsRect = new List<RectangleF>();
        }

        public void CreateIconsPosition()
        {
            for (int i = 0; i < _iconsCount; ++i)
            {
                RectangleF iconTextureRect = new RectangleF(_iconPosition.X, _iconPosition.Y, _iconSize, _iconSize);
                //RectangleF iconTextRect = new RectangleF(_iconPosition.X + _iconSize, _iconPosition.Y, _iconSize * 2, _iconSize);
                _iconsRect.Add(iconTextureRect);
                _iconPosition.X += _offset;
            }
        }
    }
}
