using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Photonware.RestUI.SL8.Xamls
{
    public class EyeableTextBlock : Control
    {
        private StackPanel stackPanel;
        private TextBlock measureBlock;

        private int mLineCount = 0;
        private int mMaxTexCount = 0;
        private const double KMaxTextBlockHeight = 2000;
        private const int KMaxTextLine = 5;

        public EyeableTextBlock()
        {
            // Get the style from generic.xaml
            this.DefaultStyleKey = typeof(EyeableTextBlock);
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                "Text",
                typeof(string),
                typeof(EyeableTextBlock),
                new PropertyMetadata("EyeableTextBlock", OnTextPropertyChanged));

        public string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }
            set
            {
                SetValue(TextProperty, value);
            }
        }

        private static void OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            EyeableTextBlock source = (EyeableTextBlock)d;
            string value = (string)e.NewValue;
            source.ParseText(value);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.stackPanel = this.GetTemplateChild("StackPanel") as StackPanel;
            this.ParseText(this.Text);
        }

        private void ParseText(string value)
        {
            StringReader reader = new StringReader(value);

            if (this.stackPanel == null)
            {
                return;
            }
            // Clear previous TextBlocks
            this.stackPanel.Children.Clear();
            if (String.IsNullOrEmpty(value))
            {
                return;
            }
            // Calculate max char count
            int maxTexCount = this.GetMaxTextSize();

            if (value.Length < maxTexCount)
            {
                TextBlock textBlock = this.GetTextBlock();
                textBlock.Text = value;
                if (textBlock.ActualHeight < KMaxTextBlockHeight) //文字少，但是回车换行多，高度超过一定的值还是会有内容不显示，仍然需要处理。
                {
                    this.stackPanel.Children.Add(textBlock);
                    return;
                }
            }

            const char newLine = '\n';
            StringBuilder sbLine = new StringBuilder();
            while (reader.Peek() > 0)
            {
                sbLine.Append(reader.ReadLine() + newLine);
                int lineCnt = 1; //已经取了一行
                while (lineCnt < KMaxTextLine && sbLine.Length < maxTexCount && reader.Peek() > 0)
                {
                    sbLine.Append(reader.ReadLine() + newLine);
                    lineCnt++;
                }
                ParseMultLine(sbLine.ToString().TrimEnd(newLine), lineCnt);
                sbLine.Clear();
            }
        }

        private void ParseMultLine(string lineText, int lineCnt)
        {
            TextBlock textBlock = this.GetTextBlock();
            textBlock.Text = lineText;
            if (textBlock.ActualHeight < KMaxTextBlockHeight) //文字少，但是回车换行多，高度超过一定的值还是会有内容不显示，仍然需要处理。
            {
                this.stackPanel.Children.Add(textBlock);
            }
            else //由于 lineCnt < KMaxTextLine && sbLine.Length < maxTexCount 的双重限制，需要else分支处理的概率很小。
            {
                StringReader reader = new StringReader(lineText);
                string line = "";

                while (reader.Peek() > 0)
                {
                    line = reader.ReadLine();
                    ParseLine(line);
                }
            }
        }

        private void ParseLine(string line)
        {
            int lineCount = 0;
            int maxLineCount = GetMaxLineCount();
            string tempLine = line;
            StringBuilder sbLine = new StringBuilder();

            while (lineCount < maxLineCount)
            {
                int charactersFitted = MeasureString(tempLine, (int)this.Width);
                string leftSide = tempLine.Substring(0, charactersFitted);
                sbLine.Append(leftSide);
                tempLine = tempLine.Substring(charactersFitted, tempLine.Length - (charactersFitted));
                lineCount++;
            }

            TextBlock textBlock = this.GetTextBlock();
            textBlock.Text = sbLine.ToString();
            this.stackPanel.Children.Add(textBlock);

            if (tempLine.Length > 0)
            {
                ParseLine(tempLine);
            }
        }

        private int MeasureString(string text, int desWidth)
        {

            int nWidth = 0;
            int charactersFitted = 0;

            StringBuilder sb = new StringBuilder();

            //get original size
            Size size = MeasureString(text);

            if (size.Width > desWidth)
            {
                string[] words = text.Split(' ');
                sb.Append(words[0]);

                for (int i = 1; i < words.Length; i++)
                {
                    sb.Append(" " + words[i]);
                    nWidth = (int)MeasureString(sb.ToString()).Width;

                    if (nWidth > desWidth)
                    {

                        sb.Remove(sb.Length - words[i].Length, words[i].Length);
                        break;
                    }
                }

                charactersFitted = sb.Length;
            }
            else
            {
                charactersFitted = text.Length;
            }

            return charactersFitted;
        }

        private Size MeasureString(string text)
        {
            if (this.measureBlock == null)
            {
                this.measureBlock = this.GetTextBlock();
            }

            this.measureBlock.Text = text;
            return new Size(measureBlock.ActualWidth, measureBlock.ActualHeight);
        }

        private int GetMaxTextSize()
        {
            if (mMaxTexCount > 0)
            {
                return mMaxTexCount;
            }
            // Get average char size
            Size size = this.MeasureText(" ");
            // Get number of char that fit in the line
            int charLineCount = (int)(this.Width / size.Width);
            // Get line count
            int lineCount = (int)(2048 / size.Height);

            //return charLineCount * lineCount / 2;
            mMaxTexCount = charLineCount * lineCount / 2;
            //调小最多字符数，最多1024个汉字。如果含有很多的回车换行符，即使字数很少也可能高度超过2000px。
            //在模拟器里面测试发现,使用UI调度器设置Text时,1600个汉字(含换行等)textBlock.ActualHeight计算并不准确(798px),导致显示不全，
            //建议根据实际情况调小最大值，不太影响效率即可。
            if (mMaxTexCount > 1024)
            {
                mMaxTexCount = 1024;
            }
            return mMaxTexCount;
        }

        private int GetMaxLineCount()
        {
            if (mLineCount > 0)
            {
                return mLineCount;
            }
            Size size = this.MeasureText(" ");
            // Get number of char that fit in the line
            int charLineCount = (int)(this.Width / size.Width);
            // Get line count
            int lineCount = (int)(2048 / size.Height) - 5;
            mLineCount = lineCount;

            return lineCount;
        }

        private TextBlock GetTextBlock()
        {
            TextBlock textBlock = new TextBlock();
            textBlock.TextWrapping = TextWrapping.Wrap;
            textBlock.FontSize = this.FontSize;
            textBlock.FontFamily = this.FontFamily;
            // textBlock.FontStyle = this.FontStyle;
            textBlock.FontWeight = this.FontWeight;
            textBlock.Foreground = this.Foreground;
            textBlock.Margin = new Thickness(0, 0, /*MeasureText(" ").Width*/5, 0);
            textBlock.Width = this.Width;
            return textBlock;
        }

        private Size MeasureText(string value)
        {
            TextBlock textBlock = this.GetTextBlockOnly();
            textBlock.Text = value;
            return new Size(textBlock.ActualWidth, textBlock.ActualHeight);
        }

        private TextBlock GetTextBlockOnly()
        {
            TextBlock textBlock = new TextBlock();
            textBlock.TextWrapping = TextWrapping.Wrap;
            textBlock.FontSize = this.FontSize;
            textBlock.FontFamily = this.FontFamily;
            textBlock.FontWeight = this.FontWeight;
            return textBlock;
        }
    }
}
