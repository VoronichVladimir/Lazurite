﻿using Lazurite.ActionsDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lazurite.ActionsDomain.ValueTypes;

namespace LazuriteUI.Windows.WpfTests
{
    public class TestDateTimeAction : IAction
    {
        public string Caption
        {
            get;

            set;
        }

        public bool IsSupportsEvent
        {
            get;
            set;
        }

        public ValueTypeBase ValueType
        {
            get
            {
                return new DateTimeValueType();
            }

            set
            {

            }
        }

        public event ValueChangedDelegate ValueChanged;

        private string _val = DateTime.Now.ToString();
        public string GetValue(ExecutionContext context)
        {
            return _val;
        }

        public void Initialize()
        {
        }

        public void SetValue(ExecutionContext context, string value)
        {
            _val = value;
        }

        public bool UserInitializeWith(ValueTypeBase valueType, bool inheritsSupportedValues)
        {
            return true;
        }
    }
}
