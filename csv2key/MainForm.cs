﻿// <copyright file="MainForm.cs" company="PublicDomainWeekly.com">
//     CC0 1.0 Universal (CC0 1.0) - Public Domain Dedication
//     https://creativecommons.org/publicdomain/zero/1.0/legalcode
// </copyright>

namespace csv2key
{
    // Directives
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Timers;
    using System.Windows.Forms;
    using System.Xml.Serialization;
    using PublicDomainWeekly;

    /// <summary>
    /// Description of MainForm.
    /// </summary>
    public partial class MainForm : Form
    {
        /// <summary>
        /// Gets or sets the associated icon.
        /// </summary>
        /// <value>The associated icon.</value>
        private Icon associatedIcon = null;

        /// <summary>
        /// The settings data.
        /// </summary>
        public SettingsData settingsData = null;

        /// <summary>
        /// The settings data path.
        /// </summary>
        private string settingsDataPath = $"{Application.ProductName}-SettingsData.txt";

        /// <summary>
        /// The hotkey timer.
        /// </summary>
        private System.Timers.Timer hotkeyTimer = new System.Timers.Timer();

        /// <summary>
        /// The index of the caret.
        /// </summary>
        private int caretIndex = 0;

        /// <summary>
        /// Registers the hot key.
        /// </summary>
        /// <returns><c>true</c>, if hot key was registered, <c>false</c> otherwise.</returns>
        /// <param name="hWnd">H window.</param>
        /// <param name="id">Identifier.</param>
        /// <param name="fsModifiers">Fs modifiers.</param>
        /// <param name="vk">Vk.</param>
        [DllImport("User32")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

        /// <summary>
        /// Unregisters the hot key.
        /// </summary>
        /// <returns><c>true</c>, if hot key was unregistered, <c>false</c> otherwise.</returns>
        /// <param name="hWnd">H window.</param>
        /// <param name="id">Identifier.</param>
        [DllImport("User32")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        /// <summary>
        /// The mod shift.
        /// </summary>
        private const int MOD_SHIFT = 0x4;

        /// <summary>
        /// The mod control.
        /// </summary>
        private const int MOD_CONTROL = 0x2;

        /// <summary>
        /// The mod alternate.
        /// </summary>
        private const int MOD_ALT = 0x1;

        /// <summary>
        /// The wm hotkey.
        /// </summary>
        private static int WM_HOTKEY = 0x0312;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:csv2key.MainForm"/> class.
        /// </summary>
        public MainForm()
        {
            // The InitializeComponent() call is required for Windows Forms designer support.
            this.InitializeComponent();

            // Add keys
            foreach (var key in Enum.GetValues(typeof(Keys)))
            {
                // Add to list box
                this.keyComboBox.Items.Add(key.ToString());
            }

            /* Set icons */

            // Set associated icon from exe file
            this.associatedIcon = Icon.ExtractAssociatedIcon(typeof(MainForm).GetTypeInfo().Assembly.Location);

            // Set public domain weekly tool strip menu item image
            this.weeklyReleasesPublicDomainWeeklycomToolStripMenuItem.Image = this.associatedIcon.ToBitmap();

            /* Process setiings */

            // Check for settings file
            if (!File.Exists(this.settingsDataPath))
            {
                // Create new settings file
                this.SaveSettingsFile(this.settingsDataPath, new SettingsData());
            }

            // Load settings from disk
            this.settingsData = this.LoadSettingsFile(this.settingsDataPath);

            // Set timer auto reset
            this.hotkeyTimer.AutoReset = false;

            // Set hotkey timer elapsed handler
            this.hotkeyTimer.Elapsed += new ElapsedEventHandler(OnHotkeyTimerElapsed);
        }

        /// <summary>
        /// Window procedure.
        /// </summary>
        /// <param name="m">M.</param>
        protected override void WndProc(ref Message m)
        {
            // Check for hotkey press
            if (m.Msg == WM_HOTKEY)
            {
                // Check there's something to work with
                if (this.csvLinesTextBox.TextLength == 0)
                {
                    // Advise user
                    MessageBox.Show("No lines to process!", "Empty", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                    // Halt flow
                    return;
                }

                /* Process hotkey press */

                // Act upon a disabled or enabled timer
                if (!this.hotkeyTimer.Enabled)
                {
                    // Try to parse delay/interval
                    if (!int.TryParse(this.delayComboBox.Text, out int parsedDelay))
                    {
                        // Set parsed delay
                        parsedDelay = 75;

                        // Set combo box to 75 ms
                        this.delayComboBox.Text = parsedDelay.ToString();
                    }

                    // Set interval to parsed delay
                    this.hotkeyTimer.Interval = parsedDelay;

                    // Start the timer
                    this.hotkeyTimer.Start();
                }
                else
                {
                    // Stop the timer
                    this.hotkeyTimer.Stop();
                }
            }
            else
            {
                // Process message
                base.WndProc(ref m);
            }
        }


        /// <summary>
        /// Handles the hotkey timer elapsed event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnHotkeyTimerElapsed(Object sender, ElapsedEventArgs e)
        {
            // TODO Add code
        }

        /// <summary>
        /// Handles the csv file browse button click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnCsvFileBrowseButtonClick(object sender, EventArgs e)
        {
            // Reset file name
            this.openFileDialog.FileName = string.Empty;

            // Show open file dialog
            if (this.openFileDialog.ShowDialog() == DialogResult.OK && this.openFileDialog.FileName.Length > 0)
            {
                try
                {
                    // Set file name
                    this.csvFileTextBox.Text = this.openFileDialog.FileName;

                    // Set CSV text 
                    this.csvLinesTextBox.Lines = File.ReadAllLines(this.openFileDialog.FileName);
                }
                catch (Exception exception)
                {
                    // Inform user
                    MessageBox.Show($"Error when opening \"{Path.GetFileName(this.openFileDialog.FileName)}\":{Environment.NewLine}{exception.Message}", "Open file error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// GUI to settings sata.
        /// </summary>
        private void GuiToSettingsSata()
        {
            // Topmost
            this.settingsData.TopMost = this.alwaysOnTopToolStripMenuItem.Checked;

            // Modifier checkboxes
            this.settingsData.Control = this.controlCheckBox.Checked;
            this.settingsData.Alt = this.altCheckBox.Checked;
            this.settingsData.Shift = this.shiftCheckBox.Checked;

            // Hotkey
            this.settingsData.Hotkey = this.keyComboBox.SelectedItem.ToString();

            // Delay
            this.settingsData.DelayMilliseconds = int.Parse(this.delayComboBox.Text);

            // Comma
            this.settingsData.CommaTranslation = this.commaTranslationTextBox.Text;

            // New line
            this.settingsData.NewLineTranslation = this.newLineTranslationTextBox.Text;

            // Input file
            this.settingsData.InputFile = this.csvFileTextBox.Text;

            // Caret index
            this.settingsData.CaretIndex = this.caretIndex;

            // Active/Inactive
            this.settingsData.EnableHotkeys = this.activeRadioButton.Checked;
        }

        /// <summary>
        /// Loads SettingsData to GUI.
        /// </summary>
        private void SettingsDataToGui()
        {
            // Topmost
            this.alwaysOnTopToolStripMenuItem.Checked = this.settingsData.TopMost;

            // Modifier checkboxes
            this.controlCheckBox.Checked = this.settingsData.Control;
            this.altCheckBox.Checked = this.settingsData.Alt;
            this.shiftCheckBox.Checked = this.settingsData.Shift;

            // Hotkey
            this.keyComboBox.SelectedItem = this.settingsData.Hotkey;

            // Delay
            this.delayComboBox.Text = this.settingsData.DelayMilliseconds.ToString();

            // Comma
            this.commaTranslationTextBox.Text = this.settingsData.CommaTranslation;

            // New line
            this.newLineTranslationTextBox.Text = this.settingsData.NewLineTranslation;

            // Input file
            this.csvFileTextBox.Text = this.settingsData.InputFile;

            // Caret index
            this.caretIndex = this.settingsData.CaretIndex;

            // Check active or inactive
            if (this.settingsData.EnableHotkeys)
            {
                // Check active
                this.activeRadioButton.Checked = true;
            }
            else
            {
                // Check inactive
                this.inactiveRadioButton.Checked = true;
            }
        }

        /// <summary>
        /// Handles the new tool strip menu item click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnNewToolStripMenuItemClick(object sender, EventArgs e)
        {
            // TODO Add code
        }

        /// <summary>
        /// Handles the options tool strip menu item drop down item clicked event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnOptionsToolStripMenuItemDropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            // Set tool strip menu item
            ToolStripMenuItem toolStripMenuItem = (ToolStripMenuItem)e.ClickedItem;

            // Toggle checked
            toolStripMenuItem.Checked = !toolStripMenuItem.Checked;

            // Set topmost by check box
            this.TopMost = this.alwaysOnTopToolStripMenuItem.Checked;
        }

        /// <summary>
        /// Handles the weekly releases public domain weeklycom tool strip menu item click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnWeeklyReleasesPublicDomainWeeklycomToolStripMenuItemClick(object sender, EventArgs e)
        {
            // Open our public domain website
            Process.Start("https://publicdomainweekly.com");
        }

        /// <summary>
        /// Handles the original thread donation codercom tool strip menu item click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnOriginalThreadDonationCodercomToolStripMenuItemClick(object sender, EventArgs e)
        {
            // Open original thread
            Process.Start("https://www.donationcoder.com/forum/index.php?topic=51658.0");
        }

        /// <summary>
        /// Handles the source code githubcom tool strip menu item click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnSourceCodeGithubcomToolStripMenuItemClick(object sender, EventArgs e)
        {
            // Open GitHub repository
            Process.Start("https://github.com/publicdomain/csv2key");
        }

        /// <summary>
        /// Handles the about tool strip menu item click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnAboutToolStripMenuItemClick(object sender, EventArgs e)
        {
            // TODO Add code
        }

        /// <summary>
        /// Handles the main form load event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnMainFormLoad(object sender, EventArgs e)
        {
            // Update GUI to reflect settings data
            this.SettingsDataToGui();

            // HACK Topmost on start [DEBUG]
            this.TopMost = this.settingsData.TopMost;

            // Load file into text box
            if (this.settingsData.InputFile.Length > 0 && File.Exists(this.settingsData.InputFile))
            {
                // Populate file text box
                this.csvLinesTextBox.Lines = File.ReadAllLines(this.settingsData.InputFile);
            }
        }

        /// <summary>
        /// Handles the main form form closing event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnMainFormFormClosing(object sender, FormClosingEventArgs e)
        {
            // Set settings from GUI
            this.GuiToSettingsSata();

            // Save to disk
            this.SaveSettingsFile(this.settingsDataPath, this.settingsData);
        }

        /// <summary>
        /// Handles the active radio button checked changed event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnActiveRadioButtonCheckedChanged(object sender, EventArgs e)
        {
            // make radio button bold
            this.inactiveRadioButton.Font = new Font(this.activeRadioButton.Font, FontStyle.Regular);
            this.activeRadioButton.Font = new Font(this.inactiveRadioButton.Font, FontStyle.Bold);

            // Register hotkey
            this.ProcessHotkeyRegistration(true);
        }


        /// <summary>
        /// Handles the inactive radio button checked changed event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnInactiveRadioButtonCheckedChanged(object sender, EventArgs e)
        {
            // make radio button bold
            this.activeRadioButton.Font = new Font(this.activeRadioButton.Font, FontStyle.Regular);
            this.inactiveRadioButton.Font = new Font(this.inactiveRadioButton.Font, FontStyle.Bold);

            // Unregister hotkey
            this.ProcessHotkeyRegistration(false);
        }

        /// <summary>
        /// Processes the hotkey registration.
        /// </summary>
        /// <param name="registerHotkey">If set to <c>true</c> register hotkey.</param>
        private void ProcessHotkeyRegistration(bool registerHotkey)
        {
            // Try to unregister the key
            try
            {
                // Unregister the hotkey
                UnregisterHotKey(this.Handle, 0);
            }
            catch
            {
                // Let it fall through
            }

            // Halt on unregister
            if (!registerHotkey)
            {
                // Halt flow
                return;
            }

            // Hotkey registration
            try
            {
                // Register the hotkey with selected modifiers
                RegisterHotKey(this.Handle, 0, (this.controlCheckBox.Checked ? MOD_CONTROL : 0) + (this.altCheckBox.Checked ? MOD_ALT : 0) + (this.shiftCheckBox.Checked ? MOD_SHIFT : 0), Convert.ToInt16((Keys)Enum.Parse(typeof(Keys), this.keyComboBox.SelectedItem.ToString(), true)));
            }
            catch
            {
                // Let it fall through
            }
        }

        /// <summary>
        /// Loads the settings file.
        /// </summary>
        /// <returns>The settings file.</returns>
        /// <param name="settingsFilePath">Settings file path.</param>
        private SettingsData LoadSettingsFile(string settingsFilePath)
        {
            // Use file stream
            using (FileStream fileStream = File.OpenRead(settingsFilePath))
            {
                // Set xml serialzer
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(SettingsData));

                // Return populated settings data
                return xmlSerializer.Deserialize(fileStream) as SettingsData;
            }
        }

        /// <summary>
        /// Saves the settings file.
        /// </summary>
        /// <param name="settingsFilePath">Settings file path.</param>
        /// <param name="settingsDataParam">Settings data parameter.</param>
        private void SaveSettingsFile(string settingsFilePath, SettingsData settingsDataParam)
        {
            try
            {
                // Use stream writer
                using (StreamWriter streamWriter = new StreamWriter(settingsFilePath, false))
                {
                    // Set xml serialzer
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(SettingsData));

                    // Serialize settings data
                    xmlSerializer.Serialize(streamWriter, settingsDataParam);
                }
            }
            catch (Exception exception)
            {
                // Advise user
                MessageBox.Show($"Error saving settings file.{Environment.NewLine}{Environment.NewLine}Message:{Environment.NewLine}{exception.Message}", "File error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Handles the csv lines text box text changed event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnCsvLinesTextBoxTextChanged(object sender, EventArgs e)
        {
            // Update line count
            this.lineCountToolStripStatusLabel.Text = this.csvLinesTextBox.Lines.Length.ToString();
        }

        /// <summary>
        /// Handles the check box checked changed event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnCheckBoxCheckedChanged(object sender, EventArgs e)
        {
            // Process hotkey
            this.ProcessHotkeyRegistration(this.activeRadioButton.Checked);
        }

        /// <summary>
        /// Handles the exit tool strip menu item click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnExitToolStripMenuItemClick(object sender, EventArgs e)
        {
            // Close program
            this.Close();
        }
    }
}
