using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Windows;
using System.Windows.Controls;

namespace LanProject.MainApplication.Model
{
    internal class Page : ObservableObject
    {
        #region 頁面名稱 英文
        private string name;
        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }
        #endregion
        #region 頁面名稱 中文
        private string displayname;
        public string DisplayName
        {
            get => displayname;
            set => SetProperty(ref displayname, value);
        }
        #endregion
        #region 圖標
        public string ImgPath { get; set; }
        #endregion
        public Type type;
        #region Making Page content to used for mainwindow binding contentcontrol source 

        private object _content;
        public object Content => (_content = CreateContent());
        private object CreateContent()
        {
            var content = Activator.CreateInstance(type);
            return content;
        }
        #endregion
        #region This is Page scroll bar  h/v visibility and Page margin
        private ScrollBarVisibility _horizontalScrollBarVisibilityRequirement = ScrollBarVisibility.Visible;
        private ScrollBarVisibility _verticalScrollBarVisibilityRequirement = ScrollBarVisibility.Visible;
        public ScrollBarVisibility HorizontalScrollBarVisibilityRequirement
        {
            get => _horizontalScrollBarVisibilityRequirement;
            set => SetProperty(ref _horizontalScrollBarVisibilityRequirement, value);
        }

        public ScrollBarVisibility VerticalScrollBarVisibilityRequirement
        {
            get => _verticalScrollBarVisibilityRequirement;
            set => SetProperty(ref _verticalScrollBarVisibilityRequirement, value);
        }
        private Thickness _marginRequirement = new Thickness(16);
        public Thickness MarginRequirement
        {
            get => _marginRequirement;
            set => SetProperty(ref _marginRequirement, value);
        }
        #endregion
    }
}
