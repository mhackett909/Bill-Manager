using System;
using System.Windows.Forms;
namespace Bills {
    public partial class Form2 : Form {
        private Form1 myForm;
        private bills newBills;
        public Form2(Form1 otherForm, bills importBills) {
            InitializeComponent();
            myForm = otherForm;
            newBills = importBills;
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e) {
            if (checkBox1.Checked) {
                radioButton1.Enabled = true;
                radioButton2.Enabled = true;
                radioButton3.Enabled = true;
                radioButton4.Enabled = true;
                radioButton5.Enabled = true;
                radioButton6.Enabled = true;
            }
            else {
                radioButton1.Enabled = false;
                radioButton2.Enabled = false;
                radioButton3.Enabled = false;
                radioButton4.Enabled = false;
                radioButton5.Enabled = false;
                radioButton6.Enabled = false;
            }
        }
        private void button1_Click(object sender, EventArgs e) {
            DateTime theDate;
            try { theDate = Convert.ToDateTime(textBox1.Text); }
            catch (Exception) {
                MessageBox.Show("Date is invalid. Try MM/DD/YYYY", "Error!");
                return;
            }
            try { double mydub = Convert.ToDouble(textBox2.Text); }
            catch (Exception) {
                MessageBox.Show("Amount is invalid", "Error!");
                return;
            }
            if (checkBox1.Checked && paymentType() == "Unpaid") MessageBox.Show("Please select payment type or uncheck \"Paid\"", "Error!");
            else if (textBox1.Text.Contains(":")) MessageBox.Show("Date is invalid. Try MM/DD/YYYY", "Error!");
            else if (textBox2.Text.Contains(",") || textBox2.Text.Contains("e")) MessageBox.Show("Amount is invalid.", "Error!");
            else if (textBox3.Text.Length > 90) MessageBox.Show("Notes too long. Please keep under 90 characters.", "Error!");
            else {
                string truncatedAmount = newBills.fixDouble(textBox2.Text), 
                    truncatedDate = theDate.ToString().Substring(0, theDate.ToString().IndexOf(" "));
                string payType = (checkBox1.Checked ? paymentType() : "Unpaid");
                bool addNew = (radioButton7.Checked ? true : false), doEdit = (radioButton8.Checked ? true : false);
                if (addNew && myForm.isMatch(truncatedDate, truncatedAmount)) {
                        this.Close();
                        Form5 matchForm = new Form5();
                        matchForm.ShowDialog();
                        if (matchForm.DialogResult == DialogResult.OK) addNewEntry(truncatedDate, truncatedAmount, payType, textBox3.Text, doEdit);
                }
                else addNewEntry(truncatedDate, truncatedAmount, payType, textBox3.Text, doEdit);
            }
        }
        private void radioButton8_CheckedChanged(object sender, EventArgs e) {
            if (radioButton8.Checked) {
                try {
                    if (myForm.dataGridView1.Rows[myForm.selectedElem].Cells[0].Value.ToString() == "") {
                        MessageBox.Show("Selected entry is empty.");
                        radioButton7.Checked = true;
                    }
                    else fillForm2();
                }
                catch (Exception) {
                    MessageBox.Show("Selected entry is empty.");
                    radioButton7.Checked = true;
                }
            }
        }
        private void radioButton7_CheckedChanged(object sender, EventArgs e) {
            if (radioButton7.Checked) resetForm2();
        }
        public void resetForm2() {
            if (!radioButton7.Checked) radioButton7.Checked = true;
            radioButton1.Enabled = false;
            radioButton2.Enabled = false;
            radioButton3.Enabled = false;
            radioButton4.Enabled = false;
            radioButton5.Enabled = false;
            radioButton6.Enabled = false;
            checkBox1.Checked = false;
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
        }
        public void fillForm2() {
            int rowNum = myForm.selectedElem;
            textBox1.Text = myForm.dataGridView1.Rows[rowNum].Cells[0].Value.ToString();
            textBox2.Text = myForm.dataGridView1.Rows[rowNum].Cells[1].Value.ToString();
            textBox3.Text = myForm.dataGridView1.Rows[rowNum].Cells[3].Value.ToString();
            string isPaid = myForm.dataGridView1.Rows[rowNum].Cells[2].Value.ToString();
            checkBox1.Checked = (isPaid == "Unpaid" ? false : true);
            switch (isPaid) {
                case "Cash":
                    radioButton1.Checked = true;
                    break;
                case "Check":
                    radioButton2.Checked = true;
                    break;
                case "Credit":
                    radioButton3.Checked = true;
                    break;
                case "Debit":
                    radioButton4.Checked = true;
                    break;
                case "Auto":
                    radioButton5.Checked = true;
                    break;
                case "Other":
                    radioButton6.Checked = true;
                    break;
            }
        }
        public void statusChanged(int rowNum) {
            try {
                if (myForm.dataGridView1.Rows[rowNum].Cells[0].Value.ToString() == "") resetForm2();
                else {
                    fillForm2();
                    radioButton8.Checked = true;
                }
            }
            catch (Exception) { resetForm2(); }
        }
        public string paymentType() {
            string whichOne = "Unpaid";
            foreach (Control c in groupBox2.Controls) {
                if (((RadioButton)c).Checked == true) whichOne = c.Name;
            }
            switch (whichOne) {
                case "radioButton1":
                    whichOne = "Cash";
                    break;
                case "radioButton2":
                    whichOne = "Check";
                    break;
                case "radioButton3":
                    whichOne = "Credit";
                    break;
                case "radioButton4":
                    whichOne = "Debit";
                    break;
                case "radioButton5":
                    whichOne = "Auto";
                    break;
                case "radioButton6":
                    whichOne = "Other";
                    break;
            }
            return whichOne;
        }
        private void addNewEntry(string date, string amount, string type, string notes, bool doEdit) {
            if (doEdit) {
                newBills.theBills[myForm.selectedElem].storeDate = date;
                newBills.theBills[myForm.selectedElem].storeAmount = amount;
                newBills.theBills[myForm.selectedElem].storeType = type;
                newBills.theBills[myForm.selectedElem].storeNotes = notes;
            }
            else newBills.theBills.Add(
                    new storeBills {
                        storeDate = date,
                        storeAmount = amount,
                        storeType = type,
                        storeNotes = notes
                    }
                );
            newBills.writeEntry(
                bills.billLocation,
                myForm.comboBox1.SelectedItem.ToString(),
                newBills.theBills,
                false
                );
            myForm.popRows();
            myForm.isMatch(date,amount,type,notes);
        }
    }
}