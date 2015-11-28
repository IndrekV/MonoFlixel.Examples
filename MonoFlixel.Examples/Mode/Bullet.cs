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
	public class Bullet : FlxSprite
	{
		private String ImgBullet = "Mode/bullet.png";
		private String SndHit = "Mode/jump.mp3";
		private String SndShoot = "Mode/shoot.mp3";
		
		private FlxSound _sfxHit;
		private FlxSound _sfxShoot;
		
		public float speed;

		public Bullet() : base ()
		{
			loadGraphic(ImgBullet,true);
			Width = 6;
			Height = 6;
			Offset.X = 1;
			Offset.Y = 1;

			addAnimation("up",new int[]{0});
			addAnimation("down",new int[]{1});
			addAnimation("left",new int[]{2});
			addAnimation("right",new int[]{3});
			addAnimation("poof",new int[]{4, 5, 6, 7}, 50, false);

			speed = 360;

			_sfxHit = new FlxSound().loadEmbedded(SndHit, false, false);
			_sfxShoot = new FlxSound().loadEmbedded(SndShoot, false, false);
		}

		
		public override void update()
		{
			if(!Alive)
			{
				if(Finished)
					Exists = false;
			}
			else if(Touching > None)
				kill();
		}

		
		public override void kill()
		{
			if(!Alive)
				return;
			Velocity.X = 0;
			Velocity.Y = 0;

			if(onScreen())
				_sfxHit.play(true);

			Alive = false;
			Solid = false;
			play("poof");
		}

		public void shoot(FlxPoint Location, int Aim)
		{
			_sfxShoot.play(true);
			base.reset(Location.X-Width/2,Location.Y-Height/2);
			Solid = true;
			switch((uint)Aim)
			{
				case Up:
					play("up");
					Velocity.Y = -speed;
					break;
				case Down:
					play("down");
					Velocity.Y = speed;
					break;
				case Left:
					play("left");
					Velocity.X = -speed;
					break;
				case Right:
					play("right");
					Velocity.X = speed;
					break;
				default:
					break;
			}
		}
	}
}
