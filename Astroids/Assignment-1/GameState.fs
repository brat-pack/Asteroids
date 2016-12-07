module State
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input
open Helpers
open Assignment

let r = new System.Random()

type GameState = {
    ship     : SpaceShip
    projectiles : Projectile list
    asteroids : Asteroid list
    textures : Map<string, Texture2D>
    asteroidTimer : float
    asteroidSpawnRate: float
} with 
    static member Zero() = 
        {
            ship = SpaceShip.Zero()
            projectiles = []
            asteroids = []
            textures = Map.empty
            asteroidTimer = 2500.0
            asteroidSpawnRate = 1200.0
        }

    member this.LoadTextures(textures : Map<string, Texture2D>) =
        let ship' = { this.ship with texture = textures.["Ship"]; thruster = textures.["thruster"]}
        { this with
            ship = ship'
            textures = textures
        }

    member this.Update(dt : GameTime) =
        let ship', createdProjectile = this.ship.Update(dt, this)
        let projectiles' = 
            List.filter (fun (x : Projectile) -> x.alive) this.projectiles |>
            List.map (fun (x : Projectile) -> x.Update(dt, this)) |>
            fun projectiles -> 
                match createdProjectile with 
                | Some x -> x :: projectiles
                | None -> projectiles

        let asteroidTimer' = this.updateAsteroidTimer this.asteroidTimer dt
        let asteroids' =
            let updatedAsteroids = this.asteroids |> List.filter (fun x-> x.alive) |> List.map (fun x -> x.Update(dt, this))
            if this.asteroidTimer < 0.0 then
                 Asteroid.Create(this.textures.["asteroid"]) :: updatedAsteroids
            else
                updatedAsteroids
        
        {this with 
            ship = ship'
            projectiles = projectiles'
            asteroids = asteroids'
            asteroidTimer = asteroidTimer'
        }

    member this.Draw(spriteBatch : SpriteBatch) = 
        List.iter (fun (x : Asteroid) -> x.Draw(spriteBatch)) this.asteroids
        List.iter (fun (x : Projectile) -> x.Draw(spriteBatch)) this.projectiles
        this.ship.Draw(spriteBatch, this)

    member this.updateAsteroidTimer = updateTimer this.asteroidSpawnRate

    member this.updateAsteroids(dt: GameTime) =
        fun x->
            List.filter(fun x -> x.alive) x |> List.map(fun x-> x.Update(dt, this))

and SpaceShip = {
    velocity : Vector2
    position : Vector2
    rotation : float
    impulse  : Vector2
    texture  : Texture2D
    thruster : Texture2D
    cooldown : float
    fireRate : float
    turnSpeed: float
} with 
    static member Zero() =
        {
            velocity = new Vector2()
            position = new Vector2((float32)(r.Next(100,1266)),(float32)(r.Next(100,570)))
            rotation = 0.0
            impulse = new Vector2()
            texture = null
            thruster = null
            cooldown = 0.0
            fireRate = 0.25
            turnSpeed = 0.05
        }

    member this.Rect() = 
        MakeRect(this.texture, this.position) 
    
    member this.Update(dt: GameTime, gameState) =
        if (this.collidesWithAsteroid(gameState.asteroids)) then
            { SpaceShip.Zero() with texture = gameState.textures.["Ship"]; thruster = gameState.textures.["thruster"]}, None
        else
            let impulse' = direction ((float32)this.rotation) * 0.007f
            let velocity' = checkInput Keys.W (this.velocity + impulse') this.velocity
            let rotation' = this.rotation + checkInput Keys.A -this.turnSpeed 0.0 + checkInput Keys.D this.turnSpeed 0.0
            let position' = move(this.position, this.velocity * ((float32)dt.ElapsedGameTime.TotalMilliseconds)) |> screenLoop 
            let ship' : SpaceShip = {this with position = position'}
            let projectile, cooldown' = this.Fire(gameState, dt)
            { ship' with velocity = velocity';rotation = rotation';impulse = impulse'; cooldown = cooldown';}, projectile

    member this.Draw(spriteBatch : SpriteBatch, gameState) = 
        let texture = gameState.textures.["Ship"]
        let origin = new Vector2((float32)texture.Width / 2.0f, (float32)texture.Height / 2.0f)
        let sourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height)
        do spriteBatch.Draw(texture, this.position, System.Nullable(sourceRectangle), Color.White, (float32)this.rotation, origin, 1.0f, SpriteEffects.None, 1.0f)
        let ks = Keyboard.GetState()
        match ks.[Keys.W] with
            | KeyState.Down -> do this.DrawThruster(spriteBatch, gameState)
            | _ -> ()

    member this.DrawThruster(spriteBatch, gameState) =        
        let texture = this.thruster
        let offset = (float32)this.texture.Height / 2.0f
        let origin = new Vector2((float32)texture.Width / 2.0f, (-(float32)this.texture.Height / 2.0f) + 1.0f)
        let sourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height)
        spriteBatch.Draw(texture, this.position, System.Nullable(sourceRectangle), Color.White, (float32)this.rotation, origin, 1.0f, SpriteEffects.None, 1.0f)

    member this.Fire(gameState, dt) =
        if checkInput Keys.Space true false && this.cooldown < 0.0 then
            Some(Projectile.Create(gameState.textures.["projectile"], this.position, this.rotation)), this.fireRate * 1000.0
        else
            None, this.cooldown - dt.ElapsedGameTime.TotalMilliseconds

    member this.collidesWithAsteroid(asteroids) =
        let asteroidRects = List.map(fun (x: Asteroid) -> x.Rect()) asteroids
        not (CheckCollision(this.Rect(), asteroidRects))

