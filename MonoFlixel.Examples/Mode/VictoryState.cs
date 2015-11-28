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

	public class VictoryState : FlxState
	{
		private String ImgGibs = "Mode/spawner_gibs.png";
		private String SndMenu = "Mode/menu_hit_2.mp3";
		private String ImgFont20 = "Mode/nokiafc.fnt";
		
		private float _timer;
		private bool _fading;

		
		public override void create()
		{
			_timer = 0;
			_fading = false;
			FlxG.flash(Color.White);

			//Gibs emitted upon death
			FlxEmitter gibs = new FlxEmitter(0,-50);
			gibs.setSize((uint)FlxG.width,0);
			gibs.setXSpeed();
			gibs.setYSpeed(0,100);
			gibs.setRotation(-360,360);
			gibs.gravity = 80;
			gibs.makeParticles(ImgGibs,800,32,true,0);
			add(gibs);
			gibs.start(false,0,0.005f);

			FlxText text = new FlxText(0,FlxG.height/2-35,FlxG.width,"VICTORY\n\nSCORE: "+FlxG.score);
			/*
			if(Gdx.app.getType() == ApplicationType.WebGL)
				text.setFormat(ImgFont20,20,0xd8eba2,"center");
			else
				text.setFormat(null,16,0xd8eba2,"center");*/
			add(text);
		}

		
		public override void update()
		{
			base.update();
			if(!_fading)
			{
				_timer += FlxG.elapsed;
				if((_timer > 0.35) && ((_timer > 10) || FlxG.keys.justPressed(Keys.X) || FlxG.keys.justPressed(Keys.C)))
				{
					_fading = true;
					/*FlxG.play(SndMenu, 1f, false, false);*/
					FlxG.fade(Color.Black,2, onPlay);
				}
			}
		}

		public void onPlay() 
		{
			FlxG.switchState(new ModePlayState());
		}
	}
}
