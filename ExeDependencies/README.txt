When you build ClassAttempt solution, the generated .exe will be built with its direct .dll references.

That .exe needs some other things in its directory:
- A functioning LvlEditor with its .dlls (you can build it from LvlEditor solution)
- Asset directories contained in this directory:
-- BONUSLEVELS contains bonus levels
-- GAME_RES contains any image used for the Actors
-- LEVELS contains regular playable levels
-- MAIN_RES contains any media used by the UI
-- THEMES contains a rigid asset structure used to generate those themes (their name are hardcoded, you can edit those files but you cannot rename them)

Tl;dr: if you build ClassAttempt.sln, go to Sandbox/bin/Release (or Debug) and paste the content of this directory.