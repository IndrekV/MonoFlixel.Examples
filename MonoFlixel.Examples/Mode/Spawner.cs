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
	public class Spawner : FlxSprite
	{
		private String ImgSpawner = "Mode/spawner.PNG";
		private String SndExplode = "Mode/asplode.mp3";
		private String SndExplode2 = "Mode/menu_hit_2.mp3";
		private String SndHit = "Mode/hit.mp3";
		
		private float _timer;
		private FlxGroup _bots;
		private FlxGroup _botBullets;
		private FlxEmitter _botGibs;
		private FlxEmitter _gibs;
		private Player _player;
		private bool _open;
		private FlxSound _sfxExplode;
		private FlxSound _sfxExplode2;
		private FlxSound _sfxHit;

		public Spawner(int x,int y,FlxEmitter Gibs,FlxGroup Bots,FlxGroup BotBullets, FlxEmitter BotGibs,Player ThePlayer) : base(x, y)
		{
			loadGraphic(ImgSpawner,true);
			_gibs = Gibs;
			_bots = Bots;
			_botBullets = BotBullets;
			_botGibs = BotGibs;
			_player = ThePlayer;
			_timer = FlxG.random()*20;
			_open = false;
			Health = 8;

			addAnimation("open", new int[]{1, 2, 3, 4, 5}, 40, false);
			addAnimation("close", new int[]{4, 3, 2, 1, 0}, 40, false);
			addAnimation("dead", new int[]{6});

			_sfxExplode = new FlxSound().loadEmbedded(SndExplode, false, false);
			_sfxExplode2 = new FlxSound().loadEmbedded(SndExplode2, false, false);
			_sfxHit = new FlxSound().loadEmbedded(SndHit, false, false);
		}

		 
		public override void destroy()
		{
			base.destroy();
			_bots = null;
			_botGibs = null;
			_botBullets = null;
			_gibs = null;
			_player = null;
		}

		
		public override void update()
		{
			_timer += FlxG.elapsed;
			int limit = 20;
			if(onScreen())
				limit = 4;
			if(_timer > limit)
			{
				_timer = 0;
				makeBot();
			}
			else if(_timer > limit - 0.35)
			{
				if(!_open)
				{
					_open = true;
					play("open");
				}
			}
			else if(_timer > 1)
			{
				if(_open)
				{
					play("close");
					_open = false;
				}
			}

			base.update();
		}

		
		public void hurt(float Damage)
		{
			_sfxHit.play(true);
			flicker(0.2f);
			FlxG.score += 50;
			base.hurt(Damage);
		}

		
		public override void kill()
		{
			if(!Alive)
				return;
			_sfxExplode.play(true);
			_sfxExplode2.play(true);
			base.kill();
			Active = false;
			Exists = true;
			Solid = false;
			flicker(0);
			play("dead");
			FlxG.camera.shake(0.007f,0.25f);
			/*
			FlxG.camera.flash(0xffd8eba2,0.65f,new IFlxCamera(){ public void callback(){turnOffSlowMo();}});
			FlxG.timeScale = 0.35f;
			makeBot();
			_gibs.at(this);
			_gibs.start(true,3);
			FlxG.score += 1000;
			*/
		}

		protected void makeBot()
		{
			//((Enemy) _bots.recycle(Enemy)).init((int) (X + Width/2), (int) (Y + Height/2), _botBullets, _botGibs, _player);
		}

		protected void turnOffSlowMo()
		{
			FlxG.timeScale = 1.0f;
		}
	}
}
