# Stupid Tally
This is a rather stupid (and rushed) tally program to help quickly create spreadsheets of recurring damage numbers from video games in hopes that science and math can help determine the underlying damage formula.

It is most helpful when trying to determine "rolled dice values" for damage formulas, by helping record and graph distribution of recurring damage values.

_Note: When trying to record recurring damage numbers for gathering statistics, run this program as administrator._

![Alt text](/StupidTally-Labelled-window.png?raw=true "Window, Labelled")

_Above: window, labelled. Explanations below._

# **Section A** - "Damage Number"
## **(Aa)** "Current damage input"
By using the keyboard shorcut `TypeDigitModifier` and a `Digit key` (numpad or number row), digits will be written out here.
* When a number is unmatched, it is colored RED.
* When a number is matched with one that is already recorded, it is colored GREEN.

_Ex: With default keybinding "Alt, Shift", holding `Alt, Shift` and typing out `123` will show the number `123` here._

## **(Ab)** "Recent (last) recorded damage"
This shows the last damage number that was recorded.
This is just to help you confirm you didn't input a mistake.

# **Section B** - "Global Shortcuts"
## **(Ba)** "List of shortcuts"

### **(Ba)** "What is this?"
This app allows keybindings to be global hotkeys that work within an active game to help record numbers easier.

It is recommended you run StupidTally.exe within another active monitor.

Be sure to run StupidTally as Administrator after the game has been opened and gameplay begins if keybindings do not seem to work.

### **(Ba)** "Default Bindings"
To reset to default bindings, simply delete the StupidTally.ini file (which exists in the same root directory as the EXE).

This will create a fresh, new INI file with the default bindings.

**The default bindings are:**

	ALT+SHIFT+# 
		to start typing a damage number.
	ALT+SHIFT+END
		to finish typing a damage number and tally it.
		if it doesn't exist, it will be added to the data table on the right.
		if it already exists, it will increase its tally.
	ALT+SHIFT+DEL
		to clear the typed damage number and start over.
	CTRL+SHIFT+S
		to export the data table to a CSV file 
		(to be used as spreadsheet)
		and to export the bar graph to a PNG
	CTRL+SHIFT+O
		to import the data table from a CSV file
		(to resume testing from a previous session)
### **(Ba) (Bb)** "Changing the Global Keybindings"
1. Simply click any cell within **(Ba)**`List of Shortcuts` to select the keybinding you wish to change.

2. Next, click on the **(Bb)** `Record button`. 
3. Hold the keys you'd like to use for this keybinding.
4. The **(Bc)** preview box will show what you currently have held down.
5. When you are pleased with this key combination, press `Enter`, `Esc`, or click `Record` button to end the recording and save this keybinding.

These settings are then saved to the StupidTally.ini file in the tool's root directory.

_Note: for `TypeDigitModifier`, only a combination of Modifier keys (Alt, Shift, Ctrl) are allowed._

_The actual key binding will be every digit (0-9) and the chosen Modifier keys._

_Ex: If the TypeDigitModifier is simply `Alt`, to type in a damage number `123`, you'd simply hold `Alt` and type out `1` and `2` and `3`. Then to accept this damage number, you'd use the keybinding for `AcceptNumber`._


# **Section C** - "Damage: Data Capture"
## **(Ca)** "Damage: Data table"
This table shows the X,Y columns:
* X: "Damage"
	
	One of the damage numbers you have recorded
* Y: "Tally"
	
	The number of times this damage number was recorded
## **(Cb)** "Damage: Bar Graph (Histogram)"
This shows a bar graph of the `damage numbers` for the bottom `X-axis`, and the `Tally` as the vertical `Y-axis`.
Above each bar, a percentage will be shown.
This percentage represents how often this damage number appeared out of the total damage numbers entered.

This helps with mapping the distribution to a spread map of known dice rolls.

The **(Cb)** `Total` number can help determine how many dice there may be. The distribution bar graph can help determine if how many dice may have been used to create the spread of damage numbers on the game.


# Troubleshooting
Since this program is unsigned and written by a fk idot, yours truly,
I suspect you may need to add this program's executable to Windows' list of ignored processes in security and virus protection.

If you encounter errors or problems, raise a ticket on the source repository: https://github.com/DDews/StupidTally.git
