using Ionic.Zip;
using Keystroke.API;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
//using System.IO.Compression;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace Backend
{
    // Root of the logic. Manages high level tasks like sending and listening of keyboard
    // but details are implemented in low level classes
    public class PasswordManager
    {
        private readonly string UserSettingFile;
        private readonly string ArchiveFile;
        private static readonly log4net.ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private string masterPassword;
        private SettingsModel activeRow;
        public delegate void SettingsUpdatedDelegate();
        public event SettingsUpdatedDelegate SettingsUpdated;
        private KeyboardListener api;
        private XmlRootElement xmlRootElement = new XmlRootElement();
        private PasswordWriter passwordWriter;
        private KeyboardStateTracker keyboardStateTracker;

        public PasswordManager()
        {
            UserSettingFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "PasswordManagerSettings.xml");
            ArchiveFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "PasswordManagerSettings.zip");
            keyboardStateTracker = new KeyboardStateTracker();
        }

        public XmlRootElement XmlRootElement 
        {
            get => xmlRootElement;
            set 
            {
                xmlRootElement = value;
                OnXmlRootElementChanged();
            }
        }

        private void OnXmlRootElementChanged()
        {
            passwordWriter = new PasswordWriter(xmlRootElement, keyboardStateTracker);
        }

        public void SaveSettingsInArchive()
        {
            using (var zip = new ZipFile())
            {
                if (!string.IsNullOrEmpty(masterPassword))
                {
                    zip.Password = masterPassword;
                }

                var mySerializer = new XmlSerializer(typeof(XmlRootElement));
                using (var ms = new StringWriter())
                {
                    mySerializer.Serialize(ms, XmlRootElement);
                    var strXmlRootElement = ms.ToString();

                    zip.AddEntry("PasswordManagerSettings.xml", strXmlRootElement);
                    zip.Save(ArchiveFile);
                }
            }
        }

        public void OpenSettings(string masterPassword)
        {
            this.masterPassword = masterPassword;
            log.Info("Opening user settings file...");
            if (!File.Exists(ArchiveFile))
            {
                CreateSettingsFile();
            }

            try
            {
                using (ZipFile zip = ZipFile.Read(ArchiveFile))
                {
                    zip.Password = masterPassword;
                    using (var ms = new MemoryStream())
                    {
                        foreach (ZipEntry zipEntry in zip)
                        {
                            zipEntry.Extract(ms);
                            ms.Position = 0;
                            var sr = new StreamReader(ms);
                            var serializer = new XmlSerializer(typeof(XmlRootElement));
                            XmlRootElement = (XmlRootElement)serializer.Deserialize(sr);

                            //Without this during deseriallization whitespaces will be lost. Empty tag in xml means space for deserialization
                            foreach (var settingsModel in XmlRootElement.SettingsModels)
                            {
                                settingsModel.Password = settingsModel.Password.Select(RestoreWhiteSpaces).ToList();
                            }
                        }
                    }
                }
                log.Info("Success!");
            }
            catch (BadPasswordException)
            {
                throw;
            }
            catch (Exception e)
            {
                log.Error(e.Message);
            }
        }

        private string RestoreWhiteSpaces(string listItem)
        {
            return string.IsNullOrEmpty(listItem) ? " " : listItem;
        }

        private void CreateSettingsFile()
        {
            log.Info("User settings file does not exist. Createing a new one.");
            try
            {
                // Create empty file
                XmlRootElement = new XmlRootElement() { SettingsModels = new List<SettingsModel>() };
                SaveSettingsInArchive();
                log.Info("Success!");
            }
            catch (Exception e)
            {
                log.Info(e.Message);
                throw;
            }
        }

        public void DeleteSettings()
        {
            try
            {
                log.Info("Deleting file " + ArchiveFile);
                File.Delete(ArchiveFile);
                log.Info("Success!");
            }
            catch (Exception e)
            {
                log.Error(e.Message);
            }
        }

        public void ListenKeyboard()
        {
            this.api = new KeyboardListener();
            log.Info("Start listening keyboard");
            api.CreateKeyboardHook(OnKeyPressed, null);
            log.Info("Success!");
        }

        public void TrackKeyboardState()
        {
            log.Info("Start tracking pressed key");
            this.keyboardStateTracker.StartTracking();
            log.Info("Success!");
        }

        public void NewPassword()
        {
            log.Info("Creating new manual row");
            log.Info("Make sure you enter valid hotkey. Example: Ctrl+Shift+B");
            var newRow = PasswordReader.NewEmptyRow();
            XmlRootElement.SettingsModels.Add(newRow);
            SaveSettingsInArchive();
            SettingsUpdated?.Invoke();
        }

        public void StopListeningKeyboark()
        {
            this.api.Dispose();
            log.Info("Stoped listening keyboard");
        }

        private bool OnKeyPressed(KeyPressed character)
        {
            log.Info(character.KeyCode + " " + character.CurrentWindow);

            var handled = false;
            if (character.CurrentWindow == "[Password Manager]")
            {
                // Password manager avoids sending keys to itself
                return handled;
            }

            try
            {
                switch (character.KeyCode)
                {
                    case KeyCode.Pause:
                    case KeyCode.NumLock:
                        if (character.ShiftPressed)
                        {
                            if (activeRow == null)
                            {
                                StartRecord(character);
                            }
                            else
                            {
                                StopRecord();
                            }
                        }
                        else
                        {
                            // Play
                            log.Info("Start playback");
                            passwordWriter.RunCommandForActiveWindow(character.CurrentWindow);
                        }
                        handled = true;
                        break;
                    default:
                        if (activeRow != null)
                        {
                            activeRow.RecordKeyStroke(character);
                        }
                        else
                        {
                            passwordWriter.RunShortcutCommand();
                        }
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return handled;
        }

        private void StartRecord(KeyPressed character)
        {
            log.Info("Start recording.");
            log.Info("Press Shift + Pause/Break to stop recording.");
            activeRow = PasswordReader.NewRow(character);
        }

        private void StopRecord()
        {
            if (activeRow != null)
            {
                XmlRootElement.SettingsModels.Add(activeRow);
                activeRow = null;
                SaveSettingsInArchive();
                SettingsUpdated?.Invoke();
            }
        }
    }
}
