﻿#pragma checksum "..\..\..\..\..\Properties\Views\Datos_Membresía_CineFans.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "F9885FEEB0B6BD3DA661780C76768E39E4BE11FB"
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
        
        
        #line 162 "..\..\..\..\..\Properties\Views\Datos_Membresía_CineFans.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblNombre;
        
        #line default
        #line hidden
        
        
        #line 179 "..\..\..\..\..\Properties\Views\Datos_Membresía_CineFans.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblNivel;
        
        #line default
        #line hidden
        
        
        #line 192 "..\..\..\..\..\Properties\Views\Datos_Membresía_CineFans.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblPuntosSiguenteLvl;
        
        #line default
        #line hidden
        
        
        #line 220 "..\..\..\..\..\Properties\Views\Datos_Membresía_CineFans.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblCash;
        
        #line default
        #line hidden
        
        
        #line 248 "..\..\..\..\..\Properties\Views\Datos_Membresía_CineFans.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblVisitaAcumulada;
        
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
            System.Uri resourceLocater = new System.Uri("/Portal.Kiosco;component/properties/views/datos_membres%c3%ada_cinefans.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\Properties\Views\Datos_Membresía_CineFans.xaml"
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
            
            #line 89 "..\..\..\..\..\Properties\Views\Datos_Membresía_CineFans.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.btnSalir_Click);
            
            #line default
            #line hidden
            return;
            case 2:
            this.lblNombre = ((System.Windows.Controls.Label)(target));
            return;
            case 3:
            this.lblNivel = ((System.Windows.Controls.Label)(target));
            return;
            case 4:
            this.lblPuntosSiguenteLvl = ((System.Windows.Controls.Label)(target));
            return;
            case 5:
            this.lblCash = ((System.Windows.Controls.Label)(target));
            return;
            case 6:
            this.lblVisitaAcumulada = ((System.Windows.Controls.Label)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

