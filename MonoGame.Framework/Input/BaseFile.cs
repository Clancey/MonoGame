using System;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework.Input.Touch;
using System.Collections.Generic;
namespace Microsoft.Xna.Framework.Input
{
	public class BaseFile
	{
		public BaseFile()
		{
			
		}
		public AccelerometerData Accel {get;set;}
		public List<TouchLocation> Touches {get;set;}
	}
}

