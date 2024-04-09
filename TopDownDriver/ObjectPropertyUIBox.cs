using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// TODO: Finer movement control of objects - drag?
// TODO: IO to JSON of level
// TODO: Create and delete objects

namespace TopDownDriver
{
    internal class ObjectPropertyUIBox
    {
        ObjectType objectType;

        Hitbox Boundary;
        Vector2 GrapplePointLocation;
        SpawnPoint SpawnPoint;

        public Rectangle Background { get; private set; } = new Rectangle(10, 10, 400, 90);
        List<Button> buttons = new List<Button>();

        bool Shift, Ctrl;
        int ModeScale => (Shift ? 10 : 1) * (Ctrl ? 100 : 1);

        public ObjectPropertyUIBox(ObjectType objectType, int index = -1)
        {
            this.objectType = objectType;
            
            if (objectType == ObjectType.Boundary)
            {
                Boundary = Globals.CurrentEditingLevel.Bounds[index].Item1;

                buttons.AddRange(new[]
                {
                    new Button(() => { Boundary.Centre -= Vector2.UnitX * ModeScale; UpdateVariables(objectType, index); }, new Rectangle(15, 15, 20, 20), Color.Black, "X-", Color.White, Fonts.PropertyEditorUI, 1f), 
                    new Button(() => { Boundary.Centre += Vector2.UnitX * ModeScale; UpdateVariables(objectType, index); }, new Rectangle(40, 15, 20, 20), Color.Black, "X+", Color.White, Fonts.PropertyEditorUI, 1f),
                    new Button(() => { Boundary.Centre -= Vector2.UnitY * ModeScale; UpdateVariables(objectType, index); }, new Rectangle(70, 15, 20, 20), Color.Black, "Y-", Color.White, Fonts.PropertyEditorUI, 1f),
                    new Button(() => { Boundary.Centre += Vector2.UnitY * ModeScale; UpdateVariables(objectType, index); }, new Rectangle(95, 15, 20, 20), Color.Black, "Y+", Color.White, Fonts.PropertyEditorUI, 1f),
                    
                    new Button(() => { Boundary.Rotation -= MathHelper.PiOver4  * ModeScale / 9f; UpdateVariables(objectType, index); }, new Rectangle(15, 45, 45, 20), Color.Black, "Anti", Color.White, Fonts.PropertyEditorUI, 1f),
                    new Button(() => { Boundary.Rotation += MathHelper.PiOver4  * ModeScale / 9f; UpdateVariables(objectType, index); }, new Rectangle(70, 45, 45, 20), Color.Black, "Clock", Color.White, Fonts.PropertyEditorUI, 1f),

                    new Button(() => { Boundary.Size -= Vector2.UnitX * ModeScale; UpdateVariables(objectType, index); }, new Rectangle(15, 75, 20, 20), Color.Black, "X-", Color.White, Fonts.PropertyEditorUI, 1f),
                    new Button(() => { Boundary.Size += Vector2.UnitX * ModeScale; UpdateVariables(objectType, index); }, new Rectangle(40, 75, 20, 20), Color.Black, "X+", Color.White, Fonts.PropertyEditorUI, 1f),
                    new Button(() => { Boundary.Size -= Vector2.UnitY * ModeScale; UpdateVariables(objectType, index); }, new Rectangle(70, 75, 20, 20), Color.Black, "Y-", Color.White, Fonts.PropertyEditorUI, 1f),
                    new Button(() => { Boundary.Size += Vector2.UnitY * ModeScale; UpdateVariables(objectType, index); }, new Rectangle(95, 75, 20, 20), Color.Black, "Y+", Color.White, Fonts.PropertyEditorUI, 1f),
                    
                });
            }
            else if (objectType == ObjectType.GrapplePoint)
            {
                GrapplePointLocation = Globals.CurrentEditingLevel.GrapplePoints[index].Item1;

                buttons.AddRange(new[]
                {
                    new Button(() => { GrapplePointLocation -= Vector2.UnitX * ModeScale; UpdateVariables(objectType, index); }, new Rectangle(15, 15, 20, 20), Color.Black, "X-", Color.White, Fonts.PropertyEditorUI, 1f),
                    new Button(() => { GrapplePointLocation += Vector2.UnitX * ModeScale; UpdateVariables(objectType, index); }, new Rectangle(40, 15, 20, 20), Color.Black, "X+", Color.White, Fonts.PropertyEditorUI, 1f),
                    new Button(() => { GrapplePointLocation -= Vector2.UnitY * ModeScale; UpdateVariables(objectType, index); }, new Rectangle(70, 15, 20, 20), Color.Black, "Y-", Color.White, Fonts.PropertyEditorUI, 1f),
                    new Button(() => { GrapplePointLocation += Vector2.UnitY * ModeScale; UpdateVariables(objectType, index); }, new Rectangle(95, 15, 20, 20), Color.Black, "Y+", Color.White, Fonts.PropertyEditorUI, 1f),
                });
            }
            else if (objectType == ObjectType.SpawnPoint)
            {
                SpawnPoint = Globals.CurrentEditingLevel.SpawnPoint.Item1;

                buttons.AddRange(new[]
                {
                    new Button(() => { SpawnPoint.Centre -= Vector2.UnitX * ModeScale; UpdateVariables(objectType, index); }, new Rectangle(15, 15, 20, 20), Color.Black, "X-", Color.White, Fonts.PropertyEditorUI, 1f),
                    new Button(() => { SpawnPoint.Centre += Vector2.UnitX * ModeScale; UpdateVariables(objectType, index); }, new Rectangle(40, 15, 20, 20), Color.Black, "X+", Color.White, Fonts.PropertyEditorUI, 1f),
                    new Button(() => { SpawnPoint.Centre -= Vector2.UnitY * ModeScale; UpdateVariables(objectType, index); }, new Rectangle(70, 15, 20, 20), Color.Black, "Y-", Color.White, Fonts.PropertyEditorUI, 1f),
                    new Button(() => { SpawnPoint.Centre += Vector2.UnitY * ModeScale; UpdateVariables(objectType, index); }, new Rectangle(95, 15, 20, 20), Color.Black, "Y+", Color.White, Fonts.PropertyEditorUI, 1f),

                    new Button(() => { SpawnPoint.Rotation -= MathHelper.PiOver4 * ModeScale / 9f; UpdateVariables(objectType, index); }, new Rectangle(15, 45, 45, 20), Color.Black, "Anti", Color.White, Fonts.PropertyEditorUI, 1f),
                    new Button(() => { SpawnPoint.Rotation += MathHelper.PiOver4 * ModeScale / 9f; UpdateVariables(objectType, index); }, new Rectangle(70, 45, 45, 20), Color.Black, "Clock", Color.White, Fonts.PropertyEditorUI, 1f),
                });
            }
        }

