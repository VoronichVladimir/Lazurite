﻿using Lazurite.IOC;
using Lazurite.Logging;
using Lazurite.MainDomain;
using LazuriteUI.Windows.Controls;
using System;
using System.Windows;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Main.Constructors
{
    /// <summary>
    /// Логика взаимодействия для ScenariosResolverView.xaml
    /// </summary>
    public partial class TriggerConstructorView : UserControl
    {
        private ScenariosRepositoryBase _repository = Singleton.Resolve<ScenariosRepositoryBase>();
        private ILogger _log = Singleton.Resolve<ILogger>();
        private TriggerView _constructorView;
        private Lazurite.MainDomain.TriggerBase _originalTrigger;
        private Lazurite.MainDomain.TriggerBase _clonedTrigger;

        public event Action Applied;
        public event Action Modified;
        public bool IsModified { get; private set; }

        public TriggerConstructorView()
        {
            InitializeComponent();
            this.buttonsView.ApplyClicked += () => Apply();
            this.buttonsView.ResetClicked += () => Revert();
            buttonsView.Modified += () => IsModified = true;
        }

        public void SetTrigger(Lazurite.MainDomain.TriggerBase trigger, Action callback = null)
        {
            MessageView.ShowLoadCrutch((complete) =>
            {
                if (trigger != null)
                {
                    _originalTrigger = trigger;
                    _clonedTrigger = (Lazurite.MainDomain.TriggerBase)Lazurite.Windows.Utils.Utils.CloneObject(_originalTrigger);
                    _clonedTrigger.Initialize(_repository);
                    buttonsView.SetTrigger(_clonedTrigger);
                    IsModified = false;
                    _constructorView = new TriggerView(_clonedTrigger);
                    _constructorView.Modified += () => Modified?.Invoke();
                    _constructorView.Modified += () => buttonsView.TriggerModified();
                    _constructorView.Modified += () => IsModified = true;
                    _constructorView.Failed += () => buttonsView.Failed();
                    _constructorView.Succeed += () => buttonsView.Success();
                    this.contentPresenter.Content = _constructorView;
                    EmptyTriggerModeOff();
                }
                else
                {
                    EmptyTriggerModeOn();
                }
                complete?.Invoke();
                callback?.Invoke();
            }, 
            "Компоновка окна...");
        }

        private void EmptyTriggerModeOn()
        {
            tbTriggerEmpty.Visibility = Visibility.Visible;
            buttonsViewHolder.Visibility = Visibility.Collapsed;
            this.contentPresenter.Content = null;
        }

        private void EmptyTriggerModeOff()
        {
            tbTriggerEmpty.Visibility = Visibility.Collapsed;
            buttonsViewHolder.Visibility = Visibility.Visible;
        }

        public Lazurite.MainDomain.TriggerBase GetTrigger()
        {
            return _originalTrigger;
        }

        public void Apply(Action callback = null)
        {
            ApplyInternal();
            callback?.Invoke();
        }

        private void ApplyInternal()
        {
            _originalTrigger.Stop();
            _repository.SaveTrigger(_clonedTrigger);
            _clonedTrigger.Initialize(_repository);
            _clonedTrigger.AfterInitialize();
            SetTrigger(_clonedTrigger, 
                ()=> {
                    Applied?.Invoke();
                    IsModified = false;
                });
        }

        public void Revert()
        {
            _clonedTrigger = (Lazurite.MainDomain.TriggerBase)Lazurite.Windows.Utils.Utils.CloneObject(_originalTrigger);
            try
            {
                _clonedTrigger.Initialize(_repository);
            }
            catch (Exception e)
            {
                _log.ErrorFormat(e, "Во время инициализации триггера {0} возникла ошибка.", _clonedTrigger.Name);
            }
            buttonsView.Revert(_clonedTrigger);
            _constructorView.Revert(_clonedTrigger);
            IsModified = false;
        }
    }
}