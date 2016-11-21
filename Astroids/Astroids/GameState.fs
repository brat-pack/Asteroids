module State

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input

let r = new System.Random()

let checkInput key onPressed onNotPressed = 
    let ks = Keyboard.GetState()
    match ks.[key] with
            | KeyState.Down -> onPressed
            | _ -> onNotPressed
let direction rotation =
    new Vector2(sin(rotation), (cos(rotation) * -1.0f))

type GameState = {
    ship     : SpaceShip
    projectiles : Projectile list
    asteroids : Asteroid list
    textures : Map<string, Texture2D>
    asteroidsTimer : float
} with 
    static member Zero() = 
        {
            ship = SpaceShip.Zero()
            projectiles = []
            asteroids = []
            textures = Map.empty
            asteroidsTimer = 2500.0
        }
    member this.LoadTextures(textures : Map<string, Texture2D>) =
        let ship' = { this.ship with texture = textures.["Ship"]; thruster = textures.["thruster"]}
        {
            ship = ship'
            projectiles = []
            asteroids = []
            textures = textures
            asteroidsTimer = 2500.0
        }
    member this.Update(dt : GameTime) = 
        let asteroidsTimer', asteroids' = this.UpdateAsteroids(dt)
        let ship', newprojectile = this.ship.Update(dt, this)
        let projectiles = 
            match newprojectile with
            | Some(x) -> x :: this.projectiles
            | None -> this.projectiles
        let projectiles' = List.map (fun (x : Projectile) -> x.Update(dt, this)) projectiles
        let projectiles'' = List.filter (fun (x : Projectile) -> x.isInsideScreen()) projectiles'
        {this with ship = ship' ; projectiles = projectiles''; asteroids = asteroids'; asteroidsTimer = asteroidsTimer'}
    member this.Draw(spriteBatch : SpriteBatch) = 
        List.iter (fun (x : Asteroid) -> x.Draw(spriteBatch)) this.asteroids
        List.iter (fun (x : Projectile) -> x.Draw(spriteBatch)) this.projectiles
        this.ship.Draw(spriteBatch, this)

    member this.UpdateAsteroids(dt) = 
        let asteroids' = List.map (fun (x : Asteroid) -> x.Update(dt, this)) this.asteroids
        if this.asteroidsTimer < 0.0 then
            2500.0, Asteroid.Create(this.textures.["asteroid"]) :: asteroids'
        else
           this.asteroidsTimer - dt.ElapsedGameTime.TotalMilliseconds, asteroids'

and SpaceShip = {
    velocity : Vector2
    position : Vector2
    rotation : float
    impulse  : Vector2
    texture  : Texture2D
    thruster : Texture2D
    cooldown : float
} with 
    static member Zero() =
        {
            velocity = new Vector2()
            position = new Vector2(300.0f,300.0f)
            rotation = 0.0
            impulse = new Vector2()
            texture = null
            thruster = null
            cooldown = 0.0
        }
    member this.Update(dt, gameState) =
        let impulse' = direction ((float32)this.rotation) * 0.007f
        let velocity' = checkInput Keys.W (this.velocity + impulse') this.velocity
        let rotation' = this.rotation + checkInput Keys.A -0.03 0.0 + checkInput Keys.D 0.03 0.0
        let position' = this.position + this.velocity * ((float32)dt.ElapsedGameTime.TotalMilliseconds) 
        let ship' : SpaceShip = {this with position = position'}.ContainX().ContainY()
        let projectile, cooldown' = this.Fire(gameState, dt)
        { ship' with velocity = velocity';rotation = rotation';impulse = impulse'; cooldown = cooldown'}, projectile
    member this.Draw(spriteBatch : SpriteBatch, gameState) = 
        let texture = gameState.textures.["Ship"]
        let origin = new Vector2((float32)texture.Width / 2.0f, (float32)texture.Height / 2.0f)
        let sourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height)
        do spriteBatch.Draw(texture, this.position, System.Nullable(sourceRectangle), Color.White, (float32)this.rotation, origin, 1.0f, SpriteEffects.None, 1.0f)
        let ks = Keyboard.GetState()
        match ks.[Keys.W] with
            | KeyState.Down -> do this.DrawThruster(spriteBatch, gameState)
            | _ -> ()
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
    member this.DrawThruster(spriteBatch, gameState) =        
        let texture = this.thruster
        let offset = (float32)this.texture.Height / 2.0f
        let origin = new Vector2((float32)texture.Width / 2.0f, (-(float32)this.texture.Height / 2.0f) + 1.0f)
        let sourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height)
        spriteBatch.Draw(texture, this.position, System.Nullable(sourceRectangle), Color.White, (float32)this.rotation, origin, 1.0f, SpriteEffects.None, 1.0f)
    member this.Fire(gameState, dt) =
        if checkInput Keys.Space true false && this.cooldown < 0.0 then
            Some (Projectile.Create(gameState.textures.["projectile"], this.position, this.rotation)), 1000.0
        else
            None, this.cooldown - dt.ElapsedGameTime.TotalMilliseconds


and Projectile = {
    texture : Texture2D
    position : Vector2
    velocity : Vector2
    rotation : float
} with
    static member Create(texture, position, rotation) = 
        let velocity = direction ((float32)rotation)
        {
            texture = texture
            position = position
            velocity = velocity
            rotation = rotation
        } 

    member this.Update(dt, gameState) = 
        let position' = this.position + this.velocity * 10.0f
        {this with position = position'}
    member this.Draw(spriteBatch) = 
        let origin = new Vector2((float32)this.texture.Width / 2.0f, ((float32)this.texture.Height / 2.0f) )
        let sourceRectangle = new Rectangle(0, 0, this.texture.Width, this.texture.Height)
        spriteBatch.Draw(this.texture, this.position, System.Nullable(sourceRectangle), Color.White, (float32)this.rotation, origin, 1.0f, SpriteEffects.None, 1.0f)
    member this.isInsideScreen() =
        this.position.X > 0.0f && this.position.X < 1920.0f && this.position.Y > 0.0f && this.position.Y < 1080.0f 

and Asteroid = {
    texture : Texture2D
    position : Vector2
    velocity  : Vector2  
    angularVelocity : float
    rotation : float
} with
    static member Create(texture) =
        let rotation = float(r.Next(-314,314)) / 100.0
        let velocity = direction ((float32)rotation) * float32(r.Next(5, 35)) / 100.0f
        {
            rotation = rotation
            velocity = velocity
            position = new Vector2(float32(r.Next(0,1920)), float32(r.Next(0, 1080)))  
            texture = texture
            angularVelocity = float(r.Next(-7,7)) / 100.0 
        }
    
    member this.Update(dt, gameState) = 
        let position' = this.position + this.velocity 
        let rotation' = this.rotation + this.angularVelocity
        {this with position = position'; rotation = rotation'}
    member this.Draw(spriteBatch : SpriteBatch) = 
        let origin = new Vector2((float32)this.texture.Width / 2.0f, ((float32)this.texture.Height / 2.0f) )
        let sourceRectangle = new Rectangle(0, 0, this.texture.Width, this.texture.Height)
        spriteBatch.Draw(this.texture, this.position, System.Nullable(sourceRectangle), Color.White, (float32)this.rotation, origin, 1.0f, SpriteEffects.None, 1.0f)
    member this.isInsideScreen() =
        this.position.X > 0.0f && this.position.X < 1920.0f && this.position.Y > 0.0f && this.position.Y < 1080.0f
            