using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace WindowSystem.WindowSystem
{
    internal class TextLabel : SectionComponent
    {
        static SpriteFont spriteFont;

        string text;

        public TextLabel(string text) 
        {
            this.text = text;
        }

        public override void Update()
        {
            // pass
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(spriteFont, text, new Vector2(), Color.White);
        }
    }
}
