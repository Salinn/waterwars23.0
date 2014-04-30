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
using System.Xml.Linq;
using System.Xml;
using System.IO;
using System.Web;

namespace waterwars23._0
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string query;
        String WeatherCode;
        int counter;
        int map_location;
        Boolean guess;

        public MainWindow()
        {
            InitializeComponent();
            SetUp();
        }

        private void SetUp()
        {
            counter = 1;
            query = "";
            map_location = 1;
            var uri = new Uri("http://image.weather.com/images/maps/forecast/precfcst_600x405.jpg");
            var bitmap = new BitmapImage(uri);
            map_pic_box.Source = bitmap;
        }

        private void yes_button_Click(object sender, RoutedEventArgs e)
        {
            guess = true;
            run();
        }

        private void no_button_Click(object sender, RoutedEventArgs e)
        {
            guess = false;
            run();
        }

        private void run()
        {
            getCityInfo();
            setWeather();
            check_answer();
        }

        private void check_answer()
        {
            if (guess == true && (WeatherCode == "11" || WeatherCode == "12"))
            {
                Console.WriteLine("RIGHT!");
                var uri = new Uri("http://1.bp.blogspot.com/-bzFqJmqgkTM/UChvEkWwKuI/AAAAAAAAA1c/Dwa1kMxo-uI/s1600/yes_logo.png");
                var bitmap = new BitmapImage(uri);
                yes_no_pic_box.Source = bitmap;
            }
            else
            {
                Console.WriteLine("WRONG!");
                var uri = new Uri("http://dogsandbabies.files.wordpress.com/2011/05/no.jpg");
                var bitmap = new BitmapImage(uri);
                yes_no_pic_box.Source = bitmap;
            }
        }

        private void getCityInfo()
        {
            if (counter == 1){
                query = String.Format("http://weather.yahooapis.com/forecastrss?w=2490383");
            }else if (counter == 2){
                query = String.Format("http://weather.yahooapis.com/forecastrss?w=2482949");
            }else if (counter == 3){
                query = String.Format("http://weather.yahooapis.com/forecastrss?w=2367105");
            }else if (counter == 4){
                query = String.Format("http://weather.yahooapis.com/forecastrss?w=2357024");
            }else{return;}

            counter += 1;
        }

        private void setWeather()
        {
            XmlDocument wData = new XmlDocument();
            wData.Load(query);

            XmlNamespaceManager manger = new XmlNamespaceManager(wData.NameTable);
            manger.AddNamespace("yweather", "http://xml.weather.yahoo.com/ns/rss/1.0");

            XmlNode channel = wData.SelectSingleNode("rss").SelectSingleNode("channel");
            XmlNodeList nodes = wData.SelectNodes("/rss/channel/item/yweather:forecast", manger);

            temp_box.Text = channel.SelectSingleNode("item").SelectSingleNode("yweather:condition", manger).Attributes["temp"].Value;
            cond_box.Text = channel.SelectSingleNode("item").SelectSingleNode("yweather:condition", manger).Attributes["text"].Value;
            humid_box.Text = channel.SelectSingleNode("yweather:atmosphere", manger).Attributes["humidity"].Value;
            wind_box.Text = channel.SelectSingleNode("yweather:wind", manger).Attributes["speed"].Value;
            city_box.Text = channel.SelectSingleNode("yweather:location", manger).Attributes["city"].Value;
            pressure_box.Text = channel.SelectSingleNode("yweather:atmosphere", manger).Attributes["pressure"].Value;
            WeatherCode = channel.SelectSingleNode("item").SelectSingleNode("yweather:condition", manger).Attributes["code"].Value;
        }

        private void left_button_Click(object sender, RoutedEventArgs e)
        {
            if (map_location < 4){
                map_location += 1;
            }else{
                map_location = 1;
            }

            pickMap();
        }

        private void right_button_Click(object sender, RoutedEventArgs e)
        {
            if (map_location > 1){
                map_location -= 1;
            }else{
                map_location = 4;
            }

            pickMap();
        }

        private void pickMap()
        {
            if (map_location == 1){
                var uri = new Uri("http://image.weather.com/images/maps/forecast/precfcst_600x405.jpg");
                var bitmap = new BitmapImage(uri);
                map_pic_box.Source = bitmap;
            }else if (map_location == 2){
                var uri = new Uri("http://i.imwx.com/images/maps/current/curwx_600x405.jpg");
                var bitmap = new BitmapImage(uri);
                map_pic_box.Source = bitmap;
            }else if (map_location == 3){
                var uri = new Uri("http://image.weather.com/images/maps/forecast/map_cldcvr_tnght_4namus_enus_600x405.jpg");
                var bitmap = new BitmapImage(uri);
                map_pic_box.Source = bitmap;
            }else if (map_location == 4){
                var uri = new Uri("http://image.weather.com/web/forecast/us_wxhi1_large_usen_600.jpg");
                var bitmap = new BitmapImage(uri);
                map_pic_box.Source = bitmap;
            }
        }
    }
}
