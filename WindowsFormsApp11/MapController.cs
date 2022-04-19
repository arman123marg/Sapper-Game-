using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp11
{
    public class MapController
    {
        static int mapSize = 10;
        static int cellsSize = 50;
        static int bombsCount = 10;
        static int[,] map = new int[mapSize, mapSize];
        static Button[,] buttons = new Button[mapSize, mapSize];
        private static int currentPictureToSet = 0;
        public static Image ImageSet;
        public static bool isFirstClick;
        public static Point firstCoord;
        public static Form form;
        public static void init(Form current)
        {
            form = current;
            currentPictureToSet = 0;
            isFirstClick = true;
            string ImagePath = "Images\\all.png";
            ImageSet = new Bitmap(Path.Combine(new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent.FullName.ToString(), ImagePath));
            InitMap();
            initButtons(current);
            SeedMap();
            configureFormSize(current);
        }
        private static void InitMap()
        {
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    map[i, j] = 0;
                }
            }
        }
        static void configureFormSize(Form current)
        {
            current.Width = mapSize * cellsSize + 16;
            current.Height = mapSize * cellsSize + 40;
        }
        static void initButtons(Form current)
        {
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    Button btn = new Button();
                    btn.Location = new Point(i * cellsSize, j * cellsSize);
                    btn.Size = new Size(cellsSize, cellsSize);
                    // Assign an image to the button.


                    // Align the image and text on the button.
                    btn.ImageAlign = ContentAlignment.MiddleRight;
                    btn.TextAlign = ContentAlignment.MiddleLeft;
                    // Give the button a flat appearance.
                    btn.FlatStyle = FlatStyle.Popup;
                    //btn.Click += new EventHandler(OnLeftButtonPresed);
                    btn.MouseUp += new MouseEventHandler(onButtonPresedMouse);
                    btn.Image = FindNeededImage(0, 0);
                    current.Controls.Add(btn);
                    buttons[i, j] = btn;
                }
            }
        }

        private static void onButtonPresedMouse(object sender, MouseEventArgs e)
        {
            Button btn = sender as Button;
            switch (e.Button.ToString())
            {
                case "Right":
                    OnRightButtonPresed(btn);
                    break;
                case "Left":
                    OnLeftButtonPresed(btn);
                    break;
                default:
                    throw new Exception("you had pressed in vrong button");

            }
        }

        private static void OnRightButtonPresed(Button pressButton)
        {
            currentPictureToSet++;
            currentPictureToSet %= 3;
            switch (currentPictureToSet)
            {
                case 0:
                    pressButton.Image = FindNeededImage(0, 0);
                    break;
                case 1:
                    pressButton.Image = FindNeededImage(0, 2);
                    break;
                case 2:
                    pressButton.Image = FindNeededImage(2, 2);
                    break;
            }
        }

        private static void OnLeftButtonPresed(Button pressedButton)
        {
            pressedButton.Enabled = false;
            int iButton = pressedButton.Location.Y / cellsSize;
            int jButton = pressedButton.Location.X / cellsSize;
            if (isFirstClick)
            {
                firstCoord = new Point(jButton, iButton);
                SeedMap();
                CountCellBomb();
                isFirstClick = false;
            }
            OpenCells(jButton, iButton);
            if (map[jButton,iButton]==-1)
            {
                ShowAllBombs(iButton, jButton);
                MessageBox.Show("You Lose ");
                form.Controls.Clear();
                init(form);
            }
            Win();
        }
        private static void Win()
        {
            int counter = 0;
            for (int i = 0; i < buttons.GetLength(0); i++)
            {
                for (int j = 0; j < buttons.GetLength(1); j++)
                {
                    if (!buttons[i,j].Enabled)
                    {
                        counter++;
                    }
                }
            }
            if (counter+bombsCount>=buttons.Length)
            {

                MessageBox.Show("You Win ");
                form.Controls.Clear();
                init(form);
            }
        }
        private static void ShowAllBombs(int iBomb, int jBomb)
        {
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    if (i == iBomb && j == jBomb)
                        continue;
                    if (map[i, j] == -1)
                    {
                        buttons[i, j].Image = FindNeededImage(3, 2);
                    }
                }
            }
        }

        public static Image FindNeededImage(int xPos, int yPos)
        {
            Image image = new Bitmap(cellsSize, cellsSize);
            Graphics g = Graphics.FromImage(image);
            g.DrawImage(ImageSet, new Rectangle(new Point(0, 0), new Size(cellsSize, cellsSize)), 0 + 32 * xPos, 0 + 32 * yPos, 33, 33, GraphicsUnit.Pixel);


            return image;
        }
        public static void SeedMap()
        {
            InitMap();
            Random rnd = new Random();
            for (int i = 0; i < bombsCount; i++)
            {
                int Xpos = rnd.Next(0, mapSize);
                int Ypos = rnd.Next(0, mapSize);
                while (map[Xpos, Ypos] == -1 || (Math.Abs(Ypos - firstCoord.Y) <= 1 && Math.Abs(Xpos - firstCoord.X) <= 1))
                {
                    Xpos = rnd.Next(0, mapSize - 1);
                    Ypos = rnd.Next(0, mapSize - 1);
                }
                map[Xpos, Ypos] = -1;
            }

        }
        private static void CountCellBomb()
        {
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    if (map[i, j] == -1)
                    {
                        for (int k = i - 1; k < i + 2; k++)
                        {
                            for (int l = j - 1; l < j + 2; l++)
                            {
                                if (!IsInBorder(k, l) || map[k, l] == -1)
                                    continue;
                                map[k, l] = map[k, l] + 1;
                            }
                        }
                    }
                }
            }
        }
        private static void OpenCell(int i, int j)
        {
            buttons[i, j].Enabled = false;

            switch (map[i, j])
            {
                case 1:
                    buttons[i, j].Image = FindNeededImage(1, 0);
                    break;
                case 2:
                    buttons[i, j].Image = FindNeededImage(2, 0);
                    break;
                case 3:
                    buttons[i, j].Image = FindNeededImage(3, 0);
                    break;
                case 4:
                    buttons[i, j].Image = FindNeededImage(4, 0);
                    break;
                case 5:
                    buttons[i, j].Image = FindNeededImage(0, 1);
                    break;
                case 6:
                    buttons[i, j].Image = FindNeededImage(1, 1);
                    break;
                case 7:
                    buttons[i, j].Image = FindNeededImage(2, 1);
                    break;
                case 8:
                    buttons[i, j].Image = FindNeededImage(3, 1);
                    break;
                case -1:
                    buttons[i, j].Image = FindNeededImage(1, 2);
                    break;
                case 0:
                    buttons[i, j].Image = FindNeededImage(0, 0);
                    break;
            }
        }
        private static void OpenCells(int i, int j)
        {
            OpenCell(i, j);

            if (map[i, j] > 0)
                return;

            for (int k = i - 1; k < i + 2; k++)
            {
                for (int l = j - 1; l < j + 2; l++)
                {
                    if (!IsInBorder(k, l))
                        continue;
                    if (!buttons[k, l].Enabled)
                        continue;
                    if (map[k, l] == 0)
                        OpenCells(k, l);
                    else if (map[k, l] > 0)
                        OpenCell(k, l);
                }
            }
        }
        private static bool IsInBorder(int i, int j)
        {
            if (i < 0 || j < 0 || i > mapSize - 1 || j > mapSize - 1)
            {
                return false;
            }
            return true;
        }
    }
}
