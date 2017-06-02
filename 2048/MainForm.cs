﻿#define SettingsFromFile

using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using Game_2048;
using G_2048 = Game_2048._2048;

namespace _2048
{
    public delegate void OptionsEventHandler();

    public partial class MainForm : Form
    {
        public event OptionsEventHandler OptionsEvent;

        private int score;
        private int bestScore;
        private int minValue = 2;
        private int matrixRows = 4;
        private int matrixCells = 4;
        private int intervalBetweenTiles = 10;
        private int borderInterval = 10;
        private bool gameOver = true;
        private bool lockOn = false;
        private bool ellipseTile = false;
        /*****/
        private Button[] tiles;
        /*****/
        private G_2048 game;
        private Size tileSize = new Size(60, 60);
        private Size normalFormSize = new Size(323, 466);
        private Dictionary<int, Color> colors;   // Цвета плиток.
        private Dictionary<string, int> records; // Рекорды для каждого размера поля.

        public MainForm()
        {
            InitializeComponent();
        }
        public MainForm(int _matrixRows, int _matrixCells, Size _tileSize, int _intervalBetweenTiles, int _borderInterval, bool _ellipseTile, Color backColor)
        {
            if (_matrixRows < 2 || _matrixCells < 2 || _tileSize.Width < 1 || _tileSize.Height < 1 || _intervalBetweenTiles < 0 || _borderInterval < 0)
                return;
            matrixRows = _matrixRows;
            matrixCells = _matrixCells;
            intervalBetweenTiles = _intervalBetweenTiles;
            borderInterval = _borderInterval;
            ellipseTile = _ellipseTile;
            tileSize = new Size(_tileSize.Width, _tileSize.Height);

            InitializeComponent();
            this.BackColor = backColor;
            /*Подгон размеров всех элементов*/
            // Панель с матрицей.
            pMatrix.Size = new Size(matrixCells * tileSize.Width + (intervalBetweenTiles * (matrixCells + 1)), matrixRows * tileSize.Height + (intervalBetweenTiles * (matrixRows + 1)));
            pMatrix.Location = new Point(borderInterval, borderInterval);

            // Создание плиток.
            CreateMatrix();

            // Предполагаемые размеры формы.
            Size newFormSize = new Size();
            int supposedFormWidth = borderInterval * 2 + pMatrix.Size.Width;
            int supposedFormHeight =  pMenu.Size.Height + (borderInterval * 2 + pMatrix.Size.Height);
            newFormSize.Width = normalFormSize.Width < supposedFormWidth ?
                supposedFormWidth : normalFormSize.Width;
            newFormSize.Height = normalFormSize.Height < supposedFormHeight ?
                supposedFormHeight : normalFormSize.Height;

            this.ClientSize = new Size(newFormSize.Width, newFormSize.Height);
            pMatrix.Location = new Point(pField.Size.Width / 2 - pMatrix.Size.Width / 2, pField.Size.Height / 2 - pMatrix.Size.Height / 2);
        }

