using Keystroke.API;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Backend
{
    // Sends commands to applications
    public class PasswordWriter
    {
        private readonly XmlRootElement xmlRootElement;
        private readonly KeyboardStateTracker stateTracker;
        private readonly GateWithTimer gateWithTimer;
        private static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        // Ex. "some value [00201] -> 00201"
        public static readonly Regex RegexValueInSquareBreakets = new Regex(@"(?<=\[).+?(?=\])", RegexOptions.Singleline 
            | RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private KeyboardHandler keyboardHandler;

        public PasswordWriter(XmlRootElement xmlRootElement, KeyboardStateTracker stateTracker)
        {
            this.xmlRootElement = xmlRootElement;
            this.stateTracker = stateTracker;
            this.gateWithTimer = new GateWithTimer(1000);
            try
            {
                this.keyboardHandler = new KeyboardHandler(this.stateTracker);
            }
            catch (Exception e)
            {
                int i = 0;
                i++;
            }
        }

        public string RunCommand(SettingsModel settingsModel)
        {
            string fullCommand = string.Join("", settingsModel.Password);
            if (!gateWithTimer.Open(fullCommand))
            {
                return null;
            }
            int pointer = 0;
            var command = NextCommand(fullCommand, ref pointer);
            while (command != null)
            {
                // Sending key
                var succcess = keyboardHandler.SendKey(command);
                if (!succcess)
                {
                    log.Info("Error! Key was not parsed");
                }
                command = NextCommand(fullCommand, ref pointer);
            }
            return fullCommand;
        }

        public void RunCommandForActiveWindow(string applicationTitle)
        {
            string fullCommand = null;
            foreach (var settingsModel in xmlRootElement.SettingsModels) 
            {
                if (applicationTitle.Contains(settingsModel.ApplicationName))
                {
                    log.Info($"Found task for window {settingsModel.ApplicationName}");
                    fullCommand = RunCommand(settingsModel);
                }

                if (!string.IsNullOrEmpty(fullCommand))
                {
                    log.Info("Finished running task");
                }
                else 
                {
                    log.InfoFormat($"Task for window {applicationTitle} was not found");
                }
            }
        }

        public bool RunShortcutCommand()
        {
            string fullCommand = null;
            var noAppTitle = xmlRootElement.SettingsModels.Where(settingsModel => string.IsNullOrEmpty(settingsModel.ApplicationName));
            foreach (var settingsModel in noAppTitle)
            {
                if (keyboardHandler.IsManualKeyCombinationPressed(settingsModel.Hotkey))
                {
                    log.Info($"Detected hotkey combination task. Combination = {settingsModel.Hotkey}");
                    fullCommand = RunCommand(settingsModel);
                }

            }
            if (!string.IsNullOrEmpty(fullCommand))
            {
                log.Info("Finished running hotkey task");
            }
            return fullCommand != null;
        }

        private string NextCommand(string fullCommand, ref int pointer)
        {
            if (string.IsNullOrEmpty(fullCommand))
            {
                return fullCommand;
            }
            if (pointer > 0 || fullCommand.Length <= pointer)
            {
                // Index is out of boundaries of the array
                return null;
            }
            var startIndex = pointer;
            if (fullCommand[pointer] == '<')
            {
                // special command
                for (; pointer < fullCommand.Length && fullCommand[pointer] != '>'; pointer++)
                {
                    // move pointer to the right
                }
            }
            var length = pointer + 1 - startIndex;
            pointer++;
            return fullCommand.Substring(startIndex, length);
        }
    }
}
