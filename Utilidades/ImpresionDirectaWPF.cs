using APIPortalKiosco.Data;
using System;
using System.Collections.Generic;
using System.Printing;
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

    public void ImprimirBoletas(List<ReportSales> boletas)
    {
        //// Obtén la cola de impresión predeterminada (asegúrate de que está configurada para tu impresora de tickets)
        //PrintQueue printQueue = LocalPrintServer.GetDefaultPrintQueue();

        //// Define el ancho fijo del ticket (en unidades de 1/96 pulgadas, DPI)
        //double ticketWidthInInches = 3.0; // Por ejemplo, 3 pulgadas de ancho
        //double ticketWidth = ticketWidthInInches * 96; // 96 DPI

        //// Crea un documento fijo y una página de contenido
        //FixedDocument fixedDocument = new FixedDocument();
        //PageContent pageContent = new PageContent();
        //FixedPage fixedPage = new FixedPage();

        //// Crear el StackPanel que contendrá las boletas
        //StackPanel stackPanel = new StackPanel();

        //// Cargar la plantilla de boleta desde los recursos
        //DataTemplate boletaTemplate = (DataTemplate)FindResource("BoletaTemplate");

        //// Crear y añadir boletas dinámicamente al StackPanel
        //foreach (var boleta in boletas)
        //{
        //    ContentPresenter presenter = new ContentPresenter();
        //    presenter.Content = boleta;
        //    presenter.ContentTemplate = boletaTemplate;
        //    stackPanel.Children.Add(presenter);
        //}

        //// Medir y arreglar el StackPanel basado en el tamaño del ticket
        //stackPanel.Measure(new Size(ticketWidth, double.PositiveInfinity));
        //stackPanel.Arrange(new Rect(new Point(0, 0), stackPanel.DesiredSize));
        //stackPanel.UpdateLayout();

        //// Ajustar el tamaño de la página basado en el contenido del StackPanel
        //double ticketHeight = stackPanel.DesiredSize.Height;
        //Size finalSize = new Size(ticketWidth, ticketHeight);
        //fixedPage.Width = ticketWidth;
        //fixedPage.Height = ticketHeight;

        //// Mide y arregla el StackPanel basado en el tamaño final
        //stackPanel.Measure(finalSize);
        //stackPanel.Arrange(new Rect(new Point(0, 0), finalSize));
        //stackPanel.UpdateLayout();

        //// Añadir el StackPanel a la página fija
        //fixedPage.Children.Add(stackPanel);

        //// Añade la página fija al contenido de la página
        //((IAddChild)pageContent).AddChild(fixedPage);
        //fixedDocument.Pages.Add(pageContent);

        //// Crea un escritor de documento XPS y escribe el documento fijo a la cola de impresión
        //XpsDocumentWriter writer = PrintQueue.CreateXpsDocumentWriter(printQueue);
        //writer.Write(fixedDocument);
    }

    



    public void ImprimirDirecto()
    {
        // Obtén la cola de impresión predeterminada (asegúrate de que está configurada para tu impresora de tickets)
        PrintQueue printQueue = LocalPrintServer.GetDefaultPrintQueue();

        // Define el ancho fijo del ticket (en unidades de 1/96 pulgadas, DPI)
        double ticketWidthInInches = 3.0; // Por ejemplo, 3 pulgadas de ancho
        double ticketWidth = ticketWidthInInches * 96; // 96 DPI

        // Crea un documento fijo y una página de contenido
        FixedDocument fixedDocument = new FixedDocument();
        PageContent pageContent = new PageContent();
        FixedPage fixedPage = new FixedPage();

        // Establece el ancho fijo y una altura inicial grande pero finita para medir el contenido
        fixedPage.Width = ticketWidth;
        fixedPage.Height = 10000; // Altura grande pero finita para la medida inicial

        // Mide y arregla el control 'vista' basado en el ancho fijo y una altura grande inicial
        Size initialSize = new Size(ticketWidth, 10000);
        vista.Measure(initialSize);
        vista.Arrange(new Rect(new Point(0, 0), initialSize));
        vista.UpdateLayout();

        // Obtén la altura real necesaria después de la disposición
        double ticketHeight = vista.DesiredSize.Height;

        // Ajusta el tamaño de la página basado en el contenido
        Size finalSize = new Size(ticketWidth, ticketHeight);
        fixedPage.Width = ticketWidth;
        fixedPage.Height = ticketHeight;

        // Mide y arregla el control 'vista' basado en el tamaño final
        vista.Measure(finalSize);
        vista.Arrange(new Rect(new Point(0, 0), finalSize));
        vista.UpdateLayout();

        // Asegúrate de que el control 'vista' tiene contenido antes de añadirlo a la página fija
        if (vista != null)
        {
            fixedPage.Children.Add(vista);
        }

       // Añade la página fija al contenido de la página
       ((IAddChild)pageContent).AddChild(fixedPage);
        fixedDocument.Pages.Add(pageContent);

        // Crea un escritor de documento XPS y escribe el documento fijo a la cola de impresión
        XpsDocumentWriter writer = PrintQueue.CreateXpsDocumentWriter(printQueue);
        writer.Write(fixedDocument);
    }





}