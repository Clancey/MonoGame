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
using System.Collections.Generic;
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
			Thread thread2 = new Thread (startListeningTouches);
			thread2.Start ();
		}
		public static void startListeningTouches ()
		{
			bool done = false;
			
			UdpClient touchesListener = new UdpClient (10553);
			IPEndPoint touchedGroup = new IPEndPoint (IPAddress.Any, 10553);
			while (true) {
				try {
					//Console.WriteLine ("trying touches");
					byte[] bytes = touchesListener.Receive (ref touchedGroup);
					var json = Encoding.ASCII.GetString (bytes, 0, bytes.Length);
					
					//Console.WriteLine(json);
					MemoryStream ms = new MemoryStream (Encoding.Unicode.GetBytes (json));
					System.Runtime.Serialization.Json.DataContractJsonSerializer serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer (typeof(List<TouchLocation>));
					var Touches = serializer.ReadObject (ms) as List<TouchLocation>;
					ms.Close ();
					//Console.WriteLine(accel);				
					if (Touches != null) {
						TouchPanel.Reset ();
						TouchPanel.Collection.AddRange (Touches);
					}
					
				} catch (Exception ex) {
					Console.WriteLine (ex);
				}
			}
			
			
		}
		static double lastTime;
		private static void startlistening ()
		{
			bool done = false;
			
			UdpClient listener = new UdpClient (ListeningPort);
			
			IPEndPoint groupEP = new IPEndPoint (IPAddress.Any, ListeningPort);
			while (true) {
				try {
					//Console.WriteLine ("Waiting for broadcast");
					byte[] bytes = listener.Receive (ref groupEP);
					var json = Encoding.ASCII.GetString (bytes, 0, bytes.Length);
					
					//Console.WriteLine (json);
					MemoryStream ms = new MemoryStream (Encoding.Unicode.GetBytes (json));
					System.Runtime.Serialization.Json.DataContractJsonSerializer serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer (typeof(AccelerometerData));
					var accel = serializer.ReadObject (ms) as AccelerometerData;
					ms.Close ();
					//Console.WriteLine(accel);
					if (accel != null)
						Accelerometer.UIAccelerometerSharedAccelerometerAcceleration (SharedAccelerometer, accel.X, accel.Y, accel.Z);
					
					
				} catch (Exception ex) {
					Console.WriteLine (ex);
				}
			}
		}
		
		
	}
}




