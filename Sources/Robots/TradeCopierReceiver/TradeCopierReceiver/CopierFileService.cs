using System;
using System.IO;

namespace cAlgo
{
    public class CopierFileService
    {
        private const string TerminalRelativePath = "MetaQuotes\\Terminal";
        private const string TradeCopierFileName = "tradecopier.tc";
        private readonly TradeCopierReceiver _robot;
        private DateTime _fileDoesNotExistSince;
        private bool _fileExists = true;
        private TimeSpan _maxTimeFileMayNotExist;

        public CopierFileService(TradeCopierReceiver robot)
        {
            _robot = robot;
            UpdateTradeCopierFilePath();
            UpdateFileExists();
        }

        public string FilePath { get; private set; }

        public DateTime LastWriteTime { get; private set; }

        public bool CanProcessFile
        {
            get { return _fileExists; }
        }

        private void UpdateTradeCopierFilePath()
        {
            var terminalFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                              TerminalRelativePath);

            var copierId = _robot.CopierID;
            var expectedTradeCopierFileName = string.Format("{0}_{1}", copierId, TradeCopierFileName);

            var files = Directory.GetFiles(terminalFolder, expectedTradeCopierFileName, SearchOption.AllDirectories);
            if (files.Length == 0)
            {
                _robot.Print("Couldn't find trade copier file with ID: " + copierId);
            }
            else if (files.Length > 1)
            {
                _robot.Print("Found more than one trade copier file with ID: " + copierId);
            }
            else
            {
                FilePath = files[0];
                _robot.Print("Established connection with trade copier ID: " + copierId);
                _robot.Print(files[0]);
            }
        }

        public void UpdateState()
        {
            UpdateFileExists();

            if (CanProcessFile)
                LastWriteTime = File.GetLastWriteTime(FilePath);
        }

        private void UpdateFileExists()
        {
            var existsNewState = File.Exists(FilePath);
            if (_fileExists != existsNewState)
            {
                if (existsNewState)
                {
                    _robot.Print("Connection established");
                }
                else
                {
                    _robot.Print("Cannot find file");
                    _fileDoesNotExistSince = _robot.Server.Time;
                }

                _fileExists = existsNewState;
            }
            StopBotIfNeeded();
        }

        private void StopBotIfNeeded()
        {
            if (_fileExists)
                return;

            _maxTimeFileMayNotExist = TimeSpan.FromSeconds(5);
            if (_robot.Server.Time - _fileDoesNotExistSince > _maxTimeFileMayNotExist)
            {
                _robot.Print(string.Format("Couldn't find file for {0}s.", (int) _maxTimeFileMayNotExist.TotalSeconds));
                _robot.Stop();
            }
        }
    }
}