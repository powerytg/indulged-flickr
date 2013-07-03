using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;

using Indulged.API.Utils;

namespace Indulged.API.Avarice.Controls
{
    public class PhotoGrid : Control
    {
        // Status
        private bool isAnimating = false;
        public bool IsAnimating
        {
            get
            {
                return isAnimating;
            }
        }

        // Current selected image index
        private int currentIndex = 0;

        // Grid definiations
        private int numRows = 0;
        private int numCols = 0;

        // Constructor
        public PhotoGrid()
            : base()
        {
            DefaultStyleKey = typeof(PhotoGrid);
        }

        private Canvas contentView;

        // Images
        private List<Image> imageViews = new List<Image>();

        // Grid coords
        private List<Rect> coords = new List<Rect>();

        // Data provider
        private List<string> photos = new List<string>();
        public List<string> Photos
        {
            get
            {
                return photos;
            }

            set
            {
                photos = value;

                // Clear all sub views
                imageViews.Clear();
                shuffledPhotos.Clear();

                if (contentView != null)
                    contentView.Children.Clear();
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            contentView = GetTemplateChild("ContentView") as Canvas;
        }

        private void GenerateGrid()
        {
            if (Width == 0 || Width == 0)
                return;

            // Calculate the grid
            coords.Clear();
            numCols = (int)Math.Ceiling(Math.Sqrt(photos.Count));
            numRows = photos.Count / numCols;
            double w = Width;
            double h = Height;

            double nextX = 0;
            double nextY = 0;
            double cellWidth = w / numCols;
            double cellHeight = h / numRows;
            for (int i = 0; i < photos.Count; i++)
            {
                if (nextX + cellWidth > w)
                {
                    nextX = 0;
                    nextY += cellHeight;
                }

                Rect rect = new Rect(nextX, nextY, cellWidth, cellHeight);
                coords.Add(rect);

                nextX += cellWidth;
            }

            // Place images. If images are not there, then create them
            if (imageViews.Count == 0)
            {
                foreach (string url in photos)
                {
                    Image image = new Image();
                    image.Source = new BitmapImage(new Uri(url));

                    imageViews.Add(image);
                    contentView.Children.Add(image);
                }
            }

            // Set clipping
            contentView.Clip = new RectangleGeometry() { Rect = new Rect(0, 0, w, h) };

        }

        protected override System.Windows.Size ArrangeOverride(System.Windows.Size finalSize)
        {
            double w = Width;
            double h = Height;

            if(photos.Count == 0 || isAnimating)
                return base.ArrangeOverride(finalSize);

            if (imageViews.Count == 0)
                GenerateGrid();

                for (int i = 0; i < photos.Count; i++)
                {
                    Image image = imageViews[i];
                    image.Stretch = Stretch.UniformToFill;
                    Rect rect = new Rect(0, 0, w, h);
                    image.Width = rect.Width;
                    image.Height = rect.Height;
                    image.SetValue(Canvas.LeftProperty, rect.X);
                    image.SetValue(Canvas.TopProperty, rect.Y);
                }

            return base.ArrangeOverride(finalSize);

        }

        private List<string> shuffledPhotos = new List<string>();

        // Randomly lookat the next photo. Photo may or may not be next to each other
        public void RandomSelectPhoto()
        {
            double w = Width;
            double h = Height;
            isAnimating = true;

            if (shuffledPhotos.Count == 0)
            {
                shuffledPhotos = new List<string>(photos);
                shuffledPhotos.Shuffle();
            }

            string photoUrl = shuffledPhotos[0];
            shuffledPhotos.RemoveAt(0);
            int newIndex = photos.IndexOf(photoUrl);
            Rect targetRect = coords[newIndex];
            int targetColId = newIndex / numRows;
            int targetRowId = newIndex / numCols;


            // Transform into grid mode
            Image currentImage = imageViews[currentIndex];

            TimeSpan startTime = TimeSpan.FromSeconds(0);
            TimeSpan freezeStartTime = TimeSpan.FromSeconds(1);
            TimeSpan freezeEndTime = TimeSpan.FromSeconds(1.6);
            TimeSpan endTime = TimeSpan.FromSeconds(3);

            Storyboard currentStoryboard = new Storyboard();
            currentStoryboard.Duration = new Duration(endTime);

            double offsetX = targetRect.X;
            double offsetY = targetRect.Y;

            foreach (Image image in imageViews)
            {
                int i = imageViews.IndexOf(image);
                int colId = i / numRows;
                int rowId = i / numCols;

                Rect rect = coords[i];

                // X animation
                var xAnimation = new DoubleAnimationUsingKeyFrames();
                currentStoryboard.Children.Add(xAnimation);
                Storyboard.SetTarget(xAnimation, image);
                Storyboard.SetTargetProperty(xAnimation, new PropertyPath("(Canvas.Left)"));
                xAnimation.KeyFrames.Add(new EasingDoubleKeyFrame
                {
                    KeyTime = KeyTime.FromTimeSpan(freezeStartTime),
                    Value = rect.X,
                    EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseIn }
                });
                
                xAnimation.KeyFrames.Add(new EasingDoubleKeyFrame
                {
                    KeyTime = KeyTime.FromTimeSpan(freezeEndTime),
                    Value = rect.X,
                });

                xAnimation.KeyFrames.Add(new EasingDoubleKeyFrame
                {
                    KeyTime = KeyTime.FromTimeSpan(endTime),
                    Value = (colId - targetColId) * w,
                    EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseIn }
                });
                

                // Y animation
                var yAnimation = new DoubleAnimationUsingKeyFrames();
                currentStoryboard.Children.Add(yAnimation);
                Storyboard.SetTarget(yAnimation, image);
                Storyboard.SetTargetProperty(yAnimation, new PropertyPath("(Canvas.Top)"));
                yAnimation.KeyFrames.Add(new EasingDoubleKeyFrame
                {
                    KeyTime = KeyTime.FromTimeSpan(freezeStartTime),
                    Value = rect.Y,
                    EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseIn }
                });

