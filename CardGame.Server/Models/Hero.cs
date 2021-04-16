using System.Collections.Generic;

namespace CardGame.Server.Models
{
    public class Hero
    {
        /// <summary>
        /// The unique id of the hero.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The name of the hero.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The description of the hero.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The custom stats of the hero.
        /// </summary>
        public Dictionary<string, int> CustomStats { get; set; } = new();

        /// <summary>
        /// The active skill of the hero.
        /// </summary>
        public ActiveSkill ActiveSkill { get; set; }
    }
}
