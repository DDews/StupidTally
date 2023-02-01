using Shortcut;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Text;

namespace StupidTally
{
	public partial class Form1 : Form
	{
		private Hotkey _tempHotkey = null;
		private List<Keys> _tempKeys = new List<Keys>();
		private List<Hotkey> _hotkeys = new List<Hotkey>();
		private string _tempDigits;
		private static int _totalTallied = 0;

		public Form1() {
			InitializeComponent();
			LoadSettings();
			this._selectedShortcutIndex = 0;
			ResetGlobalKeyboardBindings();
			foreach (DataGridViewColumn column in shortcutGrid.Columns) {
				column.SortMode = DataGridViewColumnSortMode.NotSortable;
			}
			this.scottPlot.Plot.Style(figureBackground: DefaultBackColor);
			this.scottPlot.Refresh();
		}

		private void label1_Click(object sender, EventArgs e) {

		}

		private void shortcutList_SelectedIndexChanged(object sender, EventArgs e) {

		}

		private void label1_Click_1(object sender, EventArgs e) {

		}

		private void SaveTempKeysToSelectedShortcut() {
			var keyName = this.Settings.KeyNamesSorted[_selectedShortcutIndex];
			
			var setting = this.Settings.Data[keyName];
			if (_tempKeys.Count > 1 && keyName.Equals(Settings.TypeDigitModifier)) {
				// only modifier keys allowed!
				var printedVer = _tempKeys[1].ToString();
				int intVer = -1;
				try {
					intVer = int.Parse(printedVer);
				}
				catch (Exception ex) {
					intVer = -1;
				}
				if (!printedVer.Equals("0") && intVer > 0) {
					MessageBox.Show("Only modifier keys (CTRL, ALT, SHIFT) and Number keys allowed.");
					return;
				}
			}
			var keyString = string.Join("+", _tempKeys);

			foreach (KeyValuePair<string,Setting> pair in Settings.Data) {
				var pairSetting = pair.Value;
				if (pair.Key == keyName && pair.Value.Equals(keyString)) {
					return;
				}
				if (pairSetting.Value.Equals(keyString)) {
					MessageBox.Show($"This shortcut combination is already being used for: '{pair.Key}'!");
					return;
				}
			}
			
			setting.Value = keyString;
			

			// save to ini file
			if (!string.IsNullOrEmpty(setting.SectionName) && setting.SectionName.Equals(Settings.KeyBindings)) {
				this.shortcutGrid.Rows[_selectedShortcutIndex].SetValues(new string[] { setting.KeyName, setting.Value });
			}
			this.IniFile.Write(setting.KeyName, setting.Value, setting.SectionName);
			ResetGlobalKeyboardBindings();
		}

		private Keys StringToKey(string keys) {
			Keys key = Keys.None;
			try {
				Enum.TryParse(keys, out key);
			} catch (Exception ex) {

			}
			return key;
		}
		private Modifiers StringToModifiers(string keys) {
			Modifiers key = Modifiers.None;
			
			var split = keys.Split(", ");
			uint mods = 0;
			foreach (string modifier in split) {
				try {
					Enum.TryParse(modifier, out key);
				}
					catch (Exception ex2) {

				}
				mods |= (uint)key;
			}
			key = (Modifiers)mods;
			return key;
		}

		private void ResetGlobalKeyboardBindings() {
			foreach(Hotkey hotkey in _hotkeys) {
				if (_hotkeyBinder.IsHotkeyAlreadyBound(hotkey))
				{
					_hotkeyBinder.Unbind(hotkey);
				}
			}
			_hotkeys.Clear();
			foreach (KeyValuePair<string,Setting> pair in this.Settings.Data) {
				var keyName = pair.Key;
				var setting = pair.Value;
				var stringKeys = setting.Value.Split("+");
				if (stringKeys.Length > 1) {
					// modifier keys
					var modifierKeys = StringToModifiers(stringKeys[0]);
					var keys = StringToKey(stringKeys[1]);
					Hotkey hotkey = new Hotkey(modifierKeys,keys);
					_hotkeys.Add(hotkey);
					switch (keyName) {
						case Settings.TypeDigitModifier:
							BindTypeDigitModifier(modifierKeys, hotkey);
							break;
						case Settings.ExportToFile:
							if (!_hotkeyBinder.IsHotkeyAlreadyBound(hotkey)) _hotkeyBinder.Bind(hotkey).To(ExportToFileCallback);
							break;
						case Settings.LoadCSV:
							if (!_hotkeyBinder.IsHotkeyAlreadyBound(hotkey)) _hotkeyBinder.Bind(hotkey).To(LoadFromCSVFileCallback);
							break;
						case Settings.AcceptNumber:
							if (!_hotkeyBinder.IsHotkeyAlreadyBound(hotkey)) _hotkeyBinder.Bind(hotkey).To(AcceptNumberCallback);
							break;
						case Settings.RejectNumber:
							if (!_hotkeyBinder.IsHotkeyAlreadyBound(hotkey)) _hotkeyBinder.Bind(hotkey).To(RejectNumberCallback);
							break;
						default:
							break;
					}
					
				} else {
					// no modifier keys
					BindTypeDigitModifier(StringToModifiers(setting.Value));
				}
			}
		}

