using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WisconsinSetup
{
    /// <summary>
    ///     This class provides a mapping between a human-readable label and a machine-readable number, like
    ///     "Million" and 1000000. It provides conversions between string and long. 
    /// </summary>
    class Multiplier : IConvertible
    {
        private readonly string _label;
        private readonly long _value;

        public Multiplier(string humanFriendlyLabel, long mathematicalValue)
        {
            _label = humanFriendlyLabel;
            _value = mathematicalValue;
        }

        // ========
        // SECTION: Valid casts
        // ========

        public override string ToString() => _label;

        public long ToInt64(IFormatProvider provider = null) => _value;

        public TypeCode GetTypeCode() => TypeCode.Object;

        // ========
        // SECTION: Invalid casts
        // ========

        public bool ToBoolean(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public char ToChar(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public sbyte ToSByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public byte ToByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public short ToInt16(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public ushort ToUInt16(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public int ToInt32(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public ulong ToUInt64(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public float ToSingle(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public double ToDouble(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public string ToString(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            throw new NotImplementedException();
        }
    }
}
