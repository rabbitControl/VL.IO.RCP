using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace RCP
{
    public class RCPObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected bool Equals<TValue>(TValue a, TValue b) => EqualityComparer<TValue>.Default.Equals(a, b);

        protected bool SetProperty<TValue>(ref TValue field, TValue value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected bool SetProperty<TValue>(ref TValue[] field, TValue[] value, [CallerMemberName] string propertyName = null)
        {
            if (field.SequenceEqual(value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
