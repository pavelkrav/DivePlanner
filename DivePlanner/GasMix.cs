using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DivePlanner
{
	class GasMix
	{
		private double _oxygen;
		public double Oxygen
		{
			get { return _oxygen; }
			set { _oxygen = value; }
		}

		private bool _helium; // true if helium, false if nitrogen
		public bool Helium
		{
			get { return _helium; }
			set { _helium = value; }
		}


		public GasMix(double oxygen, bool helium)
		{
			if (oxygen > 1 || oxygen < 0)
				throw new ArgumentOutOfRangeException();
			_oxygen = oxygen;
			_helium = helium;
		}
	}

	class Air : GasMix
	{
		public Air() : base(0.2315, false) { }
	}

	class Nitrox32 : GasMix
	{
		public Nitrox32() : base(0.32, false) { }
	}

	class Nitrox36 : GasMix
	{
		public Nitrox36() : base(0.36, false) { }
	}
}
