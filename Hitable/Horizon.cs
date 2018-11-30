﻿using System;
using System.Drawing;
using BlackHoleRaytracer.Equation;

namespace BlackHoleRaytracer.Hitable
{
    public class Horizon : IHitable
    {
        private bool checkered;

        public Horizon(bool checkered)
        {
            this.checkered = checkered;
        }

        public unsafe bool Hit(double* y, double* prevY, double* dydx, double hdid, KerrBlackHoleEquation equation, ref Color color, ref bool stop)
        {
            // Has the ray fallen past the horizon?
            if (y[0] < equation.Rhor)
            {
                if (checkered)
                {
                    var m1 = DoubleMod(y[2], 1.04719); // Pi / 3
                    var m2 = DoubleMod(y[1], 1.04719); // Pi / 3
                    bool foo = (m1 < 0.52359) ^ (m2 < 0.52359); // Pi / 6
                    if (foo)
                    {
                        color = Color.Black;
                    }
                    else
                    {
                        color = Color.Green;
                    }
                }
                else
                {
                    color = Color.Black;
                }
                stop = true;
                return true;
            }
            return false;
        }

        private static double DoubleMod(double n, double m)
        {
            double x = Math.Floor(n / m);
            return n - (m * x);
        }
    }
}