/**
 * 
 * @author Indrek Vändrik
 * Based on the flixel example by Tim Plummer
 */

using System;
using MonoFlixel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoFlixel.Examples
{
	public class Tilemap : FlxState
	{
		public Tilemap ()
		{
		}

		// Tileset that works with AUTO mode (best for thin walls)
		private string auto_tiles = "Tilemap/auto_tiles.png";

		// Tileset that works with ALT mode (best for thicker walls)
		private string alt_tiles = "Tilemap/alt_tiles.png";

		// Tileset that works with OFF mode (do what you want mode)
		private string empty_tiles = "Tilemap/empty_tiles.png";

		// Default tilemaps. Embedding text files is a little weird.
		private string default_auto = "Tilemap/default_auto.txt";
		private string default_alt = "Tilemap/default_alt.txt";
		private string default_empty = "Tilemap/default_empty.txt";

		private string ImgSpaceman = "Tilemap/spaceman.png";

		// Some static constants for the size of the tilemap tiles
		private const int TILE_WIDTH = 16;
		private const int TILE_HEIGHT = 16;

		// The FlxTilemap we're using
		private FlxTilemap collisionMap;

		// Box to show the user where they're placing stuff
		private FlxObject highlightBox;

		// Player modified from "Mode" demo
		private FlxSprite player;

		// Some interface buttons and text
		private FlxButton autoAltBtn;
		private FlxButton resetBtn;
		private FlxButton quitBtn;
		private FlxText helperTxt;

		override public void create ()
		{
			FlxG.Framerate = 50;
			//FlxG.flashFramerate = 50;

			// Creates a new tilemap with no arguments
			collisionMap = new FlxTilemap ();

			/*
			 * FlxTilemaps are created using strings of comma seperated values (csv)
			 * This string ends up looking something like this:
			 *
			 * 0,0,0,0,0,0,0,0,0,0,
			 * 0,0,0,0,0,0,0,0,0,0,
			 * 0,0,0,0,0,0,1,1,1,0,
			 * 0,0,1,1,1,0,0,0,0,0,
			 * ...
			 *
			 * Each '0' stands for an empty tile, and each '1' stands for
			 * a solid tile
			 *
			 * When using the auto map generation, the '1's are converted into the corresponding frame
			 * in the tileset.
			 */

			// Initializes the map using the generated string, the tile images, and the tile size
			collisionMap.loadMapFile (default_auto, auto_tiles, (int)TILE_WIDTH, (int)TILE_HEIGHT, FlxTilemap.AUTO);
			add (collisionMap);

			highlightBox = new FlxObject (0, 0, TILE_WIDTH, TILE_HEIGHT);

			setupPlayer ();

			// When switching between modes here, the map is reloaded with it's own data, so the positions of tiles are kept the same
			// Notice that different tilesets are used when the auto mode is switched
			autoAltBtn = new FlxButton (4, FlxG.height - 24, "AUTO", altButtonFunc);
			add (autoAltBtn);

			resetBtn = new FlxButton (8 + autoAltBtn.Width, FlxG.height - 24, "Reset", resetButtonFunc);
			add (resetBtn);

			quitBtn = new FlxButton (FlxG.width - resetBtn.Width - 4, FlxG.height - 24, "Quit", onQuit);
			add (quitBtn);

			helperTxt = new FlxText (5, 5, 150, "Click to place tiles. Shift-Click to remove tiles\nArrow keys to move");
			add (helperTxt);

			// Show mouse pointer
			FlxG.mouse.show ();
		}

		private void altButtonFunc ()
		{
			switch (collisionMap.auto) {
			case FlxTilemap.AUTO:
				collisionMap.loadMap (FlxTilemap.arrayToCSV (collisionMap.getData (true), collisionMap.widthInTiles),
					alt_tiles, TILE_WIDTH, TILE_HEIGHT, FlxTilemap.ALT);
				autoAltBtn.Label.text = "ALT";
				break;

			case FlxTilemap.ALT:
				collisionMap.loadMap (FlxTilemap.arrayToCSV (collisionMap.getData (true), collisionMap.widthInTiles),
					empty_tiles, TILE_WIDTH, TILE_HEIGHT, FlxTilemap.OFF);
				autoAltBtn.Label.text = "OFF";
				break;

			case FlxTilemap.OFF:
				collisionMap.loadMap (FlxTilemap.arrayToCSV (collisionMap.getData (true), collisionMap.widthInTiles),
					auto_tiles, TILE_WIDTH, TILE_HEIGHT, FlxTilemap.AUTO);
				autoAltBtn.Label.text = "AUTO";
				break;
			}

		}

		private void resetButtonFunc ()
		{
			switch (collisionMap.auto) {
			case FlxTilemap.AUTO:
				collisionMap.loadMapFile (default_auto, auto_tiles, TILE_WIDTH, TILE_HEIGHT, FlxTilemap.AUTO);
				player.X = 64;
				player.Y = 220;
				break;

			case FlxTilemap.ALT:
				collisionMap.loadMapFile (default_alt, alt_tiles, TILE_WIDTH, TILE_HEIGHT, FlxTilemap.ALT);
				player.X = 64;
				player.Y = 128;
				break;

			case FlxTilemap.OFF:
				collisionMap.loadMapFile (default_empty, empty_tiles, TILE_WIDTH, TILE_HEIGHT, FlxTilemap.OFF);
				player.X = 64;
				player.Y = 64;
				break;
			}
		}

		override public void update ()
		{
			// Tilemaps can be collided just like any other FlxObject, and flixel
			// automatically collides each individual tile with the object.
			FlxG.collide (player, collisionMap);

			highlightBox.X = (float)Math.Floor (FlxG.mouse.cursor.X / TILE_WIDTH) * TILE_WIDTH;
			highlightBox.Y = (float)Math.Floor (FlxG.mouse.cursor.Y / TILE_HEIGHT) * TILE_HEIGHT;

			if (FlxG.mouse.pressed ()) {
				// FlxTilemaps can be manually edited at runtime as well.
				// Setting a tile to 0 removes it, and setting it to anything else will place a tile.
				// If auto map is on, the map will automatically update all surrounding tiles.
				collisionMap.setTile ((uint)FlxG.mouse.x / TILE_WIDTH, (uint)FlxG.mouse.y / TILE_HEIGHT, (uint)(FlxG.keys.pressed (Keys.LeftShift) ? 0 : 1));
			}

			updatePlayer ();
			if (FlxG.keys.justPressed (Keys.Escape))
				onQuit ();
			base.update ();
		}

		public override void draw ()
		{
			base.draw ();
			highlightBox.drawDebug ();
		}

		private void setupPlayer ()
		{
			player = new FlxSprite (64, 220);
			player.loadGraphic (ImgSpaceman, true, true, 16);

			//bounding box tweaks
			player.Width = 14;
			player.Height = 14;
			player.Offset.X = 1;
			player.Offset.Y = 1;

			//basic player physics
			player.Drag.X = 640;
			player.Acceleration.Y = 420;
			player.MaxVelocity.X = 80;
			player.MaxVelocity.Y = 200;

			//animations
			player.addAnimation ("idle", new int[]{ 0 });
			player.addAnimation ("run", new int[]{ 1, 2, 3, 0 }, 12);
			player.addAnimation ("jump", new int[]{ 4 });

			add (player);
		}

		private void updatePlayer ()
		{
			wrap (player);

			//MOVEMENT
			player.Acceleration.X = 0;
			if (FlxG.keys.pressed (Keys.Left)) {
				player.Facing = FlxObject.Left;
				player.Acceleration.X -= player.Drag.X;
			} else if (FlxG.keys.pressed (Keys.Right)) {
				player.Facing = FlxObject.Right;
				player.Acceleration.X += player.Drag.X;
			}
			if (FlxG.keys.justPressed (Keys.Up) && player.Velocity.Y == 0) {
				player.Y -= 1;
				player.Velocity.Y = -200;
			}

			//ANIMATION
			if (player.Velocity.Y != 0) {
				player.play ("jump");
			} else if (player.Velocity.X == 0) {
				player.play ("idle");
			} else {
				player.play ("run");
			}
		}

		private void wrap (FlxObject obj)
		{
			obj.X = (obj.X + obj.Width / 2 + FlxG.width) % FlxG.width - obj.Width / 2;
			obj.Y = (obj.Y + obj.Height / 2) % FlxG.height - obj.Height / 2;
		}

		//This just quits - state.destroy() is automatically called upon state changing
		private void onQuit ()
		{
			FlxG.switchState (new MenuState ());
		}

	}
}

