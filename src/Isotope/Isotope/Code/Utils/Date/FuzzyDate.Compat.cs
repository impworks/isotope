﻿using System;
using Newtonsoft.Json;

namespace Isotope.Code.Utils.Date
{
    public partial struct FuzzyDate
    {
        #region JsonConverter

        public class FuzzyDateJsonConverter : JsonConverter
        {
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                writer.WriteValue(value.ToString());
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                var value = reader.Value?.ToString();

                if(objectType == typeof(FuzzyDate?))
                    return FuzzyDate.TryParse(value);

                return FuzzyDate.Parse(value);
            }

            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(FuzzyDate)
                       || objectType == typeof(FuzzyDate?);
            }
        }

        #endregion
    }
}
