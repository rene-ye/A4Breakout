using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame1
{
    class Brick
    {
        bool active;
        Texture2D texture;
        Rectangle location;
        Color brickColor;

        public Rectangle Location
        {
            get { return location; }
        }

        public Brick(Texture2D t, Rectangle r, Color c)
        {
            texture = t;
            location = r;
            brickColor = c;
            active = true;
        }

        public int CheckCollision(Ball b)
        {
            if (active && b.Bounds.Intersects(location))
            {
                active = false;
                b.Deflection();
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public void Draw(SpriteBatch sb)
        {
            if (active)
                sb.Draw(texture, location, brickColor);
        }
    }
}
