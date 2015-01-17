using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
            clusterize.Click +=clusterize_Click;

            this.kmeans = new KMeans();
        }

        void clusterize_Click(object sender, RoutedEventArgs e)
        {
 	        var file = fileName.SelectedValue.ToString();
            var c = int.Parse(clusters.Text);

            var data = ReadData(file);

            this.kmeans.Clusterize(data, c);

            this.DrawPoints(data, c);
        }

        private void DrawPoints(IList<Tuple<double, double, int>> points, int clusters)
        {
            
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

                    data.Add(new Tuple<double, double, int>(double.Parse(dataEntry[0]), double.Parse(dataEntry[1]), 0));
                }
            }
            return data;
        }

        private KMeans kmeans;
    }
}
