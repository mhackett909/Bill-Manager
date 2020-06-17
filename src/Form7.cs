using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;
namespace Bills {
    public partial class Form7 : Form {
        private ComboBox billCombo;
        bills newBills = new bills();
        Form8 getlist = new Form8();
        List<string> years = new List<string>();
        List<string> targetList = new List<string>();
        XmlDocument doc = new XmlDocument();
        double paid = 0, due = 0, amount = 0;
        public Form7(ComboBox combo) {
            InitializeComponent();
            billCombo = combo;
        }
        private void Form7_Load(object sender, EventArgs e) {
            popCombo1();
            popCombo2("All", bills.billLocation);
            popCombo3();
        }
        private void getTotals(string location, string billName, string year, string month) {
            switch (month) {
                case "January":
                    month = "1";
                    break;
                case "February":
                    month = "2";
                    break;
                case "March":
                    month = "3";
                    break;
                case "April":
                    month = "4";
                    break;
                case "May":
                    month = "5";
                    break;
                case "June":
                    month = "6";
                    break;
                case "July":
                    month = "7";
                    break;
                case "August":
                    month = "8";
                    break;
                case "September":
                    month = "9";
                    break;
                case "October":
                    month = "10";
                    break;
                case "November":
                    month = "11";
                    break;
                case "December":
                    month = "12";
                    break;
            }
            doc.Load(location);
            targetList.Clear();
            if (location == bills.billLocation) {
                for (int z = 0; z < billCombo.Items.Count; z++) targetList.Add(billCombo.Items[z].ToString());
                paid = 0;
                due = 0;
            }
            else targetList.AddRange(getlist.importList);
            XmlNode headerNode = null;
            for (int z = 0; z < targetList.Count; z++) {
                if (billName == "All") headerNode = doc.SelectSingleNode("/Bills/Bill[@name=\"" + targetList[z] + "\"]");
                else if (targetList.Contains(billName)) headerNode = doc.SelectSingleNode("/Bills/Bill[@name=\"" + billName + "\"]");
                else break;
                foreach (XmlNode nodez in headerNode.ChildNodes) {
                    string[] splitDate = { };
                    bool addIt = true;
                    foreach (XmlNode nod in nodez.ChildNodes) {
                        if (nod.OuterXml.ToString().Substring(0,5) == "<Date") splitDate = nod.InnerText.Split('/');
                        if (nod.OuterXml.ToString().Substring(0,5) == "<Paid") {
                            if (nod.InnerText == "Unpaid") {
                                switch (year) {
                                    case "All":
                                        break;
                                    default:
                                        if (splitDate[2] != year) addIt = false; 
                                        break;
                                }
                                switch (month) {
                                    case "All":
                                        break;
                                    default:
                                        if (splitDate[0] != month) addIt = false;
                                        break;
                                }
                                if (addIt) {
                              
                                    due += Convert.ToDouble(amount);
                                }
                            }
                            else {
                                switch (year) {
                                    case "All":
                                        break;
                                    default:
                                        if (splitDate[2] != year) addIt = false;
                                        break;
                                }
                                switch (month) {
                                    case "All":
                                        break;
                                    default:
                                        if (splitDate[0] != month) addIt = false;
                                        break;
                                }
                                if (addIt) paid += Convert.ToDouble(amount);
                            }
                        }
                        if (nod.OuterXml.ToString().Substring(0,7) == "<Amount") amount = Convert.ToDouble(nod.InnerText);       
                    }
                }
                if (billName != "All") break;
            }
            if (location == bills.billLocation) {
                getTotals(bills.archiveLocation, billName, year, month);
                return;
            }
            label4.Text = "Total Paid: $" + newBills.fixDouble(paid.ToString());
            label5.Text = "Total Due: $" + newBills.fixDouble(due.ToString());
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) {
            try {
                popCombo2(comboBox1.SelectedItem.ToString(), bills.billLocation);
                comboBox3.SelectedItem = "All";
            }
            catch (Exception) {  }
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e) {
            try {
                getTotals(bills.billLocation, comboBox1.SelectedItem.ToString(), comboBox2.SelectedItem.ToString(), comboBox3.SelectedItem.ToString());
            }
            catch (Exception) { }
        }
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e) {
            try {
                getTotals(bills.billLocation, comboBox1.SelectedItem.ToString(), comboBox2.SelectedItem.ToString(), comboBox3.SelectedItem.ToString());
            }catch (Exception) { }
        }
        private void popCombo1() {
            comboBox1.Items.Add("All");
            List<string> totalslist = new List<string>();
            for (int z = 0; z < billCombo.Items.Count; z++) totalslist.Add(billCombo.Items[z].ToString());
            for (int z = 0; z < getlist.importList.Count; z++) if (!totalslist.Contains(getlist.importList[z])) totalslist.Add(getlist.importList[z]);
            totalslist.Sort();
            comboBox1.Items.AddRange(totalslist.ToArray());
            comboBox1.SelectedItem = "All";
        }
        private void popCombo2(string billName, string location) {
            comboBox2.Items.Clear();
            comboBox2.Items.Add("All");
            doc.Load(location);
            targetList.Clear();
            if (location == bills.billLocation) {
                years.Clear();
                for (int z = 0; z < billCombo.Items.Count; z++) targetList.Add(billCombo.Items[z].ToString());
            }
            else targetList.AddRange(getlist.importList);
            XmlNode headerNode = null;
            for (int z = 0; z < targetList.Count; z++) {
                if (billName == "All") headerNode = doc.SelectSingleNode("/Bills/Bill[@name=\"" + targetList[z] + "\"]");
                else if (targetList.Contains(billName)) headerNode = doc.SelectSingleNode("/Bills/Bill[@name=\"" + billName + "\"]");
                else break;
                foreach (XmlNode nodez in headerNode.ChildNodes) {
                    string[] splitDate;
                    foreach (XmlNode nod in nodez.ChildNodes) {
                        if (nod.OuterXml.ToString().Substring(0, 5) == "<Date") {
                            splitDate = nod.InnerText.Split('/');
                            if (!years.Contains(splitDate[2])) years.Add(splitDate[2]);
                        }
                    }
                }
                if (billName != "All") break;
            }
            if (location == bills.billLocation) {
                popCombo2(billName, bills.archiveLocation);
                return;
            }
            years.Sort();
            years.Reverse();
            comboBox2.Items.AddRange(years.ToArray());
            comboBox2.SelectedIndex = 0;
            comboBox2.Enabled = (comboBox2.Items.Count == 1 ? false : true);
            comboBox3.Enabled = (comboBox2.Items.Count == 1 ? false : true);
        }
        private void popCombo3() {
            comboBox3.Items.Add("All");
            comboBox3.Items.Add("January");
            comboBox3.Items.Add("February");
            comboBox3.Items.Add("March");
            comboBox3.Items.Add("April");
            comboBox3.Items.Add("May");
            comboBox3.Items.Add("June");
            comboBox3.Items.Add("July");
            comboBox3.Items.Add("August");
            comboBox3.Items.Add("September");
            comboBox3.Items.Add("October");
            comboBox3.Items.Add("November");
            comboBox3.Items.Add("December");
            comboBox3.SelectedItem = "All";
        }
    }
}
