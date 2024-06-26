using Microsoft.Maui.Platform;
using Sozluk.Database;
using Sozluk.Helpers;
using Sozluk.Models;
using System.Net.NetworkInformation;
using GemBox.Pdf;
using GemBox.Pdf.Content;


namespace Sozluk.Pages;

public partial class StatsPage : ContentPage
{
    private readonly QuizHelper _quizHelper;
    

    public StatsPage()
    {
        InitializeComponent();
        var databaseService = new LocalDatabaseService();
        _quizHelper = new QuizHelper(databaseService.GetConnection());
        ComponentInfo.SetLicense("FREE-LIMITED-KEY");
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadStatsAsync();
    }

    private async Task LoadStatsAsync()
    {
        var totalStats = await _quizHelper.GetTotalStatsAsync(); // TotalStats nesnesini al
        totalLevel1CountLabel.Text = totalStats.Level1Count.ToString();
        totalLevel2CountLabel.Text = totalStats.Level2Count.ToString();
        totalLevel3CountLabel.Text = totalStats.Level3Count.ToString();
        totalLevel4CountLabel.Text = totalStats.Level4Count.ToString();
        totalLevel5CountLabel.Text = totalStats.Level5Count.ToString();
        totalLevel6CountLabel.Text = totalStats.Level6Count.ToString();
        totalLevel7CountLabel.Text = totalStats.Level7Count.ToString();

        var dailyStats = await _quizHelper.GetDailyStatsAsync(); // DailyStats listesini al
        dailyStatsListView.ItemsSource = dailyStats;

       
    }
    private async Task<string> CreateDocumentAsync()
    {
        var totalStats = await _quizHelper.GetTotalStatsAsync();
        var dailyStats = await _quizHelper.GetDailyStatsAsync();
        using var document = new PdfDocument();

        // Toplam istatistikler sayfası
        var totalPage = document.Pages.Add();
        using (var formattedText = new PdfFormattedText())
        {
            formattedText.FontFamily = new PdfFontFamily("Helvatica");
            formattedText.FontSize = 24;

            formattedText.AppendLine("**Toplam istatistikler**");
            formattedText.FontSize = 12;
            formattedText.AppendLine($"Seviye 1: {totalStats.Level1Count} Seviye 2: {totalStats.Level2Count} Seviye 3: {totalStats.Level3Count} Seviye 4: {totalStats.Level4Count} Seviye 5: {totalStats.Level5Count} Seviye 6: {totalStats.Level6Count} Seviye 7: {totalStats.Level7Count}");
            

            totalPage.Content.DrawText(formattedText, new PdfPoint(100, 400));
        }

        // Günlük istatistikler sayfası
        var dailyPage = document.Pages.Add();
        using (var formattedText = new PdfFormattedText())
        {
            formattedText.FontFamily = new PdfFontFamily("Helvatica");
            formattedText.FontSize = 24;

            formattedText.AppendLine("**Günlük istatistikler**");
            foreach (var dailyStat in dailyStats)
            {
                formattedText.FontSize = 12;
                formattedText.AppendLine($"Tarih: {dailyStat.Date.ToString("dd/MM/yyyy")}");
                formattedText.AppendLine($"Seviye 1: {dailyStat.Level1Count} Seviye 2: {dailyStat.Level2Count} Seviye 3: {dailyStat.Level3Count} Seviye 4: {dailyStat.Level4Count} Seviye 5: {dailyStat.Level5Count} Seviye 6: {dailyStat.Level6Count} Seviye 7: {dailyStat.Level7Count}");
            }

            dailyPage.Content.DrawText(formattedText, new PdfPoint(100, 100));
        }

        var filePath = Path.Combine(FileSystem.Current.AppDataDirectory, "istatistikler.pdf");
        await Task.Run(() => document.Save(filePath));

        return filePath;
    }

    private async void DownloadButtonClicked(object sender, EventArgs e)
    {
        

        button.IsEnabled = false;
        activity.IsRunning = true;

        try
        {
            var filePath = await CreateDocumentAsync();
            await Launcher.OpenAsync(new OpenFileRequest(Path.GetFileName(filePath), new ReadOnlyFile(filePath)));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "Close");
        }

        activity.IsRunning = false;
        button.IsEnabled = true;
    }

    


}