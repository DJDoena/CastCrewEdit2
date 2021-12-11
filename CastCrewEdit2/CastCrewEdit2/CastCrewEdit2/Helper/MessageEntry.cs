namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Helper
{
    using System.Windows.Forms;

    public sealed class MessageEntry
    {
        public readonly string Message;

        public readonly string Header;

        public readonly MessageBoxButtons Buttons;

        public readonly MessageBoxIcon Icon;

        private readonly int _hashCode;

        public MessageEntry(string message, string header, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            Message = message;

            Header = header;

            Buttons = buttons;

            Icon = icon;

            _hashCode = Message.GetHashCode() ^ Header.GetHashCode() ^ Buttons.GetHashCode() ^ Icon.GetHashCode();
        }

        public override int GetHashCode() => _hashCode;

        public override bool Equals(object obj)
        {
            if (!(obj is MessageEntry other))
            {
                return false;
            }

            var result = Message == other.Message
                && Header == other.Header
                && Buttons == other.Buttons
                && Icon == other.Icon;

            return result;
        }
    }
}