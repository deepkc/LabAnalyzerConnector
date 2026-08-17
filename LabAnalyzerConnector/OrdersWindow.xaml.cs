using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using LabAnalyzerConnector.Application.Transmission;
using LabAnalyzerConnector.Core.Models;
using LabAnalyzerConnector.Core.Services;

namespace LabAnalyzerConnector;

public partial class OrdersWindow : Window
{
    private readonly OrderService
        _orderService;

    private readonly ObservableCollection<OrderListItem>
        _orders =
            new();


    private List<OrderListItem>
        _allOrders =
            new();

    private readonly OrderTransmissionService
    _orderTransmissionService;

    private readonly AnalyzerConfigurationService
    _analyzerConfigurationService;


    public OrdersWindow(
    OrderService orderService,
    OrderTransmissionService orderTransmissionService,
    AnalyzerConfigurationService analyzerConfigurationService)
    {
        InitializeComponent();

        _orderService =
            orderService
            ?? throw new ArgumentNullException(
                nameof(orderService));

        OrdersDataGrid.ItemsSource =
            _orders;

     

        _orderTransmissionService =
    orderTransmissionService
    ?? throw new ArgumentNullException(
        nameof(orderTransmissionService));

        LoadOrders();
    }


    // =========================================================
    // LOAD ORDERS
    // =========================================================

    private void LoadOrders()
    {
        try
        {
            IReadOnlyCollection<LabOrder>
                orders =
                    _orderService.GetAllOrders();


            _allOrders =
                orders
                    .Select(
                        order =>
                            new OrderListItem(
                                order))
                    .ToList();


            ApplyFilter();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Unable to load laboratory orders.\n\n{ex.Message}",
                "Order Loading Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }


    // =========================================================
    // SEARCH
    // =========================================================

    private void SearchTextBox_TextChanged(
        object sender,
        TextChangedEventArgs e)
    {
        ApplyFilter();
    }


    // =========================================================
    // APPLY SEARCH FILTER
    // =========================================================

    private void ApplyFilter()
    {
        string searchText =
            SearchTextBox?.Text
                ?.Trim()
                ?? string.Empty;


        IEnumerable<OrderListItem>
            filteredOrders =
                _allOrders;


        if (!string.IsNullOrWhiteSpace(
                searchText))
        {
            filteredOrders =
                _allOrders
                    .Where(
                        order =>
                            Contains(
                                order.OrderId,
                                searchText)

                            ||

                            Contains(
                                order.Barcode,
                                searchText)

                            ||

                            Contains(
                                order.PatientId,
                                searchText)

                            ||

                            Contains(
                                order.PatientName,
                                searchText)

                            ||

                            Contains(
                                order.SpecimenId,
                                searchText)

                            ||

                            Contains(
                                order.Status,
                                searchText)

                            ||

                            Contains(
                                order.Priority,
                                searchText)

                            ||

                            Contains(
                                order.OrderedTestsDisplay,
                                searchText));
        }


        _orders.Clear();


        foreach (
            OrderListItem order
            in filteredOrders)
        {
            _orders.Add(
                order);
        }


        OrderCountTextBlock.Text =
            $"{_orders.Count} order(s) displayed";
    }


    // =========================================================
    // STRING SEARCH HELPER
    // =========================================================

    private static bool Contains(
        string? source,
        string searchText)
    {
        return
            !string.IsNullOrWhiteSpace(
                source)

            &&

            source.Contains(
                searchText,
                StringComparison.OrdinalIgnoreCase);
    }


    // =========================================================
    // REFRESH
    // =========================================================

    private void RefreshButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        LoadOrders();
    }


    // =========================================================
    // CLOSE
    // =========================================================

    private void CloseButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        Close();
    }


    // =========================================================
    // ORDER LIST DISPLAY MODEL
    // =========================================================

    private sealed class OrderListItem
    {

        public LabOrder Order
        {
            get;
        }
        public string OrderId
        {
            get;
        }

        public string Barcode
        {
            get;
        }

        public string PatientId
        {
            get;
        }

        public string PatientName
        {
            get;
        }

        public string SpecimenId
        {
            get;
        }

        public string OrderedTestsDisplay
        {
            get;
        }

        public string Priority
        {
            get;
        }

        public string Status
        {
            get;
        }

        public string CreatedAtDisplay
        {
            get;
        }

       
        public OrderListItem(
            LabOrder order)
        {

            Order = order;

            OrderId =
                order.OrderId;

            Barcode =
                order.Barcode;

            PatientId =
                order.PatientId;

            PatientName =
                order.PatientName;

            SpecimenId =
                order.SpecimenId;

            OrderedTestsDisplay =
                string.Join(
                    ", ",
                    order.OrderedTests);

            Priority =
                order.Priority;

            Status =
                order.Status;

            CreatedAtDisplay =
                order.CreatedAt
                    .ToLocalTime()
                    .ToString(
                        "yyyy-MM-dd HH:mm");
        }
    }

    private async void SendOrderButton_Click(
    object sender,
    RoutedEventArgs e)
    {
        if (OrdersDataGrid.SelectedItem is not OrderListItem selectedOrder)
        {
            MessageBox.Show(
                "Please select an order first.");

            return;
        }

        LabOrder? order =
            _orderService
                .GetAllOrders()
                .FirstOrDefault(
                    x => x.OrderId == selectedOrder.OrderId);

        if (order is null)
        {
            MessageBox.Show(
                "Unable to locate the selected order.");

            return;
        }

        if (order.AnalyzerId is null)
        {
            MessageBox.Show(
                "This order is not assigned to an analyzer.");

            return;
        }

        try
        {
            await _orderTransmissionService.SendOrderAsync(
                order.AnalyzerId.Value,
                order);

            MessageBox.Show(
                "Order sent successfully.");
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                ex.Message,
                "Send Failed");
        }
    }


}