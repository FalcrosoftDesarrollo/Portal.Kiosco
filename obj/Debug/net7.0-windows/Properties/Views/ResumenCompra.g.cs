﻿#pragma checksum "..\..\..\..\..\Properties\Views\ResumenCompra.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "C1FFE3A2BE2B33F8ABBBB8DBA00D639576C61136"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Portal.Kiosco.Properties.Views;
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


namespace Portal.Kiosco.Properties.Views {
    
    
    /// <summary>
    /// ResumenCompra
    /// </summary>
    public partial class ResumenCompra : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 83 "..\..\..\..\..\Properties\Views\ResumenCompra.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lbltemporizador;
        
        #line default
        #line hidden
        
        
        #line 87 "..\..\..\..\..\Properties\Views\ResumenCompra.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblnombre;
        
        #line default
        #line hidden
        
        
        #line 93 "..\..\..\..\..\Properties\Views\ResumenCompra.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnSalir;
        
        #line default
        #line hidden
        
        
        #line 268 "..\..\..\..\..\Properties\Views\ResumenCompra.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnPagoTarjeta;
        
        #line default
        #line hidden
        
        
        #line 273 "..\..\..\..\..\Properties\Views\ResumenCompra.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnPagarCash;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "8.0.4.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Portal.Kiosco;component/properties/views/resumencompra.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\Properties\Views\ResumenCompra.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "8.0.4.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.lbltemporizador = ((System.Windows.Controls.Label)(target));
            return;
            case 2:
            this.lblnombre = ((System.Windows.Controls.Label)(target));
            return;
            case 3:
            this.btnSalir = ((System.Windows.Controls.Button)(target));
            
            #line 93 "..\..\..\..\..\Properties\Views\ResumenCompra.xaml"
            this.btnSalir.Click += new System.Windows.RoutedEventHandler(this.btnSalir_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.btnPagoTarjeta = ((System.Windows.Controls.Button)(target));
            
            #line 268 "..\..\..\..\..\Properties\Views\ResumenCompra.xaml"
            this.btnPagoTarjeta.Click += new System.Windows.RoutedEventHandler(this.btnPagoTarjeta_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.btnPagarCash = ((System.Windows.Controls.Button)(target));
            
            #line 273 "..\..\..\..\..\Properties\Views\ResumenCompra.xaml"
            this.btnPagarCash.Click += new System.Windows.RoutedEventHandler(this.btnPagarCash_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