        #region Методы
        // Создание плиток.
        private void CreateMatrix()
        {
            tiles = ControlsGenerator.CreateButtons(matrixRows * matrixCells, tileSize, new Font("Microsoft Sans Serif", 20, FontStyle.Bold), Color.WhiteSmoke);

            Point location = new Point(intervalBetweenTiles, intervalBetweenTiles);
            for (int i = 0; i < matrixRows; i++)
            {
                for (int j = 0; j < matrixCells; j++)
                {
                    tiles[matrixCells * i + j].Location = location;
                    tiles[matrixCells * i + j].Name = "bM" + i + j;
                    tiles[matrixCells * i + j].FlatStyle = FlatStyle.Flat;
                    tiles[matrixCells * i + j].Enabled = false;
                    tiles[matrixCells * i + j].FlatAppearance.BorderSize = 0;
                    if (ellipseTile)
                    {
                        GraphicsPath myPath = new GraphicsPath();
                        myPath.AddEllipse(0, 0, tiles[matrixCells * i + j].Width, tiles[matrixCells * i + j].Height);
                        Region myRegion = new Region(myPath);
                        tiles[matrixCells * i + j].Region = myRegion;
                    }
                    this.pMatrix.Controls.Add(tiles[matrixCells * i + j]);
                    location.X += tileSize.Width + intervalBetweenTiles;
                }
                location.X = intervalBetweenTiles;
                location.Y += tileSize.Height + intervalBetweenTiles;
            }
        }
        // Отображение матрицы на экране. Оптмизированная версия.
        private void ShowMatrix(int[,] oldMatrix)
        {
            int[,] matrix = game.GetMatrix();
            Random random = new Random();
            if (oldMatrix == null || oldMatrix.Length != matrix.Length) return;
            for (int i = 0; i < matrixRows; i++)
            {
                for (int j = 0; j < matrixCells; j++)
                {
                    if (matrix[i, j] == oldMatrix[i, j]) continue;
                    if (matrix[i, j] == 0)
                    {
                        tiles[matrixCells * i + j].Text = "";
                        tiles[matrixCells * i + j].BackColor = Color.WhiteSmoke;
                        tiles[matrixCells * i + j].Font = new Font("Microsoft Sans Serif", 20, FontStyle.Bold);
                        continue;
                    }
                    else
                        tiles[matrixCells * i + j].Text = (matrix[i, j]).ToString();

                    // Установка цвета плитки.
                    int power = 0;
                    for (int number = minValue; number < matrix[i, j]; number += number, power++)
                    { }
                    if (colors.ContainsKey(power))
                        tiles[matrixCells * i + j].BackColor = colors[power];
                    else
                        tiles[matrixCells * i + j].BackColor = Color.FromArgb(random.Next(0, 256), random.Next(0, 256), random.Next(0, 256));

                    AdaptButtonFontSize(tiles[matrixCells * i + j]);
                }
            }
        }
        // Неоптимизированная.
        private void ShowMatrix()
        {
            int[,] matrix = game.GetMatrix();
            Random random = new Random();
            for (int i = 0; i < matrixRows; i++)
            {
                for (int j = 0; j < matrixCells; j++)
                {
                    if (matrix[i, j] == 0)
                    {
                        tiles[matrixCells * i + j].Text = "";
                        tiles[matrixCells * i + j].BackColor = Color.WhiteSmoke;
                        tiles[matrixCells * i + j].Font = new Font("Microsoft Sans Serif", 20, FontStyle.Bold);
                        continue;
                    }
                    else
                        tiles[matrixCells * i + j].Text = (matrix[i, j]).ToString();

                    // Установка цвета плитки.
                    int power = 0;
                    for (int number = minValue; number < matrix[i, j]; number += number, power++)
                    { }
                    if (colors.ContainsKey(power))
                        tiles[matrixCells * i + j].BackColor = colors[power];
                    else
                        tiles[matrixCells * i + j].BackColor = Color.FromArgb(random.Next(0, 256), random.Next(0, 256), random.Next(0, 256));
                    AdaptButtonFontSize(tiles[matrixCells * i + j]);
                }
            }
        }
        private void ShowScore()
        {
            score = game.GetScore();
            if (score == -1)
                bScore.Text = "Score:\n∞";
            else
                bScore.Text = "Score:\n" + score;
        }
        private void ShowBestScore()
        {
            if (bestScore==-1)
                bBest.Text = "Best:\n∞";
            else
                bBest.Text = "Best:\n" + bestScore;
        }

        private void NewGame()
        {
            game = new G_2048(matrixRows, matrixCells, minValue);
            gameOver = false;
            game.EndGameEvent += EndGameHandler;
            game.ScoreOverflowEvent += ScoreOverflowHandler;
            game.NewTileEvent += NewTileHandler;
            foreach (Button button in tiles)
            {
                button.Text = "";
            }
            ShowMatrix();
            ShowScore();
        }

        private void EndGameHandler()
        {
            MessageBox.Show("Game Over!", "\"2048\"");
            gameOver = true;
        }
        private void ScoreOverflowHandler()
        {
            MessageBox.Show("Поздравляю. Вы превысили предел допустимого значения очков. Вы задрот или читер.", "2048");
            gameOver = true;
            records[matrixRows + " " + matrixCells] = -1;
            bestScore = -1;
            score = -1;
            bScore.Text = "Score:\n∞";
            bBest.Text = "Best:\n∞";
        }
        private void NewTileHandler(int x, int y)
        {
            lockOn = true;
            ShowMatrix();
            Button newTile = null;
            foreach (Button button in this.pMatrix.Controls.OfType<Button>())
            {
                if (button.Name == "bM" + x + y)
                {
                    newTile = button;
                    break;
                }
            }
            if (newTile == null) return;

            AnimationExtension(newTile, 150);
            lockOn = false;
        }

