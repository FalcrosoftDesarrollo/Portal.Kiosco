﻿#pragma checksum "..\..\..\..\..\Properties\Views\AlgoParaComer.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "5ECC240E39D4E579A8861D2565C4071D7AB7619B"
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
    /// AlgoParaComer
    /// </summary>
    public partial class AlgoParaComer : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 81 "..\..\..\..\..\Properties\Views\AlgoParaComer.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lbltemporizador;
        
        #line default
        #line hidden
        
        
        #line 84 "..\..\..\..\..\Properties\Views\AlgoParaComer.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblnombre;
        
        #line default
        #line hidden
        
        
        #line 87 "..\..\..\..\..\Properties\Views\AlgoParaComer.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnSalir;
        
        #line default
        #line hidden
        
        
        #line 124 "..\..\..\..\..\Properties\Views\AlgoParaComer.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnCombos;
        
        #line default
        #line hidden
        
        
        #line 137 "..\..\..\..\..\Properties\Views\AlgoParaComer.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnResumen;
        
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
            System.Uri resourceLocater = new System.Uri("/Portal.Kiosco;V1.0.0.0;component/properties/views/algoparacomer.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\Properties\Views\AlgoParaComer.xaml"
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
            
            #line 87 "..\..\..\..\..\Properties\Views\AlgoParaComer.xaml"
            this.btnSalir.Click += new System.Windows.RoutedEventHandler(this.btnSalir_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.btnCombos = ((System.Windows.Controls.Button)(target));
            
            #line 124 "..\..\..\..\..\Properties\Views\AlgoParaComer.xaml"
            this.btnCombos.Click += new System.Windows.RoutedEventHandler(this.btnCombos_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.btnResumen = ((System.Windows.Controls.Button)(target));
            
            #line 137 "..\..\..\..\..\Properties\Views\AlgoParaComer.xaml"
            this.btnResumen.Click += new System.Windows.RoutedEventHandler(this.btnResumen_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

