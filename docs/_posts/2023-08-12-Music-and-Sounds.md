A critical component of any game is sound, and music. It can be difficult to make an engaging game without any sounds, not impossible, just very hard.

As far as I can tell, and the wiki lets on, with [sbox](https://sbox.facepunch.com) we have two types of sounds. Sound events and soundscapes.

### Sound Events

These are used to just play a sound whenever something happens, such as collecting a pickup, dropping an items, running around and so on.

We can add multiple sound files to a sound event file and configure how one would be picked, at random, exclusively random, or sequentially. Even though we specify multiple _source_ sounds in the sound event file (`.sound`), only one of them will be played.

This is to simplify logic so we don't have to keep track of multiple sound files in our source code, pick one at random and so on. This would be necessary if what is provided with [sbox](https://sbox.facepunch.com) is not enough.

Something to keep in mind, sound events can be spatial meaning that you can trigger them from an entity you will be hearing it depending on where you are relative to that entity. We can also play a sound from a position.

The difference is that if we play from an entity the sound "moves" with the entity. An example for this would be footsteps. When we play a sound from a position it will not "move" and we will be able to destroy any entities making this ideal for pickups.

If we do not need a sound to originate from the 3D world, we can set it as a UI sound, with the checkbox set we will be able to play the sound from screen. This works for menus and 2D games.

Unfortunatelly, when I started playing with this there wasn't much documentation, this will most likely change in the future as it is a core component for any game.

To play sounds we need to get the path to the asset, this is easily done through the editor. Simply go to the assets browser and find your sound files, you can create assets from there as well, I would recommend doing that as it will not always pickup on changes made in the file explorer unless you restart the editor.

Directory structure is important for this, I've created an `assets` base direcotry for anything other than code that will be placed there. Under this directory I've created a `sounds` directory and finally, I've made a `mixkit` directory as well as all the sounds I'm currently using are from [mixkit](https://mixkit.co).

Once you have created/found your sound file, right click on it and then click on "Copy Path", this will get the relative path to that asset in your clipboard.

To play a sound event we use the [Sound](https://asset.party/api/Sandbox.Sound) type. It has a number of useful static methods for playing sounds.

* [`FromEntity`](https://asset.party/api/Sandbox.Sound.FromEntity(string,Entity))
* [`FromWorld`](https://asset.party/api/Sandbox.Sound.FromWorld(string,Vector3))
* [`FromScreen`](https://asset.party/api/Sandbox.Sound.FromScreen(string,float,float))

We have overloads for playing a sound only for selected targets so not everyone will hear it.

Keep in mind that we need to provide the path to the sound event asset when calling any of these methods. To simplify this we can declare static classes exposing `const string`s for each sound asset that we use (or any other asset) and use that instead.

```csharp
public static class Sounds
{
    public static class Events
    {
        public const string OrangeCollected = "assets/sounds/mixkit/orange-collected.sound";
    }
}
```

When playing a sound event we use the above class to reference the asset.

```csharp
Sound.FromWorld( Sounds.Events.OrangeCollected, collectedOrange.Position );
```

Keep in mind that when calling [`FromScreen`](https://asset.party/api/Sandbox.Sound.FromScreen(string,float,float)), the related sound event must be set to UI sound, otherwise you will not hear anything.

#### Mixkit

They have a generous catalogue of free sound effects that can be used in games. Keep in mind that their free sound effect license does not allow for the source sound files, or items as they call them, to be redistributed as-is or alongside source.

The good news is that all sound files are compiled by [sbox](https://sbox.facepunch.com) when uploading to [asset.party](https://asset.party) so we don't redistribute the original sound files.

The bad news is that they cannot be part of source code, if the repository is open-source, these files cannot be uploaded.

I've solved this issue with a readme file where I keep a copy of the license and all items used from [mixkit](https://mixkit.co). I was keeping track of this regardless as I want to know what dependencies I have.

Since I have everything listed in the readme file, I've added the `.wav` and `.mp3` files in `.gitignore` so I do not commit them and eventually upload them to GitHub by accident.

I've written a PowerShell script that goes through the readme file and downloads all the dependencies for me from [mixkit](https://mixkit.co). Currenly these are the only ones I have so it's just one script that does the download.

```pwsh
# Read the contents, line by line and then pipe the results
#
# Filter each line so I get only the ones that describe sound dependencies,
# * <sound file name> - <URL for downloading sound file>
#
# Download each dependency in a foreach loop
Get-Content .\readme.md `
| Where-Object { $_ -imatch '^\* .* - .*\.(wav|mp3)$' } `
| ForEach-Object {
     [string] $fileName, [string] $url = $_.Substring(2) -split '\s*-\s*'

     [string] $extension = $url -split '\.' `
     | Select-Object -Last 1

     Invoke-WebRequest -Uri $url -OutFile "${fileName}.$extension"
}
```

This script needs to be run only when the repository is cloned and there are changes to the file, we can use git to check for such changes or just run the script whenever we perform a `git pull`. Since I'm the only one working on this project I don't really need to do any of this.

### Soundscapes

These are intended for ambient sounds or music. Same as sound events, we can configure multiple source sounds, some can be looped and some can be stings.

Looped sounds are exactly what they indicate, they play on a loop. This works well for background noise, such as people walking in a square, or general traffic sound.

Stings are sounds that we hear every now and then, such as birds chirping or some laughter in the background. These are sounds that do not follow a pattern, but still are part of ambiance.

If we were to have a park area we would have the sound of trees and leaves in a loop as the wind is always moving them, while we can have brids chirping every now and then making it seem more natural.

Unlike sound events, soundscapes are played by [`SoundscapeBoxEntity`](https://asset.party/api/Sandbox.SoundscapeBoxEntity) and [`SoundscapeRadiusEntity`](https://asset.party/api/Sandbox.SoundscapeRadiusEntity) when walking into them.

They work a little bit different, although they are intended for ambient sounds, these ambiances are triggered when walking through them. These entities are invisible and can be a bit difficult to place them and know how far they reach.

To debug this check the dropdown from the top right of the game viewer in the editor, we have a Soundscape option. If we select this we will see the soundscape entities and some debug information about the positioning of each such entity.

When creating a soundscape entity we need to specify the path to the soundscape asset, similar to sound events, find the item in the assets browser and copy the path. We can extend the static class with references to assets to include soundscapes as well.

```csharp
var ambianceSoundscape = new SoundscapeRadiusEntity
{
    Soundscape = Sounds.Soundscapes.Ambiance,
    Position = new Vector3( 985f, 2220f, 0f ),
    Radius = 230
};
```

The way this works, from how I understand it, we do not set global ambiance sounds, but rather have entities on key locations that start or switch the ambiance.

Like when entering a room, we would place a soundscape entity at the door and once we go through the ambiance will be changed. When we leave the room we would hit a different soundscape entity which once again changes the ambiance.