// @Date : 15th July 2012
// @Author : Arpan Jati (arpan4017@yahoo.com; arpan4017@gmail.com)
// @Application : RSAx Test Application
// @CodeProject: http://www.codeproject.com/Articles/421656/RSA-Library-with-Private-Key-Encryption-in-Csharp  

using ArpanTECH;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace RSA_Algorithm
{
	public partial class RSAxTestForm : Form
    {
        public RSAxTestForm()
        {
            InitializeComponent();
        }

        void ButtonState(bool state)
        {
            button_ClearPT.Enabled = state;
            button_Encrypt.Enabled = state;
            button_Decrypt.Enabled = state;
        }

        RSAxParameters.RSAxHashAlgorithm [] ha_types = new RSAxParameters.RSAxHashAlgorithm [] 
        {
            RSAxParameters.RSAxHashAlgorithm.SHA1, 
            RSAxParameters.RSAxHashAlgorithm.SHA256,
            RSAxParameters.RSAxHashAlgorithm.SHA512
        };

        int[] hLens = new int[] { 20, 32, 64 };

        private void button_Encrypt_Click(object sender, EventArgs e)
        {
            ButtonState(false);
            try
            {                
                RSAx rsax = new RSAx(richTextBox_Key.Text, int.Parse(comboBox_ModulusSize.Text));
                rsax.RSAxHashAlgorithm = ha_types[comboBox_OAEP_Hash.SelectedIndex];
                byte[] CT = rsax.Encrypt(Encoding.UTF8.GetBytes(richTextBox_PlainText.Text), checkBox_PrivateKey.Checked, checkBox_OAEP.Checked);
                richTextBox_CipherText.Text = Convert.ToBase64String(CT); 

                
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Exception while Encryption: " + ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            ButtonState(true);
        }

        private void button_Decrypt_Click(object sender, EventArgs e)
        {
            ButtonState(false);
            try
            {
                RSAx rsax = new RSAx(richTextBox_Key.Text, int.Parse(comboBox_ModulusSize.Text));
                rsax.RSAxHashAlgorithm = ha_types[comboBox_OAEP_Hash.SelectedIndex];
                byte[] PT = rsax.Decrypt(Convert.FromBase64String(richTextBox_CipherText.Text), checkBox_PrivateKey.Checked, checkBox_OAEP.Checked);
                richTextBox_PlainText.Text = Encoding.UTF8.GetString(PT);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Exception while Decryption: " + ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            ButtonState(true);
        }

        private void RSAxTestForm_Load(object sender, EventArgs e)
        {
            comboBox_ModulusSize.SelectedIndex = 1;
            comboBox_Test_Iterations.SelectedIndex = 2;
            comboBox_OAEP_Hash.SelectedIndex = 0;
            ProcessLengthChange();
        }

        private void button_ClearPT_Click(object sender, EventArgs e)
        {
            richTextBox_PlainText.Text = "";
        }

        private void button_Test_Click(object sender, EventArgs e)
        {
            richTextBox_Test.Clear();

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(int.Parse(comboBox_ModulusSize.Text));
            rsa.FromXmlString(richTextBox_Key.Text);

            RSAxParameters param = RSAxUtils.GetRSAxParameters(richTextBox_Key.Text, int.Parse(comboBox_ModulusSize.Text));
            RSAx rsax = new RSAx(param);
            rsax.UseCrtForPublicDecryption = checkBox_UseCRT.Checked;
            rsax.RSAxHashAlgorithm = ha_types[comboBox_OAEP_Hash.SelectedIndex];

            int hLen = hLens[comboBox_OAEP_Hash.SelectedIndex];
            int maxLength = 0;
            int mod_sz = int.Parse(comboBox_ModulusSize.Text);           

            if (checkBox_OAEP.Checked)
            {
                maxLength = (mod_sz / 8) - 2 * hLen - 2;
            }
            else
            {
                maxLength = (mod_sz / 8) - 11;
            }

            if (maxLength >= 0)
            {
                Random r = new Random();
                Stopwatch sw = new Stopwatch();
                sw.Start();
                int iterations = int.Parse(comboBox_Test_Iterations.Text);
                bool error = false;
                for (int i = 0; i < iterations; i++)
                {
                    byte[] da = new byte[maxLength]; // 86 for modulus of 1024.
                    r.NextBytes(da);
                    try
                    {
                        byte[] CTX = rsax.Encrypt(da, checkBox_OAEP.Checked);
                        byte[] PTX = rsax.Decrypt(CTX, checkBox_OAEP.Checked);
                        bool ok = true;
                        for (int j = 0; j < da.Length; j++)
                        {
                            if (da[j] != PTX[j])
                            {
                                ok = false;
                                break;
                            }
                        }

                        richTextBox_Test.AppendText("\nIteration: " + i + ", MATCH: " + ok + ", Time: " + sw.ElapsedMilliseconds + " ms\n");
                        richTextBox_Test.ScrollToCaret();

                        //Application.DoEvents();
                    }
                    catch (System.Exception ex)
                    {
                        richTextBox_Test.Text += "\nException: " + ex.Message;
                        error = true;
                    }
                }

                if (!error)
                {
                    richTextBox_Test.Text += "\n\nTest Successful\n" + iterations + " Iterations took " + sw.ElapsedMilliseconds + " ms" +
                        "\n\n Average Time: " + (sw.ElapsedMilliseconds / iterations) + " ms per Encryption and Decryption";
                }
                else
                {
                    richTextBox_Test.Text += "\n\nTest Failed";
                }    
            }
            else
            {
                MessageBox.Show("Invalid Settings detected. Please check the settings in the 'Test' tab.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand);           
            }
        }

        private void comboBox_ModulusSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            ProcessLengthChange();           
        }

        private void ProcessLengthChange()
        {
            try
            {               
                int hLen = hLens[comboBox_OAEP_Hash.SelectedIndex];
                int maxLength = 0;
                int mod_sz = int.Parse(comboBox_ModulusSize.Text);
                int pt_len = richTextBox_PlainText.Text.Length;
                
                if (checkBox_OAEP.Checked)
                {
                    maxLength = (mod_sz/8) - 2 * hLen - 2;                    
                }
                else
                {
                    maxLength = (mod_sz / 8) - 11;
                }

                if (pt_len > maxLength)
                {
                     label_PT.ForeColor = Color.Red;
                }
                else
                {
                    label_PT.ForeColor = Color.DarkGreen;
                }

                textBox_MaxDataLength.Text = maxLength + "";
            }
            catch { }
        }

        private void richTextBox_PlainText_TextChanged(object sender, EventArgs e)
        {
            label_PT.Text = "PlainText (" + richTextBox_PlainText.Text.Length + ")"; 
            ProcessLengthChange();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBoxMain abb = new AboutBoxMain();
            abb.ShowDialog();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void generateKeyPairToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int dwLen = int.Parse(comboBox_ModulusSize.Text);
            if(DialogResult.Yes == MessageBox.Show("Do you want to generate a " + dwLen + " bit Key-Pair?\nFor long keys it may take a while, please be patient.", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                this.Enabled = false;
                RSACryptoServiceProvider csp = new RSACryptoServiceProvider(dwLen);
                richTextBox_Key.Text = csp.ToXmlString(true).Replace("><", ">\r\n<");
                this.Enabled = true;
                MessageBox.Show("Key-Pair Updated", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Question);
            }            
        }
    }
}
