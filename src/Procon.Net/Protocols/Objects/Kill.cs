using System;

namespace Procon.Net.Protocols.Objects {
    [Serializable]
    public sealed class Kill : NetworkAction {
        /*
        /// <summary>
        /// The killer of the target
        /// </summary>
        public Player Killer { get; set; }

        /// <summary>
        /// The killers location at the time of the targets death
        /// </summary>
        public Point3D KillerLocation { get; set; }

        /// <summary>
        /// The target of the kill
        /// </summary>
        public Player Target { get; set; }

        /// <summary>
        /// The targets location at the time of death
        /// </summary>
        public Point3D TargetLocation { get; set; }
        */
        /// <summary>
        /// The weapon used to kill the player
        /// </summary>
        //public Item DamageType { get; set; }

        /// <summary>
        /// The location of the target 
        /// </summary>
        public HumanHitLocation HumanHitLocation { get; set; }

        public Kill() : base() {
            //this.Killer = new Player();
            //this.Target = new Player();
            //this.DamageType = new Item();
        }
    }
}
