using System;

namespace Procon.Net.Attributes {

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class DispatchPacketAttribute : Attribute {

        public String MatchText { get; set; }

        public PacketOrigin PacketOrigin { get; set; }

        // TODO: Add version information and allow multiples
        // Also include: "Fallback" so if no function matching the specified version
        // exist procon will fallback on the method with this flag (set to true by default
        // need to specify false on newer methods)

    }
}
