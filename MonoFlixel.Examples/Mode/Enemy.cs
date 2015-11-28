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
	public class Enemy : FlxSprite
	{
		protected String ImgBot = "Mode/bot.png";
		protected String ImgJet = "Mode/jet.png";
		protected String SndExplode = "Mode/asplode.mp3";
		protected String SndHit = "Mode/hit.mp3";
		protected String SndJet = "Mode/jet.mp3";
		
		//References to other game objects:
		protected Player _player;		//The player object
		protected FlxGroup _bullets;	//A group of enemy bullet objects (Enemies shoot these out)
		protected FlxEmitter _gibs;		//A group of bits and pieces that explode when the Enemy dies.

		//We use this number to figure out how fast the ship is flying
		protected float _thrust;

		//A special effect - little poofs shoot out the back of the ship
		protected FlxEmitter _jets;

		//These are "timers" - numbers that count down until we want something interesting to happen.
		protected float _timer;		//Helps us decide when to fly and when to stop flying.
		protected float _shotClock;	//Helps us decide when to shoot.

		//This object isn't strictly necessary, and is only used with getMidpoint().
		//By passing this object, we can avoid a potentially costly allocation of
		//a new FlxPoint() object by the getMidpoint() function.
		protected FlxPoint _playerMidpoint;
		
		private FlxSound _sfxExplode;
		private FlxSound _sfxHit;
		private FlxSound _sfxJet;

		//This is the constructor for the enemy class.  Because we are
		//recycling enemies, we don't want our constructor to have any
		//required parameters.
		public Enemy() : base()
		{
			loadRotatedGraphic(ImgBot,64,0,false,true);

			//We want the enemy's "hit box" or actual size to be
			//smaller than the enemy graphic itself, just by a few pixels.
			Width = 12;
			Height = 12;
			centerOffsets();

			//Here we are setting up the jet particles
			// that shoot out the back of the ship.
			_jets = new FlxEmitter();
			_jets.setRotation();
			//_jets.MakeParticles(ImgJet,15,0,false,0);

			//These parameters help control the ship's
			//speed and direction during the update() loop.
			MaxAngular = 120;
			AngularDrag = 400;
			Drag.X = 35;
			_thrust = 0;
			_playerMidpoint = new FlxPoint();

			_sfxExplode = new FlxSound().loadEmbedded(SndExplode, false, false);
			_sfxHit = new FlxSound().loadEmbedded(SndHit, false, false);
			_sfxJet = new FlxSound().loadEmbedded(SndJet, false, false);
		}

		//Each time an Enemy is recycled (in this game, by the Spawner object)
		//we call init() on it afterward.  That allows us to set critical parameters
		//like references to the player object and the ship's new position.
		public void init(int xPos,int yPos,FlxGroup Bullets,FlxEmitter Gibs,Player ThePlayer)
		{
			_player = ThePlayer;
			_bullets = Bullets;
			_gibs = Gibs;

			reset(xPos - Width/2,yPos - Height/2);
			Angle = angleTowardPlayer();
			Health = 2;	//Enemies take 2 shots to kill
			_timer = 0;
			_shotClock = 0;
		}

		//Called by flixel to help clean up memory.
		 
		public override void destroy()
		{
			base.destroy();

			_player = null;
			_bullets = null;
			_gibs = null;

			_jets.destroy();
			_jets = null;

			_playerMidpoint = null;
		}

		//This is the main flixel update function or loop function.
		//Most of the enemy's logic or behavior is in this function here.
		 
		public override void update()
		{
			//Then, rotate toward that angle.
			//We could rotate instantly toward the player by simply calling:
			//angle = angleTowardPlayer();
			//However, we want some less predictable, more wobbly behavior.
			float da = angleTowardPlayer();
			if(da < Angle)
				AngularAcceleration = -AngularDrag;
			else if(da > Angle)
				AngularAcceleration = AngularDrag;
			else
				AngularAcceleration = 0;

			//Figure out if we want the jets on or not.
			_timer += FlxG.elapsed;
			if(_timer > 8)
				_timer = 0;
			bool jetsOn = _timer < 6;

			//Set the bot's movement speed and direction
			//based on angle and whether the jets are on.
			_thrust = FlxU.computeVelocity(_thrust,(jetsOn?90:0),Drag.X,60);
			FlxU.rotatePoint(0,(int) _thrust,0,0,Angle,Velocity);

			//Shooting - three shots every few seconds
			if(onScreen())
			{
				bool shoot = false;
				float os = _shotClock;
				_shotClock += FlxG.elapsed;
				if((os < 4.0) && (_shotClock >= 4.0))
				{
					_shotClock = 0;
					shoot = true;
				}
				else if((os < 3.5) && (_shotClock >= 3.5))
					shoot = true;
				else if((os < 3.0) && (_shotClock >= 3.0))
					shoot = true;

				//If we rolled over one of those time thresholds,
				//shoot a bullet out along the angle we're currently facing.
				if(shoot)
				{
					//First, recycle a bullet from the bullet pile.
					//If there are none, recycle will automatically create one for us.
					EnemyBullet b = (EnemyBullet) _bullets.recycle(typeof(EnemyBullet));
					//Then, shoot it from our midpoint out along our angle.
					b.shoot(getMidpoint(_tagPoint),Angle);
				}
			}

			//Then call FlxSprite's update() function, to automate
			// our motion and animation and stuff.
			base.update();

			//Finally, update the jet particles shooting out the back of the ship.
			if(jetsOn)
			{
				if(!_jets.on)
				{
					//If they're supposed to be on and they're not,
					//turn em on and play a little sound.
					_jets.start(false,0.5f,0.01f);
					if(onScreen())
						_sfxJet.play(true);
				}
				//Then, position the jets at the center of the Enemy,
				//and point the jets the opposite way from where we're moving.
				_jets.at(this);
				_jets.setXSpeed(-Velocity.X-30,-Velocity.X+30);
				_jets.setYSpeed(-Velocity.Y-30,-Velocity.Y+30);
			}
			else	//If jets are supposed to be off, just turn em off.
				_jets.on = false;
			//Finally, update the jet emitter and all its member sprites.
			_jets.update();
		}

		//Even though we updated the jets after we updated the Enemy,
		//we want to draw the jets below the Enemy, so we call _jets.draw() first.
		
		public override void draw()
		{
			_jets.draw();
			base.draw();
		}

		//This function is called when player bullets hit the Enemy.
		//The enemy is told to flicker, points are awarded to the player,
		//and damage is dealt to the Enemy.
		 public void hurt(float Damage)
		{
			_sfxHit.play(true);
			flicker(0.2f);
			FlxG.score += 10;
			base.hurt(Damage);
		}

		//Called to kill the enemy.  A cool sound is played,
		//the jets are turned off, bits are exploded, and points are rewarded.
		 
		public override void kill()
		{
			if(!Alive)
				return;
			_sfxExplode.play(true);
			base.kill();
			flicker(0);
			_jets.kill();
			_gibs.at(this);
			_gibs.start(true,3,0,20);
			FlxG.score += 200;
		}

		//A helper function that returns the angle between
		//the Enemy's midpoint and the player's midpoint.
		protected float angleTowardPlayer()
		{
			return FlxU.getAngle(getMidpoint(_tagPoint),_player.getMidpoint(_playerMidpoint));
			//return FlxU.getAngle(getMidpoint(_tagPoint),_player.getMidpoint(_playerMidpoint));
		}
	}
}
