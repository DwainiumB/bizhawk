﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Globalization;

namespace BizHawk.MultiClient
{
    public partial class Cheats : Form
    {
        //Implement Freeze functions in all memory domains
        //Restore Window Size should restore column order as well
        //context menu - enable/disable highlight dependent items

        int defaultWidth;     //For saving the default size of the dialog, so the user can restore if desired
        int defaultHeight;
        int defaultNameWidth;
        int defaultAddressWidth;
        int defaultValueWidth;
        int defaultDomainWidth;
        int defaultOnWidth;

        List<Cheat> cheatList = new List<Cheat>();
        string currentCheatFile = "";
        bool changes = false;

        public List<Cheat> GetCheatList()
        {
            List<Cheat> c = new List<Cheat>();
            for (int x = 0; x < cheatList.Count; x++)
                c.Add(new Cheat(cheatList[x]));

            return c;
        }

        /// <summary>
        /// Looks for a .cht file that matches the name of the ROM loaded
        /// It is up to the client to determine which director it looks
        /// </summary>
        public bool AttemptLoadCheatFile()
        {
            string CheatFile = MakeDefaultFilename();
            
            var file = new FileInfo(CheatFile);
            if (file.Exists == false)
                return false;
            else
            {
                LoadCheatFile(CheatFile, false);
                return true;
            }
        }

        private string MakeDefaultFilename()
        {
            return GetCheatsPath() + "\\" + Global.Game.Name + ".cht";
        }

        private void ClearFields()
        {
            NameBox.Text = "";
            AddressBox.Text = "";
            ValueBox.Text = "";
            PopulateMemoryDomainComboBox();
            AddressBox.MaxLength = GetNumDigits(Global.Emulator.MemoryDomains[0].Size - 1);
        }

        public void Restart()
        {
            NewCheatList(); //Should be run even if dialog isn't open so cheats system can work
            if (!this.IsHandleCreated || this.IsDisposed) return;
            ClearFields();
        }

        public Cheats()
        {
            InitializeComponent();
            Closing += (o, e) => SaveConfigSettings();
            CheatListView.QueryItemText += new QueryItemTextHandler(CheatListView_QueryItemText);
            CheatListView.QueryItemBkColor += new QueryItemBkColorHandler(CheatListView_QueryItemBkColor);
            CheatListView.VirtualMode = true;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!Global.Config.CheatsAutoSaveOnClose)
            {
                if (!AskSave())
                    e.Cancel = true;
            }
            base.OnClosing(e);
        }

        private void CheatListView_QueryItemBkColor(int index, int column, ref Color color)
        {
            if (cheatList[index].address < 0)
                color = Color.DarkGray;
            else if (!cheatList[index].IsEnabled())
                color = this.BackColor;
        }

        private void CheatListView_QueryItemText(int index, int column, out string text)
        {
            text = "";
            if (cheatList[index].address == -1)
                return;

            if (column == 0) //Name
            {
                text = cheatList[index].name;
            }
            if (column == 1) //Address
            {
                text = FormatAddress(cheatList[index].address);
            }
            if (column == 2) //Value
            {
                text = String.Format("{0:X2}", cheatList[index].value);
            }
            if (column == 3) //Domain
            {
                text = cheatList[index].domain.Name;
            }
            if (column == 4) //Enabled
            {
                if (cheatList[index].IsEnabled())
                    text = "*";
                else
                    text = "";
            }
        }

        private int GetNumDigits(Int32 i)
        {
            if (i < 0x10000) return 4;
            if (i < 0x1000000) return 6;
            else return 8;
        }

        private void Cheats_Load(object sender, EventArgs e)
        {
            LoadConfigSettings();
            PopulateMemoryDomainComboBox();
            AddressBox.MaxLength = GetNumDigits(Global.Emulator.MainMemory.Size - 1);
            UpdateNumberOfCheats();
        }

