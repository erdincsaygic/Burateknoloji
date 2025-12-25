using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.DAL.Concrete
{
    public class SliderDAL : BaseDAL<Slider>, ISliderDAL
    {
        public override string TableName
        {
            get { return "Sliders"; }
        }
    }
}
