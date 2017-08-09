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
    }
}
