using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Display;

namespace ConsoleGame
{
    internal class Enemy : Entity
    {
        public Enemy(Game game, Point spawnLocation) : base(game)
        {
            Shape = new[]
            {
                new[] { '▐', '█', '█', '▌' },
                new[] { '█', '▐', '▌', '█' },
                new[] { '█', '█', '█', '█' },
                new[] { '▌', '▌', '▐', '▐' }
            };
            Velocity = new Point(0, 0);
            CurrentLocation = spawnLocation;
        }
    }
}