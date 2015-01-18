using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
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

namespace Clusterization.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var files = new string[] 
            {
                "easy_12.arff",
                "synth1.arff",
                "synth2.arff",
                "test2in1.arff",
                "test121.arff",
                "test121212.arff"
            };

            fileName.ItemsSource = files;

            fileName.SelectedValue = files[0];
            scaleBox.Text = "10";
            clusters.Text = "3";

            centerX.Text = "50";
            centerY.Text = "50";

            runs.Text = "10";

            clusterize.Click +=clusterize_Click;

            this.kmeans = new KMeans();
        }

        void clusterize_Click(object sender, RoutedEventArgs e)
        {
            canvas.Children.Clear();

 	        var file = fileName.SelectedValue.ToString();
            var c = int.Parse(clusters.Text);

            var data = ReadData(file);

            var r = int.Parse(runs.Text);

            IList<Tuple<double, double>> initialMeans;
            IList<Tuple<double, double>> centroids;
            var classifiedData = this.kmeans.Clusterize(data, c, r, out initialMeans, out centroids);

            this.DrawPoints(classifiedData, c, initialMeans, centroids);
        }

        private void DrawPoints(IList<Tuple<double, double, int>> points, int clusters, IList<Tuple<double, double>> initialMeans, IList<Tuple<double, double>> centroids)
        {
            var brushes = this.GetBrushes(clusters);
            var cX = int.Parse(centerX.Text);
            var cY = int.Parse(centerY.Text);
            var center = new Tuple<int, int>(cX, cY);
            var scale = int.Parse(scaleBox.Text);            

            foreach (var point in points)
            {
                var circle = new Ellipse
                {
                    Width = 7,
                    Height = 7,
                    Fill = brushes[point.Item3 - 1]
                };
                circle.Margin = new Thickness(-center.Item1 + point.Item1 * scale,
                -center.Item2 + point.Item2 * scale, 0, 0);

                canvas.Children.Add(circle);
            }

            //DrawInitialMeans(initialMeans, brushes, center, scale);
            
        }

        private void DrawInitialMeans(IList<Tuple<double, double>> initialMeans, Brush[] brushes, Tuple<int, int> center, int scale)
        {
            for (int i = 0; i < initialMeans.Count; i++)
            {
                var mean = initialMeans[i];

                var x = -center.Item1 + mean.Item1 * scale;
                var y = -center.Item2 + mean.Item2 * scale;
                
                var meanFigure = new Polygon
                {
                    Points = new PointCollection()
                    {
                        new Point(x - 5, y - 5),
                        new Point(x + 5, y - 5),
                        new Point(x, y + 5)
                    },
                    Fill = brushes[i]
                };
                //meanFigure.Margin = new Thickness(x, y, 0, 0);

                canvas.Children.Add(meanFigure);
            }
        }

        private Brush[] GetBrushes(int number)
        {
            var brushes = new Brush[number];

            var brushesCollection = new List<Brush>()
            {
                Brushes.Black,
                Brushes.Red,
                Brushes.Blue,
                Brushes.Green,
                Brushes.Orange,
                Brushes.Brown,
                Brushes.Purple,
                Brushes.Yellow,
                Brushes.Pink,
                Brushes.Gray
            };

            for (int i = 0; i < number; i++)
            {
                brushes[i] = PickRandomBrush(brushesCollection);
            }

            return brushes;
        }

        private Brush PickRandomBrush(List<Brush> brushes)
        {
            Brush result = Brushes.Transparent;

            int index = this.random.Next(brushes.Count);
            result = brushes[index];
            brushes.RemoveAt(index);            
            return result;
        } 

        private static List<Tuple<double, double, int>> ReadData(string fileName)
        {
            var data = new List<Tuple<double, double, int>>();

            using (var sr = new StreamReader(fileName))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.StartsWith("@"))
                    {
                        continue;
                    }

                    var dataEntry = line.Split(',');

                    data.Add(new Tuple<double, double, int>(double.Parse(dataEntry[0], CultureInfo.InvariantCulture), double.Parse(dataEntry[1], CultureInfo.InvariantCulture), 0));
                }
            }
            return data;
        }

        private KMeans kmeans;
        private Random random = new Random();
    }
}
