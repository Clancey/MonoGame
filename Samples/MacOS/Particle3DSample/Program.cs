using MonoMac.AppKit;
using MonoMac.Foundation;

namespace Particle3DSample
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main (string[] args)
		{
			NSApplication.Init ();

			using (var p = new NSAutoreleasePool ()) {
				NSApplication.SharedApplication.Delegate = new AppDelegate ();
				NSApplication.Main (args);
			}


		}
	}

	class AppDelegate : NSApplicationDelegate
	{

		public override void FinishedLaunching (MonoMac.Foundation.NSObject notification)
		{
			using (Particle3DSampleGame game = new Particle3DSampleGame ()) {
				game.Run ();
			}
		}

		public override bool ApplicationShouldTerminateAfterLastWindowClosed (NSApplication sender)
		{
			return true;
		}
	}		
}

