using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using SpriteGenerator.Utility;

namespace SpriteGenerator
{
    public class Sprite
    {
        private Dictionary<int, Image> _images;
        private Dictionary<int, int> _inputFilesIds;
        private LayoutProperties _layoutProp;
        private SpriteModel _sprite;

        public Sprite(LayoutProperties layoutProp)
        {
            _layoutProp = layoutProp;
            _sprite = new SpriteModel();
            _sprite.Places = new Dictionary<int, SpritePlace>();
        }

        public SpriteModel Create()
        {
            GetData(out _images, out _inputFilesIds);

            Image resultSprite = null;

            _sprite.FilePath = _layoutProp.OutputSpriteFilePath;

            switch (_layoutProp.LayoutMode)
            {
                case LayoutMode.Automatic:
                    resultSprite = generateAutomaticLayout();
                    break;
                case LayoutMode.Horizontal:
                    resultSprite = generateHorizontalLayout();
                    break;
                case LayoutMode.Vertical:
                    resultSprite = generateVerticalLayout();
                    break;
                case LayoutMode.Rectangular:
                    resultSprite = generateRectangularLayout();
                    break;
                default:
                    break;
            }

            FileStream outputSpriteFile = new FileStream(_layoutProp.OutputSpriteFilePath, FileMode.Create);
            resultSprite.Save(outputSpriteFile, ImageFormat.Png);
            outputSpriteFile.Close();

            return _sprite;
        }

        /// <summary>
        /// Creates dictionary of images from the given paths and dictionary of CSS classnames from the image filenames.
        /// </summary> 
        /// <param name="inputFilePaths">Array of input file paths.</param>
        /// <param name="images">Dictionary of images to be inserted into the output sprite.</param>
        /// <param name="cssClassNames">Dictionary of CSS classnames.</param>
        private void GetData(out Dictionary<int, Image> images, out Dictionary<int, int> ids)
        {
            images = new Dictionary<int, Image>();
            ids = new Dictionary<int, int>();

            for (int i = 0; i < _layoutProp.InputFilePaths.Count; i++)
            {
                var inputFile = _layoutProp.InputFilePaths[i];
                Image img = Image.FromFile(inputFile.Path);
                images.Add(i, img);
                ids.Add(i, inputFile.Id);
            }
        }

        private List<Module> CreateModules()
        {
            List<Module> modules = new List<Module>();
            foreach (int i in _images.Keys)
                modules.Add(new Module(i, _images[i], _layoutProp.DistanceBetweenImages));
            return modules;
        }

        private void AddSpritePlace(int id, Rectangle rectangle)
        {
            _sprite.Places.Add(id, new SpritePlace
            {
                Width = rectangle.Width,
                Height = rectangle.Height,
                X = rectangle.X,
                Y = rectangle.Y
            });
        }

        //Relative sprite image file path
        private string relativeSpriteImagePath(string outputSpriteFilePath, string outputCssFilePath)
        {
            string[] splittedOutputCssFilePath = outputCssFilePath.Split('\\');
            string[] splittedOutputSpriteFilePath = outputSpriteFilePath.Split('\\');

            int breakAt = 0;
            for (int i = 0; i < splittedOutputCssFilePath.Length; i++)
                if (i < splittedOutputSpriteFilePath.Length && splittedOutputCssFilePath[i] != splittedOutputSpriteFilePath[i])
                {
                    breakAt = i;
                    break;
                }

            string relativePath = "";
            for (int i = 0; i < splittedOutputCssFilePath.Length - breakAt - 1; i++)
                relativePath += "../";
            relativePath += String.Join("/", splittedOutputSpriteFilePath, breakAt, splittedOutputSpriteFilePath.Length - breakAt);

            return relativePath;
        }

        //Automatic layout
        private Image generateAutomaticLayout()
        {
            var sortedByArea = from m in CreateModules()
                               orderby m.Width * m.Height descending
                               select m;
            List<Module> moduleList = sortedByArea.ToList<Module>();
            Placement placement = Algorithm.Greedy(moduleList);

            //Creating an empty result image.
            Image resultSprite = new Bitmap(placement.Width - _layoutProp.DistanceBetweenImages + 2 * _layoutProp.MarginWidth,
                placement.Height - _layoutProp.DistanceBetweenImages + 2 * _layoutProp.MarginWidth);
            Graphics graphics = Graphics.FromImage(resultSprite);
            
            //Drawing images into the result image in the original order and writing CSS lines.
            foreach (Module m in placement.Modules)
            {
                m.Draw(graphics, _layoutProp.MarginWidth);
                Rectangle rectangle = new Rectangle(m.X + _layoutProp.MarginWidth, m.Y + _layoutProp.MarginWidth,
                    m.Width - _layoutProp.DistanceBetweenImages, m.Height - _layoutProp.DistanceBetweenImages);

                int id = _inputFilesIds[m.Index];
                AddSpritePlace(id, rectangle);
            }

            return resultSprite;
        }

