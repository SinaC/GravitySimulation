using System;
using System.Collections.Generic;
using Double3 = Math.Vector3;


//http://phet.colorado.edu/sims/my-solar-system/my-solar-system_en.html

namespace Simulator
{
    public class Scene
    {
        public double Bias { get; protected set; }
        public double G { get; protected set; }
        public List<Body> Masses { get; protected set; }

        public Scene(double g)
        {
            G = g;
            Bias = 1;
        }

        public Scene(double g, double bias)
        {
            G = g;
            Bias = bias;
        }

        public Body AddMass(Body mass)
        {
            if (Masses == null)
                Masses = new List<Body>();
            Masses.Add(mass);
            // apply bias
            mass.Mass *= Bias * Bias * Bias;
            mass.Position *= Bias;
            mass.Velocity *= Bias;
            return mass;
        }

        public Body AddMass(Body parent, Body mass)
        {
            AddMass(mass);
            mass.Position += parent.Position;
            mass.Velocity += parent.Velocity;
            return mass;
        }


        // Euler-integration  
        //  http://en.wikipedia.org/wiki/Euler_method
        public void SimulateEuler(int step, double dt)
        {
            ResetForce();

            CalculateGravitationalForce();

            // Integrate force
            foreach (Body m in Masses)
            {
                // F = m a  ==> a = F / m and a = v / dt  ==> v = F * dt / m
                m.Velocity += m.Force * dt / m.Mass;
                // v = p / dt ==> p = v * dt
                m.Position += m.Velocity * dt;
            }

            SavePreviousValues();
        }

        // TODO: http://en.wikipedia.org/wiki/Runge%E2%80%93Kutta_method
        public void SimulateRungeKutta(int step, double dt)
        {
            throw new NotImplementedException();
        }

        // BUGGY
        // Verlet-integration 
        //  http://en.wikipedia.org/wiki/Verlet_integration#Velocity_Verlet
        //  http://www.fisica.uniud.it/~ercolessi/md/md/node21.html
        //  http://www.mbnexplorer.com/users-guide/5-molecular-dynamics-simulations/52-integration-algorithms/521-velocity-verlet
        // 1) calculate new velocity at the middle of time step
        //  v(t+dt/2) = v(t) + a(t)*dt/2
        // 2) calculate new position
        //  p(t+dt) = p(t) + v(t+dt/2)*dt
        // 3) calculate new force F(t+dt)
        // 4) calculate new acceleration
        //  a(t+dt) = F(t+dt)/m
        // 5) calculate new velocity
        //  v(t+dt) = v(t+dt/2) + a(t+dt)*dt/2
        public void SimulateVerlet(int step, double dt)
        {
            ResetForce();

            if (step == 0) // Euler for first step
            {
                CalculateGravitationalForce();

                foreach (Body m in Masses)
                {
                    // F = m a  ==> a = F / m and a = v / dt  ==> v = F * dt / m
                    m.Velocity += m.Force * dt / m.Mass;
                    // v = p / dt ==> p = v * dt
                    m.Position += m.Velocity * dt;
                }
            }
            else
            {
                // 1)
                foreach (Body m in Masses)
                    m.VelocityMiddleTimeStep = m.PreviousVelocity + m.PreviousForce*dt*0.5/m.Mass;
                // 2)
                foreach (Body m in Masses)
                    m.Position = m.PreviousPosition + m.VelocityMiddleTimeStep*dt;
                // 3)
                CalculateGravitationalForce();
                // 4) 5)
                foreach (Body m in Masses)
                    m.Velocity = m.VelocityMiddleTimeStep + m.Force*dt*0.5/m.Mass;
            }

            SavePreviousValues();
        }

        protected void CalculateGravitationalForce()
        {
            // Compute gravitational force between masses ==> F = G * m1 * m2 / d²
            for (int i = 0; i < Masses.Count; i++)
            {
                Body m1 = Masses[i];
                for (int j = i + 1; j < Masses.Count; j++)
                {
                    Body m2 = Masses[j];
                    // Compute squared distance
                    Double3 vector = m1.Position - m2.Position;
                    double squaredDistance = vector.LengthSquared();
                    // Compute force length
                    double force = G * m1.Mass * m2.Mass / squaredDistance;
                    // Get force direction
                    Double3 unitVector = Double3.Normalize(vector);
                    Double3 force1 = unitVector * -force;
                    Double3 force2 = unitVector * force;
                    // Add force to masses
                    m1.Force += force1;
                    m2.Force += force2;

                    Console.WriteLine("Distance: {0}  Force: {1}  {2} {3}", vector.Length(), force, m1.Name, m2.Name);
                }
            }
        }

        protected void ResetForce()
        {
            // Reset force
            foreach (Body m in Masses)
                m.Force = new Double3(0, 0, 0);
        }

        protected void SavePreviousValues()
        {
            // Save values (even if not used)
            foreach (Body m in Masses)
            {
                m.PreviousForce = new Double3(m.Force);
                m.PreviousPosition = new Double3(m.Position);
                m.PreviousVelocity = new Double3(m.Velocity);
            }
        }

        public void Dump()
        {
            foreach (Body m in Masses)
                Console.WriteLine("{0} {1} {2}", m.Name, m.Position, m.Velocity);
        }
    }
}