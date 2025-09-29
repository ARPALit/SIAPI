using Arpal.SiApi.Utils;

namespace Arpal.SiApi.WinForm
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void generateSha256_button_Click(object sender, EventArgs e)
        {
            sha512_textBox.Text = HashManager.ComputeHash(password_textBox.Text);
        }
    }
}
