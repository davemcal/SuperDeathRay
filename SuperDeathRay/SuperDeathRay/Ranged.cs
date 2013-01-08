using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SVDU
{
    class Ranged : Enemy
    {
        static Random rand = new Random();

        public Ranged() { }

        public Ranged(Player _p)
        {
            p = _p;
            alive = true;
        }

        public override List<Object> update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            vel = p.getCenter() - getCenter();

            if (vel.Length() > 500)
            {
                vel.Normalize();
                vel = vel * 50;
                translate(vel * (float)gameTime.ElapsedGameTime.TotalSeconds);
            }

            rotation = (float)(Math.Atan2(vel.Y, vel.X) + Math.PI / 2);

            List<Object> bullets = new List<Object>();

            if (rand.Next(200) == 1 && (p.getCenter() - getCenter()).Length() < 1000)
            {
                float b_rot = rotation -(float)Math.PI / 2;

                Vector2 bullet_dir = (new Vector2((float)Math.Cos(b_rot), (float)Math.Sin(b_rot)));

                Bullet b = new Bullet(bullet_dir * 300, getCenter() + bullet_dir * tex.Width / 1.3f, false);
                b.rotation = b_rot;
                bullets.Add(b);
                playFire = true;
            }

            return bullets;
        }

    }
}
