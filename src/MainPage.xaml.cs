using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace Todos
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private List<Grid> m_renderedGrids = new List<Grid>();
        public MainPage()
        {
            this.InitializeComponent();
            var viewTitleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;
            viewTitleBar.BackgroundColor = Windows.UI.Colors.CornflowerBlue;
            viewTitleBar.ButtonBackgroundColor = Windows.UI.Colors.CornflowerBlue;
            this.ViewModel = new ViewModels.TodoItemViewModel();
        }
        private void Checkbox_Checked(object sender, RoutedEventArgs e)
        {
            int index = m_renderedGrids.Count;
            for (int i = 0; i < index; ++i)
            {
                Grid currentGrid = m_renderedGrids[i] as Grid;
                Line myLine = (Line)GetChildren(currentGrid).First(x => x.Name == "line");
                CheckBox myCheckBox = (CheckBox)GetChildren(currentGrid).First(x => x.Name == "checkbox");
                if (myCheckBox.IsChecked == true)
                {
                    myLine.Visibility = Visibility.Visible;
                }
                else
                {
                    myLine.Visibility = Visibility.Collapsed;
                }
            }
        }
        ViewModels.TodoItemViewModel ViewModel { get; set; }

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
            if (e.Parameter.GetType() == typeof(ViewModels.TodoItemViewModel))
            {
                this.ViewModel = (ViewModels.TodoItemViewModel)(e.Parameter);
            }
        }

        private void TodoItem_ItemClicked(object sender, ItemClickEventArgs e)
        {
            ViewModel.SelectedItem = (Models.TodoItem)(e.ClickedItem);
            Grid currentGrid = (Grid)GetChildren(sender as ListView).First(x => x.Name == "ItemGrid");
            Line myLine = (Line)GetChildren(currentGrid).First(x => x.Name == "line");
            CheckBox myCheckBox = (CheckBox)GetChildren(currentGrid).First(x => x.Name == "checkbox");
            if (myCheckBox.IsChecked == true)
            {
                ViewModel.SelectedItem.completed = true;
            }
            else
            {
                ViewModel.SelectedItem.completed = false;
            }
            if (All.ActualWidth < 600.0)
            {
                Frame.Navigate(typeof(NewPage), ViewModel);
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

        private void AddAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SelectedItem = null;
            Frame.Navigate(typeof(NewPage), ViewModel); 
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

            if (CUButton.Content.ToString() == "Create")
            {
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
                ViewModel.AddTodoItem(title.Text, detail.Text, defaultimage, date.Date.DateTime);
                //Frame.Navigate(typeof(MainPage), ViewModel);
            }
            else
            {
                ViewModel.UpdateTodoItem(ViewModel.SelectedItem.id, title.Text, detail.Text, date.Date.DateTime);
                CUButton.Content = "Create";
                //Frame.Navigate(typeof(MainPage), ViewModel);
            }
        }
        private async void cancelButton_Click(object sender, RoutedEventArgs e)
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
            date.Date = DateTime.Today;
            ViewModel.SelectedItem = null;
            CUButton.Content = "Create";
        }
        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            // Add the Grids inside DataTemplate into a List.
            // You can get the controls inside DataTemplate from m_renderedGrids.
            m_renderedGrids.Add(sender as Grid);
            int index = m_renderedGrids.Count;
            for (int i = 0; i < index; ++i)
            {
                Grid currentGrid = m_renderedGrids[i] as Grid;
                Image myCon = (Image)GetChildren(currentGrid).First(x => x.Name == "image");
                if (All.ActualWidth < 600.0)
                {
                    myCon.Visibility = Visibility.Collapsed;
                }
                else
                {
                    myCon.Visibility = Visibility.Visible;
                }
            }
        }
        private List<FrameworkElement> GetChildren(DependencyObject parent)
        {
            List<FrameworkElement> controls = new List<FrameworkElement>();

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); ++i)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is FrameworkElement)
                {
                    controls.Add(child as FrameworkElement);
                }
                controls.AddRange(GetChildren(child));
            }

            return controls;
        }
        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            int index = m_renderedGrids.Count;
            for (int i = 0; i < index; ++i)
            {
                Grid currentGrid = m_renderedGrids[i] as Grid;
                Image myCon = (Image)GetChildren(currentGrid).First(x => x.Name == "image");
                if (e.NewSize.Width < 600.0)
                {
                    myCon.Visibility = Visibility.Collapsed;
                }
                else
                {
                    myCon.Visibility = Visibility.Visible;
                }
            }
        }
    }
}
