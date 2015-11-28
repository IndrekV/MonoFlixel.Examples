/**
 * 
 * @author Indrek Vändrik
 * Based on the flixel example by Adam Atomic
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
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class Platformer : FlxState
	{	
		public FlxTilemap level;
		public FlxSprite exit;
		public FlxGroup coins;
		public FlxSprite player;
		public FlxText score;
		public FlxText status;

		public Platformer ()
		{

		}

		public override void create()
		{
			// Background color
			FlxG.bgColor = (FlxColor.GREEN);
			FlxG.worldBounds = new FlxRect(0, 0, 320, 240);

			//Design your platformer level with 1s and 0s (at 40x30 to fill 320x240 screen)		
			int[] data = {
				1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
				1, 1, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 1, 1, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
				1, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
				1, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
				1, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
				1, 0, 0, 1, 1, 1, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
				1, 0, 0, 0, 1, 1, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 1,
				1, 0, 0, 0, 1, 1, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 1,
				1, 1, 0, 0, 1, 1, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
				1, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1,
				1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1,
				1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
				1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1,
				1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 1,
				1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
				1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1,
				1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1,
				1, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
				1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 1,
				1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 1,
				1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 1, 1, 1, 1, 1, 0, 0, 1,
				1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
				1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
				1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
				1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
				1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
				1, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
				1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 1, 1, 1, 1, 1, 0, 1, 0, 1, 1, 1, 1, 1, 0, 0, 0, 1, 1, 1, 1, 0, 1,
				1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 1,
				1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1};

			//Create a new tilemap using our level data
			level = new FlxTilemap ();
			level.loadMap (FlxTilemap.arrayToCSV (data, 40), FlxTilemap.ImgAuto, 0, 0, FlxTilemap.AUTO);
			add (level);

			//Create the level exit, a dark gray box that is hidden at first
			exit = new FlxSprite (35 * 8 + 1, 25 * 8);
			exit.makeGraphic (14, 16, FlxColor.BLUE);
			exit.Exists = false;
			add (exit);

			//Create coins to collect (see createCoin() function below for more info)
			coins = new FlxGroup ();
			//Top left coins
			createCoin (18, 4);
			createCoin (12, 4);
			createCoin (9, 4);
			createCoin (8, 11);
			createCoin (1, 7);
			createCoin (3, 4);
			createCoin (5, 2);
			createCoin (15, 11);
			createCoin (16, 11);

			//Bottom left coins
			createCoin (3, 16);
			createCoin (4, 16);
			createCoin (1, 23);
			createCoin (2, 23);
			createCoin (3, 23);
			createCoin (4, 23);
			createCoin (5, 23);
			createCoin (12, 26);
			createCoin (13, 26);
			createCoin (17, 20);
			createCoin (18, 20);

			//Top right coins
			createCoin (21, 4);
			createCoin (26, 2);
			createCoin (29, 2);
			createCoin (31, 5);
			createCoin (34, 5);
			createCoin (36, 8);
			createCoin (33, 11);
			createCoin (31, 11);
			createCoin (29, 11);
			createCoin (27, 11);
			createCoin (25, 11);
			createCoin (36, 14);

			//Bottom right coins
			createCoin (38, 17);
			createCoin (33, 17);
			createCoin (28, 19);
			createCoin (25, 20);
			createCoin (18, 26);
			createCoin (22, 26);
			createCoin (26, 26);
			createCoin (30, 26);

			add (coins);

			//Create player (a red box)
			player = new FlxSprite (FlxG.width / 2 - 5);
			player.makeGraphic (10, 12, FlxColor.RED);
			player.MaxVelocity.X = 80;
			player.MaxVelocity.Y = 200;
			player.Acceleration.Y = 200;
			player.Drag.X = player.MaxVelocity.X * 4;
			add (player);

			score = new FlxText (2, 2, 80, "SCORE: ");
			//score.setShadow(FlxColor.BLACK);
			score.text = "SCORE: " + (coins.CountDead () * 100);
			add (score);

			status = new FlxText (FlxG.width - 160 - 2, 2, 160);
			//status.setShadow(0xff000000);
			//status.setAlignment("right");
			switch (FlxG.score) {
			case 0:
				status.text = "Collect coins.";
				break;
			case 1:
				status.text = "Aww, you died!";
				break;
			}
			add (status);
		}

		//creates a new coin located on the specified tile
		public void createCoin (int X, int Y)
		{
			FlxSprite coin = new FlxSprite (X * 8 + 3, Y * 8 + 2);
			coin.makeGraphic (2, 4, FlxColor.YELLOW);
			coins.add (coin);
		}

		public override void update ()
		{
			//Player movement and controls
			player.Acceleration.X = 0;

			if (FlxG.keys.pressed (Keys.Left))
				player.Acceleration.X = -player.MaxVelocity.X * 4;
			if (FlxG.keys.pressed (Keys.Right))
				player.Acceleration.X = player.MaxVelocity.X * 4;

			if (FlxG.keys.pressed (Keys.Space) && player.isTouching (FlxObject.Floor))
				player.Velocity.Y = -player.MaxVelocity.Y / 2;

			if (FlxG.keys.justPressed (Keys.Escape))
				onQuit ();


			//Updates all the objects appropriately
			base.update ();

			//Check if player collected a coin or coins this frame
			FlxG.overlap (coins, player, getCoin);

			//Check to see if the player touched the exit door this frame
			FlxG.overlap (exit, player, win);

			FlxG.collide (level, player);

			//Check for player lose conditions
			if (player.Y > FlxG.height) {
				FlxG.score = 1; //sets status.text to "Aww, you died!"
				FlxG.resetState ();
			}

		}

		//Called whenever the player touches a coin
		public Boolean getCoin (FlxObject Coin, FlxObject Player)
		{
			Coin.kill ();
			score.text = "SCORE: " + (coins.CountDead () * 100);
			if (coins.CountLiving () == 0) {
				status.text = "Find the exit.";
				exit.Exists = true;
			}

			return true;
		}

		//Called whenever the player touches the exit
		public Boolean win (FlxObject Exit, FlxObject Player)
		{
			status.text = "Yay, you won!";
			score.text = "SCORE: 5000";
			Player.kill ();
			return true;
		}

		//This just quits - state.destroy() is automatically called upon state changing
		private void onQuit() {
			FlxG.switchState(new MenuState());
		}

	}
}