        void UpdateVariables(ObjectType objectType, int index = -1)
        {
            switch (objectType) 
            {
                case ObjectType.Boundary:
                    Globals.CurrentEditingLevel.Bounds[index] = (Boundary, Globals.CurrentEditingLevel.Bounds[index].Item2);
                    break;

                case ObjectType.GrapplePoint:
                    Globals.CurrentEditingLevel.GrapplePoints[index] = (GrapplePointLocation, Globals.CurrentEditingLevel.GrapplePoints[index].Item2);
                    break;

                case ObjectType.SpawnPoint:
                    Globals.CurrentEditingLevel.SpawnPoint = (SpawnPoint, Globals.CurrentEditingLevel.SpawnPoint.Item2);
                    break;
            }
        }

        public void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            Shift = keyboardState.IsKeyDown(Keys.LeftShift) || keyboardState.IsKeyDown(Keys.RightShift);
            Ctrl = keyboardState.IsKeyDown(Keys.LeftControl) || keyboardState.IsKeyDown(Keys.RightControl);

            foreach (Button button in buttons)
                button.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Texture2D ColorTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            ColorTexture.SetData(new[] { Color.White } );

            spriteBatch.Draw(ColorTexture, Background, Color.LightGray);

            foreach (Button button in buttons)
                button.Draw(spriteBatch);
                
            switch (objectType)
            {
                case ObjectType.Boundary:
                    Fonts.PropertyEditorUI.LeftCenteredDraw(spriteBatch, $"Centre: {{X:{Math.Round(Boundary.Centre.X, 1)}, Y:{Math.Round(Boundary.Centre.Y, 1)}}}", Color.Black, new Vector2(130, 25), 1f);
                    Fonts.PropertyEditorUI.LeftCenteredDraw(spriteBatch, $"Rotation: {Math.Round(Boundary.Rotation * 180 / MathHelper.Pi)}", Color.Black, new Vector2(130, 55), 1f);
                    Fonts.PropertyEditorUI.LeftCenteredDraw(spriteBatch, $"Size: {{X:{Math.Round(Boundary.Size.X)}, Y:{Math.Round(Boundary.Size.Y)}}}", Color.Black, new Vector2(130, 85), 1f);
                    break;
                    
                case ObjectType.GrapplePoint:
                    Fonts.PropertyEditorUI.LeftCenteredDraw(spriteBatch, $"Location: {{X:{Math.Round(GrapplePointLocation.X, 1)}, Y:{Math.Round(GrapplePointLocation.Y, 1)}}}", Color.Black, new Vector2(130, 25), 1f);
                    break; 
                
                case ObjectType.SpawnPoint:
                    Fonts.PropertyEditorUI.LeftCenteredDraw(spriteBatch, $"Centre: {{X:{Math.Round(SpawnPoint.Centre.X, 1)}, Y:{Math.Round(SpawnPoint.Centre.Y, 1)}}}", Color.Black, new Vector2(130, 25), 1f);
                    Fonts.PropertyEditorUI.LeftCenteredDraw(spriteBatch, $"Rotation: {Math.Round(SpawnPoint.Rotation * 180 / MathHelper.Pi)}", Color.Black, new Vector2(130, 55), 1f);
                    break;
            }
        }
    }
}
