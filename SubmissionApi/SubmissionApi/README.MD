# Code Challenge Submission API

This API is for submitting code challenge functions. Eventually we will be able to support submissions for multiple challenges, but initially we have the `Lone Ant` challenge!

## Lone Ant

### Rules

Lone ant is a challenge where the player controls a single ant. Ants search the provided grid for food, and can perform the following actions:

* Moving one position in any of the cardinal or ordinal directions.
* Modifying the color of their current position.
* Reading the color of their current position, or any of the positions the ant could move to.

Using these three actions, players will attempt to design a function for their ant that would allow the ant to get the most food possible.

### Endpoints

#### GET SubmissionApi/LoneAnt

Get the rules for the code challenge. No parameters. Should always return 200 OK with the rules of the game.

#### GET SubmissionApi/LoneAnt/{Username}

Get the most recent submission for the given username. Returns 404 if no username is found.

#### GET Response Example

```
{
    "Username": "BoGoAntMaster1337",
    "Submission": "switch(view[0].color){
    case 1:
        return {cell:0, color: view[1].color+1}
    case 2:
        return {cell:1}
    case 3:
        return {cell:2}
    case 4:
        return {cell:3}
    case 5:
        return {cell:4}
    case 6:
        return {cell:5}
    case 7:
        return {cell:6}
    case 8:
        return {cell:7}
    }"
}
```

##### GET Player Parameters

Username (string, Required)

#### PUT SubmissionApi/LoneAnt/{Username}

Submit a new function for the provided username. Submissions will be manually evaluated prior to being added to a Game to ensure no malicious behavior is present. Returns a 200 OK on success, which should almost always be the case. Will fail if the Username contains [illegal characters for a Linux filesystem](https://en.wikipedia.org/wiki/Filename#Reserved_characters_and_words).

The actual submission should be in request body, as text.

Subsequent POST actions will overwrite the previous submission, but a backup of the previous submission will be made.

##### PUT Player Parameters

Username (string, Required)

##### PUT Request Example

GET request to SubmissionApi/LoneAnt/BoGoAntMaster1337
BODY:
```
"switch(view[0].color){
    case 1:
        return {cell:0, color: view[1].color+1}
    case 2:
        return {cell:1}
    case 3:
        return {cell:2}
    case 4:
        return {cell:3}
    case 5:
        return {cell:4}
    case 6:
        return {cell:5}
    case 7:
        return {cell:6}
    case 8:
        return {cell:7}
}"
```

#### DELETE SubmissionApi/LoneAnt/{Username}

Remove the submission for the provided username. Returns a 200 OK if a file is found and removed. Otherwise will return a 404.

All DELETE actions are a soft delete, and deleted submissions will be able to be manually recovered by your friendly game administrators.

##### DELETE Player Parameters

Username (string, Required)
