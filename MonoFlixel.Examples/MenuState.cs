using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;
using MonoFlixel;

namespace MonoFlixel.Examples
{
	public class MenuState : FlxState
	{
		public FlxGroup _items;
		public List<FlxState> _states;
		public FlxText _selected;
		public int selectedID = 0;
		public int startY = 50;
		public MenuState ()
		{
		}

		public override void create()
		{
			FlxG.bgColor = FlxColor.BLACK;
			_items = new FlxGroup ();
			_states = new List<FlxState> ();

			_states.Add (new Platformer());
			_states.Add (new SplitScreen());
			_states.Add (new BunnyPlayState());
			_states.Add (new ModePlayState());
			_states.Add (new Particles());
			_states.Add (new Collision());
			_states.Add (new Tilemap());

			_items.add (new FlxText(0, startY, FlxG.width, "Platformer").setFormat (null, 8, Color.White, "center", Color.Black));
			_items.add (new FlxText(0, (startY += 20), FlxG.width, "SplitScreen").setFormat (null, 8, Color.White, "center", Color.Black));
			_items.add (new FlxText(0, (startY += 20), FlxG.width, "BunnyMark").setFormat (null, 8, Color.White, "center", Color.Black));
			_items.add (new FlxText(0, (startY += 20), FlxG.width, "Mode").setFormat (null, 8, Color.White, "center", Color.Black));
			_items.add (new FlxText(0, (startY += 20), FlxG.width, "Particles").setFormat (null, 8, Color.White, "center", Color.Black));
			_items.add (new FlxText(0, (startY += 20), FlxG.width, "Collision").setFormat (null, 8, Color.White, "center", Color.Black));
			_items.add (new FlxText(0, (startY += 20), FlxG.width, "Auto Tilemap").setFormat (null, 8, Color.White, "center", Color.Black));

			add(_items);

			add (new FlxText(0, 15, FlxG.width, "-- MonoFlixel Examples --").setFormat (null, 16, Color.White, "center", Color.Black));

			_selected = new FlxText (FlxG.width/2-80, ((FlxText)_items.Members [selectedID]).Y, FlxG.width, ">").setFormat (null, 8, Color.White, "left", Color.Black);

			add (_selected);
		}

		public void updateSelected () {
			_selected.Y = ((FlxText)_items.Members [selectedID]).Y;
		}

		public override void update()
		{
			base.update ();
			if (FlxG.keys.justPressed (Keys.Up)) {
				if (selectedID == 0) {
					selectedID = (int)_items.length - 1;
				} else {
					selectedID--;
				}
				updateSelected ();
			}
			if (FlxG.keys.justPressed (Keys.Down)) {
				if (selectedID == (int)_items.length - 1) {
					selectedID = 0;
				} else {
					selectedID++;
				}
				updateSelected ();
			}
			if (FlxG.keys.justPressed (Keys.Enter)) {
				FlxG.switchState (_states[selectedID]);
			}
		}
	}
}

