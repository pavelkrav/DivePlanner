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

namespace DivePlanner
{
	public enum ClickMethods { DeletePoint, PointInfo, AddPoint }
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private Dive _dive;

		private List<Canvas> _pointCanvasList;
		private int SelectedPoint { get; set; }
		private ClickMethods ClickMethod { get; set; }

		private double TimeBeadLength { get; set; }
		private double DepthBeadLength { get; set; }

		public MainWindow()
		{
			InitializeComponent();
		}

		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);
			this.Width = SystemParameters.PrimaryScreenWidth / 2;
			this.Height = SystemParameters.PrimaryScreenHeight / 2;

			InfoGrid.Background = Brushes.DarkGray;
			MenuGrid.Background = Brushes.DarkGray;
			WindowGrid.Background = Brushes.WhiteSmoke;

			InfoGrid.Visibility = Visibility.Hidden;
			MenuGrid.Visibility = Visibility.Visible;

			InfoButton.Content = "◀";
			MenuButton.Content = "◀";

			_dive = new Dive();
			_pointCanvasList = new List<Canvas>();
			ClickMethod = ClickMethods.PointInfo;
			SelectedPoint = -1;

			DrawWindow();

		}

		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
		{
			base.OnRenderSizeChanged(sizeInfo);			
			DrawWindow();
		}

		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			base.OnMouseDown(e);
			if (ClickMethod == ClickMethods.AddPoint)
			{
				double time = GetTimeCoordinate();
				double depth = GetDepthCoordinate();
				if (time >= 0 && depth >= 0)
				{
					_dive.DivePoints.Add(new DivePoint(time, depth));
					_dive.OrderDivePoints();
					DrawGraphGrid();
				}
			}
		}

		private double GetTimeCoordinate()
		{
			double positiveField = TimeBeadLength * 10;

			double preX = Mouse.GetPosition(GraphGrid).X;
			if (preX < 0)
				return -1;

			if (preX < DepthAxisIndent() - 4)
				return -1;
			else if (preX < DepthAxisIndent())
				return 0;

			if (preX > DepthAxisIndent() + positiveField + 6)
				return -1;
			else if (preX > DepthAxisIndent() + positiveField)
				return _dive.TimeLength;

			double X = preX - DepthAxisIndent();

			return X / positiveField * _dive.TimeLength;
		}

		private double GetDepthCoordinate()
		{
			double positiveField = DepthBeadLength * 10;

			double preY = Mouse.GetPosition(GraphGrid).Y;
			if (preY < 0)
				return -1;

			if (preY < TimeAxisIndent() - 4)
				return -1;
			else if (preY < TimeAxisIndent())
				return 0;

			if (preY > TimeAxisIndent() + positiveField + 6)
				return -1;
			else if (preY > TimeAxisIndent() + positiveField)
				return _dive.MaxDepth;

			double Y = preY - TimeAxisIndent();

			return Y / positiveField * _dive.MaxDepth;
		}

		private void DrawWindow()
		{
			WindowGrid.VerticalAlignment = VerticalAlignment.Stretch;
			WindowGrid.HorizontalAlignment = HorizontalAlignment.Stretch;
			WindowGrid.Height = Height - 39;
			WindowGrid.Width = Width - 16;

			DrawInfoGrid();

			DrawMenuGrid();

			DrawGraphGrid();
		}

		private void DrawInfoGrid()
		{
			InfoGrid.Width = WindowGrid.Width / 5;
			InfoGrid.HorizontalAlignment = HorizontalAlignment.Right;
			InfoGrid.VerticalAlignment = VerticalAlignment.Stretch;
		}

		private void DrawMenuGrid()
		{
			MenuGrid.Width = WindowGrid.Width / 5;
			MenuGrid.HorizontalAlignment = HorizontalAlignment.Left;
			MenuGrid.VerticalAlignment = VerticalAlignment.Stretch;
			MenuGrid.Height = WindowGrid.Height;

			double buttonsLeftIndent = MenuGrid.Width * 0.15 / 2;
			double buttonsHeight = 25;
			double buttonsTopIndent = 15;

			PointInfoButton.Width = MenuGrid.Width - buttonsLeftIndent * 2;
			PointInfoButton.Height = buttonsHeight;
			PointInfoButton.HorizontalAlignment = HorizontalAlignment.Center;
			PointInfoButton.Margin = new Thickness(buttonsLeftIndent, buttonsTopIndent, buttonsLeftIndent, MenuGrid.Height - (buttonsTopIndent + buttonsHeight) * 1);
			PointInfoButton.Background = Brushes.LightGray;

			AddPointButton.Width = MenuGrid.Width * 0.85;
			AddPointButton.Height = 25;
			AddPointButton.HorizontalAlignment = HorizontalAlignment.Center;
			AddPointButton.Margin = new Thickness(buttonsLeftIndent, buttonsTopIndent * 2 + buttonsHeight, buttonsLeftIndent, MenuGrid.Height - (buttonsTopIndent + buttonsHeight) * 2);
			AddPointButton.Background = Brushes.LightGray;

			DeletePointButton.Width = MenuGrid.Width * 0.85;
			DeletePointButton.Height = 25;
			DeletePointButton.HorizontalAlignment = HorizontalAlignment.Center;
			DeletePointButton.Margin = new Thickness(buttonsLeftIndent, buttonsTopIndent * 3 + buttonsHeight * 2, buttonsLeftIndent, MenuGrid.Height - (buttonsTopIndent + buttonsHeight) * 3);
			DeletePointButton.Background = Brushes.LightGray;

			if (ClickMethod == ClickMethods.PointInfo)
			{
				PointInfoButton.Background = MenuGrid.Background;
				PointInfoButton.BorderThickness = new Thickness(1);
				PointInfoButton.BorderBrush = Brushes.LightGray;
			}
			else if (ClickMethod == ClickMethods.AddPoint)
			{
				AddPointButton.Background = MenuGrid.Background;
				AddPointButton.BorderThickness = new Thickness(1);
				AddPointButton.BorderBrush = Brushes.LightGray;
			}
			else if (ClickMethod == ClickMethods.DeletePoint)
			{
				DeletePointButton.Background = MenuGrid.Background;
				DeletePointButton.BorderThickness = new Thickness(1);
				DeletePointButton.BorderBrush = Brushes.LightGray;
			}
		}

		private void DrawGraphGrid()
		{
			GraphGrid.Background = WindowGrid.Background;
			GraphGrid.VerticalAlignment = VerticalAlignment.Center;
			GraphGrid.Height = WindowGrid.Height;

			GraphGrid.Children.Clear();
			GraphGrid.Children.Add(MenuButton);
			GraphGrid.Children.Add(InfoButton);

			MenuButton.Height = WindowGrid.Height / 15;
			MenuButton.Width = MenuButton.Height;

			InfoButton.Height = WindowGrid.Height / 15;
			InfoButton.Width = InfoButton.Height;

			if (MenuGrid.IsVisible && InfoGrid.IsVisible)
			{
				GraphGrid.HorizontalAlignment = HorizontalAlignment.Center;
				GraphGrid.Width = WindowGrid.Width - InfoGrid.Width - MenuGrid.Width;
			}
			else if (MenuGrid.IsVisible)
			{
				GraphGrid.HorizontalAlignment = HorizontalAlignment.Right;
				GraphGrid.Width = WindowGrid.Width - InfoGrid.Width;
			}
			else if (InfoGrid.IsVisible)
			{
				GraphGrid.HorizontalAlignment = HorizontalAlignment.Left;
				GraphGrid.Width = WindowGrid.Width - MenuGrid.Width;
			}
			else
			{
				GraphGrid.HorizontalAlignment = HorizontalAlignment.Center;
				GraphGrid.Width = WindowGrid.Width;
			}

			MenuButton.HorizontalAlignment = HorizontalAlignment.Left;
			MenuButton.VerticalAlignment = VerticalAlignment.Top;
			InfoButton.HorizontalAlignment = HorizontalAlignment.Right;
			InfoButton.VerticalAlignment = VerticalAlignment.Top;

			DrawGraphVectors();

			if (_dive.DivePoints.Count > 0)
				DrawGraphic();
			else SelectedPoint = -1;

		}

		private void DrawGraphic()
		{
			DivePoint previous = _dive.DivePoints[0];
			for (int i = 1; i < _dive.DivePoints.Count; i++)
			{
				DrawLine(previous, _dive.DivePoints[i]);
				previous = _dive.DivePoints[i];
			}

			DrawDivePoints();
		}

		private void DrawLine(DivePoint p1, DivePoint p2)
		{
			Line twoPointLine = new Line();
			twoPointLine.X1 = DepthAxisIndent() + p1.Time / (_dive.TimeLength / 10) * TimeBeadLength;
			twoPointLine.Y1 = TimeAxisIndent() + p1.Depth / (_dive.MaxDepth / 10) * DepthBeadLength;
			twoPointLine.X2 = DepthAxisIndent() + p2.Time / (_dive.TimeLength / 10) * TimeBeadLength;
			twoPointLine.Y2 = TimeAxisIndent() + p2.Depth / (_dive.MaxDepth / 10) * DepthBeadLength;
			twoPointLine.Stroke = Brushes.Red;
			twoPointLine.StrokeThickness = 1;
			GraphGrid.Children.Add(twoPointLine);
		}

		private void DrawDivePoints()
		{
			_pointCanvasList.Clear();
			int indexer = 0;
			foreach (DivePoint point in _dive.DivePoints)
			{
				Ellipse pointEllipse = new Ellipse() { Width = 9, Height = 9, Fill = Brushes.Red, Stroke = Brushes.Black };
				_pointCanvasList.Add(new Canvas() { Width = 9, Height = 9, Focusable = true });
				if (indexer == SelectedPoint)
				{
					pointEllipse.Width = 12;
					pointEllipse.Height = 12;
					pointEllipse.Fill = Brushes.Yellow;
					_pointCanvasList[indexer].Width = 12;
					_pointCanvasList[indexer].Height = 12;
				}
				_pointCanvasList[indexer].Children.Add(pointEllipse);
				double timePixels = DepthAxisIndent() + point.Time / (_dive.TimeLength / 10) * TimeBeadLength - _pointCanvasList[indexer].Width / 2;
				double depthPixels = TimeAxisIndent() + point.Depth / (_dive.MaxDepth / 10) * DepthBeadLength - _pointCanvasList[indexer].Height / 2;
				_pointCanvasList[indexer].Margin = new Thickness(timePixels, depthPixels, GraphGrid.Width - timePixels, GraphGrid.Height - depthPixels);
				GraphGrid.Children.Add(_pointCanvasList[indexer]);

				_pointCanvasList[indexer].MouseDown += new MouseButtonEventHandler((s, e) => OnPointCanvasClick(s, e, point.Time, point.Depth));

				indexer++;
			}
		}

		private void OnPointCanvasClick(object sender, MouseButtonEventArgs e, double time, double depth)
		{
			if (ClickMethod == ClickMethods.DeletePoint)
			{
				_dive.RemovePoint(time, depth);
				DrawGraphGrid();
			}
			else if (ClickMethod == ClickMethods.PointInfo)
			{
				if (SelectedPoint != _dive.GetListNumber(time, depth))
				{
					SelectedPoint = _dive.GetListNumber(time, depth);
					if (!InfoGrid.IsVisible)
					{
						OnInfoButtonClick(this, new RoutedEventArgs());
					}
				}
				else SelectedPoint = -1;
				DrawGraphGrid();				
			}
		}

		private double DepthAxisIndent()
		{
			return MenuButton.Width + 20;
		}

		private double TimeAxisIndent()
		{
			return MenuButton.Height + 5;
		}

		private void DrawGraphVectors()
		{
			Line timeAxis = new Line();
			timeAxis.X1 = MenuButton.Width + 10;
			timeAxis.Y1 = TimeAxisIndent();
			timeAxis.X2 = GraphGrid.Width - timeAxis.X1;
			timeAxis.Y2 = timeAxis.Y1;
			timeAxis.Stroke = Brushes.DarkGray;
			timeAxis.StrokeThickness = 2;
			GraphGrid.Children.Add(timeAxis);

			Line depthAxis = new Line();
			depthAxis.X1 = DepthAxisIndent();
			depthAxis.Y1 = timeAxis.Y1 - 10;
			depthAxis.X2 = depthAxis.X1;
			depthAxis.Y2 = GraphGrid.Height - depthAxis.Y1;
			depthAxis.Stroke = Brushes.DarkGray;
			depthAxis.StrokeThickness = 2;
			GraphGrid.Children.Add(depthAxis);

			Polygon timeArrow = new Polygon();
			timeArrow.Stroke = Brushes.DarkGray;
			timeArrow.Fill = Brushes.DarkGray;
			PointCollection timePoints = new PointCollection();
			timePoints.Add(new Point(timeAxis.X2 - 5, timeAxis.Y2 - 3));
			timePoints.Add(new Point(timeAxis.X2 - 5, timeAxis.Y2 + 3));
			timePoints.Add(new Point(timeAxis.X2 + 5, timeAxis.Y2));
			timeArrow.Points = timePoints;
			GraphGrid.Children.Add(timeArrow);

			Polygon depthArrow = new Polygon();
			depthArrow.Stroke = Brushes.DarkGray;
			depthArrow.Fill = Brushes.DarkGray;
			PointCollection depthPoints = new PointCollection();
			depthPoints.Add(new Point(depthAxis.X2 - 3, depthAxis.Y2 - 5));
			depthPoints.Add(new Point(depthAxis.X2 + 3, depthAxis.Y2 - 5));
			depthPoints.Add(new Point(depthAxis.X2, depthAxis.Y2 + 5));
			depthArrow.Points = depthPoints;
			GraphGrid.Children.Add(depthArrow);

			for (int i = 1; i <= 10; i++)
			{
				Line timeBead = new Line();
				timeBead.X1 = (timeAxis.X2 - DepthAxisIndent()) / 10 * i + DepthAxisIndent();
				timeBead.Y1 = TimeAxisIndent() - 3;
				timeBead.X2 = timeBead.X1;
				timeBead.Y2 = TimeAxisIndent() + 3;
				timeBead.Stroke = Brushes.DarkGray;
				timeBead.StrokeThickness = 1;
				if (i != 10)
					GraphGrid.Children.Add(timeBead);

				Canvas textCanvas = new Canvas() { Focusable = false};
				TextBox text = new TextBox()
				{
					Foreground = Brushes.DarkGray,
					Background = GraphGrid.Background,
					BorderBrush = null,
					BorderThickness = new Thickness(0),
					FontSize = 12,
					VerticalContentAlignment = VerticalAlignment.Center,
					HorizontalContentAlignment = HorizontalAlignment.Center,
					IsReadOnly = true,
					Cursor = Cursors.Arrow,
					Focusable = false
				};
				//int hours = (int)Math.Round(_dive.Length / 10 * i) / 3600;
				//int minutes = (int)Math.Round(_dive.Length / 10 * i) % 3600 / 60;
				//text.Text = $"{hours}:{minutes}";
				if (i != 10)
					text.Text = Math.Round(_dive.TimeLength / 10 * i).ToString("F0");
				else
					text.Text = "t, sec";
				text.FontWeight = FontWeights.Bold;
				textCanvas.Children.Add(text);
				textCanvas.Width = text.Text.Length * text.FontSize / 2;
				textCanvas.Height = text.FontSize;
				double left = timeBead.X1 - textCanvas.Width / 3 * 2 - 1;
				double top = timeBead.Y1 - 6 - textCanvas.Height;
				textCanvas.Margin = new Thickness(left, top, GraphGrid.Width - left, GraphGrid.Height - top);
				GraphGrid.Children.Add(textCanvas);
			}

			for (int i = 1; i <= 10; i++)
			{
				Line depthBead = new Line();
				depthBead.X1 = DepthAxisIndent() - 3;
				depthBead.Y1 = (depthAxis.Y2 - TimeAxisIndent()) / 10 * i + TimeAxisIndent();
				depthBead.X2 = DepthAxisIndent() + 3;
				depthBead.Y2 = depthBead.Y1;
				depthBead.Stroke = Brushes.DarkGray;
				depthBead.StrokeThickness = 1;
				if (i != 10)
					GraphGrid.Children.Add(depthBead);

				Canvas textCanvas = new Canvas() { Focusable = false };
				TextBox text = new TextBox()
				{
					Foreground = Brushes.DarkGray,
					Background = GraphGrid.Background,
					BorderBrush = null,
					BorderThickness = new Thickness(0),
					FontSize = 12,
					VerticalContentAlignment = VerticalAlignment.Center,
					HorizontalContentAlignment = HorizontalAlignment.Center,
					IsReadOnly = true,
					Cursor = Cursors.Arrow,
					Focusable = false
				};
				if (i != 10)
					text.Text = Math.Round(_dive.MaxDepth / 10 * i).ToString("F0");
				else
					text.Text = "h, m";
				text.FontWeight = FontWeights.Bold;
				textCanvas.Children.Add(text);
				textCanvas.Width = text.Text.Length * text.FontSize / 2;
				textCanvas.Height = text.FontSize;
				double left = depthBead.X1 - textCanvas.Width - 6;
				double top = depthBead.Y1 - textCanvas.Height / 2 - 3;
				textCanvas.Margin = new Thickness(left, top, GraphGrid.Width - left, GraphGrid.Height - top);
				GraphGrid.Children.Add(textCanvas);
			}

			TimeBeadLength = (timeAxis.X2 - DepthAxisIndent()) / 10;
			DepthBeadLength = (depthAxis.Y2 - TimeAxisIndent()) / 10;
		}

		private void OnMenuButtonClick(object sender, RoutedEventArgs e)
		{
			if (MenuGrid.IsVisible)
			{
				MenuGrid.Visibility = Visibility.Hidden;
				MenuButton.Content = "▶";
			}
			else
			{
				MenuGrid.Visibility = Visibility.Visible;
				MenuButton.Content = "◀";
				if (InfoGrid.IsVisible)
				{
					OnInfoButtonClick(this, new RoutedEventArgs());
				}
			}
			DrawGraphGrid();
		}

		private void OnInfoButtonClick(object sender, RoutedEventArgs e)
		{
			if (InfoGrid.IsVisible)
			{
				InfoGrid.Visibility = Visibility.Hidden;
				InfoButton.Content = "◀";
			}
			else
			{
				InfoGrid.Visibility = Visibility.Visible;
				InfoButton.Content = "▶";
				if (MenuGrid.IsVisible)
				{
					OnMenuButtonClick(this, new RoutedEventArgs());
				}
			}
			DrawGraphGrid();
		}

		private void OnPointInfoButtonClick(object sender, RoutedEventArgs e)
		{
			ClickMethod = ClickMethods.PointInfo;
			SelectedPoint = -1;
			DrawWindow();
		}

		private void OnAddPointButtonClick(object sender, RoutedEventArgs e)
		{
			ClickMethod = ClickMethods.AddPoint;
			SelectedPoint = -1;
			DrawWindow();
		}

		private void OnDeletePointButtonClick(object sender, RoutedEventArgs e)
		{
			ClickMethod = ClickMethods.DeletePoint;
			SelectedPoint = -1;
			DrawWindow();
		}
	}
}
