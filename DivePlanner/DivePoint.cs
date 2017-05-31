using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;

namespace DivePlanner
{
	class DivePoint : IComparable<DivePoint>
	{
		public double Time { get; set; } // in seconds
		public double Depth { get; set; } // in meters
		public GasMix Gas { get; set; }

		public DivePoint(double time, double depth)
		{
			Time = time;
			Depth = depth;
			Gas = new Air();
		}

		public int CompareTo(DivePoint other)
		{
			if (other == null) return 1;
			else
				return this.Time.CompareTo(other.Time);
		}

		public override int GetHashCode()
		{
			return Time.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == null) return false;
			DivePoint objDP = obj as DivePoint;
			if (objDP == null) return false;
			else return Equals(objDP);
		}

		public bool Equals(DivePoint other)
		{
			if (other == null) return false;
			return (this.Time.Equals(other.Time));
		}
	}
}
