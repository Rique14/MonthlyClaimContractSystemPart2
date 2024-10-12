using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;

namespace MonthlyClaimContractSystemPart2
{
    public partial class MainWindow : Window
    {
        private List<Claim> pendingClaims = new List<Claim>();

        public MainWindow()
        {
            InitializeComponent();
            LoadPendingClaims();
        }

        private void SubmitClaim(object sender, RoutedEventArgs e)
        {
            // Input validation
            if (string.IsNullOrEmpty(LecturerName.Text) || string.IsNullOrEmpty(HoursWorked.Text) ||
                string.IsNullOrEmpty(HourlyRate.Text) || string.IsNullOrEmpty(UploadedFileName.Text))
            {
                MessageBox.Show("Please fill in all required fields and upload a supporting document.");
                return;
            }

            try
            {
                // Parse hours worked and hourly rate
                double hoursWorked = Convert.ToDouble(HoursWorked.Text);
                double hourlyRate = Convert.ToDouble(HourlyRate.Text);
                double totalAmount = hoursWorked * hourlyRate;

                // Create new claim
                var claim = new Claim
                {
                    LecturerName = LecturerName.Text,
                    HoursWorked = hoursWorked,
                    HourlyRate = hourlyRate,
                    TotalAmount = totalAmount,
                    Notes = AdditionalNotes.Text,
                    Status = "Pending",
                    DocumentPath = UploadedFileName.Text
                };

                pendingClaims.Add(claim);
                LoadPendingClaims();

                // Display success message and update status
                MessageBox.Show("Claim Submitted Successfully!");
                ClaimStatus.Text = "Pending";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void UploadDocument(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "PDF Files|*.pdf|Word Documents|*.docx|Excel Files|*.xlsx",
                Title = "Upload Supporting Document"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                FileInfo fileInfo = new FileInfo(openFileDialog.FileName);

                // Limit file size to 5MB
                if (fileInfo.Length > 5 * 1024 * 1024)
                {
                    MessageBox.Show("File size exceeds the 5MB limit.");
                    return;
                }

                UploadedFileName.Text = fileInfo.FullName;
                MessageBox.Show("Document Uploaded Successfully!");
            }
        }

        private void ApproveClaim(object sender, RoutedEventArgs e)
        {
            var selectedClaim = (Claim)PendingClaims.SelectedItem;
            if (selectedClaim != null)
            {
                selectedClaim.Status = "Approved"; // Updates status in real-time
                MessageBox.Show("Claim Approved!");
                LoadPendingClaims();
            }
        }

        private void RejectClaim(object sender, RoutedEventArgs e)
        {
            var selectedClaim = (Claim)PendingClaims.SelectedItem;
            if (selectedClaim != null)
            {
                selectedClaim.Status = "Rejected"; // Updates status in real-time
                MessageBox.Show("Claim Rejected!");
                LoadPendingClaims();
            }
        }

        private void LoadPendingClaims()
        {
            PendingClaims.ItemsSource = null;
            PendingClaims.ItemsSource = pendingClaims;
        }

        private void OnClaimSelected(object sender, RoutedEventArgs e)
        {
            var selectedClaim = (Claim)PendingClaims.SelectedItem;
            if (selectedClaim != null)
            {
                LecturerNameDetails.Text = selectedClaim.LecturerName;
                HoursWorkedDetails.Text = selectedClaim.HoursWorked.ToString();
                HourlyRateDetails.Text = selectedClaim.HourlyRate.ToString();
                TotalAmountDetails.Text = selectedClaim.TotalAmount.ToString("C");
                AdditionalNotesDetails.Text = selectedClaim.Notes;
                DocumentPathDetails.Text = selectedClaim.DocumentPath;

                // Bind the status of the selected claim to the ClaimStatus TextBox
                ClaimStatus.DataContext = selectedClaim;
                ClaimStatus.SetBinding(TextBox.TextProperty, new System.Windows.Data.Binding("Status"));
            }
        }
    }

    public class Claim : INotifyPropertyChanged
    {
        private string _status;

        public string LecturerName { get; set; }
        public double HoursWorked { get; set; }
        public double HourlyRate { get; set; }
        public double TotalAmount { get; set; }
        public string Notes { get; set; }
        public string DocumentPath { get; set; }

        // Property for Claim Status with INotifyPropertyChanged implementation
        public string Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged(nameof(Status));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}