﻿#pragma checksum "..\..\..\..\..\Properties\Views\Scanear_documento.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "7AB6F2418031A212BAF9CCFAD9091643D670A0F6"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace Portal.Kiosco {
    
    
    /// <summary>
    /// Scanear_documento
    /// </summary>
    public partial class Scanear_documento : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 44 "..\..\..\..\..\Properties\Views\Scanear_documento.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnVolverComoCompra;
        
        #line default
        #line hidden
        
        
        #line 59 "..\..\..\..\..\Properties\Views\Scanear_documento.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox TextDocumento;
        
        #line default
        #line hidden
        
        
        #line 78 "..\..\..\..\..\Properties\Views\Scanear_documento.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnIngresaDocumento;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "7.0.9.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Portal.Kiosco;V1.0.0.0;component/properties/views/scanear_documento.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\Properties\Views\Scanear_documento.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "7.0.9.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.btnVolverComoCompra = ((System.Windows.Controls.Button)(target));
            
            #line 44 "..\..\..\..\..\Properties\Views\Scanear_documento.xaml"
            this.btnVolverComoCompra.Click += new System.Windows.RoutedEventHandler(this.btnVolverComoCompra_Click);
            
            #line default
            #line hidden
            return;
            case 2:
            this.TextDocumento = ((System.Windows.Controls.TextBox)(target));
            return;
            case 3:
            this.btnIngresaDocumento = ((System.Windows.Controls.Button)(target));
            
            #line 78 "..\..\..\..\..\Properties\Views\Scanear_documento.xaml"
            this.btnIngresaDocumento.Click += new System.Windows.RoutedEventHandler(this.btnIngresaDocumento_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

