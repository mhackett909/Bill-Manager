using System;
using System.Windows.Forms;
using System.Xml;
namespace Bills {
    public partial class Form1 : Form {
        int sortedBy = 0, maxRows = 15;
        bool ascending = false;
        bills newBills = new bills();
        Form2 newWindow;
        public int selectedElem = 0;
        string selectedItem;
        public Form1() { InitializeComponent(); }
        private void Form1_Load(object sender, EventArgs e) {
            newBills.popBills();
            popCombo();
        }
        private void Form1_Resize(object sender, EventArgs e) {
            if (WindowState.ToString() != "Minimized") {
                this.button5.Location = new System.Drawing.Point(this.Width - 120, 27);
                this.button1.Location = new System.Drawing.Point(2, this.Height - 65);
                this.button2.Location = new System.Drawing.Point(95, this.Height - 65);
                this.textBox1.Location = new System.Drawing.Point(this.Width - 343, this.Height - 60);
                this.textBox2.Location = new System.Drawing.Point(this.Width - 197, this.Height - 60);
                this.dataGridView1.Size = new System.Drawing.Size(this.Width - 23, this.Height - 123);
                maxRows = (dataGridView1.Height / dataGridView1.Rows[0].Height) - 1;
                int currentRows = dataGridView1.Rows.Count;
                int numBills = newBills.theBills.Count;
                Console.WriteLine(WindowState);
                Console.WriteLine("Max: "+maxRows);
                Console.WriteLine("Current: " +currentRows);
                Console.WriteLine("Bills: " + numBills);
                if (WindowState.ToString() == "Maximized") {
                    if (maxRows > numBills && numBills > currentRows) dataGridView1.Rows.Add(maxRows - numBills);
                    else if (maxRows > numBills && maxRows != currentRows) dataGridView1.Rows.Add(maxRows - currentRows);
                    this.comboBox1.Size = new System.Drawing.Size(300, 24);
                    this.button3.Location = new System.Drawing.Point(310, 27);
                    this.button4.Location = new System.Drawing.Point(415, 27);
                    this.Column1.Width = 120;
                    this.Column2.Width = 120;
                    this.Column3.Width = 120;
                    this.Column4.Width = this.dataGridView1.Width - 183;
                }
                else if (WindowState.ToString() == "Normal") {
                    for (int z = dataGridView1.Rows.Count; z > (numBills < 15 ? 15 : numBills); z--) dataGridView1.Rows.RemoveAt(z - 1);
                    this.comboBox1.Size = new System.Drawing.Size(180, 24);
                    this.button3.Location = new System.Drawing.Point(190, 27);
                    this.button4.Location = new System.Drawing.Point(295, 27);
                    this.Column1.Width = 90;
                    this.Column2.Width = 90;
                    this.Column3.Width = 90;
                    this.Column4.Width = this.dataGridView1.Width - 273;
                }
                int tester = (dataGridView1.Height - 2) - (maxRows * 22);
                tester -= 21;
                dataGridView1.ColumnHeadersHeight = 21 + tester;
            }
        }
        private void helpToolStripMenuItem_Click(object sender, EventArgs e) {
            try { System.Diagnostics.Process.Start("C:/Bill Manager/help.txt"); }
            catch (Exception) {
                MessageBox.Show("Help File Missing. Attempting to download.", "Error!");
                newBills.downloadHelpFile(true);
            }
        }
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e) {
            if (!newBills.openForm("Form3")) {
                newBills.closeForms();
                Form3 newWindow = new Form3();
                newWindow.ShowDialog();
            }
        }
        private void totalsToolStripMenuItem_Click(object sender, EventArgs e) {
            if (comboBox1.SelectedItem == null) MessageBox.Show("Please create a bill.", "Error!");
            else {
                newBills.closeForms();
                Form7 billTotals = new Form7(comboBox1);
                billTotals.ShowDialog();
            }
        }

