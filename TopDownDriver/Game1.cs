using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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

            camera = new Camera();
            camera.Zoom = 2f;

            players[0] = new Player(GraphicsDevice, new Vector2(100), 0f, PlayerIndex.One);

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
            Globals.UpdateInputs();
            if (Globals.KeyboardHeldButtons.Contains(Keys.Escape))
                Exit();

            // Options
            
            for (int i = (int)PlayerIndex.One; i <= (int)PlayerIndex.Four; i++)
            {
                if (GamePad.GetState(i).IsConnected)
                {
                    if (players[i] == null && Globals.ControllerClickedButtons[i].Contains(Buttons.Start))
                        players[i] = new Player(GraphicsDevice, new Vector2(100), 0f, (PlayerIndex)i);
                    else if (Globals.ControllerClickedButtons[i].Contains(Buttons.Back))
                        players[i] = null;
                }
                else if (players[i] != null)
                    players[i] = null;
            }

            if (camera.Zoom > 1f && (Globals.ControllerClickedButtons[0].Contains(Buttons.LeftShoulder) || Globals.KeyboardClickedButtons.Contains(Keys.OemMinus)))
                camera.Zoom -= 0.5f;
            if (camera.Zoom < 3f && (Globals.ControllerClickedButtons[0].Contains(Buttons.RightShoulder) || Globals.KeyboardClickedButtons.Contains(Keys.OemPlus)))
                camera.Zoom += 0.5f;
            if (Globals.ControllerClickedButtons[0].Contains(Buttons.Y) || Globals.KeyboardClickedButtons.Contains(Keys.OemOpenBrackets))
                camera.Zoom = 2f;

            if (!Globals.UsingController && Globals.ControllerClickedButtons[0].Length != 0) Globals.UsingController = true;
            if (Globals.UsingController && Globals.KeyboardClickedButtons.Length != 0) Globals.UsingController = false;



            //  Update players 
            foreach (Player player in players)
                if (player != null)
                    player.Update(gameTime);


            // Update camera
            Vector2 AveragePlayerPosition = Vector2.Zero;
            int ActivePlayerCount = 0;
            for (int i = 0; i < 4; i++)
                if (players[i] != null)
                {
                    AveragePlayerPosition += players[i].centre;
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
            _spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, camera.get_transformation(GraphicsDevice));

            for (int x = BackgroundOutRectangle.Left; x < BackgroundOutRectangle.Right; x += Background.Width)
                for (int y = BackgroundOutRectangle.Top; y < BackgroundOutRectangle.Bottom; y += Background.Height)
                    _spriteBatch.Draw(Background, new Vector2(x, y), null, Color.White, 0f, Vector2.Zero, 1, SpriteEffects.None, 1);

            foreach (Hitbox hitbox in Globals.Bounds)
                _spriteBatch.Draw(ColorTexture, hitbox.DisplayRectangle, null, Color.Black, hitbox.rotation, new Vector2(0.5f), SpriteEffects.None, 0f);

            foreach (Player player in players)
                if (player != null)
                    player.Draw(_spriteBatch);

            _spriteBatch.End();

            // Draw UI
            _spriteBatch.Begin();

            // TODO UI

            _spriteBatch.End();
        }
    }
}
