using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopDownDriver
{
    internal class Player
    {
        public const float SteeringStrength = 6f;
        public const float PowerStrength = 1200f;
        public const float FrictionStrength = 0.05f;
        public const float TractionStrength = 1.5f;
        public const float BounceStrength = 0.8f;

        Texture2D ColorTexture;
        Point Size = new Point(20, 10);

        Vector2 centre;
        float rotation;
        Vector2 Velocity;

        Hitbox hitbox => new Hitbox(new Rectangle(Vector2.Round(new Vector2(centre.X - Size.X / 2, centre.Y - Size.Y / 2)).ToPoint(), Size), rotation);

        public Player(GraphicsDevice graphicsDevice, Vector2 position, float rotation)
        {
            this.centre = position;
            this.rotation = rotation;

            ColorTexture = new Texture2D(graphicsDevice, 1, 1);
            ColorTexture.SetData(new[] { Color.White });
        }

        public void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            KeyboardState keyboardState = Keyboard.GetState();

            // Drive
            float power = 0f;
            power += keyboardState.IsKeyDown(Keys.Up) ? 1 : 0;
            power += keyboardState.IsKeyDown(Keys.Down) ? -1 : 0;

            float drive = power * PowerStrength;


            // Steering
            float steering = 0f;
            steering += keyboardState.IsKeyDown(Keys.Right) ? 1 : 0;
            steering += keyboardState.IsKeyDown(Keys.Left) ? -1 : 0;

            rotation += steering * SteeringStrength * delta;


            // Resistive forces
            Velocity *= 1 - FrictionStrength;
            Velocity = Vector2.Lerp(Velocity.Length() > 0 ? Vector2.Normalize(Velocity) : Vector2.Zero, AngleToVector2(rotation), TractionStrength * delta) * Velocity.Length();


            // Apply acceleration
            Velocity += AngleToVector2(rotation) * drive * delta;
            centre += Velocity * delta;


            // Collision
            foreach (Hitbox boundary in Globals.Bounds)
                if (boundary.Intersects(hitbox))
                {
                    Velocity *= -1;
                }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle DisplayRectangle = new Rectangle(Vector2.Round(new Vector2(centre.X, centre.Y)).ToPoint(), Size);
            spriteBatch.Draw(ColorTexture, DisplayRectangle, null, Color.Red, rotation, new Vector2(0.5f), SpriteEffects.None, 0);
        }

        Vector2 AngleToVector2(float theta) => new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta));
    }
}
