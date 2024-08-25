using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Threading;
using System.Threading.Channels;

namespace WindowSystem.WindowSystem
{
    internal class Window
    {
        static int Count = 0;
        public static Texture2D ColorTexture;
        public static SpriteFont spriteFont;
        
        Point HeaderSize = new Point(200, 15);
        Point WindowSize = new Point(200, 300);

        List<Section> Sections = new List<Section>();
        string Title;

        public int Depth;
        bool Highlighted;
        bool Hidden;
        public bool Closed = false;

        Vector2 position = new Vector2(10);
        bool BeingDragged = false;

        bool PreviousFrameHeld, ThisFrameHeld = false;
        Point PreviousMousePos, ThisMousePos = Point.Zero;

        Rectangle HeaderRectangle => new Rectangle(position.ToPoint(), HeaderSize);
        public Rectangle WindowRectangle => new Rectangle(position.ToPoint(), WindowSize);
        Rectangle CloseButtonRectangle => new Rectangle(HeaderRectangle.Right - HeaderSize.Y, HeaderRectangle.Top, HeaderSize.Y, HeaderSize.Y);


        public Window(string Title)
        {
            this.Title = Title;

            Count++;
            Depth = Count;
        }

        public void Update()
        {
            MouseState mouseState = Mouse.GetState();

            PreviousFrameHeld = ThisFrameHeld;
            ThisFrameHeld = mouseState.LeftButton == ButtonState.Pressed;

            PreviousMousePos = ThisMousePos;
            ThisMousePos = mouseState.Position;

            if (!PreviousFrameHeld && ThisFrameHeld)
                if (Depth == 0 && WindowRectangle.Contains(ThisMousePos))
                    Highlighted = true;
                else
                    Highlighted = false;

            if (Highlighted)
            {
                if (PreviousFrameHeld && !ThisFrameHeld && CloseButtonRectangle.Contains(ThisMousePos))
                    Closed = true;

                if (!PreviousFrameHeld && ThisFrameHeld && HeaderRectangle.Contains(ThisMousePos) && !CloseButtonRectangle.Contains(ThisMousePos))
                    BeingDragged = true;

                if (BeingDragged && !ThisFrameHeld)
                    BeingDragged = false;



                if (BeingDragged)
                    position += ThisMousePos.ToVector2() - PreviousMousePos.ToVector2();

            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(ColorTexture, WindowRectangle, new Color(0xFF0F0F0F));

            spriteBatch.Draw(ColorTexture, new Rectangle(WindowRectangle.Left + 1, WindowRectangle.Top + 1, 1, WindowRectangle.Height - 2), Color.Gray);
            spriteBatch.Draw(ColorTexture, new Rectangle(WindowRectangle.Left + 1, WindowRectangle.Top + 1, WindowRectangle.Width - 2, 1), Color.Gray);
            spriteBatch.Draw(ColorTexture, new Rectangle(WindowRectangle.Left + 1, WindowRectangle.Bottom - 2, WindowRectangle.Width - 2, 1), Color.Gray);
            spriteBatch.Draw(ColorTexture, new Rectangle(WindowRectangle.Right - 2, WindowRectangle.Top + 1, 1, WindowRectangle.Height - 2), Color.Gray);


            spriteBatch.Draw(ColorTexture, HeaderRectangle, Highlighted ? new Color(0xFF0A0830) : Color.Black);
            spriteBatch.DrawString(spriteFont, Title, position + new Vector2(2, 0), Color.LightGray);

            spriteBatch.Draw(ColorTexture, CloseButtonRectangle, PreviousFrameHeld && CloseButtonRectangle.Contains(ThisMousePos) ? Color.Red : Color.MidnightBlue);
            spriteBatch.DrawString(spriteFont, "X", CloseButtonRectangle.Center.ToVector2() - new Vector2(5, 7), Color.LightGray);
        }
    }
}
