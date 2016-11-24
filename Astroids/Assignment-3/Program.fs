module Asteroids

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input
open State
open System
open Helpers
open Assignment


let spriteLoader (path) graphics =
    use imagePath = System.IO.File.OpenRead(path)
    let texture = Texture2D.FromStream(graphics, imagePath)
    let textureData = Array.create<Color> (texture.Width * texture.Height) Color.Transparent
    texture.GetData(textureData)
    texture

type Game1() as this =
    inherit Game()

    do this.Content.RootDirectory <- "Content"
    let graphics = new GraphicsDeviceManager(this);
    let mutable spriteBatch = Unchecked.defaultof<SpriteBatch>
    let mutable gameState = GameState.Zero(Assignment.filter_projectiles, Assignment.filter_asteroids)

    override this.Initialize() =
        graphics.PreferredBackBufferWidth <- 1366
        graphics.PreferredBackBufferHeight <- 768

        graphics.IsFullScreen <- false
        graphics.ApplyChanges()
        this.IsMouseVisible <- false

        do base.Initialize()

    override this.LoadContent() = 
        System.IO.Directory.SetCurrentDirectory("Content")
        do spriteBatch <- new SpriteBatch(this.GraphicsDevice)
        let textures = 
            Map.empty.
                Add("Ship", this.Content.Load<Texture2D>("Ship")).
                Add("thruster", this.Content.Load<Texture2D>("thruster")).
                Add("projectile", this.Content.Load<Texture2D>("projectile")).
                Add("asteroid",this.Content.Load<Texture2D>("asteroid"))
        gameState <- gameState.LoadTextures(textures)
                
    override this.Update (gameTime) =
        gameState <- gameState.Update(gameTime)

    override this.Draw(gameTime) =
        do this.GraphicsDevice.Clear Color.Black
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend)
        gameState.Draw(spriteBatch)
        spriteBatch.End()