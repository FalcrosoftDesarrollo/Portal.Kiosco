// Updated by XamlIntelliSenseFileGenerator 02/05/2024 17:42:02
#pragma checksum "..\..\..\..\..\Properties\Views\Gafas3D.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "295A25D3CF69C3D21F1830F1AA155AFB889DA7E3"
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
    /// Gafas3D
    /// </summary>
    public partial class Gafas3D : System.Windows.Window, System.Windows.Markup.IComponentConnector
    {

#line default
#line hidden


#line 143 "..\..\..\..\..\Properties\Views\Gafas3D.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblPrecio;

#line default
#line hidden


#line 159 "..\..\..\..\..\Properties\Views\Gafas3D.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblCantidad;

#line default
#line hidden


#line 176 "..\..\..\..\..\Properties\Views\Gafas3D.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblTotal;

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
            System.Uri resourceLocater = new System.Uri("/Portal.Kiosco;component/properties/views/gafas3d.xaml", System.UriKind.Relative);

#line 1 "..\..\..\..\..\Properties\Views\Gafas3D.xaml"
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
                    this.lbltemporizador = ((System.Windows.Controls.Label)(target));
                    return;
                case 2:
                    this.lblPrecio = ((System.Windows.Controls.Label)(target));
                    return;
                case 3:

#line 148 "..\..\..\..\..\Properties\Views\Gafas3D.xaml"
                    ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.btnSumar_Click);

#line default
#line hidden
                    return;
                case 4:
                    this.lblCantidad = ((System.Windows.Controls.Label)(target));
                    return;
                case 5:

#line 162 "..\..\..\..\..\Properties\Views\Gafas3D.xaml"
                    ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.btnSumar_Click);

#line default
#line hidden
                    return;
                case 6:
                    this.lblTotal = ((System.Windows.Controls.Label)(target));
                    return;
                case 7:

#line 182 "..\..\..\..\..\Properties\Views\Gafas3D.xaml"
                    ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.btnSiguiente_Click);

#line default
#line hidden
                    return;
                case 8:

#line 197 "..\..\..\..\..\Properties\Views\Gafas3D.xaml"
                    ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.btnVolver_Click);

#line default
#line hidden
                    return;
                case 9:

#line 201 "..\..\..\..\..\Properties\Views\Gafas3D.xaml"
                    ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.btnSiguiente_Click);

#line default
#line hidden
                    return;
            }
            this._contentLoaded = true;
        }

        internal System.Windows.Shapes.Path circlePath;
    }
}