		private void CreateAndRememberHotkey(Action action, Modifiers modifiers, Keys keys = Keys.None) {
			var hotkey = new Hotkey(modifiers, keys);
			_hotkeys.Add(hotkey);
			if (!_hotkeyBinder.IsHotkeyAlreadyBound(hotkey)) _hotkeyBinder.Bind(hotkey).To(action);
		}
		private void BindTypeDigitModifier(Modifiers modifierKeys, Hotkey hotkey = null) {

			CreateAndRememberHotkey(TypeDigitModifierCallbackOne, modifierKeys, Keys.NumPad1);
			CreateAndRememberHotkey(TypeDigitModifierCallbackOne, modifierKeys, Keys.D1);
			CreateAndRememberHotkey(TypeDigitModifierCallbackTwo, modifierKeys, Keys.NumPad2);
			CreateAndRememberHotkey(TypeDigitModifierCallbackTwo, modifierKeys, Keys.D2);
			CreateAndRememberHotkey(TypeDigitModifierCallbackThree, modifierKeys, Keys.NumPad3);
			CreateAndRememberHotkey(TypeDigitModifierCallbackThree, modifierKeys, Keys.D3);
			CreateAndRememberHotkey(TypeDigitModifierCallbackFour, modifierKeys, Keys.NumPad4);
			CreateAndRememberHotkey(TypeDigitModifierCallbackFour, modifierKeys, Keys.D4);
			CreateAndRememberHotkey(TypeDigitModifierCallbackFive, modifierKeys, Keys.NumPad5);
			CreateAndRememberHotkey(TypeDigitModifierCallbackFive, modifierKeys, Keys.D5);
			CreateAndRememberHotkey(TypeDigitModifierCallbackSix, modifierKeys, Keys.NumPad6);
			CreateAndRememberHotkey(TypeDigitModifierCallbackSix, modifierKeys, Keys.D6);
			CreateAndRememberHotkey(TypeDigitModifierCallbackSeven, modifierKeys, Keys.NumPad7);
			CreateAndRememberHotkey(TypeDigitModifierCallbackSeven, modifierKeys, Keys.D7);
			CreateAndRememberHotkey(TypeDigitModifierCallbackEight, modifierKeys, Keys.NumPad8);
			CreateAndRememberHotkey(TypeDigitModifierCallbackEight, modifierKeys, Keys.D8);
			CreateAndRememberHotkey(TypeDigitModifierCallbackNine, modifierKeys, Keys.NumPad9);
			CreateAndRememberHotkey(TypeDigitModifierCallbackNine, modifierKeys, Keys.D9);
			CreateAndRememberHotkey(TypeDigitModifierCallbackZero, modifierKeys, Keys.NumPad0);
			CreateAndRememberHotkey(TypeDigitModifierCallbackZero, modifierKeys, Keys.D0);
		}

		private void ClearDigits() {
			_tempDigits = "";
			this.damageLabel.Text = "000";
			this.damageLabel.ForeColor = Color.DarkGray;
			dataGrid.Sort(dataGrid.Columns[0], ListSortDirection.Ascending);
			dataGrid.ShowCellToolTips = false;
		}
		Color ColorizedDigit() {
			if (_tempDigits.Length == 0) return Color.DarkGray;
			foreach (DataGridViewRow row in dataGrid.Rows) {
				if (row.Cells.Count > 0 && row.Cells[0].Value != null) {
					if (row.Cells[0].Value.ToString().Equals(_tempDigits)) {
						return Color.DarkGreen;
					}
				}
			}
			return Color.Crimson;
		}
		private void AddDigit(string digit) {
			_tempDigits += digit;
			this.damageLabel.Text = _tempDigits;
			this.damageLabel.ForeColor = ColorizedDigit();
		}
		private void TypeDigitModifierCallbackZero() {
			if (_recording) return;
			AddDigit("0");
		}

