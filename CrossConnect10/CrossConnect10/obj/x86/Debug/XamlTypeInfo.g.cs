﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------



namespace CrossConnect
{
    public partial class App : global::Windows.UI.Xaml.Markup.IXamlMetadataProvider
    {
    private global::CrossConnect.CrossConnect10_XamlTypeInfo.XamlTypeInfoProvider _provider;

        /// <summary>
        /// GetXamlType(Type)
        /// </summary>
        public global::Windows.UI.Xaml.Markup.IXamlType GetXamlType(global::System.Type type)
        {
            if(_provider == null)
            {
                _provider = new global::CrossConnect.CrossConnect10_XamlTypeInfo.XamlTypeInfoProvider();
            }
            return _provider.GetXamlTypeByType(type);
        }

        /// <summary>
        /// GetXamlType(String)
        /// </summary>
        public global::Windows.UI.Xaml.Markup.IXamlType GetXamlType(string fullName)
        {
            if(_provider == null)
            {
                _provider = new global::CrossConnect.CrossConnect10_XamlTypeInfo.XamlTypeInfoProvider();
            }
            return _provider.GetXamlTypeByName(fullName);
        }

        /// <summary>
        /// GetXmlnsDefinitions()
        /// </summary>
        public global::Windows.UI.Xaml.Markup.XmlnsDefinition[] GetXmlnsDefinitions()
        {
            return new global::Windows.UI.Xaml.Markup.XmlnsDefinition[0];
        }
    }
}

