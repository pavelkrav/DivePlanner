using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DivePlanner
{
	class DiveSite
	{
		public string Area { get; set; }
		public double Altitude { get; set; }

		public DiveSite()
		{
			Area = "Unknown";
			Altitude = 0;
		}
	}

	class Dive
	{
		public const double pressureSeaLevel = 10;

		public DiveSite Site { get; set; }
		public List<DivePoint> DivePoints { get; set; }
		public double MaxDepth { get; set; }
		public double TimeLength { get; set; }

		public Dive(List<DivePoint> points)
		{

		}

		public Dive(DiveSite site, List<DivePoint> points)
		{

		}

		public Dive(int test)
		{
			if (test == 1)
			{
				Site = new DiveSite();
				MaxDepth = 50;
				TimeLength = 6000;

				DivePoints = new List<DivePoint>();
				DivePoints.Add(new DivePoint(0, 0));
				DivePoints.Add(new DivePoint(135, 45));
				DivePoints.Add(new DivePoint(1935, 45));
				DivePoints.Add(new DivePoint(1980, 30));
				DivePoints.Add(new DivePoint(5580, 30));
			}
			if (test == 2)
			{
				Site = new DiveSite();
				MaxDepth = 50;
				TimeLength = 200 * 60;

				DivePoints = new List<DivePoint>();
				DivePoints.Add(new DivePoint(0, 0));
				DivePoints.Add(new DivePoint(135, 45));
				DivePoints.Add(new DivePoint(1935, 45));
				DivePoints.Add(new DivePoint(1980, 30));
				DivePoints.Add(new DivePoint(5580, 30));
				DivePoints.Add(new DivePoint(94.8 * 60, 12));
				DivePoints.Add(new DivePoint(96.8 * 60, 12));
				DivePoints.Add(new DivePoint(97.1 * 60, 9));
				DivePoints.Add(new DivePoint(114.1 * 60, 9));
				DivePoints.Add(new DivePoint(114.4 * 60, 6));
				DivePoints.Add(new DivePoint(143.4 * 60, 6));
				DivePoints.Add(new DivePoint(143.7 * 60, 3));
				DivePoints.Add(new DivePoint(197.7 * 60, 3));
				DivePoints.Add(new DivePoint(198 * 60, 0));
			}
		}

		public Dive()
		{
			Site = new DiveSite();
			MaxDepth = 40;
			TimeLength = 2400;

			DivePoints = new List<DivePoint>();
			DivePoints.Add(new DivePoint(0, 0));
			
		}

		public void OrderDivePoints()
		{
			DivePoints.Sort();
		}

		public void RemovePoint(int listNumber)
		{
			List <DivePoint> buffer = new List<DivePoint>();
			foreach (DivePoint point in DivePoints)
			{
				buffer.Add(point);
			}
			DivePoints.Clear();
			for (int i = 0; i < buffer.Count; i++)
			{
				if (i != listNumber)
					DivePoints.Add(buffer[i]);
			}
		}

		public void RemovePoint(double time, double depth)
		{
			List<DivePoint> buffer = new List<DivePoint>();
			foreach (DivePoint point in DivePoints)
			{
				buffer.Add(point);
			}
			DivePoints.Clear();
			for (int i = 0; i < buffer.Count; i++)
			{
				if (!(buffer[i].Time == time && buffer[i].Depth == depth))
					DivePoints.Add(buffer[i]);
			}
		}

		public int GetListNumber(double time, double depth)
		{
			for (int i = 0; i < DivePoints.Count; i++)
			{
				if (DivePoints[i].Time == time && DivePoints[i].Depth == depth)
				{
					return i;
				}
			}
			return -1;
		}

		private struct PointParams
		{
			public double[] pCurrent;
			public List<double> depth;
			public List<double> time;
			public GasMix Gas;
			public double airConsumption;
			public double tTotal;
			public int step;
		}

		private struct PointParamsAscent
		{
			public List<double> depth;
			public List<double> time;
			public double airConsumption;
		}

		private double pressureAtoB(double pAmbient, double FGas, double vRate, double t, double k, double pCurrent)
		{
			return pAmbient + (FGas * vRate * (t - 1 / k)) - (pAmbient - pCurrent - FGas * vRate / k) * Math.Exp(-k * t);
		}

		private double pressurePoint(double pAmbient, double pCurrent, double k, double t)
		{
			return pAmbient - (pAmbient - pCurrent) * Math.Exp(-k * t);
		}

		private double pressureAmbient(double FGas, double depth, double pressureSeaLevel)
		{
			return FGas * (depth + pressureSeaLevel - 0.619);
		}

		private double Kf(double t)
		{
			return Math.Log(2) / t;
		}

		private List<double> KF(List<double> t)
		{
			List<double> list = new List<double>();
			double[] array = new double[17];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = Kf(t[i]);
				list.Add(array[i]);
			}
			return list;
		}

		private double maxAmbPressure(double pCurrent, double M0, double dM, double PressureSeaLevel)
		{
			return (pCurrent - M0 + dM * PressureSeaLevel) / dM;
		}

		private double depthTo3(double pCurrent, double M0, double dM, double PressureSeaLevel)
		{
			double x, y;
			x = Math.Ceiling(((pCurrent - M0 + dM * PressureSeaLevel) / dM) - PressureSeaLevel) % 3;
			if (x == 0)
			{
				y = Math.Ceiling(((pCurrent - M0 + dM * PressureSeaLevel) / dM) - PressureSeaLevel);
			}
			else
			{
				y = Math.Ceiling(((pCurrent - M0 + dM * PressureSeaLevel) / dM) - PressureSeaLevel) - x + 3;
			}
			return y;
		}

		private double maxDepthTo3(double[] pCurrent, List<double> M0, List<double> dM, double PressureSeaLevel)
		{
			double[] array = new double[17];
			double max = 0;
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = depthTo3(pCurrent[i], M0[i], dM[i], PressureSeaLevel);
				Console.WriteLine("Current Pressure comparment N " + i + " = " + pCurrent[i]);
				Console.WriteLine("depth compartment N " + i + " = " + array[i]);
				if (array[i] > max)
				{
					max = array[i];
				}
			}
			return max;
		}

		private double pressureInCompartment(double M0, double dM, double depth)
		{
			return M0 + dM * depth;
		}

		private double timeStop(double P, double pAmbient, double pCurrent, double k)
		{
			if (((P - pAmbient) / (pCurrent - pAmbient) >= 0) && ((P - pAmbient) / (pCurrent - pAmbient) <= 1))
			{
				return Math.Ceiling(Math.Log((P - pAmbient) / (pCurrent - pAmbient)) / (-k));
			}
			else
			{
				return 0;
			}
		}

		private double findAirConsumption(double vConsumption, double t, double dCurrent, double dNext)
		{
			if (dCurrent == dNext)
			{
				return vConsumption * t * (dCurrent / 10 + 1);
			}
			else
			{
				return vConsumption * t * (Math.Abs(dCurrent - dNext) / 20 + 1);
			}
		}

		private double maxTimeToStop(double[] pCurrent, List<double> M0, List<double> dM, double pAmbient, double depth, List<double> k)
		{
			double max = 0;
			double[] time = new double[17];
			double[] check = new double[17];
			double[] test = new double[17];
			// Console.WriteLine("Pressure Ambient on " + depth + " meters = " + pAmbient);
			for (int i = 0; i < time.Length; i++)
			{
				time[i] = timeStop(pressureInCompartment(M0[i], dM[i], depth - 3), pAmbient, pCurrent[i], k[i]);
				check[i] = pressureInCompartment(M0[i], dM[i], depth - 3);
				test[i] = (check[i] - pAmbient) / (pCurrent[i] - pAmbient);
				if (time[i] > max)
				{
					max = time[i];
				}
			}
			return max;
		}


		private PointParamsAscent ascentUp(PointParams previousStep, double pressureSeaLevel, double depthTo3, double timeBeforeAscent, double lastDepth)
		{
			CompartmentParams compartmentParams = new CompartmentParams(previousStep.Gas.Helium);
			PointParams pointParams;
			pointParams.pCurrent = new double[17];
			for (int i = 0; i < pointParams.pCurrent.Length; i++)
			{
				pointParams.pCurrent[i] = previousStep.pCurrent[i];
			}
			PointParamsAscent ascentParams;
			ascentParams.depth = new List<double>();
			ascentParams.time = new List<double>();
			double timeBeforeStepAscent = timeBeforeAscent;
			int vRate = -10;
			int number = 1;
			double vAscent = 18;
			double vOnDepth = 9;
			double airConsumption = (double)previousStep.airConsumption;
			double timeToStop = 0;
			double FGas = 1 - (double)previousStep.Gas.Oxygen;
			double depthCurrent = lastDepth;
			double depthNext = depthTo3;
			ascentParams.airConsumption = airConsumption;
			while (depthNext > 0)
			{
				double t = (depthNext - depthCurrent) / vRate;
				for (int i = 0; i < pointParams.pCurrent.Length; i++)
				{
					pointParams.pCurrent[i] = pressureAtoB(pressureAmbient(FGas, depthCurrent, pressureSeaLevel), FGas, vRate, t, Kf(compartmentParams.compartment.paramT[i]), pointParams.pCurrent[i]);
				}
				airConsumption += findAirConsumption(vAscent, t, depthCurrent, depthNext);
				timeToStop = maxTimeToStop(pointParams.pCurrent, compartmentParams.compartment.paramM0, compartmentParams.compartment.paramdM, pressureAmbient(FGas, depthNext, pressureSeaLevel), depthNext, KF(compartmentParams.compartment.paramT));
				if (timeToStop > 0)
				{
					for (int i = 0; i < pointParams.pCurrent.Length; i++)
					{
						pointParams.pCurrent[i] = pressurePoint(pressureAmbient(FGas, depthNext, pressureSeaLevel), pointParams.pCurrent[i], Kf(compartmentParams.compartment.paramT[i]), timeToStop);
					}
					airConsumption += findAirConsumption(vOnDepth, timeToStop, depthNext, depthNext);
					double d = depthNext;
					ascentParams.depth.Add(d);
					ascentParams.time.Add(timeBeforeStepAscent + t);
					ascentParams.depth.Add(d);
					ascentParams.time.Add(timeBeforeStepAscent + t + timeToStop);
					timeBeforeStepAscent += t + timeToStop;
					depthCurrent = depthNext;
					depthNext -= 3;
					number += 1;
				}
				else
				{
					double d = depthNext;
					depthNext -= 3;
					timeBeforeStepAscent += t;
				}
			}
			ascentParams.depth.Add(0);
			ascentParams.time.Add(timeBeforeStepAscent + 0.3);
			ascentParams.airConsumption = airConsumption;
			return ascentParams;
		}

		public void GetAscentByIndex(int index)
		{
			List<DivePoint> myDivePoint = new List<DivePoint>();
			CompartmentParams compartmentParams = new CompartmentParams(DivePoints[0].Gas.Helium);
			double FGas = 1 - DivePoints[0].Gas.Oxygen;
			PointParams truePointParams = new PointParams();
			PointParamsAscent truePointParamsAscent = new PointParamsAscent();
			truePointParams.pCurrent = new double[17];
			for (int i = 0; i < truePointParams.pCurrent.Length; i++)
			{
				truePointParams.pCurrent[i] = pressureAmbient(FGas, 0, pressureSeaLevel);
			}
			int count = index;
			int j = 0;
			double airConsumption = 0;
			double vConsumption = 30;
			while (count > 0)
			{
				compartmentParams = new CompartmentParams(DivePoints[j].Gas.Helium);
				FGas = 1 - DivePoints[j].Gas.Oxygen;
				double vRate = (DivePoints[j + 1].Depth - DivePoints[j].Depth) / (DivePoints[j + 1].Time - DivePoints[j].Time);
				airConsumption += findAirConsumption(vConsumption, DivePoints[j + 1].Time - DivePoints[j].Time, DivePoints[j].Depth, DivePoints[j + 1].Depth);
				for (int i = 0; i < truePointParams.pCurrent.Length; i++)
				{
					truePointParams.pCurrent[i] = pressureAtoB(pressureAmbient(FGas, DivePoints[j].Depth, pressureSeaLevel), FGas, vRate, DivePoints[j + 1].Time - DivePoints[j].Time, Kf(compartmentParams.compartment.paramT[i]), truePointParams.pCurrent[i]);
				}
				j += 1;
				count -= 1;
			}
			compartmentParams = new CompartmentParams(DivePoints[j].Gas.Helium);
			FGas = 1 - DivePoints[j].Gas.Oxygen;
			double depthTo3 = maxDepthTo3(truePointParams.pCurrent, compartmentParams.compartment.paramM0, compartmentParams.compartment.paramdM, pressureSeaLevel);
			truePointParams.airConsumption = airConsumption;
			truePointParams.Gas = DivePoints[j].Gas;
			double lastDepth = DivePoints[j].Depth;
			double lastTime = DivePoints[j].Time;
			truePointParamsAscent = ascentUp(truePointParams, pressureSeaLevel, depthTo3, lastTime, lastDepth);
			for (int i = 0; i < truePointParamsAscent.depth.Count; i++)
			{
				DivePoint dv = new DivePoint(truePointParamsAscent.time[i], truePointParamsAscent.depth[i]);
				myDivePoint.Add(dv);
			}
			DivePoints.RemoveRange(index + 1, DivePoints.Count - 1 - index);
			DivePoints.AddRange(myDivePoint);
		}

		public bool PointAvailable(int index)
		{
			int ind = index;
			if (((DivePoints[ind].Depth < DivePoints[ind - 1].Depth) && ((DivePoints[ind].Depth - DivePoints[ind - 1].Depth) / (DivePoints[ind - 1].Time - DivePoints[ind].Time) > 18)) || (DivePoints[ind - 1].Time >= DivePoints[ind].Time))
			{
				return false;
			}
			else
			{
				List<DivePoint> myDivePoint = new List<DivePoint>();
				CompartmentParams compartmentParams = new CompartmentParams(DivePoints[0].Gas.Helium);
				double FGas = 1 - DivePoints[0].Gas.Oxygen;
				PointParams truePointParams = new PointParams();
				truePointParams.pCurrent = new double[17];
				for (int i = 0; i < truePointParams.pCurrent.Length; i++)
				{
					truePointParams.pCurrent[i] = pressureAmbient(FGas, 0, pressureSeaLevel);
				}
				int count = index;
				int j = 0;
				double airConsumption = 0;
				double vConsumption = 30;
				double[] check = new double[17];
				double max = 0;
				while (count > 0)
				{
					compartmentParams = new CompartmentParams(DivePoints[j].Gas.Helium);
					FGas = 1 - DivePoints[j].Gas.Oxygen;
					double vRate = (DivePoints[j + 1].Depth - DivePoints[j].Depth) / (DivePoints[j + 1].Time - DivePoints[j].Time);
					airConsumption += findAirConsumption(vConsumption, DivePoints[j + 1].Time - DivePoints[j].Time, DivePoints[j].Depth, DivePoints[j + 1].Depth);
					for (int i = 0; i < truePointParams.pCurrent.Length; i++)
					{
						truePointParams.pCurrent[i] = pressureAtoB(pressureAmbient(FGas, DivePoints[j].Depth, pressureSeaLevel), FGas, vRate, DivePoints[j + 1].Time - DivePoints[j].Time, Kf(compartmentParams.compartment.paramT[i]), truePointParams.pCurrent[i]);
					}
					j += 1;
					count -= 1;
				}
				for (int i = 0; i < truePointParams.pCurrent.Length; i++)
				{
					check[i] = truePointParams.pCurrent[i] - pressureInCompartment(compartmentParams.compartment.paramM0[i], compartmentParams.compartment.paramdM[i], DivePoints[ind].Depth);
					if (check[i] > max)
					{
						max = check[i];
					}
				}
				if (max > 0)
				{
					return false;
				}
				else
				{
					return true;
				}
			}

		}

		public string EmergencyAscentMessage(int index)
		{
			string emergencyMessage = "";
			Air air = new Air();
			List<DivePoint> myDivePoint = new List<DivePoint>();
			CompartmentParams compartmentParams = new CompartmentParams(DivePoints[0].Gas.Helium);
			double FGas = 1 - DivePoints[0].Gas.Oxygen;
			double[] check = new double[17];
			double[] d = new double[17];
			List<string> st = new List<string> { };
			double em = 0;
			double maxD = 0;
			double timeEm = 0;
			PointParams truePointParams = new PointParams();
			truePointParams.pCurrent = new double[17];
			for (int i = 0; i < truePointParams.pCurrent.Length; i++)
			{
				truePointParams.pCurrent[i] = pressureAmbient(FGas, 0, pressureSeaLevel);
				Console.WriteLine("первое давление = " + truePointParams.pCurrent[i]);
			}
			int count = index;
			int j = 0;
			double airConsumption = 0;
			double vConsumption = 30;
			while (count > 0)
			{
				compartmentParams = new CompartmentParams(DivePoints[j].Gas.Helium);
				FGas = 1 - DivePoints[j].Gas.Oxygen;
				double vRate = (DivePoints[j + 1].Depth - DivePoints[j].Depth) / (DivePoints[j + 1].Time - DivePoints[j].Time);
				airConsumption += findAirConsumption(vConsumption, DivePoints[j + 1].Time - DivePoints[j].Time, DivePoints[j].Depth, DivePoints[j + 1].Depth);
				for (int i = 0; i < truePointParams.pCurrent.Length; i++)
				{
					truePointParams.pCurrent[i] = pressureAtoB(pressureAmbient(FGas, DivePoints[j].Depth, pressureSeaLevel), FGas, vRate, DivePoints[j + 1].Time - DivePoints[j].Time, Kf(compartmentParams.compartment.paramT[i]), truePointParams.pCurrent[i]);
					Console.WriteLine("давление после шага номер " + j + " = " + truePointParams.pCurrent[i]);
				}
				j += 1;
				count -= 1;
			}
			compartmentParams = new CompartmentParams(DivePoints[j].Gas.Helium);
			FGas = 1 - DivePoints[j].Gas.Oxygen;
			for (int i = 0; i < truePointParams.pCurrent.Length; i++)
			{
				truePointParams.pCurrent[i] = pressureAtoB(pressureAmbient(FGas, DivePoints[j].Depth, pressureSeaLevel), FGas, -18, DivePoints[j].Depth / 18, Kf(compartmentParams.compartment.paramT[i]), truePointParams.pCurrent[i]);
				check[i] = truePointParams.pCurrent[i] - pressureInCompartment(compartmentParams.compartment.paramM0[i], compartmentParams.compartment.paramdM[i], 0);
				if (check[i] > em)
				{
					em = check[i];
				}
			}
			compartmentParams = new CompartmentParams(air.Helium);
			FGas = 1 - air.Oxygen;
			if (em > 0)
			{
				for (int i = 0; i < truePointParams.pCurrent.Length; i++)
				{
					d[i] = (truePointParams.pCurrent[i] - compartmentParams.compartment.paramM0[i]) / compartmentParams.compartment.paramM0[i];
					if (d[i] > maxD)
					{
						maxD = d[i];
					}
				}
				string s = "Декомпрессионные параметры в барокамере:\n";
				maxD = Math.Ceiling(maxD);
				if (maxD % 3 != 0)
				{
					maxD = maxD - (maxD % 3) + 3;
				}
				while (maxD > 0)
				{
					timeEm = maxTimeToStop(truePointParams.pCurrent, compartmentParams.compartment.paramM0, compartmentParams.compartment.paramM0, pressureAmbient(FGas, maxD, 10), maxD, KF(compartmentParams.compartment.paramT));
					for (int i = 0; i < truePointParams.pCurrent.Length; i++)
					{
						truePointParams.pCurrent[i] = pressurePoint(pressureAmbient(FGas, maxD, 10), truePointParams.pCurrent[i], Kf(compartmentParams.compartment.paramT[i]), timeEm);
					}
					string str = "";
					str = $"поместить на {timeEm} минут при давлении {maxD / 10 + 1} Бар ";
					st.Add(str);
					maxD = maxD - 3;
				}

				emergencyMessage = s + string.Join("\n", st);
			}
			else
			{
				emergencyMessage = "Экстренное всплытие не требует декомпрессионных обязательств";
			}

			return emergencyMessage;
		}

	}

}