		private void TypeDigitModifierCallbackNine() {
			if (_recording) return;
			AddDigit("9");
		}

		private void TypeDigitModifierCallbackEight() {
			if (_recording) return;
			AddDigit("8");
		}

		private void TypeDigitModifierCallbackSeven() {
			if (_recording) return;
			AddDigit("7");
		}

		private void TypeDigitModifierCallbackSix() {
			if (_recording) return;
			AddDigit("6");
		}

		private void TypeDigitModifierCallbackFive() {
			if (_recording) return;
			AddDigit("5");
		}

		private void TypeDigitModifierCallbackFour() {
			if (_recording) return;
			AddDigit("4");
		}

		private void TypeDigitModifierCallbackThree() {
			if (_recording) return;
			AddDigit("3");
		}

		private void TypeDigitModifierCallbackTwo() {
			if (_recording) return;
			AddDigit("2");
		}

		private void TypeDigitModifierCallbackOne() {
			if (_recording) return;
			AddDigit("1");
		}

		private void RejectNumberCallback() {
			if (_recording) return;
			ClearDigits();
			this.damageLabel.ForeColor = DefaultForeColor;
		}

		private void AcceptNumberCallback() {
			if (_recording) return;
			if (string.IsNullOrWhiteSpace(_tempDigits)) return;
			//this.dataGrid.Rows.Add(new string[] { _tempDigits, "1" });
			this.recentNumberLabel.Text = $"Recent: {_tempDigits}";
			_totalTallied++;
			this.Text = $"Stupid Tally - Total Tallied: {_totalTallied}";
			foreach (DataGridViewRow row in dataGrid.Rows) {
				if (row.Cells.Count > 0 && row.Cells[0].Value != null) {
					if (row.Cells[0].Value.ToString().Equals(_tempDigits)) {
						string strValue = row.Cells[1].Value.ToString();
						int intValue = 0;
						try {
							intValue = int.Parse(strValue);
						} catch (Exception ex) {

						}
						intValue++;
						row.Cells[1].Value = $"{intValue}";
						ClearDigits();
						SortGrid();
						return;
					}
				}
			}
			dataGrid.Rows.Add(new string[] { _tempDigits, "1"});
			ClearDigits();
			SortGrid();
		}
		private void ExportToFileCallback() {
			if (_recording) return;
			if (dataGrid.Rows.Count > 0) {
				SaveFileDialog sfd = new SaveFileDialog();
				sfd.Filter = "CSV (*.csv)|*.csv";
				sfd.FileName = "Output.csv";
				bool fileError = false;
				if (sfd.ShowDialog() == DialogResult.OK) {
					if (File.Exists(sfd.FileName)) {
						try {
							File.Delete(sfd.FileName);
						} catch (IOException ex) {
							fileError = true;
							MessageBox.Show("It wasn't possible to write the data to the disk." + ex.Message);
						}
					}
					if (!fileError) {
						try {
							int columnCount = dataGrid.Columns.Count;
							string columnNames = "";
							string[] outputCsv = new string[dataGrid.Rows.Count + 1];
							for (int i = 0; i < columnCount; i++) {
								columnNames += dataGrid.Columns[i].HeaderText.ToString() + ",";
							}
							outputCsv[0] += columnNames;

							for (int i = 1; (i - 1) < dataGrid.Rows.Count; i++) {
								for (int j = 0; j < columnCount; j++) {
									outputCsv[i] += dataGrid.Rows[i - 1].Cells[j].Value.ToString() + ",";
								}
							}

							File.WriteAllLines(sfd.FileName, outputCsv, Encoding.UTF8);
						} catch (Exception ex) {
							MessageBox.Show("Error :" + ex.Message);
						}
					}
				}
				sfd = new SaveFileDialog();
				sfd.Filter = "PNG (*.png)|*.png";
				sfd.FileName = "Output.png";
				fileError = false;
				if (sfd.ShowDialog() == DialogResult.OK) {
					if (File.Exists(sfd.FileName)) {
						try {
							File.Delete(sfd.FileName);
						} catch (IOException ex) {
							fileError = true;
							MessageBox.Show("It wasn't possible to write the data to the disk." + ex.Message);
						}
					}
					if (!fileError) {
						try {
							Bitmap bmp = new Bitmap(this.scottPlot.Width, this.scottPlot.Height);

							this.scottPlot.DrawToBitmap(bmp, new Rectangle(0, 0, this.scottPlot.Width, this.scottPlot.Height));

							bmp.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
						} catch (Exception ex) {
							MessageBox.Show("Error :" + ex.Message);
						}
					}
				}
			} else {
				MessageBox.Show("No data to export.", "Info");
			}
		}
		private void LoadFromCSVFileCallback() {
			if (_recording) return;
			if (dataGrid.Rows.Count > 0) {
				var confirmResult = MessageBox.Show($"You will lose your current progress. Are you sure you want to load data from a CSV file?",
									 "Confirm Starting Over?",
									 MessageBoxButtons.YesNo);
				if (confirmResult == DialogResult.No) {
					return;
				}
			}
			// Browse and Select CSV file to open
			try {
				string filename = "";
				OpenFileDialog dialog = new OpenFileDialog();
				dialog.Title = "Open CSV File";
				dialog.Filter = "CSV Files (*.csv)|*.csv";
				if (dialog.ShowDialog() == DialogResult.OK) {
					filename = dialog.FileName;
				} else {
					return;
				}
				// read from CSV to load values
				using (var reader = new StreamReader(filename)) {
					// read first line for headers
					var line = reader.ReadLine();
					var values = line.Split(',');
					string firstColumn = values[0];
					string secondColumn = values[1];
					if ((values.Length > 2 && !string.IsNullOrEmpty(values[2])) || (!firstColumn.ToLower().Equals("damage") || !secondColumn.ToLower().Equals("tally"))) {
						// Invalid headers -- Invalid CSV
						MessageBox.Show($"Error: This CSV file is invalid. Expecting two columns named: 'Damage', 'Tally'.\nEncountered line: {line}");
						return;
					}

					// input file looks good. clear data to start fresh
					this.recentNumberLabel.Text = "Recent:";
					this.dataGrid.Rows.Clear();
					this.totalDiceLabel.Text = "Total: 0";
					ClearDigits();
					scottPlot.Reset();
					int newTotal = 0;
					while (!reader.EndOfStream) {
						line = reader.ReadLine();
						values = line.Split(',');
						string damage = values[0];
						string tally = values[1];
						try {
							int damageInt = int.Parse(damage);
							int tallyInt = int.Parse(tally);
							newTotal += tallyInt;
						} catch(Exception ex) {
							MessageBox.Show($"Error loading CSV file: invalid data values from line '{line}'. Was expecting integers.");
							return;
						}
						dataGrid.Rows.Add(new string[] { values[0], values[1] });
					}
					_totalTallied = newTotal;
					this.Text = $"Stupid Tally - Total Tallied: {_totalTallied}";
					SortGrid();
				}
			} catch (Exception) {
				throw;
			}
		}
		private void TypeDigitModifierCallback() {
			
		}

