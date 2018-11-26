﻿using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.Attributes;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.Shared.ActionCategory;
using LazuriteUI.Icons;
using RunProcessPluginUI;
using System;
using System.Diagnostics;

namespace RunProcessPlugin
{
    [LazuriteIcon(Icon.Console)]
    [HumanFriendlyName("Запуск программы")]
    [SuitableValueTypes(typeof(ToggleValueType))]
    [Category(Category.Administration)]
    public class RunProcessAction : IAction, IRunProcessAction
    {
        private Process _process;
        public string ExePath { get; set; } = string.Empty;
        public string Arguments { get; set; } = string.Empty;
        public CloseProcessMode CloseMode { get; set; } = CloseProcessMode.Kill;
        public ProcessPriorityClass Priority { get; set; } = ProcessPriorityClass.Normal;
        public bool CreateNoWindow { get; set; } = false;
        public ProcessWindowStyle Style { get; set; } = ProcessWindowStyle.Normal;

        public string Caption
        {
            get
            {
                return string.IsNullOrEmpty(ExePath) ? "[пусто]" : ExePath;
            }
            set
            {
                // do nothing
            }
        }

        public bool IsSupportsEvent => true;

        public bool IsSupportsModification => true;

        public ValueTypeBase ValueType
        {
            get;
            set;
        } = new ToggleValueType();

        public event ValueChangedEventHandler ValueChanged;

        public string GetValue(ExecutionContext context)
        {
            return _process == null || _process.HasExited ? ToggleValueType.ValueOFF : ToggleValueType.ValueON;                
        }

        public void Initialize()
        {
            //
        }

        public void SetValue(ExecutionContext context, string value)
        {
            if (value == ToggleValueType.ValueON)
                RunProcess();
            else
                CloseProcess();
            ValueChanged?.Invoke(this, GetValue(null));
        }

        public bool UserInitializeWith(ValueTypeBase valueType, bool inheritsSupportedValues)
        {
            return new MainWindow(this).ShowDialog() ?? false;
        }

        private void CloseProcess()
        {
            try
            {
                if (GetValue(null) != ToggleValueType.ValueOFF)
                {
                    switch (CloseMode)
                    {
                        case CloseProcessMode.Close:
                            _process.Close();
                            break;
                        case CloseProcessMode.CloseMainWindow:
                            _process.CloseMainWindow();
                            break;
                        case CloseProcessMode.Kill:
                            _process.Kill();
                            break;
                        default: throw new Exception("Not compatible close mode");
                    }
                    _process.Dispose();
                    _process = null;
                }
            }
            catch
            {
                //do nothing
            }
        }

        private void RunProcess()
        {
            var success = false;
            try
            {
                _process = new Process();
                _process.StartInfo.FileName = ExePath;
                _process.StartInfo.Arguments = Arguments;
                _process.StartInfo.CreateNoWindow = CreateNoWindow;
                _process.StartInfo.WindowStyle = Style;
                _process.EnableRaisingEvents = true;
                _process.Exited += (o, e) => ValueChanged?.Invoke(this, GetValue(null));
                success = _process.Start();
                try
                {
                    _process.PriorityClass = Priority;
                }
                catch
                {
                    //if current program runs as another user as program by "ExePath"
                }
            }
            catch
            {
                success = false;
            }
            if (!success)
                _process = null;
        }
    }
}