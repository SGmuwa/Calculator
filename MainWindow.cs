/*
    Copyright (c) 2011-2012, Måns Andersson <mail@mansandersson.se>

    Permission to use, copy, modify, and/or distribute this software for any
    purpose with or without fee is hereby granted, provided that the above
    copyright notice and this permission notice appear in all copies.

    THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES
    WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF
    MERCHANTABILITY AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR
    ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES
    WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN
    ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF
    OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.
*/
using System;
using System.Drawing;

namespace Calculator
{
    /// <summary>
    /// Main Window
    /// </summary>
    public partial class MainWindow
    {

        /// <summary>
        /// Operational mode for expression evaluation
        /// </summary>
        private CalculatorMode Mode { get; set; }

        private readonly ConsoleEmulator emulator = new ConsoleEmulator();

        /// <summary>
        /// Constructor, init form
        /// </summary>
        public MainWindow()
        {
            ClearLabels();
            this.Mode = CalculatorMode.Mathematics;
        }

        /// <summary>
        /// Clear all labels and set size to null
        /// </summary>
        private void ClearLabels()
        {
            emulator.txtResultDecimal = "";
            emulator.txtResultHex = "";
            emulator.txtResultBits = "";

            switch (this.Mode)
            {
                default:
                case CalculatorMode.Mathematics:
                    emulator.lblMode = "M";
                    break;
                case CalculatorMode.Programming:
                    emulator.lblMode = "P";
                    break;
            }
        }

        /// <summary>
        /// Text input has changed, re-evaluate and calculate
        /// </summary>
        /// <param name="sender">sending object</param>
        /// <param name="e">event arguments</param>
        private void txtInput_TextChanged(object sender, EventArgs e)
        {
            ClearLabels();
            if (String.IsNullOrWhiteSpace(emulator.txtInput))
                return;

            Calculator calc = new Calculator(emulator.txtInput);
            calc.Mode = this.Mode;
            double? result = null;
            try
            {
                result = calc.Calculate();
            }
            catch (Exception)
            {
                result = null;
            }

            if (result.HasValue)
            {
                Int64 intResult = (Int64)result;

                emulator.txtResultDecimal = result.ToString();

                if (result == ((double)intResult))
                {
                    // This is a whole number
                    emulator.txtResultHex = String.Join(" ", intResult.ToString("X").Reverse().SplitEveryNth(2)).Reverse();

                    emulator.txtResultBits = String.Join(" ", Convert.ToString(intResult, 2).Reverse().SplitEveryNth(4)).Reverse();
                }
                else
                {
                    emulator.txtResultHex = "";
                    emulator.txtResultBits = "";
                }
            }
        }

        /// <summary>
        /// Handle input of commands
        /// </summary>
        /// <param name="sender">calling object</param>
        /// <param name="e">event arguments</param>
        private void txtInput_KeyDown(object sender, ConsoleKeyInfo e)
        {
            if (e.Key == ConsoleKey.Enter)
            {
                bool handled = false;
                switch (emulator.txtInput)
                {
                    // Set to mathematics mode
                    case "m":
                        this.Mode = CalculatorMode.Mathematics;
                        handled = true;
                        break;
                    // Set to programming mode
                    case "p":
                        this.Mode = CalculatorMode.Programming;
                        handled = true;
                        break;
                    // Enter "save/store" mode
                    default:
                        break;

                }
                if (handled)
                    emulator.txtInput = "";
            }
        }

        private void txtStore_KeyDown(object sender, ConsoleKeyInfo e)
        {
            if (e.Key == ConsoleKey.Enter)
            {
                string varName = emulator.txtStore;
                string varCalculation = emulator.txtInput;

                if (varName.Length > 0)
                {
                    Variables.Instance.AddOrUpdateVariable(varName, varCalculation, this.Mode);
                    emulator.txtInput = varName;
                }
                else
                {
                    // error...
                }

                emulator.txtStore = String.Empty;
            }
        }
    }
}
