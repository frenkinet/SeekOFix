namespace TestSeek
{
    partial class Form1
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
            this.button1 = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.trackBar2 = new System.Windows.Forms.TrackBar();
            this.button2 = new System.Windows.Forms.Button();
            this.cbAutoSave = new System.Windows.Forms.CheckBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.cbDynSlidres = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.lblSliderMin = new System.Windows.Forms.Label();
            this.lblSliderMax = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.toolTip2 = new System.Windows.Forms.ToolTip(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.rbUnitsF = new System.Windows.Forms.RadioButton();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.rbUnitsK = new System.Windows.Forms.RadioButton();
            this.rbUnitsC = new System.Windows.Forms.RadioButton();
            this.cbPal = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.lblLeft = new System.Windows.Forms.Label();
            this.lblRight = new System.Windows.Forms.Label();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.pictureBox5 = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lblMaxTemp = new System.Windows.Forms.Label();
            this.lblMinTemp = new System.Windows.Forms.Label();
            this.button4 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(151, 6);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(60, 25);
            this.button1.TabIndex = 0;
            this.button1.Text = "EXT Cal";
            this.toolTip2.SetToolTip(this.button1, "Do external calibration");
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(480, 166);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(0, 13);
            this.label5.TabIndex = 7;
            // 
            // trackBar1
            // 
            this.trackBar1.CausesValidation = false;
            this.trackBar1.LargeChange = 100;
            this.trackBar1.Location = new System.Drawing.Point(12, 396);
            this.trackBar1.Maximum = 20000;
            this.trackBar1.Minimum = 4000;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(1102, 45);
            this.trackBar1.SmallChange = 10;
            this.trackBar1.TabIndex = 11;
            this.trackBar1.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.trackBar1.Value = 4000;
            this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // trackBar2
            // 
            this.trackBar2.CausesValidation = false;
            this.trackBar2.LargeChange = 100;
            this.trackBar2.Location = new System.Drawing.Point(12, 370);
            this.trackBar2.Maximum = 20000;
            this.trackBar2.Minimum = 4000;
            this.trackBar2.Name = "trackBar2";
            this.trackBar2.Size = new System.Drawing.Size(1102, 45);
            this.trackBar2.SmallChange = 10;
            this.trackBar2.TabIndex = 12;
            this.trackBar2.TickFrequency = 100;
            this.trackBar2.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBar2.Value = 4000;
            this.trackBar2.Scroll += new System.EventHandler(this.trackBar2_Scroll);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(11, 264);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(200, 35);
            this.button2.TabIndex = 13;
            this.button2.Text = "Switch to manual range";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // cbAutoSave
            // 
            this.cbAutoSave.AutoSize = true;
            this.cbAutoSave.Location = new System.Drawing.Point(11, 94);
            this.cbAutoSave.Name = "cbAutoSave";
            this.cbAutoSave.Size = new System.Drawing.Size(73, 17);
            this.cbAutoSave.TabIndex = 14;
            this.cbAutoSave.Text = "AutoSave";
            this.cbAutoSave.UseVisualStyleBackColor = true;
            this.cbAutoSave.CheckedChanged += new System.EventHandler(this.cbAutoSave_CheckedChanged);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(23, 802);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1001, 1);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 15;
            this.pictureBox1.TabStop = false;
            // 
            // cbDynSlidres
            // 
            this.cbDynSlidres.AutoSize = true;
            this.cbDynSlidres.Location = new System.Drawing.Point(11, 305);
            this.cbDynSlidres.Name = "cbDynSlidres";
            this.cbDynSlidres.Size = new System.Drawing.Size(128, 17);
            this.cbDynSlidres.TabIndex = 16;
            this.cbDynSlidres.Text = "Enable relative sliders";
            this.cbDynSlidres.UseVisualStyleBackColor = true;
            this.cbDynSlidres.Visible = false;
            this.cbDynSlidres.CheckedChanged += new System.EventHandler(this.cbDynSlidres_CheckedChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 5);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(30, 13);
            this.label6.TabIndex = 17;
            this.label6.Text = "Live:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(699, 5);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(70, 13);
            this.label7.TabIndex = 18;
            this.label7.Text = "First after cal:";
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(11, 117);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(117, 17);
            this.checkBox2.TabIndex = 20;
            this.checkBox2.Text = "Apply Sharpen filter";
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox2.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // lblSliderMin
            // 
            this.lblSliderMin.AutoSize = true;
            this.lblSliderMin.Location = new System.Drawing.Point(16, 354);
            this.lblSliderMin.Name = "lblSliderMin";
            this.lblSliderMin.Size = new System.Drawing.Size(27, 13);
            this.lblSliderMin.TabIndex = 22;
            this.lblSliderMin.Text = "MIN";
            this.lblSliderMin.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblSliderMax
            // 
            this.lblSliderMax.AutoSize = true;
            this.lblSliderMax.Location = new System.Drawing.Point(1084, 425);
            this.lblSliderMax.Name = "lblSliderMax";
            this.lblSliderMax.Size = new System.Drawing.Size(30, 13);
            this.lblSliderMax.TabIndex = 23;
            this.lblSliderMax.Text = "MAX";
            this.lblSliderMax.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(10, 6);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(60, 25);
            this.button3.TabIndex = 27;
            this.button3.Text = "STOP";
            this.toolTip1.SetToolTip(this.button3, "Start / Stop Streaming");
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.button4);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.rbUnitsF);
            this.panel1.Controls.Add(this.pictureBox2);
            this.panel1.Controls.Add(this.rbUnitsK);
            this.panel1.Controls.Add(this.rbUnitsC);
            this.panel1.Controls.Add(this.cbPal);
            this.panel1.Controls.Add(this.label13);
            this.panel1.Controls.Add(this.cbAutoSave);
            this.panel1.Controls.Add(this.button3);
            this.panel1.Controls.Add(this.checkBox2);
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.cbDynSlidres);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Location = new System.Drawing.Point(471, 5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(225, 327);
            this.panel1.TabIndex = 28;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 73);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(62, 13);
            this.label3.TabIndex = 33;
            this.label3.Text = "Temp units:";
            // 
            // rbUnitsF
            // 
            this.rbUnitsF.AutoSize = true;
            this.rbUnitsF.Location = new System.Drawing.Point(154, 71);
            this.rbUnitsF.Name = "rbUnitsF";
            this.rbUnitsF.Size = new System.Drawing.Size(35, 17);
            this.rbUnitsF.TabIndex = 26;
            this.rbUnitsF.Text = "°F";
            this.rbUnitsF.UseVisualStyleBackColor = true;
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.pictureBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox2.Location = new System.Drawing.Point(10, 140);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(200, 100);
            this.pictureBox2.TabIndex = 40;
            this.pictureBox2.TabStop = false;
            // 
            // rbUnitsK
            // 
            this.rbUnitsK.AutoSize = true;
            this.rbUnitsK.Checked = true;
            this.rbUnitsK.Location = new System.Drawing.Point(76, 71);
            this.rbUnitsK.Name = "rbUnitsK";
            this.rbUnitsK.Size = new System.Drawing.Size(32, 17);
            this.rbUnitsK.TabIndex = 32;
            this.rbUnitsK.TabStop = true;
            this.rbUnitsK.Text = "K";
            this.rbUnitsK.UseVisualStyleBackColor = true;
            // 
            // rbUnitsC
            // 
            this.rbUnitsC.AutoSize = true;
            this.rbUnitsC.Location = new System.Drawing.Point(114, 71);
            this.rbUnitsC.Name = "rbUnitsC";
            this.rbUnitsC.Size = new System.Drawing.Size(36, 17);
            this.rbUnitsC.TabIndex = 25;
            this.rbUnitsC.Text = "°C";
            this.rbUnitsC.UseVisualStyleBackColor = true;
            // 
            // cbPal
            // 
            this.cbPal.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPal.FormattingEnabled = true;
            this.cbPal.Location = new System.Drawing.Point(57, 35);
            this.cbPal.Name = "cbPal";
            this.cbPal.Size = new System.Drawing.Size(154, 21);
            this.cbPal.TabIndex = 31;
            this.cbPal.SelectedIndexChanged += new System.EventHandler(this.cbPal_SelectedIndexChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(8, 38);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(43, 13);
            this.label13.TabIndex = 30;
            this.label13.Text = "Palette:";
            // 
            // panel2
            // 
            this.panel2.Location = new System.Drawing.Point(19, 393);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1095, 12);
            this.panel2.TabIndex = 33;
            // 
            // pictureBox3
            // 
            this.pictureBox3.Location = new System.Drawing.Point(12, 20);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(412, 312);
            this.pictureBox3.TabIndex = 34;
            this.pictureBox3.TabStop = false;
            // 
            // lblLeft
            // 
            this.lblLeft.AutoSize = true;
            this.lblLeft.Location = new System.Drawing.Point(315, 454);
            this.lblLeft.Name = "lblLeft";
            this.lblLeft.Size = new System.Drawing.Size(41, 13);
            this.lblLeft.TabIndex = 35;
            this.lblLeft.Text = "label14";
            // 
            // lblRight
            // 
            this.lblRight.AutoSize = true;
            this.lblRight.Location = new System.Drawing.Point(815, 454);
            this.lblRight.Name = "lblRight";
            this.lblRight.Size = new System.Drawing.Size(41, 13);
            this.lblRight.TabIndex = 36;
            this.lblRight.Text = "label15";
            // 
            // pictureBox4
            // 
            this.pictureBox4.Location = new System.Drawing.Point(432, 20);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(33, 312);
            this.pictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox4.TabIndex = 37;
            this.pictureBox4.TabStop = false;
            // 
            // pictureBox5
            // 
            this.pictureBox5.Location = new System.Drawing.Point(702, 20);
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.Size = new System.Drawing.Size(412, 312);
            this.pictureBox5.TabIndex = 38;
            this.pictureBox5.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(570, 454);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 13);
            this.label2.TabIndex = 39;
            this.label2.Text = "label15";
            // 
            // lblMaxTemp
            // 
            this.lblMaxTemp.AutoSize = true;
            this.lblMaxTemp.Location = new System.Drawing.Point(429, 5);
            this.lblMaxTemp.Name = "lblMaxTemp";
            this.lblMaxTemp.Size = new System.Drawing.Size(28, 13);
            this.lblMaxTemp.TabIndex = 41;
            this.lblMaxTemp.Text = "30.5";
            // 
            // lblMinTemp
            // 
            this.lblMinTemp.AutoSize = true;
            this.lblMinTemp.Location = new System.Drawing.Point(429, 335);
            this.lblMinTemp.Name = "lblMinTemp";
            this.lblMinTemp.Size = new System.Drawing.Size(28, 13);
            this.lblMinTemp.TabIndex = 42;
            this.lblMinTemp.Text = "20.5";
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(80, 6);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(60, 25);
            this.button4.TabIndex = 43;
            this.button4.Text = "INT Cal";
            this.toolTip2.SetToolTip(this.button4, "Do external calibration");
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1131, 447);
            this.Controls.Add(this.lblMinTemp);
            this.Controls.Add(this.lblMaxTemp);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pictureBox5);
            this.Controls.Add(this.pictureBox4);
            this.Controls.Add(this.lblRight);
            this.Controls.Add(this.lblLeft);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.lblSliderMax);
            this.Controls.Add(this.lblSliderMin);
            this.Controls.Add(this.trackBar1);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.trackBar2);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.panel1);
            this.Name = "Form1";
            this.Text = "SeekOFix v0.4";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.TrackBar trackBar2;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.CheckBox cbAutoSave;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.CheckBox cbDynSlidres;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.Label lblSliderMin;
        private System.Windows.Forms.Label lblSliderMax;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ToolTip toolTip2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.ComboBox cbPal;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.Label lblLeft;
        private System.Windows.Forms.Label lblRight;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.PictureBox pictureBox5;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label lblMaxTemp;
        private System.Windows.Forms.Label lblMinTemp;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton rbUnitsF;
        private System.Windows.Forms.RadioButton rbUnitsK;
        private System.Windows.Forms.RadioButton rbUnitsC;
        private System.Windows.Forms.Button button4;
    }
}

