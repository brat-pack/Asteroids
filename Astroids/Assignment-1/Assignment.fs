module Assignment

open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input
open Microsoft.Xna.Framework

// Assignment 1 is all about procedures.
// In this assignment we will make sure the ship can move foward, and in the right direction
// Remember: google is your friend.

// ASSIGNMENT 1A: FOWARD!
// position is the current a vector with the current X and Y position of the spaceship
// we want to add the velocity by the position
// Vector2(x, y) creates a vector with an X and Y value
// note, the : Vector behind the parameters tell you that it is a vector2.

let move(position: Vector2, velocity: Vector2) =
    new Vector2(position.X + velocity.X, position.Y + velocity.Y)

// Assignment 1B: TURN IT!
// rotation is a value between -3.14159 and 3.14159 (-pi and pi)
// search on the internet for a formula to calculate the direction with radians
// Vector2(x, y) creates a vector with an X and Y value
// the default values are 0, and -1.0 you have to enter the correct values
// You may also want to search for F# built-in math functions
// note: the f after 0.0f is an idiosyncrasy of F#. If you are unclear about whether to use 0.0f or 0.0, ask for help.

let direction(rotation) =
    new Vector2(0.0f, -0.1f)