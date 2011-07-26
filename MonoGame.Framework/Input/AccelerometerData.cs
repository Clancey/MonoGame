using System;
using System.Runtime.Serialization;
namespace Microsoft.Xna.Framework.Input
{
	[DataContract]
	public class AccelerometerData
	{
		public AccelerometerData () 
		{
			
		}
		public AccelerometerData (double time,double x,double y, double z)
		{
			Time = time;
			X = x;
			Y = y;
			Z = z;
		}
		[DataMember]
		public double Time {get;set;}
		[DataMember]
		public double X {get;set;}
		[DataMember]
		public double Y {get;set;}
		[DataMember]
		public double Z {get;set;}
	}
}

