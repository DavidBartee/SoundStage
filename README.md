# SoundStage
A Database-driven, .NET WPF application that allows you to use your own sound files to create a custom soundboard or music player. Features include:

* Add sounds via click and drag or the open file dialogue
* Create bindings that play sounds using keyboard shortcuts, even when the program is in the background
* Create on-screen buttons to play sounds when they are clicked (useful for touchscreens)
* Put sounds or music files in a playlist that updates in real time
* Create, Edit, and Delete listed sounds and bindings

SoundStage supports .mp3 and .wav files and validates them using the first few bytes at the beginning of the files to identify the filetype. If a filetype is not supported, the program will not import it.