                yAnimation.KeyFrames.Add(new EasingDoubleKeyFrame
                {
                    KeyTime = KeyTime.FromTimeSpan(freezeEndTime),
                    Value = rect.Y,
                });

                yAnimation.KeyFrames.Add(new EasingDoubleKeyFrame
                {
                    KeyTime = KeyTime.FromTimeSpan(endTime),
                    Value = (rowId - targetRowId) * h,
                    EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseIn }
                });
                
                // Width animation
                var widthAnimation = new DoubleAnimationUsingKeyFrames();
                currentStoryboard.Children.Add(widthAnimation);
                Storyboard.SetTarget(widthAnimation, image);
                Storyboard.SetTargetProperty(widthAnimation, new PropertyPath("Width"));
                widthAnimation.KeyFrames.Add(new EasingDoubleKeyFrame
                {
                    KeyTime = KeyTime.FromTimeSpan(freezeStartTime),
                    Value = rect.Width,
                    EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseIn }
                });

                widthAnimation.KeyFrames.Add(new EasingDoubleKeyFrame
                {
                    KeyTime = KeyTime.FromTimeSpan(freezeEndTime),
                    Value = rect.Width,
                });

                widthAnimation.KeyFrames.Add(new EasingDoubleKeyFrame
                {
                    KeyTime = KeyTime.FromTimeSpan(endTime),
                    Value = w,
                    EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseIn }
                });
                
                // Height animation
                var heightAnimation = new DoubleAnimationUsingKeyFrames();
                currentStoryboard.Children.Add(heightAnimation);
                Storyboard.SetTarget(heightAnimation, image);
                Storyboard.SetTargetProperty(heightAnimation, new PropertyPath("Height"));
                heightAnimation.KeyFrames.Add(new EasingDoubleKeyFrame
                {
                    KeyTime = KeyTime.FromTimeSpan(freezeStartTime),
                    Value = rect.Height,
                    EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseIn }
                });

                heightAnimation.KeyFrames.Add(new EasingDoubleKeyFrame
                {
                    KeyTime = KeyTime.FromTimeSpan(freezeEndTime),
                    Value = rect.Height,
                });

                heightAnimation.KeyFrames.Add(new EasingDoubleKeyFrame
                {
                    KeyTime = KeyTime.FromTimeSpan(endTime),
                    Value = h,
                    EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseIn }
                });
                
            }

            currentStoryboard.Begin();
            currentStoryboard.Completed += (sender, e) =>
                {
                    isAnimating = false;
                    currentIndex = newIndex;
                };
        }

    }
}
