using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


enum GameState
{
    START,
    INGAME,
    END
}

namespace Game1
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D ball;
        Vector2 bPosition;
        Vector2 bVelocity = new Vector2(300, 300);
        Rectangle bRect;

        Texture2D paddle1;
        Vector2 p1Position;
        Vector2 p1Velocity = new Vector2(300, 300);
        Rectangle p1Rect;

        Texture2D paddle2;
        Vector2 p2Position;
        Vector2 p2Velocity = new Vector2(300, 300);
        Rectangle p2Rect;

        Texture2D midBorder;
        Vector2 midBorderPosition;

        SpriteFont scoreFont, textFont;
        int p1Score = 0;
        Vector2 p1ScorePos;
        int p2Score = 0;
        Vector2 p2ScorePos;
        int winScore = 5;
        string textStart = "Press SPACE to start the game";
        string textReady = "Press SPACE when you ready to next round";

        GameState myGameState = GameState.START;

        int SCREEN_WIDTH;
        int SCREEN_HEIGHT;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 480;

            Content.RootDirectory = "Content";            
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            SCREEN_WIDTH = graphics.PreferredBackBufferWidth;
            SCREEN_HEIGHT = graphics.PreferredBackBufferHeight;
            
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            ball = Content.Load<Texture2D>("pong-ball");
            bPosition = new Vector2(SCREEN_WIDTH / 2, SCREEN_HEIGHT / 2);
            
            paddle1 = Content.Load<Texture2D>("pong-paddle");
            p1Position = new Vector2(40, SCREEN_HEIGHT / 2 - paddle1.Height / 2);

            paddle2 = Content.Load<Texture2D>("pong-paddle");
            p2Position = new Vector2(SCREEN_WIDTH - 40 - paddle2.Width, SCREEN_HEIGHT / 2 - paddle2.Height / 2);

            midBorder = Content.Load<Texture2D>("pong-net");
            midBorderPosition = new Vector2(SCREEN_WIDTH/2 - midBorder.Width/2, SCREEN_HEIGHT/2 - midBorder.Height/2);

            scoreFont = Content.Load<SpriteFont>("scoreFont");
            textFont = Content.Load<SpriteFont>("textFont");

            p1ScorePos = new Vector2(SCREEN_WIDTH / 2 - 100, 20);
            p2ScorePos = new Vector2(SCREEN_WIDTH / 2 + 100, 20);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            if (myGameState == GameState.START)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Space))
                {
                    myGameState = GameState.INGAME;
                }
            }
            else if (myGameState == GameState.INGAME)
            {
                //------------------- move position based on game time
                bPosition += bVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

                //check if ball position is over the screen
                if (bPosition.X > SCREEN_WIDTH - ball.Width || bPosition.X < 0)
                {
                    bVelocity.X *= -1;
                }
                if (bPosition.Y > SCREEN_HEIGHT - ball.Height || bPosition.Y < 0)
                {
                    bVelocity.Y *= -1;
                }

                //--------------------- get input to controll p1 paddle
                if (Keyboard.GetState().IsKeyDown(Keys.W))
                {
                    p1Position.Y -= p1Velocity.Y * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.S))
                {
                    p1Position.Y += p1Velocity.Y * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                //set paddle bound
                if (p1Position.Y < 0) p1Position.Y = 0;
                if (p1Position.Y > SCREEN_HEIGHT - paddle1.Height) p1Position.Y = SCREEN_HEIGHT - paddle1.Height;

                //---------------------- get input to controll p2 paddle
                if (Keyboard.GetState().IsKeyDown(Keys.Up))
                {
                    p2Position.Y -= p2Velocity.Y * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Down))
                {
                    p2Position.Y += p2Velocity.Y * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                //set paddle bound
                if (p2Position.Y < 0) p2Position.Y = 0;
                if (p2Position.Y > SCREEN_HEIGHT - paddle2.Height) p2Position.Y = SCREEN_HEIGHT - paddle2.Height;

                //-------------------- check collision detection between paddle and ball
                bRect = new Rectangle((int)bPosition.X, (int)bPosition.Y, ball.Width, ball.Height);
                p1Rect = new Rectangle((int)p1Position.X, (int)p1Position.Y, paddle1.Width, paddle1.Height);
                p2Rect = new Rectangle((int)p2Position.X, (int)p2Position.Y, paddle2.Width, paddle2.Height);

                this.checkBallCollisionWith(p1Rect);
                this.checkBallCollisionWith(p2Rect);

                //------------------- add score manager
                if (bPosition.X <= 0) //add score to p2
                {
                    p2Score++;
                    bPosition = new Vector2(SCREEN_WIDTH / 2, SCREEN_HEIGHT / 2);
                    if (bVelocity.X > 0) bVelocity *= -1;
                }
                if (bPosition.X >= SCREEN_WIDTH - ball.Width) //add score to p1
                {
                    p1Score++;
                    bPosition = new Vector2(SCREEN_WIDTH / 2, SCREEN_HEIGHT / 2);
                    if (bVelocity.X < 0) bVelocity *= -1;
                }

                //----------------- check when the game is end;
                if (p1Score == winScore || p2Score == winScore) {
                    myGameState = GameState.END;
                }
            }
            else if (myGameState == GameState.END)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Space)) {
                    //reset the game
                    bPosition = new Vector2(SCREEN_WIDTH / 2, SCREEN_HEIGHT / 2);
                    p1Score = 0;
                    p2Score = 0;
                    myGameState = GameState.INGAME;
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            if (myGameState == GameState.START)
            {
                spriteBatch.DrawString(textFont, textStart, new Vector2(250, 350), Color.White);
            }
            else if (myGameState == GameState.INGAME)
            {
                spriteBatch.Draw(midBorder, midBorderPosition, Color.White);
                spriteBatch.DrawString(scoreFont, p1Score.ToString(), p1ScorePos, Color.White);
                spriteBatch.DrawString(scoreFont, p2Score.ToString(), p2ScorePos, Color.White);
                spriteBatch.Draw(ball, bRect, Color.White);
            }
            else if (myGameState == GameState.END)
            {
                if (p1Score > p2Score)
                {
                    spriteBatch.DrawString(textFont, "Congratulation, Player 1 WIN", new Vector2(270, 300), Color.White);
                }
                else
                {
                    spriteBatch.DrawString(textFont, "Congratulation, Player 2 WIN", new Vector2(270, 300), Color.White);
                }

                    spriteBatch.DrawString(textFont, "Press SPACE to start NEW game", new Vector2(250, 380), Color.White);
                    spriteBatch.DrawString(textFont, "Press ESC to EXIT the game", new Vector2(270, 410), Color.White);
            }

            spriteBatch.Draw(paddle1, p1Position, Color.White);
            spriteBatch.Draw(paddle2, p2Position, Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }


        private void checkBallCollisionWith(Rectangle rect)
        {
            if (bRect.Intersects(rect))
            {
                // 0 - 1/3 width of paddle
                if (bRect.Y >= rect.Y && bRect.Y < rect.Y + rect.Height / 3)
                {
                    if (bVelocity.Y > 0)
                    {
                        if (bVelocity.Y == 0)
                        {
                            bVelocity.Y = 300;
                        }

                        bVelocity.Y *= -1;
                    }
                    else
                    {
                        if (bVelocity.Y == 0)
                        {
                            bVelocity.Y = -300;
                        }

                        bVelocity.X *= -1;
                    }
                }
                // 1/3 ~ 2/3 width of paddle
                if (bRect.Y >= rect.Y + rect.Height / 3 && bRect.Y < rect.Y + rect.Height * 2 / 3)
                {
                    bVelocity.Y = 0;
                    bVelocity.X *= -1;
                }
                // > 2/3 width of paddle
                if (bRect.Y >= rect.Y + rect.Height * 2 / 3)
                {
                    if (bVelocity.Y < 0)
                    {
                        if (bVelocity.Y == 0)
                        {
                            bVelocity.Y = -300;
                        }

                        bVelocity.Y *= -1;
                    }
                    else
                    {
                        if (bVelocity.Y == 0)
                        {
                            bVelocity.Y = 300;
                        }

                        bVelocity.X *= -1;
                    }
                }
            }
        }
        
    }
}
