using System;
using System.Windows.Forms;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2
{
    public sealed class MessageEntry
    {
        public readonly String Message;

        public readonly String Header;

        public readonly MessageBoxButtons Buttons;

        public readonly MessageBoxIcon Icon;

        private readonly Int32 HashCode;

        public MessageEntry(String message
            , String header
            , MessageBoxButtons buttons
            , MessageBoxIcon icon)
        {
            Message = message;
            Header = header;
            Buttons = buttons;
            Icon = icon;
            HashCode = Message.GetHashCode() + Header.GetHashCode() + Buttons.GetHashCode() + Icon.GetHashCode();
        }

        public override Int32 GetHashCode()
        {
            return (HashCode);
        }

        public override Boolean Equals(Object obj)
        {
            MessageEntry other;

            other = obj as MessageEntry;
            if (other == null)
            {
                return (false);
            }
            return ((Message == other.Message) && (Header == other.Header) && (Buttons == other.Buttons) && (Icon == other.Icon));
        }
    }
}