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

	public class EnemyBullet : FlxSprite
	{
		private String ImgBullet = "Mode/bot_bullet.png";
		private String SndHit = "Mode/jump.mp3";
		private String SndShoot = "Mode/enemy.mp3";
		
		private FlxSound _sfxHit;
		private FlxSound _sfxShoot;
		
		public float speed;

		public EnemyBullet() : base()
		{
			loadGraphic(ImgBullet,true);
			addAnimation("idle",new int[]{0, 1}, 50);
			addAnimation("poof",new int[]{2, 3, 4}, 50, false);
			speed = 120;

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
			Solid = (false);
			play("poof");
		}

		public void shoot(FlxPoint Location, float Angle)
		{
			_sfxShoot.play(true);
			base.reset(Location.X-Width/2,Location.Y-Height/2);
			FlxU.rotatePoint(0,(int) speed,0,0,Angle,_tagPoint);
			Velocity.X = _tagPoint.X;
			Velocity.Y = _tagPoint.Y;
			Solid = (true);
			play("idle");
		}
	}
}
