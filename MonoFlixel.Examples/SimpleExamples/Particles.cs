/**
 * 
 * @author Indrek Vändrik
 * Based on the flixel example by Zachary Tarvit
 */

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;
using MonoFlixel;

namespace MonoFlixel.Examples
{
	public class Particles :FlxState
	{
		public Particles ()
		{
		}

		//Our emmiter
		private FlxEmitter theEmitter;

		//Our white pixel (This is to prevent creating 200 new pixels all to a new variable each loop)
		private FlxParticle whitePixel;

		//Some buttons
		private FlxButton collisionButton;
		private FlxButton gravityButton;
		private FlxButton quitButton;

		//some walls stuff
		private FlxGroup collisionGroup;
		private FlxSprite wall;
		private FlxSprite floor;

		//We'll use these to track the current state of gravity and collision
		private Boolean isGravityOn   = false;
		private Boolean isCollisionOn = false;

		//Just a useful flxText for notifications
		private FlxText topText;

		override public void create()
		{
			FlxG.Framerate = 60;
			//FlxG.flashFramerate = 60;

			//Here we actually initialize out emitter
			//The parameters are        X   Y                Size (Maximum number of particles the emitter can store)
			theEmitter = new FlxEmitter(10, FlxG.height / 2, 200);

			//Now by default the emitter is going to have some properties set on it and can be used immediately
			//but we're going to change a few things.

			//First this emitter is on the side of the screen, and we want to show off the movement of the particles
			//so lets make them launch to the right.
			theEmitter.setXSpeed(100, 200);

			//and lets funnel it a tad
			theEmitter.setYSpeed( -50, 50);

			//Let's also make our pixels rebound off surfaces
			theEmitter.bounce = 0.8f;

			//Now let's add the emitter to the state.
			add(theEmitter);

			//Now it's almost ready to use, but first we need to give it some pixels to spit out!
			//Lets fill the emitter with some white pixels
			for (int i = 0; i < theEmitter.maxSize/2; i++) {
				whitePixel = new FlxParticle();
				whitePixel.makeGraphic(2, 2, new Color(0xFF,0xFF,0xFF));
				whitePixel.Visible = false; //Make sure the particle doesn't show up at (0, 0)
				theEmitter.add(whitePixel);
				whitePixel = new FlxParticle();
				whitePixel.makeGraphic(1, 1, new Color(0xFF,0xFF,0xFF));
				whitePixel.Visible = false;
				theEmitter.add(whitePixel);
			}

			//Now let's setup some buttons for messing with the emitter.
			collisionButton = new FlxButton(0, FlxG.height - 22, "Collision", onCollision);
			add(collisionButton);
			gravityButton = new FlxButton(80, FlxG.height - 22, "Gravity", onGravity);
			add(gravityButton);
			quitButton = new FlxButton(FlxG.width-80, FlxG.height - 22, "Quit", onQuit);
			add(quitButton);

			//I'll just leave this here
			topText = new FlxText(0, 2, FlxG.width, "Welcome");
			topText.setAlignment("center");
			add(topText);

			//Lets setup some walls for our pixels to collide against
			collisionGroup = new FlxGroup();
			wall= new FlxSprite(100, (FlxG.height/2)-50);
			wall.makeGraphic(10, 100, new Color(0xFF,0xFF,0xFF,0x50));//Make it darker - easier on the eyes :)
			wall.Visible = wall.Solid = false;//Set both the visibility AND the solidity to false, in one go
			wall.Immovable = true;//Lets make sure the pixels don't push out wall away! (though it does look funny)
			collisionGroup.add(wall);
			//Duplicate our wall but this time it's a floor to catch gravity affected particles
			floor = new FlxSprite(10, 267);
			floor.makeGraphic((uint)FlxG.width - 20, 10, new Color(0xFF,0xFF,0xFF,0x50));
			floor.Visible = floor.Solid = false;
			floor.Immovable = true;
			collisionGroup.add(floor);

			//Please note that this demo makes the walls themselves not collide, for the sake of simplicity.
			//Normally you would make the particles have solid = true or false to make them collide or not on creation,
			//because in a normal environment your particles probably aren't going to change solidity at a mouse 
			//click. If they did, you would probably be better suited with emitter.setAll("solid", true)
			//I just don't feel that setAll is applicable here(Since I would still have to toggle the walls anyways)

			//Don't forget to add the group to the state(Like I did :P)
			add(collisionGroup);

			//Now lets set our emitter free.
			//Params:        Explode, Particle Lifespan, Emit rate(in seconds)
			theEmitter.start(false, 3, .01f);

			//Let's re show the cursors
			FlxG.mouse.show();
			//Mouse.hide();
		}

		override public void update()
		{
			//This is just to make the text at the top fade out
			if (topText.Alpha > 0) {
				topText.Alpha -= .01f;
			}
			base.update();
			FlxG.collide(theEmitter, collisionGroup);

			if (FlxG.keys.justPressed (Keys.Escape))
				onQuit ();
		}

		//This is run when you flip the collision
		private void onCollision() {
			isCollisionOn = !isCollisionOn;
			if (isCollisionOn) {
				if (isGravityOn) {
					floor.Solid = true;    //Set the floor to the 'active' collision barrier
					floor.Visible = true;
					wall.Solid = false;
					wall.Visible = false;
				}else {
					floor.Solid = false;   //Set the wall to the 'active' collision barrier
					floor.Visible = false;
					wall.Solid = true;
					wall.Visible = true;
				}
				topText.text = "Collision: ON";
			}else {
				//Turn off the wall and floor, completely
				wall.Solid = floor.Solid = wall.Visible = floor.Visible = false;
				topText.text = "Collision: OFF";
			}
			topText.Alpha = 1;
			FlxG.log("Toggle Collision");
		}

		//This is run when you flip the gravity
		private void onGravity() {
			isGravityOn = !isGravityOn;
			if (isGravityOn) {
				theEmitter.gravity = 200;
				if (isCollisionOn){
					floor.Visible = true;
					floor.Solid = true;
					wall.Visible = false;
					wall.Solid = false;
				}
				//Just for the sake of completeness let's go ahead and make this change happen 
				//to all of the currently emitted particles as well.
				for (int i = 0; i < theEmitter.Members.Count; i++) {
					((FlxParticle)theEmitter.Members[i]).Acceleration.Y = 200; //Cast the pixel from the emitter as a particle so we can use it
				}
				topText.text = "Gravity: ON";
			}else {
				theEmitter.gravity = 0;
				if (isCollisionOn){
					wall.Visible = true;
					wall.Solid = true;
					floor.Visible = false;
					floor.Solid = false;
				}
				for (int i = 0; i < theEmitter.Members.Count; i++) {
					((FlxParticle)theEmitter.Members[i]).Acceleration.Y = 0;
				}
				topText.text = "Gravity: OFF";
			}
			topText.Alpha = 1;
			FlxG.log("Toggle Gravity");
		}
		//This just quits - state.destroy() is automatically called upon state changing
		private void onQuit() {
			FlxG.switchState(new MenuState());
		}
	}
}

