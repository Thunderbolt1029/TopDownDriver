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
        public const float SteeringStrength = 4f;
        public const float SteeringVelocityLinkage = 1.005f;
        public const float PowerStrength = 2500f;
        public const float ReversePowerStrength = 0.2f;
        public const float BrakeStrength = 0.4f;
        public const float FrictionStrength = 0.04f;
        public const float TractionStrength = 0.3f;
        public const float BoundaryPushbackStrength = 6f;
        public const float BounceStrength = 0.4f;
        public const float VelocityLowerBound = 7f;

        Texture2D ColorTexture;
        readonly Color[] PlayerIndexColors = new Color[] { Color.Red, Color.CornflowerBlue, Color.LightGreen, Color.MediumPurple };
        public static Texture2D[] PlayerIndexTextures;
        Color Color => PlayerIndexColors[(int)Index];
        Texture2D Texture => PlayerIndexTextures[(int)Index];
        
        Point Size = new Point(20, 10);

        public Vector2 Centre { get; private set; }
        float Rotation;
        Vector2 Velocity;
        public readonly PlayerIndex Index;

        bool MovingForwards => Vector2.Dot(AngleToVector(Rotation), Velocity) > 0;
        bool Reversing = false;
        Hitbox Hitbox => new Hitbox(new Rectangle(Vector2.Round(new Vector2(Centre.X - Size.X / 2, Centre.Y - Size.Y / 2)).ToPoint(), Size), Rotation);

        public Player(GraphicsDevice graphicsDevice, Vector2 position, float rotation, PlayerIndex index)
        {
            Centre = position;
            Rotation = rotation;
            Index = index;

            ColorTexture = new Texture2D(graphicsDevice, 1, 1);
            ColorTexture.SetData(new[] { Color.White });
        }

        public void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // IO handling
            float power, brake = 0, steering;
            if (!Globals.UsingController && Index == PlayerIndex.One)
            { // Keyboard controls
                KeyboardState keyboardState = Keyboard.GetState();
                
                // Drive
                power = keyboardState.IsKeyDown(Keys.Up) ? 1 : 0;
                if (Reversing)
                    power += keyboardState.IsKeyDown(Keys.Down) ? -ReversePowerStrength : 0;
                else
                    brake = keyboardState.IsKeyDown(Keys.Down) ? 1 : 0;

                if (keyboardState.IsKeyDown(Keys.Down) && Velocity.Length() == 0)
                    Reversing = true;
                else if (MovingForwards)
                    Reversing = false;

                // Steering
                steering = keyboardState.IsKeyDown(Keys.Right) ? 1 : 0;
                steering += keyboardState.IsKeyDown(Keys.Left) ? -1 : 0;
                steering *= (float)(1 - Math.Pow(SteeringVelocityLinkage, -Velocity.Length()));
            }
            else
            { // Controller controls
                GamePadState gamePadState = GamePad.GetState(Index - (!Globals.UsingController ? -1 : 0));

                // Drive
                power = gamePadState.Triggers.Right;
                if (Reversing)
                    power += gamePadState.Triggers.Left * -ReversePowerStrength;
                else
                    brake = gamePadState.Triggers.Left;

                if (gamePadState.Triggers.Left > 0 && Velocity.Length() == 0)
                    Reversing = true;
                else if (MovingForwards)
                    Reversing = false;

                // Steering
                steering = gamePadState.ThumbSticks.Left.X * (float)(1 - Math.Pow(SteeringVelocityLinkage, -Velocity.Length()));
            }

            // Apply drive force
            Rotation += steering * SteeringStrength * delta;
            Velocity += AngleToVector(Rotation) * power * PowerStrength * delta;
            Velocity -= Velocity.DirectionVector() * brake * PowerStrength * BrakeStrength * delta;


            // Resistive forces
            Velocity *= 1 - FrictionStrength;
            Velocity = Vector2.Lerp(Velocity.DirectionVector(), AngleToVector(Rotation) * (MovingForwards ? 1 : -1), TractionStrength * delta) * Velocity.Length();
            
            if (Velocity.Length() < VelocityLowerBound)
                Velocity = Vector2.Zero;


            // Collision
            Vector2 CollisionForce = Vector2.Zero;
            foreach (Hitbox boundary in Globals.Bounds)
                if (boundary.Intersects(Hitbox, out List<Vector2> intersectionPoints))
                {
                    Vector2 Centre = new Vector2(intersectionPoints.Average(x => x.X), intersectionPoints.Average(x => x.Y));
                    Vector2 boundaryNormal = boundary.FindNormal(Centre);

                    if (Vector2.Dot(Velocity, boundaryNormal) < 0)
                    {
                        Velocity -= Vector2.Dot(Velocity, boundaryNormal) * boundaryNormal * (1 + BounceStrength);
                        Velocity += boundaryNormal * BoundaryPushbackStrength;

                        // float fullReflectionAngle = 2 * VectorToAngle(boundaryNormal) - Rotation + MathHelper.Pi;
                        // rotation = fullReflectionAngle;

                        List<Vector2> playerNormals = new List<Vector2>();
                        foreach (Vector2 point in intersectionPoints)
                            playerNormals.Add(Hitbox.FindNormal(point));

                        //float[] boundary
                    }
                }


            // Apply acceleration
            Centre += Velocity * delta;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle DisplayRectangle = new Rectangle(Vector2.Round(new Vector2(Centre.X, Centre.Y)).ToPoint(), Size);
            spriteBatch.Draw(Texture, DisplayRectangle, null, Color, Rotation, new Vector2(Texture.Width, Texture.Height) / 2, SpriteEffects.None, 0);
        }

        static Vector2 AngleToVector(float theta) => new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta));
        static float VectorToAngle(Vector2 vector) => (float)Math.Atan2(vector.Y, vector.X);
    }
}
