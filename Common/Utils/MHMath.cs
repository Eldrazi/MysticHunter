#region Using directives

using Microsoft.Xna.Framework;

#endregion

namespace MysticHunter.Common.Utils
{
	public static class MHMath
	{
		private static readonly double[] factorialLookup = {
			1.0,
			1.0,
			2.0,
			6.0,
			24.0,
			120.0,
			720.0,
			5040.0,
			40320.0,
			362880.0,
			3628800.0,
			39916800.0,
			479001600.0,
			6227020800.0,
			87178291200.0,
			1307674368000.0,
			20922789888000.0,
			355687428096000.0,
			6402373705728000.0,
			121645100408832000.0,
			2432902008176640000.0,
			51090942171709440000.0,
			1124000727777607680000.0,
			25852016738884976640000.0,
			620448401733239439360000.0,
			15511210043330985984000000.0,
			403291461126605635584000000.0,
			10888869450418352160768000000.0,
			304888344611713860501504000000.0,
			8841761993739701954543616000000.0,
			265252859812191058636308480000000.0,
			8222838654177922817725562880000000.0,
			263130836933693530167218012160000000.0
		};

		private static double Factorial(int n)
		{
			if (n < 0) { return (1); }
			if (n > 32) { return (1); }

			return factorialLookup[n];
		}

		private static double Ni(int n, int i)
		{
			double ni;
			double a1 = Factorial(n);
			double a2 = Factorial(i);
			double a3 = Factorial(n - i);
			ni = a1 / (a2 * a3);
			return ni;
		}

		private static double Bernstein(int n, int i, double t)
		{
			double basis;
			double ti; /* t^i */
			double tni; /* (1 - t)^i */

			/* Prevent problems with pow */
			if (t == 0.0 && i == 0)
			{
				ti = 1.0;
			}
			else
			{
				ti = System.Math.Pow(t, i);
			}
			if (n == i && t == 1.0)
			{
				tni = 1.0;
			}
			else
			{
				tni = System.Math.Pow((1 - t), (n - i));
			}
			//Bernstein basis
			basis = Ni(n, i) * ti * tni;
			return basis;
		}

		public static void Bezier2D_Point(Vector2[] inPoints, Vector2[] outPoints, float stepModifier = 0)
		{
			int npts = inPoints.Length;

			double t = 0;
			double step = (double)1.0 / (outPoints.Length - 1) + stepModifier;

			for (int i = 0; i != outPoints.Length; i++)
			{
				outPoints[i] = Vector2.Zero;

				if ((1.0 - t) < 5e-6)
				{
					t = 1.0;
				}

				for (int j = 0; j != npts; j++)
				{
					float basis = (float)Bernstein(inPoints.Length - 1, j, t);
					outPoints[i] += new Vector2(basis) * inPoints[j];
				}
				t += step;
			}
		}
	}
}
