using System;
using System.Text;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2
{
    internal class Name
    {
        public StringBuilder FirstName { get; set; }

        public StringBuilder MiddleName { get; set; }

        public StringBuilder LastName { get; set; }

        public Name()
        {
            FirstName = new StringBuilder();
            MiddleName = new StringBuilder();
            LastName = new StringBuilder();
        }

        public override String ToString()
        {
            StringBuilder name = new StringBuilder();

            if (FirstName.Length != 0)
            {
                name.Append("<" + FirstName.ToString() + ">");
            }

            if (MiddleName.Length != 0)
            {
                if (name.Length != 0)
                {
                    name.Append(" ");
                }

                name.Append("{" + MiddleName.ToString() + "}");
            }

            if (LastName.Length != 0)
            {
                if (name.Length != 0)
                {
                    name.Append(" ");
                }

                name.Append("[" + LastName.ToString() + "]");
            }

            return (name.ToString());
        }
    }
}