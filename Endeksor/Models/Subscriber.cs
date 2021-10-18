using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace App4.Models
{
    public class Subscriber:Base
    {

        public bool IsReaded { get; set; } = false;
        public double NewValue {
            get
            {
                return _NewValue;
            }
            set
            {
                _NewValue = value;
                if(!(value<-1))
                    IsReaded = true;
            }
        }
        private double _NewValue;


    }
}