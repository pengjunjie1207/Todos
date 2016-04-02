using NotificationsExtensions.Tiles;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todos.Models;
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
            string sql = @"SELECT id, title, detail, date
                           FROM TodoItem
                           ORDER BY date";
            using (var statement = Services.SQLiteService.SQLiteService.conn.Prepare(sql))
            {
                while (statement.Step() == SQLiteResult.ROW)
                {
                    TodoItem todoItem = new TodoItem((string)statement[0],
                        (string)statement[1],
                        (string)statement[2],
                        DateTime.Parse((string)statement[3]));
                    this.allItems.Add(todoItem);
                }
            }
        }

        public void AddTodoItem(string title, string detail, WriteableBitmap image, DateTime date)
        {
            this.allItems.Add(new Models.TodoItem(title, detail, image, date));

            Items_changed();

            string sql = "INSERT INTO TodoItem (id, title, detail, date) VALUES (?, ?, ?, ?)";
            using (var statement = Services.SQLiteService.SQLiteService.conn.Prepare(sql))
            {
                statement.Bind(1, this.allItems.Last().id);
                statement.Bind(2, this.allItems.Last().title);
                statement.Bind(3, this.allItems.Last().detail);
                statement.Bind(4, this.allItems.Last().date.ToString("yyyy-MM-dd"));
                statement.Step();
            }

            // set selectedItem to null after add
            this.selectedItem = null;
        }

        public void RemoveTodoItem(string id)
        {
            string sql = "DELETE FROM ToDoItem WHERE id = ? ";
            using (var statement = Services.SQLiteService.SQLiteService.conn.Prepare(sql))
            {
                statement.Bind(1, id);
                statement.Step();
            }

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

            string sql = "UPDATE TodoItem SET id = ?, title = ?, detail = ?, date = ? WHERE id = ?";
            using (var statement = Services.SQLiteService.SQLiteService.conn.Prepare(sql))
            {
                statement.Bind(1, id);
                statement.Bind(2, title);
                statement.Bind(3, detail);
                statement.Bind(4, date.ToString("yyyy-MM-dd"));
                statement.Bind(5, id);
                statement.Step();
            }

            Items_changed();

            // set selectedItem to null after update
            this.selectedItem = null;
        }

        public void UpdateTodoItem(string id, string title, string detail, DateTime date)
        {
            this.allItems.SingleOrDefault(i => i.id == id).title = title;
            this.allItems.SingleOrDefault(i => i.id == id).detail = detail;
            this.allItems.SingleOrDefault(i => i.id == id).date = date;

            string sql = "UPDATE TodoItem SET id = ?, title = ?, detail = ?, date = ? WHERE id = ?";
            using (var statement = Services.SQLiteService.SQLiteService.conn.Prepare(sql))
            {
                statement.Bind(1, id);
                statement.Bind(2, title);
                statement.Bind(3, detail);
                statement.Bind(4, date.ToString("yyyy-MM-dd"));
                statement.Bind(5, id);
                statement.Step();
            }

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
