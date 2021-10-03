using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace LHSBrackets.ModelBinder
{
    public class FilterOperations<T> : List<(FilterOperationEnum operation, IEnumerable<T> values, bool hasMultipleValues)>
    {
        internal void SetValue(FilterOperationEnum operation, string value)
        {
            switch (operation){
                case FilterOperationEnum.Eq:
                case FilterOperationEnum.Ne:
                case FilterOperationEnum.Gt:
                case FilterOperationEnum.Gte:
                case FilterOperationEnum.Lt:
                case FilterOperationEnum.Lte:
                    this.Add((operation, new List<T>{ (T)ConvertValue(value) }, false));
                    break;
                case FilterOperationEnum.In:
                case FilterOperationEnum.Nin:
                    var items = value.Split(",");
                    this.Add((operation, items.Select(x => (T)ConvertValue(x.Trim(' '))), true));
                    break;
                default:
                    throw new Exception($"Operation type: {operation.ToString()} is unhandled.");
            }
        }

        private object ConvertValue(string value)
        {
            var converter = TypeDescriptor.GetConverter(typeof(T));
            object convertedValue;
            try
            {
                convertedValue = converter.ConvertFromString(null, new CultureInfo("en-GB"), value);
            }
            catch (NotSupportedException)
            {
                //RatherEasys is not a valid value for DifficultyEnum
                throw;
                // TODO: do stuff
            }

            return convertedValue;
        }
    }
}