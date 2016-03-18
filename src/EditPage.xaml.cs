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
        private BitmapImage bitmapImage;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame.CanGoBack)
            {
                // Show UI in title bar if opted-in and in-app backstack is not empty.
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Visible;
            }
            else
            {
                // Remove the UI from the title bar if in-app back stack is empty.
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Collapsed;
            }
            ViewModel = ((ViewModels.TodoItemViewModel)e.Parameter);
            if (ViewModel.SelectedItem == null)
            {
                CUButton.Content = "Create";
                //var i = new MessageDialog("Welcome!").ShowAsync();
            }
            else
            {
                title.Text = ViewModel.SelectedItem.title + ViewModel.SelectedItem.completed;
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
            BitmapImage defaultimage = new BitmapImage();
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/background.jpg"));
            if (file != null)
            {
                using (IRandomAccessStream fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
                {
                    // Set the image source to the selected bitmap 
                    defaultimage.DecodePixelWidth = 600; //match the target Image.Width, not shown
                    await defaultimage.SetSourceAsync(fileStream);
                }
            }
            if (CUButton.Content.ToString() == "Create")
            {
                if (bitmapImage == null)
                {
                    ViewModel.AddTodoItem(title.Text, detail.Text, defaultimage, date.Date.DateTime);
                }
                else
                {
                    ViewModel.AddTodoItem(title.Text, detail.Text, bitmapImage, date.Date.DateTime);
                }
            }
            else
            {
                if (bitmapImage == null)
                {
                    ViewModel.UpdateTodoItem(ViewModel.SelectedItem.id, title.Text, detail.Text, defaultimage, date.Date.DateTime);
                }
                else
                {
                    ViewModel.UpdateTodoItem(ViewModel.SelectedItem.id, title.Text, detail.Text, bitmapImage, date.Date.DateTime);
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
            BitmapImage defaultimage = new BitmapImage();
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/background.jpg"));
            if (file != null)
            {
                using (IRandomAccessStream fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
                {
                    // Set the image source to the selected bitmap 
                    defaultimage.DecodePixelWidth = 600; //match the target Image.Width, not shown
                    await defaultimage.SetSourceAsync(fileStream);
                }
            }
            image.Source = defaultimage;
            bitmapImage = defaultimage;
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
                using (IRandomAccessStream fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
                {
                    // Set the image source to the selected bitmap 
                    bitmapImage = new BitmapImage();
                    bitmapImage.DecodePixelWidth = 600; //match the target Image.Width, not shown
                    await bitmapImage.SetSourceAsync(fileStream);
                    image.Source = bitmapImage;
                }
            }
        }

    }
}
