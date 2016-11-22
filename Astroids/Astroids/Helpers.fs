module Helpers

open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input
open Microsoft.Xna.Framework

let MakeRect (texture : Texture2D, position : Vector2) = 
        let width = texture.Width
        let height = texture.Height
        let left = (int)position.X - (width / 2)
        let top = (int)position.Y - (height / 2)
        let rect = new Rectangle(left, top, width, height)
        rect

let isInsideScreen(position: Vector2) = 
    position.X > 0.0f && position.X < 1920.0f && position.Y > 0.0f && position.Y < 1080.0f


let screenLoop(position: Vector2) =
    let x' : float32 =
        if position.X > 1920.0f + 50.0f then
            -50.0f
        elif position.X < -50.0f then
            1920.0f + 50.0f
        else 
            position.X

    let y' : float32 =
        if position.Y >  1920.0f + 50.0f then
            -50.0f
        elif position.Y < -50.0f then
            1080.0f + 50.0f
        else 
            position.Y
    new Vector2(x', y')


let CheckCollision(rect : Rectangle, rectlist : Rectangle list) = 
    not (List.exists(fun (recti : Rectangle) -> rect.Intersects(recti)) rectlist)
     
let checkInput key onPressed onNotPressed = 
    let ks = Keyboard.GetState()
    match ks.[key] with
            | KeyState.Down -> onPressed
            | _ -> onNotPressed

let direction rotation =
    new Vector2(sin(rotation), (cos(rotation) * -1.0f))

let updateTimer defaultTime currentTime (dt: GameTime) =
    if currentTime < 0.0 then
        defaultTime
    else
        currentTime - dt.ElapsedGameTime.TotalMilliseconds