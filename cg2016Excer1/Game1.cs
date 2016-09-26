using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace cg2016Excer1
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        List<PlayerData> list;
        Random ran = new Random();

        //Player player;
        Player playerOne = new Player();
        Vector2 origin;
        Vector2 scale;
        Vector2 center;
        Vector2 mousePos;
        Vector2 direction;
        List<Collections> coins = new List<Collections>();
        Random rand = new Random();
        float rotation;
        protected float speed = 4.5f;
        bool gameOn = false;
        TimeSpan timeElapsedSinceLastEnterPress;
        TimeSpan currentElapsedGametime;
        public SpriteFont font;
        public string myPlayerInfo;
        public PlayerData randomPlayerInfo;
        public Vector2 fontPos;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }


        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            graphics.PreferredBackBufferWidth = 1050;
            graphics.PreferredBackBufferHeight = 650;
            graphics.ApplyChanges();

            IsMouseVisible = true;
            base.Initialize();
            list = GetScores(10);
            randomPlayerInfo = makePlayer();
            myPlayerInfo = randomPlayerInfo.FirstName + " " + randomPlayerInfo.SecondName
                + " {" + randomPlayerInfo.GamerTagScore + "} "
              ;
            playerOne.score = randomPlayerInfo.topscore;
        }

        private PlayerData makePlayer()
        {
            Random rand = new Random();
            PlayerData myPlayer = new PlayerData();

            using (TestDbContext db = new TestDbContext())
            {
                int randomIDFromList = rand.Next(db.ScoreBoard.Count);
                myPlayer = db.ScoreBoard.ToList().ElementAt(randomIDFromList);
            }
            return myPlayer;
        }


        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>("Font");
            fontPos = new Vector2(GraphicsDevice.Viewport.Width / 2,
                GraphicsDevice.Viewport.Height / 2);
            playerOne._skin = Content.Load<Texture2D>("Player");
            CreateSprites();
            playerOne.position = new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2);
            timeElapsedSinceLastEnterPress = TimeSpan.Zero;
        }

        private void CreateSprites()
        {
            for (int i = 0; i < 12; i++)
            {
                //sprites.Add(PickRandomPosition());
                coins.Add(new Collections(playerOne._skin, PickRandomPosition(), PickRandomPosition(), GetRamdomValue()));
            }
        }

        private int GetRamdomValue()
        {
            int i = ran.Next(10, 100);
            return i;
        }

        private Vector2 PickRandomPosition()
        {
            return new Vector2(rand.Next(0, GraphicsDevice.Viewport.Width), rand.Next(0, GraphicsDevice.Viewport.Height));
        }
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }


        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            timeElapsedSinceLastEnterPress = gameTime.ElapsedGameTime + timeElapsedSinceLastEnterPress;

            if (Keyboard.GetState().IsKeyDown(Keys.A))
                playerOne.position += new Vector2(-1, 0) * speed;
            if (Keyboard.GetState().IsKeyDown(Keys.D))
                playerOne.position += new Vector2(1, 0) * speed;
            if (Keyboard.GetState().IsKeyDown(Keys.S))
                playerOne.position += new Vector2(0, 1) * speed;
            if (Keyboard.GetState().IsKeyDown(Keys.W))
                playerOne.position += new Vector2(0, -1) * speed;

            currentElapsedGametime = currentElapsedGametime - gameTime.ElapsedGameTime;

            if(currentElapsedGametime <= TimeSpan.Zero)
            {
                gameOn = false;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Enter) && timeElapsedSinceLastEnterPress.TotalSeconds > 1)
            {
                timeElapsedSinceLastEnterPress = TimeSpan.Zero;
                if (gameOn)
                {
                    gameOn = false;

                }
                else if (!gameOn)
                {

                    gameOn = true;
                    gameTime.ElapsedGameTime = TimeSpan.Zero;
                    currentElapsedGametime = new TimeSpan(0,1,20);

                }
            }

            CollectionsMovement();
            CollectionsDeath();

            playerOne.position = Vector2.Clamp(playerOne.position, Vector2.Zero,
                    GraphicsDevice.Viewport.Bounds.Size.ToVector2() -
                    new Vector2(playerOne._skin.Width / 2,
                    playerOne._skin.Height / 2));
            //mousePos = new Vector2(MouseState.X, Mouse.GetState.Y);

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        private void CollectionsDeath()
        {
            foreach (var item in coins)
            {
                if (!item.visible)
                {
                    item.curPosition = PickRandomPosition();
                    playerOne.score = playerOne.score + item.value;
                    item.visible = true;
                }
            }
        }

        private void CollectionsMovement()
        {
            foreach (var item in coins)
            {
                if (item.curPosition != item.tarPosition)
                {

                    Vector2 direction = item.tarPosition - item.curPosition;
                    direction.Normalize();
                    item.curPosition += direction * speed;
                    if (Vector2.DistanceSquared(item.curPosition, item.tarPosition) <
                            Vector2.DistanceSquared(item.curPosition, item.curPosition - direction * speed))
                    {
                        item.curPosition = item.tarPosition;

                    }
                }
                else
                {
                    item.tarPosition = PickRandomPosition();
                }
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.HotPink);
            spriteBatch.Begin();
            if (gameOn)
            {
                DrawPlayerScore();
                DrawTime();
                DrawPlayer();
                DrawDimonds();
            }
            if (!gameOn)
            {
                DrawTopPlayers();
            }
            MapVectorToMouse(ref center);
            spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        private void DrawTime()
        {
            string output = "Time remaining: " + currentElapsedGametime.Minutes.ToString() + ":" + currentElapsedGametime.Seconds.ToString(); 
            Vector2 fontOrigin = font.MeasureString(output);
            fontPos.Y = 40;
            spriteBatch.DrawString(font, output, fontPos, Color.BlueViolet, 0, fontOrigin, 1.5f, SpriteEffects.None, 0.5f);
        }

        private void DrawPlayerScore()
        {
            string output = "Player Score: " + playerOne.score.ToString();
            Vector2 fontOrigin = font.MeasureString(output);
            fontPos.Y = 50 + 25;
            spriteBatch.DrawString(font, output, fontPos, Color.BlueViolet, 0, fontOrigin, 1.5f, SpriteEffects.None, 0.5f);
        }

        private void DrawDimonds()
        {

            for (int i = 0; i < coins.Count; i++)
            {
                var state = Mouse.GetState();
                var distance = Vector2.Distance(coins[i].curPosition, center);
                if (distance < 64)
                {
                    spriteBatch.Draw(coins[i].collectionTexture, coins[i].curPosition, null, Color.Red, 0, origin, new Vector2(0.5f), SpriteEffects.None, 0);
                }
                else if (distance >= 64 && distance <= 100)
                {
                    spriteBatch.Draw(coins[i].collectionTexture, coins[i].curPosition, null, Color.Orange, 0, origin, new Vector2(0.5f), SpriteEffects.None, 0);
                }
                else if (distance > 100)
                {
                    spriteBatch.Draw(coins[i].collectionTexture, coins[i].curPosition, null, Color.LawnGreen, 0, origin, new Vector2(0.5f), SpriteEffects.None, 0);
                }
                if (distance < 64 && state.LeftButton == ButtonState.Pressed)
                {
                    coins[i].visible = false;
                }
            }



        }
        private void MapVectorToMouse(ref Vector2 position)
        {
            var state = Mouse.GetState();
            position.X = state.Position.X;
            position.Y = state.Position.Y;
        }
        private void DrawPlayer()
        {
            //Draws the player pointing at the mouse Position
            scale = new Vector2(1, 1);
            origin = new Vector2(playerOne._skin.Width / 2, playerOne._skin.Height / 2);
            MouseState mouse = Mouse.GetState();
            direction = new Vector2(mouse.X, mouse.Y) - playerOne.position;
            rotation = (float)Math.Atan2(direction.Y, direction.X);
            rotation = rotation + (float)(Math.PI * 0.5f);
            spriteBatch.Draw(
                playerOne._skin,
                playerOne.position,
                null,
                Color.White,
                rotation,
                origin,
                scale,
                SpriteEffects.None,
                0);

        }

        private void DrawTopPlayers()
        {
            int count = 1;

            foreach (PlayerData player in GetScores(10))
            {
                string output = player.GamerTag + " 8==D " + player.topscore.ToString();
                Vector2 fontOrigin = font.MeasureString(output) / 2;
                fontPos.Y = 100 + count * 25;
                if (player.GamerTag == randomPlayerInfo.GamerTag)
                {
                    spriteBatch.DrawString(font, output, fontPos, Color.Red, 0, fontOrigin, 1.5f, SpriteEffects.None, 0.5f);

                }
                else
                {
                    spriteBatch.DrawString(font, output, fontPos, Color.Green, 0, fontOrigin, 1.5f, SpriteEffects.None, 0.5f);

                }
                count++;
            }
        }

        private List<PlayerData> GetScores(int count)
        {
            using (TestDbContext db = new TestDbContext())
            {

                return db.ScoreBoard.Take(count).ToList();
            }
        }

    }
}
