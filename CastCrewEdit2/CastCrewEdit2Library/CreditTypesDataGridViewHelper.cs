using System;
using System.Windows.Forms;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2
{
    public static class CreditTypesDataGridViewHelper
    {
        public static class CreditTypes
        {
            public const String Direction = "Direction";
            public const String Writing = "Writing";
            public const String Production = "Production";
            public const String Cinematography = "Cinematography";
            public const String FilmEditing = "Film Editing";
            public const String Music = "Music";
            public const String Sound = "Sound";
            public const String Art = "Art";
            public const String Other = "Other";
        }

        public static class CreditSubtypes
        {
            public const String Custom = "Custom";
        }

        public static void FillCreditSubtypes(String creditType, DataGridViewComboBoxCell.ObjectCollection items)
        {
            items.Clear();
            switch(creditType)
            {
                case (CreditTypes.Direction):
                {
                    items.Add("Director");
                    break;
                }
                case (CreditTypes.Writing):
                {
                    items.Add("Original Material By");
                    items.Add("Screenwriter");
                    items.Add("Writer");
                    items.Add("Original Characters By");
                    items.Add("Created By");
                    items.Add("Story By");
                    break;
                }
                case (CreditTypes.Production):
                {
                    items.Add("Producer");
                    items.Add("Executive Producer");
                    break;
                }
                case (CreditTypes.Cinematography):
                {
                    items.Add("Director of Photography");
                    items.Add("Cinematographer");
                    break;
                }
                case (CreditTypes.FilmEditing):
                {
                    items.Add("Film Editor");
                    break;
                }
                case (CreditTypes.Music):
                {
                    items.Add("Composer");
                    items.Add("Song Writer");
                    items.Add("Theme By");
                    break;
                }
                case (CreditTypes.Sound):
                {
                    items.Add("Sound");
                    items.Add("Sound Designer");
                    items.Add("Supervising Sound Editor");
                    items.Add("Sound Editor");
                    items.Add("Sound Re-Recording Mixer");
                    items.Add("Production Sound Mixer");
                    break;

                }
                case (CreditTypes.Art):
                {
                    items.Add("Production Designer");
                    items.Add("Art Director");
                    items.Add("Costume Designer");
                    items.Add("Make-up Artist");
                    items.Add("Visual Effects");
                    items.Add("Make-up Effects");
                    items.Add("Creature Design");
                    break;
                }
                case (CreditTypes.Other):
                {
                    break;
                }
            }
            items.Add(CreditSubtypes.Custom);
        }

    }
}