namespace CrossConnect.CrossConnect10_XamlTypeInfo
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 14.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    internal partial class XamlTypeInfoProvider
    {
        public global::Windows.UI.Xaml.Markup.IXamlType GetXamlTypeByType(global::System.Type type)
        {
            global::Windows.UI.Xaml.Markup.IXamlType xamlType;
            if (_xamlTypeCacheByType.TryGetValue(type, out xamlType))
            {
                return xamlType;
            }
            int typeIndex = LookupTypeIndexByType(type);
            if(typeIndex != -1)
            {
                xamlType = CreateXamlType(typeIndex);
            }
            if (xamlType != null)
            {
                _xamlTypeCacheByName.Add(xamlType.FullName, xamlType);
                _xamlTypeCacheByType.Add(xamlType.UnderlyingType, xamlType);
            }
            return xamlType;
        }

        public global::Windows.UI.Xaml.Markup.IXamlType GetXamlTypeByName(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                return null;
            }
            global::Windows.UI.Xaml.Markup.IXamlType xamlType;
            if (_xamlTypeCacheByName.TryGetValue(typeName, out xamlType))
            {
                return xamlType;
            }
            int typeIndex = LookupTypeIndexByName(typeName);
            if(typeIndex != -1)
            {
                xamlType = CreateXamlType(typeIndex);
            }
            if (xamlType != null)
            {
                _xamlTypeCacheByName.Add(xamlType.FullName, xamlType);
                _xamlTypeCacheByType.Add(xamlType.UnderlyingType, xamlType);
            }
            return xamlType;
        }

        public global::Windows.UI.Xaml.Markup.IXamlMember GetMemberByLongName(string longMemberName)
        {
            if (string.IsNullOrEmpty(longMemberName))
            {
                return null;
            }
            global::Windows.UI.Xaml.Markup.IXamlMember xamlMember;
            if (_xamlMembers.TryGetValue(longMemberName, out xamlMember))
            {
                return xamlMember;
            }
            xamlMember = CreateXamlMember(longMemberName);
            if (xamlMember != null)
            {
                _xamlMembers.Add(longMemberName, xamlMember);
            }
            return xamlMember;
        }

        global::System.Collections.Generic.Dictionary<string, global::Windows.UI.Xaml.Markup.IXamlType>
                _xamlTypeCacheByName = new global::System.Collections.Generic.Dictionary<string, global::Windows.UI.Xaml.Markup.IXamlType>();

        global::System.Collections.Generic.Dictionary<global::System.Type, global::Windows.UI.Xaml.Markup.IXamlType>
                _xamlTypeCacheByType = new global::System.Collections.Generic.Dictionary<global::System.Type, global::Windows.UI.Xaml.Markup.IXamlType>();

        global::System.Collections.Generic.Dictionary<string, global::Windows.UI.Xaml.Markup.IXamlMember>
                _xamlMembers = new global::System.Collections.Generic.Dictionary<string, global::Windows.UI.Xaml.Markup.IXamlMember>();

        string[] _typeNameTable = null;
        global::System.Type[] _typeTable = null;

        private void InitTypeTables()
        {
            _typeNameTable = new string[18];
            _typeNameTable[0] = "CrossConnect.Common.LayoutAwarePage";
            _typeNameTable[1] = "Windows.UI.Xaml.Controls.Page";
            _typeNameTable[2] = "Windows.UI.Xaml.Controls.UserControl";
            _typeNameTable[3] = "CrossConnect.AddNote";
            _typeNameTable[4] = "CrossConnect.BrowserTitledWindow";
            _typeNameTable[5] = "Boolean";
            _typeNameTable[6] = "CrossConnect.SerializableWindowState";
            _typeNameTable[7] = "Object";
            _typeNameTable[8] = "CrossConnect.ColorPicker";
            _typeNameTable[9] = "Windows.UI.Color";
            _typeNameTable[10] = "System.ValueType";
            _typeNameTable[11] = "String";
            _typeNameTable[12] = "CrossConnect.EasySlider";
            _typeNameTable[13] = "Double";
            _typeNameTable[14] = "CrossConnect.SelectBibleVerses";
            _typeNameTable[15] = "CrossConnect.MainPageSplit";
            _typeNameTable[16] = "CrossConnect.MediaPlayerWindow";
            _typeNameTable[17] = "CrossConnect.SelectBibleVerse";

            _typeTable = new global::System.Type[18];
            _typeTable[0] = typeof(global::CrossConnect.Common.LayoutAwarePage);
            _typeTable[1] = typeof(global::Windows.UI.Xaml.Controls.Page);
            _typeTable[2] = typeof(global::Windows.UI.Xaml.Controls.UserControl);
            _typeTable[3] = typeof(global::CrossConnect.AddNote);
            _typeTable[4] = typeof(global::CrossConnect.BrowserTitledWindow);
            _typeTable[5] = typeof(global::System.Boolean);
            _typeTable[6] = typeof(global::CrossConnect.SerializableWindowState);
            _typeTable[7] = typeof(global::System.Object);
            _typeTable[8] = typeof(global::CrossConnect.ColorPicker);
            _typeTable[9] = typeof(global::Windows.UI.Color);
            _typeTable[10] = typeof(global::System.ValueType);
            _typeTable[11] = typeof(global::System.String);
            _typeTable[12] = typeof(global::CrossConnect.EasySlider);
            _typeTable[13] = typeof(global::System.Double);
            _typeTable[14] = typeof(global::CrossConnect.SelectBibleVerses);
            _typeTable[15] = typeof(global::CrossConnect.MainPageSplit);
            _typeTable[16] = typeof(global::CrossConnect.MediaPlayerWindow);
            _typeTable[17] = typeof(global::CrossConnect.SelectBibleVerse);
        }

        private int LookupTypeIndexByName(string typeName)
        {
            if (_typeNameTable == null)
            {
                InitTypeTables();
            }
            for (int i=0; i<_typeNameTable.Length; i++)
            {
                if(0 == string.CompareOrdinal(_typeNameTable[i], typeName))
                {
                    return i;
                }
            }
            return -1;
        }

        private int LookupTypeIndexByType(global::System.Type type)
        {
            if (_typeTable == null)
            {
                InitTypeTables();
            }
            for(int i=0; i<_typeTable.Length; i++)
            {
                if(type == _typeTable[i])
                {
                    return i;
                }
            }
            return -1;
        }

        private object Activate_0_LayoutAwarePage() { return new global::CrossConnect.Common.LayoutAwarePage(); }
        private object Activate_3_AddNote() { return new global::CrossConnect.AddNote(); }
        private object Activate_4_BrowserTitledWindow() { return new global::CrossConnect.BrowserTitledWindow(); }
        private object Activate_6_SerializableWindowState() { return new global::CrossConnect.SerializableWindowState(); }
        private object Activate_8_ColorPicker() { return new global::CrossConnect.ColorPicker(); }
        private object Activate_12_EasySlider() { return new global::CrossConnect.EasySlider(); }
        private object Activate_14_SelectBibleVerses() { return new global::CrossConnect.SelectBibleVerses(); }
        private object Activate_15_MainPageSplit() { return new global::CrossConnect.MainPageSplit(); }
        private object Activate_16_MediaPlayerWindow() { return new global::CrossConnect.MediaPlayerWindow(); }
        private object Activate_17_SelectBibleVerse() { return new global::CrossConnect.SelectBibleVerse(); }

        private global::Windows.UI.Xaml.Markup.IXamlType CreateXamlType(int typeIndex)
        {
            global::CrossConnect.CrossConnect10_XamlTypeInfo.XamlSystemBaseType xamlType = null;
            global::CrossConnect.CrossConnect10_XamlTypeInfo.XamlUserType userType;
            string typeName = _typeNameTable[typeIndex];
            global::System.Type type = _typeTable[typeIndex];

            switch (typeIndex)
            {

            case 0:   //  CrossConnect.Common.LayoutAwarePage
                userType = new global::CrossConnect.CrossConnect10_XamlTypeInfo.XamlUserType(this, typeName, type, GetXamlTypeByName("Windows.UI.Xaml.Controls.Page"));
                userType.Activator = Activate_0_LayoutAwarePage;
                userType.SetIsLocalType();
                xamlType = userType;
                break;

            case 1:   //  Windows.UI.Xaml.Controls.Page
                xamlType = new global::CrossConnect.CrossConnect10_XamlTypeInfo.XamlSystemBaseType(typeName, type);
                break;

            case 2:   //  Windows.UI.Xaml.Controls.UserControl
                xamlType = new global::CrossConnect.CrossConnect10_XamlTypeInfo.XamlSystemBaseType(typeName, type);
                break;

            case 3:   //  CrossConnect.AddNote
                userType = new global::CrossConnect.CrossConnect10_XamlTypeInfo.XamlUserType(this, typeName, type, GetXamlTypeByName("CrossConnect.Common.LayoutAwarePage"));
                userType.Activator = Activate_3_AddNote;
                userType.SetIsLocalType();
                xamlType = userType;
                break;

            case 4:   //  CrossConnect.BrowserTitledWindow
                userType = new global::CrossConnect.CrossConnect10_XamlTypeInfo.XamlUserType(this, typeName, type, GetXamlTypeByName("Windows.UI.Xaml.Controls.UserControl"));
                userType.Activator = Activate_4_BrowserTitledWindow;
                userType.AddMemberName("ForceReload");
                userType.AddMemberName("State");
                userType.SetIsLocalType();
                xamlType = userType;
                break;

            case 5:   //  Boolean
                xamlType = new global::CrossConnect.CrossConnect10_XamlTypeInfo.XamlSystemBaseType(typeName, type);
                break;

            case 6:   //  CrossConnect.SerializableWindowState
                userType = new global::CrossConnect.CrossConnect10_XamlTypeInfo.XamlUserType(this, typeName, type, GetXamlTypeByName("Object"));
                userType.SetIsReturnTypeStub();
                userType.SetIsLocalType();
                xamlType = userType;
                break;

            case 7:   //  Object
                xamlType = new global::CrossConnect.CrossConnect10_XamlTypeInfo.XamlSystemBaseType(typeName, type);
                break;

            case 8:   //  CrossConnect.ColorPicker
                userType = new global::CrossConnect.CrossConnect10_XamlTypeInfo.XamlUserType(this, typeName, type, GetXamlTypeByName("Windows.UI.Xaml.Controls.UserControl"));
                userType.Activator = Activate_8_ColorPicker;
                userType.AddMemberName("ColorPicked");
                userType.AddMemberName("TextCaption");
                userType.SetIsLocalType();
                xamlType = userType;
                break;

            case 9:   //  Windows.UI.Color
                userType = new global::CrossConnect.CrossConnect10_XamlTypeInfo.XamlUserType(this, typeName, type, GetXamlTypeByName("System.ValueType"));
                userType.SetIsReturnTypeStub();
                xamlType = userType;
                break;

            case 10:   //  System.ValueType
                userType = new global::CrossConnect.CrossConnect10_XamlTypeInfo.XamlUserType(this, typeName, type, GetXamlTypeByName("Object"));
                xamlType = userType;
                break;

            case 11:   //  String
                xamlType = new global::CrossConnect.CrossConnect10_XamlTypeInfo.XamlSystemBaseType(typeName, type);
                break;

            case 12:   //  CrossConnect.EasySlider
                userType = new global::CrossConnect.CrossConnect10_XamlTypeInfo.XamlUserType(this, typeName, type, GetXamlTypeByName("Windows.UI.Xaml.Controls.UserControl"));
                userType.Activator = Activate_12_EasySlider;
                userType.AddMemberName("SliderValue");
                userType.SetIsLocalType();
                xamlType = userType;
                break;

            case 13:   //  Double
                xamlType = new global::CrossConnect.CrossConnect10_XamlTypeInfo.XamlSystemBaseType(typeName, type);
                break;

            case 14:   //  CrossConnect.SelectBibleVerses
                userType = new global::CrossConnect.CrossConnect10_XamlTypeInfo.XamlUserType(this, typeName, type, GetXamlTypeByName("Windows.UI.Xaml.Controls.UserControl"));
                userType.Activator = Activate_14_SelectBibleVerses;
                userType.SetIsLocalType();
                xamlType = userType;
                break;

            case 15:   //  CrossConnect.MainPageSplit
                userType = new global::CrossConnect.CrossConnect10_XamlTypeInfo.XamlUserType(this, typeName, type, GetXamlTypeByName("CrossConnect.Common.LayoutAwarePage"));
                userType.Activator = Activate_15_MainPageSplit;
                userType.SetIsLocalType();
                xamlType = userType;
                break;

            case 16:   //  CrossConnect.MediaPlayerWindow
                userType = new global::CrossConnect.CrossConnect10_XamlTypeInfo.XamlUserType(this, typeName, type, GetXamlTypeByName("Windows.UI.Xaml.Controls.UserControl"));
                userType.Activator = Activate_16_MediaPlayerWindow;
                userType.AddMemberName("ForceReload");
                userType.AddMemberName("State");
                userType.SetIsLocalType();
                xamlType = userType;
                break;

            case 17:   //  CrossConnect.SelectBibleVerse
                userType = new global::CrossConnect.CrossConnect10_XamlTypeInfo.XamlUserType(this, typeName, type, GetXamlTypeByName("Windows.UI.Xaml.Controls.UserControl"));
                userType.Activator = Activate_17_SelectBibleVerse;
                userType.SetIsLocalType();
                xamlType = userType;
                break;
            }
            return xamlType;
        }


        private object get_0_BrowserTitledWindow_ForceReload(object instance)
        {
            var that = (global::CrossConnect.BrowserTitledWindow)instance;
            return that.ForceReload;
        }
        private void set_0_BrowserTitledWindow_ForceReload(object instance, object Value)
        {
            var that = (global::CrossConnect.BrowserTitledWindow)instance;
            that.ForceReload = (global::System.Boolean)Value;
        }
        private object get_1_BrowserTitledWindow_State(object instance)
        {
            var that = (global::CrossConnect.BrowserTitledWindow)instance;
            return that.State;
        }
        private void set_1_BrowserTitledWindow_State(object instance, object Value)
        {
            var that = (global::CrossConnect.BrowserTitledWindow)instance;
            that.State = (global::CrossConnect.SerializableWindowState)Value;
        }
        private object get_2_ColorPicker_ColorPicked(object instance)
        {
            var that = (global::CrossConnect.ColorPicker)instance;
            return that.ColorPicked;
        }
        private void set_2_ColorPicker_ColorPicked(object instance, object Value)
        {
            var that = (global::CrossConnect.ColorPicker)instance;
            that.ColorPicked = (global::Windows.UI.Color)Value;
        }
        private object get_3_ColorPicker_TextCaption(object instance)
        {
            var that = (global::CrossConnect.ColorPicker)instance;
            return that.TextCaption;
        }
        private void set_3_ColorPicker_TextCaption(object instance, object Value)
        {
            var that = (global::CrossConnect.ColorPicker)instance;
            that.TextCaption = (global::System.String)Value;
        }
        private object get_4_EasySlider_SliderValue(object instance)
        {
            var that = (global::CrossConnect.EasySlider)instance;
            return that.SliderValue;
        }
        private void set_4_EasySlider_SliderValue(object instance, object Value)
        {
            var that = (global::CrossConnect.EasySlider)instance;
            that.SliderValue = (global::System.Double)Value;
        }
        private object get_5_MediaPlayerWindow_ForceReload(object instance)
        {
            var that = (global::CrossConnect.MediaPlayerWindow)instance;
            return that.ForceReload;
        }
        private void set_5_MediaPlayerWindow_ForceReload(object instance, object Value)
        {
            var that = (global::CrossConnect.MediaPlayerWindow)instance;
            that.ForceReload = (global::System.Boolean)Value;
        }
        private object get_6_MediaPlayerWindow_State(object instance)
        {
            var that = (global::CrossConnect.MediaPlayerWindow)instance;
            return that.State;
        }
        private void set_6_MediaPlayerWindow_State(object instance, object Value)
        {
            var that = (global::CrossConnect.MediaPlayerWindow)instance;
            that.State = (global::CrossConnect.SerializableWindowState)Value;
        }

        private global::Windows.UI.Xaml.Markup.IXamlMember CreateXamlMember(string longMemberName)
        {
            global::CrossConnect.CrossConnect10_XamlTypeInfo.XamlMember xamlMember = null;
            global::CrossConnect.CrossConnect10_XamlTypeInfo.XamlUserType userType;

            switch (longMemberName)
            {
            case "CrossConnect.BrowserTitledWindow.ForceReload":
                userType = (global::CrossConnect.CrossConnect10_XamlTypeInfo.XamlUserType)GetXamlTypeByName("CrossConnect.BrowserTitledWindow");
                xamlMember = new global::CrossConnect.CrossConnect10_XamlTypeInfo.XamlMember(this, "ForceReload", "Boolean");
                xamlMember.Getter = get_0_BrowserTitledWindow_ForceReload;
                xamlMember.Setter = set_0_BrowserTitledWindow_ForceReload;
                break;
            case "CrossConnect.BrowserTitledWindow.State":
                userType = (global::CrossConnect.CrossConnect10_XamlTypeInfo.XamlUserType)GetXamlTypeByName("CrossConnect.BrowserTitledWindow");
                xamlMember = new global::CrossConnect.CrossConnect10_XamlTypeInfo.XamlMember(this, "State", "CrossConnect.SerializableWindowState");
                xamlMember.Getter = get_1_BrowserTitledWindow_State;
                xamlMember.Setter = set_1_BrowserTitledWindow_State;
                break;
            case "CrossConnect.ColorPicker.ColorPicked":
                userType = (global::CrossConnect.CrossConnect10_XamlTypeInfo.XamlUserType)GetXamlTypeByName("CrossConnect.ColorPicker");
                xamlMember = new global::CrossConnect.CrossConnect10_XamlTypeInfo.XamlMember(this, "ColorPicked", "Windows.UI.Color");
                xamlMember.Getter = get_2_ColorPicker_ColorPicked;
                xamlMember.Setter = set_2_ColorPicker_ColorPicked;
                break;
            case "CrossConnect.ColorPicker.TextCaption":
                userType = (global::CrossConnect.CrossConnect10_XamlTypeInfo.XamlUserType)GetXamlTypeByName("CrossConnect.ColorPicker");
                xamlMember = new global::CrossConnect.CrossConnect10_XamlTypeInfo.XamlMember(this, "TextCaption", "String");
                xamlMember.Getter = get_3_ColorPicker_TextCaption;
                xamlMember.Setter = set_3_ColorPicker_TextCaption;
                break;
            case "CrossConnect.EasySlider.SliderValue":
                userType = (global::CrossConnect.CrossConnect10_XamlTypeInfo.XamlUserType)GetXamlTypeByName("CrossConnect.EasySlider");
                xamlMember = new global::CrossConnect.CrossConnect10_XamlTypeInfo.XamlMember(this, "SliderValue", "Double");
                xamlMember.Getter = get_4_EasySlider_SliderValue;
                xamlMember.Setter = set_4_EasySlider_SliderValue;
                break;
            case "CrossConnect.MediaPlayerWindow.ForceReload":
                userType = (global::CrossConnect.CrossConnect10_XamlTypeInfo.XamlUserType)GetXamlTypeByName("CrossConnect.MediaPlayerWindow");
                xamlMember = new global::CrossConnect.CrossConnect10_XamlTypeInfo.XamlMember(this, "ForceReload", "Boolean");
                xamlMember.Getter = get_5_MediaPlayerWindow_ForceReload;
                xamlMember.Setter = set_5_MediaPlayerWindow_ForceReload;
                break;
            case "CrossConnect.MediaPlayerWindow.State":
                userType = (global::CrossConnect.CrossConnect10_XamlTypeInfo.XamlUserType)GetXamlTypeByName("CrossConnect.MediaPlayerWindow");
                xamlMember = new global::CrossConnect.CrossConnect10_XamlTypeInfo.XamlMember(this, "State", "CrossConnect.SerializableWindowState");
                xamlMember.Getter = get_6_MediaPlayerWindow_State;
                xamlMember.Setter = set_6_MediaPlayerWindow_State;
                break;
            }
            return xamlMember;
        }
    }

    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 14.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    internal class XamlSystemBaseType : global::Windows.UI.Xaml.Markup.IXamlType
    {
        string _fullName;
        global::System.Type _underlyingType;

        public XamlSystemBaseType(string fullName, global::System.Type underlyingType)
        {
            _fullName = fullName;
            _underlyingType = underlyingType;
        }

        public string FullName { get { return _fullName; } }

        public global::System.Type UnderlyingType
        {
            get
            {
                return _underlyingType;
            }
        }

        virtual public global::Windows.UI.Xaml.Markup.IXamlType BaseType { get { throw new global::System.NotImplementedException(); } }
        virtual public global::Windows.UI.Xaml.Markup.IXamlMember ContentProperty { get { throw new global::System.NotImplementedException(); } }
        virtual public global::Windows.UI.Xaml.Markup.IXamlMember GetMember(string name) { throw new global::System.NotImplementedException(); }
        virtual public bool IsArray { get { throw new global::System.NotImplementedException(); } }
        virtual public bool IsCollection { get { throw new global::System.NotImplementedException(); } }
        virtual public bool IsConstructible { get { throw new global::System.NotImplementedException(); } }
        virtual public bool IsDictionary { get { throw new global::System.NotImplementedException(); } }
        virtual public bool IsMarkupExtension { get { throw new global::System.NotImplementedException(); } }
        virtual public bool IsBindable { get { throw new global::System.NotImplementedException(); } }
        virtual public bool IsReturnTypeStub { get { throw new global::System.NotImplementedException(); } }
        virtual public bool IsLocalType { get { throw new global::System.NotImplementedException(); } }
        virtual public global::Windows.UI.Xaml.Markup.IXamlType ItemType { get { throw new global::System.NotImplementedException(); } }
        virtual public global::Windows.UI.Xaml.Markup.IXamlType KeyType { get { throw new global::System.NotImplementedException(); } }
        virtual public object ActivateInstance() { throw new global::System.NotImplementedException(); }
        virtual public void AddToMap(object instance, object key, object item)  { throw new global::System.NotImplementedException(); }
        virtual public void AddToVector(object instance, object item)  { throw new global::System.NotImplementedException(); }
        virtual public void RunInitializer()   { throw new global::System.NotImplementedException(); }
        virtual public object CreateFromString(string input)   { throw new global::System.NotImplementedException(); }
    }
    
    internal delegate object Activator();
    internal delegate void AddToCollection(object instance, object item);
    internal delegate void AddToDictionary(object instance, object key, object item);

    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 14.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    internal class XamlUserType : global::CrossConnect.CrossConnect10_XamlTypeInfo.XamlSystemBaseType
    {
        global::CrossConnect.CrossConnect10_XamlTypeInfo.XamlTypeInfoProvider _provider;
        global::Windows.UI.Xaml.Markup.IXamlType _baseType;
        bool _isArray;
        bool _isMarkupExtension;
        bool _isBindable;
        bool _isReturnTypeStub;
        bool _isLocalType;

        string _contentPropertyName;
        string _itemTypeName;
        string _keyTypeName;
        global::System.Collections.Generic.Dictionary<string, string> _memberNames;
        global::System.Collections.Generic.Dictionary<string, object> _enumValues;

        public XamlUserType(global::CrossConnect.CrossConnect10_XamlTypeInfo.XamlTypeInfoProvider provider, string fullName, global::System.Type fullType, global::Windows.UI.Xaml.Markup.IXamlType baseType)
            :base(fullName, fullType)
        {
            _provider = provider;
            _baseType = baseType;
        }

        // --- Interface methods ----

        override public global::Windows.UI.Xaml.Markup.IXamlType BaseType { get { return _baseType; } }
        override public bool IsArray { get { return _isArray; } }
        override public bool IsCollection { get { return (CollectionAdd != null); } }
        override public bool IsConstructible { get { return (Activator != null); } }
        override public bool IsDictionary { get { return (DictionaryAdd != null); } }
        override public bool IsMarkupExtension { get { return _isMarkupExtension; } }
        override public bool IsBindable { get { return _isBindable; } }
        override public bool IsReturnTypeStub { get { return _isReturnTypeStub; } }
        override public bool IsLocalType { get { return _isLocalType; } }

        override public global::Windows.UI.Xaml.Markup.IXamlMember ContentProperty
        {
            get { return _provider.GetMemberByLongName(_contentPropertyName); }
        }

        override public global::Windows.UI.Xaml.Markup.IXamlType ItemType
        {
            get { return _provider.GetXamlTypeByName(_itemTypeName); }
        }

        override public global::Windows.UI.Xaml.Markup.IXamlType KeyType
        {
            get { return _provider.GetXamlTypeByName(_keyTypeName); }
        }

        override public global::Windows.UI.Xaml.Markup.IXamlMember GetMember(string name)
        {
            if (_memberNames == null)
            {
                return null;
            }
            string longName;
            if (_memberNames.TryGetValue(name, out longName))
            {
                return _provider.GetMemberByLongName(longName);
            }
            return null;
        }

        override public object ActivateInstance()
        {
            return Activator(); 
        }

        override public void AddToMap(object instance, object key, object item) 
        {
            DictionaryAdd(instance, key, item);
        }

        override public void AddToVector(object instance, object item)
        {
            CollectionAdd(instance, item);
        }

        override public void RunInitializer() 
        {
            System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(UnderlyingType.TypeHandle);
        }

        override public object CreateFromString(string input)
        {
            if (_enumValues != null)
            {
                int value = 0;

                string[] valueParts = input.Split(',');

                foreach (string valuePart in valueParts) 
                {
                    object partValue;
                    int enumFieldValue = 0;
                    try
                    {
                        if (_enumValues.TryGetValue(valuePart.Trim(), out partValue))
                        {
                            enumFieldValue = global::System.Convert.ToInt32(partValue);
                        }
                        else
                        {
                            try
                            {
                                enumFieldValue = global::System.Convert.ToInt32(valuePart.Trim());
                            }
                            catch( global::System.FormatException )
                            {
                                foreach( string key in _enumValues.Keys )
                                {
                                    if( string.Compare(valuePart.Trim(), key, global::System.StringComparison.OrdinalIgnoreCase) == 0 )
                                    {
                                        if( _enumValues.TryGetValue(key.Trim(), out partValue) )
                                        {
                                            enumFieldValue = global::System.Convert.ToInt32(partValue);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        value |= enumFieldValue; 
                    }
                    catch( global::System.FormatException )
                    {
                        throw new global::System.ArgumentException(input, FullName);
                    }
                }

                return value; 
            }
            throw new global::System.ArgumentException(input, FullName);
        }

        // --- End of Interface methods

        public Activator Activator { get; set; }
        public AddToCollection CollectionAdd { get; set; }
        public AddToDictionary DictionaryAdd { get; set; }

        public void SetContentPropertyName(string contentPropertyName)
        {
            _contentPropertyName = contentPropertyName;
        }

        public void SetIsArray()
        {
            _isArray = true; 
        }

        public void SetIsMarkupExtension()
        {
            _isMarkupExtension = true;
        }

        public void SetIsBindable()
        {
            _isBindable = true;
        }

        public void SetIsReturnTypeStub()
        {
            _isReturnTypeStub = true;
        }

        public void SetIsLocalType()
        {
            _isLocalType = true;
        }

        public void SetItemTypeName(string itemTypeName)
        {
            _itemTypeName = itemTypeName;
        }

        public void SetKeyTypeName(string keyTypeName)
        {
            _keyTypeName = keyTypeName;
        }

        public void AddMemberName(string shortName)
        {
            if(_memberNames == null)
            {
                _memberNames =  new global::System.Collections.Generic.Dictionary<string,string>();
            }
            _memberNames.Add(shortName, FullName + "." + shortName);
        }

        public void AddEnumValue(string name, object value)
        {
            if (_enumValues == null)
            {
                _enumValues = new global::System.Collections.Generic.Dictionary<string, object>();
            }
            _enumValues.Add(name, value);
        }
    }

    internal delegate object Getter(object instance);
    internal delegate void Setter(object instance, object value);

    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 14.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    internal class XamlMember : global::Windows.UI.Xaml.Markup.IXamlMember
    {
        global::CrossConnect.CrossConnect10_XamlTypeInfo.XamlTypeInfoProvider _provider;
        string _name;
        bool _isAttachable;
        bool _isDependencyProperty;
        bool _isReadOnly;

        string _typeName;
        string _targetTypeName;

        public XamlMember(global::CrossConnect.CrossConnect10_XamlTypeInfo.XamlTypeInfoProvider provider, string name, string typeName)
        {
            _name = name;
            _typeName = typeName;
            _provider = provider;
        }

        public string Name { get { return _name; } }

        public global::Windows.UI.Xaml.Markup.IXamlType Type
        {
            get { return _provider.GetXamlTypeByName(_typeName); }
        }

        public void SetTargetTypeName(string targetTypeName)
        {
            _targetTypeName = targetTypeName;
        }
        public global::Windows.UI.Xaml.Markup.IXamlType TargetType
        {
            get { return _provider.GetXamlTypeByName(_targetTypeName); }
        }

        public void SetIsAttachable() { _isAttachable = true; }
        public bool IsAttachable { get { return _isAttachable; } }

        public void SetIsDependencyProperty() { _isDependencyProperty = true; }
        public bool IsDependencyProperty { get { return _isDependencyProperty; } }

        public void SetIsReadOnly() { _isReadOnly = true; }
        public bool IsReadOnly { get { return _isReadOnly; } }

        public Getter Getter { get; set; }
        public object GetValue(object instance)
        {
            if (Getter != null)
                return Getter(instance);
            else
                throw new global::System.InvalidOperationException("GetValue");
        }

        public Setter Setter { get; set; }
        public void SetValue(object instance, object value)
        {
            if (Setter != null)
                Setter(instance, value);
            else
                throw new global::System.InvalidOperationException("SetValue");
        }
    }
}
