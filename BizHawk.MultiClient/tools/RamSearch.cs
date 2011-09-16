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
	//TODO:
	//Sorting by Prev only does "Since prev frame", find a way to integrate the various prev options
	//In DoUndo, prevList is set to searchList, instead how about a UndoPrev, so that undo restores both the current and previous values
	//Go To Address (Ctrl+G) feature
	
	/// <summary>
	/// A winform designed to search through ram values
	/// </summary>
	public partial class RamSearch : Form
	{
		string systemID = "NULL";
		List<Watch> searchList = new List<Watch>();
		List<Watch> undoList = new List<Watch>();
		List<Watch> weededList = new List<Watch>();  //When addresses are weeded out, the new list goes here, before going into searchList
		List<Watch> prevList = new List<Watch>();
		List<Watch> redoList = new List<Watch>();
		private bool IsAWeededList = false; //For deciding whether the weeded list is relevant (0 size could mean all were removed in a legit preview
		List<ToolStripMenuItem> domainMenuItems = new List<ToolStripMenuItem>();
		MemoryDomain Domain = new MemoryDomain("NULL", 1, Endian.Little, addr => 0, (a, v) => { });

		public enum SCompareTo { PREV, VALUE, ADDRESS, CHANGES };
		public enum SOperator { LESS, GREATER, LESSEQUAL, GREATEREQUAL, EQUAL, NOTEQUAL, DIFFBY };
		public enum SSigned { SIGNED, UNSIGNED, HEX };

		//Reset window position item
		int defaultWidth;       //For saving the default size of the dialog, so the user can restore if desired
		int defaultHeight;
		int defaultAddressWidth;
		int defaultValueWidth;
		int defaultPrevWidth;
		int defaultChangesWidth;
		string currentSearchFile = "";
		string addressFormatStr = "{0:X4}  ";
		bool sortReverse;
		string sortedCol;

		public void SaveConfigSettings()
		{
			ColumnPositionSet();
			Global.Config.RamSearchAddressWidth = SearchListView.Columns[Global.Config.RamSearchAddressIndex].Width;
			Global.Config.RamSearchValueWidth = SearchListView.Columns[Global.Config.RamSearchValueIndex].Width;
			Global.Config.RamSearchPrevWidth = SearchListView.Columns[Global.Config.RamSearchPrevIndex].Width;
			Global.Config.RamSearchChangesWidth = SearchListView.Columns[Global.Config.RamSearchChangesIndex].Width;

			Global.Config.RamSearchWndx = this.Location.X;
			Global.Config.RamSearchWndy = this.Location.Y;
			Global.Config.RamSearchWidth = this.Right - this.Left;
			Global.Config.RamSearchHeight = this.Bottom - this.Top;
		}

		public RamSearch()
		{
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			InitializeComponent();
			Closing += (o, e) => SaveConfigSettings();
		}

		public void UpdateValues()
		{
			if (!this.IsHandleCreated || this.IsDisposed) return;

			if (searchList.Count > 8)
				SearchListView.BlazingFast = true;
			sortReverse = false;
			sortedCol = "";
			for (int x = 0; x < searchList.Count; x++)
			{
				searchList[x].PeekAddress(Domain);
			}
			if (AutoSearchCheckBox.Checked)
				DoSearch();
			else if (Global.Config.RamSearchPreviewMode)
				DoPreview();

			SearchListView.Refresh();
			SearchListView.BlazingFast = false;
		}

		private void RamSearch_Load(object sender, EventArgs e)
		{
			ClearUndo();
			ClearRedo();
			LoadConfigSettings();
			StartNewSearch();
			SetMemoryDomainMenu();
		}

		private void LoadConfigSettings()
		{
			ColumnPositionSet();

			defaultWidth = this.Size.Width;     //Save these first so that the user can restore to its original size
			defaultHeight = this.Size.Height;
			defaultAddressWidth = SearchListView.Columns[Global.Config.RamSearchAddressIndex].Width;
			defaultValueWidth = SearchListView.Columns[Global.Config.RamSearchValueIndex].Width;
			defaultPrevWidth = SearchListView.Columns[Global.Config.RamSearchPrevIndex].Width;
			defaultChangesWidth = SearchListView.Columns[Global.Config.RamSearchChangesIndex].Width;

			if (Domain.Endian == Endian.Big)
			{
				bigEndianToolStripMenuItem.Checked = true;
				littleEndianToolStripMenuItem.Checked = false;
			}
			else
			{
				bigEndianToolStripMenuItem.Checked = false;
				littleEndianToolStripMenuItem.Checked = true;
			}

			if (Global.Config.RamSearchSaveWindowPosition && Global.Config.RamSearchWndx >= 0 && Global.Config.RamSearchWndy >= 0)
				this.Location = new Point(Global.Config.RamSearchWndx, Global.Config.RamSearchWndy);

			if (Global.Config.RamSearchWidth >= 0 && Global.Config.RamSearchHeight >= 0)
			{
				this.Size = new System.Drawing.Size(Global.Config.RamSearchWidth, Global.Config.RamSearchHeight);
			}

			if (Global.Config.RamSearchAddressWidth > 0)
				SearchListView.Columns[Global.Config.RamSearchAddressIndex].Width = Global.Config.RamSearchAddressWidth;
			if (Global.Config.RamSearchValueWidth > 0)
				SearchListView.Columns[Global.Config.RamSearchValueIndex].Width = Global.Config.RamSearchValueWidth;
			if (Global.Config.RamSearchPrevWidth > 0)
				SearchListView.Columns[Global.Config.RamSearchPrevIndex].Width = Global.Config.RamSearchPrevWidth;
			if (Global.Config.RamSearchChangesWidth > 0)
				SearchListView.Columns[Global.Config.RamSearchChangesIndex].Width = Global.Config.RamSearchChangesWidth;
		}

		private void SetMemoryDomainMenu()
		{
			memoryDomainsToolStripMenuItem.DropDownItems.Clear();
			if (Global.Emulator.MemoryDomains.Count > 0)
			{
				for (int x = 0; x < Global.Emulator.MemoryDomains.Count; x++)
				{
					string str = Global.Emulator.MemoryDomains[x].ToString();
					var item = new ToolStripMenuItem();
					item.Text = str;
					{
						int z = x;
						item.Click += (o, ev) => SetMemoryDomain(z);
					}
					if (x == 0)
					{
						SetMemoryDomain(x);
					}
					memoryDomainsToolStripMenuItem.DropDownItems.Add(item);
					domainMenuItems.Add(item);
				}
			}
			else
				memoryDomainsToolStripMenuItem.Enabled = false;
		}

		public void Restart()
		{
			if (!this.IsHandleCreated || this.IsDisposed) return;
			SetMemoryDomainMenu();  //Calls Start New Search
		}

		private void SetMemoryDomain(int pos)
		{
			if (pos < Global.Emulator.MemoryDomains.Count)  //Sanity check
			{
				Domain = Global.Emulator.MemoryDomains[pos];
			}
			SetPlatformAndMemoryDomainLabel();
			StartNewSearch();
			addressFormatStr = "X" + GetNumDigits(Domain.Size - 1).ToString();
		}

		private void SetTotal()
		{
			int x = searchList.Count;
			string str;
			if (x == 1)
				str = " address";
			else
				str = " addresses";
			TotalSearchLabel.Text = x.ToString() + str;
		}

		private void OpenSearchFile()
		{
			var file = GetFileFromUser();
			if (file != null)
			{
				LoadSearchFile(file.FullName, false, false, searchList);
				DisplaySearchList();
			}
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OpenSearchFile();
		}

		private void openToolStripButton_Click(object sender, EventArgs e)
		{
			OpenSearchFile();
		}

		private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveAs();
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.Hide();
		}

		private void SpecificValueRadio_CheckedChanged(object sender, EventArgs e)
		{
			if (SpecificValueRadio.Checked)
			{
				if (SpecificValueBox.Text == "") SpecificValueBox.Text = "0";
				SpecificValueBox.Enabled = true;
				SpecificAddressBox.Enabled = false;
				NumberOfChangesBox.Enabled = false;
				SpecificValueBox.Focus();
				SpecificValueBox.SelectAll();
			}
			DoPreview();
		}

		private void PreviousValueRadio_CheckedChanged(object sender, EventArgs e)
		{
			if (PreviousValueRadio.Checked)
			{
				SpecificValueBox.Enabled = false;
				SpecificAddressBox.Enabled = false;
				NumberOfChangesBox.Enabled = false;
			}
			DoPreview();
		}

		private void SpecificAddressRadio_CheckedChanged(object sender, EventArgs e)
		{
			if (SpecificAddressRadio.Checked)
			{
				if (SpecificAddressBox.Text == "") SpecificAddressBox.Text = "0";
				SpecificValueBox.Enabled = false;
				SpecificAddressBox.Enabled = true;
				NumberOfChangesBox.Enabled = false;
				SpecificAddressBox.Focus();
				SpecificAddressBox.SelectAll();
			}
			DoPreview();
		}

		private void NumberOfChangesRadio_CheckedChanged(object sender, EventArgs e)
		{
			if (NumberOfChangesRadio.Checked)
			{
				if (NumberOfChangesBox.Text == "") NumberOfChangesBox.Text = "0";
				SpecificValueBox.Enabled = false;
				SpecificAddressBox.Enabled = false;
				NumberOfChangesBox.Enabled = true;
				NumberOfChangesBox.Focus();
				NumberOfChangesBox.SelectAll();
			}
		}

		private void DifferentByRadio_CheckedChanged(object sender, EventArgs e)
		{
			if (DifferentByRadio.Checked)
			{
				if (DifferentByBox.Text == "0") DifferentByBox.Text = "0";
				DifferentByBox.Enabled = true;
				DoPreview();
			}
			else
				DifferentByBox.Enabled = false;
			DifferentByBox.Focus();
			DifferentByBox.SelectAll();
		}

		private void AddToRamWatch()
		{
			ListView.SelectedIndexCollection indexes = SearchListView.SelectedIndices;

			if (indexes.Count > 0)
			{
				Global.MainForm.LoadRamWatch();
				for (int x = 0; x < indexes.Count; x++)
					Global.MainForm.RamWatch1.AddWatch(searchList[indexes[x]]);
			}
		}

		private void WatchtoolStripButton1_Click(object sender, EventArgs e)
		{
			AddToRamWatch();
		}

		private void restoreOriginalWindowSizeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.Size = new System.Drawing.Size(defaultWidth, defaultHeight);
			Global.Config.RamSearchAddressIndex = 0;
			Global.Config.RamSearchValueIndex = 1;
			Global.Config.RamSearchPrevIndex = 2;
			Global.Config.RamSearchChangesIndex = 3;
			ColumnPositionSet();

			SearchListView.Columns[0].Width = defaultAddressWidth;
			SearchListView.Columns[1].Width = defaultValueWidth;
			SearchListView.Columns[2].Width = defaultPrevWidth;
			SearchListView.Columns[3].Width = defaultChangesWidth;
		}

		private void NewSearchtoolStripButton_Click(object sender, EventArgs e)
		{
			StartNewSearch();
		}

		private asigned GetDataType()
		{
			if (unsignedToolStripMenuItem.Checked)
				return asigned.UNSIGNED;
			if (signedToolStripMenuItem.Checked)
				return asigned.SIGNED;
			if (hexadecimalToolStripMenuItem.Checked)
				return asigned.HEX;

			return asigned.UNSIGNED;    //Just in case
		}

		private atype GetDataSize()
		{
			if (byteToolStripMenuItem.Checked)
				return atype.BYTE;
			if (bytesToolStripMenuItem.Checked)
				return atype.WORD;
			if (dWordToolStripMenuItem1.Checked)
				return atype.DWORD;

			return atype.BYTE;
		}

		private bool GetBigEndian()
		{
			if (bigEndianToolStripMenuItem.Checked)
				return true;
			else
				return false;
		}

		private void StartNewSearch()
		{
			ClearUndo();
			ClearRedo();
			weededList.Clear();
			IsAWeededList = false;
			searchList.Clear();
			SetPlatformAndMemoryDomainLabel();
			int count = 0;
			int divisor = 1;

			if (!includeMisalignedToolStripMenuItem.Checked)
			{
				switch (GetDataSize())
				{
					case atype.WORD:
						divisor = 2;
						break;
					case atype.DWORD:
						divisor = 4;
						break;
					default:
						divisor = 1;
						break;
				}
			}

			for (int x = 0; x <= ((Domain.Size / divisor) - 1); x++)
			{
				searchList.Add(new Watch());
				searchList[x].address = count;
				searchList[x].type = GetDataSize();
				searchList[x].bigendian = GetBigEndian();
				searchList[x].signed = GetDataType();
				searchList[x].PeekAddress(Domain);
				searchList[x].prev = searchList[x].value;
				if (includeMisalignedToolStripMenuItem.Checked)
					count++;
				else
				{
					switch (GetDataSize())
					{
						case atype.BYTE:
							count++;
							break;
						case atype.WORD:
							count += 2;
							break;
						case atype.DWORD:
							count += 4;
							break;
					}
				}

			}
			if (Global.Config.AlwaysExludeRamWatch)
				ExludeRamWatchList();
			MakePreviousList();
			SetSpecificValueBoxMaxLength();
			OutputLabel.Text = "New search started";
			sortReverse = false;
			sortedCol = "";
			DisplaySearchList();
		}

		private void MakePreviousList()
		{
			prevList = new List<Watch>();

			for (int x = 0; x < searchList.Count; x++)
				prevList.Add(new Watch(searchList[x]));
		}

		private void DisplaySearchList()
		{
			SearchListView.ItemCount = searchList.Count;
			SetTotal();
		}

		private void newSearchToolStripMenuItem_Click(object sender, EventArgs e)
		{
			StartNewSearch();
		}

		private void SetPlatformAndMemoryDomainLabel()
		{
			string memoryDomain = Domain.ToString();
			systemID = Global.Emulator.SystemId;
			MemDomainLabel.Text = systemID + " " + memoryDomain;
		}

		private Point GetPromptPoint()
		{

			Point p = new Point(SearchListView.Location.X, SearchListView.Location.Y);
			Point q = new Point();
			q = PointToScreen(p);
			return q;
		}

		private void PokeAddress()
		{
			ListView.SelectedIndexCollection indexes = SearchListView.SelectedIndices;
			Global.Sound.StopSound();
			RamPoke p = new RamPoke();
			Global.Sound.StartSound();

			if (indexes.Count > 0)
				p.SetWatchObject(searchList[indexes[0]]);
			p.location = GetPromptPoint();
			p.ShowDialog();
		}

		private void PoketoolStripButton1_Click(object sender, EventArgs e)
		{
			PokeAddress();
		}

		private string MakeAddressString(int num)
		{
			if (num == 1)
				return " 1 address";
			else if (num < 10)
				return " " + num.ToString() + " addresses";
			else
				return num.ToString() + " addresses";
		}

		private void RemoveAddresses()
		{
			ListView.SelectedIndexCollection indexes = SearchListView.SelectedIndices;
			if (indexes.Count > 0)
			{
				SaveUndo();
				OutputLabel.Text = MakeAddressString(indexes.Count) + " removed";
				for (int x = 0; x < indexes.Count; x++)
				{
					searchList.Remove(searchList[indexes[x] - x]);
				}
				indexes.Clear();
				DisplaySearchList();
			}
		}

		private void cutToolStripButton_Click(object sender, EventArgs e)
		{
			RemoveAddresses();
		}

		/// <summary>
		/// Saves the current search list to the undo list
		/// This function should be called before any destructive operation to the list!
		/// </summary>
		private void SaveUndo()
		{
			undoList.Clear();
			for (int x = 0; x < searchList.Count; x++)
				undoList.Add(new Watch(searchList[x]));
			UndotoolStripButton.Enabled = true;
		}

		private void DoUndo()
		{
			if (undoList.Count > 0)
			{
				OutputLabel.Text = MakeAddressString(undoList.Count - searchList.Count) + " restored";
				redoList = new List<Watch>(searchList);
				searchList = new List<Watch>(undoList);
				prevList = new List<Watch>(undoList);
				ClearUndo();
				RedotoolStripButton2.Enabled = true;
				DisplaySearchList();
				//OutputLabel.Text = "Undo: s" + searchList.Count.ToString() + " u" +
				//	undoList.Count.ToString() + " r" + redoList.Count.ToString();
			}
		}

		private void ClearUndo()
		{
			undoList.Clear();
			UndotoolStripButton.Enabled = false;
		}

		private void ClearRedo()
		{
			redoList.Clear();
			RedotoolStripButton2.Enabled = false;
		}

		private void DoRedo()
		{
			if (redoList.Count > 0)
			{
				OutputLabel.Text = MakeAddressString(searchList.Count - redoList.Count) + " removed";
				undoList = new List<Watch>(searchList);
				searchList = new List<Watch>(redoList);
				prevList = new List<Watch>(redoList);
				ClearRedo();
				UndotoolStripButton.Enabled = true;
				DisplaySearchList();
				//OutputLabel.Text = "Redo: s" + searchList.Count.ToString() + " u" +
				//	undoList.Count.ToString() + " r" + redoList.Count.ToString();
			}
		}

		private void UndotoolStripButton_Click(object sender, EventArgs e)
		{
			DoUndo();
		}

		private void SearchListView_QueryItemBkColor(int index, int column, ref Color color)
		{
			if (IsAWeededList && column == 0)
			{
				if (!weededList.Contains(searchList[index]))
				{
					if (color == Color.Pink) return;
					if (Global.CheatList.IsActiveCheat(Domain, searchList[index].address))
						color = Color.Purple;
					else
						color = Color.Pink;
				}
				else if (Global.CheatList.IsActiveCheat(Domain, searchList[index].address))
					color = Color.LightCyan;
				else
				{
					if (color == Color.White) return;
					color = Color.White;
				}
			}
		}

		private void SearchListView_QueryItemText(int index, int column, out string text)
		{
			text = "";
			if (column == 0)
			{
				text = searchList[index].address.ToString(addressFormatStr);
			}
			if (column == 1)
			{
				text = searchList[index].ValueToString();
			}
			if (column == 2)
			{
				if (searchList[index].signed == asigned.UNSIGNED)
				{
					if (Global.Config.RamSearchPreviousAs == 2) //If prev frame
						text = searchList[index].prev.ToString();
					else
						text = prevList[index].value.ToString();
				}
				else if (searchList[index].signed == asigned.SIGNED)
				{
					if (Global.Config.RamSearchPreviousAs == 2) //If prev frame
						text = ((sbyte)searchList[index].prev).ToString();
					else
						text = ((sbyte)prevList[index].value).ToString();
				}
				else if (searchList[index].signed == asigned.HEX)
				{
					if (Global.Config.RamSearchPreviousAs == 2) //If prev frame
					{
						text = searchList[index].PrevToString();
					}
					else
					{
						text = prevList[index].ValueToString();
					}
				}
			}
			if (column == 3)
			{
				text = searchList[index].changecount.ToString();
			}
		}

		private void ClearChangeCounts()
		{
			SaveUndo();
			for (int x = 0; x < searchList.Count; x++)
				searchList[x].changecount = 0;
			DisplaySearchList();
			OutputLabel.Text = "Change counts cleared";
		}

		private void ClearChangeCountstoolStripButton_Click(object sender, EventArgs e)
		{
			ClearChangeCounts();
		}

		private void UndotoolStripButton_Click_1(object sender, EventArgs e)
		{
			DoUndo();
		}

		private void ReplaceSearchListWithWeedOutList()
		{
			searchList = new List<Watch>(weededList);
			weededList.Clear();
			IsAWeededList = false;
		}

		private void DoPreview()
		{
			if (Global.Config.RamSearchPreviewMode)
			{
				if (GenerateWeedOutList())
				{
					//OutputLabel.Text = MakeAddressString(searchList.Count - weededList.Count) + " would be removed";
				}
			}
		}

		private void DoSearch()
		{
			if (GenerateWeedOutList())
			{
				SaveUndo();
				OutputLabel.Text = MakeAddressString(searchList.Count - weededList.Count) + " removed";
				ReplaceSearchListWithWeedOutList();
				if (Global.Config.RamSearchPreviousAs != 1) MakePreviousList(); //1 = Original value
				DisplaySearchList();
			}
			else
				OutputLabel.Text = "Search failed.";
		}

		private void toolStripButton1_Click(object sender, EventArgs e)
		{
			DoSearch();
		}

		private SCompareTo GetCompareTo()
		{
			if (PreviousValueRadio.Checked)
				return SCompareTo.PREV;
			if (SpecificValueRadio.Checked)
				return SCompareTo.VALUE;
			if (SpecificAddressRadio.Checked)
				return SCompareTo.ADDRESS;
			if (NumberOfChangesRadio.Checked)
				return SCompareTo.CHANGES;

			return SCompareTo.PREV; //Just in case
		}

		private SOperator GetOperator()
		{
			if (LessThanRadio.Checked)
				return SOperator.LESS;
			if (GreaterThanRadio.Checked)
				return SOperator.GREATER;
			if (LessThanOrEqualToRadio.Checked)
				return SOperator.LESSEQUAL;
			if (GreaterThanOrEqualToRadio.Checked)
				return SOperator.GREATEREQUAL;
			if (EqualToRadio.Checked)
				return SOperator.EQUAL;
			if (NotEqualToRadio.Checked)
				return SOperator.NOTEQUAL;
			if (DifferentByRadio.Checked)
				return SOperator.DIFFBY;

			return SOperator.LESS; //Just in case
		}

		private bool GenerateWeedOutList()
		{
			//Switch based on user criteria
			//Generate search list
			//Use search list to generate a list of flagged address (for displaying pink)
			IsAWeededList = true;
			weededList.Clear();
			switch (GetCompareTo())
			{
				case SCompareTo.PREV:
					return DoPreviousValue();
				case SCompareTo.VALUE:
					return DoSpecificValue();
				case SCompareTo.ADDRESS:
					return DoSpecificAddress();
				case SCompareTo.CHANGES:
					return DoNumberOfChanges();
				default:
					return false;
			}
		}

		private int GetPreviousValue(int pos)
		{
			if (Global.Config.RamSearchPreviousAs == 2) //If Previous frame
				return searchList[pos].prev;
			else
			{
				if (pos < prevList.Count)
					return prevList[pos].value;
				else
				{
					return 0;
				}
			}
		}

		private bool DoPreviousValue()
		{
			switch (GetOperator())
			{
				case SOperator.LESS:
					for (int x = 0; x < searchList.Count; x++)
					{
						if (searchList[x].value < GetPreviousValue(x))
							weededList.Add(searchList[x]);
					}
					break;
				case SOperator.GREATER:
					for (int x = 0; x < searchList.Count; x++)
					{
						if (searchList[x].value > GetPreviousValue(x))
							weededList.Add(searchList[x]);
					}
					break;
				case SOperator.LESSEQUAL:
					for (int x = 0; x < searchList.Count; x++)
					{
						if (searchList[x].value <= GetPreviousValue(x))
							weededList.Add(searchList[x]);
					}
					break;
				case SOperator.GREATEREQUAL:
					for (int x = 0; x < searchList.Count; x++)
					{
						if (searchList[x].value >= GetPreviousValue(x))
							weededList.Add(searchList[x]);
					}
					break;
				case SOperator.EQUAL:
					for (int x = 0; x < searchList.Count; x++)
					{
						if (searchList[x].value == GetPreviousValue(x))
							weededList.Add(searchList[x]);
					}
					break;
				case SOperator.NOTEQUAL:
					for (int x = 0; x < searchList.Count; x++)
					{
						if (searchList[x].value != GetPreviousValue(x))
							weededList.Add(searchList[x]);
					}
					break;
				case SOperator.DIFFBY:
					int diff = GetDifferentBy();
					if (diff < 0) return false;
					for (int x = 0; x < searchList.Count; x++)
					{
						if (searchList[x].value == GetPreviousValue(x) + diff || searchList[x].value == GetPreviousValue(x) - diff)
							weededList.Add(searchList[x]);
					}
					break;
			}
			return true;
		}

		private bool DoSpecificValue()
		{
			int value = GetSpecificValue();
			if (value < -99999999)
			{
				MessageBox.Show("Missing or invalid value", "Invalid value", MessageBoxButtons.OK, MessageBoxIcon.Error);
				SpecificValueBox.Focus();
				SpecificValueBox.SelectAll();
				return false;
			}
			switch (GetOperator())
			{
				case SOperator.LESS:
					for (int x = 0; x < searchList.Count; x++)
					{
						if (searchList[x].value < value)
							weededList.Add(searchList[x]);
					}
					break;
				case SOperator.GREATER:
					for (int x = 0; x < searchList.Count; x++)
					{
						if (searchList[x].value > value)
							weededList.Add(searchList[x]);
					}
					break;
				case SOperator.LESSEQUAL:
					for (int x = 0; x < searchList.Count; x++)
					{
						if (searchList[x].value <= value)
							weededList.Add(searchList[x]);
					}
					break;
				case SOperator.GREATEREQUAL:
					for (int x = 0; x < searchList.Count; x++)
					{
						if (searchList[x].value >= value)
							weededList.Add(searchList[x]);
					}
					break;
				case SOperator.EQUAL:
					for (int x = 0; x < searchList.Count; x++)
					{
						if (searchList[x].value == value)
							weededList.Add(searchList[x]);
					}
					break;
				case SOperator.NOTEQUAL:
					for (int x = 0; x < searchList.Count; x++)
					{
						if (searchList[x].value != value)
							weededList.Add(searchList[x]);
					}
					break;
				case SOperator.DIFFBY:
					int diff = GetDifferentBy();
					if (diff < 0) return false;
					for (int x = 0; x < searchList.Count; x++)
					{
						if (searchList[x].value == value + diff || searchList[x].value == value - diff)
							weededList.Add(searchList[x]);
					}
					break;
			}
			return true;
		}

		private int GetSpecificValue()
		{
			if (SpecificValueBox.Text == "" || SpecificValueBox.Text == "-") return 0;
			bool i = false;
			switch (GetDataType())
			{
				case asigned.UNSIGNED:
					i = InputValidate.IsValidUnsignedNumber(SpecificValueBox.Text);
					if (!i) return -99999999;
					return (int)Int64.Parse(SpecificValueBox.Text); //Note: 64 to be safe since 4 byte values can be entered
				case asigned.SIGNED:
					i = InputValidate.IsValidSignedNumber(SpecificValueBox.Text);
					if (!i) return -99999999;
					return (int)Int64.Parse(SpecificValueBox.Text);
				case asigned.HEX:
					i = InputValidate.IsValidHexNumber(SpecificValueBox.Text);
					if (!i) return -99999999;
					return (int)Int64.Parse(SpecificValueBox.Text, NumberStyles.HexNumber);
			}
			return -99999999; //What are the odds someone wants to search for this value?
		}

		private int GetSpecificAddress()
		{
			if (SpecificAddressBox.Text == "") return 0;
			bool i = InputValidate.IsValidHexNumber(SpecificAddressBox.Text);
			if (!i) return -1;

			return int.Parse(SpecificAddressBox.Text, NumberStyles.HexNumber);
		}

		private int GetDifferentBy()
		{
			if (DifferentByBox.Text == "") return 0;
			bool i = InputValidate.IsValidUnsignedNumber(DifferentByBox.Text);
			if (!i)
			{
				MessageBox.Show("Missing or invalid Different By value", "Invalid value", MessageBoxButtons.OK, MessageBoxIcon.Error);
				DifferentByBox.Focus();
				DifferentByBox.SelectAll();
				return -1;
			}
			else
				return int.Parse(DifferentByBox.Text);
		}

		private bool DoSpecificAddress()
		{
			int address = GetSpecificAddress();
			if (address < 0)
			{
				MessageBox.Show("Missing or invalid address", "Invalid address", MessageBoxButtons.OK, MessageBoxIcon.Error);
				SpecificAddressBox.Focus();
				SpecificAddressBox.SelectAll();
				return false;
			}
			switch (GetOperator())
			{
				case SOperator.LESS:
					for (int x = 0; x < searchList.Count; x++)
					{
						if (searchList[x].address < address)
							weededList.Add(searchList[x]);
					}
					break;
				case SOperator.GREATER:
					for (int x = 0; x < searchList.Count; x++)
					{
						if (searchList[x].address > address)
							weededList.Add(searchList[x]);
					}
					break;
				case SOperator.LESSEQUAL:
					for (int x = 0; x < searchList.Count; x++)
					{
						if (searchList[x].address <= address)
							weededList.Add(searchList[x]);
					}
					break;
				case SOperator.GREATEREQUAL:
					for (int x = 0; x < searchList.Count; x++)
					{
						if (searchList[x].address >= address)
							weededList.Add(searchList[x]);
					}
					break;
				case SOperator.EQUAL:
					for (int x = 0; x < searchList.Count; x++)
					{
						if (searchList[x].address == address)
							weededList.Add(searchList[x]);
					}
					break;
				case SOperator.NOTEQUAL:
					for (int x = 0; x < searchList.Count; x++)
					{
						if (searchList[x].address != address)
							weededList.Add(searchList[x]);
					}
					break;
				case SOperator.DIFFBY:
					{
						int diff = GetDifferentBy();
						if (diff < 0) return false;
						for (int x = 0; x < searchList.Count; x++)
						{
							if (searchList[x].address == address + diff || searchList[x].address == address - diff)
								weededList.Add(searchList[x]);
						}
					}
					break;
			}
			return true;
		}

		private int GetSpecificChanges()
		{
			if (NumberOfChangesBox.Text == "") return 0;
			bool i = InputValidate.IsValidUnsignedNumber(NumberOfChangesBox.Text);
			if (!i) return -1;

			return int.Parse(NumberOfChangesBox.Text);
		}

		private bool DoNumberOfChanges()
		{
			int changes = GetSpecificChanges();
			if (changes < 0)
			{
				MessageBox.Show("Missing or invalid number of changes", "Invalid number", MessageBoxButtons.OK, MessageBoxIcon.Error);
				NumberOfChangesBox.Focus();
				NumberOfChangesBox.SelectAll();
				return false;
			}
			switch (GetOperator())
			{
				case SOperator.LESS:
					for (int x = 0; x < searchList.Count; x++)
					{
						if (searchList[x].changecount < changes)
							weededList.Add(searchList[x]);
					}
					break;
				case SOperator.GREATER:
					for (int x = 0; x < searchList.Count; x++)
					{
						if (searchList[x].changecount > changes)
							weededList.Add(searchList[x]);
					}
					break;
				case SOperator.LESSEQUAL:
					for (int x = 0; x < searchList.Count; x++)
					{
						if (searchList[x].changecount <= changes)
							weededList.Add(searchList[x]);
					}
					break;
				case SOperator.GREATEREQUAL:
					for (int x = 0; x < searchList.Count; x++)
					{
						if (searchList[x].changecount >= changes)
							weededList.Add(searchList[x]);
					}
					break;
				case SOperator.EQUAL:
					for (int x = 0; x < searchList.Count; x++)
					{
						if (searchList[x].changecount == changes)
							weededList.Add(searchList[x]);
					}
					break;
				case SOperator.NOTEQUAL:
					for (int x = 0; x < searchList.Count; x++)
					{
						if (searchList[x].changecount != changes)
							weededList.Add(searchList[x]);
					}
					break;
				case SOperator.DIFFBY:
					int diff = GetDifferentBy();
					if (diff < 0) return false;
					for (int x = 0; x < searchList.Count; x++)
					{
						if (searchList[x].address == changes + diff || searchList[x].address == changes - diff)
							weededList.Add(searchList[x]);
					}
					break;
			}
			return true;
		}

		private void ConvertListDataType(asigned s)
		{
			for (int x = 0; x < searchList.Count; x++)
				searchList[x].signed = s;
		}

		private void signedToolStripMenuItem_Click(object sender, EventArgs e)
		{
			unsignedToolStripMenuItem.Checked = false;
			signedToolStripMenuItem.Checked = true;
			hexadecimalToolStripMenuItem.Checked = false;
			ConvertListDataType(asigned.SIGNED);
			SetSpecificValueBoxMaxLength();
			DisplaySearchList();
		}

		private void unsignedToolStripMenuItem_Click(object sender, EventArgs e)
		{
			unsignedToolStripMenuItem.Checked = true;
			signedToolStripMenuItem.Checked = false;
			hexadecimalToolStripMenuItem.Checked = false;
			ConvertListDataType(asigned.UNSIGNED);
			SetSpecificValueBoxMaxLength();
			DisplaySearchList();
		}

		private void hexadecimalToolStripMenuItem_Click(object sender, EventArgs e)
		{
			unsignedToolStripMenuItem.Checked = false;
			signedToolStripMenuItem.Checked = false;
			hexadecimalToolStripMenuItem.Checked = true;
			ConvertListDataType(asigned.HEX);
			SetSpecificValueBoxMaxLength();
			DisplaySearchList();
		}

		private void SearchListView_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			ListView.SelectedIndexCollection indexes = SearchListView.SelectedIndices;
			if (indexes.Count > 0)
			{
				AddToRamWatch();
			}
		}

		private void SetSpecificValueBoxMaxLength()
		{
			switch (GetDataType())
			{
				case asigned.UNSIGNED:
					switch (GetDataSize())
					{
						case atype.BYTE:
							SpecificValueBox.MaxLength = 3;
							break;
						case atype.WORD:
							SpecificValueBox.MaxLength = 5;
							break;
						case atype.DWORD:
							SpecificValueBox.MaxLength = 10;
							break;
						default:
							SpecificValueBox.MaxLength = 10;
							break;
					}
					break;
				case asigned.SIGNED:
					switch (GetDataSize())
					{
						case atype.BYTE:
							SpecificValueBox.MaxLength = 4;
							break;
						case atype.WORD:
							SpecificValueBox.MaxLength = 6;
							break;
						case atype.DWORD:
							SpecificValueBox.MaxLength = 11;
							break;
						default:
							SpecificValueBox.MaxLength = 11;
							break;
					}
					break;
				case asigned.HEX:
					switch (GetDataSize())
					{
						case atype.BYTE:
							SpecificValueBox.MaxLength = 2;
							break;
						case atype.WORD:
							SpecificValueBox.MaxLength = 4;
							break;
						case atype.DWORD:
							SpecificValueBox.MaxLength = 8;
							break;
						default:
							SpecificValueBox.MaxLength = 8;
							break;
					}
					break;
				default:
					SpecificValueBox.MaxLength = 11;
					break;
			}
		}

		private void byteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			byteToolStripMenuItem.Checked = true;
			bytesToolStripMenuItem.Checked = false;
			dWordToolStripMenuItem1.Checked = false;
			SetSpecificValueBoxMaxLength();
		}

		private void bytesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			byteToolStripMenuItem.Checked = false;
			bytesToolStripMenuItem.Checked = true;
			dWordToolStripMenuItem1.Checked = false;
			SetSpecificValueBoxMaxLength();
		}

		private void dWordToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			byteToolStripMenuItem.Checked = false;
			bytesToolStripMenuItem.Checked = false;
			dWordToolStripMenuItem1.Checked = true;
			SetSpecificValueBoxMaxLength();
		}

		private void bigEndianToolStripMenuItem_Click(object sender, EventArgs e)
		{
			bigEndianToolStripMenuItem.Checked = true;
			littleEndianToolStripMenuItem.Checked = false;
		}

		private void littleEndianToolStripMenuItem_Click(object sender, EventArgs e)
		{
			bigEndianToolStripMenuItem.Checked = false;
			littleEndianToolStripMenuItem.Checked = true;
		}

		private void AutoSearchCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			if (AutoSearchCheckBox.Checked)
				AutoSearchCheckBox.BackColor = Color.Pink;
			else
				AutoSearchCheckBox.BackColor = this.BackColor;
		}

		private void SpecificValueBox_Leave(object sender, EventArgs e)
		{
			DoPreview();
		}

		private void SpecificAddressBox_Leave(object sender, EventArgs e)
		{
			DoPreview();
		}

		private void NumberOfChangesBox_Leave(object sender, EventArgs e)
		{
			DoPreview();
		}

		private void DifferentByBox_Leave(object sender, EventArgs e)
		{
			if (!InputValidate.IsValidUnsignedNumber(DifferentByBox.Text))  //Actually the only way this could happen is from putting dashes after the first character
			{
				DifferentByBox.Focus();
				DifferentByBox.SelectAll();
				ToolTip t = new ToolTip();
				t.Show("Must be a valid unsigned decimal value", DifferentByBox, 5000);
				return;
			}
			DoPreview();
		}

		private bool SaveSearchFile(string path)
		{
			var file = new FileInfo(path);

			using (StreamWriter sw = new StreamWriter(path))
			{
				string str = "Domain " + Domain.Name + "\n";

				for (int x = 0; x < searchList.Count; x++)
				{
					str += string.Format("{0:X4}", searchList[x].address) + "\t";
					str += searchList[x].GetTypeByChar().ToString() + "\t";
					str += searchList[x].GetSignedByChar().ToString() + "\t";

					if (searchList[x].bigendian == true)
						str += "1\t";
					else
						str += "0\t";

					str += searchList[x].notes + "\n";
				}

				sw.WriteLine(str);
			}
			return true;
		}

		private FileInfo GetSaveFileFromUser()
		{
			var sfd = new SaveFileDialog();
			if (currentSearchFile.Length > 0)
			{
				sfd.FileName = Path.GetFileNameWithoutExtension(currentSearchFile);
				sfd.InitialDirectory = Path.GetDirectoryName(currentSearchFile);
			}
			else if (!(Global.Emulator is NullEmulator))
			{
			    sfd.FileName = PathManager.FilesystemSafeName(Global.Game);
				sfd.InitialDirectory = PathManager.MakeAbsolutePath(Global.Config.WatchPath, "");
			}
			else
			{
				sfd.FileName = "NULL";
				sfd.InitialDirectory = PathManager.MakeAbsolutePath(Global.Config.WatchPath, "");
			}
			sfd.Filter = "Watch Files (*.wch)|*.wch|All Files|*.*";
			sfd.RestoreDirectory = true;
			Global.Sound.StopSound();
			var result = sfd.ShowDialog();
			Global.Sound.StartSound();
			if (result != DialogResult.OK)
				return null;
			var file = new FileInfo(sfd.FileName);
			return file;
		}

		public void SaveAs()
		{
			var file = GetSaveFileFromUser();
			if (file != null)
			{
				SaveSearchFile(file.FullName);
				currentSearchFile = file.FullName;
				OutputLabel.Text = Path.GetFileName(currentSearchFile) + " saved.";
				Global.Config.RecentSearches.Add(currentSearchFile);
			}
		}

		private void LoadSearchFromRecent(string file)
		{
			bool r = LoadSearchFile(file, false, false, searchList);
			if (!r)
			{
				DialogResult result = MessageBox.Show("Could not open " + file + "\nRemove from list?", "File not found", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
				if (result == DialogResult.Yes)
					Global.Config.RecentSearches.Remove(file);
			}
			DisplaySearchList();
		}

		public int HowMany(string str, char c)
		{
			int count = 0;
			for (int x = 0; x < str.Length; x++)
			{
				if (str[x] == c)
					count++;
			}
			return count;
		}

		private int GetDomainPos(string name)
		{
			//Attempts to find the memory domain by name, if it fails, it defaults to index 0
			for (int x = 0; x < Global.Emulator.MemoryDomains.Count; x++)
			{
				if (Global.Emulator.MemoryDomains[x].Name == name)
					return x;
			}
			return 0;
		}

		bool LoadSearchFile(string path, bool append, bool truncate, List<Watch> list)
		{
			int y, z;
			var file = new FileInfo(path);
			if (file.Exists == false) return false;

			using (StreamReader sr = file.OpenText())
			{
				if (!append && !truncate)
					currentSearchFile = path;

				int count = 0;
				string s = "";
				string temp = "";

				if (!append)
					list.Clear();  //Wipe existing list and read from file

				while ((s = sr.ReadLine()) != null)
				{
					//parse each line and add to watchList

					//.wch files from other emulators start with a number representing the number of watch, that line can be discarded here
					//Any properly formatted line couldn't possibly be this short anyway, this also takes care of any garbage lines that might be in a file
					if (s.Length < 5) continue;

					if (s.Substring(0, 6) == "Domain")
						SetMemoryDomain(GetDomainPos(s.Substring(7, s.Length - 7)));

					z = HowMany(s, '\t');
					if (z == 5)
					{
						//If 5, then this is a .wch file format made from another emulator, the first column (watch position) is not needed here
						y = s.IndexOf('\t') + 1;
						s = s.Substring(y, s.Length - y);   //5 digit value representing the watch position number
					}
					else if (z != 4)
						continue;   //If not 4, something is wrong with this line, ignore it
					count++;
					Watch w = new Watch();

					temp = s.Substring(0, s.IndexOf('\t'));
					w.address = int.Parse(temp, NumberStyles.HexNumber);

					y = s.IndexOf('\t') + 1;
					s = s.Substring(y, s.Length - y);   //Type
					w.SetTypeByChar(s[0]);

					y = s.IndexOf('\t') + 1;
					s = s.Substring(y, s.Length - y);   //Signed
					w.SetSignedByChar(s[0]);

					y = s.IndexOf('\t') + 1;
					s = s.Substring(y, s.Length - y);   //Endian
					y = Int16.Parse(s[0].ToString());
					if (y == 0)
						w.bigendian = false;
					else
						w.bigendian = true;

					//w.notes = s.Substring(2, s.Length - 2);   //User notes

					list.Add(w);
				}

				if (!append && !truncate)
				{
					Global.Config.RecentSearches.Add(file.FullName);
					OutputLabel.Text = Path.GetFileName(file.FullName);
					//Update the number of watches
					SetTotal();
				}
			}

			return true;
		}

		private void recentToolStripMenuItem_DropDownOpened(object sender, EventArgs e)
		{
			//Clear out recent Roms list
			//repopulate it with an up to date list
			recentToolStripMenuItem.DropDownItems.Clear();

			if (Global.Config.RecentSearches.IsEmpty())
			{
				var none = new ToolStripMenuItem();
				none.Enabled = false;
				none.Text = "None";
				recentToolStripMenuItem.DropDownItems.Add(none);
			}
			else
			{
				for (int x = 0; x < Global.Config.RecentSearches.Length(); x++)
				{
					string path = Global.Config.RecentSearches.GetRecentFileByPosition(x);
					var item = new ToolStripMenuItem();
					item.Text = path;
					item.Click += (o, ev) => LoadSearchFromRecent(path);
					recentToolStripMenuItem.DropDownItems.Add(item);
				}
			}

			recentToolStripMenuItem.DropDownItems.Add("-");

			var clearitem = new ToolStripMenuItem();
			clearitem.Text = "&Clear";
			clearitem.Click += (o, ev) => Global.Config.RecentSearches.Clear();
			recentToolStripMenuItem.DropDownItems.Add(clearitem);

			var auto = new ToolStripMenuItem();
			auto.Text = "&Auto-Load";
			auto.Click += (o, ev) => UpdateAutoLoadRamSearch();
			if (Global.Config.AutoLoadRamSearch == true)
				auto.Checked = true;
			else
				auto.Checked = false;
			recentToolStripMenuItem.DropDownItems.Add(auto);
		}

		private void UpdateAutoLoadRamSearch()
		{
			autoLoadToolStripMenuItem.Checked = Global.Config.AutoLoadRamSearch ^= true;
		}

		private void appendFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var file = GetFileFromUser();
			if (file != null)
				LoadSearchFile(file.FullName, true, false, searchList);
			DisplaySearchList();
		}

		private FileInfo GetFileFromUser()
		{
			var ofd = new OpenFileDialog();
			if (currentSearchFile.Length > 0)
				ofd.FileName = Path.GetFileNameWithoutExtension(currentSearchFile);
			ofd.InitialDirectory = PathManager.MakeAbsolutePath(Global.Config.WatchPath, "");
			ofd.Filter = "Watch Files (*.wch)|*.wch|All Files|*.*";
			ofd.RestoreDirectory = true;
			if (currentSearchFile.Length > 0)
				ofd.FileName = Path.GetFileNameWithoutExtension(currentSearchFile);
			Global.Sound.StopSound();
			var result = ofd.ShowDialog();
			Global.Sound.StartSound();
			if (result != DialogResult.OK)
				return null;
			var file = new FileInfo(ofd.FileName);
			return file;
		}

		private void includeMisalignedToolStripMenuItem_Click(object sender, EventArgs e)
		{
			includeMisalignedToolStripMenuItem.Checked ^= true;
		}

		private void saveWindowPositionToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Global.Config.RamSearchSaveWindowPosition ^= true;
		}

		private void optionsToolStripMenuItem_DropDownOpened(object sender, EventArgs e)
		{
			saveWindowPositionToolStripMenuItem.Checked = Global.Config.RamSearchSaveWindowPosition;
			previewModeToolStripMenuItem.Checked = Global.Config.RamSearchPreviewMode;
			alwaysExludeRamSearchListToolStripMenuItem.Checked = Global.Config.AlwaysExludeRamWatch;
		}

		private void searchToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			DoSearch();
		}

		private void clearChangeCountsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ClearChangeCounts();
		}

		private void undoToolStripMenuItem_Click_1(object sender, EventArgs e)
		{
			DoUndo();
		}

		private void removeSelectedToolStripMenuItem_Click(object sender, EventArgs e)
		{
			RemoveAddresses();
		}

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (string.Compare(currentSearchFile, "") == 0) 
				SaveAs();
			else
				SaveSearchFile(currentSearchFile);
		}

		private void addSelectedToRamWatchToolStripMenuItem_Click(object sender, EventArgs e)
		{
			AddToRamWatch();
		}

		private void pokeAddressToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PokeAddress();
		}

		private void searchToolStripMenuItem_DropDownOpened(object sender, EventArgs e)
		{
			if (searchList.Count == 0)
				searchToolStripMenuItem.Enabled = false;
			else
				searchToolStripMenuItem.Enabled = true;

			if (undoList.Count == 0)
				undoToolStripMenuItem.Enabled = false;
			else
				undoToolStripMenuItem.Enabled = true;

			ListView.SelectedIndexCollection indexes = SearchListView.SelectedIndices;

			if (indexes.Count == 0)
			{
				removeSelectedToolStripMenuItem.Enabled = false;
				addSelectedToRamWatchToolStripMenuItem.Enabled = false;
				pokeAddressToolStripMenuItem.Enabled = false;
			}
			else
			{
				removeSelectedToolStripMenuItem.Enabled = true;
				addSelectedToRamWatchToolStripMenuItem.Enabled = true;
				pokeAddressToolStripMenuItem.Enabled = true;
			}
		}

		private void sinceLastSearchToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Global.Config.RamSearchPreviousAs = 0;
		}

		private void sinceLastFrameToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Global.Config.RamSearchPreviousAs = 2;
		}

		private void originalValueToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Global.Config.RamSearchPreviousAs = 1;
		}

		private void definePreviousValueToolStripMenuItem_DropDownOpened(object sender, EventArgs e)
		{
			switch (Global.Config.RamSearchPreviousAs)
			{
				case 0: //Since last Search
					sinceLastSearchToolStripMenuItem.Checked = true;
					originalValueToolStripMenuItem.Checked = false;
					sinceLastFrameToolStripMenuItem.Checked = false;
					break;
				case 1: //Original value (since Start new search)
					sinceLastSearchToolStripMenuItem.Checked = false;
					originalValueToolStripMenuItem.Checked = true;
					sinceLastFrameToolStripMenuItem.Checked = false;
					break;
				case 2: //Since last Frame
					sinceLastSearchToolStripMenuItem.Checked = false;
					originalValueToolStripMenuItem.Checked = false;
					sinceLastFrameToolStripMenuItem.Checked = true;
					break;
				default://Default to last search
					sinceLastSearchToolStripMenuItem.Checked = true;
					originalValueToolStripMenuItem.Checked = false;
					sinceLastFrameToolStripMenuItem.Checked = false;
					break;
			}
		}

		private void LessThanRadio_CheckedChanged(object sender, EventArgs e)
		{
			if (!DifferentByRadio.Checked) DoPreview();
		}

		private void previewModeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Global.Config.RamSearchPreviewMode ^= true;
		}

		private void SpecificValueBox_TextChanged(object sender, EventArgs e)
		{
			DoPreview();
		}

		private void SpecificValueBox_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == '\b') return;

			switch (GetDataType())
			{
				case asigned.UNSIGNED:
					if (!InputValidate.IsValidUnsignedNumber(e.KeyChar))
						e.Handled = true;
					break;
				case asigned.SIGNED:
					if (!InputValidate.IsValidSignedNumber(e.KeyChar))
						e.Handled = true;
					break;
				case asigned.HEX:
					if (!InputValidate.IsValidHexNumber(e.KeyChar))
						e.Handled = true;
					break;
			}
		}

		private void SpecificAddressBox_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == '\b') return;

			if (!InputValidate.IsValidHexNumber(e.KeyChar))
				e.Handled = true;
		}

		private void NumberOfChangesBox_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == '\b') return;

			if (!InputValidate.IsValidUnsignedNumber(e.KeyChar))
				e.Handled = true;
		}

		private void DifferentByBox_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == '\b') return;

			if (!InputValidate.IsValidUnsignedNumber(e.KeyChar))
				e.Handled = true;
		}

		private void SpecificAddressBox_TextChanged(object sender, EventArgs e)
		{
			DoPreview();
		}

		private void NumberOfChangesBox_TextChanged(object sender, EventArgs e)
		{
			DoPreview();
		}

		private void DifferentByBox_TextChanged(object sender, EventArgs e)
		{
			DoPreview();
		}

		private void TruncateFromFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			TruncateFromFile();
		}

		private void DoTruncate(List<Watch> temp)
		{
			weededList.Clear();
			bool found = false;
			for (int x = 0; x < searchList.Count; x++)
			{
				found = false;
				for (int y = 0; y < temp.Count; y++)
				{
					if (searchList[x].address == temp[y].address)
					{
						found = true;
						break;
					}

				}
				if (!found)
					weededList.Add(searchList[x]);
			}
			SaveUndo();
			OutputLabel.Text = MakeAddressString(searchList.Count - weededList.Count) + " removed";
			ReplaceSearchListWithWeedOutList();
			if (Global.Config.RamSearchPreviousAs != 1) MakePreviousList(); //1 = Original value
			DisplaySearchList();
		}

		private void TruncateFromFile()
		{
			//TODO: what about byte size? Think about the implications of this
			var file = GetFileFromUser();
			if (file != null)
			{
				List<Watch> temp = new List<Watch>();
				LoadSearchFile(file.FullName, false, true, temp);
				DoTruncate(temp);
			}
		}

		/// <summary>
		/// Removes Ram Watch list from the search list
		/// </summary>
		private void ExludeRamWatchList()
		{
			DoTruncate(Global.MainForm.RamWatch1.GetRamWatchList());
		}

		private void TruncateFromFiletoolStripButton2_Click(object sender, EventArgs e)
		{
			TruncateFromFile();
		}

		private void exludeRamWatchListToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ExludeRamWatchList();
		}

		private void ExcludeRamWatchtoolStripButton2_Click(object sender, EventArgs e)
		{
			ExludeRamWatchList();
		}

		private void alwaysExludeRamSearchListToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Global.Config.AlwaysExludeRamWatch ^= true;
		}

		private void CopyValueToPrev()
		{
			for (int x = 0; x < searchList.Count; x++)
			{
				prevList[x].value = searchList[x].value;
				searchList[x].prev = searchList[x].value;
			}
			DisplaySearchList();
			DoPreview();
		}

		private void SetCurrToPrevtoolStripButton2_Click(object sender, EventArgs e)
		{
			CopyValueToPrev();
		}

		private void copyValueToPrevToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CopyValueToPrev();
		}

		private void startNewSearchToolStripMenuItem_Click(object sender, EventArgs e)
		{
			StartNewSearch();
		}

		private void searchToolStripMenuItem2_Click(object sender, EventArgs e)
		{
			DoSearch();
		}

		private int GetNumDigits(Int32 i)
		{
			//if (i == 0) return 0;
			//if (i < 0x10) return 1;
			//if (i < 0x100) return 2;
			//if (i < 0x1000) return 3; //adelikat: commenting these out because I decided that regardless of domain, 4 digits should be the minimum
			if (i < 0x10000) return 4;
			if (i < 0x100000) return 5;
			if (i < 0x1000000) return 6;
			if (i < 0x10000000) return 7;
			else return 8;
		}

		private void FreezeAddressToolStrip_Click(object sender, EventArgs e)
		{
			FreezeAddress();
		}

		private void UnfreezeAddress()
		{
			ListView.SelectedIndexCollection indexes = SearchListView.SelectedIndices;
			if (indexes.Count > 0)
			{
				for (int i = 0; i < indexes.Count; i++)
				{
					switch (searchList[indexes[i]].type)
					{
						case atype.BYTE:
							Global.CheatList.Remove(Domain, searchList[indexes[i]].address);
							break;
						case atype.WORD:
							Global.CheatList.Remove(Domain, searchList[indexes[i]].address);
							Global.CheatList.Remove(Domain, searchList[indexes[i]].address + 1);
							break;
						case atype.DWORD:
							Global.CheatList.Remove(Domain, searchList[indexes[i]].address);
							Global.CheatList.Remove(Domain, searchList[indexes[i]].address + 1);
							Global.CheatList.Remove(Domain, searchList[indexes[i]].address + 2);
							Global.CheatList.Remove(Domain, searchList[indexes[i]].address + 3);
							break;
					}
				}
			}
		}

		private void FreezeAddress()
		{
			ListView.SelectedIndexCollection indexes = SearchListView.SelectedIndices;
			if (indexes.Count > 0)
			{
				for (int i = 0; i < indexes.Count; i++)
				{
					switch (searchList[indexes[i]].type)
					{
						case atype.BYTE:
							Cheat c = new Cheat("", searchList[indexes[i]].address, (byte)searchList[indexes[i]].value,
								true, Domain);
							Global.MainForm.Cheats1.AddCheat(c);
							break;
						case atype.WORD:
							{
								byte low = (byte)(searchList[indexes[i]].value / 256);
								byte high = (byte)(searchList[indexes[i]].value);
								int a1 = searchList[indexes[i]].address;
								int a2 = searchList[indexes[i]].address + 1;
								if (searchList[indexes[i]].bigendian)
								{
									Cheat c1 = new Cheat("", a1, low, true, Domain);
									Cheat c2 = new Cheat("", a2, high, true, Domain);
									Global.MainForm.Cheats1.AddCheat(c1);
									Global.MainForm.Cheats1.AddCheat(c2);
								}
								else
								{
									Cheat c1 = new Cheat("", a1, high, true, Domain);
									Cheat c2 = new Cheat("", a2, low, true, Domain);
									Global.MainForm.Cheats1.AddCheat(c1);
									Global.MainForm.Cheats1.AddCheat(c2);
								}
							}
							break;
						case atype.DWORD:
							{
								byte HIWORDhigh = (byte)(searchList[indexes[i]].value / 0x1000000);
								byte HIWORDlow = (byte)(searchList[indexes[i]].value / 0x10000);
								byte LOWORDhigh = (byte)(searchList[indexes[i]].value / 0x100);
								byte LOWORDlow = (byte)(searchList[indexes[i]].value);
								int a1 = searchList[indexes[i]].address;
								int a2 = searchList[indexes[i]].address + 1;
								int a3 = searchList[indexes[i]].address + 2;
								int a4 = searchList[indexes[i]].address + 3;
								if (searchList[indexes[i]].bigendian)
								{
									Cheat c1 = new Cheat("", a1, HIWORDhigh, true, Domain);
									Cheat c2 = new Cheat("", a2, HIWORDlow, true, Domain);
									Cheat c3 = new Cheat("", a3, LOWORDhigh, true, Domain);
									Cheat c4 = new Cheat("", a4, LOWORDlow, true, Domain);
									Global.MainForm.Cheats1.AddCheat(c1);
									Global.MainForm.Cheats1.AddCheat(c2);
									Global.MainForm.Cheats1.AddCheat(c3);
									Global.MainForm.Cheats1.AddCheat(c4);
								}
								else
								{
									Cheat c1 = new Cheat("", a1, LOWORDlow, true, Domain);
									Cheat c2 = new Cheat("", a2, LOWORDhigh, true, Domain);
									Cheat c3 = new Cheat("", a3, HIWORDlow, true, Domain);
									Cheat c4 = new Cheat("", a4, HIWORDhigh, true, Domain);
									Global.MainForm.Cheats1.AddCheat(c1);
									Global.MainForm.Cheats1.AddCheat(c2);
									Global.MainForm.Cheats1.AddCheat(c3);
									Global.MainForm.Cheats1.AddCheat(c4);
								}
							}
							break;
					}
				}
			}
		}

		private void freezeAddressToolStripMenuItem_Click(object sender, EventArgs e)
		{
			FreezeAddress();
		}

		private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
		{
			ListView.SelectedIndexCollection indexes = SearchListView.SelectedIndices;
			if (indexes.Count == 0)
			{
				contextMenuStrip1.Items[3].Visible = false;
				contextMenuStrip1.Items[4].Visible = false;
				contextMenuStrip1.Items[5].Visible = false;
				contextMenuStrip1.Items[6].Visible = false;
			}
			else
			{
				for (int x = 0; x < contextMenuStrip1.Items.Count; x++)
					contextMenuStrip1.Items[x].Visible = true;

				if (indexes.Count == 1)
				{
					if (Global.CheatList.IsActiveCheat(Domain, searchList[indexes[0]].address))
					{
						contextMenuStrip1.Items[6].Text = "&Unfreeze address";
						contextMenuStrip1.Items[6].Image =
							BizHawk.MultiClient.Properties.Resources.Unfreeze;
					}
					else
					{
						contextMenuStrip1.Items[6].Text = "&Freeze address";
						contextMenuStrip1.Items[6].Image =
							BizHawk.MultiClient.Properties.Resources.Freeze;
					}
				}
			}
		}

		private void removeSelectedToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			RemoveAddresses();
		}

		private void addToRamWatchToolStripMenuItem_Click(object sender, EventArgs e)
		{
			AddToRamWatch();
		}

		private void pokeAddressToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			PokeAddress();
		}

		private void freezeAddressToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			if (sender.ToString().Contains("Unfreeze"))
				UnfreezeAddress();
			else
				FreezeAddress();
		}

		private void CheckDomainMenuItems()
		{
			for (int x = 0; x < domainMenuItems.Count; x++)
			{
				if (Domain.Name == domainMenuItems[x].Text)
					domainMenuItems[x].Checked = true;
				else
					domainMenuItems[x].Checked = false;
			}
		}

		private void memoryDomainsToolStripMenuItem_DropDownOpened(object sender, EventArgs e)
		{
			CheckDomainMenuItems();
		}

		private void SearchListView_ColumnReordered(object sender, ColumnReorderedEventArgs e)
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

			if (Global.Config.RamSearchAddressIndex >= lowIndex && Global.Config.RamSearchAddressIndex <= highIndex)
				Global.Config.RamSearchAddressIndex += changeIndex;
			if (Global.Config.RamSearchValueIndex >= lowIndex && Global.Config.RamSearchValueIndex <= highIndex)
				Global.Config.RamSearchValueIndex += changeIndex;
			if (Global.Config.RamSearchPrevIndex >= lowIndex && Global.Config.RamSearchPrevIndex <= highIndex)
				Global.Config.RamSearchPrevIndex += changeIndex;
			if (Global.Config.RamSearchChangesIndex >= lowIndex && Global.Config.RamSearchChangesIndex <= highIndex)
				Global.Config.RamSearchChangesIndex += changeIndex;

			if (header.Text == "Address")
				Global.Config.RamSearchAddressIndex = e.NewDisplayIndex;
			else if (header.Text == "Value")
				Global.Config.RamSearchValueIndex = e.NewDisplayIndex;
			else if (header.Text == "Prev")
				Global.Config.RamSearchPrevIndex = e.NewDisplayIndex;
			else if (header.Text == "Changes")
				Global.Config.RamSearchChangesIndex = e.NewDisplayIndex;
		}

		private void ColumnPositionSet()
		{
			List<ColumnHeader> columnHeaders = new List<ColumnHeader>();
			int i = 0;
			for (i = 0; i < SearchListView.Columns.Count; i++)
				columnHeaders.Add(SearchListView.Columns[i]);

			SearchListView.Columns.Clear();

			i = 0;
			do
			{
				string column = "";
				if (Global.Config.RamSearchAddressIndex == i)
					column = "Address";
				else if (Global.Config.RamSearchValueIndex == i)
					column = "Value";
				else if (Global.Config.RamSearchPrevIndex == i)
					column = "Prev";
				else if (Global.Config.RamSearchChangesIndex == i)
					column = "Changes";

				for (int k = 0; k < columnHeaders.Count(); k++)
				{
					if (columnHeaders[k].Text == column)
					{
						SearchListView.Columns.Add(columnHeaders[k]);
						columnHeaders.Remove(columnHeaders[k]);
						break;
					}
				}
				i++;
			} while (columnHeaders.Count() > 0);
		}

		private void RamSearch_DragEnter(object sender, DragEventArgs e)
		{
			e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None; string[] filePaths = (string[])e.Data.GetData(DataFormats.FileDrop);
		}

		private void RamSearch_DragDrop(object sender, DragEventArgs e)
		{
			string[] filePaths = (string[])e.Data.GetData(DataFormats.FileDrop);
			if (Path.GetExtension(filePaths[0]) == (".wch"))
			{
				LoadSearchFile(filePaths[0], false, false, searchList);
				DisplaySearchList();
			}
		}

		private void OrderColumn(int columnToOrder)
		{
			string columnName = SearchListView.Columns[columnToOrder].Text;
			if (sortedCol.CompareTo(columnName) != 0)
				sortReverse = false;
			searchList.Sort((x, y) => x.CompareTo(y, columnName) * (sortReverse ? -1 : 1));
			sortedCol = columnName;
			sortReverse = !(sortReverse);
			SearchListView.Refresh();
		}

		private void SearchListView_ColumnClick(object sender, ColumnClickEventArgs e)
		{
			OrderColumn(e.Column);
		}

		private void toolStripButton1_MouseDown(object sender, MouseEventArgs e)
		{
			DoSearch();
		}

		private void RedotoolStripButton2_Click(object sender, EventArgs e)
		{
			DoRedo();
		}

		private void WatchtoolStripButton1_Click_1(object sender, EventArgs e)
		{
			AddToRamWatch();
		}

		private void SearchListView_Enter(object sender, EventArgs e)
		{
			SearchListView.Refresh();
		}

		private void RamSearch_Activated(object sender, EventArgs e)
		{
			SearchListView.Refresh();
		}

		private void SearchListView_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Delete && !e.Control && !e.Alt && !e.Shift)
			{
				RemoveAddresses();
			}
			else if (e.KeyCode == Keys.A && e.Control && !e.Alt && !e.Shift) //Select All
			{
				for (int x = 0; x < searchList.Count; x++)
				{
					SearchListView.SelectItem(x, true);
				}
			}
		}
	}
}
