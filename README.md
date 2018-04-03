# kula-world-2d-dot-net

This is an old project I made for the final term of my Interface Programming. I tried to implement a 2D version of a PS1 puzzle game called "Kula World"/"Kula Quest"/"Roll Away": this implementation is still very raw but I managed to make it run in order to play it a little bit.

I'm currently trying to make the whole thing public without leaving any important file outside of this repository. I need to write some information about this messy code when I have the time and, maybe, I'll clean it a little bit.

This project is done with C#/.NET (I've targetted .NET 4.5 but I think I didn't use any feature of that version).
Drawing was implemented using Windows Forms while sound was done by using WMPLib (WindowsMediaPlayer Library) in order to play MP3 files instead of uncompressed WAV and reduce assets' total size.

This project consists of two VS Solutions:
- ClassAttempt (this is a legacy stub-name) contains all the game logic and "engine" implementation
- LvlEditor is the GUI level editor used to quickly create levels to test and make the game actually playable
