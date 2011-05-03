using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Text;
using System.Threading;
using Microsoft.Phone.Controls;
using book.install;
using System.Runtime.Serialization;
using System.Windows.Navigation;

namespace CrossConnect
{
    
    public partial class BrowserTitledWindow : UserControl
    {
        // An event that clients can use to be notified whenever the
        // elements of the list change.
        public event EventHandler HitButtonBigger;
        public event EventHandler HitButtonSmaller;
        public event EventHandler HitButtonClose;

        /// <summary>
        /// I was forced to make this class just for serialization because a "UserControl" 
        /// cannot be serialized.
        /// </summary>
        [DataContract]
        public class SerializableWindowState
        {
            [DataMember]
            public IBrowserTextSource source = null;
            [DataMember]
            public int curIndex = 0;
            [DataMember]
            public int numRowsIown = 1;
            [DataMember]
            public string bookToLoad ="";
            [DataMember]
            public int bookNum = 1;
            [DataMember]
            public int chapterNum = 1;
            [DataMember]
            public int verseNum = 1;
            [DataMember]
            public WINDOW_TYPE windowType = WINDOW_TYPE.WINDOW_BIBLE;
        }
        public SerializableWindowState state = new SerializableWindowState();

        public BrowserTitledWindow()
        {
            InitializeComponent();
        }
        public void Initialize(string bookToLoad, int bookNum, int chapterNum, WINDOW_TYPE windowType)
        {
            state.bookToLoad = bookToLoad;
            state.bookNum = bookNum;
            state.chapterNum = chapterNum;
            state.windowType = windowType;
            foreach (var book in App.installedBooks.installedBooks)
            {
                if (book.Value.sbmd.Initials.Equals(bookToLoad))
                {
                    string bookPath = book.Value.sbmd.getCetProperty(ConfigEntryType.DATA_PATH).ToString().Substring(2);
                    try
                    {
                        state.source = new BibleZtextReader(bookPath, ((Language)book.Value.sbmd.getCetProperty(ConfigEntryType.LANG)).Code);
                    }
                    catch (Exception)
                    {
                    }
                    
                    break;
                }
            } 
        }
        private void webBrowser1_Loaded(object sender, RoutedEventArgs e)
        {/*
            string text = "";
            for (int i = 1; i < 10; i++)
            {
                text += i + "<p>  <a class=\"normalcolor\"href=\"#\" onclick=\"window.external.Notify('hello=" + i + "'); event.returnValue=false; return false;\" >Lorem ipsum dolor sit amet, consectetur adipiscing elit. Proin sit amet iaculis est. Aenean vitae sapien tellus, eu scelerisque ipsum. Morbi et vehicula massa. Sed ut diam eget sem ornare congue. Duis pulvinar mi et tellus ultrices ultrices vitae et metus. Etiam consequat, diam eu sollicitudin venenatis, metus magna auctor metus, et blandit nisi ligula non tortor. Duis nec libero eget magna tempor vulputate. Mauris bibendum pretium diam, nec rutrum tellus pretium ac. Sed id dapibus eros. Praesent rutrum accumsan semper. Cras eros dolor, adipiscing quis tempus a, sagittis sollicitudin sem.</a></p> ";
            }

            */
            UpdateBrowser();

        }
        private void UpdateBrowser()
        {
            if (state.source != null)
            {
                string html = state.source.GetChapter(state.chapterNum);
                webBrowser1.NavigateToString(html);
                int bookNum;
                int relChaptNum;
                string fullName;
                state.source.getInfo(state.chapterNum, out bookNum, out relChaptNum, out fullName);
                title.Text = fullName + " " + (relChaptNum + 1);
            }
        }
        private static string GetBrowserColor(string sourceResource)
        {
            var color = (Color)Application.Current.Resources[sourceResource];
            return "#" + color.ToString().Substring(3, 6);
        }
        public static string HtmlHeader(double viewportWidth)
        {
            var head = new StringBuilder();
            head.Append("<head>");
            head.Append(string.Format(
                "<meta name=\"viewport\" value=\"width={0}\" user-scalable=\"no\">",
                viewportWidth));
            head.Append("<style>");
            head.Append("html { -ms-text-size-adjust:150% }");
            head.Append(string.Format(
                "body {{background:{0};color:{1};font-family:'Segoe WP';font-size:{2}pt;margin:0;padding:0 }}",
                GetBrowserColor("PhoneBackgroundColor"),
                GetBrowserColor("PhoneForegroundColor"),
                (double)Application.Current.Resources["PhoneFontSizeNormal"]));
            head.Append(string.Format(
                "a {{color:{0};text-decoration:none;}}",
                GetBrowserColor("PhoneForegroundColor")));
            //                GetBrowserColor("PhoneAccentColor")));


            head.Append(string.Format(
                ".highlightedcolor {{ color: {0}; }} .normalcolor:hover {{ color: {1}; }} ",
                GetBrowserColor("PhoneAccentColor"),
                GetBrowserColor("PhoneAccentColor")));

            head.Append("</style>");
            // head.Append(NotifyScript);
            head.Append("</head>");
            return head.ToString();
        }
        public static string WrapHtml(string htmlSubString, double viewportWidth)
        {
            var html = new StringBuilder();
            html.Append("<html>");
            html.Append(HtmlHeader(viewportWidth));
            html.Append("<body>");
            html.Append(htmlSubString);
            html.Append("</body>");
            html.Append("</html>");
            return html.ToString();
        }
        public static string NotifyScript
        {
            get
            {
                return @"<script>
window.onload = function()
{
  a = document.getElementsByTagName('a');
  for(var i=0; i < a.length; i++)
  {
    a[i].onclick = notify;
  };
}
function notify(e)
{
  window.external.Notify(this.href); 
  event.returnValue=false;
  return false;
}
</script>";
            }
        }


