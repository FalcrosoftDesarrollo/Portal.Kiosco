// Updated by XamlIntelliSenseFileGenerator 09/04/2024 17:20:38
#pragma checksum "..\..\..\..\..\Properties\Views\InstruccionesDatafono.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "FABB7F89BB8E74C646ED702E0A9CE5AFADD4828D"
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


namespace Portal.Kiosco.Properties.Views
{


    /// <summary>
    /// InstruccionesDatafono
    /// </summary>
    public partial class InstruccionesDatafono : System.Windows.Window, System.Windows.Markup.IComponentConnector
    {


#line 128 "..\..\..\..\..\Properties\Views\InstruccionesDatafono.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnVolver;

#line default
#line hidden


#line 129 "..\..\..\..\..\Properties\Views\InstruccionesDatafono.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnSiguiente;

#line default
#line hidden

        private bool _contentLoaded;

        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "8.0.1.0")]
        public void InitializeComponent()
        {
            if (_contentLoaded)
            {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Portal.Kiosco;component/properties/views/instruccionesdatafono.xaml", System.UriKind.Relative);

#line 1 "..\..\..\..\..\Properties\Views\InstruccionesDatafono.xaml"
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
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target)
        {
            switch (connectionId)
            {
                case 1:
                    this.btnVolver = ((System.Windows.Controls.Button)(target));

#line 128 "..\..\..\..\..\Properties\Views\InstruccionesDatafono.xaml"
                    this.btnVolver.Click += new System.Windows.RoutedEventHandler(this.btnVolver_Click);

#line default
#line hidden
                    return;
                case 2:
                    this.btnSiguiente = ((System.Windows.Controls.Button)(target));

#line 129 "..\..\..\..\..\Properties\Views\InstruccionesDatafono.xaml"
                    this.btnSiguiente.Click += new System.Windows.RoutedEventHandler(this.btnSiguiente_Click);

#line default
#line hidden
                    return;
            }
            this._contentLoaded = true;
        }

        internal System.Windows.Controls.Button btnSalir;
    }
}

