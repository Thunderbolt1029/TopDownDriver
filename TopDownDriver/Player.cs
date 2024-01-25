using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopDownDriver
{
    internal class Player
    {
        public const float SteeringStrength = 5f;
        public const float PowerStrength = 2500f;
        public const float ReversePowerStrength = 0.2f;
        public const float BrakeStrength = 0.4f;
        public const float FrictionStrength = 0.05f;
        public const float TractionStrength = 1.7f;
        public const float BoundaryPushbackStrength = 10f;
        public const float BounceStrength = 0.2f;
        public const float VelocityLowerBound = 0.1f;

        Texture2D ColorTexture;
        Point Size = new Point(20, 10);

        Vector2 centre;
        float rotation;
        Vector2 Velocity;
        public readonly PlayerIndex index;

        bool MovingForwards => Vector2.Dot(AngleToVector2(rotation), Velocity) > 0;
        Hitbox hitbox => new Hitbox(new Rectangle(Vector2.Round(new Vector2(centre.X - Size.X / 2, centre.Y - Size.Y / 2)).ToPoint(), Size), rotation);

        public Player(GraphicsDevice graphicsDevice, Vector2 position, float rotation, PlayerIndex index)
        {
            this.centre = position;
            this.rotation = rotation;
            this.index = index;

            ColorTexture = new Texture2D(graphicsDevice, 1, 1);
            ColorTexture.SetData(new[] { Color.White });
        }

        public void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            KeyboardState keyboardState = Keyboard.GetState();
            GamePadState gamePadState = GamePad.GetState(index);

            // IO handling
            float power = 0f;
            float steering = 0f;
            if (Globals.UsingController)
            {
                // Drive
                power = gamePadState.Triggers.Right;
                if (MovingForwards)
                    power += gamePadState.Triggers.Left * -BrakeStrength;
                else
                    power += gamePadState.Triggers.Left * -ReversePowerStrength;

                // Steering
                steering = gamePadState.ThumbSticks.Left.X;
            }
            else
            {
                // Drive
                power += keyboardState.IsKeyDown(Keys.Up) ? 1 : 0;
                if (MovingForwards)
                    power += keyboardState.IsKeyDown(Keys.Down) ? -BrakeStrength : 0;
                else
                    power += keyboardState.IsKeyDown(Keys.Down) ? -ReversePowerStrength : 0;

                // Steering
                steering += keyboardState.IsKeyDown(Keys.Right) ? 1 : 0;
                steering += keyboardState.IsKeyDown(Keys.Left) ? -1 : 0;
            }


            // Apply drive force
            float drive = power * PowerStrength;
            rotation += steering * SteeringStrength * delta;
            Velocity += AngleToVector2(rotation) * drive * delta;


            // Resistive forces
            Velocity *= 1 - FrictionStrength;
            Velocity = Vector2.Lerp(Velocity.Length() > 0 ? Vector2.Normalize(Velocity) : Vector2.Zero, AngleToVector2(rotation) * (MovingForwards ? 1 : -1), TractionStrength * delta) * Velocity.Length();
            if (Velocity != Vector2.Zero && Velocity.Length() < VelocityLowerBound) 
                Velocity = Vector2.Zero;


            // Collision
            Vector2 CollisionForce = Vector2.Zero;
            foreach (Hitbox boundary in Globals.Bounds)
                if (boundary.Intersects(hitbox, out List<Vector2> intersectionPoints))
                {
                    Vector2 Centre = new Vector2(intersectionPoints.Average(x => x.X), intersectionPoints.Average(x => x.Y));
                    Vector2 normal = boundary.FindNormal(Centre);

                    if (Vector2.Dot(Velocity, normal) < 0)
                    {
                        Velocity -= Vector2.Dot(Velocity, normal) * normal;
                        Velocity += normal * BoundaryPushbackStrength;
                    }
                }


            // Apply acceleration
            centre += Velocity * delta;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle DisplayRectangle = new Rectangle(Vector2.Round(new Vector2(centre.X, centre.Y)).ToPoint(), Size);
            spriteBatch.Draw(ColorTexture, DisplayRectangle, null, Color.Red, rotation, new Vector2(0.5f), SpriteEffects.None, 0);
        }

        Vector2 AngleToVector2(float theta) => new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta));
    }
}
