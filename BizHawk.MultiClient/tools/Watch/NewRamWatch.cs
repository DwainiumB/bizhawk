﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace BizHawk.MultiClient
{
	public partial class NewRamWatch : Form
	{
		private const string ADDRESS = "AddressColumn";
		private const string VALUE = "ValueColumn";
		private const string PREV = "PrevColumn";
		private const string CHANGES = "ChangesColumn";
		private const string DIFF = "DiffColumn";
		private const string DOMAIN = "DomainColumn";
		private const string NOTES = "NotesColumn";

		private Dictionary<string, int> DefaultColumnWidths = new Dictionary<string, int>()
		{
			{ ADDRESS, 60 },
			{ VALUE, 59 },
			{ PREV, 59 },
			{ CHANGES, 55 },
			{ DIFF, 59 },
			{ DOMAIN, 55 },
			{ NOTES, 128 },
		};

		private int defaultWidth;
		private int defaultHeight;
		private WatchList Watches = new WatchList();
		private string systemID = "NULL";
		private string sortedCol = "";
		private bool sortReverse = false;

		public NewRamWatch()
		{
			InitializeComponent();
			WatchListView.QueryItemText += WatchListView_QueryItemText;
			WatchListView.QueryItemBkColor += WatchListView_QueryItemBkColor;
			WatchListView.VirtualMode = true;
			Closing += (o, e) => SaveConfigSettings();
			sortedCol = "";
			sortReverse = false;
		}

		public void UpdateValues()
		{
			if ((!IsHandleCreated || IsDisposed) && !Global.Config.DisplayRamWatch)
			{
				return;
			}
			Watches.UpdateValues();

			if (Global.Config.DisplayRamWatch)
			{
				for (int x = 0; x < Watches.Count; x++)
				{
					bool alert = Watches[x].IsSeparator ? false : Global.CheatList.IsActiveCheat(Watches[x].Domain, Watches[x].Address.Value);
					Global.OSD.AddGUIText(
						Watches[x].ToString(),
						Global.Config.DispRamWatchx, 
						(Global.Config.DispRamWatchy + (x * 14)), 
						alert, 
						Color.Black, 
						Color.White,
						0
					);
				}
			}

			if (!IsHandleCreated || IsDisposed) return;

			WatchListView.BlazingFast = true;
			WatchListView.Refresh();
			WatchListView.BlazingFast = false;
		}

		public void Restart()
		{
			if ((!IsHandleCreated || IsDisposed) && !Global.Config.DisplayRamWatch)
			{
				return;
			}

			if (!String.IsNullOrWhiteSpace(Watches.CurrentFileName))
			{
				Watches.Reload();
			}
			else
			{
				NewWatchList(true);
			}
		}

		public bool AskSave()
		{
			if (Global.Config.SupressAskSave) //User has elected to not be nagged
			{
				return true;
			}

			if (Watches.Changes)
			{
				Global.Sound.StopSound();
				DialogResult result = MessageBox.Show("Save Changes?", "Ram Watch", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button3);
				Global.Sound.StartSound();
				if (result == DialogResult.Yes)
				{
					Watches.Save();
				}
				else if (result == DialogResult.No)
				{
					return true;
				}
				else if (result == DialogResult.Cancel)
				{
					return false;
				}
			}

			return true;
		}

		public void SaveConfigSettings()
		{
			if (WatchListView.Columns[ADDRESS] != null)
			{
				Global.Config.RamWatchAddressIndex = WatchListView.Columns[ADDRESS].DisplayIndex;
				Global.Config.RamWatchColumnWidths[ADDRESS] = WatchListView.Columns[ADDRESS].Width;
			}

			if (WatchListView.Columns[VALUE] != null)
			{
				Global.Config.RamWatchValueIndex = WatchListView.Columns[VALUE].DisplayIndex;
				Global.Config.RamWatchColumnWidths[VALUE] = WatchListView.Columns[VALUE].Width;
			}

			if (WatchListView.Columns[PREV] != null)
			{
				Global.Config.RamWatchPrevIndex = WatchListView.Columns[PREV].DisplayIndex;
				Global.Config.RamWatchColumnWidths[PREV] = WatchListView.Columns[PREV].Width;
			}

			if (WatchListView.Columns[CHANGES] != null)
			{
				Global.Config.RamWatchChangeIndex = WatchListView.Columns[CHANGES].DisplayIndex;
				Global.Config.RamWatchColumnWidths[CHANGES] = WatchListView.Columns[CHANGES].Width;
			}

			if (WatchListView.Columns[DIFF] != null)
			{
				Global.Config.RamWatchDiffIndex = WatchListView.Columns[DIFF].DisplayIndex;
				Global.Config.RamWatchColumnWidths[DIFF] = WatchListView.Columns[DIFF].Width;
			}

			if (WatchListView.Columns[DOMAIN] != null)
			{
				Global.Config.RamWatchDomainIndex = WatchListView.Columns[DOMAIN].DisplayIndex;
				Global.Config.RamWatchColumnWidths[DOMAIN] = WatchListView.Columns[DOMAIN].Width;
			}

			if (WatchListView.Columns[NOTES] != null)
			{
				Global.Config.RamWatchNotesIndex = WatchListView.Columns[NOTES].Index;
				Global.Config.RamWatchColumnWidths[NOTES] = WatchListView.Columns[NOTES].Width;
			}

			Global.Config.RamWatchWndx = Location.X;
			Global.Config.RamWatchWndy = Location.Y;
			Global.Config.RamWatchWidth = Right - Left;
			Global.Config.RamWatchHeight = Bottom - Top;
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			if (!AskSave())
				e.Cancel = true;
			base.OnClosing(e);
		}

		private int GetColumnWidth(string columnName)
		{
			var width = Global.Config.RamWatchColumnWidths[columnName];
			if (width == -1)
			{
				width = DefaultColumnWidths[columnName];
			}

			return width;
		}

		private void WatchListView_QueryItemBkColor(int index, int column, ref Color color)
		{
			if (index >= Watches.ItemCount)
			{
				return;
			}

			if (column == 0)
			{
				if (Watches[index].IsSeparator)
				{
					color = BackColor;
				}
				else if (Global.CheatList.IsActiveCheat(Watches.Domain, Watches[index].Address.Value))
				{
					color = Color.LightCyan;
				}
			}
		}

		private void WatchListView_QueryItemText(int index, int column, out string text)
		{
			text = "";

			if (index >= Watches.ItemCount || Watches[index].IsSeparator)
			{
				return;
			}
			string columnName = WatchListView.Columns[column].Name;

			switch (columnName)
			{
				case ADDRESS:
					text = Watches[index].AddressString;
					break;
				case VALUE:
					text = Watches[index].ValueString;
					break;
				case PREV:
					if (Watches[index] is iWatchEntryDetails)
					{
						text = (Watches[index] as iWatchEntryDetails).PreviousStr;
					}
					break;
				case CHANGES:
					if (Watches[index] is iWatchEntryDetails)
					{
						text = (Watches[index] as iWatchEntryDetails).ChangeCount.ToString();
					}
					break;
				case DIFF:
					if (Watches[index] is iWatchEntryDetails)
					{
						text = (Watches[index] as iWatchEntryDetails).Diff;
					}
					break;
				case DOMAIN:
					text = Watches[index].Domain.Name;
					break;
				case NOTES:
					if (Watches[index] is iWatchEntryDetails)
					{
						text = (Watches[index] as iWatchEntryDetails).Notes;
					}
					break;
			}
		}

		private void DisplayWatches()
		{
			WatchListView.ItemCount = Watches.ItemCount;
		}

		private void UpdateWatchCount()
		{
			WatchCountLabel.Text = Watches.WatchCount.ToString() + (Watches.WatchCount == 1 ? " watch" : " watches");
		}

		private void SetPlatformAndMemoryDomainLabel()
		{
			MemDomainLabel.Text = Global.Emulator.SystemId + " " + Watches.Domain.Name;
		}

		private void NewWatchList(bool suppressAsk)
		{
			bool result = true;
			if (Watches.Changes)
			{
				result = AskSave();
			}

			if (result || suppressAsk)
			{
				Watches.Clear();
				DisplayWatches();
				UpdateWatchCount();
				MessageLabel.Text = "";
				sortReverse = false;
				sortedCol = "";
			}
		}

		public void LoadWatchFromRecent(string file)
		{
			bool ask_result = true;
			if (Watches.Changes)
			{
				ask_result = AskSave();
			}

			if (ask_result)
			{
				bool load_result = Watches.Load(file, details: true, append: false);
				if (load_result)
				{
					Global.Config.RecentWatches.Add(file);
					
				}
				else
				{
					DialogResult result = MessageBox.Show("Could not open " + file + "\nRemove from list?", "File not found", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
					if (result == DialogResult.Yes)
					{
						Global.Config.RecentWatches.Remove(file);
					}
				}

				DisplayWatches();
				UpdateWatchCount();
				MessageLabel.Text = Path.GetFileName(Watches.CurrentFileName) + " *";
				Watches.Changes = false;
				
			}
		}

		private void SetMemoryDomain(int pos)
		{
			if (pos < Global.Emulator.MemoryDomains.Count)  //Sanity check
			{
				Watches.Domain = Global.Emulator.MemoryDomains[pos];
			}

			SetPlatformAndMemoryDomainLabel();
			Update();
		}

		private void SelectAll()
		{
			for (int i = 0; i < Watches.Count; i++)
				WatchListView.SelectItem(i, true);
		}

		private void Changes()
		{
			Watches.Changes = true;
			MessageLabel.Text = Path.GetFileName(Watches.CurrentFileName) + " *";
		}

		private void MoveUp()
		{
			ListView.SelectedIndexCollection indexes = WatchListView.SelectedIndices;
			if (indexes.Count == 0 || indexes[0] == 0)
			{
				return;
			}

			foreach (int index in indexes)
			{
				var watch = Watches[index];
				Watches.Remove(Watches[index]);
				Watches.Insert(index - 1, watch);

				//Note: here it will get flagged many times redundantly potentially, 
				//but this avoids it being flagged falsely when the user did not select an index
				Changes();
			}
			List<int> indices = new List<int>();
			for (int i = 0; i < indexes.Count; i++)
			{
				indices.Add(indexes[i] - 1);
			}

			WatchListView.SelectedIndices.Clear();
			foreach (int t in indices)
			{
				WatchListView.SelectItem(t, true);
			}

			DisplayWatches();
		}

		private void MoveDown()
		{
			ListView.SelectedIndexCollection indexes = WatchListView.SelectedIndices;
			if (indexes.Count == 0)
			{
				return;
			}

			foreach (int index in indexes)
			{
				var watch = Watches[index];

				if (index < Watches.Count - 1)
				{
					Watches.Remove(Watches[index]);
					Watches.Insert(index + 1, watch);
				}

				//Note: here it will get flagged many times redundantly potnetially, 
				//but this avoids it being flagged falsely when the user did not select an index
				Changes();
			}

			List<int> indices = new List<int>();
			for (int i = 0; i < indexes.Count; i++)
			{
				indices.Add(indexes[i] + 1);
			}

			WatchListView.SelectedIndices.Clear();
			foreach (int t in indices)
			{
				WatchListView.SelectItem(t, true);
			}

			DisplayWatches();
		}

		private void InsertSeparator()
		{
			Changes();

			ListView.SelectedIndexCollection indexes = WatchListView.SelectedIndices;
			if (indexes.Count > 0)
			{
				if (indexes[0] > 0)
				{
					Watches.Insert(indexes[0], SeparatorWatch.Instance);
				}
			}
			else
			{
				Watches.Add(SeparatorWatch.Instance);
			}
			DisplayWatches();
		}

		private Point GetPromptPoint()
		{
			return PointToScreen(new Point(WatchListView.Location.X, WatchListView.Location.Y));
		}

		private void AddNewWatch()
		{
			WatchEditor we = new WatchEditor()
			{
				InitialLocation = GetPromptPoint()
			};
			we.SetWatch(Watches.Domain);
			Global.Sound.StopSound();
			we.ShowDialog();
			Global.Sound.StartSound();

			if (we.DialogResult == DialogResult.OK)
			{
				Watches.Add(we.Watches[0]);
				Changes();
				UpdateWatchCount();
				DisplayWatches();
			}
		}

		private void EditWatch(bool duplicate = false)
		{
			ListView.SelectedIndexCollection indexes = WatchListView.SelectedIndices;

			if (indexes.Count > 0)
			{
				WatchEditor we = new WatchEditor()
				{
					InitialLocation = GetPromptPoint(),
				};

				List<Watch> watches = new List<Watch>();
				foreach (int index in indexes)
				{
					if (!Watches[index].IsSeparator)
					{
						watches.Add(Watches[index]);
					}
				}

				if (!watches.Any())
				{
					return;
				}

				we.SetWatch(Watches.Domain, watches, duplicate ? WatchEditor.Mode.Duplicate : WatchEditor.Mode.Edit);
				Global.Sound.StopSound();
				var result = we.ShowDialog();
				if (result == DialogResult.OK)
				{
					Changes();
					if (duplicate)
					{
						Watches.AddRange(we.Watches);
						DisplayWatches();
					}
					else
					{
						for (int i = 0; i < we.Watches.Count; i++)
						{
							Watches[indexes[i]] = we.Watches[i];
						}
					}
				}

				Global.Sound.StartSound();
				UpdateValues();
			}
		}

		private void PokeAddress()
		{
			return; //TODO
			//TODO: WatchEditor can do the poking too

			ListView.SelectedIndexCollection indexes = WatchListView.SelectedIndices;
			if (indexes.Count > 0)
			{
				Global.Sound.StopSound();
				RamPoke poke = new RamPoke()
				{
					NewLocation = GetPromptPoint()
				};
				poke.ShowDialog();
				//poke.SetWatchObject(null); //TODO
				UpdateValues();
				Global.Sound.StartSound();
			}
		}

		private List<Watch> SelectedWatches
		{
			get
			{
				List<Watch> selected = new List<Watch>();
				ListView.SelectedIndexCollection indexes = WatchListView.SelectedIndices;
				if (indexes.Count > 0)
				{
					foreach (int index in indexes)
					{
						if (!Watches[index].IsSeparator)
						{
							selected.Add(Watches[index]);
						}
					}
				}
				return selected;
			}
		}

		private void AddRemoveColumn(string columnName, bool enabled)
		{
			if (enabled)
			{
				if (WatchListView.Columns[columnName] == null)
				{
					ColumnHeader column = new ColumnHeader()
					{
						Name = columnName,
						Text = columnName.Replace("Column", ""),
						Width = GetColumnWidth(columnName),
					};

					WatchListView.Columns.Add(column); //TODO: insert in right place
				}
			}
			else
			{
				WatchListView.Columns.RemoveByKey(columnName);
			}

			DisplayWatches();
		}

		private void ColumnPositionSet() //TODO: fix indexing, thrown off by columns not existing
		{
			List<ColumnHeader> columnHeaders = new List<ColumnHeader>();
			foreach(ColumnHeader column in WatchListView.Columns)
			{
				columnHeaders.Add(column);
			}
			WatchListView.Columns.Clear();

			List<KeyValuePair<int, string>> columnSettings = new List<KeyValuePair<int, string>>
				{
					new KeyValuePair<int, string>(Global.Config.RamWatchAddressIndex, ADDRESS),
					new KeyValuePair<int, string>(Global.Config.RamWatchValueIndex, VALUE),
					new KeyValuePair<int, string>(Global.Config.RamWatchPrevIndex, PREV),
					new KeyValuePair<int, string>(Global.Config.RamWatchChangeIndex, CHANGES),
					new KeyValuePair<int, string>(Global.Config.RamWatchDiffIndex, DIFF),
					new KeyValuePair<int, string>(Global.Config.RamWatchDomainIndex, DOMAIN),
					new KeyValuePair<int, string>(Global.Config.RamWatchNotesIndex, NOTES)
				};

			columnSettings = columnSettings.OrderBy(s => s.Key).ToList();


			foreach (KeyValuePair<int, string> t in columnSettings)
			{
				foreach (ColumnHeader t1 in columnHeaders)
				{
					if (t.Value == t1.Name)
					{
						WatchListView.Columns.Add(t1);
					}
				}
			}
		}

		private void LoadConfigSettings()
		{
			//Size and Positioning
			defaultWidth = Size.Width;     //Save these first so that the user can restore to its original size
			defaultHeight = Size.Height;

			if (Global.Config.RamWatchSaveWindowPosition && Global.Config.RamWatchWndx >= 0 && Global.Config.RamWatchWndy >= 0)
			{
				Location = new Point(Global.Config.RamWatchWndx, Global.Config.RamWatchWndy);
			}

			if (Global.Config.RamWatchWidth >= 0 && Global.Config.RamWatchHeight >= 0)
			{
				Size = new Size(Global.Config.RamWatchWidth, Global.Config.RamWatchHeight);
			}

			//Columns
			ColumnPositionSet();
			AddRemoveColumn(CHANGES, Global.Config.RamWatchShowChangeColumn);
			AddRemoveColumn(DIFF, Global.Config.RamWatchShowDiffColumn);
			AddRemoveColumn(DOMAIN, Global.Config.RamWatchShowDomainColumn);
			AddRemoveColumn(PREV, Global.Config.RamWatchShowPrevColumn);
			WatchListView.Columns[ADDRESS].Width = GetColumnWidth(ADDRESS);
			WatchListView.Columns[VALUE].Width = GetColumnWidth(VALUE);
			WatchListView.Columns[NOTES].Width = GetColumnWidth(NOTES);
		}

		#region Winform Events

		private void NewRamWatch_Load(object sender, EventArgs e)
		{
			LoadConfigSettings();
			
		}

		/*************File***********************/
		private void filesToolStripMenuItem_DropDownOpened(object sender, EventArgs e)
		{
			saveToolStripMenuItem.Enabled = Watches.Changes;
		}

		private void newListToolStripMenuItem_Click(object sender, EventArgs e)
		{
			NewWatchList(false);
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			bool append = sender == appendFileToolStripMenuItem;
			var file = WatchList.GetFileFromUser(Watches.CurrentFileName);
			if (file != null)
			{
				bool result = true;
				if (Watches.Changes)
				{
					result = AskSave();
				}

				if (result)
				{
					Watches.Load(file.FullName, true, append);
					DisplayWatches();
					MessageLabel.Text = Path.GetFileNameWithoutExtension(Watches.CurrentFileName) + (Watches.Changes ? " *" : String.Empty);
					UpdateWatchCount();
					Global.Config.RecentWatches.Add(Watches.CurrentFileName);
					SetMemoryDomain(WatchCommon.GetDomainPos(Watches.Domain.ToString()));
				}
			}
		}

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			bool result = Watches.Save();
			if (result)
			{
				MessageLabel.Text = Path.GetFileName(Watches.CurrentFileName) + " saved.";
			}
		}

		private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			bool result = Watches.SaveAs();
			if (result)
			{
				MessageLabel.Text = Path.GetFileName(Watches.CurrentFileName) + " saved.";
				Global.Config.RecentWatches.Add(Watches.CurrentFileName);
			}
		}

		private void recentToolStripMenuItem_DropDownOpened(object sender, EventArgs e)
		{
			recentToolStripMenuItem.DropDownItems.Clear();
			recentToolStripMenuItem.DropDownItems.AddRange(Global.Config.RecentWatches.GenerateRecentMenu(LoadWatchFromRecent));
			recentToolStripMenuItem.DropDownItems.Add(Global.Config.RecentWatches.GenerateAutoLoadItem());
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!AskSave())
			{
				return;
			}
			else
			{
				Close();
			}
		}

		/*************Watches***********************/
		private void watchesToolStripMenuItem_DropDownOpened(object sender, EventArgs e)
		{
			ListView.SelectedIndexCollection indexes = WatchListView.SelectedIndices;
			if (indexes.Count > 0)
			{
				editWatchToolStripMenuItem.Enabled = true;
				duplicateWatchToolStripMenuItem.Enabled = true;
				removeWatchToolStripMenuItem.Enabled = true;
				moveUpToolStripMenuItem.Enabled = true;
				moveDownToolStripMenuItem.Enabled = true;
				pokeAddressToolStripMenuItem.Enabled = true;
				freezeAddressToolStripMenuItem.Enabled = true;
			}
			else
			{
				editWatchToolStripMenuItem.Enabled = false;
				duplicateWatchToolStripMenuItem.Enabled = false;
				removeWatchToolStripMenuItem.Enabled = false;
				moveUpToolStripMenuItem.Enabled = false;
				moveDownToolStripMenuItem.Enabled = false;
				pokeAddressToolStripMenuItem.Enabled = false;
				freezeAddressToolStripMenuItem.Enabled = false;
			}
		}

		private void memoryDomainsToolStripMenuItem_DropDownOpened(object sender, EventArgs e)
		{
			memoryDomainsToolStripMenuItem.DropDownItems.Clear();
			memoryDomainsToolStripMenuItem.DropDownItems.AddRange(ToolHelpers.GenerateMemoryDomainMenuItems(SetMemoryDomain, Watches.Domain.Name));
		}

		private void newWatchToolStripMenuItem_Click(object sender, EventArgs e)
		{
			AddNewWatch();
		}

		private void editWatchToolStripMenuItem_Click(object sender, EventArgs e)
		{
			EditWatch();
		}

		private void removeWatchToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ListView.SelectedIndexCollection indexes = WatchListView.SelectedIndices;
			if (indexes.Count > 0)
			{
				foreach (int index in indexes)
				{
					Watches.Remove(Watches[indexes[0]]); //index[0] used since each iteration will make this the correct list index
				}
				indexes.Clear();
				DisplayWatches();
			}
			UpdateValues();
			UpdateWatchCount();
		}

		private void duplicateWatchToolStripMenuItem_Click(object sender, EventArgs e)
		{
			EditWatch(duplicate:true);
		}

		private void pokeAddressToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PokeAddress();
		}

		private void freezeAddressToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToolHelpers.FreezeAddress(SelectedWatches);
		}

		private void insertSeparatorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			InsertSeparator();
		}

		private void clearChangeCountsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Watches.ClearChangeCounts();
		}

		private void moveUpToolStripMenuItem_Click(object sender, EventArgs e)
		{
			MoveUp();
		}

		private void moveDownToolStripMenuItem_Click(object sender, EventArgs e)
		{
			MoveDown();
		}

		private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SelectAll();
		}

		/*************View***********************/
		private void viewToolStripMenuItem_DropDownOpened(object sender, EventArgs e)
		{
			showPreviousValueToolStripMenuItem.Checked = Global.Config.RamWatchShowPrevColumn;
			showChangeCountsToolStripMenuItem.Checked = Global.Config.RamWatchShowChangeColumn;
			diffToolStripMenuItem.Checked = Global.Config.RamWatchShowDiffColumn;
			domainToolStripMenuItem.Checked = Global.Config.RamWatchShowDomainColumn;
		}

		private void showPreviousValueToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Global.Config.RamWatchShowPrevColumn ^= true;
			AddRemoveColumn(PREV, Global.Config.RamWatchShowPrevColumn);
		}

		private void showChangeCountsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Global.Config.RamWatchShowChangeColumn ^= true;
			AddRemoveColumn(CHANGES, Global.Config.RamWatchShowChangeColumn);
		}

		private void diffToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Global.Config.RamWatchShowDiffColumn ^= true;
			AddRemoveColumn(DIFF, Global.Config.RamWatchShowDiffColumn);
		}

		private void domainToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Global.Config.RamWatchShowDomainColumn ^= true;
			AddRemoveColumn(DOMAIN, Global.Config.RamWatchShowDomainColumn);
		}

		/*************Options***********************/
		private void optionsToolStripMenuItem_DropDownOpened(object sender, EventArgs e)
		{
			displayWatchesOnScreenToolStripMenuItem.Checked = Global.Config.DisplayRamWatch;
			saveWindowPositionToolStripMenuItem.Checked = Global.Config.RamWatchSaveWindowPosition;
		}

		private void definePreviousValueAsToolStripMenuItem_DropDownOpened(object sender, EventArgs e)
		{
			lastChangeToolStripMenuItem.Checked = false;
			previousFrameToolStripMenuItem.Checked = false;
			originalToolStripMenuItem.Checked = false;

			switch (Global.Config.RamWatchDefinePrevious)
			{
				default:
				case Watch.PreviousType.LastFrame:
					previousFrameToolStripMenuItem.Checked = true;
					break;
				case Watch.PreviousType.LastChange:
					lastChangeToolStripMenuItem.Checked = true;
					break;
				case Watch.PreviousType.OriginalValue:
					originalToolStripMenuItem.Checked = true;
					break;
			}
		}

		private void previousFrameToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Global.Config.RamWatchDefinePrevious = Watch.PreviousType.LastFrame;
		}

		private void lastChangeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Global.Config.RamWatchDefinePrevious = Watch.PreviousType.LastChange;
		}

		private void originalToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Global.Config.RamWatchDefinePrevious = Watch.PreviousType.OriginalValue;
		}

		private void displayWatchesOnScreenToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Global.Config.DisplayRamWatch ^= true;

			if (!Global.Config.DisplayRamWatch)
			{
				Global.OSD.ClearGUIText();
			}
			else
			{
				UpdateValues();
			}
		}

		private void saveWindowPositionToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Global.Config.RamWatchSaveWindowPosition ^= true;
		}

		private void restoreWindowSizeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Size = new Size(defaultWidth, defaultHeight);

			Global.Config.RamWatchAddressIndex = 0;
			Global.Config.RamWatchValueIndex = 1;
			Global.Config.RamWatchPrevIndex = 2;
			Global.Config.RamWatchChangeIndex = 3;
			Global.Config.RamWatchDiffIndex = 4;
			Global.Config.RamWatchDomainIndex = 5;
			Global.Config.RamWatchNotesIndex = 6;

			ColumnPositionSet();

			Global.Config.RamWatchShowChangeColumn = true;
			Global.Config.RamWatchShowDomainColumn = true;
			Global.Config.RamWatchShowPrevColumn = false;
			Global.Config.RamWatchShowDiffColumn = false;

			AddRemoveColumn(CHANGES, Global.Config.RamWatchShowChangeColumn);
			AddRemoveColumn(DOMAIN, Global.Config.RamWatchShowDomainColumn);
			AddRemoveColumn(PREV, Global.Config.RamWatchShowPrevColumn);
			AddRemoveColumn(DIFF, Global.Config.RamWatchShowDiffColumn);

			WatchListView.Columns[ADDRESS].Width = DefaultColumnWidths[ADDRESS];
			WatchListView.Columns[VALUE].Width = DefaultColumnWidths[VALUE];
			//WatchListView.Columns[PREV].Width = DefaultColumnWidths[PREV];
			WatchListView.Columns[CHANGES].Width = DefaultColumnWidths[CHANGES];
			//WatchListView.Columns[DIFF].Width = DefaultColumnWidths[DIFF];
			WatchListView.Columns[DOMAIN].Width = DefaultColumnWidths[DOMAIN];
			WatchListView.Columns[NOTES].Width = DefaultColumnWidths[NOTES];
			
			Global.Config.DisplayRamWatch = false;
			Global.Config.RamWatchSaveWindowPosition = true;
		}

		#endregion
	}
}
