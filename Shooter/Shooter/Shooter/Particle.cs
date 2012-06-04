using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Shooter
{
    class Particle
    {
        // Animation representing the player
        public Texture2D ParticleTexture;

        // Position of the Player relative to the upper left side of the screen
        public Vector2 Position;

        // State of the player
        public bool Active;

        // Star speed
        public float XSpeed;
        public float YSpeed;


        // Get the width of the player ship
        public int Width
        {
            get { return ParticleTexture.Width; }
        }

        // Get the height of the player ship
        public int Height
        {
            get { return ParticleTexture.Height; }
        }


        public void Initialize(Texture2D texture, Vector2 position)
        {
            ParticleTexture = texture;

            // Set the starting position of the player around the middle of thescreen and to the back
            Position = position;

            // Set the speed to be active
            Active = true;

            // Set the speed and random with negatives
            Random rand = new Random();
            int random = rand.Next() % 4;

            if (random == 0)
            {
                rand = new Random();
                XSpeed = rand.Next() / 20.0f;
                YSpeed = rand.Next() / 20.0f;
            }
            else if (random == 1)
            {
                rand = new Random();
                XSpeed = (rand.Next() % 100) / 20.0f;
                YSpeed = -(rand.Next() % 100) / 20.0f;
            }
            else if (random == 2)
            {
                rand = new Random();
                XSpeed = -(rand.Next() % 100) / 20.0f;
                YSpeed = (rand.Next() % 100) / 20.0f;
            }
            else if (random == 3)
            {
                rand = new Random();
                XSpeed = -(rand.Next() % 100) / 20.0f;
                YSpeed = -(rand.Next() % 100) / 20.0f;
            }
        }


        public void Update(int right, int bottom)
        {
            Position.X += XSpeed;
            Position.Y += YSpeed;

            if (Position.X < 0 || Position.X > right || Position.Y < 0 || Position.Y > bottom)
                Active = false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(ParticleTexture, Position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

    }
}