        private void PopulateMemoryDomainComboBox()
        {
            DomainComboBox.Items.Clear();
            if (Global.Emulator.MemoryDomains.Count > 0)
            {
                for (int x = 0; x < Global.Emulator.MemoryDomains.Count; x++)
                {
                    string str = Global.Emulator.MemoryDomains[x].ToString();
                    DomainComboBox.Items.Add(str);
                }
                DomainComboBox.SelectedIndex = 0;
            }
        }

        public void AddCheat(Cheat c)
        {
            cheatList.Add(c);
            DisplayCheatsList();
            CheatListView.Refresh();
        }

        public void LoadCheatFromRecent(string file)
        {
            bool z = true;

            if (changes) z = AskSave();

            if (z)
            {
                bool r = LoadCheatFile(file, false);
                if (!r)
                {
                    DialogResult result = MessageBox.Show("Could not open " + file + "\nRemove from list?", "File not found", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                    if (result == DialogResult.Yes)
                        Global.Config.RecentCheats.Remove(file);
                }
                DisplayCheatsList();
                changes = false;
            }
        }

        private void recentToolStripMenuItem_DropDownOpened(object sender, EventArgs e)
        {
            //Clear out recent Roms list
            //repopulate it with an up to date list
            recentToolStripMenuItem.DropDownItems.Clear();

            if (Global.Config.RecentCheats.IsEmpty())
            {
                var none = new ToolStripMenuItem();
                none.Enabled = false;
                none.Text = "None";
                recentToolStripMenuItem.DropDownItems.Add(none);
            }
            else
            {
                for (int x = 0; x < Global.Config.RecentCheats.Length(); x++)
                {
                    string path = Global.Config.RecentCheats.GetRecentFileByPosition(x);
                    var item = new ToolStripMenuItem();
                    item.Text = path;
                    item.Click += (o, ev) => LoadCheatFromRecent(path);
                    recentToolStripMenuItem.DropDownItems.Add(item);
                }
            }

            recentToolStripMenuItem.DropDownItems.Add("-");

            var clearitem = new ToolStripMenuItem();
            clearitem.Text = "&Clear";
            clearitem.Click += (o, ev) => Global.Config.RecentCheats.Clear();
            recentToolStripMenuItem.DropDownItems.Add(clearitem);
        }

        private void LoadConfigSettings()
        {
            ColumnPositionSet();

            defaultWidth = Size.Width;     //Save these first so that the user can restore to its original size
            defaultHeight = Size.Height;
            defaultNameWidth = CheatListView.Columns[Global.Config.CheatsNameIndex].Width;
            defaultAddressWidth = CheatListView.Columns[Global.Config.CheatsAddressIndex].Width;
            defaultValueWidth = CheatListView.Columns[Global.Config.CheatsValueIndex].Width;
            defaultDomainWidth = CheatListView.Columns[Global.Config.CheatsDomainIndex].Width;
            defaultOnWidth = CheatListView.Columns[Global.Config.CheatsOnIndex].Width;

            if (Global.Config.CheatsSaveWindowPosition && Global.Config.CheatsWndx >= 0 && Global.Config.CheatsWndy >= 0)
                Location = new Point(Global.Config.CheatsWndx, Global.Config.CheatsWndy);

            if (Global.Config.CheatsWidth >= 0 && Global.Config.CheatsHeight >= 0)
            {
                Size = new System.Drawing.Size(Global.Config.CheatsWidth, Global.Config.CheatsHeight);
            }

            if (Global.Config.CheatsNameWidth > 0)
                CheatListView.Columns[Global.Config.CheatsNameIndex].Width = Global.Config.CheatsNameWidth;
            if (Global.Config.CheatsAddressWidth > 0)
                CheatListView.Columns[Global.Config.CheatsAddressIndex].Width = Global.Config.CheatsAddressWidth;
            if (Global.Config.CheatsValueWidth > 0)
                CheatListView.Columns[Global.Config.CheatsValueIndex].Width = Global.Config.CheatsValueWidth;
            if (Global.Config.CheatsDomainWidth > 0)
                CheatListView.Columns[Global.Config.CheatsDomainIndex].Width = Global.Config.CheatsDomainWidth;
            if (Global.Config.CheatsOnWidth > 0)
                CheatListView.Columns[Global.Config.CheatsOnIndex].Width = Global.Config.CheatsOnWidth;            
        }

        public void SaveConfigSettings()
        {
            ColumnPositionSet();
            Global.Config.CheatsWndx = this.Location.X;
            Global.Config.CheatsWndy = this.Location.Y;
            Global.Config.CheatsWidth = this.Right - this.Left;
            Global.Config.CheatsHeight = this.Bottom - this.Top;

            Global.Config.CheatsNameWidth = CheatListView.Columns[Global.Config.CheatsNameIndex].Width;
            Global.Config.CheatsAddressWidth = CheatListView.Columns[Global.Config.CheatsAddressIndex].Width;
            Global.Config.CheatsValueWidth = CheatListView.Columns[Global.Config.CheatsValueIndex].Width;
            Global.Config.CheatsDomainWidth = CheatListView.Columns[Global.Config.CheatsDomainIndex].Width;
            Global.Config.CheatsOnWidth = CheatListView.Columns[Global.Config.CheatsOnIndex].Width;

            if (Global.Config.CheatsAutoSaveOnClose)
            {
                if (changes)
                {
                    if (currentCheatFile.Length == 0)
                        currentCheatFile = MakeDefaultFilename();

                    SaveCheatFile(currentCheatFile);
                }
            }
        }

        private void DisplayCheatsList()
        {
            UpdateNumberOfCheats();
            CheatListView.ItemCount = cheatList.Count;
        }

        private void MoveUp()
        {
            ListView.SelectedIndexCollection indexes = CheatListView.SelectedIndices;
            Cheat temp = new Cheat();
            if (indexes[0] == 0) return;
            foreach (int index in indexes)
            {
                temp = cheatList[index];
                cheatList.Remove(cheatList[index]);
                cheatList.Insert(index - 1, temp);

                //Note: here it will get flagged many times redundantly potentially, 
                //but this avoids it being flagged falsely when the user did not select an index
                Changes();
            }
            List<int> i = new List<int>();
            for (int z = 0; z < indexes.Count; z++)
                i.Add(indexes[z] - 1);

            CheatListView.SelectedIndices.Clear();
            for (int z = 0; z < i.Count; z++)
                CheatListView.SelectItem(i[z], true);


            DisplayCheatsList();
        }

        private void MoveDown()
        {
            ListView.SelectedIndexCollection indexes = CheatListView.SelectedIndices;
            Cheat temp = new Cheat();

            foreach (int index in indexes)
            {
                temp = cheatList[index];

                if (index < cheatList.Count - 1)
                {

                    cheatList.Remove(cheatList[index]);
                    cheatList.Insert(index + 1, temp);

                }

                //Note: here it will get flagged many times redundantly potnetially, 
                //but this avoids it being flagged falsely when the user did not select an index
                Changes();
            }

            List<int> i = new List<int>();
            for (int z = 0; z < indexes.Count; z++)
                i.Add(indexes[z] + 1);

            CheatListView.SelectedIndices.Clear();
            //for (int z = 0; z < i.Count; z++)
                //CheatListView.SelectItem(i[z], true); //TODO

            DisplayCheatsList();
        }

        private void toolStripButtonMoveUp_Click(object sender, EventArgs e)
        {
            MoveUp();
        }

        private void moveUpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MoveUp();
        }

