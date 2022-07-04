using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Display;

namespace ConsoleGame
{
    public class Projectile : Entity
    {
        public Projectile(Game game, Point location, Point velocity) : base(game)
        {
            Shape = new[]
            {
                new[] {'|'}
            };
            CurrentLocation = location;
            Velocity = velocity;
        }
        public Projectile(Game game) : this(game, new Point(0,0), new Point(0,0))
        {
        }

        /// <summary>
        /// A projectiles expires when it is either out of bound, or explicitly set.
        /// </summary>
        public override bool IsExpiring => base.IsExpiring || IsOutOfBound();
    }
}
