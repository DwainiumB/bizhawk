﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BizHawk.MultiClient.config.ControllerConfig;

namespace BizHawk.MultiClient.config
{
	public partial class NewControllerConfig : Form
	{
		static readonly Dictionary<string, Bitmap> ControllerImages = new Dictionary<string, Bitmap>();
		static NewControllerConfig()
		{
			ControllerImages.Add("NES Controller", Properties.Resources.NES_Controller);
            ControllerImages.Add("SNES Controller", Properties.Resources.SNES_Controller);
            ControllerImages.Add("Nintento 64 Controller", Properties.Resources.N64);
            ControllerImages.Add("Gameboy Controller", Properties.Resources.GBController);
            ControllerImages.Add("GBA Controller", Properties.Resources.GBA_Controller);
            ControllerImages.Add("Dual Gameboy Controller", Properties.Resources.GBController);

            ControllerImages.Add("SMS Controller", Properties.Resources.SMSController);
            ControllerImages.Add("Genesis 3-Button Controller", Properties.Resources.GENController);
            ControllerImages.Add("Saturn Controller", Properties.Resources.SaturnController);

            ControllerImages.Add("Intellivision Controller", Properties.Resources.IntVController);
            ControllerImages.Add("ColecoVision Basic Controller", Properties.Resources.colecovisioncontroller);
            ControllerImages.Add("Atari 2600 Basic Controller", Properties.Resources.atari_controller);
            ControllerImages.Add("Atari 7800 ProLine Joystick Controller", Properties.Resources.A78Joystick);

            ControllerImages.Add("PC Engine Controller", Properties.Resources.PCEngineController);
			ControllerImages.Add("Commodore 64 Controller", Properties.Resources.C64Joystick);
            ControllerImages.Add("TI83 Controller", Properties.Resources.TI83_Controller);

            //ControllerImages.Add("PSP Controller", Properties.Resources); //TODO
		}

		const int MAXPLAYERS = 8;
		string ControllerType;

		private NewControllerConfig()
		{
			InitializeComponent();
		}

		delegate Control PanelCreator<T>(Dictionary<string, T> settings, List<string> buttons, Size size);

		Control CreateNormalPanel(Dictionary<string, string> settings, List<string> buttons, Size size)
		{
			var cp = new ControllerConfigPanel {Dock = DockStyle.Fill};
			cp.LoadSettings(settings, checkBoxAutoTab.Checked, buttons, size.Width, size.Height);
			return cp;
		}

		Control CreateAnalogPanel(Dictionary<string, Config.AnalogBind> settings, List<string> buttons, Size size)
		{
			var acp = new AnalogBindPanel(settings, buttons) { Dock = DockStyle.Fill };
			return acp;
		}

		static void LoadToPanel<T>(Control dest, string ControllerName, IEnumerable<string> ControllerButtons, Dictionary<string, Dictionary<string, T>> settingsblock, T defaultvalue, PanelCreator<T> createpanel)
		{
			Dictionary<string, T> settings;
			if (!settingsblock.TryGetValue(ControllerName, out settings))
			{
				settings = new Dictionary<string, T>();
				settingsblock[ControllerName] = settings;
			}
			// check to make sure that the settings object has all of the appropriate boolbuttons
			foreach (string button in ControllerButtons)
			{
				if (!settings.Keys.Contains(button))
					settings[button] = defaultvalue;
			}

			if (settings.Keys.Count == 0)
				return;

			// split the list of all settings into buckets by player number
			List<string>[] buckets = new List<string>[MAXPLAYERS + 1];
			for (int i = 0; i < buckets.Length; i++)
				buckets[i] = new List<string>();

			foreach (string button in settings.Keys)
			{
				int i;
				for (i = 1; i <= MAXPLAYERS; i++)
				{
					if (button.StartsWith("P" + i))
						break;
				}
				if (i > MAXPLAYERS) // couldn't find
					i = 0;
				buckets[i].Add(button);
			}

			if (buckets[0].Count == settings.Keys.Count)
			{
				// everything went into bucket 0, so make no tabs at all
				dest.Controls.Add(createpanel(settings, null, dest.Size));
			}
			else
			{
				// create multiple player tabs
				var tt = new TabControl {Dock = DockStyle.Fill};
				dest.Controls.Add(tt);
				int pageidx = 0;
				for (int i = 1; i <= MAXPLAYERS; i++)
				{
					if (buckets[i].Count > 0)
					{
						tt.TabPages.Add("Player " + i);
						tt.TabPages[pageidx].Controls.Add(createpanel(settings, buckets[i], tt.Size));
						pageidx++;
					}
				}
				if (buckets[0].Count > 0)
				{
					if (Global.Emulator.SystemId == "C64") //This is a kludge, if there starts to be more exceptions to this pattern, we will need a more robust solution
					{
						tt.TabPages.Add("Keyboard");
					}
					else
					{
						tt.TabPages.Add("Console");
					}
					tt.TabPages[pageidx].Controls.Add(createpanel(settings, buckets[0], tt.Size));
					pageidx++;
				}
			}
		}

