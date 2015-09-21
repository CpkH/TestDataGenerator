using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace TestDataGenerator
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private void buttonSelectFile_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() != DialogResult.OK)
            {
                return;
            }
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            textBoxFilePath.Text = openFileDialog1.FileName;
        }
    }
}