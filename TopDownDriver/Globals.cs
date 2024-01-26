using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopDownDriver
{
    internal static class Globals
    {
        public static bool UsingController = true;

        public static List<Hitbox> Bounds = new List<Hitbox>();

        public static Keys[] KeyboardHeldButtons = new Keys[0], KeyboardPreviousHeldButtons = new Keys[0];
        public static Keys[] KeyboardClickedButtons => KeyboardHeldButtons.Except(KeyboardPreviousHeldButtons).ToArray();
        public static Buttons[][] ControllerHeldButtons = new Buttons[4][], ControllerPreviousHeldButtons = new Buttons[4][];
        public static Buttons[][] ControllerClickedButtons
        {
            get
            {
                Buttons[][] clicked = new Buttons[4][];
                for (int i = 0; i < 4; i++)
                    clicked[i] = ControllerHeldButtons[i].Except(ControllerPreviousHeldButtons[i]).ToArray();
                return clicked;
            }
        }

        public static void Initialize()
        {
            Bounds.AddRange(new[]
            {
                new Hitbox(new Rectangle(-10, -10, 20, 740), 0f),
                new Hitbox(new Rectangle(-10, 710, 1100, 20), 0f),
                new Hitbox(new Rectangle(1070, -10, 20, 740), 0f),
                new Hitbox(new Rectangle(-10, -10, 1100, 20), 0f),

                new Hitbox(new Rectangle(490, 310, 100, 100), MathHelper.PiOver4 / 2)
            });

            for (int i = 0; i < 4; i++)
                ControllerHeldButtons[i] = new Buttons[0];
        }

        public static void UpdateInputs()
        {
            KeyboardPreviousHeldButtons = KeyboardHeldButtons;
            KeyboardHeldButtons = Keyboard.GetState().GetPressedKeys();

            ControllerPreviousHeldButtons = (Buttons[][])ControllerHeldButtons.Clone();
            for (int i = 0; i < 4; i++)
                ControllerHeldButtons[i] = new Buttons[0];
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
