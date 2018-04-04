using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Seps.Infomatic.Gui
{
    // класс-наследник RotedEventArgs для передачи ID функции
    public class MyCustomEventArgs : RoutedEventArgs
        {
            private int _id; // переменная для передачи ID функции
            public MyCustomEventArgs(int value, RoutedEvent re)
                : base(re)
            {
                Id = value;
            }
            public int Id { get { return _id; } set { _id = (int)value; } }

        }
 }

