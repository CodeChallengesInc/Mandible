﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeChallengeInc.SubmissionApi.Constants
{
	public class GameRules
	{
		public static string LoneAnt = @"# Rules

Lone ant is a challenge where the player controls a single ant. Ants search the provided grid for food.

## Ant Abilities

Each submission represents a single ant that will attempt to gather food on the 2500x1000 grid. Ants have the following abilities:

* Sight: Each ant can see their current square, and each square 1 space adjacent or diagnol to it. This gives the ant a total vision of 9 squares in a 3x3 grid.
* No Memory: An ant makes their decision each turn based on the section of the grid they can currently see. Ants have notoriously short memories, and can't remember the action they took on previous turns. 
* No Orientation: Due to their poor memories, ants also have no sense of orientation. The 3x3 grid that the ant is able to see will be rotated randomly, so they will need to rely on their other abilities to orient themselves each turn.
* Moving: Ants can move to any square they are able to see.
* Coloring Squares: Ants can also color any square they are able to move to, allowing them to create paths for the purposes of orientation.
* Immortality: Due to Newton's fourth law of motion, the Conservation of Ants, ants can neither be created nor destroyed. no PvP action!

## Coding Rules

### Provide a function body

Each ant is controlled by a user-submitted Javascript function. Each turn, for each ant, the player's ant function is called. The function has an input of the 3x3 grid your ant is able to see, and should return a move for your ant's turn.

### No state, No time, No random

A submitted function must not attempt to use global variables, or any other means of storing state between turns. It may use built-in functions that don't involve storing state, so using Math.abs() would be fine but Date.getTime() is forbidden.

An ant function may only use a pseudo random number generator that it supplies itself, which doesn't store state. Due to this, Math.random() is explictly forbidden, as it relies on storing state to generate the next random number. 

Instead, players may use the information they receive each turn to psuedorandomly determine their actions. As orientation of the input value is randomized each turn, simple random strategies are still possible.

An ant function is permitted to contain additional helper-functions within its body.

## Input

The orientation of the input will be chosen at random for each ant and for each turn. The input will be rotated by 0, 90, 180 or 270 degrees, but will never be reflected.

Cells are ordered in the following way:
```
0 1 2
3 4 5
6 7 8
```

The ant function will receive an array called `view`, which contains an object for each of the visible cells, with the following information:

````
color: a number from 1-8
food: 1 if present, 0 if not.
````
## Output

Output is returned as an object representing the action to take. This object should have the following information:
```
cell: a number from 0-8, the cell to act on. (mandatory)
color: a number from 1-8. (optional)
```

If `color` is omitted or zero, then `cell` indicates the to move to.

if `color` is non-zero, the indicated cell is set to that color.

Example outputs:
```
{cell:0}: move to cell 0
{cell:4}: move to cell 4 (that is, do nothing, as 4 is the central cell)
{cell:4, color:8}: set own cell to color 8
{cell:6, color:1}: set cell 6 to color 1
{cell:6, color:0}: equivalent to just `{cell:6}` - move rather than set color
```
Invalid Outputs:
```
{cell: 9}: cell must be between 0-8.
{cell0, color:9}: color must be from 1-8.
```
## Closing Notes.

Each user is allowed one submission. At this stage we don't have account creation/validation, so this is enforced based on a provided username variable at this stage. In the future this will be derived from a JWT token.";
	}
}
