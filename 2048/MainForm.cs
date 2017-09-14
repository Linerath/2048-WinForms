#define SettingsFromFile

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using G_2048 = Game_2048._2048;

namespace _2048
{
    public delegate void OptionsEventHandler();

    public partial class MainForm : Form
    {
        public event OptionsEventHandler OptionsEvent;

        private Int32 score;                        // Очки
        private Int32 bestScore;                    // Рекорд
        private Int32 minValue = 2;                 // Минимально выпадающий блок
        private Int32 matrixRows = 4;               // Количество строк
        private Int32 matrixCells = 4;              // Количество столбцов
        private Int32 intervalBetweenTiles = 10;    // Интервал между блоками (тайлами)
        private Int32 borderInterval = 10;          // Интервал до края формы
        private Boolean gameOver = true;
        private Boolean lockOn = false;             // Задержка между сдвигами
        private Boolean ellipseTile = false;        // Круговые тайлы
        /*****/
        private Button[] tiles;                     // Массив тайлов (кнопок)
        /*****/
        private G_2048 game;
        private Size tileSize = new Size(60, 60);
        private Size normalFormSize = new Size(323, 466);
        private Dictionary<Int32, Color> colors;    // Цвета плиток
        private Dictionary<String, Int32> records;  // Рекорды для каждого размера поля

        public MainForm()
        {
            InitializeComponent();
        }
        public MainForm(Int32 _matrixRows, Int32 _matrixCells, Size _tileSize, Int32 _Int32ervalBetweenTiles, Int32 _borderInt32erval, Boolean _ellipseTile, Color backColor)
        {
            if (_matrixRows < 2 || _matrixCells < 2 || _tileSize.Width < 1 || _tileSize.Height < 1 || _Int32ervalBetweenTiles < 0 || _borderInt32erval < 0)
                return;
            matrixRows = _matrixRows;
            matrixCells = _matrixCells;
            intervalBetweenTiles = _Int32ervalBetweenTiles;
            borderInterval = _borderInt32erval;
            ellipseTile = _ellipseTile;
            tileSize = new Size(_tileSize.Width, _tileSize.Height);

            InitializeComponent();
            BackColor = backColor;
            /*Подгон размеров всех элементов*/
            // Панель с матрицей.
            pMatrix.Size = new Size(matrixCells * tileSize.Width + (intervalBetweenTiles * (matrixCells + 1)), matrixRows * tileSize.Height + (intervalBetweenTiles * (matrixRows + 1)));
            pMatrix.Location = new Point(borderInterval, borderInterval);

            // Создание плиток.
            CreateMatrix();

            // Предполагаемые размеры формы.
            Size newFormSize = new Size();
            Int32 supposedFormWidth = borderInterval * 2 + pMatrix.Size.Width;
            Int32 supposedFormHeight = pMenu.Size.Height + (borderInterval * 2 + pMatrix.Size.Height);
            newFormSize.Width = normalFormSize.Width < supposedFormWidth ?
                supposedFormWidth : normalFormSize.Width;
            newFormSize.Height = normalFormSize.Height < supposedFormHeight ?
                supposedFormHeight : normalFormSize.Height;

            ClientSize = new Size(newFormSize.Width, newFormSize.Height);
            pMatrix.Location = new Point(pField.Size.Width / 2 - pMatrix.Size.Width / 2, pField.Size.Height / 2 - pMatrix.Size.Height / 2);
        }

