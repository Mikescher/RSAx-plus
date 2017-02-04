namespace RSA_Algorithm
{
    partial class RSAxTestForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RSAxTestForm));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.StatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.comboBox_OAEP_Hash = new System.Windows.Forms.ComboBox();
            this.checkBox_OAEP = new System.Windows.Forms.CheckBox();
            this.textBox_MaxDataLength = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.button_ClearPT = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBox_ModulusSize = new System.Windows.Forms.ComboBox();
            this.checkBox_PrivateKey = new System.Windows.Forms.CheckBox();
            this.button_Decrypt = new System.Windows.Forms.Button();
            this.button_Encrypt = new System.Windows.Forms.Button();
            this.richTextBox_CipherText = new System.Windows.Forms.RichTextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.richTextBox_PlainText = new System.Windows.Forms.RichTextBox();
            this.label_PT = new System.Windows.Forms.Label();
            this.richTextBox_Key = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.checkBox_UseCRT = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.comboBox_Test_Iterations = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.richTextBox_Test = new System.Windows.Forms.RichTextBox();
            this.button_Test = new System.Windows.Forms.Button();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generateKeyPairToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 409);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(751, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // StatusLabel
            // 
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(39, 17);
            this.StatusLabel.Text = "Ready";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(751, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.generateKeyPairToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 24);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(751, 385);
            this.tabControl1.TabIndex = 2;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Controls.Add(this.textBox_MaxDataLength);
            this.tabPage1.Controls.Add(this.label8);
            this.tabPage1.Controls.Add(this.button_ClearPT);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.comboBox_ModulusSize);
            this.tabPage1.Controls.Add(this.checkBox_PrivateKey);
            this.tabPage1.Controls.Add(this.button_Decrypt);
            this.tabPage1.Controls.Add(this.button_Encrypt);
            this.tabPage1.Controls.Add(this.richTextBox_CipherText);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.richTextBox_PlainText);
            this.tabPage1.Controls.Add(this.label_PT);
            this.tabPage1.Controls.Add(this.richTextBox_Key);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(743, 359);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Test";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.comboBox_OAEP_Hash);
            this.groupBox1.Controls.Add(this.checkBox_OAEP);
            this.groupBox1.Location = new System.Drawing.Point(525, 268);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(197, 84);
            this.groupBox1.TabIndex = 16;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "OAEP";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(17, 49);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(31, 13);
            this.label9.TabIndex = 12;
            this.label9.Text = "Hash";
            // 
            // comboBox_OAEP_Hash
            // 
            this.comboBox_OAEP_Hash.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_OAEP_Hash.FormattingEnabled = true;
            this.comboBox_OAEP_Hash.Items.AddRange(new object[] {
            "SHA1",
            "SHA256",
            "SHA512"});
            this.comboBox_OAEP_Hash.Location = new System.Drawing.Point(54, 46);
            this.comboBox_OAEP_Hash.Name = "comboBox_OAEP_Hash";
            this.comboBox_OAEP_Hash.Size = new System.Drawing.Size(121, 21);
            this.comboBox_OAEP_Hash.TabIndex = 11;
            this.comboBox_OAEP_Hash.SelectedIndexChanged += new System.EventHandler(this.comboBox_ModulusSize_SelectedIndexChanged);
            // 
            // checkBox_OAEP
            // 
            this.checkBox_OAEP.AutoSize = true;
            this.checkBox_OAEP.Checked = true;
            this.checkBox_OAEP.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_OAEP.Location = new System.Drawing.Point(20, 21);
            this.checkBox_OAEP.Name = "checkBox_OAEP";
            this.checkBox_OAEP.Size = new System.Drawing.Size(155, 17);
            this.checkBox_OAEP.TabIndex = 8;
            this.checkBox_OAEP.Text = "Use OAEP (Recommended)";
            this.checkBox_OAEP.UseVisualStyleBackColor = true;
            this.checkBox_OAEP.CheckedChanged += new System.EventHandler(this.comboBox_ModulusSize_SelectedIndexChanged);
            // 
            // textBox_MaxDataLength
            // 
            this.textBox_MaxDataLength.Location = new System.Drawing.Point(362, 295);
            this.textBox_MaxDataLength.Name = "textBox_MaxDataLength";
            this.textBox_MaxDataLength.ReadOnly = true;
            this.textBox_MaxDataLength.Size = new System.Drawing.Size(110, 21);
            this.textBox_MaxDataLength.TabIndex = 15;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(359, 279);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(93, 13);
            this.label8.TabIndex = 14;
            this.label8.Text = "Max data (octets)";
            // 
            // button_ClearPT
            // 
            this.button_ClearPT.Location = new System.Drawing.Point(269, 148);
            this.button_ClearPT.Name = "button_ClearPT";
            this.button_ClearPT.Size = new System.Drawing.Size(18, 23);
            this.button_ClearPT.TabIndex = 13;
            this.button_ClearPT.Text = "x";
            this.button_ClearPT.UseVisualStyleBackColor = true;
            this.button_ClearPT.Click += new System.EventHandler(this.button_ClearPT_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label5.Location = new System.Drawing.Point(15, 337);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(468, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "If you use Public key for encryption. You have to use Private key for decryption " +
                "and vice-versa.";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(243, 279);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(68, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Modulus Size";
            // 
            // comboBox_ModulusSize
            // 
            this.comboBox_ModulusSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_ModulusSize.FormattingEnabled = true;
            this.comboBox_ModulusSize.Items.AddRange(new object[] {
            "512",
            "1024",
            "2048",
            "3072",
            "4096",
            "6144",
            "8192",
            "10240",
            "16384"});
            this.comboBox_ModulusSize.Location = new System.Drawing.Point(246, 295);
            this.comboBox_ModulusSize.Name = "comboBox_ModulusSize";
            this.comboBox_ModulusSize.Size = new System.Drawing.Size(104, 21);
            this.comboBox_ModulusSize.TabIndex = 10;
            this.comboBox_ModulusSize.SelectedIndexChanged += new System.EventHandler(this.comboBox_ModulusSize_SelectedIndexChanged);
            // 
            // checkBox_PrivateKey
            // 
            this.checkBox_PrivateKey.AutoSize = true;
            this.checkBox_PrivateKey.Location = new System.Drawing.Point(58, 316);
            this.checkBox_PrivateKey.Name = "checkBox_PrivateKey";
            this.checkBox_PrivateKey.Size = new System.Drawing.Size(102, 17);
            this.checkBox_PrivateKey.TabIndex = 9;
            this.checkBox_PrivateKey.Text = "Use Private Key";
            this.checkBox_PrivateKey.UseVisualStyleBackColor = true;
            // 
            // button_Decrypt
            // 
            this.button_Decrypt.Location = new System.Drawing.Point(118, 284);
            this.button_Decrypt.Name = "button_Decrypt";
            this.button_Decrypt.Size = new System.Drawing.Size(94, 23);
            this.button_Decrypt.TabIndex = 7;
            this.button_Decrypt.Text = "Decrypt";
            this.button_Decrypt.UseVisualStyleBackColor = true;
            this.button_Decrypt.Click += new System.EventHandler(this.button_Decrypt_Click);
            // 
            // button_Encrypt
            // 
            this.button_Encrypt.Location = new System.Drawing.Point(18, 284);
            this.button_Encrypt.Name = "button_Encrypt";
            this.button_Encrypt.Size = new System.Drawing.Size(94, 23);
            this.button_Encrypt.TabIndex = 6;
            this.button_Encrypt.Text = "Encrypt";
            this.button_Encrypt.UseVisualStyleBackColor = true;
            this.button_Encrypt.Click += new System.EventHandler(this.button_Encrypt_Click);
            // 
            // richTextBox_CipherText
            // 
            this.richTextBox_CipherText.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox_CipherText.Location = new System.Drawing.Point(308, 173);
            this.richTextBox_CipherText.Name = "richTextBox_CipherText";
            this.richTextBox_CipherText.Size = new System.Drawing.Size(414, 94);
            this.richTextBox_CipherText.TabIndex = 5;
            this.richTextBox_CipherText.Text = "";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(305, 157);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "CipherText";
            // 
            // richTextBox_PlainText
            // 
            this.richTextBox_PlainText.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox_PlainText.Location = new System.Drawing.Point(18, 173);
            this.richTextBox_PlainText.Name = "richTextBox_PlainText";
            this.richTextBox_PlainText.Size = new System.Drawing.Size(269, 94);
            this.richTextBox_PlainText.TabIndex = 3;
            this.richTextBox_PlainText.Text = "Edit the text here and use the different options to verify the results.";
            this.richTextBox_PlainText.TextChanged += new System.EventHandler(this.richTextBox_PlainText_TextChanged);
            // 
            // label_PT
            // 
            this.label_PT.AutoSize = true;
            this.label_PT.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_PT.Location = new System.Drawing.Point(15, 157);
            this.label_PT.Name = "label_PT";
            this.label_PT.Size = new System.Drawing.Size(60, 13);
            this.label_PT.TabIndex = 2;
            this.label_PT.Text = "PlainText";
            // 
            // richTextBox_Key
            // 
            this.richTextBox_Key.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox_Key.Location = new System.Drawing.Point(18, 24);
            this.richTextBox_Key.Name = "richTextBox_Key";
            this.richTextBox_Key.Size = new System.Drawing.Size(704, 118);
            this.richTextBox_Key.TabIndex = 1;
            this.richTextBox_Key.Text = resources.GetString("richTextBox_Key.Text");
            this.richTextBox_Key.WordWrap = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(15, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(28, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Key";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.checkBox_UseCRT);
            this.tabPage2.Controls.Add(this.label7);
            this.tabPage2.Controls.Add(this.comboBox_Test_Iterations);
            this.tabPage2.Controls.Add(this.label6);
            this.tabPage2.Controls.Add(this.richTextBox_Test);
            this.tabPage2.Controls.Add(this.button_Test);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(743, 359);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Performance";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // checkBox_UseCRT
            // 
            this.checkBox_UseCRT.AutoSize = true;
            this.checkBox_UseCRT.Checked = true;
            this.checkBox_UseCRT.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_UseCRT.Location = new System.Drawing.Point(614, 161);
            this.checkBox_UseCRT.Name = "checkBox_UseCRT";
            this.checkBox_UseCRT.Size = new System.Drawing.Size(67, 17);
            this.checkBox_UseCRT.TabIndex = 5;
            this.checkBox_UseCRT.Text = "Use CRT";
            this.checkBox_UseCRT.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(610, 97);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(54, 13);
            this.label7.TabIndex = 4;
            this.label7.Text = "Iterations";
            // 
            // comboBox_Test_Iterations
            // 
            this.comboBox_Test_Iterations.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_Test_Iterations.FormattingEnabled = true;
            this.comboBox_Test_Iterations.Items.AddRange(new object[] {
            "10",
            "50",
            "100",
            "200",
            "500",
            "1000",
            "10000"});
            this.comboBox_Test_Iterations.Location = new System.Drawing.Point(608, 113);
            this.comboBox_Test_Iterations.Name = "comboBox_Test_Iterations";
            this.comboBox_Test_Iterations.Size = new System.Drawing.Size(121, 21);
            this.comboBox_Test_Iterations.TabIndex = 3;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(19, 22);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(92, 13);
            this.label6.TabIndex = 2;
            this.label6.Text = "Performance Test";
            // 
            // richTextBox_Test
            // 
            this.richTextBox_Test.Location = new System.Drawing.Point(22, 38);
            this.richTextBox_Test.Name = "richTextBox_Test";
            this.richTextBox_Test.Size = new System.Drawing.Size(569, 286);
            this.richTextBox_Test.TabIndex = 1;
            this.richTextBox_Test.Text = "";
            // 
            // button_Test
            // 
            this.button_Test.Location = new System.Drawing.Point(608, 38);
            this.button_Test.Name = "button_Test";
            this.button_Test.Size = new System.Drawing.Size(121, 23);
            this.button_Test.TabIndex = 0;
            this.button_Test.Text = "Begin Test";
            this.button_Test.UseVisualStyleBackColor = true;
            this.button_Test.Click += new System.EventHandler(this.button_Test_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // generateKeyPairToolStripMenuItem
            // 
            this.generateKeyPairToolStripMenuItem.Name = "generateKeyPairToolStripMenuItem";
            this.generateKeyPairToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.generateKeyPairToolStripMenuItem.Text = "Generate Key Pair";
            this.generateKeyPairToolStripMenuItem.Click += new System.EventHandler(this.generateKeyPairToolStripMenuItem_Click);
            // 
            // RSAxTestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(751, 431);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "RSAxTestForm";
            this.Text = "RSAx Test - ArpanTECH - 2012";
            this.Load += new System.EventHandler(this.RSAxTestForm_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabel;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.RichTextBox richTextBox_CipherText;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RichTextBox richTextBox_PlainText;
        private System.Windows.Forms.Label label_PT;
        private System.Windows.Forms.RichTextBox richTextBox_Key;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBox_PrivateKey;
        private System.Windows.Forms.CheckBox checkBox_OAEP;
        private System.Windows.Forms.Button button_Decrypt;
        private System.Windows.Forms.Button button_Encrypt;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboBox_ModulusSize;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button button_ClearPT;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.RichTextBox richTextBox_Test;
        private System.Windows.Forms.Button button_Test;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox comboBox_Test_Iterations;
        private System.Windows.Forms.CheckBox checkBox_UseCRT;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBox_MaxDataLength;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox comboBox_OAEP_Hash;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generateKeyPairToolStripMenuItem;
    }
}

