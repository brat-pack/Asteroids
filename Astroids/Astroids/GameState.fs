module State

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input

let checkInput key onPressed onNotPressed = 
    let ks = Keyboard.GetState()
    match ks.[key] with
            | KeyState.Down -> onPressed
            | _ -> onNotPressed

let direction rotation =
    new Vector2(sin(rotation), (cos(rotation) * -1.0f))

type GameState = {
    ship     : SpaceShip
    textures : Map<string, Texture2D>
} with 
    static member Zero() = 
        {
            ship = SpaceShip.Zero()
            textures = Map.empty
        }
    member this.Update(dt : GameTime) = 
        let ship' = this.ship.Update(dt, this)
        {this with ship = ship'}
    member this.Draw(spriteBatch : SpriteBatch) = 
        this.ship.Draw(spriteBatch, this)

and SpaceShip = {
    velocity : Vector2
    position : Vector2
    rotation : float
    impulse  : Vector2
} with 
    static member Zero() =
        {
            velocity = new Vector2()
            position = new Vector2(300.0f,300.0f)
            rotation = 0.0
            impulse = new Vector2()
        }
    member this.Update(dt, gameState) =
        let impulse' = direction ((float32)this.rotation) * 0.007f
        let velocity' = checkInput Keys.W (this.velocity + impulse') this.velocity
        let rotation' = this.rotation + checkInput Keys.A -0.03 0.0 + checkInput Keys.D 0.03 0.0
        let position' = this.position + this.velocity * ((float32)dt.ElapsedGameTime.TotalMilliseconds) 
        let ship' : SpaceShip = {this with position = position'}.ContainX().ContainY()
        { ship' with velocity = velocity';rotation = rotation';impulse = impulse'} 
    member this.Draw(spriteBatch : SpriteBatch, gameState) = 
        let texture = gameState.textures.["Ship"]
        let origin = new Vector2((float32)texture.Width / 2.0f, (float32)texture.Height / 2.0f)
        let sourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height)
        spriteBatch.Draw(texture, this.position, System.Nullable(sourceRectangle), Color.White, (float32)this.rotation, origin, 1.0f, SpriteEffects.None, 1.0f)
    // Change 50.0f to the textures width/height
    member this.ContainX() : SpaceShip =
        if this.position.X >  1920.0f + 50.0f then
            {this with position = new Vector2(-50.0f, this.position.Y) }
        elif this.position.X < -50.0f then
            {this with position = new Vector2(1920.0f + 50.0f, this.position.Y)}
        else 
            this
    member this.ContainY() : SpaceShip =
        if this.position.Y >  1080.0f + 50.0f then
            {this with position = new Vector2(this.position.X, -50.0f) }
        elif this.position.Y < -50.0f then
            {this with position = new Vector2(this.position.X, 1080.0f + 50.0f)}
        else 
            this