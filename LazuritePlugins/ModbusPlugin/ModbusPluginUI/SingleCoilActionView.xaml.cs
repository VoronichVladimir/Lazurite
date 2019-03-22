﻿using LazuriteUI.Windows.Controls;
using NModbusWrapper;
using System;
using System.Windows.Controls;

namespace ModbusPluginUI
{
    /// <summary>
    /// Логика взаимодействия для SingleCoilActionView.xaml
    /// </summary>
    public partial class SingleCoilActionView : UserControl
    {
        public SingleCoilActionView()
        {
            InitializeComponent();
            tbAddress.Validation = EntryViewValidation.UShortValidation(max: 247);
            tbCoil.Validation = EntryViewValidation.UShortValidation();
            
            btOk.Click += (o, e) => {
                _action.Manager.Transport = transportView.Transport;
                _action.CoilAddress = ushort.Parse(tbCoil.Text);
                _action.SlaveAddress = byte.Parse(tbAddress.Text);
                OkPressed?.Invoke(_action);
            };

            btCancel.Click += (o, e) => {
                CancelPressed?.Invoke();
            };
        }

        private IModbusSingleCoilAction _action;

        public void RefreshWith(IModbusSingleCoilAction action)
        {
            _action = action;
            transportView.RefreshWith(action.Manager.Transport);
            tbAddress.Text = action.SlaveAddress.ToString();
            tbCoil.Text = action.CoilAddress.ToString();
        }

        public Action<IModbusSingleCoilAction> OkPressed;
        public Action CancelPressed;
    }
}
