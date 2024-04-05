using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace TopDownDriver
{
    internal class Button
    {
        Action action;

        Rectangle Rectangle;
        Color BackgroundColor;

        string Text;
        Color TextColor;
        Font font;
        float TextScale;

        MouseState CurrentMouseState;
        bool PreviousFrameMouseHeld = false, CurrentFrameMouseHeld = false;

        public Button(Action action, Rectangle Rectangle, Color BackgroundColor, string Text, Color TextColor, Font font, float TextScale)
        {
            this.action = action;

            this.Rectangle = Rectangle;
            this.BackgroundColor = BackgroundColor;
            
            this.Text = Text;
            this.TextColor = TextColor;
            this.font = font;
            this.TextScale = TextScale;
        }

        void Update(GameTime gameTime)
        {
            CurrentMouseState = Mouse.GetState();
            PreviousFrameMouseHeld = CurrentFrameMouseHeld;
            CurrentFrameMouseHeld = CurrentMouseState.LeftButton == ButtonState.Pressed;

            if (PreviousFrameMouseHeld && !CurrentFrameMouseHeld && Rectangle.Contains(CurrentMouseState.Position))
                action();
        }

        void Draw(SpriteBatch spriteBatch)
        {
            Texture2D ColorTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            ColorTexture.SetData(new[] { Color.White });

            if (CurrentFrameMouseHeld && Rectangle.Contains(CurrentMouseState.Position))
            {
                spriteBatch.Draw(ColorTexture, Rectangle, new Color(BackgroundColor.ToVector3() * 0.7f));
                font.Draw(spriteBatch, Text, new Color(TextColor.ToVector3() * 0.7f), Rectangle.Center.ToVector2(), TextScale);
            }
            else
            {
                spriteBatch.Draw(ColorTexture, Rectangle, BackgroundColor);
                font.Draw(spriteBatch, Text, TextColor, Rectangle.Center.ToVector2(), TextScale);
            }
        }
    }
}
