module Helpers

open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework

let spriteLoader (path) graphics =
    use imagePath = System.IO.File.OpenRead(path)
    let texture = Texture2D.FromStream(graphics, imagePath)
    let textureData = Array.create<Color> (texture.Width * texture.Height) Color.Transparent
    texture.GetData(textureData)
    texture
