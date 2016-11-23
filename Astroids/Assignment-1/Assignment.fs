module Assignment

open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input
open Microsoft.Xna.Framework

// Assignment A
// 
// position is the current a vector with the current X and Y position of the spaceship
// we want to add the velocity by the position
// Vector2(x, y) creates a vector with an X and Y value

let move(position : Vector2, velocity : Vector2) =
    new Vector2(position.X, position.Y)

// Assignment B
//
// rotation is a value between -3.14159 and 3.14159 (-pi and pi)
// search on the internet for a formula to calculate the direction with radiants
// Vector2(x, y) creates a vector with an X and Y value
// the default values are 0, you have to enter the correct values

let direction rotation =
    new Vector2(0.0f, 0.0f)

