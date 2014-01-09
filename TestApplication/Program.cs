using System;
using Simulator;
using Double3 = Math.Vector3;

namespace TestApplication
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Scene scene = new Scene(6.67384e-11, 1e-6);// in N m² / kg²  or  m³ / (kg * s²)  http://en.wikipedia.org/wiki/Gravitational_constant
            //Body sun = scene.AddMass(new Body
            //    {
            //        Name = "Sun",
            //        Mass = 1.989e30,
            //        Position = new Double3(0, 0, 0),
            //        Velocity = new Double3(0, 0, 0),
            //    });
            //Body mercury = scene.AddMass(
            //    sun,
            //    new Body
            //    {
            //        Name = "Mercury",
            //        Mass = 0.33022e24,
            //        Position = new Vector3(57909100000, 0, 0),
            //        Velocity = new Double3(0, 47872.5, 0),
            //    });
            //Body venus = scene.AddMass(
            //    sun,
            //    new Body
            //        {
            //            Name = "Venus",
            //            Mass = 4.869e24,
            //            Position = new Double3(108208930000, 0, 0),
            //            Velocity = new Double3(0, 35021.4, 0),
            //        });
            //Body earth = scene.AddMass(
            //    sun,
            //    new Body
            //        {
            //            Name = "Earth",
            //            Mass = 5.97219e24,
            //            Position = new Double3(149597890000, 0, 0),
            //            Velocity = new Double3(0, 29785.9, 0),
            //        });
            //Body moon = scene.AddMass(
            //    earth,
            //    new Body
            //        {
            //            Name = "Moon",
            //            Mass = 7.3477e22,
            //            Position = new Double3(384399000, 0, 0),
            //            Velocity = new Double3(0, 1022, 0),
            //        });
            //Body mars = scene.AddMass(
            //    sun,
            //    new Body
            //        {
            //            Name = "Mars",
            //            Mass = 0.64191e24,
            //            Position = new Double3(227936640000, 0, 0),
            //            Velocity = new Double3(0, 24130.9, 0),
            //        }
            //    );
            //Body jupiter = scene.AddMass(
            //    sun,
            //    new Body
            //        {
            //            Name = "Jupiter",
            //            Mass = 1898.70e24,
            //            Position = new Double3(778412020000, 0, 0),
            //            Velocity = new Double3(0, 13069.7, 0),
            //        });
            //Body saturn = scene.AddMass(
            //    sun,
            //    new Body
            //        {
            //            Name = "Saturn",
            //            Mass = 568.51e24,
            //            Position = new Double3(1426725400000, 0, 0),
            //            Velocity = new Double3(0, 9672.4, 0),
            //        });
            //Body uranus = scene.AddMass(
            //    sun,
            //    new Body
            //        {
            //            Name = "Uranus",
            //            Mass = 86.849e24,
            //            Position = new Double3(2870972200000, 0, 0),
            //            Velocity = new Double3(0, 6835.2, 0),
            //        });
            //Body neptune = scene.AddMass(
            //    sun,
            //    new Body
            //        {
            //            Name = "Neptune",
            //            Mass = 102.44e24,
            //            Position = new Double3(4498252900000, 0, 0),
            //            Velocity = new Double3(0, 5477.8, 0),
            //        });
            //Body pluto = scene.AddMass(
            //    sun,
            //    new Body
            //        {
            //            Name = "Pluto",
            //            Mass = 0.013e24,
            //            Position = new Double3(5906376200000, 0, 0),
            //            Velocity = new Double3(0, 4749, 0),
            //        });

            Body earth = scene.AddMass(
                new Body
                    {
                        Name = "Earth",
                        Mass = 5.97219e24,
                        Position = new Double3(0, 0, 0),
                        Velocity = new Double3(0, 0, 0),
                    });
            Body moon = scene.AddMass(
                earth,
                new Body
                    {
                        Name = "Moon",
                        Mass = 7.3477e22,
                        Position = new Double3(384399000, 0, 0),
                        Velocity = new Double3(0, 1022, 0),
                    });

            //Body sun = scene.AddMass(new Body
            //    {
            //        Name = "Sun",
            //        Mass = 1.989e30,
            //        Position = new Double3(0, 0, 0),
            //        Velocity = new Double3(0, 0, 0),
            //    });
            //Body earth = scene.AddMass(
            //    sun,
            //    new Body
            //        {
            //            Name = "Earth",
            //            Mass = 5.97219e24,
            //            Position = new Double3(149597890000, 0, 0),
            //            Velocity = new Double3(0, 29785.9, 0),
            //        });

            scene.Dump();

            Console.WindowWidth = 120;
            Console.WindowHeight = 80;

            //const double increment = 24 * 60 * 60; // 1 day
            const double increment = 6 * 60 * 60; // 6 hours

            int i = 0;

            while (true)
            {
                Console.WriteLine("{0}|{1}", i / 365, i % 365);
                scene.SimulateVerlet(i, increment); //scene.SimulateEuler(i, increment);
                scene.Dump();

                System.Threading.Thread.Sleep(100);

                i++;
            }
        }
    }
}