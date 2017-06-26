﻿using Lazurite.IOC;
using Lazurite.MainDomain;
using Lazurite.Scenarios.ScenarioTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LazuriteUI.Windows.Main.Constructors
{
    /// <summary>
    /// Логика взаимодействия для ScenariosResolverView.xaml
    /// </summary>
    public partial class ScenariosConstructorsResolverView : UserControl
    {
        private ScenariosRepositoryBase _repository = Singleton.Resolve<ScenariosRepositoryBase>(); 
        private IScenarioConstructorView _constructorView;
        private ScenarioBase _originalSenario;
        private ScenarioBase _clonedScenario;

        public event Action Applied;
        public event Action Modified;

        public ScenariosConstructorsResolverView()
        {
            InitializeComponent();
            this.buttonsView.ApplyClicked += () => Apply();
            this.buttonsView.ResetClicked += () => Revert();
        }

        public void SetScenario(ScenarioBase scenario)
        {
            _originalSenario = scenario;
            _clonedScenario = (ScenarioBase)Lazurite.Windows.Utils.Utils.CloneObject(_originalSenario);
            _clonedScenario.Initialize(_repository);
            if (scenario is SingleActionScenario)
                this.contentPresenter.Content = _constructorView = new SingleActionScenarioView((SingleActionScenario)_clonedScenario);
            buttonsView.SetScenario(scenario);
            _constructorView.Modified += () => Modified?.Invoke();
            _constructorView.Modified += () => buttonsView.ScenarioModified();
        }

        public ScenarioBase GetScenario()
        {
            return _originalSenario;
        }

        public void Apply()
        {
            _originalSenario.TryCancelAll();
            _repository.SaveScenario(_clonedScenario);
            _clonedScenario.Initialize(_repository);
            _clonedScenario.AfterInitilize();
            SetScenario(_clonedScenario);
            Applied?.Invoke();
        }

        public void Revert()
        {
            _clonedScenario = (ScenarioBase)Lazurite.Windows.Utils.Utils.CloneObject(_originalSenario);
            _clonedScenario.Initialize(_repository);
            _constructorView.Revert(_clonedScenario);
        }
    }
}
