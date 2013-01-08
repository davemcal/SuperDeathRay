using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SVDU
{
    class Bullet : Object
    {
        public bool alive, player_fired;
        public static Texture2D bullet_tex;
        public static Texture2D police_tex;

        public override bool canMove() { return true; }

        public override Vector2 getCenter()
        {
            return origin + new Vector2(bullet_tex.Width / 2, bullet_tex.Height / 2);
        }

        public Bullet(Vector2 _vel, Vector2 pos, bool _player_fired)
        {
            vel = _vel;
            alive = true;
            player_fired = _player_fired;

            addPoint(new Vector2(0, 10));
            addPoint(new Vector2(10, 10));
            addPoint(new Vector2(10, 20));
            addPoint(new Vector2(20, 20));
            translate(pos - getCenter());
        }


        public override bool isAlive() { return alive; }

        public override void Draw(SpriteBatch spriteBatch, Camera c)
        {
            if (player_fired)
                spriteBatch.Draw(bullet_tex, (getCenter() - c.getOffset()) / c.getzoomout(), null, Color.White, rotation, new Vector2(bullet_tex.Width / 2, bullet_tex.Height / 2), 1.0F / c.getzoomout(), SpriteEffects.None, 0);
            else
                spriteBatch.Draw(police_tex, (getCenter() - c.getOffset()) / c.getzoomout(), null, Color.White, rotation, new Vector2(bullet_tex.Width / 2, bullet_tex.Height / 2), 1.0F / c.getzoomout(), SpriteEffects.None, 0);
            
        }

        public override void collide(Object o)
        {
            Vector2 v = find_overlap(o);

            if (!v.Equals(new Vector2(0, 0)) && !(o is Bullet))
            {
                if (player_fired)
                {
                    if (o is Player) return;
                    if (o is Enemy)
                        Score.hit();
                    else
                        Score.miss();

                    o.take_damage(1);
                    alive = false;
                    return;
                }

                if (o is Player) o.take_damage(1);
                alive = false;
                
            }

        }
    }
}
