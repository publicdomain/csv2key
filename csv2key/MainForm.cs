﻿// <copyright file="MainForm.cs" company="PublicDomainWeekly.com">
//     CC0 1.0 Universal (CC0 1.0) - Public Domain Dedication
//     https://creativecommons.org/publicdomain/zero/1.0/legalcode
// </copyright>

namespace csv2key
{
    // Directives
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;
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
        }

        /// <summary>
        /// Handles the csv file browse button click.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnCsvFileBrowseButtonClick(object sender, EventArgs e)
        {
            // TODO Add code
        }

        /// <summary>
        /// GUIs to settings sata.
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

            // Lines per press
            this.settingsData.LinesPerPress = Convert.ToInt32(this.linesNumericUpDown.Value);

            // Active/Inactive
            this.settingsData.EnableHotkeys = this.activeRadioButton.Checked;
        }

        /// <summary>
        /// Settingses the data to GUI.
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

            // Lines per press
            this.linesNumericUpDown.Value = this.settingsData.LinesPerPress;


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
        /// Handles the radio button click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnRadioButtonClick(object sender, EventArgs e)
        {
            /* TODO Improve Register/Unregister of hotkeys [This can be made more efficient by keeping a good track, especially with recurring calls due to checkboxes, et cetera] */

            /* TODO // Set bold font
            this.activeRadioButton.Font = new Font(this.activeRadioButton.Font, this.activeRadioButton.Checked ? FontStyle.Bold : FontStyle.Regular);
            this.inactiveRadioButton.Font = new Font(this.inactiveRadioButton.Font, !this.activeRadioButton.Checked ? FontStyle.Bold : FontStyle.Regular);
            */

            // Try to unregister the key
            try
            {
                // Unregister the hotkey
                UnregisterHotKey(this.Handle, 0);
            }
            catch (Exception ex)
            {
                // Let it fall through
            }

            // Try to register the key
            try
            {
                if (this.activeRadioButton.Checked && this.keyComboBox.SelectedItem.ToString() != "None")
                {
                    // Register the hotkey
                    RegisterHotKey(this.Handle, 0, (this.controlCheckBox.Checked ? MOD_CONTROL : 0) + (this.altCheckBox.Checked ? MOD_ALT : 0) + (this.shiftCheckBox.Checked ? MOD_SHIFT : 0), Convert.ToInt16((Keys)Enum.Parse(typeof(Keys), this.keyComboBox.SelectedItem.ToString(), true)));
                }
            }
            catch (Exception ex)
            {
                // Let it fall through
            }
        }

        /// <summary>
        /// Windows the proc.
        /// </summary>
        /// <param name="m">M.</param>
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WM_HOTKEY)
            {
                // Start all in list
                /*foreach (var item in this.programListBox.Items)
                {
                    // Split by tab
                    var commandArgument = item.ToString().Split("\t".ToCharArray());

                    try
                    {
                        // TODO Start via process [Can be improcess i.e. by StartInfo]
                        if (commandArgument.Length == 1)
                        {
                            // Command
                            Process.Start(commandArgument[0]);
                        }
                        else
                        {
                            // Command + arguments
                            Process.Start(commandArgument[0], commandArgument[1]);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Advise user
                        MessageBox.Show($"Command: {commandArgument[0]}{Environment.NewLine}{(commandArgument.Length > 1 ? $"Arguments: {commandArgument[1]}{Environment.NewLine}" : string.Empty)}{Environment.NewLine}Message:{Environment.NewLine}{ex.Message}", "Process start eror", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }*/
            }
        }


        /// <summary>
        /// Handles the new tool strip menu item click.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnNewToolStripMenuItemClick(object sender, EventArgs e)
        {
            // TODO Add code
        }

        /// <summary>
        /// Handles the open tool strip menu item click.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnOpenToolStripMenuItemClick(object sender, EventArgs e)
        {
            // TODO Add code
        }

        /// <summary>
        /// Handles the exit tool strip menu item click.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnExitToolStripMenuItemClick(object sender, EventArgs e)
        {
            // TODO Add code
        }

        /// <summary>
        /// Handles the options tool strip menu item drop down item clicked.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnOptionsToolStripMenuItemDropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            // TODO Add code
        }

        /// <summary>
        /// Handles the weekly releases public domain weeklycom tool strip menu item click.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnWeeklyReleasesPublicDomainWeeklycomToolStripMenuItemClick(object sender, EventArgs e)
        {
            // TODO Add code
        }

        /// <summary>
        /// Handles the original thread donation codercom tool strip menu item click.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnOriginalThreadDonationCodercomToolStripMenuItemClick(object sender, EventArgs e)
        {
            // TODO Add code
        }

        /// <summary>
        /// Handles the source code githubcom tool strip menu item click.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnSourceCodeGithubcomToolStripMenuItemClick(object sender, EventArgs e)
        {
            // TODO Add code
        }

        /// <summary>
        /// Handles the about tool strip menu item click.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnAboutToolStripMenuItemClick(object sender, EventArgs e)
        {
            // TODO Add code
        }

        /// <summary>
        /// Handles the main form load.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnMainFormLoad(object sender, EventArgs e)
        {
            // TODO Add code
        }

        /// <summary>
        /// Handles the main form form closing.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnMainFormFormClosing(object sender, FormClosingEventArgs e)
        {
            // TODO Add code
        }

        /// <summary>
        /// Handles the active radio button checked changed.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnActiveRadioButtonCheckedChanged(object sender, EventArgs e)
        {
            // TODO Add code
        }

        /// <summary>
        /// Handles the inactive radio button checked changed.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnInactiveRadioButtonCheckedChanged(object sender, EventArgs e)
        {
            // TODO Add code
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


    }
}
