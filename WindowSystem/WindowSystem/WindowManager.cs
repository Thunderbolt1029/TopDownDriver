using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowSystem.WindowSystem
{
    internal class WindowManager
    {
        SpriteBatch spriteBatch;

        List<Window> Windows = new List<Window>();

        bool PreviousFrameHeld, ThisFrameHeld = false;
        Point PreviousMousePos, ThisMousePos = Point.Zero;

        public WindowManager(SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            this.spriteBatch = spriteBatch;

            Window.ColorTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            Window.ColorTexture.SetData(new[] { Color.White });

            Window.spriteFont = spriteFont;
            
            Windows.Add(new Window("Window one"));
            Windows.Add(new Window("Window two"));
        }

        public void Update()
        {
            MouseState mouseState = Mouse.GetState();

            PreviousFrameHeld = ThisFrameHeld;
            ThisFrameHeld = mouseState.LeftButton == ButtonState.Pressed;

            PreviousMousePos = ThisMousePos;
            ThisMousePos = mouseState.Position;

            if (!PreviousFrameHeld && ThisFrameHeld)
            {
                Windows.ForEach(x => x.Depth++);

                foreach (Window window in Windows.OrderBy(x => x.Depth))
                {
                    if (window.WindowRectangle.Contains(ThisMousePos))
                    {
                        window.Depth = 0;
                        break;
                    }
                }
            }

            Windows.ForEach(x => x.Update());

            Windows.RemoveAll(x => x.Closed);
        }

        public void Draw()
        {
            Windows.OrderBy(x => -x.Depth).ToList().ForEach(x => x.Draw(spriteBatch));
        }
    }
}
