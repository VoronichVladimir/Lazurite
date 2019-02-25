﻿using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.Attributes;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.IOC;
using Lazurite.MainDomain;
using Lazurite.Shared.ActionCategory;

namespace Lazurite.CoreActions.CoreActions
{
    [HumanFriendlyName("Значение существующего сценария")]
    [VisualInitialization]
    [OnlyGetValue]
    [SuitableValueTypes(true)]
    [Category(Category.Meta)]
    public class GetExistingScenarioValueAction : IScenariosAccess, IAction
    {
        private static readonly ISystemUtils SystemUtils = Singleton.Resolve<ISystemUtils>();

        private static readonly UsersRepositoryBase UsersRepository = Singleton.Resolve<UsersRepositoryBase>();

        private static readonly ScenarioActionSource ScenarioActionSource = new ScenarioActionSource(UsersRepository.SystemUser, ScenarioStartupSource.OtherScenario, ScenarioAction.ViewValue);

        public string TargetScenarioId
        {
            get; set;
        }

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

        private ScenarioBase _scenario;

#pragma warning disable 67

        public event ValueChangedEventHandler ValueChanged;

#pragma warning restore 67

        public void SetTargetScenario(ScenarioBase scenario)
        {
            _scenario = scenario;
        }

        public ScenarioBase GetTargetScenario()
        {
            return _scenario;
        }

        public string Caption
        {
            get
            {
                return _scenario?.Name ?? "[сценарий не выбран]";
            }
            set
            {
                //
            }
        }

        public ValueTypeBase ValueType
        {
            get
            {
                return _scenario?.ValueType ?? new ButtonValueType();
            }
            set
            {
                //
            }
        }

        public void Initialize()
        {
            //do nothing
        }

        public bool UserInitializeWith(ValueTypeBase valueType, bool inheritsSupportedValues)
        {
            return false;
        }

        public string GetValue(ExecutionContext context)
        {
            if (_scenario != null)
            {
                if (_scenario.GetInitializationState() == ScenarioInitializationValue.NotInitialized)
                {
                    _scenario.FullInitialize().Wait();
                }
                else
                {
                    while (_scenario.GetInitializationState() == ScenarioInitializationValue.Initializing)
                    {
                        SystemUtils.Sleep(100, context.CancellationTokenSource);
                    }
                }

                return _scenario.CalculateCurrentValue(ScenarioActionSource, context);
            }
            return string.Empty;
        }

        public void SetValue(ExecutionContext context, string value)
        {
            //
        }
    }
}