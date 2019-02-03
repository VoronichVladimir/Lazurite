﻿using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.Data;
using Lazurite.IOC;
using Lazurite.Logging;
using Lazurite.MainDomain;
using Lazurite.MainDomain.Statistics;
using Lazurite.Scenarios.ScenarioTypes;
using Lazurite.Shared;
using Lazurite.Windows.Statistics.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Lazurite.Windows.Statistics
{
    public class StatisticsManager : IStatisticsManager
    {
        private static readonly DataManagerBase DataManager = Singleton.Resolve<DataManagerBase>();
        private static readonly ScenariosRepositoryBase ScenariosRepository = Singleton.Resolve<ScenariosRepositoryBase>();
        private static readonly ISystemUtils SystemUtils = Singleton.Resolve<ISystemUtils>();
        private static readonly ILogger Log = Singleton.Resolve<ILogger>();
        private static readonly UsersRepositoryBase UsersRepository = Singleton.Resolve<UsersRepositoryBase>();
        private static readonly ScenarioActionSource SystemActionSource = new ScenarioActionSource(UsersRepository.SystemUser, ScenarioStartupSource.System, ScenarioAction.ViewValue);

        private DateTime RefreshDate = DateTime.Now.AddDays(1);
        private List<StatisticsScenarioInfoInternal> _statisticsScenariosInfos = new List<StatisticsScenarioInfoInternal>();
        private CancellationTokenSource _timerCancellationTokenSource;
        private StatisticsDataManager _dataManager = new StatisticsDataManager();
        private Dictionary<string, string> _scenariosValuesCache = new Dictionary<string, string>();

        public StatisticsManager()
        {
            LoadData();
            Initialize();
        }

        private void AddValueCache(string scenarioId, string value)
        {
            if (_scenariosValuesCache.ContainsKey(scenarioId))
                _scenariosValuesCache[scenarioId] = value;
            else _scenariosValuesCache.Add(scenarioId, value);
        }

        private string GetValueCache(string scenarioId)
        {
            if (_scenariosValuesCache.ContainsKey(scenarioId))
                return _scenariosValuesCache[scenarioId];
            else return string.Empty;
        }

        private void SaveData()
        {
            DataManager.Set(nameof(_statisticsScenariosInfos), _statisticsScenariosInfos);
        }

        private void LoadData()
        {
            if (DataManager.Has(nameof(_statisticsScenariosInfos)))
                _statisticsScenariosInfos = DataManager.Get<List<StatisticsScenarioInfoInternal>>(nameof(_statisticsScenariosInfos));
            else _statisticsScenariosInfos = new List<StatisticsScenarioInfoInternal>();
        }

        private void Initialize()
        {
            RegisterScenariosInternal();
            InitializeTimer();
        }

        private void InitializeTimer()
        {
            _timerCancellationTokenSource?.Cancel();
            _timerCancellationTokenSource = SystemUtils.StartTimer((c) => TimerTick(), () => 30000);
        }

        private string GetStartupSourceName(ScenarioStartupSource source)
        {
            switch(source)
            {
                case ScenarioStartupSource.Network:
                    return "Сеть";
                case ScenarioStartupSource.OtherScenario:
                    return "Другой сценарий";
                case ScenarioStartupSource.System:
                    return "Система";
                case ScenarioStartupSource.SystemUI:
                    return "Системный интерфейс";
            }

            throw new InvalidOperationException("Неизвестный источник запуска");
        }

        private void TimerTick()
        {
            //force update values
            if (DateTime.Now >= RefreshDate)
            {
                RefreshDate = DateTime.Now.AddDays(1);
                _scenariosValuesCache.Clear();
            }

            var statisticsScenarios = ScenariosRepository.Scenarios.Where(x => _statisticsScenariosInfos.Any(z => z.ScenarioId == x.Id)).ToArray();
            foreach (var scenario in statisticsScenarios.ToArray())
            {
                if (scenario.ValueType is ButtonValueType == false)
                {
                    var prevVal = GetValueCache(scenario.Id);
                    var newVal = scenario.CalculateCurrentValue(SystemActionSource, null);
                    if (newVal != prevVal)
                    {
                        AddValueCache(scenario.Id, newVal);
                        var item = new StatisticsItem();
                        item.Value = newVal;
                        item.DateTime = DateTime.Now;
                        item.Target = new StatisticsScenarioInfo()
                        {
                            ID = scenario.Id,
                            Name = scenario.Name,
                            ValueTypeName = ActionsDomain.Utils.GetValueTypeClassName(scenario.ValueType.GetType())
                        };
                        AddItem(item);
                    }
                }
            }
        }
        
        private void RegisterScenariosInternal()
        {
            var statisticsScenarios = ScenariosRepository.Scenarios.Where(x => _statisticsScenariosInfos.Any(z => z.ScenarioId == x.Id)).ToArray();
            foreach (var scenario in statisticsScenarios)
                RegisterInternal(scenario);
        }

        private void AddItem(StatisticsItem item)
        {
            try
            {
                var dataItem = new StatisticsDataItem(item.Source?.ID, item.Source?.Name, item.Source?.SourceType ?? "Система", item.Value, (byte)item.DateTime.Hour, (byte)item.DateTime.Minute, (byte)item.DateTime.Second);
                _dataManager.SetItem(item.Target.ID, item.Target.ValueTypeName, dataItem);
            }
            catch (Exception e)
            {
                Log.Error($"Ошибка во время записи значения сценария в файлы статистики. Сценарий: [{item.Target.ID}]", e);
            }
        }

        private StatisticsDataItem[] GetItems(string scenarioId, string valueTypeName, DateTime since, DateTime to)
        {
            try
            {
                return _dataManager.GetDataItems(scenarioId, valueTypeName, since, to);
            }
            catch (Exception e)
            {
                Log.Error($"Ошибка во время получения статистики. Сценарий: [{scenarioId}]", e);
                return new StatisticsDataItem[0];
            }
        }

        private void RegisterInternal(ScenarioBase scenario)
        {
            scenario.SetOnStateChanged(EventTriggered);
        }

        private void UnregisterInternal(ScenarioBase scenario)
        {
            scenario.RemoveOnStateChanged(EventTriggered);
        }

        private void EventTriggered(object sender, EventsArgs<ScenarioValueChangedEventArgs> args)
        {
            if (args.Value.Scenario is RemoteScenario == false)
            {
                var newVal = args.Value.Scenario.GetCurrentValue();
                AddValueCache(args.Value.Scenario.Id, newVal);

                var item = new StatisticsItem();
                item.DateTime = DateTime.Now;

                item.Source = new StatisticsItemSource()
                {
                    ID = args.Value.Source.User?.Id,
                    Name = args.Value.Source.User?.Name,
                    SourceType = GetStartupSourceName(args.Value.Source.Source)
                };

                item.Target = new StatisticsScenarioInfo()
                {
                    ID = args.Value.Scenario.Id,
                    Name = args.Value.Scenario.Name,
                    ValueTypeName = ActionsDomain.Utils.GetValueTypeClassName(args.Value.Scenario.ValueType.GetType())
                };

                item.Value = newVal;

                AddItem(item);
            }
        }

        public async Task<StatisticsItem[]> GetItems(StatisticsScenarioInfo info, DateTime since, DateTime to, ScenarioActionSource source)
        {
            var scenario = ScenariosRepository.Scenarios.FirstOrDefault(x => x.Id == info.ID && ActionsDomain.Utils.GetValueTypeClassName(x.ValueType.GetType()) == info.ValueTypeName);
            if (scenario?.SecuritySettings.IsAvailableForUser(source.User, source.Source, source.Action) ?? false)
            {
                if (scenario is RemoteScenario remoteScenario)
                {
                    try
                    {
                        if (!scenario.GetIsAvailable())
                            throw new ScenarioExecutionException(ScenarioExecutionError.NotAvailable);
                        var remoteScenarioInfo = new StatisticsScenarioInfo()
                        {
                            Name = remoteScenario.Name,
                            ID = remoteScenario.RemoteScenarioId,
                            ValueTypeName = ActionsDomain.Utils.GetValueTypeClassName(remoteScenario.ValueType.GetType()),
                            Since = DateTime.Now,
                            To = DateTime.Now,
                        };
                        var client = ServiceClientFactory.Current.GetClient(remoteScenario.Credentials);
                        var statistics = await client.GetStatistics(since, to, remoteScenarioInfo);
                        foreach (var item in statistics)
                        {
                            //crutch
                            item.Target.ID = remoteScenario.Id;
                            item.Target.Name = remoteScenario.Name;
                        }
                        return statistics;
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
                else
                {
                    return 
                        _dataManager.GetDataItems(info.ID, info.ValueTypeName, since, to)
                        .Select(x =>
                        {
                            var item = new StatisticsItem();
                            item.Source = new StatisticsItemSource();
                            item.Source.SourceType = x.SourceType;
                            item.Source.Name = x.SourceName;
                            item.Source.ID = x.SourceId;
                            item.DateTime = new DateTime(x.Year, x.Month, x.Day, x.Hour, x.Minute, x.Second);
                            item.Value = x.Value;
                            item.Target = info;
                            return item;
                        }).ToArray();
                }
            }
            throw new ScenarioExecutionException(ScenarioExecutionError.AccessDenied);
        }

        public async Task<StatisticsScenarioInfo> GetStatisticsInfoForScenario(ScenarioBase scenario, ScenarioActionSource source)
        {
            if (scenario.SecuritySettings.IsAvailableForUser(source.User, source.Source, source.Action))
            {
                if (scenario is RemoteScenario remoteScenario)
                {
                    try
                    {
                        if (!scenario.GetIsAvailable())
                            throw new ScenarioExecutionException(ScenarioExecutionError.NotAvailable);
                        var scenarioInfo = new ScenarioInfo();
                        scenarioInfo.ScenarioId = remoteScenario.RemoteScenarioId;
                        scenarioInfo.ValueType = scenario.ValueType;
                        var client = ServiceClientFactory.Current.GetClient(remoteScenario.Credentials);
                        var remoteScenarioInfo = await client.GetStatisticsInfoForScenario(scenarioInfo);
                        remoteScenarioInfo.ID = scenario.Id; // Set current scenario id
                        remoteScenarioInfo.Name = scenario.Name;
                        return remoteScenarioInfo;
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
                else return _dataManager.GetInfo(scenario.Id, scenario.Name, ActionsDomain.Utils.GetValueTypeClassName(scenario.ValueType.GetType()));
            }
            throw new ScenarioExecutionException(ScenarioExecutionError.AccessDenied);
        }

        private async Task<ScenarioStatisticsRegistration> GetRegistrationInfoInternal(RemoteScenario[] scenarios) //only for equals credentials
        {
            if (!scenarios.Any())
                return new ScenarioStatisticsRegistration();

            try
            {
                var creds = scenarios.First().Credentials;
                var remoteScenariosInfo = scenarios.ToDictionary(x => x.RemoteScenarioId);
                var client = ServiceClientFactory.Current.GetClient(creds);
                var remoteResult = await client.GetStatisticsRegistration(remoteScenariosInfo.Select(x => x.Value.RemoteScenarioId).ToArray());
                var result = new ScenarioStatisticsRegistration(remoteResult.RegisteredIds.Select(x => remoteScenariosInfo[x].Id).ToArray());

                return result;
            }
            catch (Exception e)
            {
                //connection failed
                Log.Debug($"Error while get scenario statistic registration info", e);
                return new ScenarioStatisticsRegistration();
            }
        }

        public async Task<ScenarioStatisticsRegistration> GetRegistrationInfo(ScenarioBase[] scenarios)
        {
            var result = new ScenarioStatisticsRegistration();

            var targetIds = _statisticsScenariosInfos
                .Where(x => scenarios.Any(z => z.Id == x.ScenarioId))
                .Select(x => x.ScenarioId)
                .ToArray();

            result.Union(new ScenarioStatisticsRegistration(targetIds));

            // For remote scenarios

            if (scenarios.Length == 1 && targetIds.Length == 1)
                return result;

            var remoteScenariosGroups = scenarios
                .OfType<RemoteScenario>()
                .GroupBy(x => x.Credentials)
                .ToArray();

            foreach (var group in remoteScenariosGroups)
                result.Union(await GetRegistrationInfoInternal(group.ToArray()));

            // End -- for remote scenarios

            return result;
        }

        public async void Register(ScenarioBase scenario)
        {
            if (scenario is RemoteScenario == false && !await IsRegisteredInternal(scenario))
            {
                _statisticsScenariosInfos.Add(
                    new StatisticsScenarioInfoInternal()
                    {
                        ScenarioId = scenario.Id,
                        ValueTypeName = ActionsDomain.Utils.GetValueTypeClassName(scenario.ValueType.GetType())
                    }
                );
                RegisterInternal(scenario);
                InitializeTimer();
                SaveData();
            }
        }

        private async Task<bool> IsRegisteredInternal(ScenarioBase scenario)
        {
            var info = await GetRegistrationInfo(new[] { scenario });
            return info.IsRegistered(scenario.Id);
        }

        public async void UnRegister(ScenarioBase scenario)
        {
            if (await IsRegisteredInternal(scenario))
            {
                _statisticsScenariosInfos.RemoveAll(x=> x.ScenarioId == scenario.Id);
                UnregisterInternal(scenario);
                InitializeTimer();
                SaveData();
            }
        }

        public void ReRegister(ScenarioBase scenario)
        {
            var scenarioValueType = ActionsDomain.Utils.GetValueTypeClassName(scenario.ValueType.GetType());
            if (_statisticsScenariosInfos.Any(x => x.ScenarioId == scenario.Id && x.ValueTypeName == scenarioValueType))
            {
                UnregisterInternal(scenario);
                RegisterInternal(scenario);
            }
        }
    }
}
