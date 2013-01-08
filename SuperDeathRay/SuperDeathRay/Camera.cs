using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SVDU
{
    class Camera
    {
        Player p;
        Vector2 bounds;
        static public float zoomout = 1;

        public Camera(Player _p, Vector2 _bounds) { p = _p; bounds = _bounds; }

        public Vector2 getOffset()
        {
            Vector2 center = p.getCenter();

            Vector2 origin = center - new Vector2(1280/2*zoomout, 720/2*zoomout);

            origin.X = MathHelper.Clamp(origin.X, 0, bounds.X - 1280*zoomout);
            origin.Y = MathHelper.Clamp(origin.Y, 0, bounds.Y - 720*zoomout);

            return origin;
        }
        public float getzoomout() { return zoomout; }
    }
}
