using System;
using System.Collections.Generic;
using Simulator;
using Double3 = Math.Vector3;

//http://phet.colorado.edu/sims/my-solar-system/my-solar-system_en.html

namespace TestApplication
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //Scene scene = new Scene
            //    {
            //        G = 6.67384e-11, // in N m² / kg²  http://en.wikipedia.org/wiki/Gravitational_constant
            //        Masses = new List<Item>
            //            {
            //                //new Item
            //                //    {
            //                //        Name = "Sun",
            //                //        Mass = 1.989e30,
            //                //        Position = new Double3(0, 0, 0),
            //                //        Velocity = new Double3(0, 0, 0),
            //                //    },
            //                //new Item
            //                //    {
            //                //        Name = "Mercury",
            //                //        Mass = 0.33022e24,
            //                //        Position = new Double3(579091750000, 0, 0),
            //                //        Velocity = new Double3(0, 47872.5, 0),
            //                //    },
            //                //new Item
            //                //    {
            //                //        Name = "Venus",
            //                //        Mass = 4.869e24,
            //                //        Position = new Double3(108208930000, 0, 0),
            //                //        Velocity = new Double3(0, 35021.4, 0),
            //                //    },
            //                //new Item
            //                //    {
            //                //        Name = "Earth",
            //                //        Mass = 5.9742e24,
            //                //        Position = new Double3(149597890000, 0, 0),
            //                //        Velocity = new Double3(0, 29785.9, 0),
            //                //    },
            //                //new Item
            //                //    {
            //                //        Name = "Mars",
            //                //        Mass = 0.64191e24,
            //                //        Position = new Double3(227936640000,0 ,0),
            //                //        Velocity = new Double3(0, 24130.9, 0),
            //                //    },
            //                //new Item
            //                //    {
            //                //        Name = "Jupiter",
            //                //        Mass = 1898.70e24,
            //                //        Position = new Double3(778412020000,0 ,0),
            //                //        Velocity = new Double3(0, 13069.7, 0),
            //                //    },
            //                //new Item
            //                //    {
            //                //        Name = "Saturn",
            //                //        Mass = 568.51e24,
            //                //        Position = new Double3(1426725400000,0 ,0),
            //                //        Velocity = new Double3(0, 9672.4, 0),
            //                //    },
            //                //new Item
            //                //    {
            //                //        Name = "Uranus",
            //                //        Mass = 86.849e24,
            //                //        Position = new Double3(2870972200000,0 ,0),
            //                //        Velocity = new Double3(0, 6835.2, 0),
            //                //    },
            //                //new Item
            //                //    {
            //                //        Name = "Neptune",
            //                //        Mass = 102.44e24,
            //                //        Position = new Double3(4498252900000,0 ,0),
            //                //        Velocity = new Double3(0, 5477.8, 0),
            //                //    },
            //                //new Item
            //                //    {
            //                //        Name = "Pluto",
            //                //        Mass = 0.013e24,
            //                //        Position = new Double3(5906376200000, 0, 0),
            //                //        Velocity = new Double3(0, 4749, 0),
            //                //    },
            //                new Item
            //                    {
            //                        Name = "Sun",
            //                        Mass = 1.989e30,
            //                        Position = new Double3(0, 0, 0),
            //                        Velocity = new Double3(0, 0, 0),
            //                    },
            //                new Item
            //                    {
            //                        Name = "Earth",
            //                        Mass = 5.9742e24,
            //                        Position = new Double3(149597890000, 0, 0),
            //                        Velocity = new Double3(0, 29785.9, 0),
            //                    },
            //                new Item
            //                    {
            //                        Name = "Moon",
            //                        Mass = 7.36e22,
            //                        Position = new Double3(149597890000+384401000, 0, 0),
            //                        Velocity = new Double3(0, 29785.9+1023, 0),
            //                    },
            //            }
            //    };
            Scene scene = new Scene
                {
                    G = 6.67384e-11  // in N m² / kg²  http://en.wikipedia.org/wiki/Gravitational_constant
                };
            Item sun = scene.AddMass(new Item
                {
                    Name = "Sun",
                    Mass = 1.989e30,
                    Position = new Double3(0, 0, 0),
                    Velocity = new Double3(0, 0, 0),
                });
            Item earth = scene.AddMass(
                sun,
                new Item
                    {
                        Name = "Earth",
                        Mass = 5.9742e24,
                        Position = new Double3(149597890000, 0, 0),
                        Velocity = new Double3(0, 29785.9, 0),
                    });
            Item moon = scene.AddMass(
                earth,
                new Item
                    {
                        Name = "Moon",
                        Mass = 7.36e22,
                        Position = new Double3(384401000, 0, 0),
                        Velocity = new Double3(0, 1023, 0),
                    });
            scene.Dump();

            Console.WindowWidth = 120;
            Console.WindowHeight = 80;

            ////const double increment = 24 * 60 * 60; // 1 day
            //const double increment = 6*60*60; // 6 hours
            //while (true)
            //{
            //    scene.SimulateEuler(increment);
            //    scene.Dump();

            //    System.Threading.Thread.Sleep(100);
            //}

            //const double increment = 24 * 60 * 60; // 1 day
            const double increment = 6 * 60 * 60; // 6 hours

            scene.SimulateEuler(increment);
            scene.Dump();

            while (true)
            {
                scene.SimulateEuler(increment);
                scene.Dump();

                System.Threading.Thread.Sleep(100);
            }
        }
    }
}