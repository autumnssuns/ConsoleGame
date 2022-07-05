using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Display;

namespace ConsoleGame
{
    /// <summary>
    /// A controllable player.
    /// </summary>
    internal class Player : Creature
    {
        /// <summary>
        /// Creates a new player object
        /// </summary>
        /// <param name="game">The game session where this player belongs</param>
        /// <param name="hp">The hit point of the player. Defaults to 5</param>
        public Player(Game game, int hp = 5) : base(game, hp)
        {
            Shape = new[]
            {
                new[] { ' ', '▐', '▌', ' ' },
                new[] { ' ', '█', '█', ' ' },
                new[] { ' ', '▐', '▌', ' ' },
                new[] { '▐', '█', '█', '▌' }
            };
            Velocity = new Point(0,0);
            CurrentLocation = new Point(game.MaxRow, game.MaxCol / 2);
        }

        /// <summary>
        /// Move the player by a certain displacement.
        /// </summary>
        /// <param name="displacement">The displacement to move the player by.</param>
        public void Move(Point displacement)
        {
            Velocity = displacement;
        }

        /// <summary>
        /// Updates the location of the player, then resets its velocity.
        /// </summary>
        public override void NextFrame()
        {
            base.NextFrame();
            Velocity = new Point(0, 0);
        }
    }
}
