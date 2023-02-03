using Shortcut;
using System.Collections;
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
	public class DataGridNumericComparer : IComparer {
		public DataGridNumericComparer() {

		}
		public int Compare(Object x, Object y) {
			DataGridViewRow a = (DataGridViewRow)x;
			DataGridViewRow b = (DataGridViewRow)y;
			var firstString = a.Cells[0].Value.ToString();
			var secondString = b.Cells[0].Value.ToString();
			int firstVal = 0;
			int secondVal = 0;
			try {
				firstVal = int.Parse(firstString);
				secondVal = int.Parse(secondString);
			} catch (Exception ex) {

			}
			return firstVal.CompareTo(secondVal);
		}
	}
	public enum DataActionType {
		TallyUp,
		AddRow,
		DeleteRow,
		ChangeValue,
	}
	public class DataAction {
		public DataActionType Type;
		public string[] KeyValuePair;
		public string PreviousTally;
		public DataAction(DataActionType type, string[] keyValuePair, string previousTally) {
			Type = type;
			KeyValuePair = keyValuePair;
			if (keyValuePair.Length < 2 || keyValuePair.Length > 3) throw new Exception($"Invalid keyValuePair: {keyValuePair}");
			PreviousTally = previousTally;
		}
		public DataAction(DataActionType type, string key, string value, string previousTally) {
			Type = type;
			KeyValuePair = new string[] {key, value};
			PreviousTally = previousTally;
		}
		public override bool Equals(object obj) {
			DataAction b = (DataAction)obj;
			if (b == null) return false;
			if (!b.Type.Equals(this.Type)) return false;
			if (b.KeyValuePair.Length != this.KeyValuePair.Length) return false;
			for(var i = 0; i < this.KeyValuePair.Length; i++) {
				if (b.KeyValuePair[i] != this.KeyValuePair[i]) return false;
			}
			return true;
		}
	}
	public class Settings 
	{
		public const string KeyBindings = "KeyBindings";
		public const string TypeDigitModifier = "TypeDigitModifier";
		public const string ExportToFile = "ExportToFile";
		public const string LoadCSV = "LoadCSV";
		public const string AcceptNumber = "AcceptNumber";
		public const string RejectNumber = "RejectNumber";
		public const string Undo = "Undo";
		public const string Redo = "Redo";
		public string[] KeyNamesSorted = new string[] { TypeDigitModifier, AcceptNumber, RejectNumber, ExportToFile, LoadCSV, Undo, Redo };
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
		public int HistoryIndex = 0;
		public List<DataAction> ActionHistory = new List<DataAction>();
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
			this.percentageCheckbox = new System.Windows.Forms.CheckBox();
			this.totalDiceLabel = new System.Windows.Forms.Label();
			this.scottPlot = new ScottPlot.FormsPlot();
			this.shortcutsBox = new System.Windows.Forms.GroupBox();
			this.shortcutGrid = new System.Windows.Forms.DataGridView();
			this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.recordLabel = new System.Windows.Forms.Label();
			this.recordButton = new System.Windows.Forms.Button();
			this.colorDialog1 = new System.Windows.Forms.ColorDialog();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.recentNumberLabel = new System.Windows.Forms.Label();
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
			this.dataGrid.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.dataGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			this.dataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Damage,
            this.Tally});
			this.dataGrid.Location = new System.Drawing.Point(3, 19);
			this.dataGrid.MultiSelect = false;
			this.dataGrid.Name = "dataGrid";
			this.dataGrid.RowHeadersVisible = false;
			this.dataGrid.RowTemplate.Height = 25;
			this.dataGrid.ShowCellErrors = false;
			this.dataGrid.ShowCellToolTips = false;
			this.dataGrid.ShowEditingIcon = false;
			this.dataGrid.Size = new System.Drawing.Size(127, 366);
			this.dataGrid.TabIndex = 1;
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
			this.damageTallyBox.Controls.Add(this.percentageCheckbox);
			this.damageTallyBox.Controls.Add(this.totalDiceLabel);
			this.damageTallyBox.Controls.Add(this.scottPlot);
			this.damageTallyBox.Controls.Add(this.dataGrid);
			this.damageTallyBox.Location = new System.Drawing.Point(267, 12);
			this.damageTallyBox.Name = "damageTallyBox";
			this.damageTallyBox.Size = new System.Drawing.Size(521, 426);
			this.damageTallyBox.TabIndex = 2;
			this.damageTallyBox.TabStop = false;
			this.damageTallyBox.Text = "Data";
			this.damageTallyBox.Enter += new System.EventHandler(this.damageTallyBox_Enter);
			// 
			// percentageCheckbox
			// 
			this.percentageCheckbox.AutoSize = true;
			this.percentageCheckbox.Checked = true;
			this.percentageCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.percentageCheckbox.Location = new System.Drawing.Point(199, 19);
			this.percentageCheckbox.Name = "percentageCheckbox";
			this.percentageCheckbox.Size = new System.Drawing.Size(122, 19);
			this.percentageCheckbox.TabIndex = 4;
			this.percentageCheckbox.Text = "Show Percentages";
			this.percentageCheckbox.UseVisualStyleBackColor = true;
			this.percentageCheckbox.CheckedChanged += new System.EventHandler(this.percentageCheckbox_CheckedChanged);
			// 
			// totalDiceLabel
			// 
			this.totalDiceLabel.AutoSize = true;
			this.totalDiceLabel.Location = new System.Drawing.Point(6, 395);
			this.totalDiceLabel.Name = "totalDiceLabel";
			this.totalDiceLabel.Size = new System.Drawing.Size(44, 15);
			this.totalDiceLabel.TabIndex = 3;
			this.totalDiceLabel.Text = "Total: 0";
			this.totalDiceLabel.Click += new System.EventHandler(this.totalDiceLabel_Click);
			// 
			// scottPlot
			// 
			this.scottPlot.BackColor = System.Drawing.SystemColors.ControlLight;
			this.scottPlot.Dock = System.Windows.Forms.DockStyle.Right;
			this.scottPlot.ImeMode = System.Windows.Forms.ImeMode.On;
			this.scottPlot.Location = new System.Drawing.Point(141, 19);
			this.scottPlot.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.scottPlot.Name = "scottPlot";
			this.scottPlot.Size = new System.Drawing.Size(377, 404);
			this.scottPlot.TabIndex = 2;
			// 
			// shortcutsBox
			// 
			this.shortcutsBox.Controls.Add(this.shortcutGrid);
			this.shortcutsBox.Controls.Add(this.recordLabel);
			this.shortcutsBox.Controls.Add(this.recordButton);
			this.shortcutsBox.Location = new System.Drawing.Point(12, 156);
			this.shortcutsBox.Name = "shortcutsBox";
			this.shortcutsBox.Size = new System.Drawing.Size(249, 282);
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
			this.shortcutGrid.Size = new System.Drawing.Size(243, 222);
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
			this.groupBox1.Controls.Add(this.recentNumberLabel);
			this.groupBox1.Controls.Add(this.damageLabel);
			this.groupBox1.Location = new System.Drawing.Point(12, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(249, 138);
			this.groupBox1.TabIndex = 4;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Damage Number";
			// 
			// recentNumberLabel
			// 
			this.recentNumberLabel.AutoSize = true;
			this.recentNumberLabel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.recentNumberLabel.Location = new System.Drawing.Point(3, 120);
			this.recentNumberLabel.Name = "recentNumberLabel";
			this.recentNumberLabel.Size = new System.Drawing.Size(46, 15);
			this.recentNumberLabel.TabIndex = 5;
			this.recentNumberLabel.Text = "Recent:";
			this.recentNumberLabel.Click += new System.EventHandler(this.label1_Click_2);
			// 
			// damageLabel
			// 
			this.damageLabel.Font = new System.Drawing.Font("Segoe UI", 28F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
			this.damageLabel.ForeColor = System.Drawing.Color.DarkGray;
			this.damageLabel.Location = new System.Drawing.Point(6, 19);
			this.damageLabel.Name = "damageLabel";
			this.damageLabel.Size = new System.Drawing.Size(237, 100);
			this.damageLabel.TabIndex = 4;
			this.damageLabel.Text = "308";
			this.damageLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.damageLabel.Click += new System.EventHandler(this.damageLabel_Click);
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
			this.damageTallyBox.PerformLayout();
			this.shortcutsBox.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.shortcutGrid)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		#region Helper Methods

		public Func<double, string> customFormatter = y => $"{Math.Round(y / _totalTallied * 100)}%";
		private void LoadSettings() {
			// Creates or loads an INI file in the same directory as your executable
			// named EXE.ini (where EXE is the name of your executable)
			this.IniFile = new IniFile(Application.StartupPath + Assembly.GetExecutingAssembly().GetName().Name + ".ini");
			var readSettings = new Settings();
			// KeyBindings section of settings ini

			// #1) Type Digit Modifier
			ReadOrSetValue(readSettings, Settings.KeyBindings, Settings.TypeDigitModifier, new Keys[] { Keys.Alt | Keys.Shift } /* ALT+SHIFT (+digit) */);

			// #2) Accept inputted number
			ReadOrSetValue(readSettings, Settings.KeyBindings, Settings.AcceptNumber, new Keys[] { Keys.Alt | Keys.Shift, Keys.End } /* Alt+Shift+End */ );

			// #3)  Reject inputted number
			ReadOrSetValue(readSettings, Settings.KeyBindings, Settings.RejectNumber, new Keys[] { Keys.Alt | Keys.Shift, Keys.Delete } /* Alt+Shift+Delete */);

			// #4) Export to Files
			ReadOrSetValue(readSettings, Settings.KeyBindings, Settings.ExportToFile, new Keys[] { Keys.Control | Keys.Shift, Keys.S } /* CTRL+SHIFT+S*/);

			// #5) Load from CSV File
			ReadOrSetValue(readSettings, Settings.KeyBindings, Settings.LoadCSV, new Keys[] { Keys.Control | Keys.Shift, Keys.O } /* CTRL+SHIFT+O*/);

			// #6) Undo
			ReadOrSetValue(readSettings, Settings.KeyBindings, Settings.Undo, new Keys[] { Keys.Control | Keys.Alt, Keys.Z } /* CTRL+SHIFT+O*/);

			// #7) Redo
			ReadOrSetValue(readSettings, Settings.KeyBindings, Settings.Redo, new Keys[] { Keys.Control | Keys.Alt, Keys.Y } /* CTRL+SHIFT+O*/);



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
			this.shortcutGrid.KeyDown += shortcutGrid_KeyDown;
			this.KeyPress += RecordButton_KeyPressed;
			this.KeyUp += RecordButton_KeyUp;
			this.KeyDown += RecordButton_KeyDown;
			this.dataGrid.KeyUp += DataGrid_KeyUp;

			this.ClearDigits();
			this.Settings = readSettings;
			SortGrid();
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
			if (keyData.HasFlag(Keys.Alt) && (Keys.Alt | Keys.Enter) == keyData) {
				if (this._recording) {
					this.recordLabel.ForeColor = Color.Crimson; 
					if (!_tempKeys.Contains(Keys.Enter)) {

						_tempKeys = new List<Keys>() { ModifierKeys, Keys.Enter};
						this.recordLabel.Text = KeysToFriendlyString(_tempKeys.ToArray());
					}
				}
				return true;
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}
		private void shortcutGrid_KeyDown(object sender, KeyEventArgs e) {
			if (e.KeyCode > 0 && ((e.KeyData & ~ModifierKeys & ~Keys.Menu) != Keys.None)) {
				e.Handled = true;
			}
		}

		private void RecordButton_KeyUp(object sender, KeyEventArgs e) {
			if (!this._recording) return;
			e.Handled = true;
		}

		private void DataGrid_KeyUp(object sender, KeyEventArgs e) {
			if (this._recording) return;
			if (e.Modifiers != Keys.None) return;
			if (e.KeyCode == Keys.Delete) {
				if (this.dataGrid.SelectedCells.Count > 0) {
					var rowIndex = this.dataGrid.SelectedCells[0].RowIndex;
					var damageNumber = this.dataGrid.Rows[rowIndex].Cells[0].Value.ToString();
					var damageTally = this.dataGrid.Rows[rowIndex].Cells[1].Value.ToString();
					var confirmResult = MessageBox.Show($"Are you sure to delete tallies for {damageNumber}?",
									 "Confirm Deletion!",
									 MessageBoxButtons.YesNo);
					if (confirmResult == DialogResult.Yes) {
						var action = AddActionToHistory(DataActionType.DeleteRow, new string[] { damageNumber, damageTally });
						ProcessAction(action);
					}
				}
			}
		}

		private void SortGrid() {
			foreach (DataGridViewColumn column in dataGrid.Columns) {
				column.SortMode = DataGridViewColumnSortMode.NotSortable;
			}
			dataGrid.Sort(new DataGridNumericComparer());
			this.totalDiceLabel.Text = $"Total: {this.dataGrid.Rows.Count}";
		}

		private void DrawHistogram() {
			if (_totalTallied <= 1) return;
			this.scottPlot.Plot.Style(figureBackground: DefaultBackColor);
			scottPlot.Reset();
			if (dataGrid.Rows.Count > 1) {
				List<double> values = new List<double>();
				List<double> positions = new List<double>();
				int maxY = 0;
				int maxX = 0;
				int minX = -1;
				foreach (DataGridViewRow row in dataGrid.Rows) {
					int damageNumber = 0;
					int tally = 0;
					try {
						damageNumber = int.Parse(row.Cells[0].Value.ToString());
						if (damageNumber > maxX) maxX = damageNumber;
						if (minX == -1 || damageNumber < minX) minX = damageNumber;
						tally = int.Parse(row.Cells[1].Value.ToString());
						if (tally > maxY) maxY = tally;

					} catch (Exception ex) {

					}
					values.Add(tally);
					positions.Add(damageNumber);
				}

				var plt = scottPlot.Plot;
				plt.Style(figureBackground: DefaultBackColor);

				// generate sample heights are based on https://ourworldindata.org/human-height

				// create a histogram
				//(double[] counts, double[] binEdges) = ScottPlot.Statistics.Common.Histogram(histogramData.Values.ToArray(), min: histogramData.MinBy(kvp => kvp.Key).Key - 2, max: histogramData.MaxBy(kvp => kvp.Key).Key + 2, binSize: 1);
				//double[] leftEdges = binEdges.Take(binEdges.Length - 1).ToArray();
				// display the histogram counts as a bar plot
				var bar = plt.AddBar(values.ToArray(), positions.ToArray());
				bar.ShowValuesAboveBars = this.percentageCheckbox.Checked;

				var total = (double)_totalTallied;
				bar.ValueFormatter = customFormatter;
				bar.BarWidth = 1;

				// customize the plot style
				plt.YAxis.Label("Tally");
				plt.XAxis.Label("Damage");
				plt.SetAxisLimits(yMin: 0,yMax: maxY + 5,xMin: minX - 1,xMax: maxX + 1);
				scottPlot.Refresh();
			}
		}

		private void dataGridView1_SortCompare(object sender, DataGridViewSortCompareEventArgs e) {
			//Suppose your interested column has index 1
			if (e.Column.Index == 0) {
				e.SortResult = int.Parse(e.CellValue1.ToString()).CompareTo(int.Parse(e.CellValue2.ToString()));
				e.Handled = true;//pass by the default sorting
			}
		}
		
		private void RecordButton_KeyDown(object sender, KeyEventArgs e) {
			if (!this._recording) return;
			e.Handled = true;
			if (e.KeyValue >= 0 || e.Modifiers >= 0) {
				if (e.KeyCode == Keys.Escape) {
					this._recording = false;
					this.recordLabel.ForeColor = DefaultForeColor;
					this.recordButton.ForeColor = DefaultForeColor;
					if (_tempKeys.Count == 0) return;
					RebindHotkeysAfterRecording();
					this.SaveTempKeysToSelectedShortcut();
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
			if (this._recording) e.Handled = true;
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
		private Label recentNumberLabel;
		private ScottPlot.FormsPlot scottPlot;
		private Label totalDiceLabel;
		private CheckBox percentageCheckbox;
	}
}