		private ControllerDefinition the_definition;
		
		public NewControllerConfig(ControllerDefinition def)
			: this()
		{
			the_definition = def;
			ControllerType = def.Name;
			SuspendLayout();
			LoadPanels(Global.Config);

			Text = def.Name + " Configuration";
			checkBoxUDLR.Checked = Global.Config.AllowUD_LR;
			checkBoxAutoTab.Checked = Global.Config.InputConfigAutoTab;

			SetControllerPicture(def.Name);

			if (!MainForm.INTERIM)
				buttonSaveDefaults.Hide();

			ResumeLayout();
		}

		private void LoadPanels(Dictionary<string, Dictionary<string, string>> normal,
			Dictionary<string, Dictionary<string, string>> autofire,
			Dictionary<string, Dictionary<string, Config.AnalogBind>> analog)
		{
			LoadToPanel(tabPage1, the_definition.Name, the_definition.BoolButtons, normal, "", CreateNormalPanel);
			LoadToPanel(tabPage2, the_definition.Name, the_definition.BoolButtons, autofire, "", CreateNormalPanel);
			LoadToPanel(tabPage3, the_definition.Name, the_definition.FloatControls, analog, new Config.AnalogBind("", 1.0f, 0.0f), CreateAnalogPanel);

			if (tabPage3.Controls.Count == 0)
			{
				tabControl1.TabPages.Remove(tabPage3);
			}
		}

		private void LoadPanels(ControlDefaults cd)
		{
			LoadPanels(cd.AllTrollers, cd.AllTrollersAutoFire, cd.AllTrollersAnalog);
		}

		private void LoadPanels(Config c)
		{
			LoadPanels(c.AllTrollers, c.AllTrollersAutoFire, c.AllTrollersAnalog);
		}

		void SetControllerPicture(string ControlName)
		{
			Bitmap bmp;
			if (!ControllerImages.TryGetValue(ControlName, out bmp))
				bmp = Properties.Resources.Help;

			pictureBox1.Image = bmp;
			pictureBox1.Size = bmp.Size;
			tableLayoutPanel1.ColumnStyles[1].Width = bmp.Width;

            //Uberhack
            if (ControlName == "Commodore 64 Controller")
            {
                PictureBox pictureBox2 = new PictureBox();
                pictureBox2.Image = Properties.Resources.C64Keyboard;
                pictureBox2.Size = Properties.Resources.C64Keyboard.Size;
                tableLayoutPanel1.ColumnStyles[1].Width = Properties.Resources.C64Keyboard.Width;
                pictureBox1.Height /= 2;
                pictureBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                pictureBox1.Dock = DockStyle.Top;
                pictureBox2.Location = new Point(pictureBox1.Location.X, pictureBox1.Location.Y + pictureBox1.Size.Height + 10);
                tableLayoutPanel1.Controls.Add(pictureBox2, 1, 0);

                
                pictureBox2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            }
		}

		// lazy methods, but they're not called often and actually
		// tracking all of the ControllerConfigPanels wouldn't be simpler
		static void SetAutoTab(Control c, bool value)
		{
			if (c is ControllerConfigPanel)
				(c as ControllerConfigPanel).SetAutoTab(value);
			else if (c is AnalogBindPanel)
				;// TODO
			else if (c.HasChildren)
				foreach (Control cc in c.Controls)
					SetAutoTab(cc, value);
		}

		void Save()
		{
			ActOnControlCollection<ControllerConfigPanel>(tabPage1, (c) => c.Save(Global.Config.AllTrollers[the_definition.Name]));
			ActOnControlCollection<ControllerConfigPanel>(tabPage2, (c) => c.Save(Global.Config.AllTrollersAutoFire[the_definition.Name]));
			ActOnControlCollection<AnalogBindPanel>(tabPage3, (c) => c.Save(Global.Config.AllTrollersAnalog[the_definition.Name]));
		}
		void SaveToDefaults(ControlDefaults cd)
		{
			ActOnControlCollection<ControllerConfigPanel>(tabPage1, (c) => c.Save(cd.AllTrollers[the_definition.Name]));
			ActOnControlCollection<ControllerConfigPanel>(tabPage2, (c) => c.Save(cd.AllTrollersAutoFire[the_definition.Name]));
			ActOnControlCollection<AnalogBindPanel>(tabPage3, (c) => c.Save(cd.AllTrollersAnalog[the_definition.Name]));
		}

		static void ActOnControlCollection<T>(Control c, Action<T> proc)
			where T : Control
		{
			if (c is T)
				proc(c as T);
			else if (c.HasChildren)
				foreach (Control cc in c.Controls)
					ActOnControlCollection(cc, proc);
		}

		private void checkBoxAutoTab_CheckedChanged(object sender, EventArgs e)
		{
			SetAutoTab(this, checkBoxAutoTab.Checked);
		}

