# Server-Multi Client game for MasterMind. #
## Console Server and Windows forms Client application. ##
### Developped for my practical examination in Information Processing ###

This is a 3-part project that contains:
1. A Windows forms client application for the classic MasterMind game which sends points earned to Server if game over/completed.
2. A Server that for now, sends an ackonowledge message when client is connected and
3. A Bindings project containing network packet definitions and interpretations

Sources for this project:
[Mastermind](https://github.com/reyesreg/mastermind) by Reg Reyes
[Server/Client Solution for Visual Studio and Unity](https://www.youtube.com/watch?v=FY1QLjj2nwY) (tutorial) by Kevin Kaymak
AroundThisWorld an example Client-Server application made available by my professor D. Grobet

### How to play: ###

* The player has 6 tries to correctly guess the sequence of 5 colors. There is no repition of colours and no black spaces permitted.
* User can select any oval in curent row  and select colour sequence. When the row is filled, 5 different colours selected, user clicks _SCORE_ or evaluation 
  * Black means right color _and_ right position
  * White mean right but _wrong_ positon
  * Gray means wrong color.
* If the player guesses the 5 colors correctly, user is asked if he wants to restart and gets points +200 points for winning plus bonus for level (out of 6) game was finished.
* Client will send to server the total tally for each client that completes/finishes a game.
* If the player does not guess the colour sequence correctly, the game will reveal the colors at the end of the 6 turns.

