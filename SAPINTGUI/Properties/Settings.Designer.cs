﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.17929
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace SAPINT.Gui.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "11.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"************************************************************************
* Copyright 2013 Pactera文思海辉
* All Rights Reserved 
*----------------------------------------------------------------------*
* Program Name : {0} 
* Project : {1} 
* Program Title:{2} 
* Created by : {3} 
* Created on : {4}
* Version : 1.0 
* Function Description: 
* {5}
*----------------------------------------------------------------------*
""")]
        public string HeaderDesc {
            get {
                return ((string)(this["HeaderDesc"]));
            }
            set {
                this["HeaderDesc"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Vincent Wang,mail:vincent.wang@pactera.com")]
        public string Author {
            get {
                return ((string)(this["Author"]));
            }
            set {
                this["Author"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("ZVI_PROGRAM_001")]
        public string Program_name {
            get {
                return ((string)(this["Program_name"]));
            }
            set {
                this["Program_name"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Pactera & SAP Project")]
        public string Project_name {
            get {
                return ((string)(this["Project_name"]));
            }
            set {
                this["Project_name"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("ALV 报表")]
        public string Title_text {
            get {
                return ((string)(this["Title_text"]));
            }
            set {
                this["Title_text"] = value;
            }
        }
    }
}
