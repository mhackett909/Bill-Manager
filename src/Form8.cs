using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.Xml;
namespace Bills {
    public partial class Form8 : Form {
        Form1 theForm;
        bills newBills;
        public List<string> importList = new List<string>();
        string alterForm, billLocation = bills.billLocation;
        double due = 0, paid = 0;
        public Form8(Form1 form, string type, bills impBills) {
            InitializeComponent();
            theForm = form;
            alterForm = type;
            newBills = impBills;
        }
        public Form8() { loadList();  }
        private void Form8_Load(object sender, System.EventArgs e) {
            switch (alterForm) {
                case "import":
                    button1.Text = "Import";
                    this.Text = "Import Bill";
                    billLocation = bills.archiveLocation;
                    break;
                case "print":
                    button1.Text = "Print";
                    this.Text = "Print Bill";
                    break;
            }
            loadList();
            popForm();
        }
        private void loadList() {
            XmlReader reader;
                reader = XmlReader.Create(bills.archiveLocation);
                while (reader.Read()) {
                    if (reader.GetAttribute("name") != null) importList.Add(reader.GetAttribute("name"));
                }
                reader.Close();
        }
        private void popForm() {
            switch (alterForm) {
                case "import":
                    comboBox1.Items.AddRange(importList.ToArray());
                    comboBox1.SelectedIndex = 0;
                    break;
                default:
                    for (int z = 0; z < theForm.comboBox1.Items.Count; z++) comboBox1.Items.Add(theForm.comboBox1.Items[z]);
                    comboBox1.SelectedItem = theForm.comboBox1.SelectedItem;
                    break;
            }
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) { popForm2(); }
        private void button1_Click(object sender, EventArgs e) {
            switch (alterForm) {
                case "print":
                    loadBills(bills.billLocation);
                    if (newBills.theBills.Count == 0) MessageBox.Show("No entries to print!", "Error!");
                    else printBill();
                    break;
                default:
                    archiveWrite();
                    break;
            }
            this.Close();
        }
        private void popForm2() {
            comboBox2.Items.Clear();
            comboBox2.Items.Add("All");
            comboBox2.SelectedIndex = 0;
            List<string> years = new List<string>();
            XmlDocument doc = new XmlDocument();
            doc.Load(billLocation);
            XmlNode headerNode = doc.SelectSingleNode("/Bills/Bill[@name=\"" + comboBox1.SelectedItem.ToString() + "\"]");
            foreach (XmlNode nodez in headerNode.ChildNodes) {
                string[] splitDate;
                foreach (XmlNode nod in nodez.ChildNodes) {
                    if (nod.OuterXml.ToString().Substring(0, 5) == "<Date") {
                        splitDate = nod.InnerText.Split('/');
                        if (!years.Contains(splitDate[2])) years.Add(splitDate[2]);
                    }
                }
            }
            years.Sort();
            years.Reverse();
            comboBox2.Items.AddRange(years.ToArray());
            comboBox2.Enabled = (comboBox2.Items.Count == 1 ? false : true);
        }
        private void archiveWrite() {
            string theFile = (alterForm == "archive" ? bills.archiveLocation : bills.billLocation);
            if (!doesExist(alterForm, comboBox1.SelectedItem.ToString())) {
                newBills.writeBill(theFile, comboBox1.SelectedItem.ToString());
                if (alterForm == "import") {
                    theForm.comboBox1.Items.Add(comboBox1.SelectedItem);
                    if (theForm.comboBox1.Items.Count == 1) theForm.comboBox1.SelectedIndex = 0;
                }
            }
            theFile = (alterForm == "archive" ? bills.billLocation : bills.archiveLocation);
            loadBills(theFile);
            theFile = (alterForm == "archive" ? bills.archiveLocation : bills.billLocation);
            newBills.writeEntry(theFile, comboBox1.SelectedItem.ToString(), newBills.theBills,true);
            theFile = (alterForm == "archive" ? bills.billLocation : bills.archiveLocation);
            if (comboBox2.SelectedItem.ToString() == "All" || comboBox2.Items.Count == 2) {
                if (alterForm == "archive") theForm.comboBox1.Items.Remove(comboBox1.SelectedItem);
                newBills.delBill(theFile, comboBox1.SelectedItem.ToString());
            }
            else newBills.delEntry(theFile, comboBox1.SelectedItem.ToString(), newBills.theBills);
            if (alterForm == "import") theForm.comboBox1.SelectedItem = comboBox1.SelectedItem;
            else if (theForm.comboBox1.Items.Count > 0) theForm.comboBox1.SelectedIndex = (theForm.comboBox1.SelectedItem == null ? 0 : theForm.comboBox1.SelectedIndex);
            theForm.popRows();
        }
        private bool doesExist(string fileType, string billName) {
            bool exists = false;
            if (fileType == "archive" && importList.Contains(billName)) exists = true;
            else if (fileType == "import" && theForm.comboBox1.Items.Contains(billName)) exists = true;
            return exists;
        }
        private void loadBills(string theFile) {
            XmlDocument doc = new XmlDocument();
            doc.Load(theFile);
            XmlNode headerNode = doc.SelectSingleNode("/Bills/Bill[@name=\"" + comboBox1.SelectedItem.ToString() + "\"]");
            newBills.theBills.Clear();
            foreach (XmlNode nodez in headerNode.ChildNodes) {
                string[] splitDate;
                string date = "", amount = "", type = "", notes = "";
                bool addIt = true;
                foreach (XmlNode nod in nodez.ChildNodes) {
                    if (nod.OuterXml.ToString().Substring(0, 5) == "<Date") {
                        splitDate = nod.InnerText.Split('/');
                        date = nod.InnerText;
                        switch (comboBox2.SelectedItem.ToString()) {
                            case "All":
                                break;
                            default:
                                if (splitDate[2] != comboBox2.SelectedItem.ToString()) addIt = false;
                                break;
                        }
                    }
                    if (nod.OuterXml.ToString().Substring(0, 7) == "<Amount") amount = nod.InnerText;
                    if (nod.OuterXml.ToString().Substring(0, 5) == "<Paid") type = nod.InnerText;
                    if (nod.OuterXml.ToString().Substring(0, 6) == "<Notes") notes = nod.InnerText;
                }
                if (addIt) {
                    newBills.theBills.Add(
                        new storeBills {
                            storeDate = date,
                            storeAmount = amount,
                            storeType = type,
                            storeNotes = notes
                        }
                    );
                    if (type == "Unpaid") due += Convert.ToDouble(amount);
                    else paid += Convert.ToDouble(amount);
                }
            }
            bubbleSort(ref newBills.theBills);
        }
        private void printBill() {
            PrintDocument printDoc = new PrintDocument();
            printDoc.DefaultPageSettings.Landscape = true;
            printDoc.DefaultPageSettings.Margins.Left = 100;
            printDoc.DefaultPageSettings.Margins.Right = 100;
            printDoc.DefaultPageSettings.Margins.Top = 25;
            printDoc.DefaultPageSettings.Margins.Bottom = 25;
            printDoc.DocumentName = "Bill Report";
            printDoc.PrintPage += new PrintPageEventHandler(printDoc_PrintPage);

            PrintDialog printDialog = new PrintDialog();
            printDialog.Document = printDoc;

            DialogResult dialogResult = printDialog.ShowDialog();
            if (dialogResult == DialogResult.OK) printDoc.Print();
        }
        private void printDoc_PrintPage(object sender, PrintPageEventArgs e) {
            Graphics g = e.Graphics;
            Font font = new Font("Tahoma", 12);

            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            string printName = comboBox1.SelectedItem.ToString();
            string[] munoz = DateTime.Today.ToString().Split(' ');
            g.DrawRectangle(Pens.Blue, e.MarginBounds);
            Rectangle layout = new Rectangle(e.MarginBounds.Left, e.MarginBounds.Top, e.MarginBounds.Width, e.MarginBounds.Height);
            g.DrawString(printName, font, Brushes.Black, layout, stringFormat);
            stringFormat.Alignment = StringAlignment.Far;
            g.DrawString(munoz[0], font, Brushes.Black, layout, stringFormat);
            stringFormat.Alignment = StringAlignment.Near;
            g.DrawString("Bill Report", font, Brushes.Black, layout, stringFormat);

            for (int z = 100; z + 120 < e.MarginBounds.Bottom; z += 120) {
                g.DrawString("Date: "+newBills.theBills[0].storeDate, font, Brushes.Black, e.MarginBounds.Left, z);
                g.DrawString("Amount: $"+ newBills.theBills[0].storeAmount, font, Brushes.Black, e.MarginBounds.Left, z + 20);
                g.DrawString("Payment Type: "+ newBills.theBills[0].storeType, font, Brushes.Black, e.MarginBounds.Left, z + 40);
                g.DrawString("Notes: "+ newBills.theBills[0].storeNotes, font, Brushes.Black, e.MarginBounds.Left, z + 60);
                newBills.theBills.RemoveAt(0);
                if (newBills.theBills.Count == 0) {
                    stringFormat.Alignment = StringAlignment.Far;
                    g.DrawString("Totals: ", font, Brushes.Black, e.MarginBounds.Right, 760, stringFormat);
                    g.DrawString("$"+paid.ToString()+" (Paid) ", font, Brushes.Black, e.MarginBounds.Right, 780, stringFormat);
                    g.DrawString("$"+due.ToString()+" (Due) ", font, Brushes.Black, e.MarginBounds.Right, 800, stringFormat);
                    break;
                }
            }
            if (newBills.theBills.Count > 0) e.HasMorePages = true;
            else e.HasMorePages = false;
        }
        public void bubbleSort(ref List<storeBills> printBills) {
            storeBills tempBill;
            DateTime date1, date2;
            for (int z = 0; z < printBills.Count-1; z++) {
                date1 = Convert.ToDateTime(printBills[z].storeDate);
                date2 = Convert.ToDateTime(printBills[z + 1].storeDate);
                if (DateTime.Compare(date1, date2) < 0) {
                    tempBill = printBills[z];
                    printBills[z] = printBills[z + 1];
                    printBills[z + 1] = tempBill;
                    z = -1;
                }
            }
        }
    }
}
