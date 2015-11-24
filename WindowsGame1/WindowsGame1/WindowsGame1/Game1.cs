using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace WindowsGame1
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;

        Ball ball;
        Bar player;
        Rectangle screenRectangle;

        int bricksWide, score;
        int bricksHigh = 4;
        Texture2D brickImage;
        Brick[,] brickArray;
        enum MODE { game, console, paused };
        MODE gameMode;
        String consoleLine = "";

        KeyboardState lastKeyboardState;
        Keys[] lastKeys = new Keys[0];

        Color backgroundColor;

        SoundEffect soundBGM, soundPing;
        SoundEffectInstance soundBGMInstance, soundPingInstance;

        FileUtil myUtil;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 800;

            gameMode = MODE.paused;
            bricksWide = 1280 / 47;

            backgroundColor = Color.CornflowerBlue;
            screenRectangle = new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
        }
        
        protected override void Initialize()
        {
            myUtil = new FileUtil();

            base.Initialize();
        }
        
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
			
            Texture2D tempTexture = Content.Load<Texture2D>("bar");
            player = new Bar(tempTexture, screenRectangle);

            tempTexture = Content.Load<Texture2D>("ball");
            ball = new Ball(tempTexture, screenRectangle);

            brickImage = Content.Load<Texture2D>("brick");

            font = Content.Load<SpriteFont>("myFont");

            soundBGM = Content.Load<SoundEffect>("BGM");
            soundPing = Content.Load<SoundEffect>("ping");
            soundPingInstance = soundPing.CreateInstance();
            soundPingInstance.Volume = 0.5f;
            soundBGMInstance = soundBGM.CreateInstance();
            soundBGMInstance.Volume = 0.75f;
            soundBGMInstance.IsLooped = true;

            myUtil.readFile();

            StartGame();
        }

        private void StartGame()
        {
            player.setDefault();
            ball.setDefault(player.GetBounds());
            score = 0;
            brickArray = new Brick[bricksWide, bricksHigh];

            for (int y = 0; y < bricksHigh; y++)
            {
                Color brickColor = Color.White;

                switch (y)
                {
                    case 0:
                        brickColor = Color.AliceBlue;
                        break;
                    case 1:
                        brickColor = Color.AntiqueWhite;
                        break;
                    case 2:
                        brickColor = Color.Aqua;
                        break;
                    case 3:
                        brickColor = Color.Aquamarine;
                        break;
                    case 4:
                        brickColor = Color.Azure;
                        break;
                    case 5:
                        brickColor = Color.Beige;
                        break;
                    case 6:
                        brickColor = Color.Bisque;
                        break;
                }

                for (int x = 0; x < bricksWide; x++)
                {
                    brickArray[x, y] = new Brick(brickImage, new Rectangle(x*brickImage.Width, y*brickImage.Height, brickImage.Width, brickImage.Height), brickColor);
                }

                soundBGMInstance.Stop();
            }
        }
        
        protected override void UnloadContent()
        {

        }
        
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            if (Keyboard.GetState().IsKeyDown(Keys.OemTilde) && gameMode == MODE.game)
                gameMode = MODE.console;
            if (Keyboard.GetState().IsKeyDown(Keys.Escape) && gameMode == MODE.console)
            {
                gameMode = MODE.game;
                StartGame();
            }
            if ((Keyboard.GetState().IsKeyDown(Keys.P) || GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed) && gameMode == MODE.game)
            {
                gameMode = MODE.paused;
            } else if ((Keyboard.GetState().IsKeyDown(Keys.Space) || GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed) && gameMode == MODE.paused)
            {
                gameMode = MODE.game;
            }


            if (gameMode == MODE.game)
            {
                soundBGMInstance.Play();
                player.Update();
                if (ball.Update())
                {
                    soundPingInstance.Play();
                }

                foreach (Brick b in brickArray)
                {
                    if (b.CheckCollision(ball) == 1)
                    {
                        soundPingInstance.Play();
                        score += 10;
                    }
                }

                if (ball.PaddleCollision(player.GetBounds()))
                    soundPingInstance.Play();

                if (ball.checkBottom())
                {
                    myUtil.highscores.Add(score);
                    myUtil.saveFile();
                    myUtil.readFile();
                    StartGame();
                    gameMode = MODE.paused;
                }
            } else if (gameMode == MODE.console)
            {
                KeyboardState keyboardState = Keyboard.GetState();
                Keys[] keys = keyboardState.GetPressedKeys();

                foreach (Keys currentKey in keys)
                {
                    if (currentKey != Keys.None)
                    {
                        if (lastKeys.Contains(currentKey))
                        {
							//do nothing
                        }
                        else if (!lastKeys.Contains(currentKey))
                        {
                            HandleKey(currentKey);
                        }
                    }
                }
                lastKeyboardState = keyboardState;
                lastKeys = keys;
            } else if (gameMode == MODE.paused)
            {
                soundBGMInstance.Pause();
            }

            base.Update(gameTime);
        }

        public void HandleKey(Keys k)
        {
            if (k == Keys.Space)
            {
                consoleLine += " ";
            } else if (k == Keys.Enter)
            {
                execute(consoleLine);
                consoleLine = "";
            } else if (k == Keys.Back && consoleLine.Length > 0)
            {
                consoleLine = consoleLine.Remove(consoleLine.Length - 1);
            } else if (k == Keys.A || k == Keys.E || k == Keys.I || k == Keys.M || k == Keys.Q || k == Keys.U || k == Keys.Y ||
                       k == Keys.B || k == Keys.F || k == Keys.J || k == Keys.N || k == Keys.R || k == Keys.V || k == Keys.Z ||
                       k == Keys.C || k == Keys.G || k == Keys.K || k == Keys.O || k == Keys.S || k == Keys.W ||
                       k == Keys.D || k == Keys.H || k == Keys.L || k == Keys.P || k == Keys.T || k == Keys.X)
            {
                consoleLine += k.ToString();
            }
        }

        public void execute(String s)
        {
            if (String.Compare(s, "BACKGROUND RED") == 0)
            {
                backgroundColor = Color.Red;
            } else if (String.Compare(s, "BACKGROUND BLUE") == 0)
            {
                backgroundColor = Color.Blue;
            } else if (String.Compare(s, "BACKGROUND GREEN") == 0)
            {
                backgroundColor = Color.Green;
            } else if (String.Compare(s, "BRICK ADD") == 0)
            {
                if (bricksHigh < 10)
                    bricksHigh++;
            } else if (String.Compare(s, "BRICK MINUS") == 0)
            {
                if (bricksHigh > 1)
                    bricksHigh--;
            }
        }
                
        protected override void Draw(GameTime gameTime)
        {
            if (gameMode == MODE.game)
            {
                GraphicsDevice.Clear(backgroundColor);

                spriteBatch.Begin();

                foreach (Brick b in brickArray)
                {
                    b.Draw(spriteBatch);
                }

                player.Draw(spriteBatch);
                ball.Draw(spriteBatch);
                updateScore();

                spriteBatch.End();
            } else if (gameMode == MODE.console)
            {
                GraphicsDevice.Clear(backgroundColor);

                spriteBatch.Begin();

                consoleDraw();

            } else if (gameMode == MODE.paused)
            {
                GraphicsDevice.Clear(backgroundColor);
                int posy = 100;
                spriteBatch.Begin();
                spriteBatch.DrawString(font, "HIGHSCORES", new Vector2(640 - 60, posy), Color.Black);
                for(int i = 0; i < 10; i++)
                {
                    int highscore = myUtil.highscores[i];
                    posy += 30;
                    spriteBatch.DrawString(font, ""+highscore, new Vector2(640-20, posy), Color.Black);
                    if (i+1 >= myUtil.highscores.Count)
                    {
                        break;
                    }
                }
                spriteBatch.End();
            }
            base.Draw(gameTime);
        }

        protected void updateScore()
        {
            spriteBatch.DrawString(font, "Score: "+score, new Vector2(0, 0), Color.Black);
        }

        protected void consoleDraw()
        {
            spriteBatch.DrawString(font, consoleLine, new Vector2(0, 0), Color.Black);
        }
    }
}
