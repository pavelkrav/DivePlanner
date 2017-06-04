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

	}
}
