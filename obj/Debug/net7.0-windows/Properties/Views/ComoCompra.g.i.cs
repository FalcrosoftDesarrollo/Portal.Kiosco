﻿#pragma checksum "..\..\..\..\..\Properties\Views\ComoCompra.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "AB35615629DF8EFB9FD1B96D5343041CD330B79E"
//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
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
    /// ComoCompra
    /// </summary>
    public partial class ComoCompra : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 40 "..\..\..\..\..\Properties\Views\ComoCompra.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnVolverComoCompra;
        
        #line default
        #line hidden
        
        
        #line 64 "..\..\..\..\..\Properties\Views\ComoCompra.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnCinefans;
        
        #line default
        #line hidden
        
        
        #line 82 "..\..\..\..\..\Properties\Views\ComoCompra.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnInvitado;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "8.0.1.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Portal.Kiosco;V1.0.0.0;component/properties/views/comocompra.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\Properties\Views\ComoCompra.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "8.0.1.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.btnVolverComoCompra = ((System.Windows.Controls.Button)(target));
            
            #line 40 "..\..\..\..\..\Properties\Views\ComoCompra.xaml"
            this.btnVolverComoCompra.Click += new System.Windows.RoutedEventHandler(this.btnVolverComoCompra_Click);
            
            #line default
            #line hidden
            return;
            case 2:
            this.btnCinefans = ((System.Windows.Controls.Button)(target));
            
            #line 64 "..\..\..\..\..\Properties\Views\ComoCompra.xaml"
            this.btnCinefans.Click += new System.Windows.RoutedEventHandler(this.btnCinefans_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.btnInvitado = ((System.Windows.Controls.Button)(target));
            
            #line 82 "..\..\..\..\..\Properties\Views\ComoCompra.xaml"
            this.btnInvitado.Click += new System.Windows.RoutedEventHandler(this.btnInvitado_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