        // Недоделанный метод.
        private void AnimationExtension(Control element/*, int timeOnAnimation, int increasePercent = 1*/)
        {
            if (element == null /*|| timeOnAnimation <= 0*/)
                return;

            int originalWidth = element.Size.Width;
            int originalHeight = element.Size.Height;
            Font originalFont = element.Font;
            element.Font = new Font(element.Font.Name, 1, element.Font.Style);

            element.Location = new Point(element.Location.X + element.Width / 2, element.Location.Y + element.Height / 2);
            element.Size = new Size(0, 0);
            
            for (int i = 0; i < originalWidth; i++)
            {
                element.Width++;
                element.Height++;
                if (i % 2 == 0 || i == 0)
                    element.Location = new Point(element.Location.X - 1, element.Location.Y - 1);
                Thread.Sleep(2);
                //if (element is Button)
                //    AdaptButtonFontSize(element as Button);
                Application.DoEvents();
            }
            element.Font = originalFont;

            /*// На сколько нужно увеличивать размер/позицию за 1 мс.
            double onePercent = 100 / timeOnAnimation;
            // На сколько будем увеличивать размер элемента...
            int widthInterval = (int)(originalWidth * onePercent);
            int heightInterval = (int)(originalHeight * onePercent);
            // и смещать позицию.
            double pointXInterval = element.Location.X * increasePercent;
            double pointYInterval = element.Location.Y * increasePercent;

            element.Size = new Size(0, 0);
            while (element.Size.Width < originalWidth && element.Size.Height < originalHeight)
            {
                Thread.Sleep(timeOnStep);
                element.Size = new Size(element.Size.Width + widthInterval, element.Size.Height + heightInterval);
                Application.DoEvents();
            }*/
        }
        private void AnimationExtension(Control element, int timeOnAnimation)
        {
            if (element == null)
                return;
            if (element.Size.Width != element.Size.Height)
                throw new ArgumentException("Element width must be equal to heght");
            if (timeOnAnimation <= 0)
                throw new ArgumentException("Time can't be less than 0");

            int originalWidth = element.Size.Width;
            int originalHeight = element.Size.Height;
            Point originalLocation = new Point(element.Location.X, element.Location.Y);
            Font originalFont = element.Font;

            element.Font = new Font(element.Font.Name, 0.1f, element.Font.Style);
            element.Location = new Point(element.Location.X + element.Width / 2, element.Location.Y + element.Height / 2);
            element.Size = new Size(0, 0);

            int timeOnSleep = Convert.ToInt32(Math.Floor(Convert.ToDouble(timeOnAnimation) / Convert.ToDouble(originalWidth)));
            double fontIncrement = (Convert.ToDouble(originalFont.Size * 0.1) / Convert.ToDouble(originalWidth) * 10);

            for (int i = 0; i < originalWidth; i++)
            {
                Thread.Sleep(timeOnSleep);
                element.Size = new Size(element.Size.Width + 1, element.Size.Width + 1);
                element.Font = new Font(element.Font.Name, (float)(element.Font.Size + fontIncrement), element.Font.Style);
                if (i % 2 == 0)
                    element.Location = new Point(element.Location.X - 1, element.Location.Y - 1);
                Application.DoEvents();
            }
            if (element.Location.X != originalLocation.X || element.Location.Y != originalLocation.Y)
                element.Location = originalLocation;
            element.Font = originalFont;
        }
        private void AdaptButtonFontSize(Button element, double fontSizePecrent = 0.85)
        {
            if (element == null) return;

            Size original = element.Size;
            Size textSize;
            element.Font = new Font(element.Font.Name, 1, element.Font.Style);
            while ((textSize = TextRenderer.MeasureText(element.Text, element.Font)).Width < element.ClientSize.Width * fontSizePecrent)
            {
                element.Font = new Font(element.Font.Name, element.Font.Size + 0.5f, element.Font.Style);
            }
        }

