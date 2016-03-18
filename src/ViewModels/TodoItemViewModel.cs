using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            AddTodoItem("1", "1", null, DateTime.Today);
        }

        public void AddTodoItem(string title, string detail, BitmapImage image, DateTime date)
        {
            this.allItems.Add(new Models.TodoItem(title, detail, image, date));
            
            // set selectedItem to null after add
            this.selectedItem = null;
        }

        public void RemoveTodoItem(string id)
        {
            this.allItems.Remove(this.allItems.SingleOrDefault(i => i.id == id));

            // set selectedItem to null after remove
            this.selectedItem = null;
        }

        public void UpdateTodoItem(string id, string title, string detail, BitmapImage image, DateTime date)
        {
            this.allItems.SingleOrDefault(i => i.id == id).title = title;
            this.allItems.SingleOrDefault(i => i.id == id).detail = detail;
            this.allItems.SingleOrDefault(i => i.id == id).image = image;
            this.allItems.SingleOrDefault(i => i.id == id).date = date;
            
            // set selectedItem to null after update
            this.selectedItem = null;
        }

        internal void UpdateTodoItem(string id, string title, string detail, DateTime date)
        {
            this.allItems.SingleOrDefault(i => i.id == id).title = title;
            this.allItems.SingleOrDefault(i => i.id == id).detail = detail;
            this.allItems.SingleOrDefault(i => i.id == id).date = date;

            // set selectedItem to null after update
            this.selectedItem = null;
        }
    }
}
