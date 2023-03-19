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

Fight your way through hoards of enemy ships that are attacking your home world. Can you fight all the way to the mothership? 

### Controls

-   Movement
    -   Up: W
    -   Down: S 
    -   Left: A
    -   Right: D
-   Fire: Space

## Your Additions

Documentation:
-   Player movement: as above, when coming into contact with edges of the screen player movement is halted
-   Player has a healthbar and lives, when your health runs out you will respawn at the bottom center of the screen and will be immune for 3 seconds
-   Player shooting delay: there is a time delay between allowing the player to shoot, starting at .2 seconds, this is done with a simple timer float by increasing it by Time.deltaTime
-   reaching level 10 will spawn a boss. Defeating the boss will end the game, A player running out of lives will end the game.

v1.0.1 additions
-   bug fixes - see Known Issues
-   improved enemy spawning
-   Added easy mode (Gain 100 coins at the start of every level)

v1.0.2
-   bug fixes

v1.0.3
-   Changed how armor scaled from addative damage reduction to percent damage
-   Made armor required to kill the final boss - boss does 100 damage with each bullet
-   Changed the boss's bullet sprite to a red missile to indicate the increased danger from its bullets
-   Added an additional 10 waves of enemies before spawning a second boss
-   Added a pause and confirmation window between levels to give the player a break

Enemy types:
currently 5 simple enemy types, each type has increasing amount of health, damage, and bullet amounts

1 boss enemy that uses 1 of 2 abilities, chosen randomly
-   wave of bombs that cover the screen, bombs explode after 6 seconds
-   waves of bullets fired in an arc

## Sources

-   Player spaceship: https://craftpix.net/freebies/free-spaceship-pixel-art-sprite-sheets/
-   Backgrounds: https://craftpix.net/product/space-vertical-game-backgrounds/?num=1&count=160&sq=space%20background&pos=2
-   Enemy ships: https://craftpix.net/product/enemy-spaceship-game-sprites/?num=2&count=313&sq=enemy%20ship&pos=10
-   Gui: https://craftpix.net/freebies/free-space-shooter-game-gui/?num=1&count=160&sq=space%20background&pos=3

## Known Issues

-   game doesn't actually end when the boss dies (fixed v1.0.1)
-   Unable to exit the rules screen that is on the main menu (fixed v1.0.1)
-   Hardest enemies were not spawning (fixed v1.0.1)
-   Bosses bullets do 0 damage!! (fixed v1.0.3)
-   Enemies remaining text is not at the top of the screen due to the different aspect ratio in the Unity web player (fixed v1.0.2)
-   going into full screen mode on the web build breaks the shop
- Shop items no displaying correctly (fixed v1.0.4)

### Requirements not completed

None