        //Horizontal layout
        private Image generateHorizontalLayout()
        {
            //Calculating result image dimension.
            int width = 0;
            foreach (Image image in _images.Values)
                width += image.Width + _layoutProp.DistanceBetweenImages;
            width = width - _layoutProp.DistanceBetweenImages + 2 * _layoutProp.MarginWidth;
            int height = _images[0].Height + 2 * _layoutProp.MarginWidth;

            //Creating an empty result image.
            Image resultSprite = new Bitmap(width, height);
            Graphics graphics = Graphics.FromImage(resultSprite);
            
            //Initial coordinates.
            int actualXCoordinate = _layoutProp.MarginWidth;
            int yCoordinate = _layoutProp.MarginWidth;

            //Drawing images into the result image, writing CSS lines and increasing X coordinate.
            foreach(int i in _images.Keys)
            {
                Rectangle rectangle = new Rectangle(actualXCoordinate, yCoordinate, _images[i].Width, _images[i].Height);
                graphics.DrawImage(_images[i], rectangle);
                AddSpritePlace(_inputFilesIds[i], rectangle);
                actualXCoordinate += _images[i].Width + _layoutProp.DistanceBetweenImages;
            }

            return resultSprite;
        }

        //Vertical layout
        private Image generateVerticalLayout()
        {
            //Calculating result image dimension.
            int height = 0;
            foreach (Image image in _images.Values)
                height += image.Height + _layoutProp.DistanceBetweenImages;
            height = height - _layoutProp.DistanceBetweenImages + 2 * _layoutProp.MarginWidth;
            int width = _images[0].Width + 2 * _layoutProp.MarginWidth;

            //Creating an empty result image.
            Image resultSprite = new Bitmap(width, height);
            Graphics graphics = Graphics.FromImage(resultSprite);
            
            //Initial coordinates.
            int actualYCoordinate = _layoutProp.MarginWidth;
            int xCoordinate = _layoutProp.MarginWidth;

            //Drawing images into the result image, writing CSS lines and increasing Y coordinate.
            foreach (int i in _images.Keys)
            {
                Rectangle rectangle = new Rectangle(xCoordinate, actualYCoordinate, _images[i].Width, _images[i].Height);
                graphics.DrawImage(_images[i], rectangle);
                AddSpritePlace(_inputFilesIds[i], rectangle);
                actualYCoordinate += _images[i].Height + _layoutProp.DistanceBetweenImages;
            }

            return resultSprite;
        }

        private Image generateRectangularLayout()
        {
            //Calculating result image dimension.
            int imageWidth = _images[0].Width;
            int imageHeight = _images[0].Height;
            int width = _layoutProp.ImagesInRow * (imageWidth + _layoutProp.DistanceBetweenImages) -
                _layoutProp.DistanceBetweenImages + 2 * _layoutProp.MarginWidth;
            int height = _layoutProp.ImagesInColumn * (imageHeight + _layoutProp.DistanceBetweenImages) -
                _layoutProp.DistanceBetweenImages + 2 * _layoutProp.MarginWidth;

            //Creating an empty result image.
            Image resultSprite = new Bitmap(width, height);
            Graphics graphics = Graphics.FromImage(resultSprite);

            //Initial coordinates.
            int actualYCoordinate = _layoutProp.MarginWidth;
            int actualXCoordinate = _layoutProp.MarginWidth;

            //Drawing images into the result image, writing CSS lines and increasing coordinates.
            for (int i = 0; i < _layoutProp.ImagesInColumn; i++)
            {
                for (int j = 0; (i * _layoutProp.ImagesInRow) + j < _images.Count && j < _layoutProp.ImagesInRow; j++)
                {
                    Rectangle rectangle = new Rectangle(actualXCoordinate, actualYCoordinate, imageWidth, imageHeight);
                    graphics.DrawImage(_images[i * _layoutProp.ImagesInRow + j], rectangle);
                    AddSpritePlace(_inputFilesIds[i * _layoutProp.ImagesInRow + j], rectangle);
                    actualXCoordinate += imageWidth + _layoutProp.DistanceBetweenImages;
                }
                actualYCoordinate += imageHeight + _layoutProp.DistanceBetweenImages;
                actualXCoordinate = _layoutProp.MarginWidth;
            }

            return resultSprite;
        }
    }
}
