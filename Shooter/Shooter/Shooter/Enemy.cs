using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Shooter
{
    class Enemy
    {

        // Animation representing the player
        public Texture2D EnemyTexture;

        // Position of the Player relative to the upper left side of the screen
        public Vector2 Position;

        // State of the player
        public bool Active;

        // Amount of hit points that player has
        public int Health;

        // Star speed
        public double Speed;

        // Get the width of the player ship
        public int Width
        {
            get { return EnemyTexture.Width; }
        }

        // Get the height of the player ship
        public int Height
        {
            get { return EnemyTexture.Height; }
        }


        public void Initialize(Texture2D texture, Vector2 position)
        {
            EnemyTexture = texture;

            // Set the starting position of the player around the middle of thescreen and to the back
            Position = position;

            // Set the player to be active
            Active = true;

            // Set the player health
            Health = 100;

            Random rand = new Random();
            Speed = rand.Next(1, 100) / 10;
        }


        public void Update()
        {
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(EnemyTexture, Position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
    }
}
