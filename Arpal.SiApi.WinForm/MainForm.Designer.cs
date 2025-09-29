namespace Arpal.SiApi.WinForm
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            password_label = new Label();
            password_textBox = new TextBox();
            generateSha512_button = new Button();
            sha512_label = new Label();
            sha512_textBox = new TextBox();
            SuspendLayout();
            // 
            // password_label
            // 
            password_label.AutoSize = true;
            password_label.Font = new Font("Segoe UI", 12F);
            password_label.Location = new Point(35, 52);
            password_label.Name = "password_label";
            password_label.Size = new Size(79, 21);
            password_label.TabIndex = 0;
            password_label.Text = "Passowrd:";
            // 
            // password_textBox
            // 
            password_textBox.Font = new Font("Segoe UI", 12F);
            password_textBox.Location = new Point(120, 49);
            password_textBox.Name = "password_textBox";
            password_textBox.Size = new Size(540, 29);
            password_textBox.TabIndex = 1;
            // 
            // generateSha512_button
            // 
            generateSha512_button.BackColor = SystemColors.Highlight;
            generateSha512_button.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            generateSha512_button.ForeColor = SystemColors.ButtonHighlight;
            generateSha512_button.Location = new Point(167, 116);
            generateSha512_button.Name = "generateSha512_button";
            generateSha512_button.Size = new Size(398, 38);
            generateSha512_button.TabIndex = 2;
            generateSha512_button.Text = "GENERA SHA-512";
            generateSha512_button.UseVisualStyleBackColor = false;
            generateSha512_button.Click += generateSha256_button_Click;
            // 
            // sha512_label
            // 
            sha512_label.AutoSize = true;
            sha512_label.Font = new Font("Segoe UI", 12F);
            sha512_label.Location = new Point(38, 226);
            sha512_label.Name = "sha512_label";
            sha512_label.Size = new Size(76, 21);
            sha512_label.TabIndex = 3;
            sha512_label.Text = "SHA-512:";
            // 
            // sha512_textBox
            // 
            sha512_textBox.Font = new Font("Segoe UI", 12F);
            sha512_textBox.Location = new Point(120, 226);
            sha512_textBox.Multiline = true;
            sha512_textBox.Name = "sha512_textBox";
            sha512_textBox.Size = new Size(540, 139);
            sha512_textBox.TabIndex = 4;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(728, 377);
            Controls.Add(sha512_textBox);
            Controls.Add(sha512_label);
            Controls.Add(generateSha512_button);
            Controls.Add(password_textBox);
            Controls.Add(password_label);
            Name = "MainForm";
            Text = "SiApi";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label password_label;
        private TextBox password_textBox;
        private Button generateSha512_button;
        private Label sha512_label;
        private TextBox sha512_textBox;
    }
}
