using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Net;
using System.Windows.Forms;
namespace Bills {
    public class bills {
        public List<string> billList = new List<string>();
        public List<storeBills> theBills = new List<storeBills>();
        public const string billLocation = "C:/Bill Manager/Bills.xml",archiveLocation = "C:/Bill Manager/Archive.xml";
        public void popBills() {
            XmlReader reader;
            try {
                 reader = XmlReader.Create(billLocation);
                while (reader.Read()) {
                    if (reader.GetAttribute("name") != null) billList.Add(reader.GetAttribute("name"));
                }
                reader.Close();
            }catch (Exception) { createFiles(billLocation); }
            try {
                reader = XmlReader.Create(archiveLocation);
                reader.Close();
            }
            catch (Exception) { createFiles(archiveLocation); }
        }
        public void editID(string billName, string newID) {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(bills.billLocation);
            XmlNode billNode = xmlDoc.SelectSingleNode("/Bills/Bill[@name=\"" + billName + "\"]");
            billNode.Attributes[0].Value = newID;
            xmlDoc.Save(bills.billLocation);
        }
        public void writeEntry(string fileName,string billName, List<storeBills> writeArr, bool append) {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(fileName); 
            XmlNode billNode = xmlDoc.SelectSingleNode("/Bills/Bill[@name=\"" + billName + "\"]"),entryNode,dateNode,amountNode,paidNode,notesNode;
            if (!append) {
                billNode.RemoveAll();
                XmlAttribute attr = xmlDoc.CreateAttribute("name");
                attr.Value = billName;
                billNode.Attributes.SetNamedItem(attr);
            }
            for (int z = 0; z < writeArr.Count; z++) {
                entryNode = xmlDoc.CreateElement("Entry");
                dateNode = xmlDoc.CreateElement("Date");
                dateNode.InnerText = writeArr[z].storeDate;
                entryNode.AppendChild(dateNode);
                amountNode = xmlDoc.CreateElement("Amount");
                amountNode.InnerText = writeArr[z].storeAmount;
                entryNode.AppendChild(amountNode);
                paidNode = xmlDoc.CreateElement("Paid");
                paidNode.InnerText = writeArr[z].storeType;
                entryNode.AppendChild(paidNode);
                notesNode = xmlDoc.CreateElement("Notes");
                if (writeArr[z].storeNotes != "") notesNode.InnerText = writeArr[z].storeNotes;
                entryNode.AppendChild(notesNode);
                billNode.AppendChild(entryNode);
            }
            xmlDoc.DocumentElement.AppendChild(billNode);
            xmlDoc.Save(fileName);
        }
        public void writeBill(string fileName, string billName) {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(fileName); 
            if (xmlDoc.SelectSingleNode("/Bills/Bill[@name=\"" + billName + "\"]") == null) {
                XmlElement billElem = xmlDoc.CreateElement("Bill");
                billElem.SetAttribute("name", billName);
                xmlDoc.DocumentElement.AppendChild(billElem);
            }
            xmlDoc.Save(fileName);
        }
        public void delBill(string fileName,string billName) {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(fileName);
            XmlNodeList nodes = xmlDoc.SelectNodes("/Bills/Bill[@name=\"" + billName + "\"]");
            nodes[0].ParentNode.RemoveChild(nodes[0]);
            xmlDoc.Save(fileName);
        }
        public void delEntry(string fileName,string billName,List<storeBills> delArr) {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(fileName);
            XmlNodeList nodes = xmlDoc.SelectNodes("/Bills/Bill[@name=\"" + billName + "\"]");
            string[] dateSplit = delArr[0].storeDate.Split('/'), nodeDate;
            for (int z = 0; z < nodes[0].ChildNodes.Count; z++) {
                nodeDate = nodes[0].ChildNodes[z].ChildNodes[0].InnerText.Split('/');
                if (nodeDate[2] == dateSplit[2]) {
                    nodes[0].RemoveChild(nodes[0].ChildNodes[z]);
                    z = -1;
                }
            }
            xmlDoc.Save(fileName);
        }
        public void createFiles(string fileName) {
            if (!Directory.Exists("C:/Bill Manager/")) Directory.CreateDirectory("C:/Bill Manager/");
            File.Create(fileName).Close();
            if (fileName == billLocation) downloadHelpFile(false);
            XmlWriter xmlWriter = XmlWriter.Create(fileName);
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("Bills");
            xmlWriter.WriteEndDocument();
            xmlWriter.Close();
        }
        public void downloadHelpFile(bool msg) {
            WebClient client = new WebClient();
            try {
                client.DownloadFile("http://hackett.site/help.txt", "C:/Bill Manager/help.txt");
                if (msg) MessageBox.Show("Help file downloaded.", "Success!");
            }catch (Exception) { if (msg) MessageBox.Show("Could not download help file. Please contact program creator (Info->About)", "Error!"); }
        }
        public bool openForm(string z) {
            bool formOpen = false;
            foreach (Form f in Application.OpenForms) {
                if (f.ToString().Substring(6, z.Length) == z) formOpen = true;
            }
            return formOpen;
        }
        public void closeForms() {
            List<Form> formList = new List<Form>();
            foreach (Form f in Application.OpenForms) {
                string formName = f.ToString().Substring(6, 5);
                if (formName != "Form1") formList.Add(f);
            }
           for (int w = 0; w < formList.Count; w++) formList[w].Close();
        }
        public string fixDouble(string fixit) {
            string[] splitAmount = fixit.Split('.');
            string truncatedAmount = (splitAmount[0] == "" ? "0" : splitAmount[0]);
            if (splitAmount.Length == 2) {
                if (splitAmount[1].Length > 2) splitAmount[1] = splitAmount[1].Substring(0, 2);
                else if (splitAmount[1].Length == 1) splitAmount[1] += "0";
                else if (splitAmount[1] == "") splitAmount[1] = "00";
                truncatedAmount += "." + splitAmount[1];
            }
            else truncatedAmount += ".00";
            return truncatedAmount;
        }
    }
    public class storeBills {
        public string storeDate { get; set; }
        public string storeAmount { get; set; }
        public string storeType { get; set; }
        public string storeNotes { get; set; }
    }
}

