using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace LanProject.MainApplication.View.Setting
{
    public class NoteViewModel : ObservableObject
    {
        public NoteViewModel()
        {
            ReadingNoteTitleList();
        }
        private ObservableCollection<string> notelist;
        public ObservableCollection<string> NoteList
        {
            get => notelist;
            set => SetProperty(ref notelist, value);
        }
        private readonly string programFilesPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);//LanProject.Method.Configuration.ReadSetting("dataPath")
        private readonly string foldername = LanProject.Method.Configuration.ReadSetting("appfoldername");
        private readonly string notefolder = LanProject.Method.Configuration.ReadSetting("notes");
        public void ReadingNoteTitleList()
        {
            if (NoteList == null) NoteList = new ObservableCollection<string>();
            NoteList.Clear();
            Directory.CreateDirectory(Path.Combine(programFilesPath, foldername, notefolder));
            var fileLists = Directory.GetFiles(Path.Combine(programFilesPath, foldername, notefolder));
            foreach (var item in fileLists)
                NoteList.Add(Path.GetFileNameWithoutExtension(item));
        }
    }
}
