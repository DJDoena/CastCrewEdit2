using System;
using System.Collections.Generic;
using System.Linq;
using DoenaSoft.AbstractionLayer.IOServices;
using DoenaSoft.AbstractionLayer.UIServices;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.MergeCacheFiles
{
    public sealed class Model : IModel
    {
        private readonly IUIServices m_UIServices;

        private readonly IIOServices m_IOServices;

        public Model(IUIServices uiServices
            , IIOServices ioServices)
        {
            if (uiServices == null)
            {
                throw (new ArgumentNullException("uiServices"));
            }
            if (ioServices == null)
            {
                throw (new ArgumentNullException("ioServices"));
            }
            m_UIServices = uiServices;
            m_IOServices = ioServices;
            Load();
        }

        #region IModel Members
        public string LeftFileName { get; set; }

        public string RightFileName { get; set; }

        public string TargetFileName { get; set; }

        public void SaveSettings()
        {
            Properties.Settings.Default.LeftFile = LeftFileName;
            Properties.Settings.Default.RightFile = RightFileName;
            Properties.Settings.Default.TargetFile = TargetFileName;
            Properties.Settings.Default.Save();
        }

        public void Merge()
        {
            IFileInfo left;
            IFileInfo right;

            if (CheckPreconditions(out left, out right))
            {
                PersonInfos leftList;
                string fileName;

                fileName = left.FullName;
                if ((GetCache(fileName, out leftList)) && (CheckMergeEligibility(leftList, fileName)))
                {
                    PersonInfos rightList;

                    fileName = right.FullName;
                    if ((GetCache(fileName, out rightList)) && (CheckMergeEligibility(rightList, fileName)))
                    {
                        PersonInfos targetList;

                        targetList = Merge(leftList, rightList);
                        if (Serialize(left, targetList))
                        {
                            if (Serialize(right, targetList))
                            {
                                m_UIServices.ShowMessageBox("Done.", string.Empty, Buttons.OK, Icon.Information);
                            }
                        }
                    }
                }
            }
        }

        public void MergeIntoThirdFile()
        {
            IFileInfo left;
            IFileInfo right;
            IFileInfo target;

            if (CheckPreconditions(out left, out right, out target))
            {
                PersonInfos leftList;
                string fileName;

                fileName = left.FullName;
                if ((GetCache(fileName, out leftList)) && (CheckMergeEligibility(leftList, fileName)))
                {
                    PersonInfos rightList;

                    fileName = right.FullName;
                    if ((GetCache(fileName, out rightList)) && (CheckMergeEligibility(rightList, fileName)))
                    {
                        PersonInfos targetList;

                        targetList = Merge(leftList, rightList);
                        if (Serialize(target, targetList))
                        {
                            m_UIServices.ShowMessageBox("Done.", string.Empty, Buttons.OK, Icon.Information);
                        }
                    }
                }
            }
        }

        public event EventHandler<EventArgs<int>> ProgressMaxChanged;

        public event EventHandler<EventArgs<int>> ProgressValueChanged;
        #endregion

        private void Load()
        {
            LeftFileName = Properties.Settings.Default.LeftFile;
            RightFileName = Properties.Settings.Default.RightFile;
            TargetFileName = Properties.Settings.Default.TargetFile;
        }

        private PersonInfos Merge(PersonInfos leftList
            , PersonInfos rightList)
        {
            Dictionary<string, PersonInfo> leftDict;
            Dictionary<string, PersonInfo> rightDict;
            Dictionary<string, PersonInfo> targetDict;
            PersonInfos target;
            List<PersonInfo> list;
            const int Buffer = 1000;
            int progress;
            int maxProgress;
            int step;

            leftDict = GetDictionary(leftList);
            rightDict = GetDictionary(rightList);
            maxProgress = leftDict.Count + rightDict.Count;
            step = GetStep(maxProgress);
            FireProgressMaxChanged(maxProgress);
            if (leftDict.Count > rightDict.Count)
            {
                targetDict = new Dictionary<string, PersonInfo>(leftDict.Count + Buffer);
            }
            else
            {
                targetDict = new Dictionary<string, PersonInfo>(rightDict.Count + Buffer);
            }
            progress = 0;
            FireProgressValueChanged(progress, step);
            foreach (var kvp in leftDict)
            {
                PersonInfo rightValue;

                if (rightDict.TryGetValue(kvp.Key, out rightValue))
                {
                    if (kvp.Value.LastModified > rightValue.LastModified)
                    {
                        targetDict.Add(kvp.Key, kvp.Value);
                    }
                    else
                    {
                        targetDict.Add(kvp.Key, rightValue);
                    }
                    rightDict.Remove(kvp.Key);
                    progress += 2;
                }
                else
                {
                    targetDict.Add(kvp.Key, kvp.Value);
                    progress++;
                }
                FireProgressValueChanged(progress, step);
            }
            foreach (var kvp in rightDict)
            {
                targetDict.Add(kvp.Key, kvp.Value);
                progress++;
                FireProgressValueChanged(progress, step);
            }
            progress = 0;
            FireProgressValueChanged(progress, step);
            FireProgressMaxChanged(int.MinValue);
            list = new List<PersonInfo>(targetDict.Values);
            list.Sort(new Comparison<PersonInfo>(PersonInfo.CompareForSorting));
            target = new PersonInfos();
            target.PersonInfoList = list.ToArray();
            return (target);
        }

        private int GetStep(int maxProgress)
        {
            int step;

            step = 1;
            if (maxProgress > 100)
            {
                step = maxProgress / 100;
                if ((maxProgress % 100) != 0)
                {
                    step++;
                }
            }
            return (step);
        }

        private void FireProgressMaxChanged(int value)
        {
            FireProgressChanged(ProgressMaxChanged, value);
        }

        private void FireProgressValueChanged(int value, int step)
        {
            if ((value % step) == 0)
            {
                FireProgressChanged(ProgressValueChanged, value);
            }
        }

        private void FireProgressChanged(EventHandler<EventArgs<int>> handler, int value)
        {
            if (handler != null)
            {
                handler(this, new EventArgs<int>(value));
            }
        }

        private static Dictionary<string, PersonInfo> GetDictionary(PersonInfos list)
        {
            Dictionary<string, PersonInfo> dict;

            if ((list.PersonInfoList != null)
                && (list.PersonInfoList.Length > 0))
            {
                dict = new Dictionary<string, PersonInfo>(list.PersonInfoList.Length);
                foreach (var pi in list.PersonInfoList)
                {
                    dict.Add(pi.PersonLink, pi);
                }
            }
            else
            {
                dict = new Dictionary<string, PersonInfo>(0);
            }
            return (dict);
        }

        private bool CheckMergeEligibility(PersonInfos list
            , string fileName)
        {
            if ((list.PersonInfoList != null)
                && (list.PersonInfoList.Length > 0))
            {
                IEnumerable<PersonInfo> missingTags;

                missingTags = list.PersonInfoList.Where(pi => pi.LastModifiedSpecified == false);
                if (missingTags.Count() > 0)
                {
                    string text;

                    text = string.Format("'{0}' was not created with Cast/Crew Edit 2 v1.8.9.1 or higher!", fileName);
                    m_UIServices.ShowMessageBox(text, "Error", Buttons.OK, Icon.Error);
                    return (false);
                }
            }
            return (true);
        }

        private bool GetCache(string fileName
            , out PersonInfos personInfoList)
        {
            personInfoList = null;
            try
            {
                personInfoList = PersonInfos.Deserialize(fileName);
            }
            catch
            {
                string text;

                text = string.Format("Could not read '{0}'", fileName);
                m_UIServices.ShowMessageBox(text, "Error", Buttons.OK, Icon.Error);
                return (false);
            }
            return (true);
        }

        private bool CheckPreconditions(out IFileInfo left
            , out IFileInfo right
            , out IFileInfo target)
        {
            IFileInfo tempLeft;
            IFileInfo tempRight;
            IFileInfo tempTarget;

            left = null;
            right = null;
            target = null;
            if (CheckPreconditions(out tempLeft, out tempRight) == false)
            {
                return (false);
            }
            tempTarget = GetFile(TargetFileName, false);
            if (tempTarget == null)
            {
                m_UIServices.ShowMessageBox("Target file name is empty!", "Error", Buttons.OK, Icon.Error);
                return (false);
            }
            if (tempLeft.FullName == tempTarget.FullName)
            {
                if (m_UIServices.ShowMessageBox("Your left file is the same as your target file. Do you want to overwrite it?", "Overwrite?"
                    , Buttons.OK, Icon.Error) == Result.No)
                {
                    return (false);
                }
            }
            else if (tempRight.FullName == tempTarget.FullName)
            {
                if (m_UIServices.ShowMessageBox("Your right file is the same as your target file. Do you want to overwrite it?", "Overwrite?"
                    , Buttons.OK, Icon.Error) == Result.No)
                {
                    return (false);
                }
            }
            left = tempLeft;
            right = tempRight;
            target = tempTarget;
            return (true);
        }

        private IFileInfo GetFile(string fileName
            , bool fileMustExist)
        {
            IFileInfo fi;

            fi = null;
            if (string.IsNullOrEmpty(fileName) == false)
            {
                if (fileMustExist)
                {
                    if (m_IOServices.File.Exists(fileName))
                    {
                        fi = m_IOServices.GetFileInfo(fileName);
                    }
                }
                else
                {
                    fi = m_IOServices.GetFileInfo(fileName);
                }
            }
            return (fi);
        }

        private bool CheckPreconditions(out IFileInfo left
            , out IFileInfo right)
        {
            IFileInfo tempLeft;
            IFileInfo tempRight;
            string leftFileName;
            string rightFileName;

            left = null;
            right = null;
            tempLeft = GetFile(LeftFileName, true);
            if (tempLeft == null)
            {
                m_UIServices.ShowMessageBox("Left file name is empty or file does not exist!", "Error", Buttons.OK, Icon.Error);
                return (false);
            }
            tempRight = GetFile(RightFileName, true);
            if (tempRight == null)
            {
                m_UIServices.ShowMessageBox("Right file name is empty or file does not exist!", "Error", Buttons.OK, Icon.Error);
                return (false);
            }
            if (tempLeft.FullName == tempRight.FullName)
            {
                m_UIServices.ShowMessageBox("Left and right file name are the same!", "Error", Buttons.OK, Icon.Error);
                return (false);
            }
            leftFileName = tempLeft.Name.ToLower();
            rightFileName = tempRight.Name.ToLower();
            if ((leftFileName.Contains("cast") && (rightFileName.Contains("crew")))
                || (leftFileName.Contains("crew") && (rightFileName.Contains("cast"))))
            {
                if (m_UIServices.ShowMessageBox("You're about to merge a cast and a crew file. You should not do that. Are you sure?", "Merge cast and crew?"
                , Buttons.OK, Icon.Error) == Result.No)
                {
                    return (false);
                }
            }
            left = tempLeft;
            right = tempRight;
            return (true);
        }

        private bool Serialize(IFileInfo fileInfo
            , PersonInfos targetList)
        {
            try
            {
                string backupFile;

                backupFile = fileInfo.Name.Substring(0, fileInfo.Name.Length - fileInfo.Extension.Length);
                backupFile += ".bak";
                backupFile = m_IOServices.Path.Combine(fileInfo.FolderName, backupFile);
                if (m_IOServices.File.Exists(backupFile))
                {
                    m_IOServices.File.Delete(backupFile);
                }
                if (m_IOServices.File.Exists(fileInfo.FullName))
                {
                    m_IOServices.File.Move(fileInfo.FullName, backupFile);
                }
                targetList.Serialize(fileInfo.FullName);
            }
            catch
            {
                string text;

                text = string.Format("Could not write '{0}'", fileInfo.FullName);
                m_UIServices.ShowMessageBox(text, "Error", Buttons.OK, Icon.Error);
                return (false);
            }
            return (true);
        }
    }
}