        #region Methods
        // Создание плиток.
        private void CreateMatrix()
        {
            tiles = CreateButtons(matrixRows * matrixCells, tileSize, new Font("Microsoft Sans Serif", 20, FontStyle.Bold), Color.WhiteSmoke);

            Point location = new Point(intervalBetweenTiles, intervalBetweenTiles);
            for (Int32 i = 0; i < matrixRows; i++)
            {
                for (Int32 j = 0; j < matrixCells; j++)
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
        private Button[] CreateButtons(Int32 count, Size size, Font font, Color backColor)
        {
            if (font == null || backColor == null) return null;

            Button[] buttons = new Button[count];
            for (Int32 i = 0; i < count; i++)
            {
                buttons[i] = new Button()
                {
                    Size = size,
                    Font = font,
                    BackColor = backColor
                };
            }
            return buttons;
        }

        // Отображение матрицы на экране. Оптмизированная версия.
        private void ShowMatrix(Int32[,] oldMatrix)
        {
            Int32[,] matrix = game.GetMatrix();
            Random random = new Random();
            if (oldMatrix == null || oldMatrix.Length != matrix.Length) return;
            for (Int32 i = 0; i < matrixRows; i++)
            {
                for (Int32 j = 0; j < matrixCells; j++)
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
                    Int32 power = 0;
                    for (Int32 number = minValue; number < matrix[i, j]; number += number, power++)
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
            Int32[,] matrix = game.GetMatrix();
            Random random = new Random();
            for (Int32 i = 0; i < matrixRows; i++)
            {
                for (Int32 j = 0; j < matrixCells; j++)
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
                    Int32 power = 0;
                    for (Int32 number = minValue; number < matrix[i, j]; number += number, power++)
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
            score = game.Score;
            if (score == -1)
                bScore.Text = "Score:\n∞";
            else
                bScore.Text = "Score:\n" + score;
        }
        private void ShowBestScore()
        {
            if (bestScore == -1)
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
        private void NewTileHandler(Int32 x, Int32 y)
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
        private void AnimationExtension(Control element/*, Int32 timeOnAnimation, Int32 increasePercent = 1*/)
        {
            if (element == null /*|| timeOnAnimation <= 0*/)
                return;

            Int32 originalWidth = element.Size.Width;
            Int32 originalHeight = element.Size.Height;
            Font originalFont = element.Font;
            element.Font = new Font(element.Font.Name, 1, element.Font.Style);

            element.Location = new Point(element.Location.X + element.Width / 2, element.Location.Y + element.Height / 2);
            element.Size = new Size(0, 0);

            for (Int32 i = 0; i < originalWidth; i++)
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
            Int32 widthInt32erval = (Int32)(originalWidth * onePercent);
            Int32 heightInt32erval = (Int32)(originalHeight * onePercent);
            // и смещать позицию.
            double PointXInt32erval = element.Location.X * increasePercent;
            double PointYInt32erval = element.Location.Y * increasePercent;

            element.Size = new Size(0, 0);
            while (element.Size.Width < originalWidth && element.Size.Height < originalHeight)
            {
                Thread.Sleep(timeOnStep);
                element.Size = new Size(element.Size.Width + widthInt32erval, element.Size.Height + heightInt32erval);
                Application.DoEvents();
            }*/
        }
        private void AnimationExtension(Control element, Int32 timeOnAnimation)
        {
            if (element == null)
                return;
            if (element.Size.Width != element.Size.Height)
                throw new ArgumentException("Element width must be equal to heght");
            if (timeOnAnimation <= 0)
                throw new ArgumentException("Time can't be less than 0");

            Int32 originalWidth = element.Size.Width;
            Int32 originalHeight = element.Size.Height;
            Point originalLocation = new Point(element.Location.X, element.Location.Y);
            Font originalFont = element.Font;

            element.Font = new Font(element.Font.Name, 0.1f, element.Font.Style);
            element.Location = new Point(element.Location.X + element.Width / 2, element.Location.Y + element.Height / 2);
            element.Size = new Size(0, 0);

            Int32 timeOnSleep = Convert.ToInt32(Math.Floor(Convert.ToDouble(timeOnAnimation) / Convert.ToDouble(originalWidth)));
            double fontIncrement = (Convert.ToDouble(originalFont.Size * 0.1) / Convert.ToDouble(originalWidth) * 10);

            for (Int32 i = 0; i < originalWidth; i++)
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
            records = new Dictionary<String, Int32>();
            for (Int32 i = 2; i <= 20; i++)
            {
                for (Int32 j = 2; j <= 20; j++)
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
                colors = new Dictionary<Int32, Color>();
                using (BinaryReader br = new BinaryReader(new FileStream("colors.2048", FileMode.OpenOrCreate)))
                {
                    Int32 count = br.ReadInt32();
                    for (Int32 i = 0; i < count; i++)
                    {
                        Int32 key = br.ReadInt32();
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
                    foreach (KeyValuePair<Int32, Color> color in colors)
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
                    records = new Dictionary<String, Int32>();
                    Int32 count = br.ReadInt32();
                    String key;
                    for (Int32 i = 0; i < count; i++)
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
                    foreach (KeyValuePair<String, Int32> element in records)
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
                    Int32 size = br.ReadInt32();
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
                    bw.Write(BackColor.A);
                    bw.Write(BackColor.R);
                    bw.Write(BackColor.G);
                    bw.Write(BackColor.B);
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
        #region Events
        private void bNewGame_Click(object sender, EventArgs e)
        {
            NewGame();
        }
        private void bUndo_Click(object sender, EventArgs e)
        {
            if (game == null || gameOver) return;
            if (game.UndoAllowed())
            {
                Int32[,] oldMatrix = game.GetMatrix();
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
                var result = MessageBox.Show("Вы действительно хотите выйти? Текущий рекорд не сохранится.", "2048", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    Application.Exit();
                }
            }
            else if (e.KeyData == Keys.F11)
            {
                var result = MessageBox.Show("Обнулить текущий рекорд?", "2048", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
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
                if (result == DialogResult.Yes)
                {
                    ResetAllScores();
                    MessageBox.Show("Все рекорды сброшены", "2048");
                    ShowBestScore();
                }
                return;
            }
            if (game == null || gameOver || lockOn) return;
            Int32[,] oldMatrix;
            Boolean moved = false;
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
                Int32 tempScore = game.Score;
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

    /*Кнопка, не получающая фокус.*/
    class NonFocusButton : Button
    {
        public NonFocusButton() => SetStyle(ControlStyles.Selectable, false);
    }
}