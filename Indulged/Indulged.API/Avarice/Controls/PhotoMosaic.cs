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
    public class PhotoMosaic : Control
    {
        // Constructor
        public PhotoMosaic()
            : base()
        {
            DefaultStyleKey = typeof(PhotoMosaic);
        }

        // Status
        private bool isAnimating = false;
        public bool IsAnimating
        {
            get
            {
                return isAnimating;
            }
        }

        // Template elements
        private Canvas contentView;

        // Grid coords
        private List<Image> grid = new List<Image>();
        private List<Image> nextGrid = new List<Image>();

        // Data provider
        private List<string> shuffledPhotos = new List<string>();
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

                shuffledPhotos.Clear();
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            contentView = GetTemplateChild("ContentView") as Canvas;

            double w = (ActualWidth > 0) ? ActualWidth : Width;
            double h = (ActualHeight > 0) ? ActualHeight : Height;
            double cellWidth = w / 3;
            double cellHeight = h / 3;

            // Random brightness
            Random randomGenerator = new Random();
            List<double> brighenessArray = new List<double>();
            for(int i = 0; i < 9; i++)
            {
                brighenessArray.Add(randomGenerator.Next(10, 30) / 100.0);
            }

            brighenessArray.Shuffle();

            // Create grid elements (always 9 grid)
            for (int i = 0; i < 9; i++)
            {
                Image cell = new Image();
                cell.SetValue(Canvas.TopProperty, i / 3 * cellHeight);
                cell.SetValue(Canvas.LeftProperty, i % 3 * cellWidth);
                cell.Width = cellWidth;
                cell.Height = cellHeight;

                // Fill the cell with a random color
                double brightness = brighenessArray[i];
                WriteableBitmap backfill = new WriteableBitmap((int)cellWidth, (int)cellHeight);
                Color backfillColor = Color.FromArgb(255, 33, 33, 33);
                backfillColor = backfillColor.ColorWithBrightness(brightness);
                backfill.FillRectangle(0, 0, (int)cellWidth, (int)cellHeight, backfillColor);
                cell.Source = backfill;
                contentView.Children.Add(cell);
                grid.Add(cell);
            }
        }

        public void RandomSelectPhoto()
        {
            animatedCellIndex.Clear();

            if (shuffledPhotos.Count == 0)
            {
                shuffledPhotos = new List<string>(photos);
                shuffledPhotos.Shuffle();
            }

            string photoUrl = shuffledPhotos[0];
            shuffledPhotos.RemoveAt(0);
            int newIndex = photos.IndexOf(photoUrl);

            // Load the image
            Uri uri = new Uri(photoUrl);
            BitmapImage imageLoader = new BitmapImage();
            imageLoader.CreateOptions = BitmapCreateOptions.None;
            imageLoader.ImageOpened += OnImageLoaded;
            imageLoader.UriSource = uri;
            
        }

        private List<int> animatedCellIndex = new List<int>();

        // Image finished loading
        private void OnImageLoaded(object sender, EventArgs e)
        {
            BitmapImage sourceImage = (BitmapImage)sender;
            
            // Slide the image into 9 cells
            WriteableBitmap scaledImage = new WriteableBitmap(sourceImage);
            scaledImage.Resize((int)Width, (int)Height, WriteableBitmapExtensions.Interpolation.Bilinear);

            double w = Width;
            double h = Height;
            double cellWidth = w / 3;
            double cellHeight = h / 3;

            for (int i = 0; i < 9; i++)
            {
                // Create the target cell
                Image cell = new Image();
                Rect rect = new Rect(i % 3 * cellWidth, i / 3 * cellHeight, cellWidth, cellHeight);

                cell.SetValue(Canvas.TopProperty, rect.Y);
                cell.SetValue(Canvas.LeftProperty, rect.X);
                cell.Width = cellWidth;
                cell.Height = cellHeight;
                cell.Visibility = Visibility.Collapsed;

                contentView.Children.Add(cell);
                nextGrid.Add(cell);

                WriteableBitmap slice = scaledImage.Crop(rect);
                cell.Source = slice;
            }

            // Randomly shuffle the cells for animation
            for(int i = 0; i < 9; i++)
            {
                animatedCellIndex.Add(i);
            }

            animatedCellIndex.Shuffle();

            // Do animation
            int firstCellIndex = animatedCellIndex[0];
            PerformFlipAnimation(grid[firstCellIndex], nextGrid[firstCellIndex]);
        }

        private void OnFlipCompleted(object sender, EventArgs e)
        {
            // Flip the next cell
            if (animatedCellIndex.Count > 0)
            {
                int nextCellIndex = animatedCellIndex[0];
                PerformFlipAnimation(grid[nextCellIndex], nextGrid[nextCellIndex]);
            }
            else
            {
                foreach (Image oldImage in grid)
                {
                    contentView.Children.Remove(oldImage);
                }

                grid.Clear();
                foreach (Image newImage in nextGrid)
                {
                    grid.Add(newImage);
                }

                isAnimating = false;
                nextGrid.Clear();
            }
        }

        private void PerformFlipAnimation(Image sourceCell, Image targetCell)
        {
            isAnimating = true;

            if(animatedCellIndex.Count > 0)
                animatedCellIndex.RemoveAt(0);

            sourceCell.Visibility = Visibility.Visible;
            sourceCell.Projection = new PlaneProjection { CenterOfRotationY = 0 };

            targetCell.Visibility = Visibility.Collapsed;
            targetCell.Projection = new PlaneProjection { CenterOfRotationY = 0 };

            var storyboard = new Storyboard();
            var duration = TimeSpan.FromSeconds(0.8);

            storyboard.Children.Add(CreateVisibilityAnimation(duration, sourceCell, false));
            storyboard.Children.Add(CreateVisibilityAnimation(duration, targetCell, true));

            storyboard.Children.Add(CreateRotationAnimation(duration, 0, -90, -180, (PlaneProjection)sourceCell.Projection));
            storyboard.Children.Add(CreateRotationAnimation(duration, 180, 90, 0, (PlaneProjection)targetCell.Projection));

            storyboard.Completed += OnFlipCompleted;
            storyboard.Begin();
        }

        private static DoubleAnimationUsingKeyFrames CreateRotationAnimation(TimeSpan duration, double degreesFrom, double degreesMid, double degreesTo, PlaneProjection projection)
        {
            var _One = new EasingDoubleKeyFrame { KeyTime = new TimeSpan(0), Value = degreesFrom, EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseIn } };
            var _Two = new EasingDoubleKeyFrame { KeyTime = new TimeSpan(duration.Ticks / 2), Value = degreesMid, EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseIn } };
            var _Three = new EasingDoubleKeyFrame { KeyTime = new TimeSpan(duration.Ticks), Value = degreesTo, EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut } };

            var _Animation = new DoubleAnimationUsingKeyFrames { BeginTime = new TimeSpan(0) };
            _Animation.KeyFrames.Add(_One);
            _Animation.KeyFrames.Add(_Two);
            _Animation.KeyFrames.Add(_Three);
            Storyboard.SetTargetProperty(_Animation, new PropertyPath("RotationY"));
            Storyboard.SetTarget(_Animation, projection);
            return _Animation;
        }

        private static ObjectAnimationUsingKeyFrames CreateVisibilityAnimation(Duration duration, UIElement element, bool show)
        {
            var _One = new DiscreteObjectKeyFrame { KeyTime = new TimeSpan(0), Value = (show ? Visibility.Collapsed : Visibility.Visible) };
            var _Two = new DiscreteObjectKeyFrame { KeyTime = new TimeSpan(duration.TimeSpan.Ticks / 2), Value = (show ? Visibility.Visible : Visibility.Collapsed) };

            var _Animation = new ObjectAnimationUsingKeyFrames { BeginTime = new TimeSpan(0) };
            _Animation.KeyFrames.Add(_One);
            _Animation.KeyFrames.Add(_Two);
            Storyboard.SetTargetProperty(_Animation, new PropertyPath("Visibility"));
            Storyboard.SetTarget(_Animation, element);
            return _Animation;
        }

    }
}
