using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Objects
{
    public class ProjectileStorage
    {
        public List<Projectile> Projectiles;
        public ProjectileStorage()
        {
            Projectiles = new List<Projectile>();
        }
        public int GetLength()
        {
            return Projectiles.Count;
        }
        public void Add(Projectile projectile)
        {
            Projectiles.Add(projectile);
        }
        public void Remove(Projectile projectile)
        {
            Projectiles.Remove(projectile);
        }
    }
}