        private void toolStripButtonMoveDown_Click(object sender, EventArgs e)
        {
            MoveDown();
        }

        private void moveDownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MoveDown();
        }

        void Changes()
        {
            changes = true;
            MessageLabel.Text = Path.GetFileName(currentCheatFile) + " *";
        }

        private FileInfo GetSaveFileFromUser()
        {
            var sfd = new SaveFileDialog();
            if (currentCheatFile.Length > 0)
                sfd.FileName = Path.GetFileNameWithoutExtension(currentCheatFile);
            else if (!(Global.Emulator is NullEmulator))
                sfd.FileName = Global.Game.Name;
            sfd.InitialDirectory = GetCheatsPath();
            sfd.Filter = "Cheat Files (*.cht)|*.cht|All Files|*.*";
            sfd.RestoreDirectory = true;
            Global.Sound.StopSound();
            var result = sfd.ShowDialog();
            Global.Sound.StartSound();
            if (result != DialogResult.OK)
                return null;
            var file = new FileInfo(sfd.FileName);
            Global.Config.LastRomPath = file.DirectoryName;
            return file;
        }

        private void SaveAs()
        {
            var file = GetSaveFileFromUser();
            if (file != null)
            {
                SaveCheatFile(file.FullName);
                currentCheatFile = file.FullName;
                MessageLabel.Text = Path.GetFileName(currentCheatFile) + " saved.";
            }
        }

        private bool SaveCheatFile(string path)
        {
            var file = new FileInfo(path);

            using (StreamWriter sw = new StreamWriter(path))
            {
                string str = "";

                for (int x = 0; x < cheatList.Count; x++)
                {
                    str += FormatAddress(cheatList[x].address) + "\t";
                    str += String.Format("{0:X2}", cheatList[x].value) + "\t";
                    str += cheatList[x].domain.Name + "\t";
                    if (cheatList[x].IsEnabled())
                        str += "1\t";
                    else
                        str += "0\t";
                    str += cheatList[x].name + "\n";
                }

                sw.WriteLine(str);
            }
            changes = false;
            return true;
        }

        public bool AskSave()
        {
            if (changes)
            {
                DialogResult result = MessageBox.Show("Save Changes?", "Cheats", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button3);

                if (result == DialogResult.Yes)
                {
                    //TOOD: Do quicksave if filename, else save as
                    if (string.Compare(currentCheatFile, "") == 0)
                    {
                        SaveAs();
                    }
                    else
                        SaveCheatFile(currentCheatFile);
                    return true;
                }
                else if (result == DialogResult.No)
                    return true;
                else if (result == DialogResult.Cancel)
                    return false;
            }
            return true;
        }

        private void NewCheatList()
        {
            bool result = true;
            if (changes) result = AskSave();

            if (result == true)
            {
                cheatList.Clear();
                DisplayCheatsList();
                currentCheatFile = "";
                changes = false;
                MessageLabel.Text = "";
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewCheatList();
        }

        private void newToolStripButton_Click(object sender, EventArgs e)
        {
            NewCheatList();
        }

        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            if (changes)
            {
                SaveCheatFile(currentCheatFile);
            }
            else
            {
                SaveAs();
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.Compare(currentCheatFile, "") == 0) return;

            if (changes)
                SaveCheatFile(currentCheatFile);
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAs();
        }

        public string GetCheatsPath()
        {
            string path;
            switch (Global.Emulator.SystemId)
            {
                case "NES":
                    path = PathManager.MakeAbsolutePath(Global.Config.PathNESCheats, "NES");
                    break;
                case "SMS":
                    path = PathManager.MakeAbsolutePath(Global.Config.PathSMSCheats, "SMS");
                    break;
                case "SG":
                    path = PathManager.MakeAbsolutePath(Global.Config.PathSGCheats, "SG");
                    break;
                case "GG":
                    path = PathManager.MakeAbsolutePath(Global.Config.PathGGCheats, "GG");
                    break;
                case "GEN":
                    path = PathManager.MakeAbsolutePath(Global.Config.PathGenesisCheats, "GEN");
                    break;
                case "SFX":
                case "PCE":
                    path = PathManager.MakeAbsolutePath(Global.Config.PathPCECheats, "GB");
                    break;
                case "GB":
                    path = PathManager.MakeAbsolutePath(Global.Config.PathGBCheats, "GB");
                    break;
                case "TI83":
                    path = PathManager.MakeAbsolutePath(Global.Config.PathTI83Cheats, "TI83");
                    break;
                default:
                    path = PathManager.GetBasePathAbsolute();
                    break;
            }
            var f = new FileInfo(path);
            if (f.Directory.Exists == false)
                f.Directory.Create();
            return path;
        }

        private FileInfo GetFileFromUser()
        {
            var ofd = new OpenFileDialog();
            if (currentCheatFile.Length > 0)
                ofd.FileName = Path.GetFileNameWithoutExtension(currentCheatFile);
            ofd.InitialDirectory = GetCheatsPath();
            ofd.Filter = "Cheat Files (*.cht)|*.cht|All Files|*.*";
            ofd.RestoreDirectory = true;

            Global.Sound.StopSound();
            var result = ofd.ShowDialog();
            Global.Sound.StartSound();
            if (result != DialogResult.OK)
                return null;
            var file = new FileInfo(ofd.FileName);
            Global.Config.LastRomPath = file.DirectoryName;
            return file;
        }

        private MemoryDomain SetDomain(string name)
        {
            //Attempts to find the memory domain by name, if it fails, it defaults to index 0
            for (int x = 0; x < Global.Emulator.MemoryDomains.Count; x++)
            {
                if (Global.Emulator.MemoryDomains[x].Name == name)
                    return Global.Emulator.MemoryDomains[x];
            }
            return Global.Emulator.MemoryDomains[0];
        }

        bool LoadCheatFile(string path, bool append)
        {
            int y;
            var file = new FileInfo(path);
            if (file.Exists == false) return false;

            using (StreamReader sr = file.OpenText())
            {
                if (!append) currentCheatFile = path;

                string s = "";
                string temp = "";

                if (append == false)
                    cheatList.Clear();  //Wipe existing list and read from file

                while ((s = sr.ReadLine()) != null)
                {
                    if (s.Length < 6) continue;
                    Cheat c = new Cheat();
                    temp = s.Substring(0, s.IndexOf('\t'));   //Address
                    c.address = int.Parse(temp, NumberStyles.HexNumber);

                    y = s.IndexOf('\t') + 1;
                    s = s.Substring(y, s.Length - y);   //Value
                    temp = s.Substring(0, 2);
                    c.value = byte.Parse(temp, NumberStyles.HexNumber);
                    
                    y = s.IndexOf('\t') + 1;
                    s = s.Substring(y, s.Length - y); //Memory Domain
                    temp = s.Substring(0, s.IndexOf('\t'));
                    c.domain = SetDomain(temp);

                    y = s.IndexOf('\t') + 1;
                    s = s.Substring(y, s.Length - y); //Enabled
                    y = int.Parse(s[0].ToString());
                    if (y == 0)
                        c.Disable();
                    else
                        c.Enable();

                    y = s.IndexOf('\t') + 1;
                    s = s.Substring(y, s.Length - y); //Name
                    c.name = s;

                    cheatList.Add(c);
                }

                Global.Config.RecentCheats.Add(file.FullName);
                changes = false;
                MessageLabel.Text = Path.GetFileName(file.FullName);
                UpdateNumberOfCheats();
            }

            if (Global.Config.DisableCheatsOnLoad)
            {
                for (int x = 0; x < cheatList.Count; x++)
                    cheatList[x].Disable();
            }
            return true; //TODO
        }

        private void OpenCheatFile()
        {
            var file = GetFileFromUser();
            if (file != null)
            {
                bool r = true;
                if (changes) r = AskSave();
                if (r)
                {
                    LoadCheatFile(file.FullName, false);
                    DisplayCheatsList();
                }
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenCheatFile();
        }

        private void openToolStripButton_Click(object sender, EventArgs e)
        {
            OpenCheatFile();
        }

        private void InsertSeparator()
        {
            Cheat c = new Cheat();
            c.address = -1;

            ListView.SelectedIndexCollection indexes = CheatListView.SelectedIndices;
            int x;
            if (indexes.Count > 0)
            {
                x = indexes[0];
                if (indexes[0] > 0)
                    cheatList.Insert(indexes[0], c);
            }
            else
                cheatList.Add(c);
            DisplayCheatsList();
            CheatListView.Refresh();
        }

        private void toolStripButtonSeparator_Click(object sender, EventArgs e)
        {
            InsertSeparator();
        }

        private void insertSeparatorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertSeparator();
        }
        
        private Cheat MakeCheat()
        {
            Cheat c = new Cheat();
            c.name = NameBox.Text;
            c.address = int.Parse(AddressBox.Text, NumberStyles.HexNumber); //TODO: validation
            c.value = (byte)(int.Parse(ValueBox.Text, NumberStyles.HexNumber));
            c.domain = Global.Emulator.MemoryDomains[DomainComboBox.SelectedIndex];
            c.Enable();
            return c;
        }

        private void AddCheatButton_Click(object sender, EventArgs e)
        {
            AddCheat(MakeCheat());
        }

        private void addCheatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddCheat(MakeCheat());
        }

        private void RemoveCheat()
        {
            Changes();
            ListView.SelectedIndexCollection indexes = CheatListView.SelectedIndices;
            if (indexes.Count > 0)
            {
                foreach (int index in indexes)
                {
                    cheatList.Remove(cheatList[indexes[0]]); //index[0] used since each iteration will make this the correct list index
                }
                DisplayCheatsList();
            }
        }

        private void cutToolStripButton_Click(object sender, EventArgs e)
        {
            RemoveCheat();
        }

        private void removeCheatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveCheat();
        }

        private void UpdateNumberOfCheats()
        {
            string message = "";
            int active = 0;
            for (int x = 0; x < cheatList.Count; x++)
            {
                if (cheatList[x].IsEnabled())
                    active++;
            }

            int c = cheatList.Count;
            if (c == 1)
                message += c.ToString() + " cheat (" + active.ToString() + " active)";
            else if (c == 0)
                message += c.ToString() + " cheats";
            else
                message += c.ToString() + " cheats (" + active.ToString() + " active)";

            NumCheatsLabel.Text = message;
        }

        private void saveWindowPositionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Global.Config.CheatsSaveWindowPosition ^= true;
        }

        private void optionsToolStripMenuItem_DropDownOpened(object sender, EventArgs e)
        {
            saveWindowPositionToolStripMenuItem.Checked = Global.Config.CheatsSaveWindowPosition;
            CheatsOnOffLoadToolStripMenuItem.Checked = Global.Config.DisableCheatsOnLoad;
            autoloadDialogToolStripMenuItem.Checked = Global.Config.AutoLoadCheats;
            LoadCheatFileByGameToolStripMenuItem.Checked = Global.Config.LoadCheatFileByGame;
            saveCheatsOnCloseToolStripMenuItem.Checked = Global.Config.CheatsAutoSaveOnClose;
        }

        private void DuplicateCheat()
        {
            ListView.SelectedIndexCollection indexes = CheatListView.SelectedIndices;
            if (indexes.Count > 0)
            {
                Cheat c = new Cheat();
                int x = indexes[0];
                c.name = cheatList[x].name;
                c.address = cheatList[x].address;
                c.value = cheatList[x].value;
                Changes();
                cheatList.Add(c);
                DisplayCheatsList();
            }
        }

        private void copyToolStripButton_Click(object sender, EventArgs e)
        {
            DuplicateCheat();
        }

        private void duplicateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DuplicateCheat();
        }

        private void Toggle()
        {
            ListView.SelectedIndexCollection indexes = CheatListView.SelectedIndices;
            if (indexes.Count > 0)
            {
                for (int x = 0; x < indexes.Count; x++)
                {
                    if (cheatList[indexes[x]].IsEnabled())
                        cheatList[indexes[x]].Disable();
                    else
                        cheatList[indexes[x]].Enable();
                }
                CheatListView.Refresh();
            }
            UpdateNumberOfCheats();
        }

        private void CheatListView_DoubleClick(object sender, EventArgs e)
        {
            Toggle();
        }

        private void CheatListView_Click(object sender, EventArgs e)
        {
            ListView.SelectedIndexCollection indexes = CheatListView.SelectedIndices;
            if (indexes.Count > 0)
            {
                NameBox.Text = cheatList[indexes[0]].name;
                AddressBox.Text = FormatAddress(cheatList[indexes[0]].address);
                ValueBox.Text = String.Format("{0:X2}", cheatList[indexes[0]].value);
                CheatListView.Refresh();
            }
        }

        private string FormatAddress(int address)
        {
            return String.Format("{0:X" + GetNumDigits((Global.Emulator.MainMemory.Size - 1)).ToString() + "}", address);
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            //TODO: this fails because selected index must go away to edit values, either prevent that or keep track of the last selected index
            ListView.SelectedIndexCollection indexes = CheatListView.SelectedIndices;
            if (indexes.Count > 0)
            {
                if (AddressBox.Text.Length > 0 && ValueBox.Text.Length > 0)
                    cheatList[indexes[0]] = MakeCheat();
                CheatListView.Refresh();
            }
        }

        private void CheatListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CheatListView.SelectedIndices.Count > 0)
                EditButton.Enabled = true;
            else
                EditButton.Enabled = false;
        }

        private void AddressBox_TextChanged(object sender, EventArgs e)
        {
            if (AddressBox.Text.Length > 0 && ValueBox.Text.Length > 0)
                AddCheatButton.Enabled = true;
            else
                AddCheatButton.Enabled = false;
        }

        private void ValueBox_TextChanged(object sender, EventArgs e)
        {
            if (AddressBox.Text.Length > 0 && ValueBox.Text.Length > 0)
                AddCheatButton.Enabled = true;
            else
                AddCheatButton.Enabled = false;
        }

        private void NameBox_TextChanged(object sender, EventArgs e)
        {
            if (AddressBox.Text.Length > 0 && ValueBox.Text.Length > 0)
                AddCheatButton.Enabled = true;
            else
                AddCheatButton.Enabled = false;
        }

        private void AddressBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\b') return;
            if (!InputValidate.IsValidHexNumber(e.KeyChar))
                e.Handled = true;
        }

        private void ValueBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\b') return;
            if (!InputValidate.IsValidHexNumber(e.KeyChar))
                e.Handled = true;
        }

        private void CheatListView_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            if (e.Label == null) //If no change
                return;
            string Str = e.Label;
            int index = e.Item;
            cheatList[e.Item].name = Str;
        }

        private void restoreWindowSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Size = new System.Drawing.Size(defaultWidth, defaultHeight);
            Global.Config.CheatsNameIndex = 0;
            Global.Config.CheatsAddressIndex = 1;
            Global.Config.CheatsValueIndex = 2;
            Global.Config.CheatsDomainIndex = 3;
            Global.Config.CheatsOnIndex = 4;
            ColumnPositionSet();
            CheatListView.Columns[0].Width = defaultNameWidth;
            CheatListView.Columns[1].Width = defaultAddressWidth;
            CheatListView.Columns[2].Width = defaultValueWidth;
            CheatListView.Columns[3].Width = defaultDomainWidth;
            CheatListView.Columns[4].Width = defaultOnWidth;
        }

        public bool IsActiveCheat(MemoryDomain d, int address)
        {
            for (int x = 0; x < cheatList.Count; x++)
            {
                if (cheatList[x].address == address && cheatList[x].domain.Name == d.Name)
                {
                    return true;
                }
            }
            return false;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void appendFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var file = GetFileFromUser();
            if (file != null)
                LoadCheatFile(file.FullName, true);
            DisplayCheatsList();
            Changes();
        }

        private void fileToolStripMenuItem_DropDownOpened(object sender, EventArgs e)
        {
            if (string.Compare(currentCheatFile, "") == 0 || !changes)
            {
                saveToolStripMenuItem.Enabled = false;
            }
            else
            {
                saveToolStripMenuItem.Enabled = true;
            }
        }

        private void CheatsOnOffLoadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Global.Config.DisableCheatsOnLoad ^= true;
        }

        public void DisableAllCheats()
        {
            for (int x = 0; x < cheatList.Count; x++)
                cheatList[x].Disable();
            CheatListView.Refresh();
            UpdateNumberOfCheats();
        }

        private void disableAllCheatsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DisableAllCheats();
        }

        private void disableAllCheatsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            DisableAllCheats();
        }

        private void toggleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Toggle();
        }

        private void removeSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveCheat();
        }

        private void autoloadDialogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Global.Config.AutoLoadCheats ^= true;
        }

        private void LoadCheatFileByGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Global.Config.LoadCheatFileByGame ^= true;
        }

        private void saveCheatsOnCloseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Global.Config.CheatsAutoSaveOnClose ^= true;
        }

        private void ColumnReorder(object sender, ColumnReorderedEventArgs e)
        {
            ColumnHeader header = e.Header;

            int lowIndex = 0;
            int highIndex = 0;
            int changeIndex = 0;
            if (e.NewDisplayIndex > e.OldDisplayIndex)
            {
                changeIndex = -1;
                highIndex = e.NewDisplayIndex;
                lowIndex = e.OldDisplayIndex;
            }
            else
            {
                changeIndex = 1;
                highIndex = e.OldDisplayIndex;
                lowIndex = e.NewDisplayIndex;
            }


            if (Global.Config.CheatsNameIndex >= lowIndex && Global.Config.CheatsNameIndex <= highIndex)
                Global.Config.CheatsNameIndex += changeIndex;
            if (Global.Config.CheatsAddressIndex >= lowIndex && Global.Config.CheatsAddressIndex <= highIndex)
                Global.Config.CheatsAddressIndex += changeIndex;
            if (Global.Config.CheatsValueIndex >= lowIndex && Global.Config.CheatsValueIndex <= highIndex)
                Global.Config.CheatsValueIndex += changeIndex;
            if (Global.Config.CheatsDomainIndex >= lowIndex && Global.Config.CheatsDomainIndex <= highIndex)
                Global.Config.CheatsDomainIndex += changeIndex;
            if (Global.Config.CheatsOnIndex >= lowIndex && Global.Config.CheatsOnIndex <= highIndex)
                Global.Config.CheatsOnIndex += changeIndex;

            if (header.Text == "Name")
                Global.Config.CheatsNameIndex = e.NewDisplayIndex;
            else if (header.Text == "Address")
                Global.Config.CheatsAddressIndex = e.NewDisplayIndex;
            else if (header.Text == "Value")
                Global.Config.CheatsValueIndex = e.NewDisplayIndex;
            else if (header.Text == "Domain")
                Global.Config.CheatsDomainIndex = e.NewDisplayIndex;
            else if (header.Text == "On")
                Global.Config.CheatsOnIndex = e.NewDisplayIndex;
        }

        private void ColumnPositionSet()
        {
            List<ColumnHeader> columnHeaders = new List<ColumnHeader>();
            int i = 0;
            for (i = 0; i < CheatListView.Columns.Count; i++)
                columnHeaders.Add(CheatListView.Columns[i]);

            CheatListView.Columns.Clear();

            i = 0;
            do
            {
                string column = "";
                if (Global.Config.CheatsNameIndex == i)
                    column = "Name";
                else if (Global.Config.CheatsAddressIndex == i)
                    column = "Address";
                else if (Global.Config.CheatsValueIndex == i)
                    column = "Value";
                else if (Global.Config.CheatsDomainIndex == i)
                    column = "Domain";
                else if (Global.Config.CheatsOnIndex == i)
                    column = "On";

                for (int k = 0; k < columnHeaders.Count(); k++)
                {
                    if (columnHeaders[k].Text == column)
                    {
                        CheatListView.Columns.Add(columnHeaders[k]);
                        columnHeaders.Remove(columnHeaders[k]);
                        break;

                    }
                }
                i++;
            } while (columnHeaders.Count() > 0);
        }

        private void Cheats_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None; string[] filePaths = (string[])e.Data.GetData(DataFormats.FileDrop);
        }

        private void Cheats_DragDrop(object sender, DragEventArgs e)
        {
            string[] filePaths = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (filePaths[0].Contains(".cht")) //TODO: a less lazy way to check file extension?
            {
                LoadCheatFile(filePaths[0], false);
                DisplayCheatsList();
            }
        }
    }
}
