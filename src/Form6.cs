using System;
using System.Windows.Forms;

namespace Bills {
    public partial class Form6 : Form {
        bool billEntry = true;
        public Form6(bool erase) {
            InitializeComponent();
            billEntry = erase;
        }
        private void Form6_Load(object sender, EventArgs e) {
            if (!billEntry) {
                label1.Text = "Selected entry will be deleted!";
                label1.Location = new System.Drawing.Point(30, 16);
                this.Text = "Erase Entry";
            }
        }
    }
}
