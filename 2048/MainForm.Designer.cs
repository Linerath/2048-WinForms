﻿namespace _2048
{
    partial class MainForm
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.pMenu = new System.Windows.Forms.Panel();
            this.pField = new System.Windows.Forms.Panel();
            this.pMatrix = new System.Windows.Forms.Panel();
            this.bUndo = new _2048.NonFocusButton();
            this.bOptions = new _2048.NonFocusButton();
            this.bBest = new _2048.NonFocusButton();
            this.bScore = new _2048.NonFocusButton();
            this.bNewGame = new _2048.NonFocusButton();
            this.pMenu.SuspendLayout();
            this.pField.SuspendLayout();
            this.SuspendLayout();
            // 
            // pMenu
            // 
            this.pMenu.Controls.Add(this.bUndo);
            this.pMenu.Controls.Add(this.bOptions);
            this.pMenu.Controls.Add(this.bBest);
            this.pMenu.Controls.Add(this.bScore);
            this.pMenu.Controls.Add(this.bNewGame);
            this.pMenu.Dock = System.Windows.Forms.DockStyle.Top;
            this.pMenu.Location = new System.Drawing.Point(0, 0);
            this.pMenu.Name = "pMenu";
            this.pMenu.Size = new System.Drawing.Size(317, 126);
            this.pMenu.TabIndex = 1;
            // 
            // pField
            // 
            this.pField.Controls.Add(this.pMatrix);
            this.pField.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pField.Location = new System.Drawing.Point(0, 126);
            this.pField.Name = "pField";
            this.pField.Size = new System.Drawing.Size(317, 316);
            this.pField.TabIndex = 2;
            // 
            // pMatrix
            // 
            this.pMatrix.BackColor = System.Drawing.Color.LightGray;
            this.pMatrix.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.pMatrix.Location = new System.Drawing.Point(11, 11);
            this.pMatrix.Name = "pMatrix";
            this.pMatrix.Size = new System.Drawing.Size(295, 295);
            this.pMatrix.TabIndex = 9;
            // 
            // bUndo
            // 
            this.bUndo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bUndo.BackColor = System.Drawing.Color.Tomato;
            this.bUndo.FlatAppearance.BorderSize = 0;
            this.bUndo.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Red;
            this.bUndo.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Red;
            this.bUndo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bUndo.Font = new System.Drawing.Font("Segoe Print", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(204)));
            this.bUndo.Location = new System.Drawing.Point(206, 81);
            this.bUndo.Name = "bUndo";
            this.bUndo.Size = new System.Drawing.Size(100, 33);
            this.bUndo.TabIndex = 0;
            this.bUndo.Text = "Undo";
            this.bUndo.UseVisualStyleBackColor = false;
            this.bUndo.Click += new System.EventHandler(this.bUndo_Click);
            // 
            // bOptions
            // 
            this.bOptions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.bOptions.BackColor = System.Drawing.Color.Tomato;
            this.bOptions.FlatAppearance.BorderSize = 0;
            this.bOptions.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Red;
            this.bOptions.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Red;
            this.bOptions.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bOptions.Font = new System.Drawing.Font("Segoe Print", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.bOptions.Location = new System.Drawing.Point(101, 81);
            this.bOptions.Name = "bOptions";
            this.bOptions.Size = new System.Drawing.Size(100, 33);
            this.bOptions.TabIndex = 0;
            this.bOptions.Text = "Options";
            this.bOptions.UseVisualStyleBackColor = false;
            this.bOptions.Click += new System.EventHandler(this.bOptions_Click);
            // 
            // bBest
            // 
            this.bBest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bBest.BackColor = System.Drawing.Color.Yellow;
            this.bBest.Enabled = false;
            this.bBest.FlatAppearance.BorderSize = 0;
            this.bBest.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bBest.Font = new System.Drawing.Font("Segoe Print", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.bBest.Location = new System.Drawing.Point(206, 12);
            this.bBest.Name = "bBest";
            this.bBest.Size = new System.Drawing.Size(100, 64);
            this.bBest.TabIndex = 0;
            this.bBest.Text = "Best:";
            this.bBest.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.bBest.UseVisualStyleBackColor = false;
            // 
            // bScore
            // 
            this.bScore.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bScore.BackColor = System.Drawing.Color.Yellow;
            this.bScore.Enabled = false;
            this.bScore.FlatAppearance.BorderSize = 0;
            this.bScore.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bScore.Font = new System.Drawing.Font("Segoe Print", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.bScore.Location = new System.Drawing.Point(100, 12);
            this.bScore.Name = "bScore";
            this.bScore.Size = new System.Drawing.Size(100, 63);
            this.bScore.TabIndex = 0;
            this.bScore.Text = "Score:";
            this.bScore.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.bScore.UseVisualStyleBackColor = false;
            // 
            // bNewGame
            // 
            this.bNewGame.BackColor = System.Drawing.Color.Gainsboro;
            this.bNewGame.FlatAppearance.BorderSize = 4;
            this.bNewGame.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Silver;
            this.bNewGame.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Silver;
            this.bNewGame.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bNewGame.Font = new System.Drawing.Font("Segoe Print", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.bNewGame.Location = new System.Drawing.Point(12, 12);
            this.bNewGame.Name = "bNewGame";
            this.bNewGame.Size = new System.Drawing.Size(83, 102);
            this.bNewGame.TabIndex = 0;
            this.bNewGame.Text = "New Game";
            this.bNewGame.UseVisualStyleBackColor = false;
            this.bNewGame.Click += new System.EventHandler(this.bNewGame_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(317, 442);
            this.Controls.Add(this.pField);
            this.Controls.Add(this.pMenu);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "\"2048\"";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            this.pMenu.ResumeLayout(false);
            this.pField.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pMenu;
        private System.Windows.Forms.Panel pField;
        private System.Windows.Forms.Panel pMatrix;
        private NonFocusButton bNewGame;
        private NonFocusButton bScore;
        private NonFocusButton bBest;
        private NonFocusButton bOptions;
        private NonFocusButton bUndo;
    }
}

