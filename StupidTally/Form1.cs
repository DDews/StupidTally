using Shortcut;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Text;
using System.Linq;
using System.Media;

namespace StupidTally
{
	
	public partial class Form1 : Form
	{
		private Hotkey _tempHotkey = null;
		private List<Keys> _tempKeys = new List<Keys>();
		private List<Hotkey> _hotkeys = new List<Hotkey>();
		private string _tempDigits;
		private static int _totalTallied = 0;
		private static string _previousTally = "";
		private static Keys[] DigitKeys = new Keys[] {
			Keys.D1,Keys.D2,Keys.D3, Keys.D4, Keys.D4, Keys.D5, 
			Keys.D6, Keys.D7, Keys.D8, Keys.D9, Keys.D0, 
			Keys.NumPad1, Keys.NumPad2, Keys.NumPad3, Keys.NumPad3, Keys.NumPad4, Keys.NumPad5, 
			Keys.NumPad6, Keys.NumPad7, Keys.NumPad8, Keys.NumPad9, Keys.NumPad0
		};

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
		private void dataGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e) {
			var damageValue = this.dataGrid.Rows[e.RowIndex].Cells[0].Value.ToString();
			var cellValue = this.dataGrid.Rows[e.RowIndex].Cells[1].Value.ToString();
			string promptValue = Prompt.ShowDialog($"Enter a new value for damage value '{damageValue}' instead of '{cellValue}':", cellValue);
			if (!string.IsNullOrWhiteSpace(promptValue)) {
				var action = this.AddActionToHistory(DataActionType.ChangeValue,new string[] { damageValue, promptValue, cellValue});
				this.ProcessAction(action);
			}
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
				if (pairSetting.KeyName.Equals(Settings.TypeDigitModifier)) {
					if (DigitKeys.Contains(_tempKeys.Last()))
					{
						if (pairSetting.Value.Equals(string.Join("+", _tempKeys.GetRange(0,_tempKeys.Count - 1)))) {
							MessageBox.Show($"This shortcut combination is already being used for: '{pair.Key}'!");
							return;
						}
					}
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
						case Settings.Undo:
							if (!_hotkeyBinder.IsHotkeyAlreadyBound(hotkey)) _hotkeyBinder.Bind(hotkey).To(UndoLastActionCallback);
							break;
						case Settings.Redo:
							if (!_hotkeyBinder.IsHotkeyAlreadyBound(hotkey)) _hotkeyBinder.Bind(hotkey).To(RedoLastActionCallback);
							break;
						case Settings.NewFile:
							if (!_hotkeyBinder.IsHotkeyAlreadyBound(hotkey)) _hotkeyBinder.Bind(hotkey).To(NewFileCallback);
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

		private void NewFileCallback() {
			var confirmResult = MessageBox.Show($"You will lose your current progress and undo/redo history. Are you sure you want to start over?",
									 "Confirm Starting Over?",
									 MessageBoxButtons.YesNo);
			if (confirmResult == DialogResult.Yes) {
				this.dataGrid.Rows.Clear();
				_totalTallied = 0;
				_previousTally = "";
				this.scottPlot.Plot.Clear();
				this.scottPlot.Plot.ResetLayout();
				this.scottPlot.Reset();
				this.Text = "Stupid Tally";
				this.recentNumberLabel.Text = "Recent: ";
				this.totalDiceLabel.Text = "Rows: 0";
				this.scottPlot.Plot.Style(figureBackground: DefaultBackColor);
				this.scottPlot.Refresh();
			}
		}

		private void RedoLastActionCallback() {
			if (this._recording) return;
			if (this.HistoryIndex < this.ActionHistory.Count) {
				var action = this.ActionHistory[HistoryIndex];
				this.HistoryIndex++;
				ProcessAction(action);
			} else {
				SystemSounds.Beep.Play();
			}
		}

		private void UndoLastActionCallback() {
			if (this._recording) return;
			UndoAction();
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
			this.damageLabel.ForeColor = Color.DarkGray;
		}

		private void AcceptNumberCallback() {
			if (_recording) return;
			if (string.IsNullOrWhiteSpace(_tempDigits)) return;
			bool numberExists = false;
			if (this.dataGrid.Rows.Count > 0) {
				var rowCollection = this.dataGrid.Rows.Cast<DataGridViewRow>();
				numberExists = rowCollection.Any(x => x.Cells[0].Value.ToString().Equals(_tempDigits));
			}
			DataAction action = null;
			if (numberExists)
			{
				action = AddActionToHistory(DataActionType.TallyUp,new string[] {_tempDigits,"0"});
			} else {
				action = AddActionToHistory(DataActionType.AddRow,new string[] {_tempDigits,"1" });
			}
			ProcessAction(action);
		}

		private void CalcTotalTallied() {
			int output = 0;
			foreach (DataGridViewRow row in dataGrid.Rows) {
				string rowTallyStr = row.Cells[1].Value.ToString();
				int rowTally = 0;
				try {
					rowTally = int.Parse(rowTallyStr);
				} catch (Exception ex) {

				}
				output += rowTally;
			}
			_totalTallied = output;
		}
		private void ProcessAction(DataAction action) {
			System.Diagnostics.Debug.WriteLine($"ProcessActioon({action.ToString()})");
			_previousTally = action.PreviousTally;
			switch (action.Type) {
				case DataActionType.TallyUp:
					_previousTally = action.KeyValuePair[0];
					AddTally(action.KeyValuePair[0]);
					_totalTallied++;
					break;
				case DataActionType.AddRow:
					_previousTally = action.KeyValuePair[0];
					AddDataRow(action.KeyValuePair[0], action.KeyValuePair[1]);
					_totalTallied++;
					break;
				case DataActionType.DeleteRow:
					DeleteDataRow(action.KeyValuePair[0]);
					CalcTotalTallied();
					break;
				case DataActionType.ChangeValue:
					ChangeDataValue(action.KeyValuePair[0], action.KeyValuePair[1]);
					_previousTally = action.KeyValuePair[0];
					CalcTotalTallied();
					break;
				default:
					break;
			}
			ClearDigits();
			SortGrid();
			DrawHistogram();
			this.recentNumberLabel.Text = $"Recent: {_previousTally}";
			this.totalDiceLabel.Text = $"Rows: {this.dataGrid.Rows.Count}";
			this.Text = $"Stupid Tally - Total Tallied: {_totalTallied}";
		}
		private void ReverseAction(DataAction action) {
			System.Diagnostics.Debug.WriteLine($"ReverseAction({action.ToString()})");
			_previousTally = action.PreviousTally;
			switch (action.Type) {
				case DataActionType.TallyUp:
					UndoTally(action.KeyValuePair[0]);
					_totalTallied--;
					break;
				case DataActionType.AddRow:
					DeleteDataRow(action.KeyValuePair[0]);
					CalcTotalTallied();
					break;
				case DataActionType.DeleteRow:
					AddDataRow(action.KeyValuePair[0], action.KeyValuePair[1]);
					_totalTallied++;
					break;
				case DataActionType.ChangeValue:
					ChangeDataValue(action.KeyValuePair[0], action.KeyValuePair[2]);
					CalcTotalTallied();
					break;
				default:
					break;
			}
			ClearDigits();
			SortGrid();
			DrawHistogram();
			this.recentNumberLabel.Text = $"Recent: {_previousTally}";
			this.totalDiceLabel.Text = $"Rows: {this.dataGrid.Rows.Count}";
			this.Text = $"Stupid Tally - Total Tallied: {_totalTallied}";
		}
		// Add an action to the recorded history of actions taken.
		// For continuity to allow us to perform undo/redo
		private DataAction AddActionToHistory(DataActionType type, string[] vals) {
			var action = new DataAction(type,vals,_previousTally);
			
			// check if we've already undone actions and are "in the past"
			// so we can clean up "time paradoxes"
			if (this.HistoryIndex < this.ActionHistory.Count) {
				if (this.HistoryIndex < this.ActionHistory.Count - 1){ 
					// is this action the same as the next one?
					var nextAction = this.ActionHistory[this.HistoryIndex + 1];
					if (nextAction.Equals(action)) {
						// we're replaying history, so don't prune the branch yet
						this.HistoryIndex++;
						return this.ActionHistory[this.HistoryIndex];
					}
				}
				// We create a new path of actions and rewrite history,
				// so we remove all actions from the previous branch of history

				for(var i = this.ActionHistory.Count - 1; i >= this.HistoryIndex; i--) {
					this.ActionHistory.RemoveAt(i);
				}
				this.ActionHistory.Add(action);
				this.HistoryIndex++;
				return action;
			}
			// add this action to the recorded history of actions for undo/redo
			this.ActionHistory.Add(action);
			this.HistoryIndex++;
			return action;
		}
		// Undo the previous action recorded in history
		// and move the pointer HistoryIndex down by 1
		private DataAction UndoAction() {
			System.Diagnostics.Debug.WriteLine($"UndoAction()");
			System.Diagnostics.Debug.WriteLine($"HistoryIndex: {HistoryIndex}/{ActionHistory.Count}");
			if (this.HistoryIndex <= 0) {
				this.HistoryIndex = 0;
				this.scottPlot.Plot.ResetLayout();
				SystemSounds.Beep.Play();
				return null;
			}
			HistoryIndex--;
			var action = this.ActionHistory[HistoryIndex];
			ReverseAction(action);
			return action;
		}
		private void AddTally(string key) {
			System.Diagnostics.Debug.WriteLine($"AddTally({key})");
			foreach (DataGridViewRow row in dataGrid.Rows) {
				if (row.Cells.Count > 0 && row.Cells[0].Value != null) {
					var cell = row.Cells[0];
					if (cell.Value != null && cell.Value.ToString().Equals(key)) {
						string strValue = row.Cells[1].Value.ToString();
						int intValue = 0;
						try {
							intValue = int.Parse(strValue);
						} catch (Exception ex) {

						}
						intValue++;
						row.Cells[1].Value = $"{intValue}";
						return;
					}
				}
			}

		}
		private void UndoTally(string key) {
			System.Diagnostics.Debug.WriteLine($"UndoTally({key})");
			foreach (DataGridViewRow row in dataGrid.Rows) {
				if (row.Cells.Count > 0 && row.Cells[0].Value != null) {
					var cell = row.Cells[0];
					if (cell.Value != null && cell.Value.ToString().Equals(key)) {
						string strValue = row.Cells[1].Value.ToString();
						int intValue = 0;
						try {
							intValue = int.Parse(strValue);
						} catch (Exception ex) {

						}
						intValue--;
						row.Cells[1].Value = $"{intValue}";
						return;
					}
				}
			}

		}
		private void AddDataRow(string key, string val) {
			System.Diagnostics.Debug.WriteLine($"AddDataRow({key},{val})");
			this.dataGrid.Rows.Add(new string[] {key, val });
		}
		private void ChangeDataValue(string key, string val) {
			System.Diagnostics.Debug.WriteLine($"ChangeDataValue({key},{val})");
			for (var i = 0; i < dataGrid.Rows.Count; i++) {
				var row = dataGrid.Rows[i];
				if (row.Cells.Count > 0 && row.Cells[0].Value != null) {
					var cell = row.Cells[0];
					if (cell.Value != null && cell.Value.ToString().Equals(key)) {
						dataGrid.Rows[i].Cells[1].Value = val.ToString();
						return;
					}
				}
			}
		}
		private void DeleteDataRow(string key) {
			System.Diagnostics.Debug.WriteLine($"DeleteDataRow({key})");
			for (var i = 0; i < dataGrid.Rows.Count; i++) {
				var row = dataGrid.Rows[i];
				if (row.Cells.Count > 0 && row.Cells[0].Value != null) {
					var cell = row.Cells[0];
					if (cell.Value != null && cell.Value.ToString().Equals(key)) {
						dataGrid.Rows.RemoveAt(i);
						return;
					}
				}
			}
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

							bmp.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Png);
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
				var confirmResult = MessageBox.Show($"You will lose your current progress and undo/redo history. Are you sure you want to load data from a CSV file?",
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
					this.totalDiceLabel.Text = "Rows: 0";
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
					_previousTally = "";
					this.Text = $"Stupid Tally - Total Tallied: {_totalTallied}";
					this.recentNumberLabel.Text = $"Recent:";
					SortGrid();
					DrawHistogram();
					// the history of actions taken is not recorded in the CSV file
					// so our new starting point has data already collected
					this.ActionHistory = new List<DataAction>();
					this.HistoryIndex = 0;
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