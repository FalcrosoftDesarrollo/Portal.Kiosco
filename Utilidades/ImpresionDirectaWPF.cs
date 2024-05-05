﻿using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Xps;

public class ImpresionDirectaWPF
{
    private UIElement vista;

    public ImpresionDirectaWPF(UIElement vista)
    {
        this.vista = vista;
    }

    public void ImprimirDirecto()
    {
        PrintQueue printQueue = LocalPrintServer.GetDefaultPrintQueue();

        FixedDocument fixedDocument = new FixedDocument();
        PageContent pageContent = new PageContent();
        FixedPage fixedPage = new FixedPage();

        // Ajustar el tamaño de la página al tamaño de la impresora
        PrintCapabilities capabilities = printQueue.GetPrintCapabilities();
        double pageWidth = capabilities.PageImageableArea.ExtentWidth;
        double pageHeight = capabilities.PageImageableArea.ExtentHeight;

        Size pageSize = new Size(pageWidth, pageHeight);
        vista.Measure(pageSize);
        vista.Arrange(new Rect(new Point(0, 0), pageSize));
        vista.UpdateLayout();

        // Agregar la vista al documento
        fixedPage.Children.Add(vista);
        ((IAddChild)pageContent).AddChild(fixedPage);
        fixedDocument.Pages.Add(pageContent);

        // Imprimir directamente en la cola de impresión predeterminada
        XpsDocumentWriter writer = PrintQueue.CreateXpsDocumentWriter(printQueue);
        writer.Write(fixedDocument);
    }
}
