using Gdk;
using Gtk;

class Program
{
    private const string API_KEY = "039dfe7eb9ae61ecad4977d34955aead";
    private const string LOCATION = "Lagos, NG";
    private const string ICON_PATH = "..\\..\\..\\Images\\weather.png";

    private static Label label;
    private static StatusIcon trayIcon;
    private static DateTime lastClickTime = DateTime.MinValue;


    static void Main(string[] args)
    {
        Application.Init();

        // Create a new tray icon
        trayIcon = new StatusIcon(new Pixbuf(ICON_PATH));
        trayIcon.TooltipText = "Weather Checker";

        // Create a label to display weather information
        label = new Label();
        label.Visible = false;

        // Handle double-click event
        trayIcon.ButtonReleaseEvent += (sender, e) =>
        {
            var currentTime = DateTime.Now;
            var elapsedTime = currentTime - lastClickTime;
            lastClickTime = currentTime;

            if (elapsedTime.TotalMilliseconds < 1000) // Double-click threshold
            {
                CheckWeather();
            }
        };

        // Create a window to hold the label
        Gtk.Window window = new Gtk.Window("Weather Checker");
        window.TypeHint = WindowTypeHint.Splashscreen;
        window.Decorated = false;
        window.SkipPagerHint = true;
        window.SkipTaskbarHint = true;
        window.Add(label);

        window.ShowAll();

        Application.Run();
    }

    private static async void CheckWeather()
    {
        try
        {
            using (HttpClient client = new HttpClient())
            {
                string url = $"http://api.openweathermap.org/data/2.5/weather?q={LOCATION}&appid={API_KEY}";
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                // Parse weather data (for simplicity, just check if it's raining)
                if (responseBody.Contains("\"main\":\"Rain\""))
                {
                    label.Text = "It will rain today.";
                }
                else
                {
                    label.Text = "It will not rain today.";
                }

                label.Visible = true;
            }
        }
        catch (Exception ex)
        {
            label.Text = $"Error: {ex.Message}";
            label.Visible = true;
        }
    }
}
