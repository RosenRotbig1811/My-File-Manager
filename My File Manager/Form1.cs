using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace My_File_Manager
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            DriveTreeInit();
        }

        public void DriveTreeInit()
        {
            string[] drivesArray = Directory.GetLogicalDrives();

            treeView1.BeginUpdate();
            treeView1.Nodes.Clear();

            foreach (string s in drivesArray)
            {
                TreeNode drive = new TreeNode(s, 0, 0);
                treeView1.Nodes.Add(drive);

                GetDirs(drive);
            }


            treeView1.EndUpdate();
        }

        public void GetDirs(TreeNode node)
        {
            DirectoryInfo[] diArray;

            node.Nodes.Clear();

            string fullPath = node.FullPath;
            DirectoryInfo di = new DirectoryInfo(fullPath);

            try
            {
                diArray = di.GetDirectories();
            }
            catch
            {
                return;
            }

            foreach (DirectoryInfo dirinfo in diArray)
            {
                TreeNode dir = new TreeNode(dirinfo.Name, 1, 2);
                node.Nodes.Add(dir);
            }
        }

        private void treeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            treeView1.BeginUpdate();

            foreach (TreeNode node in e.Node.Nodes)
            {
                GetDirs(node);
            }

            treeView1.EndUpdate();
        }

        public void treeView1_OnAfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
        {
            TreeNode selectedNode = e.Node;
            string fullPath = selectedNode.FullPath;
            DirectoryInfo di = new DirectoryInfo(fullPath);
            FileInfo[] fiArray;
            DirectoryInfo[] diArray;

            try
            {
                fiArray = di.GetFiles();
                diArray = di.GetDirectories();
            }
            catch
            {
                return;
            }

            listView1.Items.Clear();

            foreach (DirectoryInfo dirInfo in diArray)
            {
                ListViewItem lvi = new ListViewItem(dirInfo.Name);
                lvi.SubItems.Add("0");
                lvi.SubItems.Add(dirInfo.LastWriteTime.ToString());
                lvi.ImageIndex = 0;
                listView1.Items.Add(lvi);
            }

            foreach (FileInfo fileInfo in fiArray)
            {
                ListViewItem lvi = new ListViewItem(fileInfo.Name);
                lvi.SubItems.Add(fileInfo.Length.ToString() + " Кб");
                lvi.SubItems.Add(fileInfo.LastWriteTime.ToString());

                string filenameExtension = Path.GetExtension(fileInfo.Name).ToLower();

                switch (filenameExtension)
                {
                    case ".com":
                        {
                            lvi.ImageIndex = 2;
                            break;
                        }
                    case ".exe":
                        {
                            lvi.ImageIndex = 2;
                            break;
                        }
                    case ".hlp":
                        {
                            lvi.ImageIndex = 3;
                            break;
                        }
                    case ".txt":
                        {
                            lvi.ImageIndex = 4;
                            break;
                        }
                    case ".doc":
                        {
                            lvi.ImageIndex = 5;
                            break;
                        }
                    case ".mp3":
                        {
                            lvi.ImageIndex = 6;
                            break;
                        }
                    case ".jpg":
                        {
                            lvi.ImageIndex = 7;
                            break;
                        }
                    case ".avi":
                        {
                            lvi.ImageIndex = 8;
                            break;
                        }
                    case ".zip":
                        {
                            lvi.ImageIndex = 9;
                            break;
                        }
                    case ".rar":
                        {
                            lvi.ImageIndex = 9;
                            break;
                        }
                    case ".docx":
                        {
                            lvi.ImageIndex = 5;
                            break;
                        }
                    default:
                        {
                            lvi.ImageIndex = 1;
                            break;
                        }
                }

                listView1.Items.Add(lvi);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            //string filename = listView1.SelectedItems[0].Tag.ToString();

            
            Process.Start(Path.Combine(treeView1.SelectedNode.FullPath.ToString(), listView1.Items[listView1.SelectedIndices[0]].Text));

        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string message = "My File Manager ONPU 2013\n\nVasiliy Kuzmenko";
            MessageBox.Show(message);
        }

        private void Delete_Click(object sender, EventArgs e)
        {

            listView1.BeginUpdate();
            string File_to_Delete = "";
            string Folder_to_Delete = "";
            string message = "Вы действительно хотите удалить файл?";
            const string caption = "Файл будет удален";
            var rezult = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (rezult == DialogResult.Yes)
            {
                if (listView1.Items[listView1.SelectedIndices[0]].Text != "")
                {
                    //Delete.Enabled = true;
                    File_to_Delete = Path.Combine(treeView1.SelectedNode.FullPath.ToString(), listView1.Items[listView1.SelectedIndices[0]].Text);
                }



                Folder_to_Delete = Path.Combine(treeView1.SelectedNode.FullPath.ToString(), listView1.Items[listView1.SelectedIndices[0]].Text);


                System.IO.DirectoryInfo directory = new System.IO.DirectoryInfo(Folder_to_Delete);
                System.IO.FileInfo file = new System.IO.FileInfo(File_to_Delete);

                try
                {
                    if (File.Exists(File_to_Delete))
                    {
                        file.Delete();
                    }
                    else
                    {                        
                        directory.Delete(true);
                    }
                }
                catch (System.IO.IOException) { }

                


                for (int i = 0; i <= 10000; i++)
                {
                    progressBar1.Value = i * 100 / 10000;
                }
 
            }


            
            listView1.Items.RemoveAt(listView1.SelectedIndices[0]);
            treeView1.Nodes.Remove(treeView1.SelectedNode);
            DriveTreeInit();
            listView1.EndUpdate();
        }

        private void Copy_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();


            string Path_Before = (Path.Combine(treeView1.SelectedNode.FullPath.ToString(), listView1.Items[listView1.SelectedIndices[0]].Text));
            string Name = listView1.Items[listView1.SelectedIndices[0]].Text;
            string Path_To_Copy = Path.Combine(folderBrowserDialog1.SelectedPath, Name);

            if (!File.Exists(Path.Combine(folderBrowserDialog1.SelectedPath, Name)))
            {
                File.Copy(Path_Before, Path_To_Copy);
            }
            else
            {
                string message = "Такой файл уже существует";
                //const string caption = "Файл будет удален";
                var rezult = MessageBox.Show(message);

            }

            for (int i = 0; i <= 10000; i++)
            {
                progressBar1.Value = i * 100 / 10000;
            }
        }

        private void Refresh_Click(object sender, EventArgs e)
        {
            DriveTreeInit();
        }

        private void listView1_Click(object sender, EventArgs e)
        {

            /*if (listView1.SelectedItems.Count > 0)
            {
                Delete.Enabled = true;
            }
            else
            {
                Delete.Enabled = false;
            }*/

        }

        private void New_Folder_Click(object sender, EventArgs e)
        {

            listView1.BeginUpdate();

            string path = System.IO.Path.Combine(Path.Combine(treeView1.SelectedNode.FullPath.ToString()), "New Folder");
            System.IO.Directory.CreateDirectory(path);
            listView1.Items.Add("New Folder", 1);
            DriveTreeInit();

            listView1.EndUpdate();
        }

        private void listView1_changed(object sender, EventArgs e)
        {
            Delete.Enabled = true;
        }
    }
}


 
    


