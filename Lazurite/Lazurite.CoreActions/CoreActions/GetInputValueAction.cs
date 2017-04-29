﻿using Lazurite.ActionsDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.MainDomain;
using Lazurite.ActionsDomain.Attributes;

namespace Lazurite.CoreActions.CoreActions
{
    [OnlyGetValue]
    [SuitableValueTypes(true)]
    [HumanFriendlyName("ВходящееЗначение")]
    [VisualInitialization]
    public class GetInputValueAction : IAction, ICoreAction
    {
        public string Caption
        {
            get
            {
                return "ВходящееЗначение";
            }
            set
            {
                //
            }
        }

        public bool IsSupportsEvent
        {
            get
            {
                return ValueChanged != null;
            }
        }

        public string TargetScenarioId
        {
            get; set;
        }

        public ValueTypeBase ValueType
        {
            get
            {
                return _scenario.ValueType;
            }

            set
            {
                //
            }
        }

        public ScenarioBase GetTargetScenario()
        {
            return _scenario;
        }

        public string GetValue(ExecutionContext context)
        {
            return context.Input;
        }

        public void Initialize()
        {
            //
        }

        private ScenarioBase _scenario;
        public void SetTargetScenario(ScenarioBase scenario)
        {
            _scenario = scenario;
        }

        public void SetValue(ExecutionContext context, string value)
        {
            //
        }

        public bool UserInitializeWith(ValueTypeBase valueType, bool inheritsSupportedValues)
        {
            return false;
        }

        public event ValueChangedDelegate ValueChanged;
    }
}