        public static void OpenBrowser(string url)
        {
            //titles[0].Text = url;
            // WebBrowserTask webBrowserTask = new WebBrowserTask { URL = url };
            // webBrowserTask.Show();
        }

        private void webBrowser1_ScriptNotify(object sender, Microsoft.Phone.Controls.NotifyEventArgs e)
        {
            //PivotHolder.SelectedIndex = PivotHolder.SelectedIndex + 1;
            //TextTitle1.Text = e.Value;
            //webBrowser1.Dispatcher.BeginInvoke(
            //    () => OpenBrowser(e.Value)
            //    );
        }

        private SlideTransition SlideTransitionElement(string mode)
        {
            SlideTransitionMode slideTransitionMode = (SlideTransitionMode)Enum.Parse(typeof(SlideTransitionMode), mode, false);
            return new SlideTransition { Mode = slideTransitionMode };
            //SlideTransitionMode slideTransitionMode = (SlideTransitionMode)Enum.Parse(typeof(SlideTransitionMode), mode, false);
            //return new SlideTransition { Mode = slideTransitionMode };
        }

        private void butPrevious_Click(object sender, RoutedEventArgs e)
        {
            state.chapterNum--;
            if (state.chapterNum < 0)
            {
                state.chapterNum = BibleZtextReader.CHAPTERS_IN_BIBLE-1;
            }
            string mode = "SlideRightFadeOut";
            TransitionElement transitionElement = null;

            transitionElement = SlideTransitionElement(mode);

            ITransition transition = transitionElement.GetTransition(this);

            transition.Completed += delegate
            {
                UpdateBrowser();
                transition.Stop();
                mode = "SlideRightFadeIn";
                transitionElement = null;

                transitionElement = SlideTransitionElement(mode);

                transition = transitionElement.GetTransition(this);
                transition.Completed += delegate
                {
                    transition.Stop();
                };
                transition.Begin();
            };
            transition.Begin();
        }

        private void butNext_Click(object sender, RoutedEventArgs e)
        {
            state.chapterNum++;
            if (state.chapterNum >= (BibleZtextReader.CHAPTERS_IN_BIBLE-1))
            {
                state.chapterNum = 0;
            } 
            string mode = "SlideLeftFadeOut";
            TransitionElement transitionElement = null;

            transitionElement = SlideTransitionElement(mode);

            PhoneApplicationPage phoneApplicationPage = (PhoneApplicationPage)(((PhoneApplicationFrame)Application.Current.RootVisual)).Content;
            ITransition transition = transitionElement.GetTransition(this);
            transition.Completed += delegate
            {
                UpdateBrowser();
                transition.Stop();
                mode = "SlideLeftFadeIn";
                transitionElement = null;

                transitionElement = SlideTransitionElement(mode);

                transition = transitionElement.GetTransition(this);
                transition.Completed += delegate
                {
                    transition.Stop();
                };
                transition.Begin();
            };
            transition.Begin();
        }

        private void butMenu_Click(object sender, RoutedEventArgs e)
        {
            App.windowSettings.openWindowIndex = state.curIndex;
            MainPageSplit parent = (MainPageSplit)((Grid)((Grid)this.Parent).Parent).Parent;
            parent.NavigationService.Navigate(new Uri("/WindowSettings.xaml", UriKind.Relative));
        }

        private void butLink_Click(object sender, RoutedEventArgs e)
        {

        }
        private void butLarger_Click(object sender, RoutedEventArgs e)
        {
            state.numRowsIown++;
            ShowSizeButtons();
            if (HitButtonBigger != null)
                HitButtonBigger(this, e);
        }

        private void butSmaller_Click(object sender, RoutedEventArgs e)
        {
            state.numRowsIown--;
            ShowSizeButtons();
            if (HitButtonSmaller != null)
                HitButtonSmaller(this, e);
        }

        private void butClose_Click(object sender, RoutedEventArgs e)
        {
            if (HitButtonClose != null)
                HitButtonClose(this, e);
        }
        public void ShowSizeButtons(bool isShow=true)
        {
            if (!isShow)
            {
                butLarger.Visibility = System.Windows.Visibility.Collapsed;
                butSmaller.Visibility = System.Windows.Visibility.Collapsed;
                butClose.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                if (state.numRowsIown > 1)
                {
                    butSmaller.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    butSmaller.Visibility = System.Windows.Visibility.Collapsed;
                }
                butLarger.Visibility = System.Windows.Visibility.Visible;
                butClose.Visibility = System.Windows.Visibility.Visible;
            }
        }
    }
}
