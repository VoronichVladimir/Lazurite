﻿using Lazurite.ActionsDomain;
using Lazurite.CoreActions;
using Lazurite.CoreActions.CheckerLogicalOperators;
using System;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Main.Constructors.Decomposition
{
    /// <summary>
    /// Логика взаимодействия для CheckerOperatorPairView.xaml
    /// </summary>
    public partial class ComplexCheckerOperatorPairView : Grid, IConstructorElement
    {
        private CheckerOperatorPair _pair;
        public ComplexCheckerOperatorPairView()
        {
            InitializeComponent();

            operatorView.Modified += (e) => Modified?.Invoke(this);
            complexCheckerView.Modified += (e) => Modified?.Invoke(this);

            buttonsEnd.AddNewClick += () => NeedAddNext?.Invoke(this);
            buttonsGroup.RemoveClick += () => NeedRemove?.Invoke(this);
            buttonsGroup.AddNewClick += () => complexCheckerView.AddFirst();

        }

        public void Refresh(ActionHolder actionHolder, IAlgorithmContext algoContext)
        {
            ActionHolder = actionHolder;
            AlgorithmContext = algoContext;
            _pair = (CheckerOperatorPair)actionHolder.Action;
            operatorView.Refresh(ActionHolder, AlgorithmContext);
            complexCheckerView.Refresh(new ActionHolder((ComplexCheckerAction)_pair.Checker), algoContext);
        }

        public ActionHolder ActionHolder
        {
            get;
            private set;
        }

        public bool EditMode
        {
            get;
            set;
        }

        public IAlgorithmContext AlgorithmContext
        {
            get
            {
                return complexCheckerView.AlgorithmContext;
            }
            set
            {
                complexCheckerView.AlgorithmContext = 
                    operatorView.AlgorithmContext = value;
            }
        }

        public void MakeOperatorInvisible()
        {
            operatorView.MakeOperatorInvisible();
        }

        public void MakeOperatorVisible()
        {
            operatorView.MakeOperatorVisible();
        }

        public event Action<IConstructorElement> Modified;
        public event Action<IConstructorElement> NeedAddNext;
        public event Action<IConstructorElement> NeedRemove;
    }
}
