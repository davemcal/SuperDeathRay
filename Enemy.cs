using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SVDU
{
    class Enemy : Object
    {
        public Player p;
        public bool alive;
        public double fire_cooldown = 0;

        public Enemy() { }

        public Enemy(Player _p)
        {
            p = _p;
            alive = true;
        }

        public override bool isAlive()
        {
            return alive;
        }

        public override bool canMove() { return true; }

        public override List<Object> update(GameTime gameTime)
        {
            vel = p.getCenter() - getCenter();
            vel.Normalize();
            if(fire_cooldown < 0) translate(vel * 125 * (float)gameTime.ElapsedGameTime.TotalSeconds);

            

            rotation = (float)(Math.Atan2(vel.Y, vel.X) + Math.PI / 2);

            List<Object> bullets = new List<Object>();

            if ((p.getCenter() - getCenter()).Length() < 66 && fire_cooldown < 0)
            {
                float b_rot = rotation -(float)Math.PI / 2;

                Vector2 bullet_dir = (new Vector2((float)Math.Cos(b_rot), (float)Math.Sin(b_rot)));

                Bullet b = new Bullet(bullet_dir * 100, getCenter() + bullet_dir * tex.Width, false);
                b.rotation = b_rot;
                bullets.Add(b);
                fire_cooldown = 5;
                playPunch = true;
            }
            fire_cooldown -= gameTime.ElapsedGameTime.TotalSeconds;
            return bullets;
        }

        public override void collide(Object o)
        {
            Vector2 v = find_overlap(o);
            if (!v.Equals(new Vector2(0, 0)))
            {
                if (o is Bullet)
                {
                    if (((Bullet)o).player_fired)
                    {
                        take_damage(1);
                        Score.hit();
                        playHurt = true;
                    }
                    ((Bullet)o).alive = false;
                    return;
                }
            }

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

        public override void take_damage(int damage)
        {
            alive = false;
        }

    }
}
