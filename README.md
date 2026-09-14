# MG2 Prototype #1

Goal of this prototype is to create a standard merge board for MG2 concept that makes exploration more intrinsic to the merge game mechanic. Specifically, this prototype is play testing the integration of spawners into the board itself and the dig powerup.

## Screenshots

## Demo Video

## Gameplay Elements

**Board**
- Fixed 6x8 board with constant background image
- Certain areas of the board are a "dead zone" where no objects can exist
- Organizes position of spawned objects, including a full board

**Board Objects**
- Indexed (row, column) to their position on the board
- Move when dragged and dropped
  - Bounce back to original position if dropped on "dead zone"
  - Swap positions if placed on another object

**Spawners (Board Objects)**
- Created manually by the game and added to the board
- Spawn items when tapped
- Environment spawners: use no energy and are locked into a board position in the dead zone
- Energy spawners: require energy to spawn

**Items (Board Objects)**
- Created by spawners
- Have numbered levels
- Merge behavior:
  - When dragged together, they "combine" and increment their level
  - Items must be exactly the same to be merged

**Powerups (Board Objects)**
- Dig powerup:
  - Spawn every X time an item is merged (tested with X = 10)
  - When double-tapped, spawn a random high-level item or reward before disappearing
  - Do not move once spawned
  - Visual idea: potentially shovel in a pile of dirt/sand/rocks

 **Energy System**
 - Full amount of energy is 100
 - Decreases by 1 with each spawn

## Core Loop
1. Use energy currency to spawn items
2. Merge items to create new items
3. New items earn rewards (potentially including energy currency indirectly)
4. Wait for refill on energy currency

__image of core loop__