		private void recordButton_Click(object sender, EventArgs e) {
			if (_recording) {
				_recording = false;
				this.recordButton.ForeColor = DefaultForeColor;
				this.recordLabel.ForeColor = DefaultForeColor;
				if (_tempKeys.Count == 0) return;
				RebindHotkeysAfterRecording();
				SaveTempKeysToSelectedShortcut();
			} else {
				if (shortcutGrid.SelectedCells.Count > 0) _selectedShortcutIndex = shortcutGrid.SelectedCells[0].RowIndex;
				this.recordButton.ForeColor = Color.Crimson;
				_recording = true;
				TemporarilyUnbindHotkeys();
				_tempKeys.Clear();
			}
		}

		private void TemporarilyUnbindHotkeys() {
			foreach (Hotkey hotkey in _hotkeys) {
				if (_hotkeyBinder.IsHotkeyAlreadyBound(hotkey)) _hotkeyBinder.Unbind(hotkey);
			}
		}

		private void RebindHotkeysAfterRecording() {
			ResetGlobalKeyboardBindings();
		}

		private void shortcutGrid_CellContentClick(object sender, DataGridViewCellEventArgs e) {
			
			if (e.RowIndex <= this.Settings.Data.Keys.Count) {
				this._selectedShortcutIndex = e.RowIndex;
				_recording = true;
				TemporarilyUnbindHotkeys();
				this._tempKeys.Clear();
				this.recordButton.ForeColor = Color.Crimson;
			}
		}

		private void label1_Click_2(object sender, EventArgs e) {

		}

		private void damageTallyBox_Enter(object sender, EventArgs e) {

		}

		private void totalDiceLabel_Click(object sender, EventArgs e) {

		}

		private void damageLabel_Click(object sender, EventArgs e) {

		}

		private void percentageCheckbox_CheckedChanged(object sender, EventArgs e) {
			this.DrawHistogram();
		}
	}
}