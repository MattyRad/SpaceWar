using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Shooter
{
    class Star
    {
        // Animation representing the player
        public Texture2D StarTexture;

        // Position of the Player relative to the upper left side of the screen
        public Vector2 Position;

        // State of the player
        public bool Active;

        // Star speed
        public double Speed;

        // Get the width of the player ship
        public int Width
        {
            get { return StarTexture.Width; }
        }

        // Get the height of the player ship
        public int Height
        {
            get { return StarTexture.Height; }
        }


        public void Initialize(Texture2D texture, Vector2 position)
        {
            StarTexture = texture;

            // Set the starting position of the player around the middle of thescreen and to the back
            Position = position;

            // Set the speed to be active
            Active = true;

            // Set the star speed
            Random rand = new Random();
            Speed = rand.Next(1, 100) / 10;
        }


        public void Update(float max)
        {
            Position.Y = Position.Y + (float)Speed;

            if (Position.Y > max)
            {
                Position.Y = 0;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(StarTexture, Position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

    }
}
