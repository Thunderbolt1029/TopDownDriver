using Microsoft.Xna.Framework;
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

        public void CenteredDraw(SpriteBatch spriteBatch, string text, Color color, Vector2 centre, float scale)
        {
            spriteBatch.DrawString(spriteFont, text, centre - spriteFont.MeasureString(text) * scale / 2, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        public void LeftCenteredDraw(SpriteBatch spriteBatch, string text, Color color, Vector2 leftCentre, float scale)
        {
            spriteBatch.DrawString(spriteFont, text, leftCentre - new Vector2(0, spriteFont.MeasureString(text).Y / 2) * scale, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }
    }
}
