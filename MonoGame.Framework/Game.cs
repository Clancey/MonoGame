#region License
/*
Microsoft Public License (Ms-PL)
XnaTouch - Copyright © 2009 The XnaTouch Team

All rights reserved.

This license governs use of the accompanying software. If you use the software, you accept this license. If you do not
accept the license, do not use the software.

1. Definitions
The terms "reproduce," "reproduction," "derivative works," and "distribution" have the same meaning here as under 
U.S. copyright law.

A "contribution" is the original software, or any additions or changes to the software.
A "contributor" is any person that distributes its contribution under this license.
"Licensed patents" are a contributor's patent claims that read directly on its contribution.

2. Grant of Rights
(A) Copyright Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, 
each contributor grants you a non-exclusive, worldwide, royalty-free copyright license to reproduce its contribution, prepare derivative works of its contribution, and distribute its contribution or any derivative works that you create.
(B) Patent Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, 
each contributor grants you a non-exclusive, worldwide, royalty-free license under its licensed patents to make, have made, use, sell, offer for sale, import, and/or otherwise dispose of its contribution in the software or derivative works of the contribution in the software.

3. Conditions and Limitations
(A) No Trademark License- This license does not grant you rights to use any contributors' name, logo, or trademarks.
(B) If you bring a patent claim against any contributor over patents that you claim are infringed by the software, 
your patent license from such contributor to the software ends automatically.
(C) If you distribute any portion of the software, you must retain all copyright, patent, trademark, and attribution 
notices that are present in the software.
(D) If you distribute any portion of the software in source code form, you may do so only under this license by including 
a complete copy of this license with your distribution. If you distribute any portion of the software in compiled or object 
code form, you may only do so under a license that complies with this license.
(E) The software is licensed "as-is." You bear the risk of using it. The contributors give no express warranties, guarantees
or conditions. You may have additional consumer rights under your local laws which this license cannot change. To the extent
permitted under your local laws, the contributors exclude the implied warranties of merchantability, fitness for a particular
purpose and non-infringement.
*/
#endregion License
   
using System;
using System.IO;

using MonoTouch.CoreAnimation;
using MonoTouch.CoreFoundation;
using MonoTouch.Foundation;
using MonoTouch.OpenGLES;
using MonoTouch.UIKit;

