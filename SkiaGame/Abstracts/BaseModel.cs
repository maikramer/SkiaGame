#region

using System.ComponentModel;
using System.Runtime.CompilerServices;

// ReSharper disable MemberCanBeProtected.Global

#endregion

namespace SkiaGame.Abstracts
{
    /// <summary>
    ///     Base model
    /// </summary>
    public abstract class BaseModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged

        /// <summary>
        /// </summary>
        /// <param name="backingStore"></param>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName] string propertyName = "")
        {

            switch (backingStore)
            {
                case float f:
                {
                    if (value is float vf)
                        if (Math.Abs(vf - f) < Tolerance)
                            return false;

                    break;
                }
                case double d:
                {
                    if (value is double vf)
                        if (Math.Abs(vf - d) < Tolerance)
                            return false;

                    break;
                }
                default:
                    if (EqualityComparer<T>.Default.Equals(backingStore, value)) return false;
                    break;
            }

            backingStore = value;

            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="backingStore"></param>
        /// <param name="value"></param>
        /// <param name="onChanged"></param>
        /// <param name="propertyName"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        // ReSharper disable once UnusedMember.Global
        protected bool SetProperty<T>(ref T backingStore, T value, Action onChanged, [CallerMemberName] string propertyName="")
        {
            if (!SetProperty(ref backingStore, value, propertyName)) return false;
            
            onChanged.Invoke();
            return true;

        }

        /// <summary>
        ///     The tolerance used for double and float comparing
        /// </summary>
        public double Tolerance { get; } = 0.000001;

        /// <summary>
        /// </summary>
        public virtual event PropertyChangedEventHandler? PropertyChanged = (_, _) => { };

        /// <summary>
        ///     Called on Property changed
        /// </summary>
        /// <param name="propertyName"></param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;

            changed?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}