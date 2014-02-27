using Indulged.API.Avarice.Controls;
using Indulged.API.Avarice.Events;
using Indulged.Plugins.ProFX.Events;
using Indulged.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Indulged.Plugins.ProFX
{
    public partial class ProFXPage
    {
        private void InitializeEventListeneres()
        {
            filterManager.InvalidatePreview += OnPreviewInvalidated;
            filterManager.FilterAdded += OnFilterAdded;
            filterManager.FilterCountChanged += OnFilterCountChanged;

            FilterGalleryView.OnDismiss += OnFilterGalleryDismiss;
            FilterGalleryView.RequestFilter += OnFilterRequested;

            ActiveFilterView.OnDismiss += OnActiveFilterViewDismiss;
            ActiveFilterView.RequestFilter += OnFilterRequested;
            ActiveFilterView.RequestCropFilter += OnCropFilterRequested;
            ActiveFilterView.RequestRotationFilter += OnRotationFilterRequested;
            ActiveFilterView.RequestFilterGallery += OnRequestFilterGalleryFromFilterList;

            FilterContainerView.OnDismiss += OnFilterContainerDismiss;
            FilterContainerView.OnDelete += OnFilterDeleted;

            ViewFinder.CropAreaChanged += OnCropAreaChanged;
            CropView.OnDismiss += OnCropFilterDismiss;
            CropView.OnDelete += OnCropFilterDelete;

            RotationView.OnDismiss += OnRotationFilterDismiss;
            RotationView.OnDelete += OnRotationFilterDelete;
            RotationView.ValueChanged += OnRotationValueChanged;

            UploaderPage.RequestDismiss += OnUploaderRequestDismiss;
            UploaderPage.RequestExit += OnUploaderRequestExit;
        }

        private void OnPreviewInvalidated(object sender, EventArgs e)
        {
            UpdatePreviewAsync();
        }

        private void OnProcessorButtonClick(object sender, RoutedEventArgs e)
        {
            ShowFilterGallery();
        }

        private void OnAddFilterButtonClick(object sender, RoutedEventArgs e)
        {
            ShowFilterGallery();
        }

        private void OnFilterGalleryDismiss(object sender, EventArgs e)
        {
            DismissFilterGallery(true);
        }

        private void OnActiveFilterViewDismiss(object sender, EventArgs e)
        {
            DismissActiveFilterList();
        }

        private void OnRequestFilterGalleryFromFilterList(object sender, EventArgs e)
        {
            DismissActiveFilterList(false, () => {
                ShowFilterGallery();
            });
        }

        private void OnFilterContainerDismiss(object sender, EventArgs e)
        {
            DismissFilterOSD();
        }

        private void OnCropFilterRequested(object sender, EventArgs e)
        {
            DismissActiveFilterList(false, () => {
                OnCropButtonClick(this, null);
            });            
        }

        private void OnRotationFilterRequested(object sender, EventArgs e)
        {
            DismissActiveFilterList(false, () =>
            {
                OnRotationButtonClick(this, null);
            });
        }

        private void OnFilterRequested(object sender, RequestFilterEventArgs e)
        {
            if (FilterGalleryView.Visibility != Visibility.Collapsed)
            {
                DismissFilterGallery(false, () =>
                {
                    ShowFilterOSD(e.Filter);
                });
            }
            else if (ActiveFilterView.Visibility != Visibility.Collapsed)
            {
                DismissActiveFilterList(false, () =>
                {
                    ShowFilterOSD(e.Filter);
                });
            }
            else
            {
                ShowFilterOSD(e.Filter);
            }
        }

        private void OnFilterAdded(object sender, AddFilterEventArgs e)
        {
            e.Filter.OriginalImage = originalBitmap;
            e.Filter.CurrentImage = currentPreviewBitmap;
            e.Filter.OriginalPreviewImage = originalPreviewBitmap;
            e.Filter.Buffer = previewBuffer;

            if (FilterGalleryView.Visibility != Visibility.Collapsed)
            {
                DismissFilterGallery(false, () =>
                {
                    ShowFilterOSD(e.Filter);
                });
            }
            else
            {
                ShowFilterOSD(e.Filter);
            }

        }

        private void OnFilterDeleted(object sender, DeleteFilterEventArgs e)
        {
            filterManager.DeleteFilter(e.Filter);

            if (FilterContainerView.Visibility != Visibility.Collapsed)
            {
                DismissFilterOSD();
            }

        }

        private void OnAutoEnhanceButtonClick(object sender, RoutedEventArgs e)
        {
            PerformAutoEnhance();
        }

        private void OnFilterCountChanged(object sender, EventArgs e)
        {
            if (filterManager.AppliedFilters.Count == 0)
            {
                FilterCountLabel.Text = "0 FILTER";
                FilterListButton.IsEnabled = false;
            }
            else if (filterManager.AppliedFilters.Count == 1)
            {
                FilterCountLabel.Text = "1 FILTER";
                FilterListButton.IsEnabled = true;
            }
            else
            {
                FilterCountLabel.Text = filterManager.AppliedFilters.Count.ToString() + " FILTERS";
                FilterListButton.IsEnabled = true;
            }
        }

        private void OnCropButtonClick(object sender, RoutedEventArgs e)
        {
            ViewFinder.Source = originalPreviewBitmap;

            ShowCropOSD(() => {
                ViewFinder.ShowCropFinder();
            });            
        }

        private void OnCropAreaChanged(object sender, CropAreaChangedEventArgs e)
        {
            filterManager.CropFilter.UpdateCropRect(e.X, e.Y, e.Width, e.Height);
        }

        private void OnCropFilterDismiss(object sender, EventArgs e)
        {
            ViewFinder.DismissCropFinder();

            DismissCropOSD(() => {
                filterManager.CropFilter.OriginalImage = originalBitmap;
                filterManager.CropFilter.CurrentImage = currentPreviewBitmap;
                filterManager.CropFilter.OriginalPreviewImage = originalPreviewBitmap;
                filterManager.CropFilter.Buffer = previewBuffer;

                ViewFinder.Source = currentPreviewBitmap;
                filterManager.ApplyCrop();
            });
        }

        private void OnCropFilterDelete(object sender, EventArgs e)
        {
            ViewFinder.DismissCropFinder();
            DismissCropOSD(() => {
                ViewFinder.Source = currentPreviewBitmap;
                filterManager.DiscardCrop();
            });
        }

        private void OnRotationButtonClick(object sender, RoutedEventArgs e)
        {
            ShowRotationOSD(() => {
                filterManager.RotationFilter.OriginalImage = originalBitmap;
                filterManager.RotationFilter.CurrentImage = currentPreviewBitmap;
                filterManager.RotationFilter.OriginalPreviewImage = originalPreviewBitmap;
                filterManager.RotationFilter.Buffer = previewBuffer;

                filterManager.ApplyRotationFilter();
            });
        }

        private void OnRotationValueChanged(object sender, EventArgs e)
        {            
            int deg = (int)RotationView.Degree;
            RotationButton.Content = deg.ToString() + " degree";

            filterManager.RotationFilter.Degree = RotationView.Degree;
            filterManager.RotationFilter.UpdatePreviewAsync();
        }

        private void OnRotationFilterDismiss(object sender, EventArgs e)
        {
            DismissRotationOSD();
        }

        private void OnRotationFilterDelete(object sender, EventArgs e)
        {
            DismissRotationOSD(() =>
            {
                RotationButton.Content = "0 degree";
                RotationView.Degree = 0;
                RotationView.AmountSlider.Value = 0;
                filterManager.DiscardRotationFilter();
            });
        }

        private void OnFilterListButtonClick(object sender, RoutedEventArgs e)
        {
            ShowActiveFilterList();
        }

        private void OnResetTransformButtonClick(object sender, RoutedEventArgs e)
        {
            if (!filterManager.AppliedFilters.Contains(filterManager.CropFilter) 
                && !filterManager.AppliedFilters.Contains(filterManager.RotationFilter))
            {
                return;
            }

            var dialog = ModalPopup.Show("This will reset cropping and rotation settings.\n\nDo you wish to continue?",
                   "Reset Transform", new List<string> { AppResources.GenericConfirmText, AppResources.GenericCancelText });
            dialog.DismissWithButtonClick += (s, args) =>
            {
                int buttonIndex = (args as ModalPopupEventArgs).ButtonIndex;
                if (buttonIndex == 0)
                {
                    filterManager.ResetTransform();
                }
            };
        }

        private void OnClearFXFiltersButtonClick(object sender, RoutedEventArgs e)
        {
            if (!filterManager.HasAppliedFilterOtherThan(Filters.FilterCategory.Transform))
            {
                return;
            }

            var dialog = ModalPopup.Show("All filters (except for crop and rotation) will be removed. \n\nDo you with to continue?",
                   "Clear Effects", new List<string> { AppResources.GenericConfirmText, AppResources.GenericCancelText });
            dialog.DismissWithButtonClick += (s, args) =>
            {
                int buttonIndex = (args as ModalPopupEventArgs).ButtonIndex;
                if (buttonIndex == 0)
                {
                    filterManager.ClearAllFiltersOtherThan(Filters.FilterCategory.Transform);
                    OnFilterCountChanged(this, null);
                    UpdatePreviewAsync();
                }
            };

        }

        private void OnNextButtonClick(object sender, RoutedEventArgs e)
        {
            ShowUploaderView();
        }

        private void OnUploaderRequestDismiss(object sender, EventArgs e)
        {
            DismissUploaderView();
        }

        private void OnUploaderRequestExit(object sender, EventArgs e)
        {
            NavigationService.GoBack();
            NavigationService.RemoveBackEntry();
        }
    }
}
