using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Math;
using Simulator;

//http://tipsandtricks.runicsoft.com/CSharp/WpfPixelDrawing.html

namespace WpfApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BitmapSource _bitmap;
        private PixelFormat _pf = PixelFormats.Rgb24;
        private int _width, _height, _rawStride;
        private byte[] _pixelData;
        private DispatcherTimer _timer;

        private Scene _scene;
        private int _step;
        private const double Increment = 24*60*60; // 6 hours

        public MainWindow()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Init();
            Task t = Task.Factory.StartNew(Render);
            _timer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(100)
                };
            _timer.Tick += UpdateScreen;
            _timer.Start();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _timer.Stop();
        }

        private void Init()
        {
            _width = (int) image1.Width;
            _height = (int) image1.Height;
            _rawStride = (_width*_pf.BitsPerPixel + 7)/8;
            _pixelData = new byte[_rawStride*_height];

            //
            _step = 0;

            //
            _scene = new Scene(6.67384e-11, 1e-9); // in N m² / kg²  or  m³ / (kg * s²)  http://en.wikipedia.org/wiki/Gravitational_constant
            //Body earth = _scene.AddMass(
            //    new Body
            //    {
            //        Name = "Earth",
            //        Mass = 5.97219e24,
            //        Position = new Vector3(0, 0, 0),
            //        Velocity = new Vector3(0, 0, 0),
            //    });
            //Body moon = _scene.AddMass(
            //    earth,
            //    new Body
            //    {
            //        Name = "Moon",
            //        Mass = 7.3477e22,
            //        Position = new Vector3(384399000, 0, 0),
            //        Velocity = new Vector3(0, 1022, 0),
            //    });
            Body sun = _scene.AddMass(new Body
                {
                    Name = "Sun",
                    Mass = 1.989e30,
                    Position = new Vector3(0, 0, 0),
                    Velocity = new Vector3(0, 0, 0),
                });
            Body mercury = _scene.AddMass(
                sun,
                new Body
                {
                    Name = "Mercury",
                    Mass = 0.33022e24,
                    Position = new Vector3(57909100000, 0, 0),
                    Velocity = new Vector3(0, 47872.5, 0),
                });
            Body venus = _scene.AddMass(
                sun,
                new Body
                    {
                        Name = "Venus",
                        Mass = 4.869e24,
                        Position = new Vector3(108208930000, 0, 0),
                        Velocity = new Vector3(0, 35021.4, 0),
                    });
            Body earth = _scene.AddMass(
                sun,
                new Body
                    {
                        Name = "Earth",
                        Mass = 5.97219e24,
                        Position = new Vector3(149597890000, 0, 0),
                        Velocity = new Vector3(0, 29785.9, 0),
                    });
            Body moon = _scene.AddMass(
                earth,
                new Body
                    {
                        Name = "Moon",
                        Mass = 7.3477e22,
                        Position = new Vector3(384399000, 0, 0),
                        Velocity = new Vector3(0, 1022, 0),
                    });
            Body mars = _scene.AddMass(
                sun,
                new Body
                    {
                        Name = "Mars",
                        Mass = 0.64191e24,
                        Position = new Vector3(227939100000, 0, 0),
                        Velocity = new Vector3(0, 24130.9, 0),
                    }
                );
        }

        private static void SetPixel(int x, int y, Color c, byte[] buffer, int rawStride)
        {
            int xIndex = x*3;
            int yIndex = y*rawStride;
            buffer[xIndex + yIndex] = c.R;
            buffer[xIndex + yIndex + 1] = c.G;
            buffer[xIndex + yIndex + 2] = c.B;
        }

        private void UpdateScreen(object o, EventArgs e)
        {
            _bitmap = BitmapSource.Create(_width, _height, 96, 96, _pf, null, _pixelData, _rawStride);
            image1.Source = _bitmap;
        }

        private void Render()
        {
            while (true)
            {
                _scene.SimulateVerlet(_step, Increment);
                //_scene.SimulateEuler(_step, Increment);

                //for (int y = 0; y < _height; y++)
                //{
                //    int yIndex = y * _rawStride;
                //    for (int x = 0; x < _rawStride; x += 3)
                //    {
                //        _pixelData[x + yIndex] = 0;
                //        _pixelData[x + yIndex + 1] = 0;
                //        _pixelData[x + yIndex + 2] = 255;
                //    }
                //}
                int i = 0;
                foreach (Body body in _scene.Masses)
                {
                    // minX, minY   -> 0, 0
                    // 0, 0         -> width/2, height/2
                    // maxX, maxY   -> width, height
                    int x = (int) (body.Position.X + _width/2);
                    int y = (int) (body.Position.Y + _height/2);
                    Color color = Colors.Fuchsia;
                    switch(i)
                    {
                        case 0:
                            color = Colors.Yellow;
                            break;
                        case 1:
                            color = Colors.Orange;
                            break;
                        case 2:
                            color = Colors.Green;
                            break;
                        case 3:
                            color = Colors.Aqua;
                            break;
                        case 4:
                            color = Colors.White;
                            break;
                        case 5:
                            color = Colors.Red;
                            break;
                    }
                    SetPixel(x, y, color, _pixelData, _rawStride);
                    i++;
                }
                _step++;
            }
        }
    }
}