﻿// // <copyright file="SettingsData.cs" company="PublicDomainWeekly.com">
// //     CC0 1.0 Universal (CC0 1.0) - Public Domain Dedication
// //     https://creativecommons.org/publicdomain/zero/1.0/legalcode
// // </copyright>
// // <auto-generated />

namespace PublicDomainWeekly
{
    // Directives
    using System.Collections.Generic;
    using System.Drawing;

    /// <summary>
    /// Urlister settings.
    /// </summary>
    public class SettingsData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:PublicDomain.SettingsData"/> class.
        /// </summary>
        public SettingsData()
        {
            // Parameterless constructor
        }

        /// <summary>
        /// The top most.
        /// </summary>
        public bool TopMost { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:PublicDomainWeekly.SettingsData"/> is control.
        /// </summary>
        /// <value><c>true</c> if control; otherwise, <c>false</c>.</value>
        public bool Control { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:PublicDomainWeekly.SettingsData"/> is alternate.
        /// </summary>
        /// <value><c>true</c> if alternate; otherwise, <c>false</c>.</value>
        public bool Alt { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:PublicDomainWeekly.SettingsData"/> is shift.
        /// </summary>
        /// <value><c>true</c> if shift; otherwise, <c>false</c>.</value>
        public bool Shift { get; set; } = false;

        /// <summary>
        /// Gets or sets the hotkey.
        /// </summary>
        /// <value>The hotkey.</value>
        public string Hotkey { get; set; } = "None";

        /// <summary>
        /// Gets or sets the delay milliseconds.
        /// </summary>
        /// <value>The delay milliseconds.</value>
        public int DelayMilliseconds { get; set; } = 75;

        /// <summary>
        /// Gets or sets the comma translation.
        /// </summary>
        /// <value>The comma translation.</value>
        public string CommaTranslation { get; set; } = "{TAB}";

        /// <summary>
        /// Gets or sets the new line translation.
        /// </summary>
        /// <value>The new line translation.</value>
        public string NewLineTranslation { get; set; } = "{ENTER}";

        /// <summary>
        /// Gets or sets the input file.
        /// </summary>
        /// <value>The input file.</value>
        public string InputFile { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        /// <value>The index.</value>
        public int CaretIndex { get; set; } = 1;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:PublicDomain.SettingsData"/> enable hotkeys.
        /// </summary>
        /// <value><c>true</c> if enable hotkeys; otherwise, <c>false</c>.</value>
        public bool EnableHotkeys { get; set; } = true;
    }
}