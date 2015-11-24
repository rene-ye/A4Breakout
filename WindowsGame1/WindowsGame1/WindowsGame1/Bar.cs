using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace WindowsGame1
{
    class Bar
    {
        Vector2 position;
        Vector2 motion;
        int paddleSpeed = 12;

        KeyboardState keyboardState;
        GamePadState gamePadState;

        Texture2D texture;
        Rectangle screenBounds;

        public Bar(Texture2D t, Rectangle r)
        {
            texture = t;
            screenBounds = r;
            setDefault();
        }

        public void setDefault()
        {
            position.X = (screenBounds.Width - texture.Width) / 2;
            position.Y = screenBounds.Height - texture.Height - 5;
        }

        public void Update()
        {
            motion = Vector2.Zero;

            keyboardState = Keyboard.GetState();
            gamePadState = GamePad.GetState(PlayerIndex.One);

            if (keyboardState.IsKeyDown(Keys.Left) || gamePadState.IsButtonDown(Buttons.LeftThumbstickLeft))
                motion.X = -1;

            if (keyboardState.IsKeyDown(Keys.Right) || gamePadState.IsButtonDown(Buttons.LeftThumbstickRight))
                motion.X = 1;

            motion.X *= paddleSpeed;
            position += motion;
            checkInBounds();
        }

        private void checkInBounds()
        {
            if (position.X < 0)
                position.X = 0;
            if (position.X + texture.Width > screenBounds.Width)
                position.X = screenBounds.Width - texture.Width;
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(texture, position, Color.White);
        }

        public Rectangle GetBounds()
        {
            return new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
        }
    }
}
