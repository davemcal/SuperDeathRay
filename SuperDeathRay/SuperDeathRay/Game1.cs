using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace SVDU
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont scoreFont, announceFont;
        Player player;
        Camera c;
        Song bgmusic;
        SoundEffect hurtsound, lasersound, punchsound, shotsound;
        float d;
        int objectCount;
        int gamestate = 0;

        bool prevA = false, prevStart = false;

        public static float startTime;

        Random r = new Random();

        Dictionary<string, Texture2D> tex_dict = new Dictionary<string,Texture2D>();

        List<Object> objs;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 720;
            graphics.PreferredBackBufferWidth = 1280;
            //graphics.ToggleFullScreen();
            Content.RootDirectory = "Content";
            Window.Title = "Super Death Ray";

            Mouse.WindowHandle = Window.Handle;
        }

        Object createObject(int x, int y)
        {
            Object p = new Object();
            //p.offset = new Vector2(50, 0);
            p.addPoint(new Vector2(50, 0));
            p.addPoint(new Vector2(100, 0));
            p.addPoint(new Vector2(150, 50));
            p.addPoint(new Vector2(150, 100));
            p.addPoint(new Vector2(100, 150));
            p.addPoint(new Vector2(50, 150));
            p.addPoint(new Vector2(0, 100));
            p.addPoint(new Vector2(0, 50));
            p.translate(new Vector2(x, y));

            if (tex_dict.ContainsKey("circle3"))
                p.tex = tex_dict["circle3"];

            return p;
        }

        Enemy createEnemy(int x, int y, int z)
        {
            Enemy e = new Enemy(player);
            e.tex = tex_dict["dog"];
            if (z < 3)
            {
                e = new Ranged(player);
                e.tex = tex_dict["police"];
                e.addPoint(new Vector2(0, 7));
                e.addPoint(new Vector2(64, 7));
                e.addPoint(new Vector2(64, 37));
                e.addPoint(new Vector2(0, 37));
                e.translate(new Vector2(x + 50, y + 50));
            }
            else
            {
                e.addPoint(new Vector2(16, 0));
                e.addPoint(new Vector2(45, 0));
                e.addPoint(new Vector2(45, 60));
                e.addPoint(new Vector2(0, 60));
                e.translate(new Vector2(x + 50, y + 50));


            }
            //e.offset = new Vector2(21, 0);
            /*e.addPoint(new Vector2(21, 0));
            e.addPoint(new Vector2(42, 0));
            e.addPoint(new Vector2(64, 21));
            e.addPoint(new Vector2(64, 42));
            e.addPoint(new Vector2(42, 64));
            e.addPoint(new Vector2(21, 64));
            e.addPoint(new Vector2(0, 42));
            e.addPoint(new Vector2(0, 21));
            e.translate(new Vector2(x+50, y+50));*/

            

            return e;
        }

        public void generateRandomMap()
        {
            objs = new List<Object>();

            //make map
            List<Vector2> list = new List<Vector2>();
            int numObj = 50;
            int r1 = r.Next(1000), r2 = r.Next(1000);
            for (int h = 0; h < 3; h++)
            {
                for (int g = 0; g < 3; g++)
                {
                    for (int i = 0; i < numObj / 9; i++)
                    {
                        for (int j = 0; j < list.Count; j++)
                        {
                            if ((r1 < list[j].X + 150 && r1 > list[j].X - 150) && (r2 < list[j].Y + 150 && r2 > list[j].Y - 150))
                            {
                                r1 = r.Next(1000);
                                r2 = r.Next(1000);
                                j = -1;
                                continue;
                            }
                        }
                        list.Add(new Vector2(r1, r2));
                        objs.Add(createObject(r1 + 1000 * h, r2 + 1000 * g));
                    }
                    list.Clear();
                }
            }

            //make outer bounds
            Object wall = new Object();
            wall.tex = new Texture2D(GraphicsDevice, 1, 1);
            //wall.offset = new Vector2(0, 0);

            //left
            wall.addPoint(new Vector2(-50, -50));
            wall.addPoint(new Vector2(0, -50));
            wall.addPoint(new Vector2(0, 3050));
            wall.addPoint(new Vector2(-50, 3050));
            objs.Add(new Object(wall));

            //top
            wall.clearPoints();
            wall.addPoint(new Vector2(-50, -50));
            wall.addPoint(new Vector2(3050, -50));
            wall.addPoint(new Vector2(3050, 0));
            wall.addPoint(new Vector2(-50, 0));
            objs.Add(new Object(wall));

            //right
            wall.clearPoints();
            wall.addPoint(new Vector2(3050, -50));
            wall.addPoint(new Vector2(3050, 3050));
            wall.addPoint(new Vector2(3000, 3050));
            wall.addPoint(new Vector2(3000, -50));
            objs.Add(new Object(wall));

            //bottom
            wall.clearPoints();
            wall.addPoint(new Vector2(3050, 3050));
            wall.addPoint(new Vector2(-50, 3050));
            wall.addPoint(new Vector2(-50, 3000));
            wall.addPoint(new Vector2(3050, 3000));
            objs.Add(new Object(wall));
        }

        public void loadLevel()
        {
            objs = new List<Object>();

            Score.reset();

            StreamReader SR;

            SR = File.OpenText("level1.txt");
            string S = SR.ReadLine();
            while (S != null)
            {
                string tex = S;
                int x = Int32.Parse(SR.ReadLine());
                int y = Int32.Parse(SR.ReadLine());

                Object o = loadObject(tex);
                o.translate(new Vector2(x, y));

                objs.Add(o);

                S = SR.ReadLine();
            }

            SR.Close();

            //add player
            player = new Player();
            //player.offset = new Vector2(21, 0);
            /*player.addPoint(new Vector2(21, 0));
            player.addPoint(new Vector2(42, 0));
            player.addPoint(new Vector2(64, 21));
            player.addPoint(new Vector2(64, 42));
            player.addPoint(new Vector2(42, 64));
            player.addPoint(new Vector2(21, 64));
            player.addPoint(new Vector2(0, 42));
            player.addPoint(new Vector2(0, 21));*/
            player.addPoint(new Vector2(0, 0));
            player.addPoint(new Vector2(64, 0));
            player.addPoint(new Vector2(64, 54));
            player.addPoint(new Vector2(0, 54));
            player.translate(new Vector2(2800, 2800));
            objs.Add(player);

            player.tex = tex_dict["character"];


            c = new Camera(player, new Vector2(3000, 3000));

            objectCount = objs.Count;

            //make enemies
            for (int i = 0; i < 20; i++) objs.Add(createEnemy(40+r.Next(2900), r.Next(2900), r.Next(10)));

        }

        Object loadObject(string tex)
        {
            Object o = new Object();

            o.tex = tex_dict[tex];

            if (tex == "table")
            {
                o.addPoint(new Vector2(0, 0));
                o.addPoint(new Vector2(64, 0));
                o.addPoint(new Vector2(64, 128));
                o.addPoint(new Vector2(0, 128));
            }
            if (tex == "bench")
            {
                o.addPoint(new Vector2(20, 2));
                o.addPoint(new Vector2(47, 10));
                o.addPoint(new Vector2(47, 50));
                o.addPoint(new Vector2(20, 50));
            }
            if (tex == "tree")
            {
                /*o.addPoint(new Vector2(30, 9));
                o.addPoint(new Vector2(96, 13));
                o.addPoint(new Vector2(122, 44));
                o.addPoint(new Vector2(122, 95));
                o.addPoint(new Vector2(67, 124));
                o.addPoint(new Vector2(17, 106));
                o.addPoint(new Vector2(7, 41));*/

                o.addPoint(new Vector2(0, 0));
                o.addPoint(new Vector2(128, 0));
                o.addPoint(new Vector2(128, 128));
                o.addPoint(new Vector2(0, 128));
            }
            if (tex == "wall_horz_288")
            {
                o.addPoint(new Vector2(0, 0));
                o.addPoint(new Vector2(288, 0));
                o.addPoint(new Vector2(288, 16));
                o.addPoint(new Vector2(0, 16));
            }
            if (tex == "wall_horz_32")
            {
                o.addPoint(new Vector2(0, 0));
                o.addPoint(new Vector2(32, 0));
                o.addPoint(new Vector2(32, 16));
                o.addPoint(new Vector2(0, 16));
            }
            if (tex == "wall_horz_864")
            {
                o.addPoint(new Vector2(0, 0));
                o.addPoint(new Vector2(864, 0));
                o.addPoint(new Vector2(864, 16));
                o.addPoint(new Vector2(0, 16));
            }
            if (tex == "wall_horz_96")
            {
                o.addPoint(new Vector2(0, 0));
                o.addPoint(new Vector2(96, 0));
                o.addPoint(new Vector2(96, 16));
                o.addPoint(new Vector2(0, 16));
            }
            if (tex == "wall_vert_288")
            {
                o.addPoint(new Vector2(0, 0));
                o.addPoint(new Vector2(16, 0));
                o.addPoint(new Vector2(16, 288));
                o.addPoint(new Vector2(0, 288));
            }
            if (tex == "wall_vert_32")
            {
                o.addPoint(new Vector2(0, 0));
                o.addPoint(new Vector2(16, 0));
                o.addPoint(new Vector2(16, 32));
                o.addPoint(new Vector2(0, 32));
            }
            if (tex == "wall_vert_864")
            {
                o.addPoint(new Vector2(0, 0));
                o.addPoint(new Vector2(16, 0));
                o.addPoint(new Vector2(16, 864));
                o.addPoint(new Vector2(0, 864));
            }
            if (tex == "wall_vert_96")
            {
                o.addPoint(new Vector2(0, 0));
                o.addPoint(new Vector2(16, 0));
                o.addPoint(new Vector2(16, 96));
                o.addPoint(new Vector2(0, 96));
            }

            return o;
        }


        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        /// 
        protected override void Initialize()
        {
            
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

            Bullet.bullet_tex = Content.Load<Texture2D>("laser2");
            Bullet.police_tex = Content.Load<Texture2D>("p_bullet");

            tex_dict["circle"] = Content.Load<Texture2D>("circle");
            tex_dict["circle3"] = Content.Load<Texture2D>("circle3");
            tex_dict["health"] = Content.Load<Texture2D>("health");
            tex_dict["energy"] = Content.Load<Texture2D>("energy");
            tex_dict["health_bg"] = Content.Load<Texture2D>("health_bg");
            tex_dict["character"] = Content.Load<Texture2D>("character");
            tex_dict["tree"] = Content.Load<Texture2D>("tree");
            tex_dict["bench"] = Content.Load<Texture2D>("bench");
            tex_dict["table"] = Content.Load<Texture2D>("table");
            tex_dict["wall_horz_288"] = Content.Load<Texture2D>("wall_horz_288");
            tex_dict["wall_horz_32"] = Content.Load<Texture2D>("wall_horz_32");
            tex_dict["wall_horz_864"] = Content.Load<Texture2D>("wall_horz_864");
            tex_dict["wall_horz_96"] = Content.Load<Texture2D>("wall_horz_96");
            tex_dict["wall_vert_288"] = Content.Load<Texture2D>("wall_vert_288");
            tex_dict["wall_vert_32"] = Content.Load<Texture2D>("wall_vert_32");
            tex_dict["wall_vert_864"] = Content.Load<Texture2D>("wall_vert_864");
            tex_dict["wall_vert_96"] = Content.Load<Texture2D>("wall_vert_96");
            tex_dict["police"] = Content.Load<Texture2D>("police");
            tex_dict["dog"] = Content.Load<Texture2D>("dog");
            tex_dict["title"] = Content.Load<Texture2D>("title");
            tex_dict["instructions"] = Content.Load<Texture2D>("instructions");
            tex_dict["grass"] = Content.Load<Texture2D>("grass");
            tex_dict["crosshairs"] = Content.Load<Texture2D>("crosshairs");

            bgmusic = Content.Load<Song>("Phat Beat");
            MediaPlayer.Play(bgmusic);
            MediaPlayer.IsRepeating = true;
            hurtsound = Content.Load<SoundEffect>("hurt");
            lasersound = Content.Load<SoundEffect>("laser");
            punchsound = Content.Load<SoundEffect>("punch");
            shotsound = Content.Load<SoundEffect>("shot");

            scoreFont = Content.Load<SpriteFont>("scorefont");
            announceFont = Content.Load<SpriteFont>("announcefont");

            loadLevel();

            
        }


        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
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
        float cooldown = 0;
        char currchar = 'A';
        bool prevB;
        string name = "";
        List<String> names = new List<String>();
        List<int> scores = new List<int>();


        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back ==
                ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            if (gamestate == 0)
            {
                if ((GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed || Mouse.GetState().LeftButton == ButtonState.Pressed) && !prevA)
                {
                    gamestate = 1;
                }

                prevA = GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed || Mouse.GetState().LeftButton == ButtonState.Pressed;
                return;
            }
            if (gamestate == 1)
            {
                if ((GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed || Mouse.GetState().LeftButton == ButtonState.Pressed) && !prevA)
                {
                    gamestate = 2;
                    loadLevel();
                    startTime = (float)gameTime.TotalGameTime.TotalSeconds;
                }

                prevA = GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed || Mouse.GetState().LeftButton == ButtonState.Pressed;
                return;
            }

            String alph = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            if (gamestate == 3)
            {
                if ((GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y > .75 || Keyboard.GetState().IsKeyDown(Keys.Down)) && cooldown > 0.1)
                {
                    //dec 1
                    int k = (alph.IndexOf(currchar) - 1) % 26;
                    if (k == -1) k = 25;
                    currchar = alph.ToCharArray()[k];
                    cooldown = 0;
                }
                else if ((GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y < -.75 || Keyboard.GetState().IsKeyDown(Keys.Up)) && cooldown > 0.1)
                {
                    //inc 1
                    currchar = alph.ToCharArray()[(alph.IndexOf(currchar) + 1) % 26];
                    cooldown = 0;
                }
                if ((GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed || Mouse.GetState().LeftButton == ButtonState.Pressed) && !prevA && name.Length < 8)
                {
                    name = name + currchar;
                    currchar = 'A';
                }
                if ((GamePad.GetState(PlayerIndex.One).Buttons.B == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.X)) && !prevB && name.Length > 0)
                {
                    currchar = name.ToCharArray().Last();
                    name = name.Substring(0, name.Length - 1);
                }
                if ((GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Space)) && prevStart == false)
                {

                    objs.Clear();

                    StreamReader SR;
                    SR = File.OpenText("score.txt");
                    for (int i = 0; i < 5; i++) names.Add(SR.ReadLine());
                    for (int i = 0; i < 5; i++) scores.Add(Int32.Parse(SR.ReadLine()));
                    SR.Close();
                    int a = 0;

                    while (Score.score < scores[a])
                    {
                        a++;
                        if (a == scores.Count) break;
                    }
                    scores.Insert(a, Score.score);
                    names.Insert(a, name);

                    StreamWriter SW = new StreamWriter("score.txt");
                    for (int i = 0; i < 5; i++) SW.WriteLine(names[i]);
                    for (int i = 0; i < 5; i++) SW.WriteLine(scores[i]);
                    SW.Close();
                    gamestate = 4;
                    prevStart = GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Space);
                    name = "";
                    Score.reset();
                    return;
                }

                //fixed chars
                //if ((int)currchar == 41) currchar = (char) 42;
                //else if ((int)currchar == 68) currchar = (char) 67;

                cooldown += (float)gameTime.ElapsedGameTime.TotalSeconds;
                prevA = GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed || Mouse.GetState().LeftButton == ButtonState.Pressed;
                prevB = GamePad.GetState(PlayerIndex.One).Buttons.B == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.X);
                prevStart = GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Space);

                return;
            }
            else if (gamestate == 4)
            {
                if ((GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Space)) && !prevStart)
                {
                    objs.Clear();
                    gamestate = 0;
                    prevA = GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed || Mouse.GetState().LeftButton == ButtonState.Pressed;
                 
                }
                prevStart = GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Space);
                return;
            }

            prevStart = GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Space);


            if ((GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Space)))
            {
                gamestate = 0;
                prevA = GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed || Mouse.GetState().LeftButton == ButtonState.Pressed;
                return;
            }

            if ((GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed || Mouse.GetState().RightButton == ButtonState.Pressed) && !prevA && player.isAlive())
            {
                loadLevel();
                startTime = (float)gameTime.TotalGameTime.TotalSeconds;
            }

            if ((GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed || Mouse.GetState().LeftButton == ButtonState.Pressed) && !prevA && !player.isAlive())
            {
                gamestate = 3;
            }

            prevA = GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed || Mouse.GetState().LeftButton == ButtonState.Pressed;
                 
            if (GamePad.GetState(PlayerIndex.One).Buttons.Y == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.C)) Camera.zoomout = Math.Min(Camera.zoomout * 1.01F, 1.2F);
            if (GamePad.GetState(PlayerIndex.One).Buttons.X == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.V)) Camera.zoomout = Math.Max(0.5F, Camera.zoomout / 1.01F);

            if (Camera.zoomout > 1) 
                Camera.zoomout *= 0.999F;

            player.vel = gameTime.ElapsedGameTime.Ticks * GamePad.GetState(PlayerIndex.One).ThumbSticks.Left;
            if (true) //!GamePad.GetState(PlayerIndex.One).IsConnected)
            {
                int y_val = (Keyboard.GetState().IsKeyDown(Keys.Up) ? 1 : 0) - (Keyboard.GetState().IsKeyDown(Keys.Down) ? 1 : 0);
                int x_val = (Keyboard.GetState().IsKeyDown(Keys.Right) ? 1 : 0) - (Keyboard.GetState().IsKeyDown(Keys.Left) ? 1 : 0);
                player.vel = gameTime.ElapsedGameTime.Ticks * new Vector2(x_val, y_val);
            }

            Vector2 stick = GamePad.GetState(PlayerIndex.One).ThumbSticks.Right;

            if (stick != new Vector2(0, 0))
                player.rotation = -(float)(Math.Atan2(GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.Y, GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.X) - Math.PI / 2);
            else
            {
                Vector2 p_center = (player.getCenter() - c.getOffset()) / c.getzoomout();

                float y_val = Mouse.GetState().Y - p_center.Y;
                float x_val = Mouse.GetState().X - p_center.X;
                player.rotation = (float)(Math.Atan2(y_val, x_val) + Math.PI / 2);
            }

            if (GamePad.GetState(PlayerIndex.One).Triggers.Right > 0.1 || Mouse.GetState().LeftButton == ButtonState.Pressed) player.fire();
            else player.release();

            // Move the sprite around.
            UpdateSprites(gameTime);

            base.Update(gameTime);
        }

        void UpdateSprites(GameTime gameTime)
        {
            List<Object> newObjs = new List<Object>();

            if (r.Next(100) == 1 && (objs.Count - objectCount) < 60) newObjs.Add(createEnemy(r.Next(2900), r.Next(2900), r.Next(10)));

            // Move the sprite by speed, scaled by elapsed time.
            for (int i = 0; i < objs.Count; i++)
            {
                newObjs.AddRange(objs[i].update(gameTime));

                if (objs[i] is Player) d = 0.8F;
                else d = 1 / (float)Math.Pow((player.getCenter() - objs[i].getCenter()).Length(), 0.33);

                if (objs[i].playFire)
                {
                    if (objs[i] is Player) lasersound.Play(0.8F, 0, 0);
                    else shotsound.Play(d,0,0);
                    
                    objs[i].playFire = false;
                }
                if (objs[i].playHurt)
                {
                    hurtsound.Play(d, 0, 0);
                    objs[i].playHurt = false;
                }
                if (objs[i].playPunch)
                {
                if (objs[i] is Player) d = 0.8F;
                else d = 1 / (float) Math.Pow((player.getCenter() - objs[i].getCenter()).Length(), 0.33);
                    punchsound.Play(0.6F, 0, 0);
                    objs[i].playPunch = false;
                }
            }

            for (int i = 0; i < objs.Count; i++)
            {
                if (!objs[i].canMove()) continue;
                for (int j = 0; j < objs.Count; j++)
                {
                    if (i == j) continue;
                    objs[i].collide(objs[j]);
                }
            }

            objs.RemoveAll(o => !o.isAlive());
            objs.AddRange(newObjs);
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(new Color(20 , 100, 0));

            // Draw the sprite.
            //spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            if (gamestate == 0)
            {
                spriteBatch.Draw(tex_dict["title"], new Vector2(0, 0), Color.White);
            }
            else if (gamestate == 1)
            {
                spriteBatch.Draw(tex_dict["instructions"], new Vector2(0, 0), Color.White);
            }
            else if (gamestate == 3)
            {
                graphics.GraphicsDevice.Clear(Color.Black);
                if (((int)gameTime.TotalGameTime.TotalSeconds * 2) % 2 == 0)
                {
                    spriteBatch.DrawString(scoreFont, name + currchar, new Vector2(500, 300), Color.White);
                }
                else
                {
                    spriteBatch.DrawString(scoreFont, name, new Vector2(500, 300), Color.White);
                }
            }
            else if (gamestate == 4)
            {
                graphics.GraphicsDevice.Clear(Color.Black);
                StreamReader SR;

                SR = File.OpenText("score.txt");
                for (int i = 0; i < 5; i++) names[i] = SR.ReadLine();

                spriteBatch.DrawString(scoreFont, names[0] + "\n" + names[1] + "\n" + names[2] + "\n" + names[3] + "\n" + names[4], new Vector2(300, 200), Color.White);

                string a = SR.ReadLine();
                string b = SR.ReadLine();
                string c = SR.ReadLine();
                string d = SR.ReadLine();
                string e = SR.ReadLine();


                spriteBatch.DrawString(announceFont, "Highscores", new Vector2(1280 / 2 - (announceFont.MeasureString("Highscores").X / 2), 50), Color.White);
                spriteBatch.DrawString(scoreFont, String.Format("{0:0000000000}\n{1:0000000000}\n{2:0000000000}\n{3:0000000000}\n{4:0000000000}", a, b, c, d, e), new Vector2(800, 200), Color.White);
                SR.Close();

            }
            else
            {
                for (int y = 360; y < 3360; y += 720)
                {
                    for (int x = 1280 / 2; x < 3640; x += 1280)
                    {
                        spriteBatch.Draw(tex_dict["grass"], (new Vector2(x, y) - c.getOffset()) / c.getzoomout(), null, Color.White, 0,
                            new Vector2(tex_dict["grass"].Width / 2, tex_dict["grass"].Height / 2), 1.0F / c.getzoomout(), SpriteEffects.None, 0);

                    }
                }
                for (int i = 0; i < objs.Count; i++)
                {
                    objs[i].Draw(spriteBatch, c);
                }

                spriteBatch.Draw(tex_dict["health_bg"], new Vector2(198, 678), null, Color.White, 0, new Vector2(0, 0), new Vector2(804, 14), SpriteEffects.None, 0);
                spriteBatch.Draw(tex_dict["health"], new Vector2(200, 680), null, Color.White, 0, new Vector2(0, 0), new Vector2(player.health * 80, 10), SpriteEffects.None, 0);

                spriteBatch.Draw(tex_dict["health_bg"], new Vector2(98, 38), null, Color.White, 0, new Vector2(0, 0), new Vector2(1004, 24), SpriteEffects.None, 0);
                spriteBatch.Draw(tex_dict["energy"], new Vector2(100, 40), null, Color.White, 0, new Vector2(0, 0), new Vector2(player.energy * 1000, 20), SpriteEffects.None, 0);

                spriteBatch.DrawString(scoreFont, "Score: " + Score.score, new Vector2(0, 60), Color.White);
                spriteBatch.DrawString(scoreFont, "Combo: " + Score.miss_combo, new Vector2(1000, 60), Color.White);
                //spriteBatch.DrawString(scoreFont, "Objects: " + (objs.Count - objectCount) + "\nTime: " + (int)(gameTime.TotalGameTime.TotalMinutes - startTime / 60) + ":" + String.Format("{0:00}", ((int)gameTime.TotalGameTime.TotalSeconds - startTime) % 60), new Vector2(0, 60), Color.White);

                if (player == null || !player.isAlive())
                {
                    spriteBatch.DrawString(announceFont, "YOU ARE DEAD", new Vector2(400, 250), Color.White);
                    spriteBatch.DrawString(announceFont, "Score: " + Score.score, new Vector2(400, 310), Color.White);
                }
            }

            spriteBatch.Draw(tex_dict["crosshairs"], new Vector2(Mouse.GetState().X, Mouse.GetState().Y), null, Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }



    }
}
