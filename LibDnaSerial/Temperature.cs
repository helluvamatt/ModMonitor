namespace LibDnaSerial
{
    /// <summary>
    /// Represents a temperature with a value and a unit
    /// 
    /// This is just a model, no conversion is done when setting properties
    /// </summary>
    public struct Temperature
    {
        /// <summary>
        /// Temperature value
        /// </summary>
        public float Value { get; set; }

        /// <summary>
        /// Temperature unit
        /// </summary>
        public TemperatureUnit Unit { get; set; }

        /// <summary>
        /// Get the temperature value, automatically converting from the stored unit into the given desired unit
        /// </summary>
        /// <param name="desiredUnit">Desired <see cref="TemperatureUnit" /></param>
        /// <returns>Converted value</returns>
        public float GetValue(TemperatureUnit desiredUnit)
        {
            if (desiredUnit == Unit)
            {
                return Value;
            }
            else
            {
                float value = Value;
                // F -> C -> K
                if (Unit == TemperatureUnit.F)
                {
                    value = FtoC(value);
                    if (desiredUnit == TemperatureUnit.K) return CtoK(value);
                    else return value;
                }
                else if (Unit == TemperatureUnit.K)
                {
                    value = KtoC(value);
                    if (desiredUnit == TemperatureUnit.F) return CtoF(value);
                    else return value;
                }
                else // temp.Unit = TemperatureUnit.C
                {
                    if (desiredUnit == TemperatureUnit.F) return CtoF(value);
                    else return CtoK(value);
                }
            }
        }

        private static float FtoC(float f)
        {
            return (f - 32f) / 1.8f;
        }

        private static float CtoF(float c)
        {
            return c * 1.8f + 32f;
        }

        private static float KtoC(float k)
        {
            return k - 273.15f;
        }

        private static float CtoK(float c)
        {
            return c + 273.15f;
        }
    }
}
