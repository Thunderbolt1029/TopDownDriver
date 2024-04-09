using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.IO;

namespace TopDownDriver
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Camera camera;

        Texture2D ColorTexture;

        const int GrapplePointTextureRadius = 10;
        Texture2D GrapplePointTexture;

        Texture2D Background;
        readonly Rectangle BackgroundOutRectangle = new Rectangle(-600, -400, 2200, 1500);

        Player[] players = new Player[4];

        bool UsingController = true;

        Keys[] KeyboardHeldButtons = System.Array.Empty<Keys>(), KeyboardPreviousHeldButtons = System.Array.Empty<Keys>();
        Keys[] KeyboardClickedButtons => KeyboardHeldButtons.Except(KeyboardPreviousHeldButtons).ToArray();
        Buttons[][] ControllerHeldButtons = new Buttons[4][], ControllerPreviousHeldButtons = new Buttons[4][];
        Buttons[][] ControllerClickedButtons
        {
            get
            {
                Buttons[][] clicked = new Buttons[4][];
                for (int i = 0; i < 4; i++)
                    clicked[i] = ControllerHeldButtons[i].Except(ControllerPreviousHeldButtons[i]).ToArray();
                return clicked;
            }
        }

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _graphics.PreferredBackBufferWidth = 1080;
            _graphics.PreferredBackBufferHeight = 720;
        }

        protected override void Initialize()
        {
            ColorTexture = new Texture2D(GraphicsDevice, 1, 1);
            ColorTexture.SetData(new[] { Color.White });

            camera = new Camera
            {
                Zoom = 1.0f
            };

            for (int i = 0; i < 4; i++)
                ControllerHeldButtons[i] = Array.Empty<Buttons>();

            Globals.Initialize(GraphicsDevice);
            LoadLevelIntoEditor("Level0");

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load car textures
            Texture2D[] PlayerTextures = new Texture2D[4];
            for (int i = 0; i < 4; i++)
                PlayerTextures[i] = Content.Load<Texture2D>("Textures/Player/WhiteCar");
            Player.PlayerIndexTextures = PlayerTextures;

            // Background tiled texture
            Background = Content.Load<Texture2D>("Textures/Background");

            // Procedural grapple point "circle"
            GrapplePointTexture = CreatePolygonTexture(10, GrapplePointTextureRadius);

            // Level files
            foreach (string file in Directory.EnumerateFiles("Levels/", "*.json"))
                Globals.Levels.Add(Level.FromJson(File.ReadAllText(file)));

            // Fonts
            Fonts.PropertyEditorUI = new Font(Content.Load<SpriteFont>("Fonts/PropertyEditorUI"));
        }

        protected override void Update(GameTime gameTime)
        {
            UpdateInputs();
            if (KeyboardHeldButtons.Contains(Keys.Escape))
                Exit();

            //InGameUpdate(gameTime);
            LevelEditorUpdate(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //InGameDraw(gameTime);
            LevelEditorDraw(gameTime);
        }

        #region InGame
        void InGameUpdate(GameTime gameTime)
        {
            // Options

            for (int i = (int)PlayerIndex.One; i <= (int)PlayerIndex.Four; i++)
            {
                if (!UsingController && i == 3) break;
                int j = UsingController ? i : i + 1;

                if (GamePad.GetState(i).IsConnected)
                {
                    if (players[j] == null && ControllerClickedButtons[i].Contains(Buttons.Start))
                        players[j] = new Player(GraphicsDevice, Globals.CurrentLevel.SpawnPoint.Centre, Globals.CurrentLevel.SpawnPoint.Rotation, (PlayerIndex)j, UsingController);
                    else if (ControllerClickedButtons[i].Contains(Buttons.Back))
                        players[j] = null;
                }
                else if (players[j] != null)
                    players[j] = null;
            }

            if (camera.Zoom > 0.0f && (ControllerClickedButtons[0].Contains(Buttons.LeftShoulder) || KeyboardClickedButtons.Contains(Keys.OemMinus)))
                camera.Zoom -= 0.5f;
            if (camera.Zoom < 2.0f && (ControllerClickedButtons[0].Contains(Buttons.RightShoulder) || KeyboardClickedButtons.Contains(Keys.OemPlus)))
                camera.Zoom += 0.5f;
            if (ControllerClickedButtons[0].Contains(Buttons.Y) || KeyboardClickedButtons.Contains(Keys.OemOpenBrackets))
                camera.Zoom = 1.0f;


            //  Update players 
            foreach (Player player in players)
                player?.Update(gameTime);


            // Update camera
            Vector2 AveragePlayerPosition = Vector2.Zero;
            int ActivePlayerCount = 0;
            for (int i = 0; i < 4; i++)
                if (players[i] != null)
                {
                    AveragePlayerPosition += players[i].Centre;
                    ActivePlayerCount++;
                }
            if (ActivePlayerCount != 0)
            {
                AveragePlayerPosition /= ActivePlayerCount;
                camera.Position = AveragePlayerPosition;
            }
        }

        void InGameDraw(GameTime gameTime)
        {
            // Draw world objects from camera perspective
            _spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, camera.GetTransformation(GraphicsDevice));

            for (int x = BackgroundOutRectangle.Left; x < BackgroundOutRectangle.Right; x += Background.Width)
                for (int y = BackgroundOutRectangle.Top; y < BackgroundOutRectangle.Bottom; y += Background.Height)
                    _spriteBatch.Draw(Background, new Vector2(x, y), null, Color.White, 0f, Vector2.Zero, 1, SpriteEffects.None, 1);

            foreach (Hitbox hitbox in Globals.CurrentLevel.Bounds)
                _spriteBatch.Draw(ColorTexture, hitbox.DisplayRectangle, null, Color.Black, hitbox.Rotation, new Vector2(0.5f), SpriteEffects.None, 0f);

            float Scale = 0.4f;
            for (int i = 0; i < Globals.CurrentLevel.GrapplePoints.Count; i++)
                _spriteBatch.Draw(GrapplePointTexture, Globals.CurrentLevel.GrapplePoints[i] - GrapplePointTexture.Bounds.Size.ToVector2() * Scale / (2 * (float)GrapplePointTextureRadius), null, Color.LightBlue, 0f, Vector2.Zero, Scale / (float)GrapplePointTextureRadius, SpriteEffects.None, 0f);

            foreach (Player player in players)
                player?.Draw(_spriteBatch);

            _spriteBatch.End();

            // Draw UI
            _spriteBatch.Begin();

            // TODO: InGame UI

            _spriteBatch.End();
        }
        #endregion

        #region LevelEditor
        MouseState CurrentMouseState, PreviousMouseState;
        ObjectPropertyUIBox objectPropertyUIBox = new ObjectPropertyUIBox(ObjectType.None);

        void LevelEditorUpdate(GameTime gameTime)
        {
            // Manage mouse inputs
            PreviousMouseState = CurrentMouseState;
            CurrentMouseState = Mouse.GetState();
            bool MouseClick = CurrentMouseState.LeftButton == ButtonState.Pressed && PreviousMouseState.LeftButton == ButtonState.Released;
            
            // Calculate mouse pos in world space
            Vector2 WorldMousePos = CurrentMouseState.Position.ToVector2() - GraphicsDevice.Viewport.Bounds.Size.ToVector2() / 2;
            WorldMousePos /= camera.Zoom;
            WorldMousePos += camera.Position;

            // Handle UI selections
            if (!objectPropertyUIBox.Background.Contains(CurrentMouseState.Position))
            {
                // Handle highlighting for level objects

                for (int i = 0; i < Globals.CurrentEditingLevel.Bounds.Count; i++)
                {
                    if (MouseClick && Globals.CurrentEditingLevel.Bounds[i].Item1.Contains(WorldMousePos))
                    {
                        Globals.CurrentEditingLevel.Bounds[i] = (Globals.CurrentEditingLevel.Bounds[i].Item1, true);
                        objectPropertyUIBox = new ObjectPropertyUIBox(ObjectType.Boundary, i);
                    }
                    else if (MouseClick)
                        Globals.CurrentEditingLevel.Bounds[i] = (Globals.CurrentEditingLevel.Bounds[i].Item1, false);
                }

                for (int i = 0; i < Globals.CurrentEditingLevel.GrapplePoints.Count; i++)
                {
                    if (MouseClick && (Globals.CurrentEditingLevel.GrapplePoints[i].Item1 - WorldMousePos).Length() < GrapplePointTextureRadius)
                    {
                        Globals.CurrentEditingLevel.GrapplePoints[i] = (Globals.CurrentEditingLevel.GrapplePoints[i].Item1, true);
                        objectPropertyUIBox = new ObjectPropertyUIBox(ObjectType.GrapplePoint, i);
                    }
                    else if (MouseClick)
                        Globals.CurrentEditingLevel.GrapplePoints[i] = (Globals.CurrentEditingLevel.GrapplePoints[i].Item1, false);
                }

                if (MouseClick && new Rectangle((int)Math.Round(Globals.CurrentEditingLevel.SpawnPoint.Item1.Centre.X - 10), (int)Math.Round(Globals.CurrentEditingLevel.SpawnPoint.Item1.Centre.Y - 10), (int)Math.Round(Player.Size.X), (int)Math.Round(Player.Size.Y)).Contains(WorldMousePos))
                {
                    Globals.CurrentEditingLevel.SpawnPoint = (Globals.CurrentEditingLevel.SpawnPoint.Item1, true);
                    objectPropertyUIBox = new ObjectPropertyUIBox(ObjectType.SpawnPoint);
                }
                else if (MouseClick)
                    Globals.CurrentEditingLevel.SpawnPoint = (Globals.CurrentEditingLevel.SpawnPoint.Item1, false);

                if (!(Globals.CurrentEditingLevel.Bounds.Any(x => x.Item2) || Globals.CurrentEditingLevel.GrapplePoints.Any(x => x.Item2) || Globals.CurrentEditingLevel.SpawnPoint.Item2))
                    objectPropertyUIBox = new ObjectPropertyUIBox(ObjectType.None);
            }
            else
                // Handle manipulation of highlighted objects
                objectPropertyUIBox.Update(gameTime);

            // Update camera
            if (KeyboardHeldButtons.Contains(Keys.D))
                camera.Position += Vector2.UnitX * 10 / camera.Zoom;
            if (KeyboardHeldButtons.Contains(Keys.A))
                camera.Position -= Vector2.UnitX * 10 / camera.Zoom;
            if (KeyboardHeldButtons.Contains(Keys.S))
                camera.Position += Vector2.UnitY * 10 / camera.Zoom;
            if (KeyboardHeldButtons.Contains(Keys.W))
                camera.Position -= Vector2.UnitY * 10 / camera.Zoom;

            if (KeyboardClickedButtons.Contains(Keys.OemMinus))
                camera.Zoom -= 0.1f;
            if (KeyboardClickedButtons.Contains(Keys.OemPlus))
                camera.Zoom += 0.1f;
            if (KeyboardClickedButtons.Contains(Keys.OemOpenBrackets))
                camera.Zoom = 1.0f;
        }

        void LevelEditorDraw(GameTime gameTime)
        {
            // Draw world objects from camera perspective
            _spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, camera.GetTransformation(GraphicsDevice));

            // Background tiles
            for (int x = BackgroundOutRectangle.Left; x < BackgroundOutRectangle.Right; x += Background.Width)
                for (int y = BackgroundOutRectangle.Top; y < BackgroundOutRectangle.Bottom; y += Background.Height)
                    _spriteBatch.Draw(Background, new Vector2(x, y), null, Color.White, 0f, Vector2.Zero, 1, SpriteEffects.None, 1);

            
            // Draw game objects
            // Boundaries
            foreach ((Hitbox, bool) hitbox in Globals.CurrentEditingLevel.Bounds)
                _spriteBatch.Draw(ColorTexture, hitbox.Item1.DisplayRectangle, null, new Color(Color.Black, hitbox.Item2 ? 1 : 0.5f), hitbox.Item1.Rotation, new Vector2(0.5f), SpriteEffects.None, 0f);

            // Grapple points
            float Scale = 0.4f;
            for (int i = 0; i < Globals.CurrentEditingLevel.GrapplePoints.Count; i++)
                _spriteBatch.Draw(GrapplePointTexture, Globals.CurrentEditingLevel.GrapplePoints[i].Item1 - GrapplePointTexture.Bounds.Size.ToVector2() * Scale / (2 * (float)GrapplePointTextureRadius), null, new Color(Color.LightBlue, Globals.CurrentEditingLevel.GrapplePoints[i].Item2 ? 1 : 0.5f), 0f, Vector2.Zero, Scale / (float)GrapplePointTextureRadius, SpriteEffects.None, 0f);

            // Spawn point
            _spriteBatch.Draw(ColorTexture, new Rectangle((int)Math.Round(Globals.CurrentEditingLevel.SpawnPoint.Item1.Centre.X - 10), (int)Math.Round(Globals.CurrentEditingLevel.SpawnPoint.Item1.Centre.Y - 10), 20, 10), null, new Color(Color.Red, Globals.CurrentEditingLevel.SpawnPoint.Item2 ? 1 : 0.5f), Globals.CurrentEditingLevel.SpawnPoint.Item1.Rotation, new Vector2(0.5f), SpriteEffects.None, 0f);

            _spriteBatch.End();

            // Draw UI
            _spriteBatch.Begin();

            // Object property UI
            objectPropertyUIBox.Draw(_spriteBatch);

            _spriteBatch.End();
        }

        void LoadLevelIntoEditor(string name)
        {
            string json = File.ReadAllText($"Levels/{name}.json");
            Level level = Level.FromJson(json);
            Globals.CurrentEditingLevel = EditingLevel.LoadLevel(level);
        }
        #endregion

        void UpdateInputs()
        {
            KeyboardPreviousHeldButtons = KeyboardHeldButtons;
            KeyboardHeldButtons = Keyboard.GetState().GetPressedKeys();

            ControllerPreviousHeldButtons = (Buttons[][])ControllerHeldButtons.Clone();
            for (int i = 0; i < 4; i++)
                ControllerHeldButtons[i] = System.Array.Empty<Buttons>();
            for (PlayerIndex i = PlayerIndex.One; i <= PlayerIndex.Four; i++)
            {
                List<Buttons> buttons = new List<Buttons>();
                GamePadState state = GamePad.GetState(i);
                if (!state.IsConnected) continue;

                if (state.Buttons.A == ButtonState.Pressed)
                    buttons.Add(Buttons.A);
                if (state.Buttons.B == ButtonState.Pressed)
                    buttons.Add(Buttons.B);
                if (state.Buttons.Back == ButtonState.Pressed)
                    buttons.Add(Buttons.Back);
                if (state.Buttons.BigButton == ButtonState.Pressed)
                    buttons.Add(Buttons.BigButton);
                if (state.Buttons.LeftShoulder == ButtonState.Pressed)
                    buttons.Add(Buttons.LeftShoulder);
                if (state.Buttons.LeftStick == ButtonState.Pressed)
                    buttons.Add(Buttons.LeftStick);
                if (state.Buttons.RightShoulder == ButtonState.Pressed)
                    buttons.Add(Buttons.RightShoulder);
                if (state.Buttons.RightStick == ButtonState.Pressed)
                    buttons.Add(Buttons.RightStick);
                if (state.Buttons.Start == ButtonState.Pressed)
                    buttons.Add(Buttons.Start);
                if (state.Buttons.X == ButtonState.Pressed)
                    buttons.Add(Buttons.X);
                if (state.Buttons.Y == ButtonState.Pressed)
                    buttons.Add(Buttons.Y);

                if (state.DPad.Up == ButtonState.Pressed)
                    buttons.Add(Buttons.DPadUp);
                if (state.DPad.Down == ButtonState.Pressed)
                    buttons.Add(Buttons.DPadDown);
                if (state.DPad.Left == ButtonState.Pressed)
                    buttons.Add(Buttons.DPadLeft);
                if (state.DPad.Right == ButtonState.Pressed)
                    buttons.Add(Buttons.DPadRight);

                ControllerHeldButtons[(int)i] = buttons.ToArray();
            }
        }

        Texture2D CreatePolygonTexture(int noSides, int radius)
        {
            radius *= 10;

            Color[,] colors = new Color[radius * 2 + 2, radius * 2 + 2];
            for (int x = 0; x < radius * 2 + 2; x++)
                for (int y = 0; y < radius * 2 + 2; y++)
                    colors[x, y] = Color.Transparent;

            Point centre = new Point(radius);

            // Create lines for the sides of the polygon
            Point[] vertices = new Point[noSides];
            for (int i = 0; i < noSides; i++)
                vertices[i] = (centre.ToVector2() + AngleToVector(((float)i + 0.5f) * MathHelper.TwoPi / (float)noSides) * radius).ToPoint();

            for (int i = 0; i < noSides; i++)
            {
                Point Start = vertices[i];
                Point End = vertices[(i + 1) % noSides];

                int x0 = Start.X;
                int x1 = End.X;
                int y0 = Start.Y;
                int y1 = End.Y;

                int dx = Math.Abs(x1 - x0);
                int sx = x0 < x1 ? 1 : -1;
                int dy = -Math.Abs(y1 - y0);
                int sy = y0 < y1 ? 1 : -1;
                int error = dx + dy;

                while (true)
                {
                    colors[x0, y0] = Color.White;

                    if (x0 == x1 && y0 == y1) break;
                    int e2 = 2 * error;
                    if (e2 >= error)
                    {
                        if (x0 == x1) break;
                        error += dy;
                        x0 += sx;
                    }
                    if (e2 <= dx)
                    {
                        if (y0 == y1) break;
                        error += dx;
                        y0 += sy;
                    }
                }
            }

            List<Color> colors1 = new List<Color>();
            for (int x = 0; x < radius * 2 + 2; x++)
                for (int y = 0; y < radius * 2 + 2; y++)
                    colors1.Add(colors[x, y]);

            Color[] colors2 = FloodFill(colors1.ToArray(), centre, radius);

            Texture2D texture = new Texture2D(GraphicsDevice, radius * 2 + 2, radius * 2 + 2);
            texture.SetData(colors2);
            return texture;
        }

        static Color[] FloodFill(Color[] colors, Point startPos, int radius)
        {
            Queue<Point> queue = new Queue<Point>();
            queue.Enqueue(startPos);

            while (queue.Count > 0)
            {
                Point point = queue.Dequeue();

                colors[point.X + point.Y * (radius * 2 + 2)] = Color.White;

                if ((startPos.X - point.X) * (startPos.X - point.X) + (startPos.Y - point.Y) * (startPos.Y - point.Y) > radius * radius || queue.Contains(point))
                    continue;

                if (colors[point.X + 1 + point.Y * (radius * 2 + 2)].A == 0)
                    queue.Enqueue(new Point(point.X + 1, point.Y));

                if (colors[point.X - 1 + point.Y * (radius * 2 + 2)].A == 0)
                    queue.Enqueue(new Point(point.X - 1, point.Y));

                if (colors[point.X + (point.Y + 1) * (radius * 2 + 2)].A == 0)
                    queue.Enqueue(new Point(point.X, point.Y + 1));

                if (colors[point.X + (point.Y - 1) * (radius * 2 + 2)].A == 0)
                    queue.Enqueue(new Point(point.X, point.Y - 1));
            }

            return colors;
        }

        static Vector2 AngleToVector(float theta) => new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta));
    }
}
