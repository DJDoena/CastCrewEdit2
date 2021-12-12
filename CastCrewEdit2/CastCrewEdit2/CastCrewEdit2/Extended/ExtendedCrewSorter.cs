namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Extended
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using DVDProfilerXML.Version400;

    public static class ExtendedCrewSorter
    {
        private class CrewComparer : IComparable<CrewComparer>
        {
            private int EpisodeId { get; set; }

            private int GroupId { get; set; }

            private int OriginalOrderId { get; set; }

            private ExtendedCrewMember CrewMember { get; set; }

            private ExtendedCrewDivider CrewDivider { get; set; }

            internal object CrewEntry => this.CrewMember != null
                ? (object)this.CrewMember
                : this.CrewDivider;

            private CrewComparer(int episodeId, int groupId, int originalOrderId)
            {
                this.EpisodeId = episodeId;

                this.GroupId = groupId;

                this.OriginalOrderId = originalOrderId;
            }

            internal CrewComparer(int episodeId, int groupId, int originalOrderId, ExtendedCrewMember crewMember) : this(episodeId, groupId, originalOrderId)
            {
                this.CrewMember = crewMember;
            }

            internal CrewComparer(int episodeId, int groupId, int originalOrderId, ExtendedCrewDivider crewDivider) : this(episodeId, groupId, originalOrderId)
            {
                this.CrewDivider = crewDivider;
            }

            public int CompareTo(CrewComparer other)
            {
                if (other == null)
                {
                    return 1;
                }

                var compare = this.EpisodeId.CompareTo(other.EpisodeId);

                if (compare != 0)
                {
                    return compare;
                }
                compare = GetCompareValue(this.CrewEntry, other.CrewEntry);

                if (compare != 0)
                {
                    return compare;
                }

                compare = this.GroupId.CompareTo(other.GroupId);

                if (compare != 0)
                {
                    return compare;
                }

                return this.OriginalOrderId.CompareTo(other.OriginalOrderId);
            }

            private static int GetCompareValue(object left, object right)
            {
                var compareLeft = GetCompareValue(left);

                var compareRight = GetCompareValue(right);

                return compareLeft.CompareTo(compareRight);
            }

            private static int GetCompareValue(object crewEntry)
            {
                int compare;
                if (crewEntry is ExtendedCrewDivider crewDivider)
                {
                    switch (crewDivider.Type)
                    {
                        case DividerType.Episode:
                            {
                                compare = -1;

                                break;
                            }
                        default:
                            {
                                compare = GetCompareValue(crewDivider.CreditType);

                                break;
                            }
                    }
                }
                else
                {
                    if (crewEntry is ExtendedCrewMember crewMember)
                    {
                        compare = GetCompareValue(crewMember.CreditType);
                    }
                    else
                    {
                        Debug.Fail(string.Format("Unknown object type {0}", crewEntry));

                        compare = -2;
                    }
                }

                return compare;
            }

            private static int GetCompareValue(string creditType)
            {
                switch (creditType)
                {
                    case "Direction":
                        {
                            return 1;
                        }
                    case "Writing":
                        {
                            return 2;
                        }
                    case "Production":
                        {
                            return 3;
                        }
                    case "Cinematography":
                        {
                            return 4;
                        }
                    case "Film Editing":
                        {
                            return 5;
                        }
                    case "Music":
                        {
                            return 6;
                        }
                    case "Sound":
                        {
                            return 7;
                        }
                    case "Art":
                        {
                            return 8;
                        }
                    case "Other":
                        {
                            return 9;
                        }
                    default:
                        {
                            Debug.Fail(string.Format("Unknown Credit Type '{0}'", creditType));

                            return 0;
                        }
                }
            }
        }

        public static ExtendedCrewInformation GetSortedCrew(ExtendedCrewInformation unsortedCrew)
        {
            if (!(unsortedCrew?.CrewList?.Length > 0))
            {
                return unsortedCrew;
            }

            var sortedCrew = new ExtendedCrewInformation()
            {
                ImdbLink = unsortedCrew.ImdbLink,
                Title = unsortedCrew.Title,
            };

            var sortedList = new List<CrewComparer>(unsortedCrew.CrewList.Length);

            var currentEpisodeId = 0;

            var currentGroupId = 0;

            var originalOrderId = 0;

            foreach (var crewEntry in unsortedCrew.CrewList)
            {
                if (crewEntry is ExtendedCrewDivider divider)
                {
                    if (divider.Type == DividerType.Episode)
                    {
                        currentEpisodeId++;
                    }
                    else if (divider.Type == DividerType.Group)
                    {
                        currentGroupId++;
                    }

                    sortedList.Add(new CrewComparer(currentEpisodeId, currentGroupId, originalOrderId, divider));

                    if (divider.Type == DividerType.EndDiv)
                    {
                        currentGroupId++;
                    }
                }
                else
                {
                    sortedList.Add(new CrewComparer(currentEpisodeId, currentGroupId, originalOrderId, crewEntry as ExtendedCrewMember));
                }

                originalOrderId++;
            }

            sortedList.Sort();

            sortedCrew.CrewList = sortedList.ConvertAll(sortedEntry => sortedEntry.CrewEntry).ToArray();

            return sortedCrew;
        }
    }
}