        private void editBillNameToolStripMenuItem_Click(object sender, EventArgs e) {
            if (comboBox1.SelectedItem == null) MessageBox.Show("Please create a bill.", "Error!");
            else {
                newBills.closeForms();
                Form4 editBill = new Form4(comboBox1, true);
                editBill.ShowDialog();
            }
        }
        private void importToolStripMenuItem_Click(object sender, EventArgs e) {
            newBills.closeForms();
            XmlTextReader reader = new XmlTextReader(bills.archiveLocation);
            XmlDocument testDoc = new XmlDocument();
            bool archiveExist = false;
            testDoc.Load(bills.archiveLocation); 
            while (reader.Read()) {
                if (archiveExist) break;
                switch (reader.NodeType) {
                    case XmlNodeType.Element:
                        if (reader.Name == "Bill") archiveExist = true;
                        break;
                }
            }
            reader.Close();
            if (archiveExist) {
                Form8 importForm = new Form8(this, "import",newBills);
                importForm.ShowDialog();
            }
            else MessageBox.Show("No archives exist!", "Error!");
        }
        private void printToolStripMenuItem_Click(object sender, EventArgs e) {
            if (comboBox1.SelectedItem == null) MessageBox.Show("Please create a bill.", "Error!");
            else {
                newBills.closeForms();
                Form8 printForm = new Form8(this,"print",newBills);
                printForm.ShowDialog();
            }
        }
        private void button1_Click(object sender, EventArgs e) {
            if (comboBox1.SelectedItem == null) MessageBox.Show("Please create a bill.", "Error!");
            else if (!newBills.openForm("Form2")) {
                newBills.closeForms();
                newWindow = new Form2(this,newBills);
                newWindow.Show(this);
            }
        }
        private void button2_Click(object sender, EventArgs e) {
            var firstRowCell = dataGridView1.SelectedRows[0].Cells[0].Value;
            if (firstRowCell == null) MessageBox.Show("Can't remove empty row.", "Error!");
            else {
                Form6 confirmDiag = new Form6(false);
                confirmDiag.ShowDialog();
                if (confirmDiag.DialogResult == DialogResult.OK) {
                    newBills.theBills.RemoveAt(selectedElem);
                    newBills.writeEntry(bills.billLocation, selectedItem, newBills.theBills, false);
                    popRows();
                }
            }
        }
        private void button3_Click(object sender, EventArgs e) {
            newBills.closeForms();
            Form4 addBillForm = new Form4(comboBox1,false);
            addBillForm.ShowDialog();
        }
        private void button4_Click(object sender, EventArgs e) {
            if (comboBox1.SelectedItem == null) MessageBox.Show("Please create a bill.", "Error!");
            else {
                newBills.closeForms();
                XmlDocument testDoc = new XmlDocument();
                testDoc.Load(bills.archiveLocation); 
                Form8 archiveForm = new Form8(this,"archive",newBills);
                archiveForm.ShowDialog();
            }
        }
        private void button5_Click(object sender, EventArgs e) {
            if (comboBox1.SelectedItem == null) MessageBox.Show("Please create a bill.", "Error!");
            else {
                Form6 confirmDiag = new Form6(true);
                confirmDiag.ShowDialog();
                if (confirmDiag.DialogResult == DialogResult.OK) {
                    newBills.delBill(bills.billLocation, comboBox1.SelectedItem.ToString());
                    comboBox1.Items.Remove(comboBox1.SelectedItem);
                    try { comboBox1.SelectedIndex = 0; }
                    catch (Exception) {
                        button3.Select();
                        dataGridView1.Rows.Clear();
                        dataGridView1.Rows.Add(maxRows);
                        textBox1.Text = "Due: $0";
                        textBox2.Text = "Paid: $0";
                    }
                }
            }
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) {
            if (selectedItem != comboBox1.SelectedItem.ToString()) {
                selectedItem = comboBox1.SelectedItem.ToString();
                ascending = false;
                sortedBy = 0;
                popRows();
            }
        }
        public void dataGridView1_SelectionChanged(object sender, EventArgs e) {
            selectedElem = dataGridView1.CurrentCell.RowIndex;
            if (newBills.openForm("Form2")) newWindow.statusChanged(selectedElem);
        }
        public void dataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e) {
            sortedBy = e.ColumnIndex;
            ascending = !ascending;
            popRows();
        }
        public void popCombo() {
            for (int z = 0; z < newBills.billList.Count; z++) comboBox1.Items.Add(newBills.billList[z]);
            try { comboBox1.SelectedIndex = 0; }
            catch (Exception) {
                button3.Select();
                dataGridView1.Rows.Add(maxRows);
            }
        }
        public void popRows() {
            dataGridView1.Rows.Clear();
            newBills.theBills.Clear();
            XmlReader reader = XmlReader.Create(bills.billLocation);
            bool readNext = false, endit = false;
            int rowNum = 0;
            while (reader.Read()) {
                if (endit) break;
                if (reader.GetAttribute("name") == selectedItem) readNext = true;
                if (reader.NodeType == XmlNodeType.Element && readNext) {
                    switch (reader.Name) {
                        case "Bill":
                            if (reader.GetAttribute("name") != selectedItem) endit = true;
                            break;
                        case "Date":
                            DateTime theDate = Convert.ToDateTime(reader.ReadInnerXml());
                            dataGridView1.Rows[rowNum].Cells[0].Value = theDate;
                            break;
                        case "Amount":
                            dataGridView1.Rows[rowNum].Cells[1].Value = Convert.ToDouble(reader.ReadInnerXml());
                            break;
                        case "Notes":
                            dataGridView1.Rows[rowNum].Cells[3].Value = reader.ReadInnerXml();
                            rowNum++;
                            break;
                        case "Bills":
                            break;
                        case "Entry":
                            dataGridView1.Rows.Add();
                            break;
                        default:
                            dataGridView1.Rows[rowNum].Cells[2].Value = reader.ReadInnerXml();
                            break;
                    }
                }
            }
            reader.Close();
            dataGridView1.Sort(dataGridView1.Columns[sortedBy], 
                (ascending ? System.ComponentModel.ListSortDirection.Ascending : System.ComponentModel.ListSortDirection.Descending));
            fixDates(rowNum);
            for (int z = 0; z < rowNum; z++)
                newBills.theBills.Add(
                        new storeBills {
                            storeDate = dataGridView1.Rows[z].Cells[0].Value.ToString(),
                            storeAmount = dataGridView1.Rows[z].Cells[1].Value.ToString(),
                            storeType = dataGridView1.Rows[z].Cells[2].Value.ToString(),
                            storeNotes = dataGridView1.Rows[z].Cells[3].Value.ToString()
                        }
                );
            if (dataGridView1.RowCount < maxRows) dataGridView1.Rows.Add(maxRows - rowNum);
            popTextBoxes(rowNum);
            if (newBills.openForm("Form2")) newWindow.Close();
            dataGridView1.Rows[0].Selected = true;
            selectedElem = 0;
        }
        private void popTextBoxes(int rowNum) {
            double paidAmount = 0, dueAmount = 0;
            for (int z = 0; z < rowNum; z++) {
                double theValue = Convert.ToDouble(dataGridView1.Rows[z].Cells[1].Value.ToString());
                if (dataGridView1.Rows[z].Cells[2].Value.ToString() == "Unpaid") dueAmount += theValue;
                else paidAmount += theValue;
            }
            textBox1.Text = "Due: $" + newBills.fixDouble(dueAmount.ToString());
            textBox2.Text = "Paid: $" + newBills.fixDouble(paidAmount.ToString());
        }
        public void isMatch(string date, string amount, string type, string notes) {
            for (int z = 0; z < newBills.theBills.Count; z++) {
                if (dataGridView1.Rows[z].Cells[0].Value.ToString() == date
                    && dataGridView1.Rows[z].Cells[1].Value.ToString() == amount
                    && dataGridView1.Rows[z].Cells[2].Value.ToString() == type
                    && dataGridView1.Rows[z].Cells[3].Value.ToString() == notes) {
                        dataGridView1.Rows[z].Selected = true;
                        selectedElem = z;
                        if (z > 14) dataGridView1.FirstDisplayedCell = dataGridView1.Rows[z - 14].Cells[0];
                        break;
                }
            }
        }
        public bool isMatch(string date, string amount) {
            bool matched = false;
            for (int z = 0; z < newBills.theBills.Count; z++) {
                if (dataGridView1.Rows[z].Cells[0].Value.ToString() == date 
                    && dataGridView1.Rows[z].Cells[1].Value.ToString() == amount) {
                        matched = true;
                        dataGridView1.Rows[z].Selected = true;
                        selectedElem = z;
                        dataGridView1.FirstDisplayedCell = dataGridView1.Rows[z].Cells[0];
                        break;
                }
            }
            return matched;
        }
        public void fixDates(int rows) {
            string truncatedDate, actualDate;
            for (int z = 0; z < rows; z++) {
                actualDate = dataGridView1.Rows[z].Cells[0].Value.ToString();
                try { truncatedDate = actualDate.Substring(0, actualDate.IndexOf(" ")); }
                catch (Exception) { truncatedDate = actualDate; }
                dataGridView1.Rows[z].Cells[0].Value = truncatedDate;
                dataGridView1.Rows[z].Cells[1].Value = newBills.fixDouble(dataGridView1.Rows[z].Cells[1].Value.ToString());
            }
        }
    }
}
