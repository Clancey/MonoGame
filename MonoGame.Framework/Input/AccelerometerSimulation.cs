using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework.Input.Touch;
using System.Linq;
namespace Microsoft.Xna.Framework.Input
{
	internal class UIAccelerationSimulation : NSObject
	{
		public UIAcceleration Values;
		public DateTime Timestamp;

		public UIAccelerationSimulation (DateTime timeStamp, UIAcceleration values)
		{
			Values = values;
		}
	}

	internal class AccelerometerSimulation : UIAccelerometer
	{
		static AccelerometerSimulation ()
		{
			SharedAccelerometer = new AccelerometerSimulation ();
			//StartListener();
			//UIAccelerometer.SharedAccelerometer;
		}

		//public override
		public int UdpSocket { get; set; }
		public Thread Thread { get; set; }
		public bool IsExiting { get; set; }
		public UIAccelerometerDelegate AccelDelegate { get; set; }
		public UIAccelerationSimulation AccelObject { get; set; }
		public static AccelerometerSimulation SharedAccelerometer { get; private set; }
		public static int ListeningPort = 10552;

		public static void StartListener ()
		{
			Thread thread = new Thread (startlistening);
			thread.Start ();
		}
		static double lastTime;
		private static void startlistening ()
		{
			bool done = false;
			
			UdpClient listener = new UdpClient (ListeningPort);
			
			IPEndPoint groupEP = new IPEndPoint (IPAddress.Any, ListeningPort);
			
			try {
				while (!done) {
					Console.WriteLine ("Waiting for broadcast");
					byte[] bytes = listener.Receive (ref groupEP);
					var json = Encoding.ASCII.GetString (bytes, 0, bytes.Length);
					
					Console.WriteLine(json);
					MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(json));
					System.Runtime.Serialization.Json.DataContractJsonSerializer serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(BaseFile));
					var baseObject = serializer.ReadObject(ms) as BaseFile;
					ms.Close();
					var accel = baseObject.Accel;
					Console.WriteLine(accel);
					if(accel != null)
						Accelerometer.UIAccelerometerSharedAccelerometerAcceleration(SharedAccelerometer,accel.X,accel.Y,accel.Z);
					if(baseObject.Touches != null)
					{
						TouchPanel.Reset();
						TouchPanel.Collection.AddRange(baseObject.Touches);
					}
					
					
				}
				
			} catch (Exception e) {
				Console.WriteLine (e.ToString ());
			} finally {
				listener.Close ();
			}
		}
	}
}




