using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopDownDriver
{
    internal class ObjectPropertyUIBox
    {
        ObjectType objectType;

        Hitbox Boundary;
        Vector2 GrapplePointLocation;
        SpawnPoint spawnPoint;

        public ObjectPropertyUIBox(object Object)
        {
            if (Object == null) 
                throw new ArgumentNullException();
            else if (Object is Hitbox)
            {
                objectType = ObjectType.Boundary;
                Boundary = (Hitbox)Object;
            }
            else if (Object is Vector2)
            {
                objectType = ObjectType.GrapplePoint;
                GrapplePointLocation = (Vector2)Object;
            }
            else if (Object is SpawnPoint)
            {
                objectType = ObjectType.SpawnPoint;
                spawnPoint = (SpawnPoint)Object;
            }
            else
                throw new ArgumentException("Object parameter must be a valid world object");
        }

        public void Update(GameTime gameTime)
        {

        }

        public void Draw(GameTime gameTime)
        {

        }
    }
}
