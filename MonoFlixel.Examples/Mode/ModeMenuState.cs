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

	public class ModeMenuState : FlxState
	{
		//Some graphics and sounds
		protected String ImgEnemy = "Mode/bot.png";
		public String ImgGibs = "Mode/spawner_gibs.png";
		public String ImgCursor = "Mode/cursor.png";
		public String ImgFont40 = "Mode/nokiafc.fnt";
		public String SndHit = "Mode/menu_hit.mp3";
		public String SndHit2 = "Mode/menu_hit_2.mp3";

		//Replay data for the "Attract Mode" gameplay demos
		public String Attract1 = "Mode/attract1.fgr";
		public String Attract2 = "Mode/attract2.fgr";
		
		public FlxEmitter gibs;
		/*public FlxButton playButton;*/
		public FlxText title1;
		public FlxText title2;
		public bool fading;
		public float timer;
		static bool attractMode;
		/*private FlxVirtualPad _pad;*/

		
		public override void create()
		{
			FlxG.width = (int)FlxG.camera.Width;// ViewportWidth;
			FlxG.resetCameras();
			
			FlxG.bgColor = Color.Black;

			//Simple use of flixel save game object.
			//Tracks number of times the game has been played.
			/*FlxSave save = new FlxSave();
			if(save.bind("Mode"))
			{
				if(save.data.get("plays", Integer.class) == null)
					save.data.put("plays", 0);
				else
					save.data.put("plays", save.data.get("plays", Integer.class) + 1);
				FlxG.log("Number of plays: "+save.data.get("plays", Integer.class));
				//save.erase();
				save.close();
			}
			*/

			//All the bits that blow up when the text smooshes together
			gibs = new FlxEmitter(FlxG.width/2-50,FlxG.height/2-10);
			gibs.setSize(100,30);
			gibs.setYSpeed(-200,-20);
			gibs.setRotation(-720,720);
			gibs.gravity = 100;
			gibs.makeParticles(ImgGibs,650,32,true,0);
			add(gibs);

			//the letters "mo"
			title1 = new FlxText(FlxG.width + 16,FlxG.height/3,64,"mo");
			/*
			if(Gdx.app.getType() == ApplicationType.WebGL)
				title1.setFormat(ImgFont40, 40);
			else 
				title1.SetSize(32);
			title1.SetColor(0x3a5c39);
			title1.Antialiasing = true;
			*/
			title1.Velocity.X = -FlxG.width;
			title1.Moves = true;
			add(title1);

			//the letters "de"
			title2 = new FlxText(-60,title1.Y,(int) title1.Width,"de");
			/*
			if(Gdx.app.getType() == ApplicationType.WebGL)
				title2.setFormat(ImgFont40, 40);
			else 
				title2.SetSize(32);

			title2.SetColor(title1.GetColor());
			title2.Antialiasing = title1.Antialiasing;
			*/
			title2.Velocity.X = FlxG.width;
			title2.Moves = true;
			add(title2);

			fading = false;
			timer = 0;
			attractMode = false;
				
			//FlxG.mouse.show(FlxS.ContentManager.Load<Texture2D>(ImgCursor),2);
			/*
			_pad = new FlxVirtualPad(FlxVirtualPad.DPAD_None, FlxVirtualPad.A_B);
			_pad.setAlpha(0.5f);

			if(Gdx.app.getType() != ApplicationType.Desktop)
				add(_pad);
				*/
		}

		
		public override void destroy()
		{
			base.destroy();
			gibs = null;
			/*playButton = null;*/
			title1 = null;
			title2 = null;
		}

		
		public override void update()
		{			
			base.update();

			if(title2.X > title1.X + title1.Width - 4)
			{
				//Once mo and de cross each other, fix their positions
				title2.X = title1.X + title1.Width - 4;
				title1.Velocity.X = 0;
				title2.Velocity.X = 0;

				//Then, play a cool sound, change their color, and blow up pieces everywhere
				/*
				FlxG.play(SndHit, 1f, false, false);
				*/
				FlxG.flash(Color.White,0.5f);
				FlxG.shake(0.035f,0.5f);
				title1.Color = Color.White;
				title2.Color = Color.White;
				gibs.start(true,5);
				title1.Angle = FlxG.random()*30-15;
				title2.Angle = FlxG.random()*30-15;

				//Then we're going to add the text and buttons and things that appear
				//If we were hip we'd use our own button animations, but we'll just recolor
				//the stock ones for now instead.
				FlxText text;
				text = new FlxText(FlxG.width/2-50,FlxG.height/3+39,100,"by Adam Atomic");
				/*text.Alignment = "center";*/
				text.Color = Color.White;
				add(text);
				/*
				FlxButton flixelButton = new FlxButton(FlxG.width/2-40,FlxG.height/3+54,"flixel.org",new IFlxButton(){ public void callback(){onFlixel();}});
				flixelButton.setColor(0xff729954);
				flixelButton.label.setColor(0xffd8eba2);
				ModeMenuState(flixelButton);

				FlxButton dannyButton = new FlxButton(flixelButton.X,flixelButton.Y + 22,"music: dannyB",new IFlxButton(){ public void callback(){onDanny();}});
				dannyButton.setColor(flixelButton.getColor());
				dannyButton.label.setColor(flixelButton.label.getColor());
				add(dannyButton);
				*/

				text = new FlxText(FlxG.width/2-40,FlxG.height/3+139,80,"X+C TO PLAY");
				text.Color = Color.White;
				//text.setAlignment("center");
				add(text);
				/*
				playButton = new FlxButton(flixelButton.X,flixelButton.Y + 82,"CLICK HERE", onPlay());
				playButton.setColor(flixelButton.getColor());
				playButton.label.setColor(flixelButton.label.getColor());
				add(playButton);
				*/
			}

			//X + C were pressed, fade out and change to play state.
			//OR, if we sat on the menu too long, launch the attract mode instead!
			timer += FlxG.elapsed;
			if(timer >= 10) //go into demo mode if no buttons are pressed for 10 seconds
				attractMode = true;
			if(!fading 
				&& ((FlxG.keys.pressed(Keys.X) && FlxG.keys.pressed(Keys.C)) 
				/*|| (_pad.buttonA.status == FlxButton.Pressed && _pad.buttonB.status == FlxButton.Pressed) */
				|| attractMode)) 
			{
				fading = true;
				/*FlxG.play(SndHit2, 1f, false, false);*/
				FlxG.flash(Color.White,0.5f);
				FlxG.fade(Color.Black,1, onFade);
			}
		}

		//These are all "event handlers", or "callbacks".
		//These first three are just called when the
		//corresponding buttons are pressed with the mouse.
		protected void onFlixel()
		{
			FlxU.openURL("http://flixel.org");
		}

		protected void onDanny()
		{
			FlxU.openURL("http://dbsoundworks.com");
		}

		protected void onPlay()
		{
			//playButton.Exists = false;
			//FlxG.play(SndHit2, 1f, false, false);
		}

		//This function is passed to FlxG.fade() when we are ready to go to the next game state.
		//When FlxG.fade() finishes, it will call this, which in turn will either load
		//up a game demo/replay, or let the player start playing, depending on user input.
		protected void onFade()
		{
			/*if(attractMode)
				FlxG.loadReplay((FlxG.random()<0.5f)?FlxG.loadString(Attract1):FlxG.loadString(Attract2),new ModePlayState(),new String[]{"ANY"},22, onDemoComplete);
			else*/
				FlxG.switchState(new ModePlayState());
		}

		//This function is called by FlxG.loadReplay() when the replay finishes.
		//Here, we initiate another fade effect.
		protected void onDemoComplete()
		{
			//FlxG.fade(0xff131c1b,1, onDemoFaded);
		}

		//Finally, we have another function called by FlxG.fade(), this time
		//in relation to the callback above.  It stops the replay, and resets the game
		//once the gameplay demo has faded out.
		protected void onDemoFaded()
		{
			//FlxG.stopReplay();
			FlxG.resetGame();
		}
	}
}
