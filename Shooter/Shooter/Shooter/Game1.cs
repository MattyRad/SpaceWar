using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input.Touch;

namespace Shooter
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        const float ENEMY_SPAWN_TIME = 0.5f;
        const float ENEMY_TWO_SPAWN_TIME = 2.0f;
        const float FIRE_TIME = 0.20f;

        const int MAX_ENEMY_TWO = 25;

        bool GAME_OVER;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // Represents the player 
        Player player;

        // A movement speed for the player
        float playerMoveSpeed;

        //Number that holds the player score
        int score;

        // Keyboard states used to determine key presses
        KeyboardState currentKeyboardState;
        KeyboardState previousKeyboardState;

        // Gamepad states used to determine button presses
        GamePadState currentGamePadState;
        GamePadState previousGamePadState;

        // Enemies
        Texture2D enemyTexture;
        List<Enemy> enemies;

        Texture2D enemyTwoTexture;
        List<EnemyTwo> enemyTwo;

        // Projectiles
        Texture2D projectileTexture;
        List<Projectile> projectiles;

        // Explosions
        Texture2D explosionTexture;
        List<Animation> explosions;

        //Stars
        Texture2D starTexture;
        List<Star> stars;

        Texture2D particleTexture;
        List<Particle> particles;

        // The rate at which the enemies appear
        TimeSpan enemySpawnTime;
        TimeSpan enemyTwoSpawnTime;

        TimeSpan previousSpawnTime;
        TimeSpan previousTwoSpawnTime;

        TimeSpan enemyOneShifter;
        TimeSpan enemyTwoShifter;

        // The rate of fire of the player laser
        TimeSpan fireTime;
        TimeSpan previousFireTime;

        // A random number generator
        Random random;

        // The sound that is played when a laser is fired
        SoundEffect laserSound;

        // The sound used when the player or an enemy dies
        SoundEffect explosionSound;

        // The music played during gameplay
        Song gameplayMusic;

        // The font used to display UI elements
        SpriteFont font;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            //Initialize the player class
            player = new Player();

            // Set a constant player move speed
            playerMoveSpeed = 5.0f;

            //Set player's score to zero
            score = 0;

            //Enable the FreeDrag gesture.
            TouchPanel.EnabledGestures = GestureType.FreeDrag;

            // Initialize the enemies list
            enemies = new List<Enemy>();
            enemyTwo = new List<EnemyTwo>();
            projectiles = new List<Projectile>();
            explosions = new List<Animation>();
            stars = new List<Star>();
            particles = new List<Particle>();

            // Set the time keepers to zero
            previousSpawnTime = TimeSpan.Zero;
            previousTwoSpawnTime = TimeSpan.Zero;

            // Used to determine how fast enemy respawns
            enemySpawnTime = TimeSpan.FromSeconds(ENEMY_SPAWN_TIME);
            enemyTwoSpawnTime = TimeSpan.FromSeconds(ENEMY_TWO_SPAWN_TIME);

            // Set the laser to fire every quarter second
            fireTime = TimeSpan.FromSeconds(FIRE_TIME);

            // Difficulty shift
            enemyOneShifter = new TimeSpan(0, 0, 0);
            enemyTwoShifter = new TimeSpan(0, 0, 0);

            GAME_OVER = false;

            // Initialize our random number generator
            random = new Random();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load the player resources
            Vector2 playerPosition = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
            player.Initialize(Content.Load<Texture2D>("player"), playerPosition);

            // Load textures
            enemyTexture = Content.Load<Texture2D>("mine");
            enemyTwoTexture = Content.Load<Texture2D>("enemyTwo");
            projectileTexture = Content.Load<Texture2D>("laser");
            explosionTexture = Content.Load<Texture2D>("explosion");
            starTexture = Content.Load<Texture2D>("star");
            particleTexture = Content.Load<Texture2D>("particle");

            // Sounds
            gameplayMusic = Content.Load<Song>("sound/gameMusic");
            laserSound = Content.Load<SoundEffect>("sound/laserFire");
            explosionSound = Content.Load<SoundEffect>("sound/explosion");

            // Font
            font = Content.Load<SpriteFont>("gameFont");

            // Start the music right away
            PlayMusic(gameplayMusic);

            // Load the stars right away
            for (int i = 0; i < 200; i++)
            {
                AddStar();
            }
        }

        private void AddEnemy()
        {
            // Randomly generate the position of the enemy
            Vector2 position = new Vector2(random.Next(0, GraphicsDevice.Viewport.Width), -10);

            // Create an enemy
            Enemy enemy = new Enemy();

            // Initialize the enemy
            enemy.Initialize(enemyTexture, position);

            // Add the enemy to the active enemies list
            enemies.Add(enemy);
        }

        private void AddEnemyTwo()
        {
            // Randomly generate the position of the enemy
            Vector2 position = new Vector2(random.Next(-20, GraphicsDevice.Viewport.Width + 20), -10);

            // Create an enemy
            EnemyTwo enemy = new EnemyTwo();

            // Initialize the enemy
            enemy.Initialize(enemyTwoTexture, position);

            // Add the enemy to the active enemies list
            enemyTwo.Add(enemy);
        }

        private void AddParticle(int x, int y)
        {
            // Randomly generate the position of the enemy
            Vector2 position = new Vector2((float)x, (float)y);

            // Create an enemy
            Particle part = new Particle();

            // Initialize the enemy
            part.Initialize(particleTexture, position);

            // Add the enemy to the active enemies list
            particles.Add(part);
        }

        private void UpdateEnemies(GameTime gameTime)
        {
            // Spawn a new enemy enemy every 1.5 seconds
            if (gameTime.TotalGameTime - (previousSpawnTime -enemyOneShifter) > enemySpawnTime)
            {
                previousSpawnTime = gameTime.TotalGameTime;

                if (enemyOneShifter.Milliseconds < 400)
                    enemyOneShifter = enemyOneShifter.Add(new TimeSpan(0, 0, 0, 0, 5));

                // Add an Enemy
                AddEnemy();
            }

            // Update the Enemies
            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                enemies[i].Update(GraphicsDevice.Viewport.Height);

                if (enemies[i].Active == false)
                {
                    enemies.RemoveAt(i);
                }
            }
        }

        private void UpdateEnemyTwo(GameTime gameTime)
        {
            // Spawn a new enemy enemy every 1.5 seconds
            if (gameTime.TotalGameTime - (previousTwoSpawnTime - enemyTwoShifter) > enemyTwoSpawnTime)
            {
                previousTwoSpawnTime = gameTime.TotalGameTime;

                if (enemyTwoShifter.Milliseconds < 1000 && enemyTwo.Count < MAX_ENEMY_TWO)
                {
                    enemyTwoShifter = enemyTwoShifter.Add(new TimeSpan(0, 0, 0, 0, 20));
                }

                // Add an Enemy
                AddEnemyTwo();
            }

            if (enemyTwo.Count > MAX_ENEMY_TWO)
            {
                enemyTwoShifter = new TimeSpan(0, 0, 0);
            }

            // Update the Enemies
            for (int i = enemyTwo.Count - 1; i >= 0; i--)
            {
                enemyTwo[i].Update(player);

                if (enemyTwo[i].Active == false)
                {
                    enemyTwo.RemoveAt(i);
                }
            }
        }

        private void UpdateParticles()
        {
            for (int i = particles.Count - 1; i >= 0; i--)
            {
                particles[i].Update(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

                if (particles[i].Active == false)
                {
                    particles.RemoveAt(i);
                }
            }
        }

        private void AddProjectile(Vector2 position)
        {
            Projectile projectile = new Projectile();
            projectile.Initialize(GraphicsDevice.Viewport, projectileTexture, position);
            projectile.xmove = currentGamePadState.ThumbSticks.Right.X * 10.0f;
            projectile.ymove = currentGamePadState.ThumbSticks.Right.Y * 10.0f;
            projectiles.Add(projectile);
        }

        private void AddStar()
        {
            Star star = new Star();

            Vector2 starPosition = new Vector2(random.Next(0, GraphicsDevice.Viewport.Width), random.Next(0, GraphicsDevice.Viewport.Height));

            star.Initialize(starTexture, starPosition);

            stars.Add(star);
        }

        private void UpdateProjectiles()
        {
            // Update the Projectiles
            for (int i = projectiles.Count - 1; i >= 0; i--)
            {
                projectiles[i].Update(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

                if (projectiles[i].Active == false)
                {
                    projectiles.RemoveAt(i);
                }
            }
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // Save the previous state of the keyboard and game pad so we can determinesingle key/button presses
            previousGamePadState = currentGamePadState;
            previousKeyboardState = currentKeyboardState;

            // Read the current state of the keyboard and gamepad and store it
            currentKeyboardState = Keyboard.GetState();
            currentGamePadState = GamePad.GetState(PlayerIndex.One);

            if (!GAME_OVER)
            {
                //Update the player
                UpdatePlayer(gameTime);

                // Update the enemies
                UpdateEnemies(gameTime);
                UpdateEnemyTwo(gameTime);

                // Update the collision
                UpdateCollision();

                // Update the projectiles
                UpdateProjectiles();

                // Update the explosions
                UpdateExplosions(gameTime);

                UpdateParticles();

                // Update stars
                UpdateStars(gameTime);
            }
            else
            {
                // Update the enemies
                UpdateEnemies(gameTime);
                UpdateEnemyTwo(gameTime);

                // Update the explosions
                UpdateExplosions(gameTime);

                UpdateParticles();

                // Update stars
                UpdateStars(gameTime);
            }
            base.Update(gameTime);
        }

        private void UpdateExplosions(GameTime gameTime)
        {
            for (int i = explosions.Count - 1; i >= 0; i--)
            {
                explosions[i].Update(gameTime);
                if (explosions[i].Active == false)
                {
                    explosions.RemoveAt(i);
                }
            }
        }

        private void UpdatePlayer(GameTime gameTime)
        {
            // Windows Phone Controls
            while (TouchPanel.IsGestureAvailable)
            {
                GestureSample gesture = TouchPanel.ReadGesture();
                if (gesture.GestureType == GestureType.FreeDrag)
                {
                    player.Position += gesture.Delta;
                }
            }

            // Get Thumbstick Controls
            player.Position.X += currentGamePadState.ThumbSticks.Left.X * playerMoveSpeed;
            player.Position.Y -= currentGamePadState.ThumbSticks.Left.Y * playerMoveSpeed;

            // Use the Keyboard / Dpad
            if (currentKeyboardState.IsKeyDown(Keys.Left) ||
            currentGamePadState.DPad.Left == ButtonState.Pressed)
            {
                player.Position.X -= playerMoveSpeed;
            }
            if (currentKeyboardState.IsKeyDown(Keys.Right) ||
            currentGamePadState.DPad.Right == ButtonState.Pressed)
            {
                player.Position.X += playerMoveSpeed;
            }
            if (currentKeyboardState.IsKeyDown(Keys.Up) ||
            currentGamePadState.DPad.Up == ButtonState.Pressed)
            {
                player.Position.Y -= playerMoveSpeed;
            }
            if (currentKeyboardState.IsKeyDown(Keys.Down) ||
            currentGamePadState.DPad.Down == ButtonState.Pressed)
            {
                player.Position.Y += playerMoveSpeed;
            }


            // Make sure that the player does not go out of bounds
            player.Position.X = MathHelper.Clamp(player.Position.X, 0, GraphicsDevice.Viewport.Width - player.Width);
            player.Position.Y = MathHelper.Clamp(player.Position.Y, 0, GraphicsDevice.Viewport.Height - player.Height);

            // Fire only every interval we set as the fireTime
            if (gameTime.TotalGameTime - previousFireTime > fireTime && (currentGamePadState.ThumbSticks.Right.X != 0 || currentGamePadState.ThumbSticks.Right.Y != 0))
            {
                // Reset our current time
                previousFireTime = gameTime.TotalGameTime;

                // Add the projectile, but add it to the front and center of the player
                AddProjectile(player.Position + new Vector2(player.Width / 2, 0));

                // Play the laser sound
                laserSound.Play();
            }
        }

        private void UpdateStars(GameTime gameTime)
        {
            for (int i = stars.Count - 1; i >= 0; i--)
            {
                stars[i].Update((float)GraphicsDevice.Viewport.Height);
                if (stars[i].Active == false)
                {
                    stars.RemoveAt(i);
                }
            }
        }

        private void UpdateCollision()
        {
            // Use the Rectangle's built-in intersect functionto 
            // determine if two objects are overlapping
            Rectangle rectangle1;
            Rectangle rectangle2;

            // Only create the rectangle once for the player
            rectangle1 = new Rectangle((int)player.Position.X,
            (int)player.Position.Y,
            player.Width,
            player.Height);

            // PLAYER -><- ENEMY
            for (int i = 0; i < enemies.Count; i++)
            {
                rectangle2 = new Rectangle((int)enemies[i].Position.X,
                (int)enemies[i].Position.Y,
                enemies[i].Width,
                enemies[i].Height);

                // Determine if the two objects collided with each
                // other
                if (rectangle1.Intersects(rectangle2))
                {
                    GAME_OVER = true;

                    AddExplosion(player.Position);

                    // YOU LOSE
                    player.Active = false;
                }

            }

            // PLAYER -><- ENEMYTWO
            for (int i = 0; i < enemyTwo.Count; i++)
            {
                rectangle2 = new Rectangle((int)enemyTwo[i].Position.X,
                (int)enemyTwo[i].Position.Y,
                enemyTwo[i].Width,
                enemyTwo[i].Height);

                // Determine if the two objects collided with each
                // other
                if (rectangle1.Intersects(rectangle2))
                {
                    GAME_OVER = true;

                    AddExplosion(player.Position);

                    // YOU LOSE
                    player.Active = false;
                }

            }

            // PROJECTILE -><- ENEMY
            for (int i = 0; i < projectiles.Count; i++)
            {
                for (int j = 0; j < enemies.Count; j++)
                {
                    // Create the rectangles we need to determine if we collided with each other
                    rectangle1 = new Rectangle((int)projectiles[i].Position.X, (int)projectiles[i].Position.Y, projectiles[i].Width, projectiles[i].Height);

                    rectangle2 = new Rectangle((int)enemies[j].Position.X, (int)enemies[j].Position.Y, enemies[j].Width, enemies[j].Height);

                    // Determine if the two objects collided with each other
                    if (rectangle1.Intersects(rectangle2))
                    {
                        enemies[j].Active = false;
                        projectiles[i].Active = false;

                        // Add an explosion
                        AddExplosion(enemies[j].Position);
                        Fragment((int)enemies[j].Position.X, (int)enemies[j].Position.Y);

                        // Play the explosion sound
                        explosionSound.Play();

                        score += 5;
                    }
                }
            }

            // PROJECTILE -><- ENEMYTWO
            for (int i = 0; i < projectiles.Count; i++)
            {
                for (int j = 0; j < enemyTwo.Count; j++)
                {
                    // Create the rectangles we need to determine if we collided with each other
                    rectangle1 = new Rectangle((int)projectiles[i].Position.X, (int)projectiles[i].Position.Y, projectiles[i].Width, projectiles[i].Height);

                    rectangle2 = new Rectangle((int)enemyTwo[j].Position.X, (int)enemyTwo[j].Position.Y, enemyTwo[j].Width, enemyTwo[j].Height);

                    // Determine if the two objects collided with each other
                    if (rectangle1.Intersects(rectangle2))
                    {
                        enemyTwo[j].Active = false;
                        projectiles[i].Active = false;

                        // Add an explosion
                        AddExplosion(enemyTwo[j].Position);
                        //Fragment((int)enemyTwo[j].Position.X, (int)enemyTwo[j].Position.Y);

                        // Play the explosion sound
                        explosionSound.Play();

                        score += 20;
                    }
                }
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // Start drawing
            spriteBatch.Begin();

            // Draw the Player
            player.Draw(spriteBatch);

            // Draw the Enemies
            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].Draw(spriteBatch);
            }

            // Draw the Projectiles
            for (int i = 0; i < projectiles.Count; i++)
            {
                projectiles[i].Draw(spriteBatch);
            }

            // Draw the explosions
            for (int i = 0; i < explosions.Count; i++)
            {
                explosions[i].Draw(spriteBatch);
            }

            // Draw the stars
            for (int i = 0; i < stars.Count; i++)
            {
                stars[i].Draw(spriteBatch);
            }

            for (int i = 0; i < enemyTwo.Count; i++)
            {
                enemyTwo[i].Draw(spriteBatch);
            }

            for (int i = 0; i < particles.Count; i++)
            {
                particles[i].Draw(spriteBatch);
            }

            // Draw the score
            if ( !GAME_OVER )
                spriteBatch.DrawString(font, "score: " + score, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y), Color.White);
            else
                spriteBatch.DrawString(font, "GAME OVER: " + score, new Vector2((GraphicsDevice.Viewport.Width / 2) - 10, (GraphicsDevice.Viewport.Height / 2) - 10), Color.White);

            //Stop drawing
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void AddExplosion(Vector2 position)
        {
            Animation explosion = new Animation();
            explosion.Initialize(explosionTexture, position, 134, 134, 12, 45, Color.White, 1f, false);
            explosions.Add(explosion);
        }

        private void Fragment(int x, int y)
        {
            for (int i = 0; i < 30; i++)
            {
                AddParticle(x, y);
            }
        }

        private void PlayMusic(Song song)
        {
            // Due to the way the MediaPlayer plays music,
            // we have to catch the exception. Music will play when the game is not tethered
            try
            {
                // Play the music
                MediaPlayer.Play(song);

                // Loop the currently playing song
                MediaPlayer.IsRepeating = true;
            }
            catch { }
        }
    }
}
