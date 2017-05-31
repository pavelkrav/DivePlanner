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
