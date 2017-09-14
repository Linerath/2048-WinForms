namespace _2048
{
    partial class OptionsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.bOK = new System.Windows.Forms.Button();
            this.lMatrixSize = new System.Windows.Forms.Label();
            this.nudRows = new System.Windows.Forms.NumericUpDown();
            this.lRows = new System.Windows.Forms.Label();
            this.lCells = new System.Windows.Forms.Label();
            this.nudCells = new System.Windows.Forms.NumericUpDown();
            this.lTileSize = new System.Windows.Forms.Label();
            this.nudTileSize = new System.Windows.Forms.NumericUpDown();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.nudInterval2 = new System.Windows.Forms.NumericUpDown();
            this.lInt32erval2 = new System.Windows.Forms.Label();
            this.nudInterval1 = new System.Windows.Forms.NumericUpDown();
            this.lInt32erval1 = new System.Windows.Forms.Label();
            this.cbEllipse = new System.Windows.Forms.CheckBox();
            this.bClose = new System.Windows.Forms.Button();
            this.pColor = new System.Windows.Forms.Panel();
            this.lColor = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.nudRows)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCells)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTileSize)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudInterval2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudInterval1)).BeginInit();
            this.SuspendLayout();
            // 
            // bOK
            // 
            this.bOK.FlatAppearance.BorderSize = 3;
            this.bOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bOK.Font = new System.Drawing.Font("Tahoma", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.bOK.Location = new System.Drawing.Point(343, 238);
            this.bOK.Name = "bOK";
            this.bOK.Size = new System.Drawing.Size(86, 33);
            this.bOK.TabIndex = 0;
            this.bOK.Text = "OK";
            this.toolTip1.SetToolTip(this.bOK, "Предупреждение: при нажатии кнопки \"ОК\", предыдущая игра будет завершена без сохр" +
                    "анения. Чтобы вернуться к игре, используйте крестик.");
            this.bOK.UseVisualStyleBackColor = true;
            this.bOK.Click += new System.EventHandler(this.bOK_Click);
            // 
            // lMatrixSize
            // 
            this.lMatrixSize.AutoSize = true;
            this.lMatrixSize.Font = new System.Drawing.Font("Segoe PrInt32", 11.25F);
            this.lMatrixSize.Location = new System.Drawing.Point(20, 6);
            this.lMatrixSize.Name = "lMatrixSize";
            this.lMatrixSize.Size = new System.Drawing.Size(186, 26);
            this.lMatrixSize.TabIndex = 1;
            this.lMatrixSize.Text = "Размер игрового поля:";
            // 
            // nudRows
            // 
            this.nudRows.BackColor = System.Drawing.Color.Yellow;
            this.nudRows.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.nudRows.Location = new System.Drawing.Point(113, 35);
            this.nudRows.Maximum = new decimal(new int[] {
            25,
            0,
            0,
            0});
            this.nudRows.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nudRows.Name = "nudRows";
            this.nudRows.Size = new System.Drawing.Size(57, 26);
            this.nudRows.TabIndex = 2;
            this.nudRows.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            // 
            // lRows
            // 
            this.lRows.AutoSize = true;
            this.lRows.Font = new System.Drawing.Font("Segoe PrInt32", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lRows.Location = new System.Drawing.Point(31, 36);
            this.lRows.Name = "lRows";
            this.lRows.Size = new System.Drawing.Size(66, 23);
            this.lRows.TabIndex = 3;
            this.lRows.Text = "Строки:";
            // 
            // lCells
            // 
            this.lCells.AutoSize = true;
            this.lCells.Font = new System.Drawing.Font("Segoe PrInt32", 9.75F);
            this.lCells.Location = new System.Drawing.Point(31, 69);
            this.lCells.Name = "lCells";
            this.lCells.Size = new System.Drawing.Size(76, 23);
            this.lCells.TabIndex = 5;
            this.lCells.Text = "Столбцы:";
            // 
            // nudCells
            // 
            this.nudCells.BackColor = System.Drawing.Color.Yellow;
            this.nudCells.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.nudCells.Location = new System.Drawing.Point(113, 68);
            this.nudCells.Maximum = new decimal(new int[] {
            25,
            0,
            0,
            0});
            this.nudCells.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nudCells.Name = "nudCells";
            this.nudCells.Size = new System.Drawing.Size(57, 26);
            this.nudCells.TabIndex = 4;
            this.nudCells.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            // 
            // lTileSize
            // 
            this.lTileSize.AutoSize = true;
            this.lTileSize.Font = new System.Drawing.Font("Segoe PrInt32", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lTileSize.Location = new System.Drawing.Point(13, 12);
            this.lTileSize.Name = "lTileSize";
            this.lTileSize.Size = new System.Drawing.Size(204, 26);
            this.lTileSize.TabIndex = 6;
            this.lTileSize.Text = "Размер игровой клетки:";
            // 
            // nudTileSize
            // 
            this.nudTileSize.BackColor = System.Drawing.Color.Aqua;
            this.nudTileSize.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.nudTileSize.Location = new System.Drawing.Point(84, 56);
            this.nudTileSize.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.nudTileSize.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.nudTileSize.Name = "nudTileSize";
            this.nudTileSize.Size = new System.Drawing.Size(57, 26);
            this.nudTileSize.TabIndex = 7;
            this.nudTileSize.Value = new decimal(new int[] {
            70,
            0,
            0,
            0});
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Yellow;
            this.panel1.Controls.Add(this.nudRows);
            this.panel1.Controls.Add(this.lRows);
            this.panel1.Controls.Add(this.lCells);
            this.panel1.Controls.Add(this.lMatrixSize);
            this.panel1.Controls.Add(this.nudCells);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(241, 131);
            this.panel1.TabIndex = 8;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Aqua;
            this.panel2.Controls.Add(this.lTileSize);
            this.panel2.Controls.Add(this.nudTileSize);
            this.panel2.Location = new System.Drawing.Point(200, 80);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(229, 112);
            this.panel2.TabIndex = 9;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.DarkOrange;
            this.panel3.Controls.Add(this.nudInterval2);
            this.panel3.Controls.Add(this.lInt32erval2);
            this.panel3.Controls.Add(this.nudInterval1);
            this.panel3.Controls.Add(this.lInt32erval1);
            this.panel3.Location = new System.Drawing.Point(50, 125);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(195, 133);
            this.panel3.TabIndex = 10;
            // 
            // nudInt32erval2
            // 
            this.nudInterval2.BackColor = System.Drawing.Color.DarkOrange;
            this.nudInterval2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.nudInterval2.Location = new System.Drawing.Point(64, 104);
            this.nudInterval2.Name = "nudInt32erval2";
            this.nudInterval2.Size = new System.Drawing.Size(57, 22);
            this.nudInterval2.TabIndex = 10;
            this.nudInterval2.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // lInt32erval2
            // 
            this.lInt32erval2.AutoSize = true;
            this.lInt32erval2.Font = new System.Drawing.Font("Segoe PrInt32", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lInt32erval2.Location = new System.Drawing.Point(10, 74);
            this.lInt32erval2.Name = "lInt32erval2";
            this.lInt32erval2.Size = new System.Drawing.Size(158, 21);
            this.lInt32erval2.TabIndex = 9;
            this.lInt32erval2.Text = "Интервал до матрицы";
            // 
            // nudInt32erval1
            // 
            this.nudInterval1.BackColor = System.Drawing.Color.DarkOrange;
            this.nudInterval1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.nudInterval1.Location = new System.Drawing.Point(64, 35);
            this.nudInterval1.Name = "nudInt32erval1";
            this.nudInterval1.Size = new System.Drawing.Size(57, 22);
            this.nudInterval1.TabIndex = 8;
            this.nudInterval1.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // lInt32erval1
            // 
            this.lInt32erval1.AutoSize = true;
            this.lInt32erval1.Font = new System.Drawing.Font("Segoe PrInt32", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lInt32erval1.Location = new System.Drawing.Point(3, 5);
            this.lInt32erval1.Name = "lInt32erval1";
            this.lInt32erval1.Size = new System.Drawing.Size(192, 21);
            this.lInt32erval1.TabIndex = 7;
            this.lInt32erval1.Text = "Интервал между клетками:";
            // 
            // cbEllipse
            // 
            this.cbEllipse.AutoSize = true;
            this.cbEllipse.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.cbEllipse.Location = new System.Drawing.Point(251, 199);
            this.cbEllipse.Name = "cbEllipse";
            this.cbEllipse.Size = new System.Drawing.Size(157, 25);
            this.cbEllipse.TabIndex = 12;
            this.cbEllipse.Text = "Круглые плитки";
            this.cbEllipse.UseVisualStyleBackColor = true;
            // 
            // bClose
            // 
            this.bClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bClose.BackColor = System.Drawing.Color.Transparent;
            this.bClose.BackgroundImage = global::_2048.Properties.Resources.close;
            this.bClose.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.bClose.FlatAppearance.BorderSize = 2;
            this.bClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bClose.Location = new System.Drawing.Point(404, 7);
            this.bClose.Name = "bClose";
            this.bClose.Size = new System.Drawing.Size(25, 25);
            this.bClose.TabIndex = 11;
            this.bClose.UseVisualStyleBackColor = false;
            this.bClose.Click += new System.EventHandler(this.bClose_Click);
            // 
            // pColor
            // 
            this.pColor.BackColor = System.Drawing.Color.SteelBlue;
            this.pColor.Location = new System.Drawing.Point(304, 18);
            this.pColor.Name = "pColor";
            this.pColor.Size = new System.Drawing.Size(47, 29);
            this.pColor.TabIndex = 13;
            this.pColor.Click += new System.EventHandler(this.pColor_Click);
            // 
            // lColor
            // 
            this.lColor.AutoSize = true;
            this.lColor.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lColor.Location = new System.Drawing.Point(280, 47);
            this.lColor.Name = "lColor";
            this.lColor.Size = new System.Drawing.Size(94, 21);
            this.lColor.TabIndex = 14;
            this.lColor.Text = "Цвет фона";
            // 
            // OptionsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.SpringGreen;
            this.ClientSize = new System.Drawing.Size(436, 283);
            this.Controls.Add(this.lColor);
            this.Controls.Add(this.pColor);
            this.Controls.Add(this.cbEllipse);
            this.Controls.Add(this.bClose);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.bOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OptionsForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Настройки";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.StartForm_FormClosing);
            this.Load += new System.EventHandler(this.StartForm_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OptionsForm_MouseDown);
            ((System.ComponentModel.ISupportInitialize)(this.nudRows)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCells)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTileSize)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudInterval2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudInterval1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bOK;
        private System.Windows.Forms.Label lMatrixSize;
        private System.Windows.Forms.NumericUpDown nudRows;
        private System.Windows.Forms.Label lRows;
        private System.Windows.Forms.Label lCells;
        private System.Windows.Forms.NumericUpDown nudCells;
        private System.Windows.Forms.Label lTileSize;
        private System.Windows.Forms.NumericUpDown nudTileSize;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label lInt32erval1;
        private System.Windows.Forms.NumericUpDown nudInterval1;
        private System.Windows.Forms.NumericUpDown nudInterval2;
        private System.Windows.Forms.Label lInt32erval2;
        private System.Windows.Forms.Button bClose;
        private System.Windows.Forms.CheckBox cbEllipse;
        private System.Windows.Forms.Panel pColor;
        private System.Windows.Forms.Label lColor;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}