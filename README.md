# Project SHMUP Project

[Markdown Cheatsheet](https://github.com/adam-p/markdown-here/wiki/Markdown-Here-Cheatsheet)

### Student Info

-   Name: Michael Eaton
-   Section: IGME.202.01-02

## Game Design

-   Camera Orientation: top down
-   Camera Movement: No camera movement
-   Player Health: Healthbar and Lives
-   End Condition: each level has a preset number or pattern of enemies, buy upgrades in between levels,
		boss ends the game, player out of lives ends the game
-   Scoring: Killing enemies awards coins that can be spent on upgrades as well as giving a score to the player

### Game Description

_A brief explanation of your game. Inculde what is the objective for the player. Think about what would go on the back of a game box._

### Controls

-   Movement
    -   Up: W
    -   Down: S 
    -   Left: A
    -   Right: D
-   Fire: Space

## Your Additions

Documentation:
Player movement: as above, when coming into contact with edges of the screen player movement is halted
Player has a healthbar and lives, when your health runs out you will respawn at the bottom center of the screen and will be immune for 3 seconds,
Player shooting delay: there is a time delay between allowing the player to shoot, starting at .2 seconds, this is done with a simple timer float by increasing it by Time.deltaTime

Enemy types:
currently 5 simple enemy types, each type has increasing amount of health, damage, and bullet amounts

## Sources

-   Player spaceship: https://craftpix.net/freebies/free-spaceship-pixel-art-sprite-sheets/
-   Backgrounds: https://craftpix.net/product/space-vertical-game-backgrounds/?num=1&count=160&sq=space%20background&pos=2
-   Enemy ships: https://craftpix.net/product/enemy-spaceship-game-sprites/?num=2&count=313&sq=enemy%20ship&pos=10
-   Gui: https://craftpix.net/freebies/free-space-shooter-game-gui/?num=1&count=160&sq=space%20background&pos=3

## Known Issues

Game crashes at wave 10
Died and lives dropped to -900, next wave didnt spawn?

### Requirements not completed

_If you did not complete a project requirement, notate that here_

