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
        public const float SteeringStrength = 65f;
        public const float SteeringVelocityLinkage = 1.005f;
        public const float PowerStrength = 2500f;
        public const float ReversePowerStrength = 0.2f;
        public const float BrakeStrength = 0.4f;
        public const float LinearFrictionStrength = 0.04f;
        public const float RotationalFrictionStrength = 0.14f;
        public const float TractionStrength = 0.05f;
        public const float BoundaryPushbackStrength = 6f;
        public const float LinearBounceStrength = 0.4f;
        public const float LinearVelocityLowerBound = 7f;
        public const float AngularVelocityLowerBound = 0.05f;
        public const float GrappleRotationSnapSpeed = 10f;

        Texture2D ColorTexture;
        readonly Color[] PlayerIndexColors = new Color[] { Color.Red, Color.CornflowerBlue, Color.LightGreen, Color.MediumPurple };
        public static Texture2D[] PlayerIndexTextures;
        Color Color => PlayerIndexColors[(int)Index];
        Texture2D Texture => PlayerIndexTextures[(int)Index];

        Vector2 Size = new Vector2(20, 10);


        public Vector2 Centre { get; private set; }
        float Rotation, AngularVelocity = 0f;
        Vector2 LinearVelocity;


        public readonly PlayerIndex Index;
        readonly bool UsingController;


        bool MovingForwards => Vector2.Dot(AngleToVector(Rotation), LinearVelocity) > 0;
        bool Reversing = false;

        Vector2 GrapplePoint;
        bool Grappling, PrevGrapple, ClockwiseGrapple;
        Vector2 GrappleMoveDirection
        {
            get
            {
                Vector2 vector = Vector3.Cross(new Vector3(GrapplePoint - Centre, 0), Vector3.UnitZ).IgnoreZ();
                vector.Normalize();
                if (ClockwiseGrapple)
                    return -vector;
                return vector;
            }
        }
        Hitbox Hitbox => new Hitbox(Centre, Size, Rotation);

        public Player(GraphicsDevice graphicsDevice, Vector2 position, float rotation, PlayerIndex index, bool usingController)
        {
            Centre = position;
            Rotation = rotation;
            Index = index;
            UsingController = usingController;

            ColorTexture = new Texture2D(graphicsDevice, 1, 1);
            ColorTexture.SetData(new[] { Color.White });
        }

        public void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            // IO handling
            float power, brake = 0, steering;
            if (UsingController || (int)Index > 0)
            { 
                // Controller controls
                GamePadState gamePadState = GamePad.GetState(Index - (UsingController ? 0 : 1));

                Grappling = gamePadState.IsButtonDown(Buttons.A);

                // Drive
                power = gamePadState.Triggers.Right;
                if (Reversing)
                    power += gamePadState.Triggers.Left * -ReversePowerStrength;
                else
                    brake = gamePadState.Triggers.Left;

                if (gamePadState.Triggers.Left > 0 && LinearVelocity.Length() == 0)
                    Reversing = true;
                else if (MovingForwards)
                    Reversing = false;

                // Steering
                steering = gamePadState.ThumbSticks.Left.X * (float)(1 - Math.Pow(SteeringVelocityLinkage, -LinearVelocity.Length()));
            }
            else
            { 
                // Keyboard controls
                KeyboardState keyboardState = Keyboard.GetState();

                Grappling = keyboardState.IsKeyDown(Keys.Space);

                // Drive
                power = keyboardState.IsKeyDown(Keys.Up) ? 1 : 0;
                if (Reversing)
                    power += keyboardState.IsKeyDown(Keys.Down) ? -ReversePowerStrength : 0;
                else
                    brake = keyboardState.IsKeyDown(Keys.Down) ? 1 : 0;

                if (keyboardState.IsKeyDown(Keys.Down) && LinearVelocity.Length() == 0)
                    Reversing = true;
                else if (MovingForwards)
                    Reversing = false;

                // Steering
                steering = keyboardState.IsKeyDown(Keys.Right) ? 1 : 0;
                steering += keyboardState.IsKeyDown(Keys.Left) ? -1 : 0;
                steering *= (float)(1 - Math.Pow(SteeringVelocityLinkage, -LinearVelocity.Length()));
            }

            // Set grapple point
            if (Grappling && !PrevGrapple)
            {
                GrapplePoint = Globals.GrapplePoints.MinBy(x => (Centre - x).LengthSquared());
                Vector2 vector = Vector3.Cross(new Vector3(GrapplePoint - Centre, 0), Vector3.UnitZ).IgnoreZ();
                vector.Normalize();
                ClockwiseGrapple = Vector2.Dot(vector, LinearVelocity) < 0;
            }


            // Apply drive force
            LinearVelocity += AngleToVector(Rotation) * power * PowerStrength * delta;
            LinearVelocity -= LinearVelocity.DirectionVector() * brake * PowerStrength * BrakeStrength * delta;


            if (Grappling)
            {
                // Apply grapple turn forces
                float IdealRotation = MathHelper.WrapAngle(VectorToAngle(GrappleMoveDirection));

                if (IdealRotation > Rotation + MathHelper.Pi)
                    IdealRotation -= MathHelper.TwoPi;
                else if (IdealRotation < Rotation - MathHelper.Pi)
                    IdealRotation += MathHelper.TwoPi;

                AngularVelocity = Math.Clamp(MathHelper.Lerp(Rotation, IdealRotation, GrappleRotationSnapSpeed) - Rotation, -SteeringStrength, SteeringStrength) * delta;
                Rotation += Math.Clamp(MathHelper.Lerp(Rotation, IdealRotation, GrappleRotationSnapSpeed) - Rotation, -SteeringStrength, SteeringStrength) * delta;
            }
            else
            {
                // Apply turn force
                AngularVelocity += steering * SteeringStrength * delta;
            }

            if (Grappling)
            {
                // Apply grapple turn forces
                float IdealRotation = MathHelper.WrapAngle(VectorToAngle(GrappleMoveDirection));

                if (IdealRotation > Rotation + MathHelper.Pi)
                    IdealRotation -= MathHelper.TwoPi;
                else if (IdealRotation < Rotation - MathHelper.Pi)
                    IdealRotation += MathHelper.TwoPi;

                AngularVelocity = Math.Clamp(MathHelper.Lerp(Rotation, IdealRotation, GrappleRotationSnapSpeed) - Rotation, -SteeringStrength, SteeringStrength) * delta;
                Rotation += Math.Clamp(MathHelper.Lerp(Rotation, IdealRotation, GrappleRotationSnapSpeed) - Rotation, -SteeringStrength, SteeringStrength) * delta;
            }
            else
            {
                // Apply turn force
                AngularVelocity += steering * SteeringStrength * delta;
            }

            // Resistive forces
            AngularVelocity *= 1 - RotationalFrictionStrength;
            LinearVelocity *= 1 - LinearFrictionStrength;
            if (Grappling)
            {
                Vector2 friction = Vector2.Normalize(GrapplePoint - Centre) * LinearVelocity.LengthSquared() / (GrapplePoint - Centre).Length();
                while (friction.Length() > 4000f)
                    friction *= 0.99f;
                LinearVelocity +=  friction * delta;
            }
            else
                LinearVelocity = Vector2.Lerp(LinearVelocity.DirectionVector(), AngleToVector(Rotation) * (MovingForwards ? 1 : -1), TractionStrength) * LinearVelocity.Length();

            if (LinearVelocity.Length() < LinearVelocityLowerBound)
                LinearVelocity = Vector2.Zero;
            if (Math.Abs(AngularVelocity) < AngularVelocityLowerBound)
                AngularVelocity = 0;


            // Collision
            Vector2 CollisionForce = Vector2.Zero;
            foreach (Hitbox boundary in Globals.Bounds)
                if (boundary.Intersects(Hitbox, out List<Vector2> intersectionPoints))
                {
                    Vector2 intersectionCentre = new Vector2(intersectionPoints.Average(x => x.X), intersectionPoints.Average(x => x.Y));
                    Vector2 boundaryNormal = boundary.FindNormal(intersectionCentre);

                    if (Vector2.Dot(LinearVelocity, boundaryNormal) < 0)
                    {
                        if (Grappling)
                            LinearVelocity *= -LinearBounceStrength;
                        else
                        {
                            LinearVelocity -= Vector2.Dot(LinearVelocity, boundaryNormal) * boundaryNormal * (1 + LinearBounceStrength);
                            LinearVelocity += boundaryNormal * BoundaryPushbackStrength;
                        }
                    }
                }


            // Apply velocity
            Centre += LinearVelocity * delta;
            Rotation += AngularVelocity * delta;
            Rotation = MathHelper.WrapAngle(Rotation);

            PrevGrapple = Grappling;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Hitbox.DisplayRectangle, null, Color, Rotation, new Vector2(Texture.Width, Texture.Height) / 2, SpriteEffects.None, 0);

            if (Grappling)
            {
                Line line = new Line(Centre, GrapplePoint);
                line.Draw(spriteBatch, Color.Goldenrod);
            }
        }

        static Vector2 AngleToVector(float theta) => new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta));
        static float VectorToAngle(Vector2 vector) => (float)Math.Atan2(vector.Y, vector.X);
    }
}
