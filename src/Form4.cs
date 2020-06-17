using System;
using System.Windows.Forms;
namespace Bills {
    public partial class Form4 : Form {
        private ComboBox comboOne;
        bool doEdit = false;
        string editingString = "";
        public Form4(ComboBox combo, bool editing) {
            InitializeComponent();
            comboOne = combo;
            doEdit = editing;
        }
        private void Form4_Load(object sender, EventArgs e) {
            if (doEdit) {
                this.Text = "Edit Bill";
                button1.Text = "Save";
                editingString = comboOne.Text;
                textBox2.Text = editingString;      
            }
        }
        private void button1_Click(object sender, EventArgs e) {
            string billName = textBox2.Text;
            if (billName == "") MessageBox.Show("Please enter a bill name.", "Error!");
            else if (comboOne.Items.Contains(billName) && !doEdit) MessageBox.Show("Entry already exists.", "Error!");
            else if (billName == "All") MessageBox.Show("Cannot name bill \"All.\" Choose another name.", "Error!");
            else {
                bills newBills = new bills();
                if (!doEdit) newBills.writeBill(bills.billLocation, billName);
                else {
                    newBills.editID(editingString, billName);
                    comboOne.Items.Remove(editingString);
                }
                comboOne.Items.Add(billName);
                comboOne.SelectedItem = billName;
                this.Close();
            }
         }
    }
}
