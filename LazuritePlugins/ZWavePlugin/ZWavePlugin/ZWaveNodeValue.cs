﻿using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.Attributes;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.Shared.ActionCategory;
using LazuriteUI.Icons;
using OpenZWrapper;
using System;
using System.Linq;
using ZWPluginUI;

namespace ZWavePlugin
{
    [HumanFriendlyName("ZWave устройство")]
    [SuitableValueTypes(
        typeof(StateValueType), typeof(InfoValueType), typeof(FloatValueType), 
        typeof(ButtonValueType), typeof(ToggleValueType))]
    [LazuriteIcon(Icon.ManSensor)]
    [Category(Category.Control)]
    public class ZWaveNodeValue : IAction, IDisposable
    {
        public byte NodeId { get; set; }
        public uint HomeId { get; set; }
        public ulong ValueId { get; set; }
        
        private NodeValue _nodeValue;

        public string Caption
        {
            get => _nodeValue?.Node.ProductName + " -> " + _nodeValue?.Name + " (ID=" + _nodeValue?.Id + ")";
            set { }
        }
        
        public ValueTypeBase ValueType
        {
            get;
            set;
        }

        public bool IsSupportsEvent => true;

        public bool IsSupportsModification => true;

        public event ValueChangedEventHandler ValueChanged;

        public string GetValue(ExecutionContext context)
        {
            if (_nodeValue != null)
                return _nodeValue.Current.ToString();
            throw new InvalidOperationException(string.Format("Узел не загружен или не существует. Возможно он будет загружен позднее. HomeID={0}, NodeID={1}, ValueID={2}", HomeId, NodeId, ValueId));
        }

        public void Initialize()
        {
            ZWaveManager.Current.NodeValueLoaded -= Current_NodeValueLoaded; //crutch
            ZWaveManager.Current.WaitForInitialized();
            var nodes = ZWaveManager.Current.GetNodes();
            var node = nodes.FirstOrDefault(x => x.Id.Equals(NodeId) && x.HomeId == HomeId);
            _nodeValue = node?.Values.FirstOrDefault(x => x.Id.Equals(ValueId));
            if (_nodeValue != null)
                _nodeValue.Changed += NodeValue_Changed;
            else
                ZWaveManager.Current.NodeValueLoaded += Current_NodeValueLoaded;
        }

        private void Current_NodeValueLoaded(object sender, Lazurite.Shared.EventsArgs<NodeValue> args)
        {
            var nodeValue = args.Value;
            if (nodeValue.Id == ValueId && nodeValue.Node.Id == NodeId && nodeValue.Node.HomeId == HomeId)
            {
                _nodeValue = args.Value;
                _nodeValue.Changed += NodeValue_Changed;
                ValueChanged?.Invoke(this, _nodeValue.Current.ToString());
                ZWaveManager.Current.NodeValueLoaded -= Current_NodeValueLoaded;
            }
        }

        private void NodeValue_Changed(object arg1, NodeValueChangedEventArgs arg2)
        {
            ValueChanged?.Invoke(this, _nodeValue.Current.ToString());
        }

        public void SetValue(ExecutionContext context, string value)
        {
            if (_nodeValue != null)
            {
                if (_nodeValue.ValueType == OpenZWrapper.ValueType.Bool)
                    _nodeValue.Current = value == ToggleValueType.ValueON;
                else if (_nodeValue.ValueType == OpenZWrapper.ValueType.Byte ||
                    _nodeValue.ValueType == OpenZWrapper.ValueType.Decimal ||
                    _nodeValue.ValueType == OpenZWrapper.ValueType.Int ||
                    _nodeValue.ValueType == OpenZWrapper.ValueType.Short)
                    _nodeValue.Current = TranslateNumric(value, _nodeValue.ValueType);
                else
                    _nodeValue.Current = value;
            }
            else
                throw new InvalidOperationException(string.Format("Узел не загружен или не существует. Возможно он будет загружен позднее. HomeID ={0}, NodeID={1}, ValueID={2}", HomeId, NodeId, ValueId));
        }

        private object TranslateNumric(string value, OpenZWrapper.ValueType valueType)
        {
            if (valueType == OpenZWrapper.ValueType.Byte ||
                valueType == OpenZWrapper.ValueType.Int ||
                valueType == OpenZWrapper.ValueType.Short ||
                valueType == OpenZWrapper.ValueType.Decimal)
            {
                var valueNum = double.Parse(value);
                var range = OpenZWrapper.Utils.GetRangeFor(valueType);
                if (valueType == OpenZWrapper.ValueType.Byte)
                    return (byte)TranslateByMargin(Math.Round(valueNum,0), (byte)range.Min, (byte)range.Max);
                if (valueType == OpenZWrapper.ValueType.Int)
                    return (int)TranslateByMargin(Math.Round(valueNum, 0), (int)range.Min, (int)range.Max);
                if (valueType == OpenZWrapper.ValueType.Short)
                    return (short)TranslateByMargin(Math.Round(valueNum, 0), (short)range.Min, (short)range.Max);
                if (valueType == OpenZWrapper.ValueType.Decimal)
                    return (decimal)TranslateByMargin(valueNum, (double)range.Min, (double)range.Max);
            }
            return value;
        }

        private double TranslateByMargin(double val, double min, double max)
        {
            if (val > max)
                return max;
            if (val < min)
                return min;
            return val;
        }

        public bool UserInitializeWith(ValueTypeBase valueType, bool inheritsSupportedValueTypes)
        {
            var manager = ZWaveManager.Current;
            var window = new MainWindow();
            window.RefreshWith(manager, _nodeValue, (nodeValue) => ZWaveTypeComparability.IsTypesComparable(nodeValue, valueType, inheritsSupportedValueTypes));
            if (window.ShowDialog() ?? false)
            {
                if (_nodeValue != null)
                    _nodeValue.Changed -= NodeValue_Changed;
                _nodeValue = window.GetSelectedNodeValue();
                NodeId = _nodeValue.Node.Id;
                HomeId = _nodeValue.Node.HomeId;
                ValueId = _nodeValue.Id;
                ValueType = ZWaveTypeComparability.CreateValueTypeFromNodeValue(_nodeValue);
                _nodeValue.Changed += NodeValue_Changed;
                return true;
            }
            else return false;
        }

        public void Dispose()
        {
            if (_nodeValue == null)
                ZWaveManager.Current.NodeValueLoaded -= Current_NodeValueLoaded;
        }
    }
}
