using TibiaWiki.WebScrapper;

namespace TibiaWiki.App
{
    public partial class MainPage : ContentPage
    {
        int count = 0;
        private readonly DataLoader _dataLoader;

        public MainPage(DataLoader dataLoader)
        {
            InitializeComponent();
            _dataLoader = dataLoader;
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            count++;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);

            Task.Run(async () =>
            {
                await _dataLoader.GetData("/Machados", "");
            });
        }
    }
}