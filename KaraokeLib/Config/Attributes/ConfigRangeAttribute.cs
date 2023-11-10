namespace KaraokeLib.Config.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ConfigRangeAttribute : Attribute
    {
        private bool _isDecimal = false;
        private bool _hasMin = true;
        private bool _hasMax = true;
        private double _min;
        private double _max;

        public bool HasMax => _hasMax;

        public bool IsDecimal => _isDecimal;

        public double Minimum => _min;

        public double Maximum => _max;

        public ConfigRangeAttribute(double min, double max)
        {
            _min = min;
            _max = max;
            _isDecimal = true;
        }

        public ConfigRangeAttribute(int min, int max)
        {
            _isDecimal = false;
            _min = min;
            _max = max;
        }

        public ConfigRangeAttribute(double min)
        {
            _min = min;
            _max = -1;
            _hasMax = false;
            _isDecimal = true;
        }

        public ConfigRangeAttribute(int min)
        {
            _isDecimal = false;
            _min = min;
            _max = -1;
            _hasMax = false;
        }
    }
}
