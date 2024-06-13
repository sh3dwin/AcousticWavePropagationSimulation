using System;

namespace NewtonRaphsonSolver
{
    public static class NewtonRaphsonSolver
    {
        private static double Function(double r, double A1, double A2, double a)
        {
            return A1 * Math.Exp(-a * r) / r - A2;
        }

        private static double Derivative(double r, double A1, double A2, double a)
        {
            double term1 = -A1 * Math.Exp(-a * r) / (r * r);
            double term2 = -a * A1 * Math.Exp(-a * r) / r;
            return term1 + term2;
        }

        public static double NewtonRaphson(double initialGuess, double A1, double A2, double a, double tolerance = 1e-7, int maxIterations = 1000)
        {
            double r = initialGuess;
            for (int i = 0; i < maxIterations; i++)
            {
                double fValue = Function(r, A1, A2, a);
                double fDerivative = Derivative(r, A1, A2, a);

                if (Math.Abs(fDerivative) < tolerance)
                {
                    throw new Exception("Derivative too small; Newton-Raphson method fails.");
                }

                double newR = r - fValue / fDerivative;

                if (Math.Abs(newR - r) < tolerance)
                {
                    return newR;
                }

                r = newR;
            }

            throw new Exception("Maximum iterations reached; Newton-Raphson method fails to converge.");
        }
    }
}
