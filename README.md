The Orange Run
--------------

A small platformer-like game where you have to collect oranges. The player with most oranges collected at the end of the game wins.

MVP
---

* Add some orange spawners for testing, the used map is [`facepunch.square`](https://asset.party/facepunch/square)
* Game loop
  1. Lobby for players to gather (1 or 2 minutes)
  2. Play time, players run around gathering oranges
     * Oranges are dropped at some crates
     * There is a cap for how many oranges one player can carry at a time
     * Falling off the map will respawn the player, all oranges that they are carrying are discarded and 3 or 5 oranges are taken from what they collected. If they don't have enough oranges to respawn they can fall until the game is over
     * The more oranges you carry the slower you become
     * Oranges respawn after a certain time limit (2-3 minutes)
  3. Score board, players are shown the stats at the end the of the game as well as the winner
* Sounds are added to the game
  * Ambient sound
  * Sound when the orange run starts
  * Sound when the game nears its end (time pressure)
  * Sound when the game ends
  * Sound when collecting an orange
  * Sound when offloading oranges
