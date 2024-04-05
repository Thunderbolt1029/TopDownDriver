﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopDownDriver
{
    internal static class Fonts
    {
        public static Font PropertyEditorUI;
    }

    internal class Font
    {
        SpriteFont spriteFont;

        public Font(SpriteFont spriteFont)
        {
            this.spriteFont = spriteFont;
        }

        public void Draw(SpriteBatch spriteBatch, string text, Color color, Vector2 centre, float scale)
        {
            spriteBatch.DrawString(spriteFont, text, centre - spriteFont.MeasureString(text) * scale / 2, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }
    }
}
