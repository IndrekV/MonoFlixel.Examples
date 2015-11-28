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

	public class Player : FlxSprite
	{
		protected String ImgSpaceman = "Mode/spaceman.png";

		protected String SndJump = "Mode/jump.mp3";
		protected String SndLand = "Mode/land.mp3";
		protected String SndExplode = "Mode/asplode.mp3";
		protected String SndExplode2 = "Mode/menu_hit_2.mp3";
		protected String SndHurt = "Mode/hurt.mp3";
		protected String SndJam = "Mode/jam.mp3";
		
		protected int _jumpPower;
		protected FlxGroup _bullets;
		protected uint _aim;
		protected float _restart;
		protected FlxEmitter _gibs;

		private FlxSound _sfxJump;
		private FlxSound _sfxLand;
		private FlxSound _sfxExplode;
		private FlxSound _sfxExplode2;
		private FlxSound _sfxHurt;
		private FlxSound _sfxJam;
		
		//private FlxVirtualPad _pad;
		private bool _justShoot;

		//This is the player object class.  Most of the comments I would put in here
		//would be near duplicates of the Enemy class, so if you're confused at all
		//I'd recommend checking that out for some ideas!
		public Player(int x,int y,FlxGroup Bullets,FlxEmitter Gibs/*, FlxVirtualPad pad*/): base (x, y)
		{
			loadGraphic(ImgSpaceman,true,true,8);
			_restart = 0;

			//bounding box tweaks
			Width = 6;
			Height = 7;
			Offset.X = 1;
			Offset.Y = 1;
			/*
			_pad = pad;
			_pad.buttonA.onDown = new IFlxButton(){ public void callback(){jump();}};
*/
			//basic player physics
			int runSpeed = 80;
			Drag.X = runSpeed*8;
			Acceleration.Y = 420;
			_jumpPower = 200;
			MaxVelocity.X = runSpeed;
			MaxVelocity.Y = _jumpPower;

			//animations
			addAnimation("idle", new int[]{0});
			addAnimation("run", new int[]{1, 2, 3, 0}, 12);
			addAnimation("jump", new int[]{4});
			addAnimation("idle_up", new int[]{5});
			addAnimation("run_up", new int[]{6, 7, 8, 5}, 12);
			addAnimation("jump_up", new int[]{9});
			addAnimation("jump_down", new int[]{10});

			//bullet stuff
			_bullets = Bullets;
			_gibs = Gibs;

			_sfxExplode = new FlxSound().loadEmbedded(SndExplode, false, false);
			_sfxExplode2 = new FlxSound().loadEmbedded(SndExplode2, false, false);
			_sfxHurt = new FlxSound().loadEmbedded(SndHurt, false, false);
			_sfxJam = new FlxSound().loadEmbedded(SndJam, false, false);
			_sfxJump = new FlxSound().loadEmbedded(SndJump, false, false);
			_sfxLand = new FlxSound().loadEmbedded(SndLand, false, false);

		}

		
		public override void destroy()
		{
			base.destroy();
			_bullets = null;
			_gibs = null;
		}

		
		public override void update()
		{
			//game restart timer
			if(!Alive)
			{
				_restart += FlxG.elapsed;
				if(_restart > 2)
					FlxG.resetState();
				return;
			}

			//make a little noise if you just touched the floor
			if(justTouched(Floor) && (Velocity.Y > 50))
				_sfxLand.play(true);

			//MOVEMENT
			Acceleration.X = 0;
			if(FlxG.keys.pressed(Keys.Left) /*|| _pad.buttonLeft.status == FlxButton.Pressed*/)
			{
				Facing = Left;
				Acceleration.X -= Drag.X;
			}
			else if(FlxG.keys.pressed(Keys.Right) /*|| _pad.buttonRight.status == FlxButton.Pressed*/)
			{
				Facing = Right;
				Acceleration.X += Drag.X;
			}
			if(FlxG.keys.justPressed(Keys.X))
			{			
				jump();
			}

			//AIMING
			if(FlxG.keys.pressed(Keys.Up)  /*|| _pad.buttonUp.status == FlxButton.Pressed*/)
				_aim = Up;
			else if((FlxG.keys.pressed(Keys.Down) /*|| _pad.buttonDown.status == FlxButton.Pressed*/) && Velocity.Y != 0)
				_aim = Down;
			else
				_aim = Facing;

			//ANIMATION
			if(Velocity.Y != 0)
			{
				if(_aim == Up) play("jump_up");
				else if(_aim == Down) play("jump_down");
				else play("jump");
			}
			else if(Velocity.X == 0)
			{
				if(_aim == Up) play("idle_up");
				else play("idle");
			}
			else
			{
				if(_aim == Up) play("run_up");
				else play("run");
			}

			//SHOOTING
			if(FlxG.keys.pressed(Keys.C)  /*|| _pad.buttonB.status == FlxButton.Pressed*/)
			{
				if(!_justShoot)
				{
					if(flickering)
						_sfxJam.play(true);
					else
					{
						getMidpoint(_tagPoint);
						((Bullet) _bullets.recycle(typeof(Bullet))).shoot(_tagPoint,(int)_aim);
						if(_aim == Down)
							Velocity.Y -= 36;
					}
					_justShoot = true;
				}			
			}
			else
				_justShoot = false;
		}

		
		public void hurt(float Damage)
		{
			Damage = 0;
			if(flickering)
				return;
			_sfxHurt.play(true);
			flicker(1.3f);
			if(FlxG.score > 1000) FlxG.score -= 1000;
			if(Velocity.X > 0)
				Velocity.X = -MaxVelocity.X;
			else
				Velocity.X = MaxVelocity.X;
			base.hurt(Damage);
		}
		
		private void jump()
		{
			if((int) Velocity.Y == 0)
			{
				Velocity.Y = -_jumpPower;
				_sfxJump.play(true);			
			}
		}

		 
		public override void kill()
		{
			if(!Alive)
				return;
			Solid = false;
			_sfxExplode.play(true);
			_sfxExplode2.play(true);
			base.kill();
			flicker(0);
			Exists = true;
			Visible = false;
			Velocity.make();
			Acceleration.make();
			FlxG.camera.shake(0.005f,0.35f);
			FlxG.camera.Flash(Color.White,0.35f);
			if(_gibs != null)
			{
				_gibs.at(this);
				_gibs.start(true,5,0,50);
			}
		}
	}
}
