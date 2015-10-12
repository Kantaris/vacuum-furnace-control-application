namespace FurnaceController
{
    partial class Setup
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.sync1 = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.sync2 = new System.Windows.Forms.NumericUpDown();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.brazetime = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.brazetemp = new System.Windows.Forms.NumericUpDown();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label10 = new System.Windows.Forms.Label();
            this.speed3 = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.speed2 = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.speed1 = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.pressure3 = new System.Windows.Forms.NumericUpDown();
            this.pressure2 = new System.Windows.Forms.NumericUpDown();
            this.pressure1 = new System.Windows.Forms.NumericUpDown();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sync1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sync2)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.brazetime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.brazetemp)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pressure3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pressure2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pressure1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.sync1);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.sync2);
            this.groupBox1.Location = new System.Drawing.Point(6, 19);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(281, 77);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Temperature sync levels";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(17, 21);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(88, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "Sync level 1 (°C):";
            // 
            // sync1
            // 
            this.sync1.Location = new System.Drawing.Point(113, 19);
            this.sync1.Maximum = new decimal(new int[] {
            1200,
            0,
            0,
            0});
            this.sync1.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.sync1.Name = "sync1";
            this.sync1.Size = new System.Drawing.Size(155, 20);
            this.sync1.TabIndex = 6;
            this.sync1.Value = new decimal(new int[] {
            1060,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 47);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Sync level 2 (°C):";
            // 
            // sync2
            // 
            this.sync2.Location = new System.Drawing.Point(113, 45);
            this.sync2.Maximum = new decimal(new int[] {
            1200,
            0,
            0,
            0});
            this.sync2.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.sync2.Name = "sync2";
            this.sync2.Size = new System.Drawing.Size(155, 20);
            this.sync2.TabIndex = 2;
            this.sync2.Value = new decimal(new int[] {
            1060,
            0,
            0,
            0});
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.brazetime);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.brazetemp);
            this.groupBox2.Location = new System.Drawing.Point(6, 235);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(302, 93);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Brazing";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 54);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(104, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Braze time (minutes):";
            // 
            // brazetime
            // 
            this.brazetime.Location = new System.Drawing.Point(134, 52);
            this.brazetime.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.brazetime.Name = "brazetime";
            this.brazetime.Size = new System.Drawing.Size(155, 20);
            this.brazetime.TabIndex = 4;
            this.brazetime.Value = new decimal(new int[] {
            70,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 28);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(116, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Braze temperature (°C):";
            // 
            // brazetemp
            // 
            this.brazetemp.Location = new System.Drawing.Point(134, 26);
            this.brazetemp.Maximum = new decimal(new int[] {
            1200,
            0,
            0,
            0});
            this.brazetemp.Minimum = new decimal(new int[] {
            1060,
            0,
            0,
            0});
            this.brazetemp.Name = "brazetemp";
            this.brazetemp.Size = new System.Drawing.Size(155, 20);
            this.brazetemp.TabIndex = 2;
            this.brazetemp.Value = new decimal(new int[] {
            1121,
            0,
            0,
            0});
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.speed3);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.speed2);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.speed1);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Location = new System.Drawing.Point(6, 102);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(352, 127);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Heat up speed";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(17, 97);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(169, 13);
            this.label10.TabIndex = 15;
            this.label10.Text = "Between sync level 2 and brazing:";
            // 
            // speed3
            // 
            this.speed3.FormattingEnabled = true;
            this.speed3.Items.AddRange(new object[] {
            "0",
            "0.5",
            "1",
            "1.5",
            "2",
            "2.5",
            "3.0",
            "3.5",
            "4.0",
            "4.5",
            "5.0",
            "5.5",
            "6.0",
            "6.5",
            "7.0",
            "7.5",
            "8.0",
            "8.5",
            "9.0",
            "9.5",
            "10.0",
            "Full Speed"});
            this.speed3.Location = new System.Drawing.Point(214, 94);
            this.speed3.Name = "speed3";
            this.speed3.Size = new System.Drawing.Size(119, 21);
            this.speed3.TabIndex = 14;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(17, 70);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(191, 13);
            this.label7.TabIndex = 11;
            this.label7.Text = "Between sync level 1 and sync level 2:";
            // 
            // speed2
            // 
            this.speed2.FormattingEnabled = true;
            this.speed2.Items.AddRange(new object[] {
            "0",
            "0.5",
            "1",
            "1.5",
            "2",
            "2.5",
            "3.0",
            "3.5",
            "4.0",
            "4.5",
            "5.0",
            "5.5",
            "6.0",
            "6.5",
            "7.0",
            "7.5",
            "8.0",
            "8.5",
            "9.0",
            "9.5",
            "10.0",
            "Full Speed"});
            this.speed2.Location = new System.Drawing.Point(214, 67);
            this.speed2.Name = "speed2";
            this.speed2.Size = new System.Drawing.Size(119, 21);
            this.speed2.TabIndex = 10;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(17, 43);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(155, 13);
            this.label9.TabIndex = 9;
            this.label9.Text = "Between start and sync level 1:";
            // 
            // speed1
            // 
            this.speed1.FormattingEnabled = true;
            this.speed1.Items.AddRange(new object[] {
            "0.5",
            "1",
            "1.5",
            "2",
            "2.5",
            "3.0",
            "3.5",
            "4.0",
            "4.5",
            "5.0",
            "5.5",
            "6.0",
            "6.5",
            "7.0",
            "7.5",
            "8.0",
            "8.5",
            "9.0",
            "9.5",
            "10.0",
            "Full Speed"});
            this.speed1.Location = new System.Drawing.Point(214, 40);
            this.speed1.Name = "speed1";
            this.speed1.Size = new System.Drawing.Size(119, 21);
            this.speed1.TabIndex = 8;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(17, 21);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(222, 13);
            this.label6.TabIndex = 7;
            this.label6.Text = "Max temperature increase per minute (°C/min)";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.groupBox1);
            this.groupBox4.Controls.Add(this.groupBox2);
            this.groupBox4.Controls.Add(this.groupBox3);
            this.groupBox4.Location = new System.Drawing.Point(12, 12);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(396, 339);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Heating";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.groupBox8);
            this.groupBox5.Location = new System.Drawing.Point(425, 12);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(396, 136);
            this.groupBox5.TabIndex = 4;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Cooling";
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.pressure3);
            this.groupBox8.Controls.Add(this.pressure2);
            this.groupBox8.Controls.Add(this.pressure1);
            this.groupBox8.Controls.Add(this.label17);
            this.groupBox8.Controls.Add(this.label18);
            this.groupBox8.Controls.Add(this.label19);
            this.groupBox8.Location = new System.Drawing.Point(6, 19);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(352, 103);
            this.groupBox8.TabIndex = 2;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Max pressure";
            // 
            // pressure3
            // 
            this.pressure3.DecimalPlaces = 1;
            this.pressure3.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.pressure3.Location = new System.Drawing.Point(185, 73);
            this.pressure3.Maximum = new decimal(new int[] {
            35,
            0,
            0,
            65536});
            this.pressure3.Name = "pressure3";
            this.pressure3.Size = new System.Drawing.Size(148, 20);
            this.pressure3.TabIndex = 18;
            this.pressure3.Value = new decimal(new int[] {
            23,
            0,
            0,
            65536});
            // 
            // pressure2
            // 
            this.pressure2.DecimalPlaces = 1;
            this.pressure2.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.pressure2.Location = new System.Drawing.Point(185, 47);
            this.pressure2.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.pressure2.Name = "pressure2";
            this.pressure2.Size = new System.Drawing.Size(148, 20);
            this.pressure2.TabIndex = 17;
            this.pressure2.Value = new decimal(new int[] {
            18,
            0,
            0,
            65536});
            // 
            // pressure1
            // 
            this.pressure1.DecimalPlaces = 1;
            this.pressure1.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.pressure1.Location = new System.Drawing.Point(185, 21);
            this.pressure1.Maximum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.pressure1.Name = "pressure1";
            this.pressure1.Size = new System.Drawing.Size(148, 20);
            this.pressure1.TabIndex = 16;
            this.pressure1.Value = new decimal(new int[] {
            15,
            0,
            0,
            65536});
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(17, 75);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(162, 13);
            this.label17.TabIndex = 13;
            this.label17.Text = "Between 850°C and 150°C (Bar):";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(17, 49);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(162, 13);
            this.label18.TabIndex = 11;
            this.label18.Text = "Between 930°C and 850°C (Bar):";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(17, 23);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(168, 13);
            this.label19.TabIndex = 9;
            this.label19.Text = "Between 1020°C and 930°C (Bar):";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(762, 329);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 5;
            this.button1.Text = "Cancel";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(616, 329);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(140, 23);
            this.button2.TabIndex = 6;
            this.button2.Text = "Use these settings";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Setup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(849, 364);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(865, 402);
            this.MinimumSize = new System.Drawing.Size(865, 402);
            this.Name = "Setup";
            this.Text = "Configure";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sync1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sync2)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.brazetime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.brazetemp)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pressure3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pressure2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pressure1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown sync1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown sync2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown brazetime;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown brazetemp;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ComboBox speed1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox speed3;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox speed2;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.NumericUpDown pressure1;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.NumericUpDown pressure3;
        private System.Windows.Forms.NumericUpDown pressure2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}