using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowSystem.WindowSystem
{
    internal abstract class SectionComponent
    {
        public abstract void Update();
        public abstract void Draw(SpriteBatch spriteBatch);
    }
}
