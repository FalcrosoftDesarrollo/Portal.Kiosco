﻿#pragma checksum "..\..\..\..\..\Properties\Views\Cartelera.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "AE171D0D75F383E9427B2907E6385AD928C99427"
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
    /// Cartelera
    /// </summary>
    public partial class Cartelera : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 125 "..\..\..\..\..\Properties\Views\Cartelera.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnVolver;
        
        #line default
        #line hidden
        
        
        #line 126 "..\..\..\..\..\Properties\Views\Cartelera.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnSiguiente;
        
        #line default
        #line hidden
        
        
        #line 140 "..\..\..\..\..\Properties\Views\Cartelera.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.WrapPanel ContenedorPeliculas;
        
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
            System.Uri resourceLocater = new System.Uri("/Portal.Kiosco;component/properties/views/cartelera.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\Properties\Views\Cartelera.xaml"
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
            
            #line 87 "..\..\..\..\..\Properties\Views\Cartelera.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.btnSalir_Click);
            
            #line default
            #line hidden
            return;
            case 2:
            
            #line 102 "..\..\..\..\..\Properties\Views\Cartelera.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.btnSoyCineFans_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.btnVolver = ((System.Windows.Controls.Button)(target));
            
            #line 125 "..\..\..\..\..\Properties\Views\Cartelera.xaml"
            this.btnVolver.Click += new System.Windows.RoutedEventHandler(this.btnVolver_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.btnSiguiente = ((System.Windows.Controls.Button)(target));
            
            #line 126 "..\..\..\..\..\Properties\Views\Cartelera.xaml"
            this.btnSiguiente.Click += new System.Windows.RoutedEventHandler(this.btnSiguiente_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            
            #line 135 "..\..\..\..\..\Properties\Views\Cartelera.xaml"
            ((System.Windows.Controls.ScrollViewer)(target)).PreviewMouseDown += new System.Windows.Input.MouseButtonEventHandler(this.ScrollViewer_PreviewMouseDown);
            
            #line default
            #line hidden
            
            #line 136 "..\..\..\..\..\Properties\Views\Cartelera.xaml"
            ((System.Windows.Controls.ScrollViewer)(target)).PreviewMouseUp += new System.Windows.Input.MouseButtonEventHandler(this.ScrollViewer_PreviewMouseUp);
            
            #line default
            #line hidden
            
            #line 137 "..\..\..\..\..\Properties\Views\Cartelera.xaml"
            ((System.Windows.Controls.ScrollViewer)(target)).PreviewTouchDown += new System.EventHandler<System.Windows.Input.TouchEventArgs>(this.ScrollViewer_PreviewTouchDown);
            
            #line default
            #line hidden
            
            #line 138 "..\..\..\..\..\Properties\Views\Cartelera.xaml"
            ((System.Windows.Controls.ScrollViewer)(target)).PreviewTouchUp += new System.EventHandler<System.Windows.Input.TouchEventArgs>(this.ScrollViewer_PreviewTouchUp);
            
            #line default
            #line hidden
            
            #line 139 "..\..\..\..\..\Properties\Views\Cartelera.xaml"
            ((System.Windows.Controls.ScrollViewer)(target)).PreviewMouseMove += new System.Windows.Input.MouseEventHandler(this.ScrollViewer_PreviewMouseMove);
            
            #line default
            #line hidden
            return;
            case 6:
            this.ContenedorPeliculas = ((System.Windows.Controls.WrapPanel)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

