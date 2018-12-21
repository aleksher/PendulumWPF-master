using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNumerics.ODE;

namespace ODE
{
    public static class WilberforcePendulum
    {
        private static OdeExplicitRungeKutta45 odeRK = new OdeExplicitRungeKutta45();
        // TODO: m, k...
        public static double[,] GetOscillations(double start_t, double delta_t, double end_t, double[] y0)
        {
            OdeFunction fun = new OdeFunction(ODEs);
            //x(i) = z, zdot, theta, thetadot
            odeRK.InitializeODEs(fun, 4);
            double[,] sol = odeRK.Solve(y0, start_t, delta_t, end_t);
            return sol;
        }

        private static double[] dxdt = new double[4];
        private static double omega = 2.314;       // rad.s-1
        private static double epsilon = 9.27e-3;   // N
        private static double m = 0.4905;          // kg
        //private static double m = 49.05;          // kg
        private static double I = 1.39e-4;         // kg.m2
        private static double[] ODEs(double t, double[] x)
        {
            //x(i) = z, zdot, theta, thetadot
            dxdt[0] = x[1];
            dxdt[1] = -omega * omega * x[0] - epsilon / 2 / m * x[2];
            dxdt[2] = x[3];
            dxdt[3] = -omega * omega * x[2] - epsilon / 2 / I * x[0];
            return dxdt;
        }
    }
}
