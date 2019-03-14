using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AppUI
{
    public class TextBoxProperties
    {
        public static readonly DependencyProperty FormatTextProperty = DependencyProperty.RegisterAttached("FormatText", typeof(bool), typeof(TextBoxProperties), new PropertyMetadata(false, OnCallback));

        private static void OnCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textBox = (d as TextBox);

            if (textBox == null)
                return;

            if((bool)e.NewValue)
            {
                textBox.PreviewTextInput += TextBox_PreviewTextInput;
                textBox.LostFocus += TextBox_LostFocus;
            }
        }
        
        private static void TextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex _regex;
            var textBox = (TextBox)sender;

            if (textBox.Text.Contains(","))
            {                
               _regex = new Regex(@"[^0-9]");
            }
            else
            {
               _regex = new Regex(@"[^0-9\,]");                
            }
                e.Handled = _regex.IsMatch(e.Text); 
        }

        private static void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = (TextBox)sender;
            
            if (!textBox.Text.Contains(",") && !string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text += ",00";
            } 
            else if(textBox.Text.IndexOf(",") == 0)
            {
                string s = textBox.Text.Insert(0, "0");
                textBox.Text = s;
            }           
        }

        public static void SetFormatText(TextBox control, bool value)
        {
            control.SetValue(FormatTextProperty, value);
        }

        public static bool GetFormatText(TextBox control)
        {
            return (bool)control.GetValue(FormatTextProperty);
        }
    }
}
