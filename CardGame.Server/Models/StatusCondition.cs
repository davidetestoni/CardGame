namespace CardGame.Server.Models
{
    public abstract class StatusCondition
    {
        /// <summary>
        /// The unique id of this status condition, for example Stunned.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The name of this status condition, for example Stunned.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The description of this status condition.
        /// </summary>
        public string Description { get; set; }
    }
}
