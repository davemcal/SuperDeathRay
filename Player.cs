using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SVDU
{
    class Player : Object
    {
        bool firing = false;
        double fire_cooldown = 0;
        public int health = 10;
        bool alive = true;
        public float energy = .50f;
        public float speed;

        public override bool canMove() { return true; }

        public override bool isAlive()
        {
            return alive;
        }

        public override List<Object> update(GameTime gameTime)
        {
           

            translate(vel * (float)gameTime.ElapsedGameTime.TotalSeconds * .001F * new Vector2(1,-1) * speed);

            if (energy >= 1) alive = false;
             
            energy += (float)(gameTime.ElapsedGameTime.TotalSeconds*MathHelper.Min((float)(( gameTime.TotalGameTime.TotalSeconds - Game1.startTime)/200.0), 0.5F));

            energy = MathHelper.Clamp(energy, 0, 1);

            List<Object> bullets = new List<Object>();

            if (firing)
            {
                float b_rot = rotation - (float)Math.PI / 2;

                Vector2 bullet_dir = (new Vector2((float)Math.Cos(b_rot), (float)Math.Sin(b_rot)));

                Bullet b = new Bullet(bullet_dir * 500, getCenter() + bullet_dir * tex.Width / 5, true);
                b.rotation = b_rot;
                bullets.Add(b);
                firing = false;
                energy -= 0.15f;
                playFire = true;
                
            }

            fire_cooldown -= gameTime.ElapsedGameTime.TotalSeconds;

            return bullets;
        }

        public void fire()
        {
            if (fire_cooldown <= 0 && energy >= 0.15)
            {
                firing = true;
                fire_cooldown = 0.3;
            }
            speed = 0.75F;
        }

        public void release() { if (fire_cooldown < 0) speed = 1.5F; }

        public override void collide(Object o)
        {
            Vector2 v = find_overlap(o);

            if (!v.Equals(new Vector2(0, 0)))
            {
                if (o is Bullet && !((Bullet) o).player_fired)
                {
                    take_damage(1);
                    ((Bullet)o).alive = false;
                }
            }

            if (!(o is Bullet))
            {
                if (o.canMove())
                {
                    translate(-v / 2);
                    o.translate(v / 2);
                }
                else
                {
                    translate(-v);
                }

            }
        }
        public override void Draw(SpriteBatch spriteBatch, Camera c)
        {

            spriteBatch.Draw(tex, (getCenter() - c.getOffset()) / c.getzoomout(), null, Color.White, rotation, new Vector2(tex.Width / 2, tex.Height / 2), 1.0F / c.getzoomout(), SpriteEffects.None, 0);
        }
        public override void take_damage(int damage)
        {
            health -= damage;
            if (health <= 0) alive = false;
        }
    }
}
