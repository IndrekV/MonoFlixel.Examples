
/**
 * 
 * @author Indrek Vändrik
 * Based on the flixel-gdx example by Ka Wing Chin
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
	public class Bunny : FlxSprite
	{
		private const String ImgBunny = "Bunnymark/bunny.png";

		private Boolean _complex;

		public Bunny(float x, float y) :base(x, y)
		{
			loadGraphic(ImgBunny, false, false, 26, 37);
			float speedMultiplier = 50f;
			Velocity.X = speedMultiplier * (FlxG.random() * 5f) * (FlxG.random() < 0.5f ? 1f : -1f);
			Velocity.Y = speedMultiplier * ((FlxG.random() * 5f) - 2.5f) * (FlxG.random() < 0.5f ? 1f : -1f);
			Acceleration.Y = 5;
			Angle = 15 - FlxG.random() * 30;
			AngularVelocity = 30f * (FlxG.random() * 5f) * (FlxG.random() < 0.5f ? 1f : -1f);
			Elasticity = 1;
		}

		
		public override void update()
		{
			if(_complex)
			{
				Alpha = (.3f + .7f * Y / FlxG.height);
			}

			if((X + Width) >= FlxG.width)
			{
				Velocity.X *= -1;
				X = FlxG.width - Width;
			}
			else if(X <= 0)
			{
				Velocity.X *= -1;
				X = 0;
			}

			if((Y + Height) >= FlxG.height)
			{
				Velocity.Y *= -0.8f;
				Y = FlxG.height - Height;

				if(FlxG.random() > 0.5f)
				{
					Velocity.Y -= 3 + FlxG.random() * 4;
				}
			}
			else if(Y <= 0)
			{
				Velocity.Y *= -0.8f;
				Y = 0;
			}
		}

		public void setComplex(Boolean complex)
		{
			_complex = complex;
			if(_complex)
				Scale.X = Scale.Y = .3f + FlxG.random();
			else
			{
				Scale.X = Scale.Y = 1f;
				Alpha = 1f;
			}
		}
	}
}
