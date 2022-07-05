using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleGame
{
    public class Creature : Entity
    {
        private int hitPoint;

        /// <summary>
        /// The hit point of the entity.
        /// </summary>
        public int HitPoint { get => hitPoint; set => hitPoint = value; }

        /// <summary>
        /// Constructs a creature in a game with some hit points.
        /// </summary>
        /// <param name="game">The game the creature is in.</param>
        /// <param name="hp">The hit point of the creature. Defaults to 1.</param>
        public Creature(Game game, int hp = 1) : base(game)
        {
            HitPoint = hp;
        }

        /// <summary>
        /// A creature expires when their HP is 0.
        /// </summary>
        public override bool IsExpiring => base.IsExpiring || HitPoint <= 0;
    }
}
