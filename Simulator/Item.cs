using Double3 = Math.Vector3;

namespace Simulator
{
    public class Item
    {
        public string Name { get; set; }
        public double Mass { get; set; } // in kg

        // Current
        public Double3 Position { get; set; } // in m
        public Double3 Velocity { get; set; } // in m/s
        public Double3 Force { get; set; } // in N

        // Previous
        public Double3 PreviousPosition { get; set; }
        public Double3 PreviousVelocity { get; set; }
        public Double3 PreviousForce { get; set; }

        // 
        public Double3 VelocityMiddleTimeStep { get; set; }
    }
}
