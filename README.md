# SpriteGenerator

zmiana tak żeby windows forms projekt użyć na canvasie


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

```json
{

   "FilePath": "C:\\output.png",

   "Places": {

      "7862": {

         "X": 0,

         "Y": 69,

         "Width": 1600,

         "Height": 1000

      },

      "3456": {

         "X": 0,

         "Y": 0,

         "Width": 96,

         "Height": 69

      },

      "9876": {

         "X": 96,

         "Y": 0,

         "Width": 96,

         "Height": 69

      }

   }

}
```
