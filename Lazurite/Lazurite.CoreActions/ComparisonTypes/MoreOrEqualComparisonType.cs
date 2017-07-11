﻿using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.ValueTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lazurite.CoreActions.ComparisonTypes
{
    public class MoreOrEqualComparisonType : IComparisonType
    {
        public string Caption
        {
            get
            {
                return ">=";
            }
            set
            {
                //
            }
        }

        public bool OnlyNumeric
        {
            get
            {
                return true;
            }
        }

        public bool Calculate(IAction val1, IAction val2, ExecutionContext context)
        {
            try
            { 
                return val1.ValueType is DateTimeValueType ?
                    DateTime.Parse(val1.GetValue(context)) >= DateTime.Parse(val2.GetValue(context)) :
                    decimal.Parse(val1.GetValue(context)) >= decimal.Parse(val2.GetValue(context));
            }
            catch
            {
                return false;
            }
        }
    }
}
