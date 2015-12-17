# SpriteGenerator

This is fork of [http://spritegenerator.codeplex.com/](http://spritegenerator.codeplex.com/).
Changes:
- C# library instead of  WindowsForms project
- save png file but css file generation was deleted
- list of coordinates on sprite is returned

Basic usage:
```cs
var sprite = new Sprite(new LayoutProperties
{
    InputFilePaths = new List<InputFile>
    {
        new InputFile { Id = 3456, Path = @"C:\imgs_to_join\a.jpg" },
        new InputFile { Id = 7862, Path = @"C:\imgs_to_join\b.png" },
        new InputFile { Id = 9876, Path = @"C:\imgs_to_join\c.jpg" },
    },
    OutputSpriteFilePath = @"C:\output.png"

});
var output = sprite.Create();
```

Output:
```json
{
   "FilePath": "C:\\output.png",
   "Places": {
      "7862": {
         "X": 0, "Y": 69, "Width": 1600, "Height": 1000
      },
      "3456": {
         "X": 0, "Y": 0, "Width": 96, "Height": 69
      },
      "9876": {
         "X": 96, "Y": 0, "Width": 96, "Height": 69
      }
   }
}
```
