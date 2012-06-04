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

        public float enemyMoveSpeed;

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

            Random rand = new Random();
            enemyMoveSpeed = rand.Next(1, 50) / 10;
            if (enemyMoveSpeed == 0)
            {
                enemyMoveSpeed = 1;
            }
        }


        public void Update(int bottom)
        {
            // The enemy always moves to the left so decrement x
            Position.Y += enemyMoveSpeed;

            // If the enemy is past the screen or its health reaches 0, deactivate
            if (Position.Y > (Height + bottom) )
            {
                // By setting the Active flag to false, the game will remove
                Active = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(EnemyTexture, Position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

    }
}
