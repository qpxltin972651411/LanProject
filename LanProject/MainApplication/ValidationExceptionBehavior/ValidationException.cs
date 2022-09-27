using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LanProject.MainApplication.ValidationExceptionBehavior
{
    public interface IMySelfValidationException
    {
        int MySelfErrorCount { get; set; }
    }
    public interface ICustomValidationException
    {
        int CustomErrorCount { get; set; }
    }
    public interface IFormValidationException
    {
        int FormErrorCount { get; set; }
        Dictionary<int, int> RowErrorCount { get; set; }
    }
    public class ValidationExceptionForMyUnit : Behavior<FrameworkElement>
    {
        //錯誤次數 等於0沒有錯誤
        private int _count = 0;
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.AddHandler(Validation.ErrorEvent, new EventHandler<ValidationErrorEventArgs>(OnValidationError));
        }

        private void OnValidationError(object sender, ValidationErrorEventArgs e)
        {
            IMySelfValidationException handler = null;

            if (AssociatedObject.DataContext is IMySelfValidationException)
                handler = this.AssociatedObject.DataContext as IMySelfValidationException;

            if (handler == null) return;

            var element = e.OriginalSource as UIElement;
            if (element == null) return;

            if (e.Action == ValidationErrorEventAction.Added)
                _count++;
            else if (e.Action == ValidationErrorEventAction.Removed)
                _count--;

            handler.MySelfErrorCount = _count;
        }
        /// <summary>
        /// 解除關聯的事件
        /// </summary> 
        protected override void OnDetaching()
        {
            base.OnDetaching();
            //AssociatedObject.MouseMove -= AssociatedObject_MouseMove;
            //AssociatedObject.MouseLeave -= AssociatedObject_MouseLeave;
        }


    }
    public class ValidationExceptionForCustomUnit : Behavior<FrameworkElement>
    {
        //錯誤次數 等於0沒有錯誤
        private int _count = 0;
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.AddHandler(Validation.ErrorEvent, new EventHandler<ValidationErrorEventArgs>(OnValidationError));
        }
        private void OnValidationError(object sender, ValidationErrorEventArgs e)
        {
            ICustomValidationException handler = null;

            if (AssociatedObject.DataContext is ICustomValidationException)
                handler = this.AssociatedObject.DataContext as ICustomValidationException;

            if (handler == null) return;

            var element = e.OriginalSource as UIElement;
            if (element == null) return;

            if (e.Action == ValidationErrorEventAction.Added)
                _count++;
            else if (e.Action == ValidationErrorEventAction.Removed)
                _count--;

            handler.CustomErrorCount = _count;
        }
        /// <summary>
        /// 解除關聯的事件
        /// </summary> 
        protected override void OnDetaching()
        {
            base.OnDetaching();
            //AssociatedObject.MouseMove -= AssociatedObject_MouseMove;
            //AssociatedObject.MouseLeave -= AssociatedObject_MouseLeave;
        }
    }
    public class ValidationExceptionForForm : Behavior<FrameworkElement>
    {
        private Dictionary<int, int> _dict = new Dictionary<int, int>();
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.AddHandler(Validation.ErrorEvent, new EventHandler<ValidationErrorEventArgs>(OnValidationError));
        }
        private void OnValidationError(object sender, ValidationErrorEventArgs e)
        {
            IFormValidationException handler = null;

            if (AssociatedObject.DataContext is IFormValidationException)
                handler = this.AssociatedObject.DataContext as IFormValidationException;

            if (handler == null) return;

            var element = e.OriginalSource as UIElement;
            if (element == null) return;
            if (e.Action == ValidationErrorEventAction.Added)
            {
                if ((element.GetType() == typeof(TextBox) && ((TextBox)element).Name == "workaddress") || element.GetType() == typeof(ComboBox))
                {
                    if (_dict.ContainsKey(-1))
                        _dict[-1] += 1;
                    else
                        _dict[-1] = 1;
                }

                if (element.GetType() == typeof(TextBox) && ((TextBox)element).ToolTip != null && ((TextBox)element).ToolTip.ToString() != String.Empty)
                {
                    string id = ((TextBox)element).ToolTip.ToString();
                    var idx = Convert.ToInt32(id);
                    if (_dict.ContainsKey(idx))
                        _dict[idx] += 1;
                    else
                        _dict[idx] = 1;
                }
            }
            else if (e.Action == ValidationErrorEventAction.Removed)
            {
                if ((element.GetType() == typeof(TextBox) && ((TextBox)element).Name == "workaddress") || element.GetType() == typeof(ComboBox))
                {
                    if (_dict.ContainsKey(-1))
                        _dict[-1] -= 1;
                    if (_dict[-1] == 0)
                        _dict.Remove(-1);
                }
                if (element.GetType() == typeof(TextBox) && ((TextBox)element).ToolTip != null && ((TextBox)element).ToolTip.ToString() != String.Empty)
                {
                    string id = ((TextBox)element).ToolTip.ToString();
                    var idx = Convert.ToInt32(id);
                    if (_dict.ContainsKey(idx))
                        _dict[idx] -= 1;
                    if (_dict[idx] == 0) _dict.Remove(idx);

                }
            }
            int sum = 0;
            foreach (var item in _dict)
                sum = sum + item.Value;
            handler.RowErrorCount = _dict;
            handler.FormErrorCount = sum;
        }
        /// <summary>
        /// 解除關聯的事件
        /// </summary> 
        protected override void OnDetaching()
        {
            base.OnDetaching();
            //AssociatedObject.MouseMove -= AssociatedObject_MouseMove;
            //AssociatedObject.MouseLeave -= AssociatedObject_MouseLeave;
        }
    }
}
