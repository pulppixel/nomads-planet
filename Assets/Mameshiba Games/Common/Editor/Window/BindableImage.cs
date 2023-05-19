using System;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace MameshibaGames.Common.Editor.Window
{
    public class BindableImage : Image, IBindable, INotifyValueChanged<Object>
    {
        private Object _value;
        public IBinding binding { get; set; }
        public string bindingPath { get; set; }
        
        public void SetValueWithoutNotify(Object newValue)
        {
            value = newValue;
        }

        public Object value
        {
            get => _value;
            set
            {
                if (value is Texture2D || value == null)
                {
                    Object previous = _value;
                    _value = value;
                    image = _value as Texture2D;
                    
                    using (ChangeEvent<Object> evt = ChangeEvent<Object>.GetPooled(previous, value))
                    {
                        evt.target = this;
                        SendEvent(evt);
                    }
                }
                else
                {
                    throw new Exception("Bindable Type must be Texture2D");
                }
            }
        }
    }
}