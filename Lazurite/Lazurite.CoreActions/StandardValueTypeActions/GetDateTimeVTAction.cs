﻿using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.Attributes;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.Shared.ActionCategory;
using System;

namespace Lazurite.CoreActions.StandardValueTypeActions
{
    [VisualInitialization]
    [HumanFriendlyName("# Дата и время")]
    [SuitableValueTypes(typeof(DateTimeValueType))]
    [Category(Category.Meta)]
    public class GetDateTimeVTAction : IAction, IStandardValueAction
    {
        public GetDateTimeVTAction()
        {
            Value = DateTime.Now.ToString();
        }

        public string Caption
        {
            get
            {
                return Value;
            }
            set
            {
                //
            }
        }

        public string Value
        {
            get;
            set;
        }
        
        public ValueTypeBase ValueType
        {
            get;
            set;
        } = new DateTimeValueType();

        public bool IsSupportsEvent
        {
            get
            {
                return false;
            }
        }

        public bool IsSupportsModification
        {
            get
            {
                return true;
            }
        }

        public void Initialize()
        {
            //
        }

        public bool UserInitializeWith(ValueTypeBase valueType, bool inheritsSupportedValues)
        {
            return true;
        }

        public string GetValue(ExecutionContext context)
        {
            return Value;
        }

        public void SetValue(ExecutionContext context, string value)
        {
            Value = value;
        }

#pragma warning disable 67
        public event ValueChangedEventHandler ValueChanged;
#pragma warning restore 67
    }
}