module Assignment
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input
open Microsoft.Xna.Framework
open State

// This is assignment 3 Arguably the hardest of the three.
// In here you will be using lists and recursive procedures.
// Some syntax may be alien to you. It is up to you to solve them. Different parts of the recursion have been left 
// out of all three procedures. Careful studying will show you what parts you need to re-use to make them complete.
// Remember: Google is your friend.


// ASSIGNMENT 3A: CREATING A PREDICATE
// This procedure is going to be a predicate that returns true when the project has hit an asteroid.
// projectile is the current projectile.
// asteroids is a collection containing many asteroids.
// The default value of count is 0

let rec projectile_collides_with_asteroid(projectile: Projectile, asteroids: Asteroid list) =
    if asteroids.IsEmpty then //TODO: Correctly fix the length check
        false
    elif projectile.collides_with(asteroids.Item(0)) then
        true
    else
        false //TODO: Add the looping mechanism


// ASSIGNMENT 3B: FILTER OUT DESTROYED PROJECTILES
// When projectile hits an asteroid, it must be destroyed.
// This procedure fills up and returns a list with all the projectiles that have not been destroyed.
// PARAMETERS:
// Projectiles is a list of all currently existing projectiles.
// Asteroids is a list of all currently existing asteroids.
// newProjectiles is an empty list that will eventually contain all the projectiles not destroyed.


let rec filter_projectiles(projectiles: Projectile list, asteroids: Asteroid list, newProjectiles) =
    if projectiles.IsEmpty then
        newProjectiles
    elif projectile_collides_with_asteroid(projectiles.Item(0), asteroids) then //TODO: Add predicate
        projectiles //TODO: Add a loop here.
    else
        filter_projectiles(projectiles.Tail, asteroids, projectiles.Item(0) :: newProjectiles)


// ASSIGNMENT 3C: FILTER OUT DESTROYED ASTEROIDS
// This procedure returns a new list with only the alive asteroids.
// PARAMETERS:
// Projectiles is a collection of all currently existing projectiles.
// Asteroids is a collection of all currently existing asteroids.
// HINT:
// Each asteroid has an associated variable. The .alive variable.
// e.g.: asteroid.alive
// This is a predicate that is false if the asteroid has hit a projectile.
// Use this to determine whether an asteroid needs to be filtered out

let rec filter_asteroids(projectiles: Projectile list, asteroids: Asteroid list) =
    //TODO: Filter out all the asteroids that are hit by a projectile.
    asteroids