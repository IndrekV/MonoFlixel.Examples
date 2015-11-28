/**
 * 
 * @author Indrek Vändrik
 * Based on the flixel-gdx example by Ka Wing Chin
 */

#region Using Statements
using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;
using MonoFlixel;

#endregion

namespace MonoFlixel.Examples
{
	public class BunnyPlayState : FlxState
	{
		private const String ImgGrass = "Bunnymark/grass.png";
		private const int INITIAL_AMOUNT = 500;
		private const float INTERVAL = 0.1f;
		private string _buffer;
		private Boolean _complex;
		private Boolean _collisions;
		
		private FlxTimer _memoryTimer;
		private FlxGroup _bunnies;
		private FlxButton _complexityButton;
		private FlxButton _collisionButton;
		private FlxText _bunnyCounter;
		private FlxText _memoryUsage;

		public override void create ()
		{

			FlxG.debug = true;
					
			// The grass background
			FlxSprite grass = new FlxSprite (0, 0).loadGraphic (ImgGrass, false, false, FlxG.width, FlxG.height);
			add (grass);

			add (_bunnies = new FlxGroup ());
			
			
			// Text display
			add (_bunnyCounter = new FlxText (0, FlxG.height - 20, FlxG.width).setFormat (null, 8, Color.White, "right", Color.Black));
			add (_memoryUsage = new FlxText (5, FlxG.height - 20, 200).setFormat (null, 8, Color.White, "left", Color.Black));
			
			// Buttons Left side
			float leftBtnY = 20;

			add (new FlxButton (leftBtnY, 25, "+" + INITIAL_AMOUNT, addBunnies));
			add (new FlxButton (leftBtnY, 50, "-" + INITIAL_AMOUNT, removeBunnies));
			
			// Buttons right side
			float rightBtnX = FlxG.width - 100;
			add (_complexityButton = new FlxButton (rightBtnX, 25, "Complex", ComplexityCallback));			
			add (_collisionButton = new FlxButton (rightBtnX, 65, "Collision ON", CollisionCallback));			
			
			// Finally create the bunnies
			addBunnies ();
			
			// Force GC
			GC.Collect ();
			
			// Timer to update the memory usage
			_memoryTimer = new FlxTimer ();
			updateMemoryUsage ();

			// Show mouse pointer
			FlxG.mouse.show ();
		}

		public void ComplexityCallback ()
		{
			_complex = !_complex;
			_complexityButton.Label.text = (_complex ? "Simple" : "Complex");

			foreach (FlxBasic bunny in _bunnies.Members)
				((Bunny)bunny).setComplex (_complex);
		}

		public void CollisionCallback ()
		{
			_collisions = !_collisions;
			_collisionButton.Label.text = (_collisions ? "Collision OFF" : "Collision ON");
		}

		
		public override void update ()
		{
			base.update ();
			if (_collisions)
				FlxG.collide (_bunnies);
			if (_memoryTimer.time != 0) {
				_memoryTimer.update ();
			}
			if (FlxG.keys.justPressed (Keys.Escape))
				onQuit ();
		}

		public void addBunnies ()
		{
			float rx;
			float ry;
			for (int i = 0; i < 500; i++) {
				rx = FlxG.random () * (FlxG.width - 26);
				ry = FlxG.random () * (FlxG.height - 37);
				_bunnies.add ((FlxBasic)new Bunny (rx, ry));
			}
			_bunnyCounter.text = "Bunnies: " + _bunnies.length;
		}

		public void removeBunnies ()
		{
			FlxBasic bunny;
			for (int i = 0; i < 500; i++) {
				bunny = _bunnies.GetFirstAlive ();
				if (bunny != null) {
					bunny = _bunnies.Remove (bunny, true);
					bunny.destroy ();
				}
			}
			_bunnies.Members.TrimExcess ();

			_bunnyCounter.text = "Bunnies: " + _bunnies.length;
		}

		public void updateMemoryUsage ()
		{
			// Infinite loop
			_memoryTimer.start (INTERVAL, 1, updateMemoryCallback);
		}

		public bool updateMemoryCallback (FlxTimer Timer)
		{
			_buffer = ((GC.GetTotalMemory (false) / 1024f) / 1024f).ToString ("0.00");
			_memoryUsage.text = _buffer + " Mb";

			updateMemoryUsage ();
			return true;
		}

		//This just quits - state.destroy() is automatically called upon state changing
		private void onQuit() {
			FlxG.switchState(new MenuState());
		}
	}
}