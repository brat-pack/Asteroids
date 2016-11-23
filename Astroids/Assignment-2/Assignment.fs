module Assignment

open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input
open Microsoft.Xna.Framework

// Assignment A
//
// check if the screen contains the spaceship
// the size of the ship is 50.0f * 50.0f
// the size of the screen is 1920.0f * 1080.0f
// if the ship is outside the screen, its position should be set to the opposite side of the screen
// change 0.0f to the correct values

let containInsideScreen(position: Vector2) =
    let x =
        if position.X > 0.0f + 50.0f then
            -50.0f
        elif position.X < -50.0f then
            0.0f + 50.0f
        else 
            position.X
    let y =
        if position.Y > 0.0f + 50.0f then
            -50.0f
        elif position.Y < -50.0f then
            0.0f + 50.0f
        else 
            position.Y
    new Vector2(x, y)

// Assignment B
//
// the deltaTime is the time between now and the last procedure executed
// the cooldown is the time the missiles can't shoot
// we want to know the new cooldown  


let updateMissileCooldown(cooldown : float, deltaTime : float) =
    -0.1f