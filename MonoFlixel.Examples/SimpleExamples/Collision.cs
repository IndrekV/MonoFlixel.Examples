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
	public class Collision :FlxState
	{
		public Collision ()
		{
		}

		//This is for our messages
		private FlxText topText;

		//This is our elevator, for smashing the crates
		private FlxSprite elevator;
		private string elevatorPNG = "Collision/elevator.png";

		//We'll reuse this when we make a bunch of crates
		private FlxSprite crate;
		private string cratePNG = "Collision/crate.png";

		//We'll make 100 per group crates to smash about
		private int numCrates = 100;

		//these are the groups that will hold all of our crates
		private FlxGroup crateStormGroup;
		private FlxGroup crateStormGroup2;
		private FlxGroup crateStormMegaGroup;

		//We'll make a sweet flixel logo to ride the elevator for option #2
		private FlxSprite flixelRider;
		private string flixelRiderPNG = "Collision/flixelLogo.png";

		//Here we have a few buttons for use in altering the demo
		private FlxButton crateStorm;
		private FlxButton crateStormG1;
		private FlxButton crateStormG2;
		private FlxButton quitButton;
		private FlxButton flxRiderButton;
		private FlxButton groupCollision;

		//Some toggle variables for use with the buttons
		private Boolean isCrateStormOn = true;
		private Boolean isFlxRiderOn = false;
		private Boolean collideGroups = false;
		private Boolean redGroup = true;
		private Boolean blueGroup = true;
		private Boolean rising = true;

		public override void create()
		{
			//Kick the framerate back up
			FlxG.Framerate = 60;
			//FlxG.flashFramerate = 60;

			//Let's setup our elevator, for some wonderful crate bashing goodness
			elevator = new FlxSprite((FlxG.width / 2) - 100, 250, elevatorPNG);
			//Make it able to collide, and make sure it's not tossed around
			elevator.Solid = elevator.Immovable = true;
			//And add it to the state
			add(elevator);

			//Now lets get some crates to smash around, normally I would use an emitter for this
			//kind of scene, but for this demo I wanted to use regular sprites 
			//(See ParticlesDemo for an example of an emitter with colliding particles)
			//We'll need a group to place everything in - this helps a lot with collisions
			crateStormGroup = new FlxGroup();
			for (int i = 0; i < numCrates; i++) {
				crate = new FlxSprite((FlxG.random() * 200) + 100, 20);
				crate.loadRotatedGraphic(cratePNG, 16, 0); //This loads in a graphic, and 'bakes' some rotations in so we don't waste resources computing real rotations later
				crate.AngularVelocity = FlxG.random() * 50-150; //Make it spin a tad
				crate.Acceleration.Y = 300; //Gravity
				crate.Acceleration.X = -50; //Some wind for good measure
				crate.MaxVelocity.Y = 500; //Don't fall at 235986mph
				crate.MaxVelocity.X = 200; //"      fly  "  "
				crate.Elasticity = FlxG.random(); //Let's make them all bounce a little bit differently
				crateStormGroup.add(crate);
			}
			add(crateStormGroup);
			//And another group, this time - Red crates
			crateStormGroup2 = new FlxGroup();
			for (int i = 0; i < numCrates; i++) {
				crate = new FlxSprite((FlxG.random() * 200) + 100, 20);
				crate.loadRotatedGraphic(cratePNG, 16, 1);
				crate.AngularVelocity = FlxG.random() * 50-150;
				crate.Acceleration.Y = 300;
				crate.Acceleration.X = 50;
				crate.MaxVelocity.Y = 500;
				crate.MaxVelocity.X = 200;
				crate.Elasticity = FlxG.random();
				crateStormGroup2.add(crate);
			}
			add(crateStormGroup2);

			//Now what we're going to do here is add both of those groups to a new containter group
			//This is useful if you had something like, coins, enemies, special tiles, etc.. that would all need
			//to check for overlaps with something like a player.
			crateStormMegaGroup = new FlxGroup ();
			crateStormMegaGroup.add(crateStormGroup);
			crateStormMegaGroup.add(crateStormGroup2);

			//Cute little flixel logo that will ride the elevator
			flixelRider = new FlxSprite((FlxG.width / 2) - 13, 0, flixelRiderPNG);
			flixelRider.Solid = flixelRider.Visible = flixelRider.Exists = false; //But we don't want him on screen just yet...
			flixelRider.Acceleration.Y = 800;
			add(flixelRider);

			//This is for the text at the top of the screen
			topText = new FlxText(0, 2, FlxG.width, "Welcome");
			topText.setAlignment("center");
			add(topText);

			//Lets make a bunch of buttons! YEAH!!!
			crateStorm = new FlxButton(2, FlxG.height - 22, "Crate Storm", onCrateStorm);
			add(crateStorm);
			flxRiderButton = new FlxButton(82, FlxG.height - 22, "Flixel Rider", onFlixelRider);
			add(flxRiderButton);
			crateStormG1 = new FlxButton(162, FlxG.height - 22, "Blue Group", onBlue);
			add(crateStormG1);
			crateStormG2 = new FlxButton(242, FlxG.height - 22, "Red Group", onRed);
			add(crateStormG2);
			groupCollision = new FlxButton(202, FlxG.height - 42, "Collide Groups", onCollideGroups);
			add(groupCollision);
			quitButton = new FlxButton(320, FlxG.height - 22, "Quit", onQuit);
			add(quitButton);

			//And lets get the flixel cursor visible again
			FlxG.mouse.show();
			//Mouse.hide();
		}

		override public void update()
		{
			//This is just to make the text at the top fade out
			if (topText.Alpha > 0) {
				topText.Alpha -= .01f;
			}

			//Here we'll make the elevator rise and fall - all of the constants chosen here are just after tinkering
			if (rising) {
				elevator.Velocity.Y-= 10;
			}else {
				elevator.Velocity.Y+= 10;
			}
			if (elevator.Velocity.Y == -300) {
				rising = false;
			}else if (elevator.Velocity.Y == 300) {
				rising = true;
			}

			//Run through the groups, and if a crate is off screen, get it back!
			foreach(FlxSprite a in crateStormGroup.Members) {
				if (a.X < -10)
					a.X = 400;
				if (a.X > 400)
					a.X = -10;
				if (a.Y > 300)
					a.Y = -10;
			}
			foreach(FlxSprite a in crateStormGroup2.Members) {
				if (a.X > 400)
					a.X = -10;
				if (a.X < -10)
					a.X = 400;
				if (a.Y > 300)
					a.Y = -10;
			}
			base.update();

			//Here we call our simple collide() function, what this does is checks to see if there is a collision
			//between the two objects specified, But if you pass in a group then it checks the group against the object,
			//or group against a group, You can even check a group of groups against an object - You can see the possibilities this presents.
			//To use it, simply call FlxG.collide(Group/Object1, Group/Object2, Notification(optional))
			//If you DO pass in a notification it will fire the function you created when two objects collide - allowing for even more functionality.
			if(collideGroups)
				FlxG.collide(crateStormGroup, crateStormGroup2);
			if(isCrateStormOn)
				FlxG.collide(elevator, crateStormMegaGroup);
			if (isFlxRiderOn) 
				FlxG.collide(elevator, flixelRider);
			//We don't specify a callback here, because we aren't doing anything base specific - just using the default collide method.
		}

		//This calls our friend the Flixel Rider into play
		private void onFlixelRider() {
			if(!isFlxRiderOn){
				isFlxRiderOn = true; //Make the state aware that Flixel Rider is here
				isCrateStormOn = false; //Tell the state that the crates are off as of right now
				crateStormGroup.Visible = crateStormGroup.Exists = false; //Turn off the Blue crates
				crateStormGroup2.Visible = crateStormGroup2.Exists = false; //Turn off the Red crates
				flixelRider.Solid = flixelRider.Visible = flixelRider.Exists = true; //Turn on the Flixel Rider
				flixelRider.Y = flixelRider.Velocity.Y = 0; //Reset him at the top of the screen(Dont be like me and have him appear under the elevator :P)
				crateStormG1.Visible = false; //Turn off the button for toggling the Blue group
				crateStormG2.Visible = false; //Turn ooff the button for toggling the Red group
				groupCollision.Visible = false; //Turn off the button for toggling group collision
				topText.text = "Flixel Elevator Rider!";
				topText.Alpha = 1;
			}
		}

		//Enable the CRATE STOOOOOORM!
		private void onCrateStorm() {
			isCrateStormOn = true;
			isFlxRiderOn = false;
			if(blueGroup)
				crateStormGroup.Visible = crateStormGroup.Exists = true;
			if(redGroup)
				crateStormGroup2.Visible = crateStormGroup2.Exists = true;
			flixelRider.Solid = flixelRider.Visible = flixelRider.Exists =false;
			crateStormG1.Visible = true;
			crateStormG2.Visible = true;
			if(blueGroup && redGroup)
				groupCollision.Visible = true;
			topText.text = "CRATE STOOOOORM!";
			topText.Alpha = 1;
		}

		//Toggle the Blue group
		private void onBlue() {
			blueGroup = !blueGroup;
			crateStormGroup.Visible = crateStormGroup.Exists = !crateStormGroup.Exists;
			foreach(FlxSprite a in crateStormGroup.Members) {
				a.Solid = !a.Solid;//Run through and make them not collide - I'm not sure if this is neccesary
			}
			if (blueGroup && redGroup) {
				groupCollision.Visible = true;
			}else {
				groupCollision.Visible = false;
			}
			if(!blueGroup){
				topText.text = "Blue Group: Disabled";
				topText.Alpha = 1;
			}else {
				topText.text = "Blue Group: Enabled";
				topText.Alpha = 1;
			}
		}

		//Toggle the Red group
		private void onRed() {
			redGroup = !redGroup;
			crateStormGroup2.Visible = crateStormGroup2.Exists = !crateStormGroup2.Exists;
			foreach(FlxSprite a in crateStormGroup2.Members) {
				a.Solid = !a.Solid;
			}
			if (blueGroup && redGroup) {
				groupCollision.Visible = true;
			}else {
				groupCollision.Visible = false;
			}
			if(!redGroup){
				topText.text = "Red Group: Disabled";
				topText.Alpha = 1;
			}else {
				topText.text = "Red Group: Enabled";
				topText.Alpha = 1;
			}
		}

		//Toggle the group collision
		private void onCollideGroups() {
			collideGroups = !collideGroups;
			if(!collideGroups){
				topText.text = "Group Collision: Disabled";
				topText.Alpha = 1;
			}else {
				topText.text = "Group Collision: Enabled";
				topText.Alpha = 1;
			}
		}
		//This just quits - state.destroy() is automatically called upon state changing
		private void onQuit() {
			FlxG.switchState(new MenuState());
		}
	}
}

