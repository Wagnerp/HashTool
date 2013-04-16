/*
 
    Author : Anurag Saini
 
*/


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.IO;
using System.Threading;

namespace HashTool {
    public partial class MainFrame : Form {

        string txt = "", filePath = "";
        byte[] inArr;
        int bufferSize = 1024, bufferDivide = 100;
        long fileSize = 0;

        MD5CryptoServiceProvider serviceMD5;
        SHA1CryptoServiceProvider serviceSHA1;
        OpenFileDialog diag;
        FileStream stream;

        public MainFrame() {
            InitializeComponent();

            serviceMD5  = new MD5CryptoServiceProvider();
            serviceSHA1 = new SHA1CryptoServiceProvider();
            diag = new OpenFileDialog();
        }

        private void button1_Click(object sender, EventArgs e) {
            txt = inputTextBox.Text;
            if(txt.Length > 0) {

                inArr = System.Text.Encoding.ASCII.GetBytes(txt);
                textBox2.Text =  System.BitConverter.ToString(serviceMD5.ComputeHash(inArr)).Replace("-", "").ToLowerInvariant();
                textBox3.Text = System.BitConverter.ToString(serviceSHA1.ComputeHash(inArr)).Replace("-", "").ToLowerInvariant();
            }
            else {
                textBox2.Text = "";
                textBox3.Text = "";
                txt = "";
            }
        }

        private void button4_Click(object sender, EventArgs e) {

            if(diag.ShowDialog() == DialogResult.OK) {

                textBox5.Text = "";
                textBox6.Text = "";

                filePath = diag.FileName;
                if(filePath.Length > 40) { locationTextBox.Text = filePath.Substring(0, 20) + "[...]" + filePath.Substring(diag.FileName.Length - 20, 20); }
                else { locationTextBox.Text = filePath; }
                stream = new FileStream(diag.FileName, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite);
                fileSize = stream.Length;

                if(fileSize <= 0) {
                    locationTextBox.Text = "";
                    filePath = "";
                    MessageBox.Show("Empty File!");                    
                }
                else if(fileSize < 1024) {
                    bufferSize = (int)(fileSize / 1);
                    progressBar1.Maximum = (int)(fileSize / bufferSize);
                    progressBar2.Maximum = progressBar1.Maximum;
                    stream.Close();
                }
                else {
                    bufferSize = (int)(fileSize / bufferDivide);
                    progressBar1.Maximum = (int)(fileSize / bufferSize);
                    progressBar2.Maximum = progressBar1.Maximum;
                    stream.Close();
                }
            }
        }


        private void button2_Click(object sender, EventArgs e) {
            if(txt.Length > 0) {
                System.Windows.Forms.Clipboard.SetText(textBox2.Text);
            }
        }

        private void button3_Click(object sender, EventArgs e) {
            if(txt.Length > 0) {
                System.Windows.Forms.Clipboard.SetText(textBox3.Text);
            }
        }

        public void moveProgressBar(ProgressBar p) {
            if(p.Value < 100) { p.Value += 1; }
            else { p.Value = 0; }
        }

        private void button5_Click(object sender, EventArgs e) {
            if(filePath.Length > 0) {
                progressBar1.Show();
                BrowseButton.Enabled = false;
                fMD5Button.Enabled = false;
                fSHAButton.Enabled = false;
                BackgroundWorker getFileMD5 = new BackgroundWorker();
                getFileMD5.DoWork += new DoWorkEventHandler(MD5_DoWork);
                getFileMD5.RunWorkerCompleted += new RunWorkerCompletedEventHandler(MD5_Completed);
                getFileMD5.RunWorkerAsync();
            }
        }

        public void MD5_DoWork(object sender, DoWorkEventArgs e) {
            stream = new FileStream(diag.FileName, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite);
            byte[] buffer = new byte[bufferSize];
            int readCount;
            HashAlgorithm algorithm = MD5.Create();

            while((readCount = stream.Read(buffer, 0, bufferSize)) > 0) {
                algorithm.TransformBlock(buffer, 0, readCount, buffer, 0);
                progressBar1.Invoke((MethodInvoker)delegate() {
                    if(progressBar1.Value < progressBar1.Maximum)
                        progressBar1.Value += 1;
                    else
                        progressBar1.Value = 0;
                });
            }
            algorithm.TransformFinalBlock(buffer, 0, readCount);
            string result = System.BitConverter.ToString(algorithm.Hash).Replace("-", "");            
            e.Result = result;
        }

        public void MD5_Completed(object sender, RunWorkerCompletedEventArgs e) {
            stream.Close();
            progressBar1.Value = 0;
            progressBar1.Hide();
            textBox5.Text = ((string)e.Result).ToLowerInvariant();
            BrowseButton.Enabled = true;
            fMD5Button.Enabled = true;
            fSHAButton.Enabled = true;

            System.Windows.Forms.Clipboard.SetText(textBox5.Text);
            MessageBox.Show("MD5 Hash of file copied to clipboard");
        }





        private void button6_Click(object sender, EventArgs e) {
            if(filePath.Length > 0) {
                if(filePath.Length > 0) {
                    progressBar2.Show();                        
                    BrowseButton.Enabled = false;
                    fMD5Button.Enabled = false;
                    fSHAButton.Enabled = false;
                    BackgroundWorker getFileSHA1 = new BackgroundWorker();
                    getFileSHA1.DoWork += new DoWorkEventHandler(SHA1_DoWork);
                    getFileSHA1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(SHA1_Completed);
                    getFileSHA1.RunWorkerAsync();
                }
            }
        }

        public void SHA1_DoWork(object sender, DoWorkEventArgs e) {
            stream = new FileStream(diag.FileName, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite);
            byte[] buffer = new byte[bufferSize];
            int readCount;
            HashAlgorithm algorithm = SHA1.Create();

            while((readCount = stream.Read(buffer, 0, bufferSize)) > 0) {
                algorithm.TransformBlock(buffer, 0, readCount, buffer, 0);
                progressBar2.Invoke((MethodInvoker)delegate() {
                    if(progressBar2.Value < progressBar1.Maximum)
                        progressBar2.Value += 1;
                    else
                        progressBar2.Value = 0;
                });
            }
            algorithm.TransformFinalBlock(buffer, 0, readCount);
            string result = System.BitConverter.ToString(algorithm.Hash).Replace("-", "");
            e.Result = result;
        }

        public void SHA1_Completed(object sender, RunWorkerCompletedEventArgs e) {
            stream.Close();
            progressBar2.Value = 0;
            progressBar2.Hide();
            textBox6.Text = ((string)e.Result).ToLowerInvariant();
            BrowseButton.Enabled = true;
            fMD5Button.Enabled = true;
            fSHAButton.Enabled = true;

            System.Windows.Forms.Clipboard.SetText(textBox6.Text);
            MessageBox.Show("SHA1 Hash of file copied to clipboard");
        }

        private void Form1_Load(object sender, EventArgs e) {

        }

    }
}
