using System;
using System.Collections.Generic;
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

namespace MapTool
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            xLength.Text = xSlider.Minimum.ToString();
            yLength.Text = ySlider.Minimum.ToString();
        }

        private void OnClick_Create(object sender, RoutedEventArgs e)
        {
            parPanel.Children.Clear();
            int xLen = int.Parse(xLength.Text);
            int yLen = int.Parse(yLength.Text);

            for (int y = 0; y < yLen; ++y)
            {
                StackPanel panel = new StackPanel
                {
                    Orientation = Orientation.Horizontal
                };

                for (int x = 0; x < xLen; ++x)
                {
                    Stream imageStreamSource = new FileStream(imageComboBox.Text, FileMode.Open, FileAccess.Read, FileShare.Read);
                    PngBitmapDecoder decoder = new PngBitmapDecoder(imageStreamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                    BitmapSource bitmapSource = decoder.Frames[0];
                    ImageSource imageSource = bitmapSource;

                    MyImage newImage = new MyImage
                    {
                        Width = 10,
                        Height = 10,
                        m_offset = (y + 1) * (x + 1),
                        m_path = imageComboBox.Text,
                        Source = imageSource
                    };
                    newImage.DragEnter += NewImage_DragEnter;
                    newImage.MouseDown += NewImage_MouseDown;
                    panel.Children.Add(newImage);
                }
                parPanel.Children.Add(panel);
            }
        }

        private void NewImage_DragEnter(object sender, DragEventArgs e)
        {
            MyImage myImg = sender as MyImage;

            try
            {
                Stream imageStreamSource = new FileStream(imageComboBox.Text, FileMode.Open, FileAccess.Read, FileShare.Read);
                PngBitmapDecoder decoder = new PngBitmapDecoder(imageStreamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                BitmapSource bitmapSource = decoder.Frames[0];
                ImageSource imageSource = bitmapSource;

                myImg.Source = imageSource;
                myImg.m_path = imageComboBox.Text;
            }
            catch (NotImplementedException)
            {
                MessageBox.Show("MouseDown -> NotImplementedException error");
            }
        }

        private void OnClick_Save(object sender, RoutedEventArgs e)
        {
            string textPath = @"./map.txt";

            if (File.Exists(textPath))
            {
                string[] lines = File.ReadAllLines(textPath);

                foreach (string line in lines)
                {
                    File.WriteAllText(textPath, string.Empty);
                }
            }
            else
            {
                File.Create(textPath);
            }

            //====================================================

            using (var sw = File.AppendText(textPath))
            {
                sw.WriteLine("tileStart");
                foreach (ComboBoxItem item in imageComboBox.Items)
                {
                    sw.WriteLine(item.Content.ToString());
                }
                sw.WriteLine("tileEnd");

                sw.WriteLine("sizeStart");
                sw.WriteLine(xLength.Text + "," + yLength.Text);
                sw.WriteLine("sizeEnd");

                sw.WriteLine("mapStart");

                int xLen = int.Parse(xLength.Text);
                int yLen = int.Parse(yLength.Text);

                for (int y = 0; y < yLen; ++y)
                {
                    MyImage myImage;
                    StackPanel childPanel;
                    string line = string.Empty, path = string.Empty;

                    for (int x = 0; x < xLen; ++x)
                    {
                        childPanel = parPanel.Children[y] as StackPanel;
                        myImage = childPanel.Children[x] as MyImage;
                        path = myImage.m_path;
                        switch (path)
                        {
                            case "block/bog_green0.png":
                                line += 0;
                                break;
                            case "block/block.png":
                                line += 1;
                                break;
                            case "block/cobble_blood1.png":
                                line += 2;
                                break;
                            case "block/ice0.png":
                                line += 3;
                                break;
                            default:
                                line += "-";
                                break;
                        }
                        line += ",";
                    }
                    sw.WriteLine(line);
                }
                sw.WriteLine("mapEnd");
            }

        }

        private void OnClick_Load(object sender, RoutedEventArgs e)
        {
            StreamReader file = File.OpenText("@./map.txt");
            if(!File.Exists(@"/map.txt")) return;
            
        }

        private void xSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (xLength != null)
                xLength.Text = e.NewValue.ToString();
        }

        private void ySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (yLength != null)
                yLength.Text = e.NewValue.ToString();
        }

        private void xLength_TextChanged(object sender, TextChangedEventArgs e)
        {
            xSlider.Value = int.Parse((sender as TextBox).Text);
        }

        private void yLength_TextChanged(object sender, TextChangedEventArgs e)
        {
            ySlider.Value = int.Parse((sender as TextBox).Text);
        }

        private void TextInputIsDigit(object sender, TextCompositionEventArgs e)
        {
            foreach (char c in e.Text)
            {
                if (!Char.IsDigit(c))
                {
                    e.Handled = true;
                    break;
                }
            }
        }

        private void NewImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MyImage myImg = sender as MyImage;

            try
            {
                Stream imageStreamSource = new FileStream(imageComboBox.Text, FileMode.Open, FileAccess.Read, FileShare.Read);
                PngBitmapDecoder decoder = new PngBitmapDecoder(imageStreamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                BitmapSource bitmapSource = decoder.Frames[0];
                ImageSource imageSource = bitmapSource;

                myImg.Source = imageSource;
                myImg.m_path = imageComboBox.Text;
            }
            catch (NotImplementedException)
            {
                MessageBox.Show("MouseDown -> NotImplementedException error");
            }
        }

        public class MyImage : Image
        {
            public int m_offset;
            public string m_path;
        }
    }
}

