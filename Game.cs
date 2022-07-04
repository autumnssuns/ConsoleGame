using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Display;

namespace ConsoleGame
{
    /// <summary>
    /// The main game loop
    /// </summary>
    public class Game
    {
        public Grid GameGrid { get; }
        private bool running;
        private Player player;
        private List<Entity> entities;
        private int framesPerSecond;
        private Random rand;
        private int killCount;
        public int MaxRow { get; }
        public int MaxCol { get; }
        /// <summary>
        /// Initialise a game
        /// </summary>
        /// <param name="rows">The number of rows in the game grid</param>
        /// <param name="cols">The number of columns in the game rid</param>
        /// <param name="framesPerSecond">The game's refresh rate</param>
        public Game(int rows = 24, int cols = 48, int framesPerSecond = 30)
        {
            GameGrid = new Grid(rows, cols);
            MaxRow = rows - 1;
            MaxCol = cols - 1;
            rand = new Random();
            killCount = 0;
            entities = new List<Entity>();
            player = new Player(this);
            entities.Add(player);
            SpawnEnemies();
            GameGrid.InitializeWindow();
            Update();

            this.framesPerSecond = framesPerSecond;
        }

        /// <summary>
        /// Starts the game loop, polling for key press
        /// </summary>
        public void Start()
        {
            running = true;
            while (running)
            {
                if (Console.KeyAvailable) KeyAction(Console.ReadKey(true));
                Update();
                Thread.Sleep(1000 / framesPerSecond);
            }
        }

        /// <summary>
        /// Update the game state.
        /// </summary>
        public void Update()
        {
            List<Entity> projectiles = entities.Where(e => e is Projectile).ToList();
            entities.ForEach(e =>
            {
                bool hit = e is Enemy && projectiles.Any(p =>
                {
                    bool bulletHit = p.IsColliding(e) && !p.IsExpiring;
                    if (bulletHit) p.IsExpiring = true;
                    return bulletHit;
                });
                if (hit)
                {
                    e.IsExpiring = true;
                    killCount++;
                }
            });
            entities.RemoveAll(e =>
            {
                if (e.IsExpiring)
                {
                    e.NextFrame();
                    e.ErasePreviousImage();
                }
                return e.IsExpiring;
            });
            if (!entities.Any(e => e is Enemy)) SpawnEnemies();
            entities.ForEach(e =>
            {
                e.NextFrame();
                e.Draw();
            });
            GameGrid.Render();
        }

        /// <summary>
        /// Summons 6 enemies across the screen.
        /// </summary>
        private void SpawnEnemies()
        {
            for (int i = 0; i < 8; i++)
            {
                Enemy enemy = new Enemy(this, new Point(1, 1 + i * 6));
                // Set a random color, excluding black
                enemy.SetColor((ConsoleColor)rand.Next(1,16));
                entities.Add(enemy);
            }
        }

        /// <summary>
        /// Performs an action based on the pressed key.
        /// </summary>
        /// <param name="key">The pressed key</param>
        private void KeyAction(ConsoleKeyInfo key)
        {
            switch (key.Key)
            {
                case ConsoleKey.Escape:
                    running = false;
                    break;
                case ConsoleKey.LeftArrow:
                    player.Move(new Point(0, -1));
                    break;
                case ConsoleKey.RightArrow:
                    player.Move(new Point(0, 1));
                    break;
                case ConsoleKey.UpArrow:
                    player.Move(new Point(-1, 0));
                    break;
                case ConsoleKey.DownArrow:
                    player.Move(new Point(1, 0));
                    break;
                case ConsoleKey.Spacebar:
                    Fire();
                    break;
                case ConsoleKey.C:
                    GameGrid.Clear();
                    break;
            }
        }

        /// <summary>
        /// Fire projectiles from the player.
        /// </summary>
        private void Fire()
        {
            // Use insert to ensure the projectile are first in the list (so that their
            // images are overriden)
            if (killCount >= 0)
            {
                for (int i = 0; i < killCount / 4 + 1; i++)
                {
                    Projectile bullet = new Projectile(this,
                        player.CurrentLocation.Shift(-1, 1 + i - killCount / 8),
                        new Point(-1, 0));
                    bullet.SetColor(ConsoleColor.Red);
                    entities.Insert(0, bullet);
                }
            } 
        }
    }
}
