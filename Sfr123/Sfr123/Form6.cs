using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sfr123
{
    public partial class Form6 : Form
    {
        private TrackBar sizeTrackBar;
        private TrackBar densityTrackBar;
        private TrackBar spotsCountTrackBar;
        private TrackBar lineCountTrackBar;
        private Button colorButton;
        private Button sizeToggleButton;
        private Button modeButton;
        private Button backButton; // кн назад
        private Panel drawingPanel;

        private Font commonFont = new Font("Bookman Old Style", 8.75f);
       

        private Color gridColor = Color.Green;
        private bool isGreenMode = true; // летний/зимн режим

        private int cellSize = 20;
        private int gridDensity = 10;
        private int spotsCount = 50;
        private int lineCount = 100; // колво линий

        private int gridWidthPixels = 600; //изнач квадрат
        private int gridHeightPixels = 500;

        private Random rand = new Random();

        private List<(Point start, Point control1, Point control2, Point end, bool isHorizontal)> curvedLines = new List<(Point, Point, Point, Point, bool)>();

        public Form6()
        {
            InitializeComponent();

            // Настр формы
            this.Text = "Камуфляжная сетка";
            this.Size = new Size(900, 750);

            // Панель рис
            drawingPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };
            drawingPanel.Paint += DrawingPanel_Paint;
            this.Controls.Add(drawingPanel);

            // упр
            Panel controlPanel = new Panel
            {
                Dock = DockStyle.Right,
                Width = 240,
                Padding = new Padding(10)
            };
            this.Controls.Add(controlPanel);

            // шрифт для всех
            Font controlFont = commonFont;

            // размер квадратиков
            controlPanel.Controls.Add(new Label { Text = "Размер ячейки", Top = 10, Left = 10, Font = controlFont });
            sizeTrackBar = new TrackBar
            {
                Minimum = 3,
                Maximum = 20,
                Value = cellSize,
                Top = 30,
                Left = 10,
                Width = 180
            };
            sizeTrackBar.Scroll += (s, e) =>
            {
                cellSize = sizeTrackBar.Value;
                drawingPanel.Invalidate();
            };
            controlPanel.Controls.Add(sizeTrackBar);

            // плотность
            controlPanel.Controls.Add(new Label { Text = "Плотность линий", Top = 80, Left = 10, Font = controlFont });
            densityTrackBar = new TrackBar
            {
                Minimum = 5,
                Maximum = 20,
                Value = gridDensity,
                Top = 100,
                Left = 10,
                Width = 180
            };
            densityTrackBar.Scroll += (s, e) =>
            {
                gridDensity = densityTrackBar.Value;
                drawingPanel.Invalidate();
            };
            controlPanel.Controls.Add(densityTrackBar);

            //пятна
            controlPanel.Controls.Add(new Label { Text = "Кол-во пятен", Top = 150, Left = 10, Font = controlFont });
            spotsCountTrackBar = new TrackBar
            {
                Minimum = 49,
                Maximum = 2000,
                Value = spotsCount,
                Top = 170,
                Left = 10,
                Width = 180
            };
            spotsCountTrackBar.Scroll += (s, e) =>
            {
                spotsCount = spotsCountTrackBar.Value;
                drawingPanel.Invalidate();
            };
            controlPanel.Controls.Add(spotsCountTrackBar);

            // линии
            controlPanel.Controls.Add(new Label { Text = "Кол-во линий", Top = 220, Left = 10, Font = controlFont });
            lineCountTrackBar = new TrackBar
            {
                Minimum = 50,
                Maximum = 500,
                Value = lineCount,
                Top = 240,
                Left = 10,
                Width = 180
            };
            lineCountTrackBar.Scroll += (s, e) =>
            {
                lineCount = lineCountTrackBar.Value;
                GenerateCurvedLines();
                drawingPanel.Invalidate();
            };
            controlPanel.Controls.Add(lineCountTrackBar);



            //кнопочки
            // выбор цвет линий
            colorButton = new Button
            {
                Text = "Цвет линий",
                Top = 290,
                Left = 10,
                Width = 180,
                Height = 35,
                Font = controlFont
            };
            colorButton.Click += (s, e) =>
            {
                using (ColorDialog colorDialog = new ColorDialog())
                {
                    if (colorDialog.ShowDialog() == DialogResult.OK)
                    {
                        gridColor = colorDialog.Color;
                        drawingPanel.Invalidate();
                    }
                }
            };
            controlPanel.Controls.Add(colorButton);

            //  размер перекл
            sizeToggleButton = new Button
            {
                Text = "Размер: 500x300",
                Top = 330,
                Left = 10,
                Width = 180,
                Height = 35,
                Font = controlFont
            };
            sizeToggleButton.Click += (s, e) =>
            {
                if (gridWidthPixels == 600 && gridHeightPixels == 500)
                {
                    gridWidthPixels = 500;
                    gridHeightPixels = 300;
                    sizeToggleButton.Text = "Размер: 500x300";
                }
                else
                {
                    gridWidthPixels = 600;
                    gridHeightPixels = 500;
                    sizeToggleButton.Text = "Размер: 600x500";
                }
                GenerateCurvedLines();
                drawingPanel.Invalidate();
            };
            controlPanel.Controls.Add(sizeToggleButton);

            //  окрас зел и светлый
            modeButton = new Button
            {
                Text = "Окрас: летний",
                Top = 370,
                Left = 10,
                Width = 180,
                Height = 35,
                Font = controlFont
            };
            modeButton.Click += (s, e) =>
            {
                isGreenMode = !isGreenMode;
                modeButton.Text = isGreenMode ? "Окрас: летний" : "Окрас: зимний";
                GenerateCurvedLines();
                drawingPanel.Invalidate();
            };
            controlPanel.Controls.Add(modeButton);


            //кн назад
            backButton = new Button
            {
                Text = "Вернуться",
                Top = controlPanel.Height - 100,
                Left = 10,
                Width = 180,
                Height = 35,
                Font = controlFont
            };
            backButton.Click += (s, e) =>
            {
                Form5 f5 = new Form5();
                f5.Show();
                this.Hide();
            };
            controlPanel.Controls.Add(backButton);

            // Изнач делаем кр линии
            GenerateCurvedLines();
        }

        private void GenerateCurvedLines()
        {
            curvedLines.Clear();
            int totalWidth = gridWidthPixels;
            int totalHeight = gridHeightPixels;
            int maxLength = 70; // макс длин
            for (int i = 0; i < lineCount; i++)
            {
                bool isHorizontal = rand.Next(2) == 0;
                if (isHorizontal)
                {
                    int xStart = rand.Next(0, totalWidth - maxLength);
                    int y = rand.Next(0, totalHeight);
                    Point start = new Point(xStart, y);
                    Point control1 = new Point(xStart + rand.Next(5, 20), y + rand.Next(-10, 10));
                    Point control2 = new Point(xStart + rand.Next(20, 40), y + rand.Next(-10, 10));
                    Point end = new Point(xStart + rand.Next(10, maxLength), y + rand.Next(-5, 5));
                    curvedLines.Add((start, control1, control2, end, true));
                }
                else
                {
                    int x = rand.Next(0, totalWidth);
                    int yStart = rand.Next(0, totalHeight - maxLength);
                    Point start = new Point(x, yStart);
                    Point control1 = new Point(x + rand.Next(-10, 10), yStart + rand.Next(5, 20));
                    Point control2 = new Point(x + rand.Next(-10, 10), yStart + rand.Next(20, 40));
                    Point end = new Point(x + rand.Next(-5, 5), yStart + rand.Next(10, maxLength));
                    curvedLines.Add((start, control1, control2, end, false));
                }
            }
        }

        private void DrawingPanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            if (isGreenMode)
            {
                g.Clear(Color.FromArgb(240, 240, 240));
            }
            else
            {
                g.Clear(Color.White);
            }

            int size = cellSize;
            int totalWidth = gridWidthPixels;
            int totalHeight = gridHeightPixels;

            // Рис сетку
            Color fillColor = isGreenMode ? Color.FromArgb(94, 103, 55) : Color.FromArgb(245, 245, 220);
            using (Brush fillBrush = new SolidBrush(fillColor))
            {
                g.FillRectangle(fillBrush, 0, 0, totalWidth, totalHeight);
            }

            using (Pen gridPen = new Pen(gridColor))
            {
                int rows = totalHeight / size;
                for (int i = 0; i <= rows; i++)
                {
                    int y = i * size;
                    if (i % gridDensity == 0)
                        g.DrawLine(gridPen, 0, y, totalWidth, y);
                }

                int cols = totalWidth / size;
                for (int j = 0; j <= cols; j++)
                {
                    int x = j * size;
                    if (j % gridDensity == 0)
                        g.DrawLine(gridPen, x, 0, x, totalHeight);
                }
            }

            // Пятна 
            for (int i = 0; i < spotsCount; i++)
            {
                int x = rand.Next(0, totalWidth);
                int y = rand.Next(0, totalHeight);
                int spotSize = rand.Next(5, 20);

                Color spotColor;
                if (isGreenMode)
                {
                    spotColor = rand.NextDouble() > 0.5 ? Color.FromArgb(69, 171, 65) : Color.FromArgb(139, 122, 51);
                }
                else
                {
                    spotColor = rand.NextDouble() > 0.5 ? Color.FromArgb(101, 121, 169) : Color.FromArgb(204, 238, 255);
                }

                using (Brush spotBrush = new SolidBrush(spotColor))
                {
                    g.FillEllipse(spotBrush, x, y, spotSize, spotSize);
                }
            }

            // линии
            using (Pen linePen = new Pen(Color.Black, 1))
            {
                foreach (var (start, control1, control2, end, isHorizontal) in curvedLines)
                {
                    if (isGreenMode)
                    {
                        linePen.Color = rand.NextDouble() > 0.5 ? Color.FromArgb(81, 166, 84) : Color.FromArgb(153, 153, 0);
                    }
                    else
                    {
                        linePen.Color = rand.NextDouble() > 0.5 ? Color.FromArgb(160, 160, 160) : Color.FromArgb(153, 204, 255);
                    }
                    g.DrawBezier(linePen, start, control1, control2, end);
                }
            }
        }

        private void Form6_Load(object sender, EventArgs e)
        {

        }
    }
}
