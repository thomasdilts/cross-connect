namespace CrossConnect
{
    using System;
    using System.IO.IsolatedStorage;
    using System.Windows;
    using System.Windows.Controls;

    public partial class SelectLanguage : AutoRotatePage
    {
        #region Fields

        private static string[] SUPPORTED_LANGUAGES = 
        {
        "default",
        "af",
        "ar",
        "az",
        "be",
        "bg",
        "cs",
        "da",
        "de",
        "el",
        "en",
        "es",
        "et",
        "fa",
        "fi",
        "fr",
        "hi",
        "hr",
        "hu",
        "hy",
        "id",
        "is",
        "it",
        "iw",
        "ja",
        "ko",
        "lt",
        "lv",
        "mk",
        "ms",
        "mt",
        "nl",
        "no",
        "pl",
        "pt",
        "ro",
        "ru",
        "sk",
        "sl",
        "sq",
        "sr",
        "sw",
        "sv",
        "th",
        "tl",
        "tr",
        "uk",
        "ur",
        "vi",
        "zh",
        "zh_cn",
        };

        private Boolean isInUpdateList = false;

        #endregion Fields

        #region Constructors

        public SelectLanguage()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Methods

        private void AutoRotatePage_Loaded(object sender, RoutedEventArgs e)
        {
            PageTitle.Text = Translations.translate("Select the language");
            SelectList.Items.RemoveAt(0);
            SelectList.Items.Insert(0, Translations.translate("Default system language"));

            isInUpdateList = true;
            string isoLanguageCode = (string)IsolatedStorageSettings.ApplicationSettings["LanguageIsoCode"];
            int i=0;
            for (i = 0; i < SUPPORTED_LANGUAGES.Length; i++)
            {
                if (SUPPORTED_LANGUAGES[i].Equals(isoLanguageCode))
                {
                    SelectList.SelectedIndex = i;
                    break;
                }
            }
            if (i == SUPPORTED_LANGUAGES.Length)
            {
                SelectList.SelectedIndex = 0;
            }
            isInUpdateList = false;
        }

        private void SelectList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isInUpdateList)
            {
                Translations.isoLanguageCode = SUPPORTED_LANGUAGES[SelectList.SelectedIndex];
                if (NavigationService.CanGoBack)
                {
                    NavigationService.GoBack();
                }
            }
        }

        #endregion Methods
    }
}