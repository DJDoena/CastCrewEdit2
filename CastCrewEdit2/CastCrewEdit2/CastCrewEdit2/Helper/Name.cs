namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Helper
{
    using System.Text;

    internal class Name
    {
        public StringBuilder FirstName { get; set; }

        public StringBuilder MiddleName { get; set; }

        public StringBuilder LastName { get; set; }

        public string OriginalName { get; set; }

        public string PlainName
        {
            get
            {
                var name = new StringBuilder();

                if (this.FirstName.Length != 0)
                {
                    name.Append(this.FirstName.ToString());
                }

                if (this.MiddleName.Length != 0)
                {
                    if (name.Length != 0)
                    {
                        name.Append(" ");
                    }

                    name.Append(this.MiddleName.ToString());
                }

                if (this.LastName.Length != 0)
                {
                    if (name.Length != 0)
                    {
                        name.Append(" ");
                    }

                    name.Append(this.LastName.ToString());
                }

                return name.ToString();
            }
        }

        public Name()
        {
            this.FirstName = new StringBuilder();

            this.MiddleName = new StringBuilder();

            this.LastName = new StringBuilder();
        }

        public override string ToString()
        {
            var name = new StringBuilder();

            if (this.FirstName.Length != 0)
            {
                name.Append("<" + this.FirstName.ToString() + ">");
            }

            if (this.MiddleName.Length != 0)
            {
                if (name.Length != 0)
                {
                    name.Append(" ");
                }

                name.Append("{" + this.MiddleName.ToString() + "}");
            }

            if (this.LastName.Length != 0)
            {
                if (name.Length != 0)
                {
                    name.Append(" ");
                }

                name.Append("[" + this.LastName.ToString() + "]");
            }

            return name.ToString();
        }
    }
}