﻿#pragma checksum "..\..\..\..\..\Properties\Views\Datos_Membresía_CineFans.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "4EEF034440CFA78BF0FA7397CE007D4896E6B77D"
//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

using Portal.Kiosco;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
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
    /// Datos_Membresía_CineFans
    /// </summary>
    public partial class Datos_Membresía_CineFans : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 115 "..\..\..\..\..\Properties\Views\Datos_Membresía_CineFans.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblNombre;
        
        #line default
        #line hidden
        
        
        #line 132 "..\..\..\..\..\Properties\Views\Datos_Membresía_CineFans.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblNivel;
        
        #line default
        #line hidden
        
        
        #line 145 "..\..\..\..\..\Properties\Views\Datos_Membresía_CineFans.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblPuntosSiguenteLvl;
        
        #line default
        #line hidden
        
        
        #line 173 "..\..\..\..\..\Properties\Views\Datos_Membresía_CineFans.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblCash;
        
        #line default
        #line hidden
        
        
        #line 201 "..\..\..\..\..\Properties\Views\Datos_Membresía_CineFans.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblVisitaAcumulada;
        
        #line default
        #line hidden
        
        
        #line 244 "..\..\..\..\..\Properties\Views\Datos_Membresía_CineFans.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnSalir;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "7.0.4.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Portal.Kiosco;component/properties/views/datos_membres%c3%ada_cinefans.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\Properties\Views\Datos_Membresía_CineFans.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "7.0.4.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.lblNombre = ((System.Windows.Controls.Label)(target));
            return;
            case 2:
            this.lblNivel = ((System.Windows.Controls.Label)(target));
            return;
            case 3:
            this.lblPuntosSiguenteLvl = ((System.Windows.Controls.Label)(target));
            return;
            case 4:
            this.lblCash = ((System.Windows.Controls.Label)(target));
            return;
            case 5:
            this.lblVisitaAcumulada = ((System.Windows.Controls.Label)(target));
            return;
            case 6:
            
            #line 236 "..\..\..\..\..\Properties\Views\Datos_Membresía_CineFans.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Button_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            this.btnSalir = ((System.Windows.Controls.Button)(target));
            
            #line 247 "..\..\..\..\..\Properties\Views\Datos_Membresía_CineFans.xaml"
            this.btnSalir.Click += new System.Windows.RoutedEventHandler(this.btnSalir_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

