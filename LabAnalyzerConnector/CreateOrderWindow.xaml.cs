using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using LabAnalyzerConnector.Core.Models;
using LabAnalyzerConnector.Core.Services;

namespace LabAnalyzerConnector;

public partial class CreateOrderWindow : Window
{
    private readonly OrderService _orderService;

    private readonly IAnalyzerManager _analyzerManager;


    // =========================================================
    // CONSTRUCTOR
    // =========================================================

    public CreateOrderWindow(
        OrderService orderService,
        IAnalyzerManager analyzerManager)
    {
        InitializeComponent();

        _orderService =
            orderService
            ?? throw new ArgumentNullException(
                nameof(orderService));

        _analyzerManager =
            analyzerManager
            ?? throw new ArgumentNullException(
                nameof(analyzerManager));


        GenerateOrderId();

        LoadAnalyzers();
    }


    // =========================================================
    // LOAD ANALYZERS
    // =========================================================

    private void LoadAnalyzers()
    {
        try
        {
            var analyzers =
                _analyzerManager
                    .GetAnalyzers()
                    .ToList();


            AnalyzerComboBox.ItemsSource =
                analyzers;


            if (analyzers.Count > 0)
            {
                AnalyzerComboBox.SelectedIndex = 0;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Unable to load analyzers.\n\n{ex.Message}",
                "Analyzer Loading Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }


    // =========================================================
    // GENERATE ORDER ID
    // =========================================================

    private void GenerateOrderId()
    {
        // Order ID is generated internally.
        //
        // It is not shown in the UI.
        //
        // We generate it when saving the order.

    }


    // =========================================================
    // ADD TEST
    // =========================================================

    private void AddTestButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        string testCode =
            TestCodeTextBox.Text.Trim();


        if (string.IsNullOrWhiteSpace(
                testCode))
        {
            MessageBox.Show(
                "Please enter a test code.",
                "Validation",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            TestCodeTextBox.Focus();

            return;
        }


        // Prevent duplicate test codes.

        bool alreadyExists =
            SelectedTestsListBox.Items
                .Cast<string>()
                .Any(
                    existingTest =>
                        string.Equals(
                            existingTest,
                            testCode,
                            StringComparison.OrdinalIgnoreCase));


        if (alreadyExists)
        {
            MessageBox.Show(
                "This test code has already been added.",
                "Duplicate Test",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            TestCodeTextBox.SelectAll();

            TestCodeTextBox.Focus();

            return;
        }


        SelectedTestsListBox.Items.Add(
            testCode);


        TestCodeTextBox.Clear();

        TestCodeTextBox.Focus();
    }


    // =========================================================
    // REMOVE SELECTED TEST
    // =========================================================

    private void RemoveSelectedTestButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (SelectedTestsListBox.SelectedItem
            is not string selectedTest)
        {
            MessageBox.Show(
                "Please select a test to remove.",
                "Remove Test",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            return;
        }


        SelectedTestsListBox.Items.Remove(
            selectedTest);
    }


    // =========================================================
    // CREATE / SAVE ORDER
    // =========================================================

    private void CreateOrderButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        try
        {
            // =================================================
            // SELECTED ANALYZER
            // =================================================

            if (AnalyzerComboBox.SelectedItem
                is null)
            {
                MessageBox.Show(
                    "Please select an analyzer.",
                    "Validation",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                AnalyzerComboBox.Focus();

                return;
            }


            // =================================================
            // PATIENT ID
            // =================================================

            string patientId =
                PatientIdTextBox.Text.Trim();


            if (string.IsNullOrWhiteSpace(
                    patientId))
            {
                MessageBox.Show(
                    "Please enter a Patient ID.",
                    "Validation",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                PatientIdTextBox.Focus();

                return;
            }


            // =================================================
            // PATIENT NAME
            // =================================================

            string patientName =
                PatientNameTextBox.Text.Trim();


            if (string.IsNullOrWhiteSpace(
                    patientName))
            {
                MessageBox.Show(
                    "Please enter the patient name.",
                    "Validation",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                PatientNameTextBox.Focus();

                return;
            }


            // =================================================
            // SPECIMEN ID
            // =================================================

            string specimenId =
                SpecimenIdTextBox.Text.Trim();


            if (string.IsNullOrWhiteSpace(
                    specimenId))
            {
                MessageBox.Show(
                    "Please enter a Specimen ID.",
                    "Validation",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                SpecimenIdTextBox.Focus();

                return;
            }


            // =================================================
            // BARCODE
            // =================================================

            string barcode =
                BarcodeTextBox.Text.Trim();


            if (string.IsNullOrWhiteSpace(
                    barcode))
            {
                MessageBox.Show(
                    "Please enter a barcode.",
                    "Validation",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                BarcodeTextBox.Focus();

                return;
            }


            // =================================================
            // SELECTED TESTS
            // =================================================

            List<string> orderedTests =
                SelectedTestsListBox.Items
                    .Cast<string>()
                    .ToList();


            if (orderedTests.Count == 0)
            {
                MessageBox.Show(
                    "Please add at least one test code.",
                    "Validation",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                TestCodeTextBox.Focus();

                return;
            }


            // =================================================
            // GET ANALYZER ID
            // =================================================

            var selectedAnalyzer =
                AnalyzerComboBox.SelectedItem;


            Guid analyzerId =
                GetAnalyzerId(
                    selectedAnalyzer);


            // =================================================
            // GENERATE ORDER ID
            // =================================================

            string orderId =
                $"ORD-{DateTime.Now:yyyyMMddHHmmssfff}";


            // =================================================
            // GET PRIORITY
            // =================================================

            string priority =
                (PriorityComboBox.SelectedItem
                    as ComboBoxItem)
                    ?.Content
                    ?.ToString()
                ?? "Routine";


            // =================================================
            // GET STATUS
            // =================================================

            string status =
                (StatusComboBox.SelectedItem
                    as ComboBoxItem)
                    ?.Content
                    ?.ToString()
                ?? "Pending";


            // =================================================
            // CREATE LAB ORDER
            // =================================================

            LabOrder order =
                new LabOrder
                {
                    Id =
                        Guid.NewGuid(),

                    AnalyzerId =
                        analyzerId,

                    OrderId =
                        orderId,

                    PatientId =
                        patientId,

                    PatientName =
                        patientName,

                    SpecimenId =
                        specimenId,

                    Barcode =
                        barcode,

                    OrderedTests =
                        orderedTests,

                    Priority =
                        priority,

                    CreatedAt =
                        DateTime.UtcNow,

                    Status =
                        status
                };


            // =================================================
            // SAVE TO SQLITE
            // =================================================

            _orderService.AddOrder(
                order);


            // =================================================
            // SUCCESS
            // =================================================

            MessageBox.Show(
                "Laboratory order created successfully.",
                "Order Created",
                MessageBoxButton.OK,
                MessageBoxImage.Information);


            DialogResult =
                true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Unable to create laboratory order.\n\n{ex.Message}",
                "Create Order Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }


    // =========================================================
    // GET ANALYZER ID
    // =========================================================

    private static Guid GetAnalyzerId(
     object analyzer)
    {
        var property =
            analyzer
                .GetType()
                .GetProperty("AnalyzerId");

        if (property is null)
        {
            throw new InvalidOperationException(
                "The selected analyzer does not contain an AnalyzerId property.");
        }

        object? value =
            property.GetValue(analyzer);

        if (value is Guid guid)
        {
            return guid;
        }

        throw new InvalidOperationException(
            "The selected analyzer has an invalid AnalyzerId.");
    }


    // =========================================================
    // CANCEL
    // =========================================================

    private void CancelButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        DialogResult =
            false;
    }
}