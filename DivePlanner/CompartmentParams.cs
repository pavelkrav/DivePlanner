using System.Collections.Generic;

namespace DivePlanner
{
	class CompartmentParams
	{
		public struct Compartment
		{
			public List<double> paramT;
			public List<double> paramM0;
			public List<double> paramdM;
		}

		bool helium;
		public Compartment compartment;

		public CompartmentParams(bool helium)
		{
			this.helium = helium;
			if (helium)
			{
				compartment.paramT = new List<double>() { 1.51, 1.88, 3.02, 4.72, 6.99, 10.21, 14.48, 20.53, 29.11, 41.20, 55.19, 70.69, 90.34, 115.29, 147.42, 188.24, 240.03 };
				compartment.paramM0 = new List<double>() { 41, 37.2, 31.2, 27.2, 24.3, 22.4, 20.8, 19.4, 18.2, 17.4, 16.8, 16.4, 16.2, 16.1, 16.1, 16.0, 15.9 };
				compartment.paramdM = new List<double>() { 2.3557, 2.0964, 1.74, 1.5321, 1.3845, 1.3189, 1.2568, 1.2079, 1.1692, 1.1419, 1.1232, 1.1115, 1.1022, 1.0963, 1.0904, 1.0850, 1.0791 };
			}
			else
			{
				compartment.paramT = new List<double>() { 4, 5, 8, 12.5, 18.5, 27, 38.3, 54.3, 77, 109, 146, 187, 239, 305, 390, 498, 635 };
				compartment.paramM0 = new List<double>() { 32.4, 29.6, 25.4, 22.5, 20.3, 18.5, 16.9, 15.9, 15.2, 14.7, 14.3, 14.0, 13.7, 13.4, 13.1, 12.9, 12.7 };
				compartment.paramdM = new List<double>() { 1.9082, 1.7928, 1.5352, 1.3847, 1.2780, 1.2306, 1.1857, 1.1504, 1.1223, 1.0999, 1.0844, 1.0731, 1.0635, 1.0552, 1.0478, 1.0414, 1.0359 };
			}
		}
	}
}
