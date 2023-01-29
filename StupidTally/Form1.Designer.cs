using Shortcut;
using System.ComponentModel;
using System.Reflection;

namespace StupidTally
{

	public class Setting {
		public string KeyName { get; set; }
		public string SectionName { get; set; }
		public string Value { get; set; }
		public Setting(string keyName, string value) {
			this.KeyName = keyName;
			this.Value = value;
		}
		public Setting(string keyName, string sectionName, string value) {
			this.KeyName = keyName;
			this.SectionName = sectionName;
			this.Value = value;
		}

	}
	public class Settings 
	{
	
		public const string KeyBindings = "KeyBindings";
		public const string TypeDigitModifier = "TypeDigitModifier";
		public const string ExportToFile = "ExportToFile";
		public const string AcceptNumber = "AcceptNumber";
		public const string RejectNumber = "RejectNumber";
		public string[] KeyNamesSorted = new string[] { TypeDigitModifier, ExportToFile, AcceptNumber, RejectNumber };
		public Dictionary<string,Setting> Data;
		public Settings() {
			this.Data = new Dictionary<string,Setting>();
		}
		public Settings(Dictionary<string,Setting> data) {
			this.Data = data;
		}
		public void SetValue(string keyName, string value) {
			if (this.Data.ContainsKey(keyName)) {
				this.Data[keyName].Value = value;
			} else {
				this.Data.Add(keyName,new Setting(keyName,value));
			}

		}
		public void SetValue(string sectionName, string keyName, string value) {
			if (this.Data.ContainsKey(keyName)) {
				this.Data[keyName].Value = value;
				this.Data[keyName].SectionName = sectionName;
			} else {
				this.Data.Add(keyName, new Setting(keyName, sectionName, value));
			}

		}
		public string GetValue(string keyName, string sectionName = null) {
			var key = DictionaryKey(keyName, sectionName);
			return this.Data[keyName]?.Value;
		}
		public void WriteTo(IniFile file) {
			foreach (KeyValuePair<string,Setting> pair in this.Data) {
				var setting = pair.Value;
				file.Write(setting.KeyName, setting.Value, setting.SectionName);
			}
		}
		private string DictionaryKey(string keyName) { return $"{keyName}"; }
		private string DictionaryKey(string keyName, string sectionName) { return $"{sectionName}.{keyName}"; }
	}
	partial class Form1
	{
		private readonly HotkeyBinder _hotkeyBinder = new HotkeyBinder();
		private IniFile IniFile = null;
		private Settings Settings = null;
		/// <summary>
		///  Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		///  Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		///  Required method for Designer support - do not modify
		///  the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.dataGrid = new System.Windows.Forms.DataGridView();
			this.Damage = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Tally = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.damageTallyBox = new System.Windows.Forms.GroupBox();
			this.shortcutsBox = new System.Windows.Forms.GroupBox();
			this.shortcutGrid = new System.Windows.Forms.DataGridView();
			this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.recordLabel = new System.Windows.Forms.Label();
			this.recordButton = new System.Windows.Forms.Button();
			this.colorDialog1 = new System.Windows.Forms.ColorDialog();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.damageLabel = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.dataGrid)).BeginInit();
			this.damageTallyBox.SuspendLayout();
			this.shortcutsBox.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.shortcutGrid)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// dataGrid
			// 
			this.dataGrid.AllowUserToAddRows = false;
			this.dataGrid.AllowUserToDeleteRows = false;
			this.dataGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			this.dataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Damage,
            this.Tally});
			this.dataGrid.Dock = System.Windows.Forms.DockStyle.Top;
			this.dataGrid.Enabled = false;
			this.dataGrid.Location = new System.Drawing.Point(3, 19);
			this.dataGrid.Name = "dataGrid";
			this.dataGrid.RowHeadersVisible = false;
			this.dataGrid.RowTemplate.Height = 25;
			this.dataGrid.Size = new System.Drawing.Size(515, 398);
			this.dataGrid.TabIndex = 1;
			this.dataGrid.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGrid_CellContentClick);
			// 
			// Damage
			// 
			this.Damage.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
			this.Damage.HeaderText = "Damage";
			this.Damage.MaxInputLength = 5;
			this.Damage.Name = "Damage";
			this.Damage.ReadOnly = true;
			this.Damage.ToolTipText = "The Damage number you saw";
			this.Damage.Width = 76;
			// 
			// Tally
			// 
			this.Tally.HeaderText = "Tally";
			this.Tally.Name = "Tally";
			// 
			// damageTallyBox
			// 
			this.damageTallyBox.Controls.Add(this.dataGrid);
			this.damageTallyBox.Location = new System.Drawing.Point(267, 12);
			this.damageTallyBox.Name = "damageTallyBox";
			this.damageTallyBox.Size = new System.Drawing.Size(521, 426);
			this.damageTallyBox.TabIndex = 2;
			this.damageTallyBox.TabStop = false;
			this.damageTallyBox.Text = "Damage Tally";
			// 
			// shortcutsBox
			// 
			this.shortcutsBox.Controls.Add(this.shortcutGrid);
			this.shortcutsBox.Controls.Add(this.recordLabel);
			this.shortcutsBox.Controls.Add(this.recordButton);
			this.shortcutsBox.Location = new System.Drawing.Point(12, 156);
			this.shortcutsBox.Name = "shortcutsBox";
			this.shortcutsBox.Size = new System.Drawing.Size(249, 276);
			this.shortcutsBox.TabIndex = 3;
			this.shortcutsBox.TabStop = false;
			this.shortcutsBox.Text = "Global Shortcuts";
			// 
			// shortcutGrid
			// 
			this.shortcutGrid.AllowUserToAddRows = false;
			this.shortcutGrid.AllowUserToDeleteRows = false;
			this.shortcutGrid.AllowUserToResizeRows = false;
			this.shortcutGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			this.shortcutGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.shortcutGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2});
			this.shortcutGrid.Dock = System.Windows.Forms.DockStyle.Top;
			this.shortcutGrid.ImeMode = System.Windows.Forms.ImeMode.Alpha;
			this.shortcutGrid.Location = new System.Drawing.Point(3, 19);
			this.shortcutGrid.Name = "shortcutGrid";
			this.shortcutGrid.ReadOnly = true;
			this.shortcutGrid.RowHeadersVisible = false;
			this.shortcutGrid.RowTemplate.Height = 25;
			this.shortcutGrid.Size = new System.Drawing.Size(243, 207);
			this.shortcutGrid.TabIndex = 3;
			this.shortcutGrid.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.shortcutGrid_CellContentClick);
			// 
			// dataGridViewTextBoxColumn1
			// 
			this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
			this.dataGridViewTextBoxColumn1.ReadOnly = true;
			// 
			// dataGridViewTextBoxColumn2
			// 
			this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
			this.dataGridViewTextBoxColumn2.ReadOnly = true;
			// 
			// recordLabel
			// 
			this.recordLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.recordLabel.Location = new System.Drawing.Point(87, 247);
			this.recordLabel.Name = "recordLabel";
			this.recordLabel.Size = new System.Drawing.Size(156, 23);
			this.recordLabel.TabIndex = 2;
			this.recordLabel.Click += new System.EventHandler(this.label1_Click);
			// 
			// recordButton
			// 
			this.recordButton.Location = new System.Drawing.Point(6, 247);
			this.recordButton.Name = "recordButton";
			this.recordButton.Size = new System.Drawing.Size(75, 23);
			this.recordButton.TabIndex = 1;
			this.recordButton.Text = "Record";
			this.recordButton.UseVisualStyleBackColor = true;
			this.recordButton.Click += new System.EventHandler(this.recordButton_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.damageLabel);
			this.groupBox1.Location = new System.Drawing.Point(12, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(249, 122);
			this.groupBox1.TabIndex = 4;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Damage Number";
			// 
			// damageLabel
			// 
			this.damageLabel.Font = new System.Drawing.Font("Segoe UI", 28F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
			this.damageLabel.ForeColor = System.Drawing.Color.Crimson;
			this.damageLabel.Location = new System.Drawing.Point(6, 19);
			this.damageLabel.Name = "damageLabel";
			this.damageLabel.Size = new System.Drawing.Size(243, 100);
			this.damageLabel.TabIndex = 4;
			this.damageLabel.Text = "308";
			this.damageLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.shortcutsBox);
			this.Controls.Add(this.damageTallyBox);
			this.KeyPreview = true;
			this.Name = "Form1";
			this.Text = "Stupid Tally";
			((System.ComponentModel.ISupportInitialize)(this.dataGrid)).EndInit();
			this.damageTallyBox.ResumeLayout(false);
			this.shortcutsBox.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.shortcutGrid)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		#region Helper Methods
		private void LoadSettings() {
			// Creates or loads an INI file in the same directory as your executable
			// named EXE.ini (where EXE is the name of your executable)
			this.IniFile = new IniFile(Application.StartupPath + Assembly.GetExecutingAssembly().GetName().Name + ".ini");
			var readSettings = new Settings();

			// KeyBindings section of settings ini

			// #1) Type Digit Modifier
			ReadOrSetValue(readSettings, Settings.KeyBindings, Settings.TypeDigitModifier, new Keys[] { Keys.Alt | Keys.Shift } /* ALT+SHIFT (+digit) */);

			// #2) Export to File
			ReadOrSetValue(readSettings, Settings.KeyBindings, Settings.ExportToFile, new Keys[] { Keys.Control | Keys.Shift, Keys.S } /* CTRL+SHIFT+S*/);

			// #3) Accept inputted number
			ReadOrSetValue(readSettings, Settings.KeyBindings, Settings.AcceptNumber, new Keys[] {Keys.Alt | Keys.Shift, Keys.End } /* Alt+Shift+End */ );

			// #4)  Reject inputted number
			ReadOrSetValue(readSettings, Settings.KeyBindings, Settings.RejectNumber, new Keys[] {Keys.Alt | Keys.Shift, Keys.Delete } /* Alt+Shift+Delete */);

			// Setup Shortcut Grid in form

			this.shortcutGrid.ColumnCount = 2;
			this.shortcutGrid.Columns[0].Name = "Name";
			this.shortcutGrid.Columns[1].Name = "Shortcut";
			foreach (KeyValuePair<string, Setting> pair in readSettings.Data) {
				var keyName = pair.Key;
				var setting = pair.Value;
				if (!string.IsNullOrEmpty(setting.SectionName) && setting.SectionName.Equals(Settings.KeyBindings)) {
					this.shortcutGrid.Rows.Add(new string[] { setting.KeyName, setting.Value });
				}
				this.IniFile.Write(setting.KeyName,setting.Value, setting.SectionName);
			}
			this.KeyPress += RecordButton_KeyPressed;
			this.KeyDown += RecordButton_KeyDown;
			this.ClearDamage();
			this.Settings = readSettings;
			SortGrid();
		}

		private void SortGrid() {
			dataGrid.Sort(dataGrid.Columns[0], ListSortDirection.Ascending);
		}

		private void RecordButton_KeyDown(object sender, KeyEventArgs e) {
			if (!this._recording) return;
			e.Handled = true;
			if (e.KeyValue >= 0) {
				if (e.KeyData == Keys.Escape || e.KeyData == Keys.Enter) {
					this._recording = false;
					this.recordLabel.ForeColor = DefaultForeColor;
					this.recordButton.ForeColor = DefaultForeColor;
					this.SaveTempKeysToSelectedShortcut();
					RebindHotkeysAfterRecording();
					return;
				}
				if ((e.KeyData & ~ModifierKeys & ~Keys.Menu) == Keys.None) {
					// only modifier key pressed
					_tempKeys.Clear();
					_tempKeys.Add(ModifierKeys);
					this.recordLabel.Text = KeysToFriendlyString(_tempKeys.ToArray());
				}
				else {
					this.recordLabel.ForeColor = Color.Crimson;
					if (!_tempKeys.Contains((Keys)e.KeyValue)) {

						_tempKeys = new List<Keys>() {ModifierKeys,(Keys)e.KeyValue };
					}
					this.recordLabel.Text = KeysToFriendlyString(_tempKeys.ToArray());
				}
			}
		}

		private void RecordButton_KeyPressed(object sender, KeyPressEventArgs e) {
		/*	if (!this._recording) return;
			int keyCode = -1;
			try {
				keyCode = int.Parse(e.KeyChar.ToString());
			}
			catch {

			}
			if (keyCode != -1) {
				_tempKeys.Add((Keys)keyCode);
				this.recordLabel.Text = KeysToFriendlyString(_tempKeys.ToArray());
			}
		*/
		}

		private string KeysToFriendlyString(Keys[] keys) {
			return string.Join("+",keys.Select(x => x.ToString()).ToArray());
		}
		private void ReadOrSetValue(Settings readSettings, string sectionName, string keyName, Keys[] keys) {
			if (this.IniFile.KeyExists(keyName, sectionName)) {
				//read from ini file
				readSettings.SetValue(sectionName, keyName, this.IniFile.Read(keyName, sectionName));
			} else {
				// default keybinding
				readSettings.SetValue(sectionName, keyName, string.Join("+",keys));
			}
			
		}

		private void UnmatchedDamage(string damageNumber) {
			this.damageLabel.Text = damageNumber;
			this.damageLabel.ForeColor = Color.Red;
		}
		private void MatchedDamage(string damageNumber) {
			this.damageLabel.Text = damageNumber;
			this.damageLabel.ForeColor = Color.DarkGreen;
		}
		private void ClearDamage() {
			UnmatchedDamage("000");
		}
		#endregion

		private DataGridView dataGrid;
		private GroupBox damageTallyBox;
		private GroupBox shortcutsBox;
		private Label recordLabel;
		private Button recordButton;
		private ListBox shortcutList;
		private ColorDialog colorDialog1;
		private GroupBox groupBox1;
		private Label damageLabel;
		private DataGridView shortcutGrid;
		private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
		private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
		private DataGridViewTextBoxColumn Damage;
		private DataGridViewTextBoxColumn Tally;
		private int _selectedShortcutIndex;
		private bool _recording;
	}
}