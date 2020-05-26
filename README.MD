# Code Challenge Submission API

This API is for submitting code challenge functions. Eventually we will be able to support submissions for multiple challenges, but initially we have the `Lone Ant` challenge!

## Lone Ant

### Rules

Lone ant is a challenge where the player controls a single ant. Ants search the provided grid for food, and can perform the following actions:

* Moving one position in any of the cardinal or ordinal directions.
* Modifying the color of their current position.
* Reading the color of their current position, or any of the positions the ant could move to.

Using these three actions, players will attempt to design a function for their ant that would allow the ant to get the most food possible.

Each user is allowed one submission. At this stage we don't have account creation/validation, so this is enforced based on a provided username variable at this stage. In the future this will be derived from a JWT token.

### Endpoints

#### GET SubmissionApi/LoneAnt

Get the rules for the code challenge. No parameters. Should always return 200 OK with the rules of the game.

#### GET SubmissionApi/LoneAnt/Player?{Username}

Get the most recent submission for the given username. Returns 404 if no username is found.

##### GET Player Parameters

Username (string, Required)

#### POST SubmissionApi/LoneAnt/Player?{Username}

Submit a new function for the provided username. Submissions will be manually evaluated prior to being added to a Game to ensure no malicious behavior is present. Returns a 200 OK on success, which should almost always be the case. Will fail if the Username contains [illegal characters for a Linux filesystem](https://en.wikipedia.org/wiki/Filename#Reserved_characters_and_words).

Subsequent POST actions will overwrite the previous submission, but a backup of the previous submission will be made.

##### POST Player Parameters

Username (string, Required)

#### DELETE SubmissionApi/LoneAnt/Player?{Username}

Remove the submission for the provided username. Returns a 200 OK if a file is found and removed. Otherwise will return a 404.

All DELETE actions are a soft delete, and deleted submissions will be able to be manually recovered by your friendly game administrators.

##### DELETE Player Parameters

Username (string, Required)
