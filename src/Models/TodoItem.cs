using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace Todos.Models
{
    class TodoItem
    {

        public string id;

        public string title { get; set; }

        public string detail { get; set; }

        public BitmapImage image { get; set; }

        public bool? completed { get; set; }

        public DateTime date { get; set; }

        public TodoItem(string title, string detail, BitmapImage image, DateTime date)
        {
            this.id = Guid.NewGuid().ToString(); //生成id
            this.title = title;
            this.detail = detail;
            this.image = image;
            this.date = date;
            this.completed = false; //默认为未完成
        }
    }
}
