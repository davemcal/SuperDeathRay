using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SVDU
{
    class Object
    {
        public bool playFire = false;
        public bool playHurt = false;
        public bool playPunch = false;
        protected List<Vector2> points = new List<Vector2>();
        public Texture2D tex;
        public Vector2 vel;
        public Vector2 origin = new Vector2(0,0);
        public  float rotation = 0;// cos = 1, sin = 0;
        /*public float rotation
        {
            get { return Rotation; }
            set { Rotation = value; cos = (float)Math.Cos(value); sin = (float)Math.Sin(value); }
        }*/

        public Object() { }

        public Object(Object o)
        {
            points = new List<Vector2>(o.points);
            tex = o.tex;
        }

        public virtual bool isAlive() { return true; }

        public virtual List<Object> update(GameTime gameTime)
        {
            translate(vel * (float)gameTime.ElapsedGameTime.TotalSeconds);
            return new List<Object>();
        }

        public virtual Vector2 getCenter()
        {
            return origin + new Vector2(tex.Width / 2, tex.Height / 2);
        }

        public virtual void Draw(SpriteBatch spriteBatch, Camera c)
        {

            spriteBatch.Draw(tex, (getCenter() - c.getOffset()) / c.getzoomout(), null, Color.White, rotation, new Vector2(tex.Width / 2, tex.Height / 2), 1.0F/c.getzoomout(), SpriteEffects.None, 0);
        }

        public void addPoint(Vector2 p)
        {
            points.Add(p);
        }

        public Vector2 getPoint(int i)
        {
            return points[i] + origin;

            Vector2 v = points[i] - new Vector2(tex.Width / 2, tex.Height / 2);

            //double r = rotation;
            //float cos = (float)Math.Cos(r);
            //float sin = (float)Math.Sin(r);

            //v.X = v.X * cos - v.Y * sin;
            //v.Y = v.X * sin + v.Y * cos;

            return v + new Vector2(tex.Width / 2, tex.Height / 2) +origin;
        }

        public virtual bool canMove() { return false; }

        public void translate(Vector2 v){

            origin += v;

            /*
            for (int i = 0; i < points.Count; i++)
            {
                points[i] += v;
            }*/
        }

        Vector2 project(Vector2 v) {
            Vector2 p = new Vector2(getPoint(0).X * v.X + getPoint(0).Y * v.Y, getPoint(0).X * v.X + getPoint(0).Y * v.Y);

            for (int i = 0; i < points.Count; i++)
            {
                float proj = v.X * getPoint(i).X + v.Y * getPoint(i).Y;
                p.X = (proj < p.X) ? proj : p.X;
                p.Y = (proj > p.Y) ? proj : p.Y;
            }

		    return p;
	    }

        delegate bool D(Object obj);

        protected Vector2 find_overlap(Object o)
        {
            Vector2 smallest_overlap = new Vector2(0, 0);
            float ln = float.MaxValue;

            D test_points = (obj) =>
            {
                for (int j = obj.points.Count - 1, i = 0; i < obj.points.Count; j = i, i++)
                {
                    Vector2 edge = obj.getPoint(i) - obj.getPoint(j);
                    Vector2 normal = new Vector2(-1.0f * edge.Y, edge.X);

                    Vector2 p1 = project(normal);
                    Vector2 p2 = o.project(normal);

                    float d0 = p2.Y - p1.X;
                    float d1 = p2.X - p1.Y;

                    //no overlap?
                    if (d0 < 0.0f || d1 > 0.0f) return true;

                    float overlap = (d0 < -d1) ? d0 : d1;
                    float axis_length_squared = normal.LengthSquared();
                    Vector2 sep = new Vector2(normal.X * (overlap / axis_length_squared), normal.Y * (overlap / axis_length_squared));
                    float sep_length_squared = sep.LengthSquared();

                    if (sep_length_squared < ln)
                    {
                        ln = sep_length_squared;
                        smallest_overlap = sep * -1.0f;
                    }
                }
                return false;
            };

		    if (test_points(this)) return new Vector2(0.0f, 0.0f);
		    if (test_points(o)) return new Vector2(0.0f, 0.0f);

		    return smallest_overlap;
        }

        public virtual void collide(Object o)
        {
            Vector2 v = find_overlap(o);

            translate(-v);
        }

        public virtual void take_damage(int damage)
        {

        }

        public void clearPoints() { points.Clear(); }

    }
}
