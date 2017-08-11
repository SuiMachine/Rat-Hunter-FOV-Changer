using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;
using System.Xml;
using System.Xml.Linq;

namespace Rat_Hunter_FOV_Changer
{
    public partial class Form1 : Form
    {
        string installDirectory { get; set; }
        string[] files = {
            "0.xml",
            "1.xml",
            "2.xml",
            "3.xml",
            "4.xml",
            "5.xml",
            "6.xml",
            "7.xml",
            "8.xml",
            "9.xml",
            "9b.xml",
            "10.xml",
            "11.xml",
            "12.xml",
            "16.xml",
            "17.xml",
            "19.xml",
            "20.xml",
            "20a.xml",
            "20b.xml",
            "21.xml",
            "22b.xml",
            "23.xml",
            "24.xml"
        };

        public Form1()
        {
            InitializeComponent();
            try
            {
                RegistryKey whatever = Registry.CurrentUser.OpenSubKey("Software\\GFI\\RatHunter_Release\\InstallPath");
                installDirectory = Path.GetDirectoryName(whatever.GetValue("Path").ToString() + "\\");
                L_Install_Location.Text = installDirectory;
            }
            catch (Exception e)
            {
                MessageBox.Show("Error", e.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                Debug.WriteLine(e);
                Close();
            }

            if(!Directory.Exists(Path.Combine(installDirectory, "Data", "levels")))
            {
                MessageBox.Show("Levels folder doesn\'t exist!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Close();
            }
        }

        private void B_DoIt_Click(object sender, EventArgs e)
        {
            float desiredFOV = 75f;
            if (float.TryParse(TB_FOV.Text, out desiredFOV))
            {
                if(desiredFOV<55)
                {
                    MessageBox.Show("Desired FOV is too low!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if(desiredFOV>170)
                {
                    MessageBox.Show("Desired FOV is too high!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    doMagick(desiredFOV);
                }
            }
            else
                MessageBox.Show("Please enter a numerical value for desired FOV", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void doMagick(float desiredFOV)
        {
            foreach(string file in files)
            {
                string path = Path.Combine(installDirectory, "Data", "levels", file);
                if (File.Exists(path))
                {
                    string text = File.ReadAllText(path);
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(new StringReader(text));
                    var xmlNodeList = xmlDoc.ChildNodes;
                    for(int i=0; i<xmlNodeList.Count; i++)
                    {
                        string name = xmlNodeList.Item(i).Name;
                        if(name.ToLower() == "world")
                        {
                            bool foundFOVNode = false;
                            var childAttributes = xmlNodeList.Item(i).Attributes;
                            for(int j=0; j< childAttributes.Count; j++)
                            {
                                string nodeName = childAttributes.Item(j).Name;
                                if(nodeName.ToLower() == "fov")
                                {
                                    childAttributes.Item(j).Value = desiredFOV.ToString();
                                    foundFOVNode = true;
                                }
                            }
                            if(!foundFOVNode)
                            {
                                XmlAttribute attribute = xmlDoc.CreateAttribute("FOV");
                                attribute.Value = desiredFOV.ToString();
                                xmlNodeList.Item(i).Attributes.Append(attribute);
                            }
                        }
                        xmlDoc.Save(path);
                    }
                }
            }
            MessageBox.Show("Finished", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
