module Assignment

open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input
open Microsoft.Xna.Framework

// Assignment 2 is all about conditionals.
// In this assignment we will regulate the firing speed of the missiles and make sure
// when the spaceship leaves the screen, he returns on the opposite side.
// This will use knowledge you gained from assignment 1.
// Remember: google is your friend.

// Assignment 2A: SPACESHIP LEAVING THE SCREEN
// check if the screen contains the spaceship
// the size of the ship is 50.0f * 50.0f
// the size of the screen is 1366.0f * 768.0f
// if the ship is outside the screen, its position should be set to the opposite side of the screen
// change 0.0f to the correct values
// Note: The 0,0 position is in the top left
// Your job is to add the predicates for checking whether the ship has left the screen & to fill in the correct values
// inside each if block.

let containInsideScreen(position: Vector2) =
    let x =
        if false then //TODO: Add a predicate for when the ship leaves at the right side.
            -50.0f
        elif false then //TODO: Add a predicate for when the ship leaves at the left side.
            0.0f + 50.0f //TODO: Change 0.0f to the proper number.
        else 
            position.X
    let y =
        if false then //TODO: Add a predicate for when the ship leaves at the bottom.
            -50.0f
        elif false then //TODO: Add a predicate for when the ship leaves at the top.
            0.0f + 50.0f //TODO: Change 0.0f to the proper number.
        else 
            position.Y
    new Vector2(x, y)



// ASSIGNMENT 2B: COOL IT
// Firing the ship's weapon must incur a cooldown. Below is a procedure to either calculate the new current cooldown or reset
// it to the full timer after firing.
// current_cooldown is the remaining cooldown time of the weapon in milliseconds.
// deltaTime is the time between now and the last time the procedure was executed in milliseconds.
// default_cooldown is the total cooldown a weapon should have once it has been fired
// hasFired is the result of a predicate that checked whether the ship has fired its weapons.

let updateMissileCooldown(current_cooldown: float, deltaTime, default_cooldown, hasFired) =
    0.0