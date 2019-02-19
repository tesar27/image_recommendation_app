using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Lot_art
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Dictionary<BitmapImage, Image> map = new Dictionary<BitmapImage, Image>();
        public MainWindow()
        {
            InitializeComponent();
            Button_Click_1(null, null);
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            ImportImages.ImportToDatabase();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            List<BitmapImage> imgs = new List<BitmapImage>();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var query = db.Images.Include("Scores").OrderBy(d => Guid.NewGuid()).Take(100).ToList();
                int i = 0;
                
                foreach (var img in query)
                {
                    var src = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + img.Url.Replace("/", "\\")));
                    imgs.Add(src);
                    map.Add(src, img);
                }
            }
            items.ItemsSource = imgs;
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var src = (BitmapImage)((System.Windows.Controls.Image)sender).Source;
            var img = map[src];
            map.Clear();
            List<BitmapImage> imgs = new List<BitmapImage>();
            imgs.Add(src);
            map.Add(src, img);
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var scs = img.Scores.ToDictionary(s => s.TagId, s => s.Value);
                var tagIds = img.Scores.Select(s => s.TagId).ToList();
                var res = db.Scores.Include("Image").Where(s => tagIds.Contains(s.TagId)).ToList();
                var resImg = res.GroupBy(s => s.ImageId).OrderByDescending(g => g.Sum(s => scs[s.TagId] * s.Value)).Select(s => s.FirstOrDefault().Image).Where(i => i.Id != img.Id).ToList();
                foreach (var imgR in resImg.Take(10))
                {
                    var srcR = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + imgR.Url.Replace("/", "\\")));
                    imgs.Add(srcR);
                    map.Add(srcR, imgR);
                }
            }
            items.ItemsSource = imgs;
        }



    }
}
