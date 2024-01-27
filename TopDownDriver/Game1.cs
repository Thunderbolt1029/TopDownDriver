using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TopDownDriver
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Camera camera;

        Texture2D ColorTexture;

        Texture2D Background;
        readonly Rectangle BackgroundOutRectangle = new Rectangle(-600, -400, 2200, 1500);

        Player[] players = new Player[4];

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
                Zoom = 1.5f
            };

            players[0] = new Player(GraphicsDevice, new Vector2(100), 0f, PlayerIndex.One);

            for (int i = 0; i < 4; i++)
                ControllerHeldButtons[i] = System.Array.Empty<Buttons>();

            Globals.Initialize();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            Texture2D[] PlayerTextures = new Texture2D[4];
            for (int i = 0; i < 4; i++)
                PlayerTextures[i] = Content.Load<Texture2D>("Textures/Player/WhiteCar");
            Player.PlayerIndexTextures = PlayerTextures;

            Background = Content.Load<Texture2D>("Textures/Background");
        }

        protected override void Update(GameTime gameTime)
        {
            UpdateInputs();
            if (KeyboardHeldButtons.Contains(Keys.Escape))
                Exit();

            // Options
            
            for (int i = (int)PlayerIndex.One; i <= (int)PlayerIndex.Four; i++)
            {
                if (GamePad.GetState(i).IsConnected)
                {
                    if (players[i] == null && ControllerClickedButtons[i].Contains(Buttons.Start))
                        players[i] = new Player(GraphicsDevice, new Vector2(100), 0f, (PlayerIndex)i);
                    else if (ControllerClickedButtons[i].Contains(Buttons.Back))
                        players[i] = null;
                }
                else if (players[i] != null)
                    players[i] = null;
            }

            if (camera.Zoom > 0.5f && (ControllerClickedButtons[0].Contains(Buttons.LeftShoulder) || KeyboardClickedButtons.Contains(Keys.OemMinus)))
                camera.Zoom -= 0.5f;
            if (camera.Zoom < 2.5f && (ControllerClickedButtons[0].Contains(Buttons.RightShoulder) || KeyboardClickedButtons.Contains(Keys.OemPlus)))
                camera.Zoom += 0.5f;
            if (ControllerClickedButtons[0].Contains(Buttons.Y) || KeyboardClickedButtons.Contains(Keys.OemOpenBrackets))
                camera.Zoom = 1.5f;

            //if (!Globals.UsingController && ControllerClickedButtons[0].Length != 0) Globals.UsingController = true;
            //if (Globals.UsingController && KeyboardClickedButtons.Length != 0) Globals.UsingController = false;



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

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Draw from camera perspective
            _spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, camera.GetTransformation(GraphicsDevice));

            for (int x = BackgroundOutRectangle.Left; x < BackgroundOutRectangle.Right; x += Background.Width)
                for (int y = BackgroundOutRectangle.Top; y < BackgroundOutRectangle.Bottom; y += Background.Height)
                    _spriteBatch.Draw(Background, new Vector2(x, y), null, Color.White, 0f, Vector2.Zero, 1, SpriteEffects.None, 1);

            foreach (Hitbox hitbox in Globals.Bounds)
                _spriteBatch.Draw(ColorTexture, hitbox.DisplayRectangle, null, Color.Black, hitbox.rotation, new Vector2(0.5f), SpriteEffects.None, 0f);

            foreach (Player player in players)
                player?.Draw(_spriteBatch);

            _spriteBatch.End();

            // Draw UI
            _spriteBatch.Begin();

            // TODO UI

            _spriteBatch.End();
        }

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
    }
}
