using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeChallengeInc.SubmissionApi.Constants
{
	public class GameRules
	{
		public static string LoneAnt = @"Lone ant is a challenge where the player controls a single ant. Ants search the provided grid for food, and can perform the following actions:

* Moving one position in any of the cardinal or ordinal directions.
* Modifying the color of their current position.
* Reading the color of their current position, or any of the positions the ant could move to.

Using these three actions, players will attempt to design a function for their ant that would allow the ant to get the most food possible.";
	}
}
