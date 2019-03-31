using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TECAirlines_WebAPI.Classes
{
    public class Pair<X,Y>
    {
        private X first_obj;
        private Y second_obj;

        public Pair(X first, Y second)
        {
            first_obj = first;
            second_obj = second;
        }

        public void ChangeFirst(X nfirst)
        {
            first_obj = nfirst;
        }

        public void ChangeSecond(Y nsecond)
        {
            second_obj = nsecond;
        }

        public void Update(X nfirst, Y nsecond)
        {
            ChangeFirst(nfirst);
            ChangeSecond(nsecond);
        }

        public X First()
        {
            return first_obj;
        }

        public Y Second()
        {
            return second_obj;
        }
    }
}