		private void checkBoxUDLR_CheckedChanged(object sender, EventArgs e)
		{
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			Global.Config.AllowUD_LR = checkBoxUDLR.Checked;
			Global.Config.InputConfigAutoTab = checkBoxAutoTab.Checked;

			Save();

			Global.OSD.AddMessage("Controller settings saved");
			DialogResult = DialogResult.OK;
			Close();
		}

		private void buttonCancel_Click(object sender, EventArgs e)
		{
			Global.OSD.AddMessage("Controller config aborted");
			Close();
		}

		private void NewControllerConfig_Load(object sender, EventArgs e)
		{

		}

		private static string ControlDefaultPath
		{
			get { return PathManager.MakeProgramRelativePath("defctrl.json"); }
		}

        private TabControl GetTabControl(System.Windows.Forms.Control.ControlCollection controls)
        {
            if (controls != null)
            {
                foreach (Control c in controls)
                {
                    if (c is TabControl)
                    {
                        return (c as TabControl);
                    }
                }
            }
            return null;
        }

		private void buttonLoadDefaults_Click(object sender, EventArgs e)
		{
            tabControl1.SuspendLayout();

            string wasTabbedMain = tabControl1.SelectedTab.Name;
            TabControl tb1 = GetTabControl(tabPage1.Controls ?? null);
            TabControl tb2 = GetTabControl(tabPage2.Controls ?? null);
            TabControl tb3 = GetTabControl(tabPage3.Controls ?? null);
            int? wasTabbedPage1 = null;
            int? wasTabbedPage2 = null;
            int? wasTabbedPage3 = null;

            if (tb1 != null && tb1.SelectedTab != null) { wasTabbedPage1 = tb1.SelectedIndex; }
            if (tb2 != null && tb2.SelectedTab != null) { wasTabbedPage2 = tb2.SelectedIndex; }
            if (tb3 != null && tb3.SelectedTab != null) { wasTabbedPage2 = tb3.SelectedIndex; }

            tabPage1.Controls.Clear();
            tabPage2.Controls.Clear();
            tabPage3.Controls.Clear();

			// load panels directly from the default config.
			// this means that the changes are NOT committed.  so "Cancel" works right and you
			// still have to hit OK at the end.
			ControlDefaults cd = new ControlDefaults();
			cd = ConfigService.Load(ControlDefaultPath, cd);
			LoadPanels(cd);

            tabControl1.SelectTab(wasTabbedMain);

            if (wasTabbedPage1.HasValue)
            {
                TabControl newTb1 = GetTabControl(tabPage1.Controls ?? null);
                if (newTb1 != null)
                {
                    newTb1.SelectTab(wasTabbedPage1.Value);
                }
            }

            if (wasTabbedPage2.HasValue)
            {
                TabControl newTb2 = GetTabControl(tabPage2.Controls ?? null);
                if (newTb2 != null)
                {
                    newTb2.SelectTab(wasTabbedPage2.Value);
                }
            }

            if (wasTabbedPage3.HasValue)
            {
                TabControl newTb3 = GetTabControl(tabPage3.Controls ?? null);
                if (newTb3 != null)
                {
                    newTb3.SelectTab(wasTabbedPage3.Value);
                }
            }

            tabControl1.ResumeLayout();
		}

		class ControlDefaults
		{
			public Dictionary<string, Dictionary<string, string>> AllTrollers = new Dictionary<string, Dictionary<string, string>>();
			public Dictionary<string, Dictionary<string, string>> AllTrollersAutoFire = new Dictionary<string, Dictionary<string, string>>();
			public Dictionary<string, Dictionary<string, Config.AnalogBind>> AllTrollersAnalog = new Dictionary<string, Dictionary<string, Config.AnalogBind>>();
		}

		public static void ConfigCheckAllControlDefaults(Config c)
		{
			if (c.AllTrollers.Count == 0 && c.AllTrollersAutoFire.Count == 0 && c.AllTrollersAnalog.Count == 0)
			{
				ControlDefaults cd = new ControlDefaults();
				cd = ConfigService.Load(ControlDefaultPath, cd);
				c.AllTrollers = cd.AllTrollers;
				c.AllTrollersAutoFire = cd.AllTrollersAutoFire;
				c.AllTrollersAnalog = cd.AllTrollersAnalog;
			}
		}

		private void buttonSaveDefaults_Click(object sender, EventArgs e)
		{
			var result = MessageBox.Show(this, "OK to overwrite defaults for current control scheme?", "Save Defaults", MessageBoxButtons.YesNo);
			if (result == DialogResult.Yes)
			{
				ControlDefaults cd = new ControlDefaults();
				cd = ConfigService.Load(ControlDefaultPath, cd);
				cd.AllTrollers[the_definition.Name] = new Dictionary<string, string>();
				cd.AllTrollersAutoFire[the_definition.Name] = new Dictionary<string, string>();
				cd.AllTrollersAnalog[the_definition.Name] = new Dictionary<string, Config.AnalogBind>();

				SaveToDefaults(cd);

				ConfigService.Save(ControlDefaultPath, cd);
			}
		}
	}
}
