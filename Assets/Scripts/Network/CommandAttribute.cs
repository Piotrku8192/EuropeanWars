using System;

namespace EuropeanWars.Network {
    public class CommandAttribute : Attribute {
        public ushort id;

        public CommandAttribute(ushort id) {
            this.id = id;
        }
    }
}
