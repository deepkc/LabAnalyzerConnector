using System;
using System.Linq;
using System.Windows;
using LabAnalyzerConnector.Mapping.Abstractions;
using LabAnalyzerConnector.Mapping.Models;

namespace LabAnalyzerConnector.Views;

public partial class AnalyzerMappingWindow : Window
{
    private readonly ITestCodeMappingRepository _repository;

    private readonly Guid _analyzerId;

    private Guid? _editingMappingId;


    // =========================================================
    // CONSTRUCTOR
    // =========================================================

    public AnalyzerMappingWindow(
        Guid analyzerId,
        ITestCodeMappingRepository repository)
    {
        InitializeComponent();

        _analyzerId = analyzerId;

        _repository = repository;

        LoadMappings();
    }


    // =========================================================
    // LOAD
    // =========================================================

    private async void LoadMappings()
    {
        try
        {
            var mappings =
                await _repository.GetByAnalyzerIdAsync(
                    _analyzerId);

            MappingsGrid.ItemsSource =
                mappings.ToList();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Unable to load mappings.\n\n{ex.Message}",
                "Mapping Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }


    // =========================================================
    // SAVE / UPDATE
    // =========================================================

    private async void SaveButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        try
        {
            string analyzerTestCode =
                AnalyzerTestCodeTextBox.Text.Trim();

            string standardTestCode =
                StandardTestCodeTextBox.Text.Trim();

            string standardTestName =
                StandardTestNameTextBox.Text.Trim();


            // -------------------------------------------------
            // VALIDATION
            // -------------------------------------------------

            if (string.IsNullOrWhiteSpace(
                analyzerTestCode))
            {
                MessageBox.Show(
                    "Enter the analyzer test code.",
                    "Mapping",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }


            if (string.IsNullOrWhiteSpace(
                standardTestCode))
            {
                MessageBox.Show(
                    "Enter the LOINC code.",
                    "Mapping",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }


            // =================================================
            // UPDATE EXISTING MAPPING
            // =================================================

            if (_editingMappingId.HasValue)
            {
                var existing =
                    await _repository.GetByIdAsync(
                        _editingMappingId.Value);


                if (existing == null)
                {
                    MessageBox.Show(
                        "The selected mapping could not be found.",
                        "Mapping",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    ClearEditingState();

                    return;
                }


                existing.AnalyzerTestCode =
                    analyzerTestCode;

                existing.StandardTestCode =
                    standardTestCode;

                existing.StandardTestName =
                    standardTestName;


                await _repository.UpdateAsync(
                    existing);


                MessageBox.Show(
                    "Mapping updated successfully.",
                    "Mapping",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);


                ClearEditingState();

                LoadMappings();

                return;
            }


            // =================================================
            // CREATE NEW MAPPING
            // =================================================

            var mapping =
                new TestCodeMapping
                {
                    Id =
                        Guid.NewGuid(),

                    AnalyzerId =
                        _analyzerId,

                    AnalyzerTestCode =
                        analyzerTestCode,

                    StandardTestCode =
                        standardTestCode,

                    StandardTestName =
                        standardTestName,

                    IsActive =
                        true
                };


            await _repository.AddAsync(
                mapping);


            MessageBox.Show(
                "Mapping saved successfully.",
                "Mapping",
                MessageBoxButton.OK,
                MessageBoxImage.Information);


            ClearFields();

            LoadMappings();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Unable to save mapping.\n\n{ex.Message}",
                "Mapping Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }


    // =========================================================
    // EDIT
    // =========================================================

    private void EditButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (MappingsGrid.SelectedItem
            is not TestCodeMapping mapping)
        {
            MessageBox.Show(
                "Please select a mapping to edit.",
                "Edit Mapping",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }


        _editingMappingId =
            mapping.Id;


        AnalyzerTestCodeTextBox.Text =
            mapping.AnalyzerTestCode;

        StandardTestCodeTextBox.Text =
            mapping.StandardTestCode;

        StandardTestNameTextBox.Text =
            mapping.StandardTestName;


        SaveButton.Content =
            "Update Mapping";
    }


    // =========================================================
    // DELETE
    // =========================================================

    private async void DeleteButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (MappingsGrid.SelectedItem
            is not TestCodeMapping mapping)
        {
            MessageBox.Show(
                "Please select a mapping to delete.",
                "Delete Mapping",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }


        MessageBoxResult confirmation =
            MessageBox.Show(
                $"Delete mapping '{mapping.AnalyzerTestCode}' → '{mapping.StandardTestCode}'?",
                "Delete Mapping",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);


        if (confirmation !=
            MessageBoxResult.Yes)
        {
            return;
        }


        try
        {
            await _repository.DeleteAsync(
                mapping.Id);


            MessageBox.Show(
                "Mapping deleted successfully.",
                "Delete Mapping",
                MessageBoxButton.OK,
                MessageBoxImage.Information);


            ClearEditingState();

            LoadMappings();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Unable to delete mapping.\n\n{ex.Message}",
                "Delete Mapping Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }


    // =========================================================
    // CLEAR
    // =========================================================

    private void ClearButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        ClearEditingState();
    }


    // =========================================================
    // CLEAR FIELDS
    // =========================================================

    private void ClearFields()
    {
        AnalyzerTestCodeTextBox.Clear();

        StandardTestCodeTextBox.Clear();

        StandardTestNameTextBox.Clear();
    }


    // =========================================================
    // CLEAR EDITING STATE
    // =========================================================

    private void ClearEditingState()
    {
        _editingMappingId = null;

        ClearFields();

        SaveButton.Content =
            "Save Mapping";

        MappingsGrid.SelectedItem =
            null;
    }
}