﻿#pragma checksum "..\..\..\..\..\Properties\Views\BoletasGafasAlimentos.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "89E8DF9F8599011CF1AB52205C5232EE58B3DA94"
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
    /// BoletasGafasAlimentos
    /// </summary>
    public partial class BoletasGafasAlimentos : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 82 "..\..\..\..\..\Properties\Views\BoletasGafasAlimentos.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Path circlePath;
        
        #line default
        #line hidden
        
        
        #line 101 "..\..\..\..\..\Properties\Views\BoletasGafasAlimentos.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblnombre;
        
        #line default
        #line hidden
        
        
        #line 107 "..\..\..\..\..\Properties\Views\BoletasGafasAlimentos.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnSalir;
        
        #line default
        #line hidden
        
        
        #line 183 "..\..\..\..\..\Properties\Views\BoletasGafasAlimentos.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnEnviar;
        
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
            System.Uri resourceLocater = new System.Uri("/Portal.Kiosco;component/properties/views/boletasgafasalimentos.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\Properties\Views\BoletasGafasAlimentos.xaml"
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
            this.circlePath = ((System.Windows.Shapes.Path)(target));
            return;
            case 2:
            this.lblnombre = ((System.Windows.Controls.Label)(target));
            return;
            case 3:
            this.btnSalir = ((System.Windows.Controls.Button)(target));
            
            #line 107 "..\..\..\..\..\Properties\Views\BoletasGafasAlimentos.xaml"
            this.btnSalir.Click += new System.Windows.RoutedEventHandler(this.btnSalir_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.btnEnviar = ((System.Windows.Controls.Button)(target));
            
            #line 183 "..\..\..\..\..\Properties\Views\BoletasGafasAlimentos.xaml"
            this.btnEnviar.Click += new System.Windows.RoutedEventHandler(this.btnEnviar_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

