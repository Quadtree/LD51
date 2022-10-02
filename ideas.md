# Theme: Every 10 Seconds
Seriously? This is just "10 Seconds" with "Every" added!

- Tower defense game where waves spawn every 10 seconds
- Text based adventure where you need to make a decision every 10 seconds
- Any game where waves come every 10 seconds
- Cook serve delicious type game where customers arrive every 10 sec

## RoboKitchen
- Carts enter every 10 seconds with orders
- Every order has a recipe with 2 different types of actions
  - Collect ingredient
  - Use processing station (Chop or Cook)
- So for example Toasted Sandwich is:
  - Collect lettuice
  - Chop
  - Collect bread
  - Collect protein
  - Cook

The player places stations. Carts can't travel through stations. Carts decide on their path when they first appear and cannot deviate from it. If a station is placed in the way of a cart, it will crash and be destroyed. That order is lost.

The player gets credit for the time it takes orders to complete. Each order probably has like 30 seconds. So the score is 30 - Time.

### Stations
- Lettuce
- Bread
- Protein
- Water
- Tomato
- Cook
- Chop
- Controller (Free turns when adjacent)
- Accelerator (Speeds up _everything_)

### DONE
- Placing buildings
- Getting money over time, buildings cost money
- More recipies
- Cart/cart collision
- Display cooldown of buildings
- Carts disappear when they reach the end
- Final UI

### TODO
- Instantly fail if distance field is empty
- Figure out lighting issue
- Figure out UI freeze issue
- Multiple levels
- Final graphics
- Sound
- Title screen
- Win screen
- Lose screen
- Music
