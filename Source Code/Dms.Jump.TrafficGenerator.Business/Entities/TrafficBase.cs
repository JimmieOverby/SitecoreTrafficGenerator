using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Dms.Jump.TrafficGenerator.Business.Entities
{
    public abstract class TrafficBase
    {
        private Random random = new Random();

        public int GetRandomNumber(int max)
        {
            var number = random.Next(max); 
            return number; 
        }
    }
}