using OpenTK.Graphics;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace Microsoft.Xna.Framework
{
	internal class GameVc : UIViewController
	{
		Game Game;
		public GameVc(Game game):base()
		{
			Game = game;
			this.View = game.View;
		}
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
		
			var manager = Game.graphicsDeviceManager as GraphicsDeviceManager;
			Console.WriteLine(manager == null);
			if(manager == null)
				return true;
			DisplayOrientation supportedOrientations = manager.SupportedOrientations;
			switch(toInterfaceOrientation)
			{
			case UIInterfaceOrientation.LandscapeLeft :
				return (supportedOrientations & DisplayOrientation.LandscapeLeft) != 0;
			case UIInterfaceOrientation.LandscapeRight:
				return (supportedOrientations & DisplayOrientation.LandscapeRight) != 0;
			case UIInterfaceOrientation.Portrait:
				return (supportedOrientations & DisplayOrientation.Portrait) != 0;
			case UIInterfaceOrientation.PortraitUpsideDown :
				return (supportedOrientations & DisplayOrientation.PortraitUpsideDown) != 0;
			default :
				return false;
			}
			return true;
			//return base.ShouldAutorotateToInterfaceOrientation (toInterfaceOrientation);
		}
		public override void DidRotate (UIInterfaceOrientation fromInterfaceOrientation)
		{
			/*
			switch(this.InterfaceOrientation)
			{
			case UIInterfaceOrientation.LandscapeLeft:
			case UIInterfaceOrientation.LandscapeRight:
				Game.View.Frame = new System.Drawing.RectangleF(this.View.Frame.Location,new System.Drawing.SizeF(this.View.Frame.Height,this.View.Frame.Width));
				break;
			default:				
				Game.View.Frame = new System.Drawing.RectangleF(this.View.Frame.Location,new System.Drawing.SizeF(this.View.Frame.Width,this.View.Frame.Height));
				break;
			}
			//Game.View.Frame = this.View.Frame;
			Console.WriteLine("Main View's Frame:" + Game.View.Frame);
			*/
			base.DidRotate (fromInterfaceOrientation);
		}
	}
    public class Game : IDisposable
    {
		private const float FramesPerSecond = 60.0f; // ~60 frames per second
		
        private GameTime _updateGameTime;
        private GameTime _drawGameTime;
        private DateTime _lastUpdate;
        private bool _initialized = false;
		private bool _initializing = false;
		private bool _isActive = true;
        private GameComponentCollection _gameComponentCollection;
        public GameServiceContainer _services;
        private ContentManager _content;
        public GameWindow View;
		private GameVc gameVc;
		private bool _isFixedTimeStep = true;
        private TimeSpan _targetElapsedTime = TimeSpan.FromSeconds(1 / FramesPerSecond); 
        
		internal IGraphicsDeviceManager graphicsDeviceManager;
		private IGraphicsDeviceService graphicsDeviceService;
		private UIWindow _mainWindow;

		internal static bool _playingVideo = false;
		private SpriteBatch spriteBatch;
		private Texture2D splashScreen;
		
		delegate void InitialiseGameComponentsDelegate();
		
		public Game()
        {           
			// Initialize collections
			_services = new GameServiceContainer();
			_gameComponentCollection = new GameComponentCollection();

			//Create a full-screen window
			_mainWindow = new UIWindow (UIScreen.MainScreen.Bounds);			
			View = new GameWindow();
			View.game = this;
			gameVc = new GameVc(this);
			_mainWindow.Add(gameVc.View);							
					
			// Initialize GameTime
            _updateGameTime = new GameTime();
            _drawGameTime = new GameTime();  	
		}
		
		~Game()
		{
			UIDevice.CurrentDevice.EndGeneratingDeviceOrientationNotifications(); 
		}
		
		private void ObserveDeviceRotation ()
		{
			NSNotificationCenter.DefaultCenter.AddObserver( new NSString("UIDeviceOrientationDidChangeNotification"), (notification) => { 
			UIDeviceOrientation orientation = UIDevice.CurrentDevice.Orientation;
				
				// Calculate supported orientations if it has been left as "default"
            DisplayOrientation supportedOrientations = (graphicsDeviceManager as GraphicsDeviceManager).SupportedOrientations;
            if ((supportedOrientations & DisplayOrientation.Default) != 0)
            {
                if (GraphicsDevice.PresentationParameters.BackBufferWidth > GraphicsDevice.PresentationParameters.BackBufferHeight)
                {
                    supportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
                }
                else
                {
                    supportedOrientations = DisplayOrientation.Portrait | DisplayOrientation.PortraitUpsideDown;
                }
            }
				//Console.WriteLine(orientation);
				switch (orientation)
				{
					case UIDeviceOrientation.Portrait :
						if ((supportedOrientations & DisplayOrientation.Portrait) != 0)
						{
							View.CurrentOrientation = DisplayOrientation.Portrait;
							GraphicsDevice.PresentationParameters.DisplayOrientation = DisplayOrientation.Portrait;
							TouchPanel.DisplayOrientation = DisplayOrientation.Portrait;
						}
						break;
					case UIDeviceOrientation.LandscapeLeft :
						if ((supportedOrientations & DisplayOrientation.LandscapeLeft) != 0)
			            {
			            	View.CurrentOrientation = DisplayOrientation.LandscapeLeft;
			             	GraphicsDevice.PresentationParameters.DisplayOrientation = DisplayOrientation.LandscapeLeft;
			              	TouchPanel.DisplayOrientation = DisplayOrientation.LandscapeLeft;							
			            }
						break;
					case UIDeviceOrientation.LandscapeRight :						
						if ((supportedOrientations & DisplayOrientation.LandscapeRight) != 0)
			            {
			            	View.CurrentOrientation = DisplayOrientation.LandscapeRight;
			            	GraphicsDevice.PresentationParameters.DisplayOrientation = DisplayOrientation.LandscapeRight;
			            	TouchPanel.DisplayOrientation = DisplayOrientation.LandscapeRight;							
			            }
						break;
					case UIDeviceOrientation.FaceDown :
						if ((supportedOrientations & DisplayOrientation.FaceDown) != 0)
						{
							View.CurrentOrientation = DisplayOrientation.FaceDown;
							GraphicsDevice.PresentationParameters.DisplayOrientation = DisplayOrientation.FaceDown;
							TouchPanel.DisplayOrientation = DisplayOrientation.FaceDown;							
						}
						break;
					case UIDeviceOrientation.FaceUp :
						if ((supportedOrientations & DisplayOrientation.FaceUp) != 0)						
						{
							View.CurrentOrientation = DisplayOrientation.FaceUp;
							GraphicsDevice.PresentationParameters.DisplayOrientation = DisplayOrientation.FaceUp;
							TouchPanel.DisplayOrientation = DisplayOrientation.FaceUp;
						}
						break;
					case UIDeviceOrientation.PortraitUpsideDown :
						if ((supportedOrientations & DisplayOrientation.PortraitUpsideDown) != 0)						
						{
							View.CurrentOrientation = DisplayOrientation.PortraitUpsideDown;
							GraphicsDevice.PresentationParameters.DisplayOrientation = DisplayOrientation.PortraitUpsideDown;
							TouchPanel.DisplayOrientation = DisplayOrientation.PortraitUpsideDown;
						}
						break;
					case UIDeviceOrientation.Unknown :
						if ((supportedOrientations & DisplayOrientation.Unknown) != 0)						
						{
							View.CurrentOrientation = DisplayOrientation.Unknown;
							TouchPanel.DisplayOrientation = DisplayOrientation.Unknown;
						}
						break;						
					default:						
						break;
				}					  
			});
			
			UIDevice.CurrentDevice.BeginGeneratingDeviceOrientationNotifications();
		}
		
		public void Dispose ()
		{
			// do nothing
		}
		
        public bool IsActive
        {
            get
			{
				return _isActive;
			}
			protected set
			{
				if (_isActive != value )
				{
					_isActive = value;
				}
			}
        }

        public bool IsMouseVisible
        {
            get
			{
				return false;
			}
            set
			{
				// do nothing; ignore
			}
        }

        public TimeSpan TargetElapsedTime
        {
            get
            {
                return _targetElapsedTime;
            }
            set
            {
                _targetElapsedTime = value;			
				if(_initialized) {
					throw new NotSupportedException();
				}
            }
        }
		
        public void Run()
    	{			
			_lastUpdate = DateTime.Now;
			
			View.Run( FramesPerSecond / ( FramesPerSecond * TargetElapsedTime.TotalSeconds ) );	
			
			View.MainContext = View.EAGLContext;
			View.ShareGroup = View.MainContext.ShareGroup;
			View.BackgroundContext = new MonoTouch.OpenGLES.EAGLContext(View.ContextRenderingApi, View.ShareGroup);
			
			//Show the window			
			_mainWindow.MakeKeyAndVisible();	
			
			// Get the Accelerometer going
			Accelerometer.SetupAccelerometer();			
			Initialize();
			
			// Listen out for rotation changes
			ObserveDeviceRotation();
        }
		
		internal void DoUpdate(GameTime aGameTime)
		{
			if (_isActive)
			{
				Update(aGameTime);
			}
		}
		
		internal void DoDraw(GameTime aGameTime)
		{
			if (_isActive)
			{
				Draw(aGameTime);
			}
		}
		
		internal void DoStep()
		{
			var timeNow = DateTime.Now;
			
			// Update the game			
            _updateGameTime.Update(timeNow - _lastUpdate);
            Update(_updateGameTime);

            // Draw the screen
            _drawGameTime.Update(timeNow - _lastUpdate);
            _lastUpdate = timeNow;
            Draw(_drawGameTime);       			
		}

        public bool IsFixedTimeStep
        {
            get
			{
				return _isFixedTimeStep;
			}
            set
			{
				_isFixedTimeStep = value;
			}
        }

        public GameWindow Window
        {
            get
            {
                return View;
            }
        }
		
		public void ResetElapsedTime()
        {
            _lastUpdate = DateTime.Now;
        }


        public GameServiceContainer Services
        {
            get
            {
                return _services;
            }
		}

        public ContentManager Content
        {
            get
            {
                if (_content == null)
                {
                    _content = new ContentManager(_services);
                }
                return _content;
            }
        }

        public GraphicsDevice GraphicsDevice
        {
            get
            {
                if (this.graphicsDeviceService == null)
                {
                    this.graphicsDeviceService = this.Services.GetService(typeof(IGraphicsDeviceService)) as IGraphicsDeviceService;
                    if (this.graphicsDeviceService == null)
                    {
                        throw new InvalidOperationException("No Graphics Device Service");
                    }
                }
                return this.graphicsDeviceService.GraphicsDevice;
            }
        }
		
		public void EnterBackground()
    	{
			_isActive = false;
			 if (Deactivated != null)
                Deactivated.Invoke(this, null);
		}
		
		public void EnterForeground()
    	{
			_isActive = true;
			if (Activated != null)
                Activated.Invoke(this, null);
		}
		
		protected virtual bool BeginDraw()
		{
			return true;
		}
		
		protected virtual void EndDraw()
		{
			
		}
		
		protected virtual void LoadContent()
		{			
			string DefaultPath = "Default.png";
			if (File.Exists(DefaultPath))
			{
				// Store the RootDir for later 
				string backup = Content.RootDirectory;
				
				try 
				{
					// Clear the RootDirectory for this operation
					Content.RootDirectory = string.Empty;
					
					spriteBatch = new SpriteBatch(GraphicsDevice);
					splashScreen = Content.Load<Texture2D>(DefaultPath);			
				}
				finally 
				{
					// Reset RootDir
					Content.RootDirectory = backup;
				}
				
			}
			else
			{
				spriteBatch = null;
				splashScreen = null;
			}
		}
		
		protected virtual void UnloadContent()
		{
			// do nothing
		}
		
        protected virtual void Initialize()
        {
			this.graphicsDeviceManager = this.Services.GetService(typeof(IGraphicsDeviceManager)) as IGraphicsDeviceManager;			
			this.graphicsDeviceService = this.Services.GetService(typeof(IGraphicsDeviceService)) as IGraphicsDeviceService;			

			if ((this.graphicsDeviceService != null) && (this.graphicsDeviceService.GraphicsDevice != null))
            {
                LoadContent();
            }
        }
		
		private void InitializeGameComponents()
		{
			EAGLContext.SetCurrentContext(View.BackgroundContext);
			
			foreach (GameComponent gc in _gameComponentCollection)
            {
                gc.Initialize();
            }
			
			EAGLContext.SetCurrentContext(View.MainContext);
		}

        protected virtual void Update(GameTime gameTime)
        {			
			if ( _initialized  && !Guide.IsVisible )
			{
				foreach (GameComponent gc in _gameComponentCollection)			
				{
					if (gc.Enabled)
	                {
	                    gc.Update(gameTime);
	                }
	            }
			}
			else
			{
				if (!_initializing) 
				{
					_initializing = true;
					
					// Use OpenGLES context switching as described here
					// http://developer.apple.com/iphone/library/qa/qa2010/qa1612.html
					InitialiseGameComponentsDelegate initD = new InitialiseGameComponentsDelegate(InitializeGameComponents);

					// Invoke on thread from the pool
        			initD.BeginInvoke( 
						delegate (IAsyncResult iar) 
					    {
							// We must have finished initialising, so set our flag appropriately
							// So that we enter the Update loop
						    _initialized = true;
							_initializing = false;
						}, 
					initD);
				}
			}
        }
		
        protected virtual void Draw(GameTime gameTime)
        {
			if ( _initializing )
			{
				if ( spriteBatch != null )
				{
					spriteBatch.Begin();
					
					// We need to turn this into a progress bar or animation to give better user feedback
					spriteBatch.Draw(splashScreen, new Vector2(0, 0), Color.White );
					spriteBatch.End();
				}
			}
			else
			{
				if (!_playingVideo) 
				{
		            foreach (GameComponent gc in _gameComponentCollection)
		            {
		                if (gc.Enabled && gc is DrawableGameComponent)
		                {
		                    DrawableGameComponent dc = gc as DrawableGameComponent;
		                    if (dc.Visible)
		                    {
		                        dc.Draw(gameTime);
		                    }
		                }
		            }
				}
			}
        }

        public void Exit()
        {
			//TODO: Fix this
			UIAlertView alert = new UIAlertView("Game Exit", "Hit Home Button to Exit",null,null,null);
			alert.Show();		
        }

        public GameComponentCollection Components
        {
            get
            {
                return _gameComponentCollection;
            }
        }
		
		#region Events
		public event EventHandler Activated;
		public event EventHandler Deactivated;
		public event EventHandler Disposed;
		public event EventHandler Exiting;
		#endregion
    }
}