        private void ResetAllScores()
        {
            records = new Dictionary<string, int>();
            for (int i = 2; i <= 20; i++)
            {
                for (int j = 2; j <= 20; j++)
                {
                    records[i + " " + j] = 0;
                }
            }
            bestScore = 0;
        }
        private void ResetCurrentRecord()
        {
            if (records.ContainsKey(matrixRows + " " + matrixCells))
            {
                records[matrixRows + " " + matrixCells] = 0;
                bestScore = 0;
            }
        }
        private void ReadColors()
        {
            try
            {
                colors = new Dictionary<int, Color>();
                using (BinaryReader br = new BinaryReader(new FileStream("colors.2048", FileMode.OpenOrCreate)))
                {
                    int count = br.ReadInt32();
                    for (int i = 0; i < count; i++)
                    {
                        int key = br.ReadInt32();
                        colors[key] = Color.FromArgb(br.ReadByte(), br.ReadByte(), br.ReadByte(), br.ReadByte());
                    }

                }
            }
            catch (IOException)
            {
                MessageBox.Show("Ошибка чтения файла!", "Error");
            }
            catch
            {
                MessageBox.Show("Ошибка!", "Error");
            }
        }
        private void WriteColors()
        {
            try
            {
                using (BinaryWriter bw = new BinaryWriter(new FileStream("colors.2048", FileMode.Create)))
                {
                    bw.Write(colors.Count);
                    foreach (KeyValuePair<int, Color> color in colors)
                    {
                        bw.Write(color.Key);
                        bw.Write(color.Value.A);
                        bw.Write(color.Value.R);
                        bw.Write(color.Value.G);
                        bw.Write(color.Value.B);
                    }
                }
            }
            catch (IOException)
            {
                MessageBox.Show("Ошибка записи файла!", "Error");
            }
            catch
            {
                MessageBox.Show("Ошибка!", "Error");
            }
        }
        private void ReadRecords()
        {
            try
            {
                using (BinaryReader br = new BinaryReader(new FileStream("record.2048", FileMode.OpenOrCreate)))
                {
                    records = new Dictionary<string, int>();
                    int count = br.ReadInt32();
                    string key;
                    for (int i = 0; i < count; i++)
                    {
                        key = br.ReadString();
                        records[key] = br.ReadInt32();
                    }
                    if (records.ContainsKey(matrixRows + " " + matrixCells))
                        bestScore = records[matrixRows + " " + matrixCells];
                    else
                        bestScore = 0;
                }
                ShowBestScore();
            }
            catch (IOException)
            {
                MessageBox.Show("Ошибка чтения файла!", "Error");
            }
            catch
            {
                MessageBox.Show("Ошибка!", "Error");
            }
        }
        private void WriteRecords()
        {
            try
            {
                using (BinaryWriter bw = new BinaryWriter(new FileStream("record.2048", FileMode.Create)))
                {
                    bw.Write(records.Count);
                    foreach (KeyValuePair<string, int> element in records)
                    {
                        bw.Write(element.Key);
                        bw.Write(element.Value);
                    }
                }
            }
            catch (IOException)
            {
                MessageBox.Show("Ошибка записи файла!", "Error");
            }
            catch
            {
                MessageBox.Show("Ошибка!", "Error");
            }
        }
        private void ReadSettings()
        {
            try
            {
                using (BinaryReader br = new BinaryReader(new FileStream("colors.2048", FileMode.OpenOrCreate)))
                {
                    matrixRows = br.ReadInt32();
                    matrixCells = br.ReadInt32();
                    int size = br.ReadInt32();
                    tileSize = new Size(size, size);
                    intervalBetweenTiles = br.ReadInt32();
                    borderInterval = br.ReadInt32();
                    ellipseTile = br.ReadBoolean();
                }
            }
            catch (IOException)
            {
                MessageBox.Show("Ошибка чтения файла!", "Error");
            }
            catch
            {
                MessageBox.Show("Ошибка!", "Error");
            }
        }
        private void WriteSettings()
        {
            try
            {
                using (BinaryWriter bw = new BinaryWriter(new FileStream("data.2048", FileMode.Create)))
                {
                    bw.Write(matrixRows);
                    bw.Write(matrixCells);
                    bw.Write(tileSize.Width);
                    bw.Write(intervalBetweenTiles);
                    bw.Write(borderInterval);
                    bw.Write(ellipseTile);
                    bw.Write(this.BackColor.A);
                    bw.Write(this.BackColor.R);
                    bw.Write(this.BackColor.G);
                    bw.Write(this.BackColor.B);
                }
            }
            catch (IOException)
            {
                MessageBox.Show("Ошибка записи файла!", "Error");
            }
            catch
            {
                MessageBox.Show("Ошибка!", "Error");
            }
        }
        #endregion
        #region События формы
        private void bNewGame_Click(object sender, EventArgs e)
        {
            NewGame();
        }
        private void bUndo_Click(object sender, EventArgs e)
        {
            if (game == null || gameOver) return;
            if (game.UndoAllowed())
            {
                int[,] oldMatrix = game.GetMatrix();
                game.Undo();
                ShowMatrix(oldMatrix);
                ShowScore();
            }
        }
        private void bOptions_Click(object sender, EventArgs e)
        {
            OptionsEvent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            ReadColors();
            ReadRecords();
        }
        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.N)
            {
                NewGame();
                return;
            }
            else if (e.KeyData == Keys.F1)
            {
                MessageBox.Show(@"Горячие клавиши:
F1 - помощь;
F2 - выход;
F11 - сбросить текущий рекорд;
F12 - сбросить все рекорды.
", "2048", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else if (e.KeyData == Keys.F2)
            {
                Application.Exit();
            }
            else if (e.KeyData == Keys.F11)
            {
                var result = MessageBox.Show("Обнулить текущий рекорд?", "2048", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    ResetCurrentRecord();
                    MessageBox.Show("Рекорд сброшен", "2048");
                    ShowBestScore();
                }
                return;
            }
            else if (e.KeyData == Keys.F12)
            {
                var result = MessageBox.Show("Обнулить все рекорды?", "2048", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    ResetAllScores();
                    MessageBox.Show("Все рекорды сброшены", "2048");
                    ShowBestScore();
                }
                return;
            }
            if (game == null || gameOver || lockOn) return;
            int[,] oldMatrix;
            bool moved = false;
            switch (e.KeyData)
            {
                case Keys.R:
                case Keys.B:
                    oldMatrix = game.GetMatrix();
                    game.Undo();
                    break;
                case Keys.Up:
                case Keys.W:
                    oldMatrix = game.GetMatrix();
                    moved = game.TryMoveUp();
                    break;
                case Keys.Down:
                case Keys.S:
                    oldMatrix = game.GetMatrix();
                    moved = game.TryMoveDown();
                    break;
                case Keys.Left:
                case Keys.A:
                    oldMatrix = game.GetMatrix();
                    moved = game.TryMoveLeft();
                    break;
                case Keys.Right:
                case Keys.D:
                    oldMatrix = game.GetMatrix();
                    moved = game.TryMoveRight();
                    break;
                default:
                    return;
            }
            ShowMatrix(oldMatrix);
            if (moved)
            {
                int tempScore = game.GetScore();
                if (bestScore != -1 && tempScore > bestScore)
                {
                    bestScore = tempScore;
                    records[matrixRows + " " + matrixCells] = tempScore;
                    ShowBestScore();
                }
                ShowScore();
            }
        }
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            WriteColors();
            WriteRecords();
            WriteSettings();
            Application.Exit();
        }
        #endregion
    }

    struct ControlsGenerator
    {
        public static Button[] CreateButtons(int count, Size size, Font font, Color backColor)
        {
            if (font == null || backColor == null) return null;

            Button[] buttons = new Button[count];
            for (int i = 0; i < count; i++)
            {
                buttons[i] = new Button();
                buttons[i].Size = size;
                buttons[i].Font = font;
                buttons[i].BackColor = backColor;
            }
            return buttons;
        }
    }

    class NonFocusButton : Button
    {
        public NonFocusButton()
        {
            this.SetStyle(ControlStyles.Selectable, false);
        }
    }
}