using System;

using App4.Invoices;

namespace App4.Models
{
    public class Lperiod
    {
        public period Period;
        public Lperiod(period item)
        {
            Period = item;
        }

        public override string ToString()
        {
            return Period.Info;
        }
    }
}