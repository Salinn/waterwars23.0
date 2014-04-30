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
using Leap;
using System.Windows.Threading;

namespace waterwars23._0
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly GestureListener _leapListener;
        private readonly Controller _leapController;

        int counter;
        int map_location;

        Boolean guess;

        String WeatherCode;
        String Condition;
        String query;

        private Object thisLock = new Object();

        private void SafeWriteLine(String line)
        {
            lock (thisLock)
            {
                Console.WriteLine(line);
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            _leapController = new Controller();
            _leapListener = new GestureListener();

            _leapController.AddListener(_leapListener);

            _leapListener.OnFrameChanged += _leapListener_OnFrameChanged;
            SetUp();
        }

        public void _leapListener_OnFrameChanged(object sender, LeapListenerEventArgs e)
        {
            if (!Dispatcher.CheckAccess())
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)((() => _leapListener_OnFrameChanged(sender, e))));
            else
            {
                var controller = e.LmController;
                // Get the most recent frame and report some basic information
                Leap.Frame frame = _leapController.Frame();

                /*SafeWriteLine("Frame id: " + frame.Id
                            + ", timestamp: " + frame.Timestamp
                            + ", hands: " + frame.Hands.Count
                            + ", fingers: " + frame.Fingers.Count
                            + ", tools: " + frame.Tools.Count
                            + ", gestures: " + frame.Gestures().Count);*/

                if (!frame.Hands.IsEmpty)
                {
                    // Get the first hand
                    Hand hand = frame.Hands[0];

                    // Check if the hand has any fingers
                    FingerList fingers = hand.Fingers;
                    if (!fingers.IsEmpty)
                    {
                        // Calculate the hand's average finger tip position
                        Leap.Vector avgPos = Leap.Vector.Zero;
                        foreach (Finger finger in fingers)
                        {
                            avgPos += finger.TipPosition;

                        }
                        avgPos /= fingers.Count;
                        if (fingers.Rightmost.TipVelocity.z > 0)
                        {
                            //SafeWriteLine("Pinky down bitch!");
                        }
                        //SafeWriteLine("Hand has " + fingers.Count
                        //            + " fingers, average finger tip position: " + avgPos);
                    }

                    // Get the hand's sphere radius and palm position
                    //SafeWriteLine("Hand sphere radius: " + hand.SphereRadius.ToString("n2")
                    //            + " mm, palm position: " + hand.PalmPosition);

                    // Get the hand's normal vector and direction
                    Leap.Vector normal = hand.PalmNormal;
                    Leap.Vector direction = hand.Direction;

                    // Calculate the hand's pitch, roll, and yaw angles
                    //SafeWriteLine("Hand pitch: " + direction.Pitch * 180.0f / (float)Math.PI + " degrees, "
                    //            + "roll: " + normal.Roll * 180.0f / (float)Math.PI + " degrees, "
                    //            + "yaw: " + direction.Yaw * 180.0f / (float)Math.PI + " degrees");
                }

                // Get gestures
                GestureList gestures = frame.Gestures();
                for (int i = 0; i < gestures.Count; i++)
                {
                    SafeWriteLine("Gesture!!!");
                    Gesture gesture = gestures[i];

                    switch (gesture.Type)
                    {
                        case Gesture.GestureType.TYPESWIPE:
                            SwipeGesture swipe = new SwipeGesture(gesture);
                            /*SafeWriteLine("Swipe id: " + swipe.Id
                                          + ", " + swipe.State
                                           + ", position: " + swipe.Position
                                           + ", direction: " + swipe.Direction
                                           + ", speed: " + swipe.Speed);*/
                            if (swipe.State == Gesture.GestureState.STATESTOP)
                            {

                                if (swipe.StartPosition.y > (swipe.Position.y + 15))
                                {
                                    SafeWriteLine("Select motion accepted");
                                }
                                if (swipe.StartPosition.x > (swipe.Position.x + 15))
                                {
                                    SafeWriteLine("DIS IS A SWIPE TO DA LEFT");
                                    no_button_Click(sender, new RoutedEventArgs());
                                }
                                else if (swipe.StartPosition.x < (swipe.Position.x - 15))
                                {
                                    SafeWriteLine("DIS IS A SWIPE TO DA RIGHT");
                                    yes_button_Click(sender, new RoutedEventArgs());
                                }
                            }
                            break;
                        case Gesture.GestureType.TYPEKEYTAP:
                            KeyTapGesture keytap = new KeyTapGesture(gesture);
                            SafeWriteLine("Tap id: " + keytap.Id
                                           + ", " + keytap.State
                                           + ", position: " + keytap.Position
                                           + ", direction: " + keytap.Direction);
                            break;
                        case Gesture.GestureType.TYPESCREENTAP:
                            ScreenTapGesture screentap = new ScreenTapGesture(gesture);
                            SafeWriteLine("Tap id: " + screentap.Id
                                           + ", " + screentap.State
                                           + ", position: " + screentap.Position
                                           + ", direction: " + screentap.Direction);
                            break;
                        default:
                            SafeWriteLine("Unknown gesture type.");
                            break;
                    }
                }

                if (!frame.Hands.IsEmpty || !frame.Gestures().IsEmpty)
                {
                    SafeWriteLine("");
                }
            }
        }

        private void SetUp()
        {
            counter = 1;
            map_location = 1;
            query = "0";
            Condition = "0";

            var uri = new Uri("http://i.imwx.com/images/maps/current/curwx_600x405.jpg");
            var bitmap = new BitmapImage(uri);
            map_pic_box.Source = bitmap;
            uri = new Uri("http://upload.wikimedia.org/wikipedia/en/4/44/Question_mark_(black_on_white).png");
            bitmap = new BitmapImage(uri);
            yes_no_pic_box.Source = bitmap;

            temp_box.Text = "0";
            cond_box.Text = "0";
            humid_box.Text = "0";
            wind_box.Text = "0";
            city_box.Text = "0";
            last_city_box.Text = "0";
            pressure_box.Text = "0";
        }

        private void yes_button_Click(object sender, RoutedEventArgs e){
            guess = true;
            run();
        }

        private void no_button_Click(object sender, RoutedEventArgs e){
            guess = false;
            run();
        }

        private void run()
        {
            if (counter != 11)
            {
                getCityInfo();
                setWeather();
                check_answer();
            }
            else { SetUp(); }
        }

        private void check_answer(){
            if ((guess && (WeatherCode == "11" || WeatherCode == "12")) || (!guess && WeatherCode != "11" && WeatherCode != "12"))
            {
                Console.WriteLine("---------------");
                Console.WriteLine(WeatherCode);
                Console.WriteLine("RIGHT!");
                var uri = new Uri("http://png-4.findicons.com/files/icons/1008/quiet/256/yes.png");
                var bitmap = new BitmapImage(uri);
                yes_no_pic_box.Source = bitmap;
                return;
            }else{
                Console.WriteLine("---------------");
                Console.WriteLine(WeatherCode);
                Console.WriteLine("WRONG!");
                var uri = new Uri("http://png-4.findicons.com/files/icons/1008/quiet/256/no.png");
                var bitmap = new BitmapImage(uri);
                yes_no_pic_box.Source = bitmap;
            }
        }

        private void getCityInfo(){
            if (counter == 1){
                query = String.Format("http://weather.yahooapis.com/forecastrss?w=2490383");
            }else if (counter == 2){
                query = String.Format("http://weather.yahooapis.com/forecastrss?w=2482949");
            }else if (counter == 3){
                query = String.Format("http://weather.yahooapis.com/forecastrss?w=2367105");
            }else if (counter == 4){
                query = String.Format("http://weather.yahooapis.com/forecastrss?w=2357024");
            }else if (counter == 5){
                query = String.Format("http://weather.yahooapis.com/forecastrss?w=2436704");
            }else if (counter == 6){
                query = String.Format("http://weather.yahooapis.com/forecastrss?w=2503863");
            }else if (counter == 7){
                query = String.Format("http://weather.yahooapis.com/forecastrss?w=2487610");
            }else if (counter == 8){
                query = String.Format("http://weather.yahooapis.com/forecastrss?w=12590014");
            }else if (counter == 9){
                query = String.Format("http://weather.yahooapis.com/forecastrss?w=2364681");
            }else if (counter == 10){
                query = String.Format("http://weather.yahooapis.com/forecastrss?w=12588041");
            }

            counter += 1;
        }

        private void setWeather(){
            XmlDocument wData = new XmlDocument();
            wData.Load(query);

            XmlNamespaceManager manger = new XmlNamespaceManager(wData.NameTable);
            manger.AddNamespace("yweather", "http://xml.weather.yahoo.com/ns/rss/1.0");

            XmlNode channel = wData.SelectSingleNode("rss").SelectSingleNode("channel");
            XmlNodeList nodes = wData.SelectNodes("/rss/channel/item/yweather:forecast", manger);

            temp_box.Text = channel.SelectSingleNode("item").SelectSingleNode("yweather:condition", manger).Attributes["temp"].Value;
            humid_box.Text = channel.SelectSingleNode("yweather:atmosphere", manger).Attributes["humidity"].Value;
            wind_box.Text = channel.SelectSingleNode("yweather:wind", manger).Attributes["speed"].Value;
            last_city_box.Text = city_box.Text;
            city_box.Text = channel.SelectSingleNode("yweather:location", manger).Attributes["city"].Value;
            pressure_box.Text = channel.SelectSingleNode("yweather:atmosphere", manger).Attributes["pressure"].Value;
            WeatherCode = channel.SelectSingleNode("item").SelectSingleNode("yweather:condition", manger).Attributes["code"].Value;
            cond_box.Text = Condition;
            Condition = channel.SelectSingleNode("item").SelectSingleNode("yweather:condition", manger).Attributes["text"].Value;
        }

        private void left_button_Click(object sender, RoutedEventArgs e){
            if (map_location < 2){
                map_location += 1;
            }else{
                map_location = 1;
            }

            pickMap();
        }

        private void right_button_Click(object sender, RoutedEventArgs e){
            if (map_location > 1){
                map_location -= 1;
            }else{
                map_location = 2;
            }

            pickMap();
        }

        private void pickMap(){
            if (map_location == 1){
                var uri = new Uri("http://i.imwx.com/images/maps/current/curwx_600x405.jpg");
                var bitmap = new BitmapImage(uri);
                map_pic_box.Source = bitmap;
            }else if (map_location == 2){
                var uri = new Uri("http://image.weather.com/images/maps/forecast/map_cldcvr_tnght_4namus_enus_600x405.jpg");
                var bitmap = new BitmapImage(uri);
                map_pic_box.Source = bitmap;
            }else if (map_location == 3){
                var uri = new Uri("http://image.weather.com/images/maps/forecast/precfcst_600x405.jpg");
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