and Projectile = {
    texture : Texture2D
    position : Vector2
    velocity : Vector2
    rotation : float
    alive : bool
} with
    static member Create(texture, position, rotation) = 
        let velocity = direction ((float32)rotation)
        {
            texture = texture
            position = position
            velocity = velocity
            rotation = rotation
            alive = true
        } 

    member this.Rect() = 
        MakeRect(this.texture, this.position) 

    member this.Update(dt, gameState) =
        let alive' = this.isInsideScreen() || this.Collide(gameState.asteroids)
        let position' = this.position + this.velocity * 10.0f
        {this with position = position'; alive = alive'}

    member this.Draw(spriteBatch) = 
        let origin = new Vector2((float32)this.texture.Width / 2.0f, ((float32)this.texture.Height / 2.0f) )
        let sourceRectangle = new Rectangle(0, 0, this.texture.Width, this.texture.Height)
        spriteBatch.Draw(this.texture, this.position, System.Nullable(sourceRectangle), Color.White, (float32)this.rotation, origin, 1.0f, SpriteEffects.None, 1.0f)

    member this.isInsideScreen() =
        this.position.X > 0.0f && this.position.X < 1920.0f && this.position.Y > 0.0f && this.position.Y < 1080.0f 

    member this.Collide(asteroids : Asteroid list) =
        let asteroidRects = List.map(fun (asteroid : Asteroid) -> asteroid.Rect()) asteroids
        CheckCollision(this.Rect(), asteroidRects)

and Asteroid = {
    texture : Texture2D
    position : Vector2
    velocity  : Vector2  
    angularVelocity : float
    rotation : float
    alive : bool
} with
    static member Create(texture) =
        let rotation = float(r.Next(-314,314)) / 100.0
        let xSpawnPoint = if (r.Next(-1,1) = 0) then 1950.0f else -30.0f
        let ySpawnPoint = if (r.Next(-1,1) = 0) then 1110.0f else -30.0f
            
        let velocity = new Vector2((float32)(r.Next(-350, 350)) / 100.0f, (float32)(r.Next(-350, 350)) / 100.0f)
        {
            rotation = rotation
            velocity = velocity
            position = new Vector2(xSpawnPoint, ySpawnPoint)  
            texture = texture
            angularVelocity = float(r.Next(-7,7)) / 100.0
            alive = true;
        }

    member this.Rect() =
        MakeRect(this.texture, this.position)
    
    member this.Collide(projectiles : Projectile list) =
        let projectiles' = List.map(fun (x : Projectile) -> x.Rect()) projectiles
        CheckCollision(this.Rect(), projectiles')

    member this.Update(dt, gameState) =
        let alive' = this.Collide(gameState.projectiles)
        let position' = this.position + this.velocity |> screenLoop
        let rotation' = this.rotation + this.angularVelocity
        {this with position = position'; rotation = rotation'; alive = alive'}

    member this.Draw(spriteBatch : SpriteBatch) = 
        let origin = new Vector2((float32)this.texture.Width / 2.0f, ((float32)this.texture.Height / 2.0f) )
        let sourceRectangle = new Rectangle(0, 0, this.texture.Width, this.texture.Height)
        spriteBatch.Draw(this.texture, this.position, System.Nullable(sourceRectangle), Color.White, (float32)this.rotation, origin, 1.0f, SpriteEffects.None, 1.0f)