using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;
using Windows.Graphics.Imaging;

namespace Todos
{
    public sealed partial class NewPage : Page
    {
        public NewPage()
        {
            this.InitializeComponent();
            var viewTitleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;
            viewTitleBar.BackgroundColor = Windows.UI.Colors.CornflowerBlue;
            viewTitleBar.ButtonBackgroundColor = Windows.UI.Colors.CornflowerBlue;
        }

        private ViewModels.TodoItemViewModel ViewModel;
        private WriteableBitmap writeableBitmap;
        private StorageFile imageFile;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                AppViewBackButtonVisibility.Visible;

            ViewModel = ((ViewModels.TodoItemViewModel)e.Parameter);
            if (ViewModel.SelectedItem == null)
            {
                CUButton.Content = "Create";
                //var i = new MessageDialog("Welcome!").ShowAsync();
            }
            else
            {
                title.Text = ViewModel.SelectedItem.title;
                detail.Text = ViewModel.SelectedItem.detail;
                image.Source = ViewModel.SelectedItem.image;
                date.Date = ViewModel.SelectedItem.date;
                CUButton.Content = "Update";
            }
        }

        private async void CUButton_Click(object sender, RoutedEventArgs e)
        {
            if (title.Text == "")
            {
                var i = new MessageDialog("Title Empty!").ShowAsync();
                return;
            }
            if (detail.Text == "")
            {
                var i = new MessageDialog("Detail Empty!").ShowAsync();
                return;
            }
            if (date.Date.Date < DateTime.Today.Date)
            {
                var i = new MessageDialog("Invalid Date!").ShowAsync();
                return;
            }
            WriteableBitmap defaultimage = null;
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/background.jpg"));
            if (file != null)
            {
                using (IRandomAccessStream fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
                {
                    BitmapDecoder bitmapDecoder = await BitmapDecoder.CreateAsync(fileStream);

                    BitmapTransform dummyTransform = new BitmapTransform();
                    PixelDataProvider pixelDataProvider =
                       await bitmapDecoder.GetPixelDataAsync(BitmapPixelFormat.Bgra8,
                       BitmapAlphaMode.Premultiplied, dummyTransform,
                       ExifOrientationMode.RespectExifOrientation,
                       ColorManagementMode.ColorManageToSRgb);
                    byte[] pixelData = pixelDataProvider.DetachPixelData();

                    defaultimage = new WriteableBitmap(
                       (int)bitmapDecoder.OrientedPixelWidth,
                       (int)bitmapDecoder.OrientedPixelHeight);
                    using (Stream pixelStream = defaultimage.PixelBuffer.AsStream())
                    {
                        await pixelStream.WriteAsync(pixelData, 0, pixelData.Length);
                    }
                }
            }
            if (CUButton.Content.ToString() == "Create")
            {
                if (writeableBitmap == null)
                {
                    ViewModel.AddTodoItem(title.Text, detail.Text, defaultimage, date.Date.DateTime);
                    ViewModel.AllItems.Last().imageFile = file;
                }
                else
                {
                    ViewModel.AddTodoItem(title.Text, detail.Text, writeableBitmap, date.Date.DateTime);
                    ViewModel.AllItems.Last().imageFile = imageFile;
                }
            }
            else
            {
                if (writeableBitmap == null)
                {
                    ViewModel.SelectedItem.imageFile = file;
                    ViewModel.UpdateTodoItem(ViewModel.SelectedItem.id, title.Text, detail.Text, defaultimage, date.Date.DateTime);
                }
                else
                {
                    ViewModel.SelectedItem.imageFile = imageFile;
                    ViewModel.UpdateTodoItem(ViewModel.SelectedItem.id, title.Text, detail.Text, writeableBitmap, date.Date.DateTime);
                }
            }
            Frame.Navigate(typeof(MainPage), ViewModel);
        }
        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.RemoveTodoItem(ViewModel.SelectedItem.id);
            Frame.Navigate(typeof(MainPage), ViewModel);
        }
        private async void resetButton_Click(object sender, RoutedEventArgs e)
        {
            title.Text = "";
            detail.Text = "";
            WriteableBitmap defaultimage = null;
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/background.jpg"));
            if (file != null)
            {
                imageFile = file;
                using (IRandomAccessStream fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
                {
                    BitmapDecoder bitmapDecoder = await BitmapDecoder.CreateAsync(fileStream);

                    BitmapTransform dummyTransform = new BitmapTransform();
                    PixelDataProvider pixelDataProvider =
                       await bitmapDecoder.GetPixelDataAsync(BitmapPixelFormat.Bgra8,
                       BitmapAlphaMode.Premultiplied, dummyTransform,
                       ExifOrientationMode.RespectExifOrientation,
                       ColorManagementMode.ColorManageToSRgb);
                    byte[] pixelData = pixelDataProvider.DetachPixelData();

                    defaultimage = new WriteableBitmap(
                       (int)bitmapDecoder.OrientedPixelWidth,
                       (int)bitmapDecoder.OrientedPixelHeight);
                    using (Stream pixelStream = defaultimage.PixelBuffer.AsStream())
                    {
                        await pixelStream.WriteAsync(pixelData, 0, pixelData.Length);
                    }
                }
            }
            image.Source = defaultimage;
            writeableBitmap = defaultimage;
            date.Date = DateTime.Today;
        }
        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SelectedItem = null;
            Frame.Navigate(typeof(MainPage), ViewModel);
        }
        private async void SelectPictureButton_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker open = new FileOpenPicker();
            open.ViewMode = PickerViewMode.Thumbnail;
            open.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            open.FileTypeFilter.Add(".jpg");
            open.FileTypeFilter.Add(".png");
            StorageFile file = await open.PickSingleFileAsync();
            if (file != null)
            {
                imageFile = file;
                using (IRandomAccessStream fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
                {
                    BitmapDecoder bitmapDecoder = await BitmapDecoder.CreateAsync(fileStream);

                    BitmapTransform dummyTransform = new BitmapTransform();
                    PixelDataProvider pixelDataProvider =
                       await bitmapDecoder.GetPixelDataAsync(BitmapPixelFormat.Bgra8,
                       BitmapAlphaMode.Premultiplied, dummyTransform,
                       ExifOrientationMode.RespectExifOrientation,
                       ColorManagementMode.ColorManageToSRgb);
                    byte[] pixelData = pixelDataProvider.DetachPixelData();

                    writeableBitmap = new WriteableBitmap(
                       (int)bitmapDecoder.OrientedPixelWidth,
                       (int)bitmapDecoder.OrientedPixelHeight);
                    using (Stream pixelStream = writeableBitmap.PixelBuffer.AsStream())
                    {
                        await pixelStream.WriteAsync(pixelData, 0, pixelData.Length);
                    }
                }
            }
            image.Source = writeableBitmap;
        }
    }
}
