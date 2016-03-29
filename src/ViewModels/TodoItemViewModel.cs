using NotificationsExtensions.Tiles;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace Todos.ViewModels
{
    class TodoItemViewModel
    {
        private ObservableCollection<Models.TodoItem> allItems = new ObservableCollection<Models.TodoItem>();
        public ObservableCollection<Models.TodoItem> AllItems { get { return this.allItems; } }

        private Models.TodoItem selectedItem = default(Models.TodoItem);
        public Models.TodoItem SelectedItem { get { return selectedItem; } set { this.selectedItem = value; }  }

        public TodoItemViewModel()
        {
        }

        public void AddTodoItem(string title, string detail, WriteableBitmap image, DateTime date)
        {
            this.allItems.Add(new Models.TodoItem(title, detail, image, date));

            Items_changed();

            // set selectedItem to null after add
            this.selectedItem = null;
        }

        public void RemoveTodoItem(string id)
        {
            this.allItems.Remove(this.allItems.SingleOrDefault(i => i.id == id));

            Items_changed();

            // set selectedItem to null after remove
            this.selectedItem = null;
        }

        public void UpdateTodoItem(string id, string title, string detail, WriteableBitmap image, DateTime date)
        {
            this.allItems.SingleOrDefault(i => i.id == id).title = title;
            this.allItems.SingleOrDefault(i => i.id == id).detail = detail;
            this.allItems.SingleOrDefault(i => i.id == id).image = image;
            this.allItems.SingleOrDefault(i => i.id == id).date = date;

            Items_changed();

            // set selectedItem to null after update
            this.selectedItem = null;
        }

        public void UpdateTodoItem(string id, string title, string detail, DateTime date)
        {
            this.allItems.SingleOrDefault(i => i.id == id).title = title;
            this.allItems.SingleOrDefault(i => i.id == id).detail = detail;
            this.allItems.SingleOrDefault(i => i.id == id).date = date;

            Items_changed();

            // set selectedItem to null after update
            this.selectedItem = null;
        }

        internal void Items_changed()
        {
            if (this.allItems.Count == 0)
            {
                return;
            }
            TileContent content = new TileContent()
            {
                Visual = new TileVisual()
                {
                    TileSmall = new TileBinding()
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                            {
                                new TileText()
                                {
                                    Text = this.allItems.Last().title,
                                    Style = TileTextStyle.Subtitle
                                }
                            }
                        }
                    },
                    TileMedium = new TileBinding()
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                            {
                                new TileText()
                                {
                                    Text = this.allItems.Last().detail,
                                    Style = TileTextStyle.CaptionSubtle
                                }
                            }
                        }
                    },
                    TileWide = new TileBinding()
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                            {
                                new TileText()
                                {
                                    Text = this.allItems.Last().title,
                                    Style = TileTextStyle.Subtitle
                                },
                                new TileText()
                                {
                                    Text = this.allItems.Last().detail,
                                    Style = TileTextStyle.CaptionSubtle
                                }
                            }
                        }
                    },
                }
            };
            var notification = new TileNotification(content.GetXml());
            notification.ExpirationTime = DateTimeOffset.UtcNow.AddMinutes(3);
            TileUpdateManager.CreateTileUpdaterForApplication().Update(notification);
        }
    }
}
