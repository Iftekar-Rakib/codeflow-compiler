namespace CodeFlow
{
    partial class CompilerBeta
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CompilerBeta));
            this.button1 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.listBox = new System.Windows.Forms.ListBox();
            this.buttonParse = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.labelTitle = new System.Windows.Forms.Label();
            this.SuspendLayout();

            // ── TITLE LABEL ──
            this.labelTitle.Text = "⚡ CodeFlow Compiler";
            this.labelTitle.Font = new System.Drawing.Font("Consolas", 22F, System.Drawing.FontStyle.Bold);
            this.labelTitle.ForeColor = System.Drawing.Color.Cyan;
            this.labelTitle.BackColor = System.Drawing.Color.Transparent;
            this.labelTitle.Location = new System.Drawing.Point(450, 10);
            this.labelTitle.Size = new System.Drawing.Size(500, 50);
            this.labelTitle.Name = "labelTitle";

            // ── TEXTBOX ──
            this.textBox1.Font = new System.Drawing.Font("Consolas", 11F);
            this.textBox1.Location = new System.Drawing.Point(11, 70);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(1357, 125);
            this.textBox1.TabIndex = 1;
            this.textBox1.BackColor = System.Drawing.Color.FromArgb(15, 15, 40);
            this.textBox1.ForeColor = System.Drawing.Color.Cyan;
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);

            // ── TOKENIZE BUTTON ──
            this.button1.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Bold);
            this.button1.Location = new System.Drawing.Point(35, 210);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(180, 50);
            this.button1.TabIndex = 0;
            this.button1.Text = "⚡ Tokenize";
            this.button1.BackColor = System.Drawing.Color.FromArgb(0, 120, 215);
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.FlatAppearance.BorderColor = System.Drawing.Color.Cyan;
            this.button1.FlatAppearance.BorderSize = 2;
            this.button1.UseMnemonic = false;
            this.button1.Click += new System.EventHandler(this.buttonTokenize_Click);

            // ── PARSE BUTTON ──
            this.buttonParse.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Bold);
            this.buttonParse.Location = new System.Drawing.Point(240, 210);
            this.buttonParse.Name = "buttonParse";
            this.buttonParse.Size = new System.Drawing.Size(180, 50);
            this.buttonParse.TabIndex = 3;
            this.buttonParse.Text = "🚀 Parse";
            this.buttonParse.BackColor = System.Drawing.Color.FromArgb(0, 180, 100);
            this.buttonParse.ForeColor = System.Drawing.Color.White;
            this.buttonParse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonParse.FlatAppearance.BorderColor = System.Drawing.Color.LimeGreen;
            this.buttonParse.FlatAppearance.BorderSize = 2;
            this.buttonParse.Click += new System.EventHandler(this.HandleParseClick);

            // ── LISTBOX LEFT (tokens/parse) ──
            this.listBox.Font = new System.Drawing.Font("Consolas", 10F);
            this.listBox.FormattingEnabled = true;
            this.listBox.HorizontalScrollbar = true;
            this.listBox.ItemHeight = 20;
            this.listBox.Location = new System.Drawing.Point(11, 275);
            this.listBox.Name = "listBox";
            this.listBox.ScrollAlwaysVisible = true;
            this.listBox.Size = new System.Drawing.Size(670, 580);
            this.listBox.TabIndex = 2;
            this.listBox.BackColor = System.Drawing.Color.FromArgb(10, 10, 30);
            this.listBox.ForeColor = System.Drawing.Color.LightGreen;
            this.listBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listBox_KeyDown);

            // ── LISTBOX RIGHT (debug/memory) ──
            this.listBox1.Font = new System.Drawing.Font("Consolas", 10F);
            this.listBox1.FormattingEnabled = true;
            this.listBox1.HorizontalScrollbar = true;
            this.listBox1.ItemHeight = 20;
            this.listBox1.Location = new System.Drawing.Point(700, 275);
            this.listBox1.Name = "listBox1";
            this.listBox1.ScrollAlwaysVisible = true;
            this.listBox1.Size = new System.Drawing.Size(670, 580);
            this.listBox1.TabIndex = 4;
            this.listBox1.BackColor = System.Drawing.Color.FromArgb(10, 10, 30);
            this.listBox1.ForeColor = System.Drawing.Color.Orange;
            this.listBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listBox1_KeyDown);

            // ── FORM ──
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::CodeFlow.Properties.Resources.wallhaven_2ywd3y;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1383, 881);
            this.Controls.Add(this.labelTitle);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.buttonParse);
            this.Controls.Add(this.listBox);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button1);
            this.Name = "CompilerBeta";
            this.Text = "CodeFlow Compiler";
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }


        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ListBox listBox;
        private System.Windows.Forms.Button buttonParse;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Label labelTitle;
    }
}